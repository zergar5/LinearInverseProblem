using Electrostatics.Core.Global;
using Electrostatics.Core.Local;

namespace Electrostatics.FEM.Assembling;

public interface IInserter<in TMatrix>
{
    public void InsertMatrix(TMatrix globalMatrix, LocalMatrix localMatrix);
    public void InsertVector(GlobalVector vector, LocalVector localVector);
}