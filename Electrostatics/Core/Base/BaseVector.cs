namespace Electrostatics.Core.Base;

public class BaseVector
{
    public double[] Vector { get; }

    public BaseVector() : this(Array.Empty<double>()) { }
    public BaseVector(double[] vector)
    {
        Vector = vector;
    }
    public BaseVector(int size) : this(new double[size]) { }

    public int Count => Vector.Length;
    public double this[int index]
    {
        get => Vector[index];
        set => Vector[index] = value;
    }

    public static BaseVector Multiply(double number, BaseVector localVector, BaseVector? result = null)
    {
        result ??= new BaseVector(localVector.Vector);

        for (var i = 0; i < localVector.Count; i++)
        {
            result[i] = number * localVector[i];
        }

        return result;
    }

    public static BaseVector Sum(BaseVector vector1, BaseVector vector2, BaseVector? result = null)
    {
        if (vector1.Count != vector2.Count) throw new Exception("Can't sum vectors");

        result ??= new BaseVector(vector1.Vector);

        for (var i = 0; i < vector1.Count; i++)
        {
            result[i] = vector1[i] + vector2[i];
        }

        return result;
    }

    public void Clear()
    {
        Array.Clear(Vector);
    }

    public IEnumerator<double> GetEnumerator() => ((IEnumerable<double>)Vector).GetEnumerator();
}