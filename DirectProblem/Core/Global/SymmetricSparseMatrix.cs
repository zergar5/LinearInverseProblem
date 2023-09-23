using DirectProblem.Extensions;

namespace DirectProblem.Core.Global;

public class SymmetricSparseMatrix
{
    private readonly double[] _diagonal;
    private readonly double[] _values;
    public int[] RowsIndexes { get; }
    public int[] ColumnsIndexes { get; }

    public int Count => _diagonal.Length;

    public int[] this[int rowIndex] => ColumnsIndexes[RowsIndexes[rowIndex]..RowsIndexes[rowIndex + 1]];

    public double this[int rowIndex, int columnIndex]
    {
        get
        {
            if (rowIndex < 0 || columnIndex < 0) throw new ArgumentOutOfRangeException(nameof(rowIndex));

            if (rowIndex == columnIndex)
            {
                return _diagonal[rowIndex];
            }

            if (columnIndex > rowIndex)
                (rowIndex, columnIndex) = (columnIndex, rowIndex);

            var index = this[rowIndex].FindIndex(columnIndex);
            
            return _values[index];
        }
        set
        {
            if (rowIndex < 0 || columnIndex < 0) throw new ArgumentOutOfRangeException(nameof(rowIndex));

            if (rowIndex == columnIndex)
            {
                _diagonal[rowIndex] = value;
                return;
            }

            if (columnIndex > rowIndex)
                (rowIndex, columnIndex) = (columnIndex, rowIndex);

            var index = this[rowIndex].FindIndex(columnIndex);

            _values[index] = value;
        }
    }

    public SymmetricSparseMatrix(int[] rowsIndexes, int[] columnsIndexes)
    {
        _diagonal = new double[rowsIndexes.Length - 1];
        _values = new double[rowsIndexes[^1]];
        RowsIndexes = rowsIndexes;
        ColumnsIndexes = columnsIndexes;
    }

    public SymmetricSparseMatrix
    (
        int[] rowsIndexes,
        int[] columnsIndexes,
        double[] diagonal,
        double[] values
    )
    {
        RowsIndexes = rowsIndexes;
        ColumnsIndexes = columnsIndexes;
        _diagonal = diagonal;
        _values = values;
    }

    public static GlobalVector Multiply(SymmetricSparseMatrix matrix, GlobalVector vector, GlobalVector? result = null)
    {
        if (matrix.Count != vector.Count)
            throw new ArgumentOutOfRangeException(
                $"{nameof(matrix)} and {nameof(vector)} must have same size");

        result ??= new GlobalVector(matrix.Count);

        var rowsIndexes = matrix.RowsIndexes;

        for (var i = 0; i < matrix.Count; i++)
        {
            result[i] += matrix[i, i] * vector[i];

            for (var j = rowsIndexes[i]; j < rowsIndexes[i + 1]; j++)
            {
                result[i] += matrix[i, j] * vector[j];
                result[j] += matrix[i, j] * vector[i];
            }
        }

        return result;
    }

    public SymmetricSparseMatrix Clone()
    {
        var rowIndexes = new int[RowsIndexes.Length];
        var columnIndexes = new int[ColumnsIndexes.Length];
        var diagonal = new double[_diagonal.Length];
        var values = new double[_values.Length];

        Array.Copy(RowsIndexes, rowIndexes, RowsIndexes.Length);
        Array.Copy(ColumnsIndexes, columnIndexes, ColumnsIndexes.Length);
        Array.Copy(_diagonal, diagonal, _diagonal.Length);
        Array.Copy(_values, values, _values.Length);

        return new SymmetricSparseMatrix(rowIndexes, columnIndexes, diagonal, values);
    }

    public SymmetricSparseMatrix Copy(SymmetricSparseMatrix sparseMatrix)
    {
        Array.Copy(_diagonal, sparseMatrix._diagonal, _diagonal.Length);
        Array.Copy(_values, sparseMatrix._values, _values.Length);

        return sparseMatrix;
    }
}