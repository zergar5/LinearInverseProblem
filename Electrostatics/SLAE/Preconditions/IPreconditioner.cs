namespace Electrostatics.SLAE.Preconditions;

public interface IPreconditioner<TMatrix>
{
    public TMatrix Decompose(TMatrix globalMatrix);
}