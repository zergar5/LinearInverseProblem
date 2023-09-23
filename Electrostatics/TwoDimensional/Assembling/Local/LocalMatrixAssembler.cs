using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.GridComponents;
using DirectProblem.FEM.Assembling;
using DirectProblem.FEM.Assembling.Local;

namespace DirectProblem.TwoDimensional.Assembling.Local;

public class LocalMatrixAssembler : ILocalMatrixAssembler
{
    private readonly Grid<Node2D> _grid;
    private readonly ITemplateMatrixProvider _stiffnessMatrixTemplateProvider;
    private readonly ITemplateMatrixProvider _massMatrixTemplateProvider;
    private readonly ITemplateMatrixProvider _massMatrixRTemplateProvider;
    private readonly BaseMatrix _stiffness = new(4);
    private readonly BaseMatrix _stiffnessR = new(2);
    private readonly BaseMatrix _stiffnessZ = new(2);
    private readonly BaseMatrix _mass = new(4);
    private readonly BaseMatrix _massR = new(2);
    private readonly BaseMatrix _massZ = new(2);

    public LocalMatrixAssembler
    (
        Grid<Node2D> grid,
        ITemplateMatrixProvider stiffnessMatrixTemplateProvider,
        ITemplateMatrixProvider massMatrixTemplateProvider,
        ITemplateMatrixProvider massMatrixRTemplateProvider
    )
    {
        _grid = grid;
        _stiffnessMatrixTemplateProvider = stiffnessMatrixTemplateProvider;
        _massMatrixTemplateProvider = massMatrixTemplateProvider;
        _massMatrixRTemplateProvider = massMatrixRTemplateProvider;
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

    public BaseMatrix AssembleMassMatrix(Element element)
    {
        var massR = AssembleMassR(element);
        var massZ = AssembleMassZ(element);

        for (var i = 0; i < element.NodesIndexes.Length; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                _mass[i, j] = massR[GetMuIndex(i), GetMuIndex(j)] * massZ[GetNuIndex(i), GetNuIndex(j)];
                _mass[j, i] = _mass[i, j];
            }
        }

        return _mass;
    }

    private BaseMatrix AssembleStiffnessR(Element element)
    {
        var stiffnessR = _stiffnessMatrixTemplateProvider.GetMatrix();
        BaseMatrix.Multiply((2 * _grid.Nodes[element.NodesIndexes[0]].R + element.Length) / (2 * element.Length),
            stiffnessR, _stiffnessR);

        return _stiffnessR;
    }

    private BaseMatrix AssembleStiffnessZ(Element element)
    {
        var stiffnessZ = _stiffnessMatrixTemplateProvider.GetMatrix();
        BaseMatrix.Multiply(1d / element.Height,
            stiffnessZ, _stiffnessZ);

        return _stiffnessZ;
    }

    private BaseMatrix AssembleMassR(Element element)
    {
        var massR = _massMatrixRTemplateProvider.GetMatrix();
        BaseMatrix.Multiply(Math.Pow(element.Length, 2) / 12d,
            massR, _massR);

        var massZ = _massMatrixTemplateProvider.GetMatrix();
        BaseMatrix.Multiply(element.Length * _grid.Nodes[element.NodesIndexes[0]].R / 6d,
            massZ, _massZ);

        BaseMatrix.Sum(_massR, _massZ, _massR);

        return _massR;
    }

    private BaseMatrix AssembleMassZ(Element element)
    {
        var massZ = _massMatrixTemplateProvider.GetMatrix();
        BaseMatrix.Multiply(element.Height / 6d,
            massZ, _massZ);

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