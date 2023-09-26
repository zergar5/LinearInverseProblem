using DirectProblem.Core.Base;
using DirectProblem.SLAE;
using InverseProblem.Assembling;
using InverseProblem.SLAE;

namespace InverseProblem;

public class InverseProblemSolver
{
    private readonly SLAEAssembler _slaeAssembler;
    private readonly GaussElimination _gaussElimination;
    private readonly Regularizer _regularizer;

    public InverseProblemSolver(SLAEAssembler slaeAssembler, GaussElimination gaussElimination, Regularizer regularizer)
    {
        _slaeAssembler = slaeAssembler;
        _gaussElimination = gaussElimination;
        _regularizer = regularizer;
    }

    public Vector Solve()
    {
        var residual = 1d;
        for (var i = 1; i <= MethodsConfig.MaxIterations && residual > Math.Pow(MethodsConfig.Eps, 2); i++)
        {
            var equation = _slaeAssembler.BuildEquation();
            try
            {
                _gaussElimination.Solve(equation);
            }
            catch (Exception)
            {
                _regularizer.Regularize(equation);
                _gaussElimination.Solve(equation);
            }

            residual = Vector.Subtract(
                equation.RightSide, 
                Matrix.Multiply(equation.Matrix, equation.Solution), equation.RightSide).Norm;
        }
            
    }
}