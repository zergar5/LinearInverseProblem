using DirectProblem.Core.Base;
using DirectProblem.Core.GridComponents;
using DirectProblem.Core.Local;
using DirectProblem.FEM.Assembling.Local;
using DirectProblem.TwoDimensional.Parameters;

namespace DirectProblem.TwoDimensional.Assembling.Local;

public class LocalAssembler : ILocalAssembler
{
    private readonly ILocalMatrixAssembler _localMatrixAssembler;
    private readonly MaterialFactory _materialFactory;
    private readonly BaseVector _rightPart = new(4);

    public LocalAssembler
    (
        ILocalMatrixAssembler localMatrixAssembler,
        MaterialFactory materialFactory
    )
    {
        _localMatrixAssembler = localMatrixAssembler;
        _materialFactory = materialFactory;
    }

    public LocalMatrix AssembleMatrix(Element element)
    {
        var matrix = GetStiffnessMatrix(element);
        var sigma = _materialFactory.GetById(element.MaterialId).Sigma;

        return new LocalMatrix(element.NodesIndexes, BaseMatrix.Multiply(sigma, matrix, matrix));
    }

    public LocalVector AssembleRightSide(Element element)
    {
        var vector = GetRightPart(element);

        return new LocalVector(element.NodesIndexes, vector);
    }

    private BaseMatrix GetStiffnessMatrix(Element element)
    {
        var stiffness = _localMatrixAssembler.AssembleStiffnessMatrix(element);

        return stiffness;
    }

    private BaseVector GetRightPart(Element element)
    {
        return _rightPart;
    }
}