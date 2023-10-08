using DirectProblem.Core.Base;
using DirectProblem.Core.GridComponents;
using Electrostatics.Calculus;
using InverseProblem;
using InverseProblem.Assembling;
using InverseProblem.Calculus;
using InverseProblem.SLAE;
using System.Globalization;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var derivativeCalculator = new DerivativeCalculator();

var sourceA1 = new Source(new Node2D(0, -500), 2d);
var sourceB1 = new Source(new Node2D(100, -500), 2d);
var sourceA2 = new Source(new Node2D(0, 0), 3d);
var sourceB2 = new Source(new Node2D(100, 0), 3d);
var sourceA3 = new Source(new Node2D(0, 500), 4d);
var sourceB3 = new Source(new Node2D(100, 500), 4d);

var receiverM1 = new Receiver(new Node2D(200, 0));
var receiverN1 = new Receiver(new Node2D(300, 0));
var receiverM2 = new Receiver(new Node2D(500, 0));
var receiverN2 = new Receiver(new Node2D(600, 0));
var receiverM3 = new Receiver(new Node2D(1000, 0));
var receiverN3 = new Receiver(new Node2D(1100, 0));

var sigma = 0.01;

var sourcesLines = new[] { (sourceA1, sourceB1), (sourceA2, sourceB2), (sourceA3, sourceB3) };
var receiversLines = new[] { (receiverM1, receiverN1), (receiverM2, receiverN2), (receiverM3, receiverN3) };

var potentialDifferenceFunctionProvider = new PotentialDifferenceFunctionProvider(new PotentialDifferenceCalculator());
var slaeAssembler = new SLAEAssembler(potentialDifferenceFunctionProvider, derivativeCalculator, 
    sourcesLines, receiversLines, sigma);
var gaussElimination = new GaussElimination();
var regularizer = new Regularizer(gaussElimination);
var inverseProblemSolver = new InverseProblemSolver(slaeAssembler, regularizer, gaussElimination);

var solution = inverseProblemSolver
    .SetInitialValues(new[] { 2d, 3d, 4d})
    .Solve();

Console.Write("Solution: ");
foreach (var value in solution)
{
    Console.Write($"{value:E14} ");
}