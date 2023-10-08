using DirectProblem.Core.GridComponents;

namespace InverseProblem.Calculus;

public class PotentialDifferenceCalculator
{
    public double Calculate((Source A, Source B) sourcesLine, (Receiver M, Receiver N) receiversLine, double current, double sigma)
    {
        return current / (2 * Math.PI * sigma)
               * ((1 / Node2D.Distance(receiversLine.M.Point, sourcesLine.B.Point) -
                  1 / Node2D.Distance(receiversLine.M.Point, sourcesLine.A.Point))
                  - (1 / Node2D.Distance(receiversLine.N.Point, sourcesLine.B.Point) -
                     1 / Node2D.Distance(receiversLine.N.Point, sourcesLine.A.Point)));
    }
}