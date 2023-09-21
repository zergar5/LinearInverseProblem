namespace Electrostatics.Core.Base;

public class BaseMatrix
{
    public double[,] Values { get; }

    public BaseMatrix() : this(new double[0, 0]) { }
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
        result ??= new BaseMatrix(matrix1.CountRows);

        if (matrix1.CountRows != matrix2.CountRows && matrix1.CountColumns != matrix2.CountColumns)
        {
            throw new Exception("Can't sum matrix");
        }

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
        result ??= new BaseVector(vector.Count);

        if (matrix.CountRows != vector.Count)
        {
            throw new Exception("Can't multiply matrix");
        }

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
        if (matrix.CountRows != vector.Length)
        {
            throw new Exception("Can't multiply matrix");
        }

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