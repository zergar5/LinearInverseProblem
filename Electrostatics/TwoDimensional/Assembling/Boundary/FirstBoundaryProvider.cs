using DirectProblem.Core;
using DirectProblem.Core.Boundary;
using DirectProblem.Core.GridComponents;

namespace DirectProblem.TwoDimensional.Assembling.Boundary;

public class FirstBoundaryProvider
{
    private readonly Grid<Node2D> _grid;
    private readonly Func<Node2D, double> _u;

    public FirstBoundaryProvider(Grid<Node2D> grid, Func<Node2D, double> u)
    {
        _grid = grid;
        _u = u;
    }

    public FirstConditionValue[] GetConditions(int[] elementsIndexes, Bound[] bounds)
    {
        var conditions = new List<FirstConditionValue>(elementsIndexes.Length);

        for (var i = 0; i < elementsIndexes.Length; i++)
        {
            var (indexes, _) = _grid.Elements[elementsIndexes[i]].GetBoundNodeIndexes(bounds[i]);

            var values = new double[indexes.Length];

            for (var j = 0; j < indexes.Length; j++)
            {
                values[j] = Calculate(indexes[j]);
            }

            conditions.Add(new FirstConditionValue(indexes, values));
        }

        return conditions.ToArray();
    }

    public FirstCondition[] GetConditions(int elementsByLength, int elementsByHeight)
    {
        var elementsIndexes = new List<int>(elementsByHeight);
        var bounds = new List<Bound>(elementsByHeight);

        for (var i = 0; i < elementsByHeight; i++)
        {
            elementsIndexes.Add((i + 1) * elementsByLength - 1);
            bounds.Add(Bound.Right);
        }

        return GetConditions(elementsIndexes.ToArray(), bounds.ToArray());
    }

    private double Calculate(int index)
    {
        return _u(_grid.Nodes[index]);
    }
}