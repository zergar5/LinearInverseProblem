using Electrostatics.Core.Boundary;
using Electrostatics.Core.Global;

namespace Electrostatics.FEM.Assembling.Global;

public interface IGaussExcluder<TMatrix>
{
    public void Exclude(Equation<TMatrix> equation, FirstCondition condition);
}