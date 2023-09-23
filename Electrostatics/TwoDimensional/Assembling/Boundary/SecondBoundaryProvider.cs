using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.Boundary;
using DirectProblem.Core.GridComponents;
using DirectProblem.Core.Local;
using DirectProblem.FEM.Assembling;
using DirectProblem.TwoDimensional.Parameters;
using Electrostatics.Calculus;

namespace Electrostatics.TwoDimensional.Assembling.Boundary;

public class SecondBoundaryProvider
{
    private readonly Grid<Node2D> _grid;
    private readonly MaterialFactory _materialFactory;
    //private readonly Func<Node2D, double> _u;
    private readonly DerivativeCalculator _derivativeCalculator;
    private readonly BaseMatrix _templateMatrixR;
    private readonly BaseMatrix _templateMatrixZ;
    private readonly BaseVector _rBufferVector = new(2);
    private readonly BaseVector _zBufferVector = new(2);
    private BaseVector[]? _vectors;

    public SecondBoundaryProvider
    (
        Grid<Node2D> grid,
        MaterialFactory materialFactory,
        //Func<Node2D, double> u,
        DerivativeCalculator derivativeCalculator,
        ITemplateMatrixProvider templateMatrixProviderR,
        ITemplateMatrixProvider templateMatrixProviderZ
    )
    {
        _grid = grid;
        _materialFactory = materialFactory;
        //_u = u;
        _derivativeCalculator = derivativeCalculator;
        _templateMatrixR = templateMatrixProviderR.GetMatrix();
        _templateMatrixZ = templateMatrixProviderZ.GetMatrix();
    }

    public SecondCondition[] GetConditions(int[] elementsIndexes, Bound[] bounds)
    {
        var conditions = new List<SecondCondition>(elementsIndexes.Length);
        if (_vectors is null)
        {
            _vectors = new BaseVector[elementsIndexes.Length];

            for (var i = 0; i < _vectors.Length; i++)
            {
                _vectors[i] = new BaseVector(2);
            }
        }

        for (var i = 0; i < elementsIndexes.Length; i++)
        {
            var (indexes, h) = _grid.Elements[elementsIndexes[i]].GetBoundNodeIndexes(bounds[i]);

            var sigma = _materialFactory.GetById(_grid.Elements[elementsIndexes[i]].MaterialId).Sigma;

            if (bounds[i] == Bound.Left || bounds[i] == Bound.Right)
            {
                _vectors[i] = GetRVector(indexes, bounds[i], h, sigma, _vectors[i]);
            }
            else
            {
                _vectors[i] = GetZVector(indexes, bounds[i], h, sigma, _vectors[i]);
            }

            conditions.Add(new SecondCondition(new LocalVector(indexes, _vectors[i])));
        }

        return conditions.ToArray();
    }

    private BaseVector GetRVector(int[] indexes, Bound bound, double h, double sigma, BaseVector vector)
    {
        for (var i = 0; i < vector.Count; i++)
        {
            vector[i] = 0; /*_derivativeCalculator.Calculate(_u, _grid.Nodes[indexes[i]], 'r');*/
        }

        //if (bound == Bound.Left)
        //{
        //    BaseVector.Multiply(-sigma * h * _grid.Nodes[indexes[0]].R / 6d,
        //        BaseMatrix.Multiply(_templateMatrixR, vector, _rBufferVector), vector);
        //    _rBufferVector.Clear();
        //}
        //else
        //{
        //    BaseVector.Multiply(sigma * h * _grid.Nodes[indexes[0]].R / 6d,
        //        BaseMatrix.Multiply(_templateMatrixR, vector, _rBufferVector), vector);
        //    _rBufferVector.Clear();
        //}

        return vector;
    }

    private BaseVector GetZVector(int[] indexes, Bound bound, double h, double sigma, BaseVector vector)
    {
        for (var i = 0; i < vector.Count; i++)
        {
            vector[i] = 0; /*_derivativeCalculator.Calculate(_u, _grid.Nodes[indexes[i]], 'z');*/
        }

        //if (bound == Bound.Lower)
        //{
        //    BaseVector.Sum
        //    (
        //        BaseVector.Multiply(-sigma * h * _grid.Nodes[indexes[0]].R / 6d,
        //            BaseMatrix.Multiply(_templateMatrixR, vector, _rBufferVector), _rBufferVector),
        //        BaseVector.Multiply(-sigma * Math.Pow(h, 2) / 12d,
        //            BaseMatrix.Multiply(_templateMatrixZ, vector, _zBufferVector), _zBufferVector),
        //        vector
        //    );
        //    _rBufferVector.Clear();
        //    _zBufferVector.Clear();
        //}
        //else
        //{
        //    BaseVector.Sum
        //    (
        //        BaseVector.Multiply(sigma * h * _grid.Nodes[indexes[0]].R / 6d,
        //            BaseMatrix.Multiply(_templateMatrixR, vector, _rBufferVector), _rBufferVector),
        //        BaseVector.Multiply(sigma * Math.Pow(h, 2) / 12d,
        //            BaseMatrix.Multiply(_templateMatrixZ, vector, _zBufferVector), _zBufferVector),
        //        vector
        //    );
        //    _rBufferVector.Clear();
        //    _zBufferVector.Clear();
        //}

        return vector;
    }

    public SecondCondition[] GetConditions(int elementsByLength, int elementsByHeight)
    {
        var elementsIndexes = new List<int>(2 * elementsByLength + elementsByHeight);
        var bounds = new List<Bound>(2 * elementsByLength + elementsByHeight);
        _vectors = new BaseVector[2 * elementsByLength + elementsByHeight];

        for (var i = 0; i < _vectors.Length; i++)
        {
            _vectors[i] = new BaseVector(2);
        }

        for (var i = 0; i < elementsByLength; i++)
        {
            elementsIndexes.Add(i);
            bounds.Add(Bound.Lower);
        }

        for (var i = 0; i < elementsByHeight; i++)
        {
            elementsIndexes.Add(i * elementsByLength);
            bounds.Add(Bound.Left);
        }

        for (var i = elementsByLength * (elementsByHeight - 1); i < elementsByLength * elementsByHeight; i++)
        {
            elementsIndexes.Add(i);
            bounds.Add(Bound.Upper);
        }

        return GetConditions(elementsIndexes.ToArray(), bounds.ToArray());
    }
}