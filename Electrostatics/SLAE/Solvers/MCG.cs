using CourseProject.SLAE;
using Electrostatics.Core.Global;
using Electrostatics.FEM;
using Electrostatics.SLAE.Preconditions.LLT;

namespace Electrostatics.SLAE.Solvers;

public class MCG
{
    private readonly LLTPreconditioner _lltPreconditioner;
    private readonly LLTSparse _lltSparse;
    private SymmetricSparseMatrix _preconditionMatrix;
    private GlobalVector _r;
    private GlobalVector _z;

    public MCG(LLTPreconditioner lltPreconditioner, LLTSparse lltSparse)
    {
        _lltPreconditioner = lltPreconditioner;
        _lltSparse = lltSparse;
    }

    private void PrepareProcess(Equation<SymmetricSparseMatrix> equation)
    {
        _preconditionMatrix = _lltPreconditioner.Decompose(_preconditionMatrix);

        _r = GlobalVector.Subtract(equation.RightSide, SymmetricSparseMatrix.Multiply(equation.Matrix, equation.Solution, _r), _r);
        _z = _lltSparse.Solve(_preconditionMatrix, _r);
    }

    public GlobalVector Solve(Equation<SymmetricSparseMatrix> equation, SymmetricSparseMatrix preconditionMatrix)
    {
        _preconditionMatrix = preconditionMatrix;
        PrepareProcess(equation);
        IterationProcess(equation);
        return equation.Solution;
    }

    private void IterationProcess(Equation<SymmetricSparseMatrix> equation)
    {
        var x = equation.Solution;
        var rzBufferVector = new GlobalVector(x.Count);
        var xBufferVector = new GlobalVector(x.Count);

        var bNorm = equation.RightSide.Norm;
        var residual = _r.Norm / bNorm;

        for (var i = 1; i <= MethodsConfig.MaxIterations && residual > Math.Pow(MethodsConfig.Eps, 2); i++)
        {
            rzBufferVector = _r.Clone(rzBufferVector);
            var scalarMrR = GlobalVector.ScalarProduct(_lltSparse.Solve(_preconditionMatrix, _r, rzBufferVector), _r);

            var AxZ = SymmetricSparseMatrix.Multiply(equation.Matrix, _z, xBufferVector);

            var alphaK = scalarMrR / GlobalVector.ScalarProduct(AxZ, _z);

            GlobalVector.Sum(x, GlobalVector.Multiply(alphaK, _z, rzBufferVector), x);

            var rNext = GlobalVector.Subtract(_r, GlobalVector.Multiply(alphaK, AxZ, AxZ), _r);

            xBufferVector = rNext.Clone(xBufferVector);
            var betaK = GlobalVector.ScalarProduct(_lltSparse.Solve(_preconditionMatrix, rNext, xBufferVector), rNext) /
                        scalarMrR;

            xBufferVector = rNext.Clone(xBufferVector);
            var zNext = GlobalVector.Sum(_lltSparse.Solve(_preconditionMatrix, rNext, xBufferVector),
                GlobalVector.Multiply(betaK, _z, _z), _z);

            residual = rNext.Norm / bNorm;

            _r = rNext;
            _z = zNext;

            CourseHolder.GetInfo(i, residual);
        }

        Console.WriteLine();
    }
}