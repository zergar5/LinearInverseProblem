namespace Electrostatics.Core.Global;

public class GlobalVector
{
    public double[] Vector { get; }

    public GlobalVector()
    {
        Vector = Array.Empty<double>();
    }

    public GlobalVector(int size)
    {
        Vector = new double[size];
    }

    public GlobalVector(double[] vector)
    {
        Vector = vector;
    }

    public double this[int index]
    {
        get => Vector[index];
        set => Vector[index] = value;
    }

    public int Count => Vector.Length;
    public double Norm => Math.Sqrt(ScalarProduct(this, this));

    public GlobalVector Clone()
    {
        var clone = new double[Count];
        Array.Copy(Vector, clone, Count);

        return new GlobalVector(clone);
    }

    public GlobalVector Clone(GlobalVector vector)
    {
        for (var i = 0; i < Vector.Length; i++)
        {
            vector[i] = Vector[i];
        }

        return vector;
    }

    public static double ScalarProduct(GlobalVector globalVector1, GlobalVector globalVector2)
    {
        var result = 0d;
        for (var i = 0; i < globalVector1.Count; i++)
        {
            result += globalVector1[i] * globalVector2[i];
        }
        return result;
    }

    public double ScalarProduct(GlobalVector vector)
    {
        return ScalarProduct(this, vector);
    }

    public static GlobalVector Sum(GlobalVector localVector1, GlobalVector localVector2, GlobalVector? result = null)
    {
        result ??= new GlobalVector(localVector1.Count);

        if (localVector1.Count != localVector2.Count) throw new Exception("Can't sum vectors");

        for (var i = 0; i < localVector1.Count; i++)
        {
            result[i] = localVector1[i] + localVector2[i];
        }

        return result;
    }

    public static GlobalVector Subtract(GlobalVector localVector1, GlobalVector localVector2, GlobalVector? result = null)
    {
        result ??= new GlobalVector(localVector1.Count);

        if (localVector1.Count != localVector2.Count) throw new Exception("Can't sum vectors");

        for (var i = 0; i < localVector1.Count; i++)
        {
            result[i] = localVector1[i] - localVector2[i];
        }

        return result;
    }

    public static GlobalVector Multiply(double number, GlobalVector vector, GlobalVector? result = null)
    {
        result ??= new GlobalVector(vector.Count);

        for (var i = 0; i < vector.Count; i++)
        {
            result[i] = vector[i] * number;
        }

        return result;
    }

    public IEnumerator<double> GetEnumerator() => ((IEnumerable<double>)Vector).GetEnumerator();
}