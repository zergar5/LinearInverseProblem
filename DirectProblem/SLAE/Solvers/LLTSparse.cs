using DirectProblem.Core.Global;

namespace DirectProblem.SLAE.Solvers;

public class LLTSparse
{
    public GlobalVector Solve(SymmetricSparseMatrix matrix, GlobalVector vector, GlobalVector? result = null)
    {
        var y = CalcY(matrix, vector, result);
        var x = CalcX(matrix, y, y);

        return x;
    }

    public GlobalVector CalcY(SymmetricSparseMatrix sparseMatrix, GlobalVector b, GlobalVector? y = null)
    {
        y ??= new GlobalVector(b.Count);

        for (var i = 0; i < sparseMatrix.Count; i++)
        {
            var sum = 0.0;
            foreach (var j in sparseMatrix[i])
            {
                sum += sparseMatrix[i, j] * y[j];
            }
            y[i] = (b[i] - sum) / sparseMatrix[i, i];
        }

        return y;
    }

    public GlobalVector CalcX(SymmetricSparseMatrix sparseMatrix, GlobalVector y, GlobalVector? x = null)
    {
        x = x == null ? y.Clone() : y.Copy(x);

        for (var i = sparseMatrix.Count - 1; i >= 0; i--)
        {
            x[i] /= sparseMatrix[i, i];
            var columns = sparseMatrix[i];
            for (var j = columns.Length - 1; j >= 0; j--)
            {
                x[columns[j]] -= sparseMatrix[i, columns[j]] * x[i];
            }
        }

        return x;
    }


}