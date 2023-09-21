namespace Electrostatics.Core.Global;

public record Equation<TMatrix>(TMatrix Matrix, GlobalVector Solution, GlobalVector RightSide);