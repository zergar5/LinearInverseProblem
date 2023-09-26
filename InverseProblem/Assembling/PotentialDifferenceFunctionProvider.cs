using DirectProblem.Core.GridComponents;
using InverseProblem.Calculus;

namespace InverseProblem.Assembling;

public class PotentialDifferenceFunctionProvider
{
    private readonly PotentialDifferenceCalculator _potentialDifferenceCalculator;

    public PotentialDifferenceFunctionProvider(PotentialDifferenceCalculator potentialDifferenceCalculator)
    {
        _potentialDifferenceCalculator = potentialDifferenceCalculator;
    }

    public Func<double, double> CreateForOneSource((Source A, Source B) sourcesLine, (Receiver N, Receiver M) receiversLine, double sigma)
    {
        return current => _potentialDifferenceCalculator.Calculate(sourcesLine, receiversLine, current, sigma);
    }

    public Func<double[], double> CreateForSeveralSources((Source A, Source B)[] sourcesLines,
        (Receiver N, Receiver M) receiversLine, double sigma)
    {
        return currents =>
        {
            if (currents.Length != sourcesLines.Length)
                throw new ArgumentOutOfRangeException(
                    $"{nameof(currents)} and {nameof(sourcesLines)} must have same size");

            var potentialDifference = 0d;

            for (var i = 0; i < currents.Length; i++)
            {
                potentialDifference += 
                    _potentialDifferenceCalculator.Calculate(
                        sourcesLines[i], receiversLine, sigma, currents[i]);
            }

            return potentialDifference;
        };
    }
}