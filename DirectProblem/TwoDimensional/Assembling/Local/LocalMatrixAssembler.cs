using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.GridComponents;
using DirectProblem.FEM.Assembling.Local;
using DirectProblem.TwoDimensional.Assembling.MatrixTemplates;

namespace DirectProblem.TwoDimensional.Assembling.Local;

public class LocalMatrixAssembler : ILocalMatrixAssembler
{
    private readonly Grid<Node2D> _grid;
    private readonly BaseMatrix _stiffnessTemplate;
    private readonly BaseMatrix _massTemplate;
    private readonly BaseMatrix _massRTemplate;
    private readonly BaseMatrix _stiffness = new(4);
    private readonly BaseMatrix _stiffnessR = new(2);
    private readonly BaseMatrix _stiffnessZ = new(2);
    private readonly BaseMatrix _massR = new(2);
    private readonly BaseMatrix _massZ = new(2);

    public LocalMatrixAssembler
    (
        Grid<Node2D> grid,
        StiffnessMatrixTemplatesProvider stiffnessMatrixTemplateProvider,
        MassMatrixTemplateProvider massMatrixTemplateProvider
    )
    {
        _grid = grid;
        _stiffnessTemplate = stiffnessMatrixTemplateProvider.StiffnessMatrix;
        _massTemplate = massMatrixTemplateProvider.MassMatrix;
        _massRTemplate = massMatrixTemplateProvider.MassRMatrix;
    }

    public BaseMatrix AssembleStiffnessMatrix(Element element)
    {
        var stiffnessR = AssembleStiffnessR(element);
        var stiffnessZ = AssembleStiffnessZ(element);

        var massR = AssembleMassR(element);
        var massZ = AssembleMassZ(element);

        for (var i = 0; i < element.NodesIndexes.Length; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                _stiffness[i, j] = stiffnessR[GetMuIndex(i), GetMuIndex(j)] * massZ[GetNuIndex(i), GetNuIndex(j)] +
                                   massR[GetMuIndex(i), GetMuIndex(j)] * stiffnessZ[GetNuIndex(i), GetNuIndex(j)];
                _stiffness[j, i] = _stiffness[i, j];
            }
        }

        return _stiffness;
    }

    private BaseMatrix AssembleStiffnessR(Element element)
    {
        BaseMatrix.Multiply((2 * _grid.Nodes[element.NodesIndexes[0]].R + element.Length) / (2 * element.Length),
            _stiffnessTemplate, _stiffnessR);

        return _stiffnessR;
    }

    private BaseMatrix AssembleStiffnessZ(Element element)
    {
        BaseMatrix.Multiply(1d / element.Height,
            _stiffnessTemplate, _stiffnessZ);

        return _stiffnessZ;
    }

    private BaseMatrix AssembleMassR(Element element)
    {
        BaseMatrix.Multiply(Math.Pow(element.Length, 2) / 12d,
            _massRTemplate, _massR);

        BaseMatrix.Multiply(element.Length * _grid.Nodes[element.NodesIndexes[0]].R / 6d,
            _massTemplate, _massZ);

        BaseMatrix.Sum(_massR, _massZ, _massR);

        return _massR;
    }

    private BaseMatrix AssembleMassZ(Element element)
    {
        BaseMatrix.Multiply(element.Height / 6d,
            _massTemplate, _massZ);

        return _massZ;
    }

    private int GetMuIndex(int i)
    {
        return i % 2;
    }

    private int GetNuIndex(int i)
    {
        return i / 2;
    }
}