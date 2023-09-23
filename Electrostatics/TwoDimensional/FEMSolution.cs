using DirectProblem.Core;
using DirectProblem.Core.Global;
using DirectProblem.Core.GridComponents;
using DirectProblem.FEM;
using DirectProblem.TwoDimensional.Assembling.Local;

namespace Electrostatics.TwoDimensional;

public class FEMSolution
{
    private readonly Grid<Node2D> _grid;
    private readonly GlobalVector _solution;
    private readonly LocalBasisFunctionsProvider _basisFunctionsProvider;

    public FEMSolution(Grid<Node2D> grid, GlobalVector solution, LocalBasisFunctionsProvider basisFunctionsProvider)
    {
        _grid = grid;
        _solution = solution;
        _basisFunctionsProvider = basisFunctionsProvider;
    }

    public double Calculate(Node2D point)
    {
        if (AreaHas(point))
        {
            var element = _grid.Elements.First(x => ElementHas(x, point));

            var basisFunctions = _basisFunctionsProvider.GetBilinearFunctions(element);

            var sum = 0d;

            sum += element.NodesIndexes
                .Select((t, i) => _solution[t] * basisFunctions[i].Calculate(point))
                .Sum();


            //CourseHolder.WriteSolution(point, sum);

            return sum;
        }

        CourseHolder.WriteAreaInfo();
        CourseHolder.WriteSolution(point, double.NaN);
        return double.NaN;
    }

    public double CalculatePotentialDifference(Node2D firstPoint, Node2D secondPoint)
    {
        if (AreaHas(firstPoint) && AreaHas(secondPoint))
        {
            var element = _grid.Elements.First(x => ElementHas(x, firstPoint));

            var basisFunctions = _basisFunctionsProvider.GetBilinearFunctions(element);

            var firstPhi = element.NodesIndexes
                .Select((t, i) => _solution[t] * basisFunctions[i].Calculate(firstPoint))
            .Sum();

            element = _grid.Elements.First(x => ElementHas(x, secondPoint));

            basisFunctions = _basisFunctionsProvider.GetBilinearFunctions(element);

            var secondPhi = element.NodesIndexes
                .Select((t, i) => _solution[t] * basisFunctions[i].Calculate(firstPoint))
                .Sum();

            var potentialDifference = (firstPhi - secondPhi) / Math.Abs(firstPoint.Z - secondPoint.Z);

            CourseHolder.WriteSolution(firstPoint, secondPoint, potentialDifference);

            return potentialDifference;
        }

        CourseHolder.WriteAreaInfo();
        CourseHolder.WriteSolution(firstPoint, secondPoint, double.NaN);
        return double.NaN;
    }

    public double CalcError(Func<Node2D, double> u)
    {
        var solution = new GlobalVector(_solution.Count);
        var trueSolution = new GlobalVector(_solution.Count);

        for (var i = 0; i < _solution.Count; i++)
        {
            solution[i] = Calculate(_grid.Nodes[i]);
            trueSolution[i] = u(_grid.Nodes[i]);
        }

        GlobalVector.Subtract(solution, trueSolution, trueSolution);

        return trueSolution.Norm;
    }

    private bool ElementHas(Element element, Node2D node)
    {
        var leftCornerNode = _grid.Nodes[element.NodesIndexes[0]];
        var rightCornerNode = _grid.Nodes[element.NodesIndexes[^1]];
        return node.R >= leftCornerNode.R && node.Z >= leftCornerNode.Z &&
               node.R <= rightCornerNode.R && node.Z <= rightCornerNode.Z;
    }

    private bool AreaHas(Node2D node)
    {
        var leftCornerNode = _grid.Nodes[0];
        var rightCornerNode = _grid.Nodes[^1];
        return node.R >= leftCornerNode.R && node.Z >= leftCornerNode.Z &&
               node.R <= rightCornerNode.R && node.Z <= rightCornerNode.Z;
    }
}