using DirectProblem.Core.Base;

namespace DirectProblem.FEM.Assembling;

public interface ITemplateMatrixProvider
{
    public BaseMatrix GetMatrix();
}