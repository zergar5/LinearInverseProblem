using Electrostatics.Core;

namespace Electrostatics.GridGenerator;

public interface IGridBuilder<TPoint>
{
    public Grid<TPoint> Build();
}