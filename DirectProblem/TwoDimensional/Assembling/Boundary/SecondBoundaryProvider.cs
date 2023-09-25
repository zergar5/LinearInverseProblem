using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.Boundary;
using DirectProblem.Core.GridComponents;
using DirectProblem.Core.Local;
using DirectProblem.TwoDimensional.Assembling.MatrixTemplates;
using DirectProblem.TwoDimensional.Parameters;
using Electrostatics.Calculus;

namespace DirectProblem.TwoDimensional.Assembling.Boundary;

public class SecondBoundaryProvider
{
    private readonly Grid<Node2D> _grid;
    private readonly MaterialFactory _materialFactory;
    private readonly Func<Node2D, double> _u;
    private readonly DerivativeCalculator _derivativeCalculator;
    private readonly BaseMatrix _massZ;
    private readonly BaseMatrix _massR;
    private readonly BaseVector _rBufferVector = new(2);
    private readonly BaseVector _zBufferVector = new(2);
    private BaseVector[]? _vectors;

    public SecondBoundaryProvider
    (
        Grid<Node2D> grid,
        MaterialFactory materialFactory,
        Func<Node2D, double> u,
        DerivativeCalculator derivativeCalculator,
        MassMatrixTemplateProvider massMatrixTemplateProvider
    )
    {
        _grid = grid;
        _materialFactory = materialFactory;
        _u = u;
        _derivativeCalculator = derivativeCalculator;
        _massZ = massMatrixTemplateProvider.MassMatrix;
        _massR = massMatrixTemplateProvider.MassRMatrix;
    }

    public SecondConditionValue[] GetConditions(SecondCondition[] conditions)
    {
        var conditionsValue = new SecondConditionValue[conditions.Length];

        if (_vectors is null)
        {
            _vectors = new BaseVector[conditions.Length];

            for (var i = 0; i < _vectors.Length; i++)
            {
                _vectors[i] = new BaseVector(2);
            }
        }

        for (var i = 0; i < conditions.Length; i++)
        {
            var (indexes, h) = _grid.Elements[conditions[i].ElementIndex].GetBoundNodeIndexes(conditions[i].Bound);

            var sigma = _materialFactory.GetById(_grid.Elements[conditions[i].ElementIndex].MaterialId).Sigma;

            if (conditions[i].Bound == Bound.Left || conditions[i].Bound == Bound.Right)
            {
                _vectors[i] = GetRVector(indexes, conditions[i].Bound, h, sigma, _vectors[i]);
            }
            else
            {
                _vectors[i] = GetZVector(indexes, conditions[i].Bound, h, sigma, _vectors[i]);
            }

            conditionsValue[i] = new SecondConditionValue(new LocalVector(indexes, _vectors[i]));
        }

        return conditionsValue;
    }

    private BaseVector GetRVector(int[] indexes, Bound bound, double h, double sigma, BaseVector vector)
    {
        for (var i = 0; i < vector.Count; i++)
        {
            vector[i] = _derivativeCalculator.Calculate(_u, _grid.Nodes[indexes[i]], 'r');
        }

        BaseMatrix.Multiply(_massR, vector, _rBufferVector);

        if (bound == Bound.Left)
        {
            BaseVector.Multiply(-sigma * h * _grid.Nodes[indexes[0]].R / 6d,
                _rBufferVector, vector);
        }
        else
        {
            BaseVector.Multiply(sigma * h * _grid.Nodes[indexes[0]].R / 6d,
                _rBufferVector, vector);
        }

        _rBufferVector.Clear();

        return vector;
    }

    private BaseVector GetZVector(int[] indexes, Bound bound, double h, double sigma, BaseVector vector)
    {
        for (var i = 0; i < vector.Count; i++)
        {
            vector[i] = _derivativeCalculator.Calculate(_u, _grid.Nodes[indexes[i]], 'z');
        }

        BaseMatrix.Multiply(_massR, vector, _rBufferVector);
        BaseMatrix.Multiply(_massZ, vector, _zBufferVector);

        if (bound == Bound.Lower)
        {
            BaseVector.Sum
            (
                BaseVector.Multiply(-sigma * h * _grid.Nodes[indexes[0]].R / 6d,
                    _rBufferVector, _rBufferVector),
                BaseVector.Multiply(-sigma * Math.Pow(h, 2) / 12d,
                    _zBufferVector, _zBufferVector),
                vector
            );
        }
        else
        {
            BaseVector.Sum
            (
                BaseVector.Multiply(sigma * h * _grid.Nodes[indexes[0]].R / 6d,
                    _rBufferVector, _rBufferVector),
                BaseVector.Multiply(sigma * Math.Pow(h, 2) / 12d,
                    _zBufferVector, _zBufferVector),
                vector
            );
        }

        _rBufferVector.Clear();
        _zBufferVector.Clear();

        return vector;
    }

    public SecondConditionValue[] GetConditions(int elementsByLength, int elementsByHeight)
    {
        var conditions = new SecondCondition[2 * elementsByLength + elementsByHeight];

        var j = 0;

        for (var i = 0; i < elementsByHeight; i++, j++)
        {
            conditions[j] = new SecondCondition(i * elementsByLength, Bound.Left);
        }

        for (var i = elementsByLength * (elementsByHeight - 1); i < elementsByLength * elementsByHeight; i++, j++)
        {
            conditions[j] = new SecondCondition(i, Bound.Upper);
        }

        return GetConditions(conditions);
    }
}