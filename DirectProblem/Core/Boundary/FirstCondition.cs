namespace DirectProblem.Core.Boundary;

public record struct FirstCondition(int ElementIndex, Bound Bound);
public record struct FirstConditionValue(int[] NodesIndexes, double[] Values);