using Electrostatics.Core.GridComponents;
using Electrostatics.Core.Local;

namespace Electrostatics.FEM.Assembling.Local;

public interface ILocalAssembler
{
    public LocalMatrix AssembleMatrix(Element element);
    public LocalVector AssembleRightSide(Element element);
}