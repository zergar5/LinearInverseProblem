using DirectProblem.Core.Base;
using DirectProblem.Core.Global;
using DirectProblem.SLAE;
using InverseProblem.Assembling;
using InverseProblem.SLAE;

namespace InverseProblem;

public class InverseProblemSolver
{
    private readonly SLAEAssembler _slaeAssembler;
    private readonly Regularizer _regularizer;
    private readonly GaussElimination _gaussElimination;
    private Vector _trueCurrentsVector;
    private Matrix _identityMatrix;
    private Matrix _bufferMatrix;
    private Vector _bufferVector;

    public InverseProblemSolver(SLAEAssembler slaeAssembler, Regularizer regularizer, GaussElimination gaussElimination)
    {
        _slaeAssembler = slaeAssembler;
        _regularizer = regularizer;
        _gaussElimination = gaussElimination;
    }

    public InverseProblemSolver SetInitialValues(double initialValue)
    {
        _slaeAssembler.SetInitialValues(initialValue);

        return this;
    }

    public InverseProblemSolver SetInitialValues(double[] initialValues)
    {
        _slaeAssembler.SetInitialValues(initialValues);

        return this;
    }

    private void Prepare()
    {
        _slaeAssembler.CalculateTrueValues();
        _trueCurrentsVector = new Vector(_slaeAssembler.TrueCurrents);
        _identityMatrix = Matrix.CreateIdentityMatrix(_trueCurrentsVector.Count);
        _bufferMatrix = new Matrix(_trueCurrentsVector.Count);
        _bufferVector = new Vector(_bufferMatrix.CountRows);
        _regularizer.BufferMatrix = _bufferMatrix;
        _regularizer.BufferVector = _bufferVector;
        _regularizer.IdentityMatrix = _identityMatrix;
    }

    public Vector Solve()
    {
        Prepare();

        var residual = 1d;
        Equation<Matrix> equation = null;

        for (var i = 1; i <= MethodsConfig.MaxIterations && residual > MethodsConfig.Eps; i++)
        {
            equation = _slaeAssembler.BuildEquation();

            var alpha = _regularizer.Regularize(equation, _trueCurrentsVector);

            Matrix.Sum(equation.Matrix, Matrix.Multiply(alpha, _identityMatrix, _bufferMatrix), equation.Matrix);

            Vector.Subtract(
                equation.RightPart, Vector.Multiply(
                    alpha, Vector.Subtract(equation.Solution, _trueCurrentsVector, _bufferVector),
                    _bufferVector),
                equation.RightPart);

            _bufferMatrix = equation.Matrix.Copy(_bufferMatrix);
            _bufferVector = equation.RightPart.Copy(_bufferVector);

            _bufferVector = _gaussElimination.Solve(_bufferMatrix, _bufferVector);

            residual = Vector.Subtract(
                equation.RightPart,
                Matrix.Multiply(equation.Matrix, Vector.Sum(equation.Solution, _bufferVector, equation.Solution), _bufferVector), _bufferVector).Norm;
        }

        return equation.Solution;
    }
}