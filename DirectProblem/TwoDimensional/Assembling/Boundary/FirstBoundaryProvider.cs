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

    public FirstConditionValue[] GetConditions(FirstCondition[] conditions)
    {
        var conditionsValues = new FirstConditionValue[conditions.Length];

        for (var i = 0; i < conditions.Length; i++)
        {
            var (indexes, _) = _grid.Elements[conditions[i].ElementIndex].GetBoundNodeIndexes(conditions[i].Bound);

            var values = new double[indexes.Length];

            for (var j = 0; j < indexes.Length; j++)
            {
                values[j] = Calculate(indexes[j]);
            }

            conditionsValues[i] = new FirstConditionValue(indexes, values);
        }

        return conditionsValues;
    }

    public FirstConditionValue[] GetConditions(int elementsByLength, int elementsByHeight)
    {
        var conditions = new FirstCondition[elementsByHeight];

        for (var i = 0; i < elementsByHeight; i++)
        {
            conditions[i] = new FirstCondition((i + 1) * elementsByLength - 1, Bound.Right);
        }

        return GetConditions(conditions);
    }

    private double Calculate(int index)
    {
        return _u(_grid.Nodes[index]);
    }
}