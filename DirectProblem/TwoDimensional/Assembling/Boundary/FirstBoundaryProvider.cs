using DirectProblem.Core;
using DirectProblem.Core.Boundary;
using DirectProblem.Core.GridComponents;

namespace DirectProblem.TwoDimensional.Assembling.Boundary;

public class FirstBoundaryProvider
{
    private readonly Grid<Node2D> _grid;
    private readonly Func<Node2D, double> _u;
    private double[][]? _buffers;

    public FirstBoundaryProvider(Grid<Node2D> grid, Func<Node2D, double> u)
    {
        _grid = grid;
        _u = u;
    }

    public FirstConditionValue[] GetConditions(FirstCondition[] conditions)
    {
        var conditionsValues = new FirstConditionValue[conditions.Length];

        if (_buffers is null)
        {
            _buffers = new double[conditionsValues.Length][];

            for (var i = 0; i < conditions.Length; i++)
            {
                _buffers[i] = new double[2];
            }
        }

        for (var i = 0; i < conditions.Length; i++)
        {
            var (indexes, _) = _grid.Elements[conditions[i].ElementIndex].GetBoundNodeIndexes(conditions[i].Bound);

            for (var j = 0; j < indexes.Length; j++)
            {
                _buffers[i][j] = Calculate(indexes[j]);
            }

            conditionsValues[i] = new FirstConditionValue(indexes, _buffers[i]);
        }

        return conditionsValues;
    }

    public FirstConditionValue[] GetConditions(int elementsByLength, int elementsByHeight)
    {
        var conditions = new FirstCondition[elementsByLength + elementsByHeight];
        var j = 0;

        for (var i = 0; i < elementsByLength; i++, j++)
        {
            conditions[j] = new FirstCondition(i, Bound.Lower);
        }

        for (var i = 0; i < elementsByHeight; i++, j++)
        {
            conditions[j] = new FirstCondition((i + 1) * elementsByLength - 1, Bound.Right);
        }

        return GetConditions(conditions);
    }

    private double Calculate(int index)
    {
        return _u(_grid.Nodes[index]);
    }
}