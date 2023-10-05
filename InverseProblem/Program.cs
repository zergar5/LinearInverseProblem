//using DirectProblem.Core.GridComponents;
//using DirectProblem.FEM;
//using DirectProblem.GridGenerator;
//using DirectProblem.GridGenerator.Intervals.Splitting;
//using DirectProblem.SLAE.Preconditions;
//using DirectProblem.SLAE.Solvers;
//using DirectProblem.TwoDimensional.Assembling;
//using DirectProblem.TwoDimensional.Assembling.Boundary;
//using DirectProblem.TwoDimensional.Assembling.Global;
//using DirectProblem.TwoDimensional.Assembling.Local;
//using DirectProblem.TwoDimensional.Assembling.MatrixTemplates;
//using DirectProblem.TwoDimensional.Parameters;
//using Electrostatics.Calculus;
//using Electrostatics.TwoDimensional;
//using Electrostatics.TwoDimensional.Assembling.Global;
//using System.Globalization;

using DirectProblem.Core.Base;
using DirectProblem.Core.GridComponents;
using Electrostatics.Calculus;
using InverseProblem;
using InverseProblem.Assembling;
using InverseProblem.Calculus;
using InverseProblem.SLAE;

//Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

//var gridBuilder2D = new GridBuilder2D();
//var grid = gridBuilder2D
//    .SetRAxis(new AxisSplitParameter(
//            new[] { 1d, 3d },
//            new UniformSplitter(2)
//        )
//    )
//    .SetZAxis(new AxisSplitParameter(
//            new[] { 1d, 3d },
//            new UniformSplitter(2)
//        )
//    )
//    .SetAreas(new Area[]
//        { new(0, new Node2D(1d, 1d), new Node2D(3d, 3d)) }
//    )
//    .SetSources(new Source[]
//        { new(new Node2D(1d, 1d), 1d) }
//    )
//    .Build();

//var materialFactory = new MaterialFactory
//(
//    new List<double> { 1d }
//);

//var localBasisFunctionsProvider = new LocalBasisFunctionsProvider(grid, new LinearFunctionsProvider());

//Func<Node2D, double> u = p => Math.Pow(p.Z, 1);

////var f = new RightPartParameter
////(
////    p => 0,
////    grid
////);

//var stiffnessMatrixTemplatesProvider = new StiffnessMatrixTemplatesProvider();
//var massMatrixTemplatesProvider = new MassMatrixTemplateProvider();

var derivativeCalculator = new DerivativeCalculator();

//var localAssembler =
//    new LocalAssembler(new LocalMatrixAssembler(grid, stiffnessMatrixTemplatesProvider, massMatrixTemplatesProvider),
//        materialFactory);

//var inserter = new Inserter();
//var globalAssembler = new GlobalAssembler<Node2D>(grid, new MatrixPortraitBuilder(), localAssembler, inserter,
//    new GaussExcluder(), localBasisFunctionsProvider);

//var firstBoundaryProvider = new FirstBoundaryProvider(grid, u);
//var secondBoundaryProvider = new SecondBoundaryProvider(grid, materialFactory, u, derivativeCalculator,
//    massMatrixTemplatesProvider);

//var firstConditions = firstBoundaryProvider.GetConditions(10, 40);
//var secondConditions = secondBoundaryProvider.GetConditions(10, 40);

//var firstSource = new Node2D(1.05, -11.5d);

//var equation = globalAssembler
//    .AssembleEquation()
//    .ApplySecondConditions(secondConditions)
//    .ApplySources()
//    .ApplyFirstConditions(firstConditions)
//    .BuildEquation();

//var preconditionMatrix = globalAssembler.BuildPreconditionMatrix();

//var lltPreconditioner = new LLTPreconditioner();

//var mcg = new MCG(lltPreconditioner, new LLTSparse());
//var solution = mcg.Solve(equation, preconditionMatrix);

//var femSolution = new FEMSolution(grid, solution, localBasisFunctionsProvider);

////var error = femSolution.CalcError(u);
////Console.WriteLine(error);

//var potentialDifference = femSolution.CalculatePotentialDifference(new Node2D(1.05, -10d), new Node2D(1.05, -13d));
//Console.WriteLine(potentialDifference);

var sourceA1 = new Source(new Node2D(0, -500), 1d);
var sourceB1 = new Source(new Node2D(100, -500), 1d);
var sourceA2 = new Source(new Node2D(0, 0), 2d);
var sourceB2 = new Source(new Node2D(100, 0), 2d);
var sourceA3 = new Source(new Node2D(0, 500), 3d);
var sourceB3 = new Source(new Node2D(100, 500), 3d);

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
    .SetInitialValues(1)
    .Solve();

Console.WriteLine();