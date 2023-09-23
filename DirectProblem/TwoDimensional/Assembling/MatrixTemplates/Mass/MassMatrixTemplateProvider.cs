using DirectProblem.Core.Base;
using DirectProblem.FEM.Assembling;

namespace DirectProblem.TwoDimensional.Assembling.MatrixTemplates.Mass;

public class MassMatrixTemplateProvider : ITemplateMatrixProvider
{
    public BaseMatrix GetMatrix()
    {
        return new BaseMatrix(
            new[,]
            {
                { 2d, 1d },
                { 1d, 2d }
            }
        );
    }
}