using DirectProblem.Core.Base;
using DirectProblem.FEM.Assembling;

namespace DirectProblem.TwoDimensional.Assembling.MatrixTemplates.Mass;

public class MassMatrixRTemplateProvider : ITemplateMatrixProvider
{
    public BaseMatrix GetMatrix()
    {
        return new BaseMatrix(
            new[,]
            {
                { 1d, 1d },
                { 1d, 3d }
            }
        );
    }
}