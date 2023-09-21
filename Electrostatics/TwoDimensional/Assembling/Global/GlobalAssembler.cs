using Electrostatics.Core;
using Electrostatics.Core.Boundary;
using Electrostatics.Core.Global;
using Electrostatics.Core.GridComponents;
using Electrostatics.FEM.Assembling;
using Electrostatics.FEM.Assembling.Global;
using Electrostatics.FEM.Assembling.Local;
using System;
using Electrostatics.Core.Base;
using Electrostatics.Core.Local;
using Electrostatics.TwoDimensional.Assembling.Local;

namespace Electrostatics.TwoDimensional.Assembling.Global;

public class GlobalAssembler<TNode>
{
    private readonly Grid<Node2D> _grid;
    private readonly IMatrixPortraitBuilder<SymmetricSparseMatrix> _matrixPortraitBuilder;
    private readonly ILocalAssembler _localAssembler;
    private readonly IInserter<SymmetricSparseMatrix> _inserter;
    private readonly IGaussExcluder<SymmetricSparseMatrix> _gaussExcluder;
    private readonly LocalBasisFunctionsProvider _localBasisFunctionsProvider;
    private Equation<SymmetricSparseMatrix> _equation;
    private SymmetricSparseMatrix _preconditionMatrix;
    private BaseVector _bufferVector = new(4);

    public GlobalAssembler
    (
        Grid<Node2D> grid,
        IMatrixPortraitBuilder<SymmetricSparseMatrix> matrixPortraitBuilder,
        ILocalAssembler localAssembler,
        IInserter<SymmetricSparseMatrix> inserter,
        IGaussExcluder<SymmetricSparseMatrix> gaussExcluder,
        LocalBasisFunctionsProvider localBasisFunctionsProvider
    )
    {
        _grid = grid;
        _matrixPortraitBuilder = matrixPortraitBuilder;
        _localAssembler = localAssembler;
        _inserter = inserter;
        _gaussExcluder = gaussExcluder;
        _localBasisFunctionsProvider = localBasisFunctionsProvider;
    }

    public GlobalAssembler<TNode> AssembleEquation()
    {
        var globalMatrix = _matrixPortraitBuilder.Build(_grid);
        _preconditionMatrix = globalMatrix.Clone();
        _equation = new Equation<SymmetricSparseMatrix>(
            globalMatrix,
            new GlobalVector(_grid.Nodes.Length),
            new GlobalVector(_grid.Nodes.Length)
        );

        foreach (var element in _grid)
        {
            var localMatrix = _localAssembler.AssembleMatrix(element);
            //var localVector = _localAssembler.AssembleRightSide(element);

            _inserter.InsertMatrix(_equation.Matrix, localMatrix);
            //_inserter.InsertVector(_equation.RightSide, localVector);
        }

        return this;
    }

    public GlobalAssembler<TNode> ApplySecondConditions(SecondCondition[] conditions)
    {
        foreach (var condition in conditions)
        {
            _inserter.InsertVector(_equation.RightSide, condition.Vector);
        }

        return this;
    }

    public GlobalAssembler<TNode> SetSource(Node2D sourcePoint, double power)
    {
        var element = _grid.Elements.First(x => ElementHas(x, sourcePoint));

        var basisFunctions = _localBasisFunctionsProvider.GetBilinearFunctions(element);

        for (var i = 0; i < element.NodesIndexes.Length; i++)
        {
            _bufferVector[i] = power * basisFunctions[i].Calculate(sourcePoint);
        }

        _inserter.InsertVector(_equation.RightSide, new LocalVector(element.NodesIndexes, _bufferVector));

        return this;
    }

    public GlobalAssembler<TNode> ApplyFirstConditions(FirstCondition[] conditions)
    {
        foreach (var condition in conditions)
        {
            _gaussExcluder.Exclude(_equation, condition);
        }

        return this;
    }

    public Equation<SymmetricSparseMatrix> BuildEquation()
    {
        return _equation;
    }

    public SymmetricSparseMatrix BuildPreconditionMatrix()
    {
        _preconditionMatrix = _equation.Matrix.Clone(_preconditionMatrix);
        return _preconditionMatrix;
    }

    private bool ElementHas(Element element, Node2D node)
    {
        var leftCornerNode = _grid.Nodes[element.NodesIndexes[0]];
        var rightCornerNode = _grid.Nodes[element.NodesIndexes[^1]];
        return node.R >= leftCornerNode.R && node.Z >= leftCornerNode.Z &&
               node.R <= rightCornerNode.R && node.Z <= rightCornerNode.Z;
    }
}