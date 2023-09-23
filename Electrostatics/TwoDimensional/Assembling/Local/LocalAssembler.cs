using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.GridComponents;
using DirectProblem.Core.Local;
using DirectProblem.FEM.Assembling.Local;
using DirectProblem.FEM.Parameters;
using DirectProblem.TwoDimensional.Parameters;

namespace Electrostatics.TwoDimensional.Assembling.Local;

public class LocalAssembler : ILocalAssembler
{
    private readonly Grid<Node2D> _grid;
    private readonly ILocalMatrixAssembler _localMatrixAssembler;
    private readonly MaterialFactory _materialFactory;
    private readonly IFunctionalParameter _functionalParameter;
    private readonly BaseVector _rightPart = new(4);

    public LocalAssembler
    (
        Grid<Node2D> grid,
        ILocalMatrixAssembler localMatrixAssembler,
        MaterialFactory materialFactory,
        IFunctionalParameter functionalParameter
    )
    {
        _grid = grid;
        _localMatrixAssembler = localMatrixAssembler;
        _materialFactory = materialFactory;
        _functionalParameter = functionalParameter;
    }

    public LocalMatrix AssembleMatrix(Element element)
    {
        var matrix = GetStiffnessMatrix(element);
        var sigma = _materialFactory.GetById(element.MaterialId).Sigma;

        return new LocalMatrix(element.NodesIndexes, BaseMatrix.Multiply(sigma, matrix, matrix));
    }

    public LocalVector AssembleRightSide(Element element)
    {
        var vector = GetRightPart(element);

        return new LocalVector(element.NodesIndexes, vector);
    }

    private BaseMatrix GetStiffnessMatrix(Element element)
    {
        var stiffness = _localMatrixAssembler.AssembleStiffnessMatrix(element);

        return stiffness;
    }

    private BaseVector GetRightPart(Element element)
    {
        //var rInterval = new Interval(_grid.Nodes[element.NodesIndexes[0]].R, _grid.Nodes[element.NodesIndexes[1]].R);
        //var zInterval = new Interval(_grid.Nodes[element.NodesIndexes[0]].Z, _grid.Nodes[element.NodesIndexes[2]].Z);

        //var localBasisFunctions = _localBasisFunctionsProvider.GetBilinearFunctions(element);

        //for (var i = 0; i < element.NodesIndexes.Length; i++)
        //{
        //    _rightPart[i] = _doubleIntegralCalculator.Calculate
        //    (
        //        rInterval,
        //        zInterval,
        //        (r, z) =>
        //        {
        //            var node = new Node2D(r, z);
        //            return
        //                _functionalParameter.Calculate(node) * localBasisFunctions[i].Calculate(node) * r;
        //        }
        //    );
        //}

        return _rightPart;
    }
}