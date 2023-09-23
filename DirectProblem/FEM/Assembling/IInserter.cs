using DirectProblem.Core.Global;
using DirectProblem.Core.Local;

namespace DirectProblem.FEM.Assembling;

public interface IInserter<in TMatrix>
{
    public void InsertMatrix(TMatrix globalMatrix, LocalMatrix localMatrix);
    public void InsertVector(GlobalVector vector, LocalVector localVector);
}