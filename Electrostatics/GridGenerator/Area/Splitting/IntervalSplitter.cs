using Electrostatics.GridGenerator.Area.Core;

namespace Electrostatics.GridGenerator.Area.Splitting;

public interface IntervalSplitter
{
    public IEnumerable<double> EnumerateValues(Interval interval);
    public int Steps { get; }
}