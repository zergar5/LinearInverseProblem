namespace DirectProblem.Core.GridComponents;

public record struct Node2D(double R, double Z)
{
    public static double Distance(Node2D node1, Node2D node2)
    {
        return Math.Sqrt(Math.Pow(node2.R - node1.R, 2) + Math.Pow(node2.Z - node1.Z, 2));
    }
}