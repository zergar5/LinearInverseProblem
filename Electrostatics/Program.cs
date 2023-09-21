using Electrostatics.Calculus;
using Electrostatics.Core.Global;
using Electrostatics.Core.GridComponents;
using Electrostatics.FEM;
using Electrostatics.GridGenerator;
using Electrostatics.GridGenerator.Area.Splitting;
using Electrostatics.SLAE.Preconditions.LLT;
using Electrostatics.SLAE.Solvers;
using Electrostatics.TwoDimensional;
using Electrostatics.TwoDimensional.Assembling;
using Electrostatics.TwoDimensional.Assembling.Boundary;
using Electrostatics.TwoDimensional.Assembling.Global;
using Electrostatics.TwoDimensional.Assembling.Local;
using Electrostatics.TwoDimensional.Parameters;
using System.Globalization;
using Electrostatics.TwoDimensional.Assembling.MatrixTemplates;
using static System.Math;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var gridBuilder2D = new GridBuilder2D();
var grid = gridBuilder2D
    .SetRAxis(new AxisSplitParameter(
            new[] { 1d, 1.1d, 1.101d, 15d },
            new UniformSplitter(2),
            new UniformSplitter(1),
            new UniformSplitter(7)
        )
    )
    .SetZAxis(new AxisSplitParameter(
            new[] { -13d, -10d, -7d, -4d, -1d },
            new ProportionalSplitter(4, 0.7),
            new ProportionalSplitter(16, 0.9),
            new ProportionalSplitter(16, 1.1),
            new ProportionalSplitter(4, 1.3)
        )
    )
    .SetMaterials(new []
    {
        0, 0, 1, 2, 2, 2, 2, 2, 2, 2, //0
        0, 0, 1, 2, 2, 2, 2, 2, 2, 2,
        0, 0, 1, 2, 2, 2, 2, 2, 2, 2,
        0, 0, 1, 2, 2, 2, 2, 2, 2, 2, //3
        0, 0, 1, 3, 3, 4, 4, 4, 4, 4, //0
        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
        0, 0, 1, 3, 3, 4, 4, 4, 4, 4, // 15
        0, 0, 1, 4, 4, 3, 3, 3, 3, 3, //0
        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
        0, 0, 1, 4, 4, 3, 3, 3, 3, 3, // 15
        0, 0, 1, 2, 2, 2, 2, 2, 2, 2, //0
        0, 0, 1, 2, 2, 2, 2, 2, 2, 2,
        0, 0, 1, 2, 2, 2, 2, 2, 2, 2,
        0, 0, 1, 2, 2, 2, 2, 2, 2, 2, //3
    })
    .Build();

//var grid = gridBuilder2D
//    .SetRAxis(new AxisSplitParameter(
//            new[] { 1d, 2d },
//            new UniformSplitter(2)
//        )
//    )
//    .SetZAxis(new AxisSplitParameter(
//            new[] { 1d, 2d },
//            new UniformSplitter(2)
//        )
//    )
//    .Build();

var materialFactory = new MaterialFactory
(
    new List<double> { 1d, 0.9, 0.5, 0.1, 0.25 }
);

var localBasisFunctionsProvider = new LocalBasisFunctionsProvider(grid, new LinearFunctionsProvider());

//Func<Node2D, double> u = p => Pow(p.Z, 1);

//var f = new RightPartParameter
//(
//    p => 0,
//    grid
//);

var derivativeCalculator = new DerivativeCalculator();

var localAssembler = new LocalAssembler(grid, localBasisFunctionsProvider, materialFactory,
    new DoubleIntegralCalculator(), derivativeCalculator);

var inserter = new Inserter();
var globalAssembler = new GlobalAssembler<Node2D>(grid, new MatrixPortraitBuilder(), localAssembler, inserter,
    new GaussExcluder(), localBasisFunctionsProvider);

var firstBoundaryProvider = new FirstBoundaryProvider(grid);
var secondBoundaryProvider = new SecondBoundaryProvider(grid, materialFactory, derivativeCalculator,
    new MatrixRTemplateProvider(), new MatrixZTemplateProvider());

var firstConditions = firstBoundaryProvider.GetConditions(10, 40);
var secondConditions = secondBoundaryProvider.GetConditions(10, 40);

var firstSource = new Node2D(1.05, -11.5d);

var equation = globalAssembler
    .AssembleEquation()
    .ApplySecondConditions(secondConditions)
    .SetSource(firstSource, 10d)
    .ApplyFirstConditions(firstConditions)
    .BuildEquation();

var preconditionMatrix = globalAssembler.BuildPreconditionMatrix();

var luPreconditioner = new LLTPreconditioner();

var mcg = new MCG(luPreconditioner, new LLTSparse(luPreconditioner));
var solution = mcg.Solve(equation, preconditionMatrix);

var femSolution = new FEMSolution(grid, solution, localBasisFunctionsProvider);

//var error = femSolution.CalcError(u);
//Console.WriteLine(error);

var potentialDifference = femSolution.CalculatePotentialDifference(new Node2D(1.05, -10d), new Node2D(1.05, -13d));
Console.WriteLine(potentialDifference);