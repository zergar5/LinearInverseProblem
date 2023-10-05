using DirectProblem.Core.Base;
using DirectProblem.Core.Global;
using DirectProblem.Core.GridComponents;

namespace InverseProblem.SLAE;

public class Regularizer
{
    private readonly GaussElimination _gaussElimination;
    public Matrix IdentityMatrix { get; set; }
    public Matrix BufferMatrix { get; set; }
    public Vector BufferVector { get; set; }

    public Regularizer(GaussElimination gaussElimination)
    {
        _gaussElimination = gaussElimination;
    }

    public double Regularize(Equation<Matrix> equation, Vector trueCurrents)
    {
        var alpha = CalculateAlpha(equation.Matrix);
        var error = CalculateError(equation.Solution, trueCurrents);
        var ratio = 0d;

        do
        {
            try
            {
                Matrix.Sum(equation.Matrix, Matrix.Multiply(alpha, IdentityMatrix, BufferMatrix), BufferMatrix);

                Vector.Subtract(
                    equation.RightPart, Vector.Multiply(
                        alpha, Vector.Subtract(equation.Solution, trueCurrents, BufferVector),
                        BufferVector),
                    BufferVector);

                //BufferVector = equation.RightPart.Copy(BufferVector);

                BufferVector = _gaussElimination.Solve(BufferMatrix, BufferVector);

                var currentError = CalculateError(
                    Vector.Sum(equation.Solution, BufferVector, BufferVector), trueCurrents);

                ratio = currentError / error;
            }
            catch
            {
                alpha *= 1.5;
            }
            finally
            {
                alpha *= 1.5;
            }
        } while (ratio < 2d);

        return alpha / 1.5;
    }

    private double CalculateAlpha(Matrix matrix)
    {
        var n = matrix.CountRows;
        var alpha = 0d;

        for (var i = 0; i < n; i++)
        {
            alpha += matrix[i, i];
        }

        alpha /= n * 10e14;

        return alpha;
    }

    private double CalculateError(Vector currents, Vector trueCurrents)
    {
        var n = currents.Count;
        var sum = 0d;

        for (var i = 0; i < n; i++)
        {
            sum += Math.Pow(currents[i] - trueCurrents[i], 2);
        }

        return sum / n;
    }
}