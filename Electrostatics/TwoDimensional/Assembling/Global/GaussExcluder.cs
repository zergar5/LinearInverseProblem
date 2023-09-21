using Electrostatics.Core.Boundary;
using Electrostatics.Core.Global;
using Electrostatics.FEM.Assembling.Global;

namespace Electrostatics.TwoDimensional.Assembling.Global;

public class GaussExcluder : IGaussExcluder<SymmetricSparseMatrix>
{
    public void Exclude(Equation<SymmetricSparseMatrix> equation, FirstCondition condition)
    {

        equation.RightSide[condition.NodeIndex] = condition.Value;
        equation.Matrix.Diagonal[condition.NodeIndex] = 1d;

        for (var j = equation.Matrix.RowsIndexes[condition.NodeIndex];
             j < equation.Matrix.RowsIndexes[condition.NodeIndex + 1];
             j++)
        {
            equation.RightSide[equation.Matrix.ColumnsIndexes[j]] -= equation.Matrix.Values[j] * condition.Value;
            equation.Matrix.Values[j] = 0d;
        }

        for (var j = condition.NodeIndex + 1; j < equation.Matrix.CountRows; j++)
        {
            var elementIndex = equation.Matrix[j, condition.NodeIndex];

            if (elementIndex == -1) continue;

            equation.RightSide[j] -= equation.Matrix.Values[elementIndex] * condition.Value;
            equation.Matrix.Values[elementIndex] = 0d;
        }

    }
}