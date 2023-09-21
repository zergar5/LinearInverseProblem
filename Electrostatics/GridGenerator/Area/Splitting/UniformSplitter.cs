using Electrostatics.GridGenerator.Area.Core;

namespace Electrostatics.GridGenerator.Area.Splitting;

public readonly record struct UniformSplitter(int Steps) : IntervalSplitter
{
    public IEnumerable<double> EnumerateValues(Interval interval)
    {
        var step = interval.Length / Steps;

        for (var stepNumber = 0; stepNumber <= Steps; stepNumber++)
        {
            var value = interval.Begin + stepNumber * step;

            yield return value;
        }
    }
}