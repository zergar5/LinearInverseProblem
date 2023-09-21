using Electrostatics.Core.GridComponents;

namespace Electrostatics.FEM.Parameters;

public interface IFunctionalParameter
{
    public double Calculate(int nodeIndex);

    public double Calculate(Node2D node);
}