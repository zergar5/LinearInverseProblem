using DirectProblem.Core.Global;
using DirectProblem.Extensions;

namespace DirectProblem.SLAE.Preconditions.LLT;

public class LLTPreconditioner : IPreconditioner<SymmetricSparseMatrix>
{
    public SymmetricSparseMatrix Decompose(SymmetricSparseMatrix globalMatrix)
    {
        var preconditionMatrix = globalMatrix;

        for (var i = 0; i < preconditionMatrix.Count; i++)
        {
            var sumD = 0.0;

            for (var j = preconditionMatrix.RowsIndexes[i]; j < preconditionMatrix.RowsIndexes[i + 1]; j++)
            {
                var sum = 0d;

                for (var k = preconditionMatrix.RowsIndexes[i]; k < j; k++)
                {
                    var iPrev = i - preconditionMatrix.ColumnsIndexes[j];
                    var kPrev = preconditionMatrix[i - iPrev].FindIndex(preconditionMatrix.ColumnsIndexes[k]);

                    if (kPrev == -1) continue;

                    sum += preconditionMatrix[iPrev, preconditionMatrix.ColumnsIndexes[k]] *
                           preconditionMatrix[iPrev, preconditionMatrix.ColumnsIndexes[kPrev]];
                }

                preconditionMatrix[i, j] = (preconditionMatrix[i, j] - sum) /
                                           preconditionMatrix[preconditionMatrix.ColumnsIndexes[j],
                                               preconditionMatrix.ColumnsIndexes[j]];

                sumD += Math.Pow(preconditionMatrix[i, j], 2);
            }

            preconditionMatrix[i, i] = Math.Sqrt(preconditionMatrix[i, i] - sumD);
        }

        return preconditionMatrix;
    }
}