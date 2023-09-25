using DirectProblem.Core.Base;

namespace DirectProblem.TwoDimensional.Assembling.MatrixTemplates;

public class MassMatrixTemplateProvider
{
    public BaseMatrix MassMatrix => new(
        new[,]
        {
            { 2d, 1d },
            { 1d, 2d }
        });

    public BaseMatrix MassRMatrix => new(
        new[,]
        {
            { 1d, 1d },
            { 1d, 3d }
        });
}