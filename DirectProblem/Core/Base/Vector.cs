namespace DirectProblem.Core.Base;

public class Vector
{
    public double[] Values { get; }

    public Vector()
    {
        Values = Array.Empty<double>();
    }

    public Vector(int size)
    {
        Values = new double[size];
    }

    public Vector(double[] values)
    {
        Values = values;
    }

    public double this[int index]
    {
        get => Values[index];
        set => Values[index] = value;
    }

    public int Count => Values.Length;
    public double Norm => Math.Sqrt(ScalarProduct(this, this));

    public static double ScalarProduct(Vector vector1, Vector vector2)
    {
        var result = 0d;
        for (var i = 0; i < vector1.Count; i++)
        {
            result += vector1[i] * vector2[i];
        }
        return result;
    }

    public double ScalarProduct(Vector vector)
    {
        return ScalarProduct(this, vector);
    }

    public static Vector Sum(Vector localVector1, Vector localVector2, Vector? result = null)
    {
        result ??= new Vector(localVector1.Count);

        if (localVector1.Count != localVector2.Count) 
            throw new ArgumentOutOfRangeException(
            $"{nameof(localVector1)} and {nameof(localVector2)} must have same size");

        for (var i = 0; i < localVector1.Count; i++)
        {
            result[i] = localVector1[i] + localVector2[i];
        }

        return result;
    }

    public static Vector Subtract(Vector localVector1, Vector localVector2, Vector? result = null)
    {
        result ??= new Vector(localVector1.Count);

        if (localVector1.Count != localVector2.Count) 
            throw new ArgumentOutOfRangeException(
            $"{nameof(localVector1)} and {nameof(localVector2)} must have same size");

        for (var i = 0; i < localVector1.Count; i++)
        {
            result[i] = localVector1[i] - localVector2[i];
        }

        return result;
    }

    public static Vector Multiply(double number, Vector vector, Vector? result = null)
    {
        result ??= new Vector(vector.Count);

        for (var i = 0; i < vector.Count; i++)
        {
            result[i] = vector[i] * number;
        }

        return result;
    }

    public void Fill(double value)
    {
        Array.Fill(Values, value);
    }

    public void Clear()
    {
        Array.Clear(Values);
    }

    public Vector Clone()
    {
        var clone = new double[Count];
        Array.Copy(Values, clone, Count);

        return new Vector(clone);
    }

    public Vector Copy(Vector vector)
    {
        for (var i = 0; i < Values.Length; i++)
        {
            vector[i] = Values[i];
        }

        return vector;
    }

    public IEnumerator<double> GetEnumerator() => ((IEnumerable<double>)Values).GetEnumerator();
}