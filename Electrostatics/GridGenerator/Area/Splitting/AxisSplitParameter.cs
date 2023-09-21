using Electrostatics.GridGenerator.Area.Core;

namespace Electrostatics.GridGenerator.Area.Splitting;

public class AxisSplitParameter
{
    public Interval[] Sections { get; }
    public IntervalSplitter[] Splitters { get; }

    public IEnumerable<(Interval section, IntervalSplitter parameter)> SectionWithParameter =>
        Sections.Select((section, index) => new ValueTuple<Interval, IntervalSplitter>(section, Splitters[index]));

    public AxisSplitParameter(double[] points, params IntervalSplitter[] splitters)
    {
        if (points.Length - 1 != splitters.Length)
            throw new ArgumentException();

        Sections = GenerateSections(points).ToArray();
        Splitters = splitters;
    }

    private IEnumerable<Interval> GenerateSections(IEnumerable<double> points)
    {
        var left = points.First();
        foreach (var point in points.Skip(1))
        {
            var end = point;
            yield return new Interval(left, end);

            left = end;
        }
    }
}