namespace DirectProblem.Core.Base;

public class BaseMatrix
{
    public double[,] Values { get; }

    public BaseMatrix(double[,] matrix)
    {
        Values = matrix;
    }
    public BaseMatrix(int n) : this(new double[n, n]) { }

    public int CountRows => Values.GetLength(0);
    public int CountColumns => Values.GetLength(1);

    public double this[int i, int j]
    {
        get => Values[i, j];
        set => Values[i, j] = value;
    }

    public static BaseMatrix Sum(BaseMatrix matrix1, BaseMatrix matrix2, BaseMatrix? result = null)
    {
        if (matrix1.CountRows != matrix2.CountRows || matrix1.CountColumns != matrix2.CountColumns)
            throw new ArgumentOutOfRangeException(
                $"{nameof(matrix1)} and {nameof(matrix2)} must have same size");

        result ??= new BaseMatrix(matrix1.CountRows);

        for (var i = 0; i < matrix1.CountRows; i++)
        {
            for (var j = 0; j < matrix1.CountColumns; j++)
            {
                result[i, j] = matrix1[i, j] + matrix2[i, j];
            }
        }

        return result;
    }

    public static BaseMatrix Multiply(double coefficient, BaseMatrix matrix, BaseMatrix? result = null)
    {
        result ??= new BaseMatrix(matrix.CountRows);

        for (var i = 0; i < matrix.CountRows; i++)
        {
            for (var j = 0; j < matrix.CountColumns; j++)
            {
                result[i, j] = matrix[i, j] * coefficient;
            }
        }

        return result;
    }

    public static BaseVector Multiply(BaseMatrix matrix, BaseVector vector, BaseVector? result = null)
    {
        if (matrix.CountRows != vector.Count)
            throw new ArgumentOutOfRangeException(
                $"{nameof(matrix)} and {nameof(vector)} must have same size");

        result ??= new BaseVector(vector.Count);

        for (var i = 0; i < matrix.CountRows; i++)
        {
            for (var j = 0; j < matrix.CountColumns; j++)
            {
                result[i] += matrix[i, j] * vector[j];
            }
        }

        return result;
    }

    public static Span<double> Multiply(BaseMatrix matrix, Span<double> vector, Span<double> result)
    {
        if (matrix.CountRows != vector.Length || vector.Length != result.Length)
            throw new ArgumentOutOfRangeException(
                $"{nameof(matrix)}, {nameof(vector)} and {nameof(result)} must have same size");

        for (var i = 0; i < matrix.CountRows; i++)
        {
            for (var j = 0; j < matrix.CountColumns; j++)
            {
                result[i] += matrix[i, j] * vector[j];
            }
        }

        return result;
    }
}