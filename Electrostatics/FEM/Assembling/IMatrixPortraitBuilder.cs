using Electrostatics.Core;
using Electrostatics.Core.GridComponents;

namespace Electrostatics.FEM.Assembling;

public interface IMatrixPortraitBuilder<out TMatrix>
{
    TMatrix Build(Grid<Node2D> grid);
}