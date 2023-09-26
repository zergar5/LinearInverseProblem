using DirectProblem.Core.Base;
using DirectProblem.Core.Global;
using DirectProblem.Core.GridComponents;

namespace InverseProblem.SLAE;

public class Regularizer
{
    private readonly GaussElimination _gaussElimination;
    private readonly Matrix _identityMatrix;
    private readonly Vector _trueCurrentsVector;
    private readonly Matrix _bufferMatrix;
    private readonly Vector _bufferVector;

    public Regularizer(GaussElimination gaussElimination, Matrix identityMatrix, double[] trueCurrents)
    {
        _gaussElimination = gaussElimination;
        _identityMatrix = identityMatrix;
        _trueCurrentsVector = new Vector(trueCurrents);
        _bufferMatrix = new Matrix(identityMatrix.CountRows);
        _bufferVector = new Vector(_bufferMatrix.CountRows);
    }

    public double Regularize(Equation<Matrix> equation)
    {
        var alpha = CalculateAlpha(equation.Matrix);

        while (true)
        {
            try
            {
                Matrix.Sum(equation.Matrix, Matrix.Multiply(alpha, _identityMatrix, _bufferMatrix), _bufferMatrix);

                Vector.Subtract(equation.RightSide,
                    Vector.Multiply(alpha, 
                        Vector.Subtract(equation.Solution, _trueCurrentsVector, _bufferVector), 
                        _bufferVector), 
                    _bufferVector);

                _gaussElimination.Solve(equation);

                break;
            }
            catch (Exception)
            {
                alpha *= 10;
            }
        }

        var countStable = 0;
        var error = CalculateError(equation.Solution, _trueCurrentsVector);

        do
        {
            Matrix.Sum(equation.Matrix, Matrix.Multiply(alpha, _identityMatrix, _bufferMatrix), _bufferMatrix);

            Vector.Subtract(equation.RightSide,
                Vector.Multiply(alpha,
                    Vector.Subtract(equation.Solution, _trueCurrentsVector, _bufferVector),
                    _bufferVector),
                _bufferVector);

            _gaussElimination.Solve(equation);

            var currentError = CalculateError(equation.Solution, _trueCurrentsVector);

            var ratio = error / currentError;

            if (ratio is > 0.9 and < 1d) {countStable++;}
            else countStable = 0;

            error = currentError;

        } while (countStable < 4);

        return alpha;
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