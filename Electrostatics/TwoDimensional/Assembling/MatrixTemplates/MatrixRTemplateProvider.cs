using Electrostatics.Core.Base;
using Electrostatics.FEM.Assembling;

namespace Electrostatics.TwoDimensional.Assembling.MatrixTemplates;

public class MatrixRTemplateProvider : ITemplateMatrixProvider
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