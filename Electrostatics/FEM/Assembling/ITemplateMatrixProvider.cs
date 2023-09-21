using Electrostatics.Core.Base;

namespace Electrostatics.FEM.Assembling;

public interface ITemplateMatrixProvider
{
    public BaseMatrix GetMatrix();
}