using Electrostatics.Core;
using Electrostatics.Core.GridComponents;
using Electrostatics.FEM.Parameters;

namespace Electrostatics.TwoDimensional.Parameters;

public class RightPartParameter : IFunctionalParameter
{
    private readonly Func<Node2D, double> _function;
    private readonly Grid<Node2D> _grid;

    public RightPartParameter(
        Func<Node2D, double> function,
        Grid<Node2D> grid
    )
    {
        _function = function;
        _grid = grid;
    }

    public double Calculate(int nodeNumber)
    {
        var node = _grid.Nodes[nodeNumber];
        return _function(node);
    }

    public double Calculate(Node2D node)
    {
        return _function(node);
    }
}