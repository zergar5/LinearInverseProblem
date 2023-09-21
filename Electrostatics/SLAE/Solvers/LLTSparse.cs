using Electrostatics.Core.Global;
using Electrostatics.SLAE.Preconditions.LLT;

namespace Electrostatics.SLAE.Solvers;

public class LLTSparse
{
    private readonly LLTPreconditioner _lltPreconditioner;

    public LLTSparse(LLTPreconditioner lltPreconditioner)
    {
        _lltPreconditioner = lltPreconditioner;
    }

    public GlobalVector Solve(SymmetricSparseMatrix matrix, GlobalVector vector, GlobalVector? result = null)
    {
        var y = CalcY(matrix, vector, result);
        var x = CalcX(matrix, y, y);

        return x;
    }

    public GlobalVector CalcY(SymmetricSparseMatrix sparseMatrix, GlobalVector b, GlobalVector? y = null)
    {
        y ??= b.Clone();

        for (var i = 0; i < sparseMatrix.CountRows; i++)
        {
            var sum = 0.0;
            for (var j = sparseMatrix.RowsIndexes[i]; j < sparseMatrix.RowsIndexes[i + 1]; j++)
            {
                sum += sparseMatrix.Values[j] * y[sparseMatrix.ColumnsIndexes[j]];
            }
            y[i] = (b[i] - sum) / sparseMatrix.Diagonal[i];
        }

        return y;
    }

    public GlobalVector CalcX(SymmetricSparseMatrix sparseMatrix, GlobalVector y, GlobalVector? x = null)
    {
        x ??= y.Clone();

        for (var i = sparseMatrix.CountRows - 1; i >= 0; i--)
        {
            x[i] /= sparseMatrix.Diagonal[i];
            for (var j = sparseMatrix.RowsIndexes[i + 1] - 1; j >= sparseMatrix.RowsIndexes[i]; j--)
            {
                x[sparseMatrix.ColumnsIndexes[j]] -= sparseMatrix.Values[j] * x[i];
            }
        }

        return x;
    }
}