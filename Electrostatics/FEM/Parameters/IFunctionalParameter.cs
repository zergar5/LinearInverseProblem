using DirectProblem.Core.GridComponents;

namespace DirectProblem.FEM.Parameters;

public interface IFunctionalParameter
{
    public double Calculate(int nodeIndex);

    public double Calculate(Node2D node);
}