using DirectProblem.Core.Base;
using DirectProblem.Core.GridComponents;

namespace DirectProblem.FEM.Assembling.Local;

public interface ILocalMatrixAssembler
{
    public BaseMatrix AssembleStiffnessMatrix(Element element);
    public BaseMatrix AssembleMassMatrix(Element element);
}