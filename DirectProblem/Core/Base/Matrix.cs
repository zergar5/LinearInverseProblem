using System.Runtime.InteropServices.Marshalling;

namespace DirectProblem.Core.Base;

public class Matrix
{
    public double[,] Values { get; }

    public Matrix(double[,] matrix)
    {
        Values = matrix;
    }
    public Matrix(int n) : this(new double[n, n]) { }

    public int CountRows => Values.GetLength(0);
    public int CountColumns => Values.GetLength(1);

    public double this[int i, int j]
    {
        get => Values[i, j];
        set => Values[i, j] = value;
    }

    public static Matrix Sum(Matrix matrix1, Matrix matrix2, Matrix? result = null)
    {
        if (matrix1.CountRows != matrix2.CountRows || matrix1.CountColumns != matrix2.CountColumns)
            throw new ArgumentOutOfRangeException(
                $"{nameof(matrix1)} and {nameof(matrix2)} must have same size");

        result ??= new Matrix(matrix1.CountRows);

        for (var i = 0; i < matrix1.CountRows; i++)
        {
            for (var j = 0; j < matrix1.CountColumns; j++)
            {
                result[i, j] = matrix1[i, j] + matrix2[i, j];
            }
        }

        return result;
    }

    public static Matrix Multiply(double coefficient, Matrix matrix, Matrix? result = null)
    {
        result ??= new Matrix(matrix.CountRows);

        for (var i = 0; i < matrix.CountRows; i++)
        {
            for (var j = 0; j < matrix.CountColumns; j++)
            {
                result[i, j] = matrix[i, j] * coefficient;
            }
        }

        return result;
    }

    public static Vector Multiply(Matrix matrix, Vector vector, Vector? result = null)
    {
        if (matrix.CountRows != vector.Count)
            throw new ArgumentOutOfRangeException(
                $"{nameof(matrix)} and {nameof(vector)} must have same size");

        result ??= new Vector(vector.Count);

        for (var i = 0; i < matrix.CountRows; i++)
        {
            for (var j = 0; j < matrix.CountColumns; j++)
            {
                result[i] += matrix[i, j] * vector[j];
            }
        }

        return result;
    }

    public static Span<double> Multiply(Matrix matrix, Span<double> vector, Span<double> result)
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

    public void SwapRows(int row1, int row2)
    {
        for (var i = 0; i < CountColumns; i++)
        {
            (Values[row2, i], Values[row1, i]) = (Values[row1, i], Values[row2, i]);
        }
    }
}