using DirectProblem.Core.Base;
using DirectProblem.Core.Global;
using DirectProblem.FEM;
using DirectProblem.SLAE.Preconditions;
using DirectProblem.SLAE.Solvers;
using DirectProblem.SLAE;

namespace InverseProblem.SLAE;

public class GaussElimination
{
    public Vector Solve(Equation<Matrix> equation)
    {
        ForwardElimination(equation);
        return BackSubstitution(equation);
    }

    private void ForwardElimination(Equation<Matrix> equation)
    {
        var matrix = equation.Matrix;
        var b = equation.RightSide;

        for (var i = 0; i < matrix.CountRows - 1; i++)
        {
            var max = Math.Abs(matrix[i, i]);

            var rowNumber = i;

            for (var j = i + 1; j < matrix.CountRows; j++)
            {
                if (max < Math.Abs(matrix[j, i]))
                {
                    max = Math.Abs(matrix[j, i]);
                    rowNumber = j;
                }
            }
            if (rowNumber != i)
            {
                matrix.SwapRows(i, rowNumber);
                (b[i], b[rowNumber]) = (b[rowNumber], b[i]);
            }

            if (Math.Abs(matrix[i, i]) > MethodsConfig.Eps)
            {
                for (var j = i + 1; j < matrix.CountRows; j++)
                {
                    var coefficient = matrix[j, i] / matrix[i, i];
                    matrix[j, i] = 0d;
                    b[j] -= coefficient * b[i];

                    for (var k = i + 1; k < matrix.CountRows; k++)
                    {
                        matrix[j, k] -= coefficient * matrix[i, k];
                    }
                }
            }
            else throw new DivideByZeroException();
        }
    }

    private Vector BackSubstitution(Equation<Matrix> equation)
    {
        var matrix = equation.Matrix;
        var solution = equation.Solution;
        var b = equation.RightSide;

        for (var i = matrix.CountRows - 1; i >= 0; i--)
        {
            var sum = 0d;

            for (var j = i + 1; j < matrix.CountRows; j++)
            {
                sum += matrix[i, j] * solution[j];
            }
            solution[i] = (b[i] - sum) / matrix[i, i];
        }

        return solution;
    }
}