using Electrostatics.Calculus;
using Electrostatics.Core;
using Electrostatics.Core.Base;
using Electrostatics.Core.GridComponents;
using Electrostatics.Core.Local;
using Electrostatics.FEM.Assembling.Local;
using Electrostatics.FEM.Parameters;
using Electrostatics.GridGenerator.Area.Core;
using Electrostatics.TwoDimensional.Parameters;

namespace Electrostatics.TwoDimensional.Assembling.Local;

public class LocalAssembler : ILocalAssembler
{
    private readonly Grid<Node2D> _grid;
    private readonly LocalBasisFunctionsProvider _localBasisFunctionsProvider;
    private readonly MaterialFactory _materialFactory;
    //private readonly IFunctionalParameter _functionalParameter;
    private readonly DoubleIntegralCalculator _doubleIntegralCalculator;
    private readonly DerivativeCalculator _derivativeCalculator;
    private readonly BaseMatrix _stiffnessMatrix = new(4);
    private readonly BaseVector _rightPart = new(4);

    public LocalAssembler
    (
        Grid<Node2D> grid,
        LocalBasisFunctionsProvider localBasisFunctionsProvider,
        MaterialFactory materialFactory,
        //IFunctionalParameter functionalParameter,
        DoubleIntegralCalculator doubleIntegralCalculator,
        DerivativeCalculator derivativeCalculator
    )
    {
        _grid = grid;
        _localBasisFunctionsProvider = localBasisFunctionsProvider;
        _materialFactory = materialFactory;
        //_functionalParameter = functionalParameter;
        _doubleIntegralCalculator = doubleIntegralCalculator;
        _derivativeCalculator = derivativeCalculator;
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
        var rInterval = new Interval(_grid.Nodes[element.NodesIndexes[0]].R, _grid.Nodes[element.NodesIndexes[1]].R);
        var zInterval = new Interval(_grid.Nodes[element.NodesIndexes[0]].Z, _grid.Nodes[element.NodesIndexes[2]].Z);

        var localBasisFunctions = _localBasisFunctionsProvider.GetBilinearFunctions(element);

        for (var i = 0; i < element.NodesIndexes.Length; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                _stiffnessMatrix[i, j] = _doubleIntegralCalculator.Calculate
                (
                    rInterval,
                    zInterval,
                    (r, z) =>
                    {
                        var node = new Node2D(r, z);
                        return
                            (_derivativeCalculator.Calculate(localBasisFunctions[i], node, 'r') *
                             _derivativeCalculator.Calculate(localBasisFunctions[j], node, 'r') +
                             _derivativeCalculator.Calculate(localBasisFunctions[i], node, 'z') *
                             _derivativeCalculator.Calculate(localBasisFunctions[j], node, 'z')) *
                            r;
                    }
                );

                _stiffnessMatrix[j, i] = _stiffnessMatrix[i, j];
            }
        }

        return _stiffnessMatrix;
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