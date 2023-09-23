namespace DirectProblem.Extensions;

public static class ArrayExtensions
{
    public static int FindIndex<T>(this T[] array, T value)
    {
        var index = Array.IndexOf(array, value, 0,
            array.Length);

        return index;
    }
}