using Electrostatics.Core.Base;
using Electrostatics.FEM.Assembling;

namespace Electrostatics.TwoDimensional.Assembling.MatrixTemplates;

public class MatrixZTemplateProvider : ITemplateMatrixProvider
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