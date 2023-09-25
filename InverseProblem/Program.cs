using DirectProblem.Core.GridComponents;
using DirectProblem.FEM;
using DirectProblem.GridGenerator;
using DirectProblem.GridGenerator.Intervals.Splitting;
using DirectProblem.SLAE.Preconditions;
using DirectProblem.SLAE.Solvers;
using DirectProblem.TwoDimensional.Assembling;
using DirectProblem.TwoDimensional.Assembling.Boundary;
using DirectProblem.TwoDimensional.Assembling.Global;
using DirectProblem.TwoDimensional.Assembling.Local;
using DirectProblem.TwoDimensional.Assembling.MatrixTemplates;
using DirectProblem.TwoDimensional.Parameters;
using Electrostatics.Calculus;
using Electrostatics.TwoDimensional;
using Electrostatics.TwoDimensional.Assembling.Global;
using System.Globalization;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var gridBuilder2D = new GridBuilder2D();
var grid = gridBuilder2D
    .SetRAxis(new AxisSplitParameter(
            new[] { 1d, 3d },
            new UniformSplitter(2)
        )
    )
    .SetZAxis(new AxisSplitParameter(
            new[] { 1d, 3d },
            new UniformSplitter(2)
        )
    )
    .SetAreas(new Area[]
        { new(0, new Node2D(1d, 1d), new Node2D(3d, 3d)) }
    )
    .SetSources(new Source[]
        { new(new Node2D(1d, 1d), 1d) }
    )
    .Build();

var materialFactory = new MaterialFactory
(
    new List<double> { 1d }
);

var localBasisFunctionsProvider = new LocalBasisFunctionsProvider(grid, new LinearFunctionsProvider());

Func<Node2D, double> u = p => Math.Pow(p.Z, 1);

//var f = new RightPartParameter
//(
//    p => 0,
//    grid
//);

var stiffnessMatrixTemplatesProvider = new StiffnessMatrixTemplatesProvider();
var massMatrixTemplatesProvider = new MassMatrixTemplateProvider();

var derivativeCalculator = new DerivativeCalculator();

var localAssembler =
    new LocalAssembler(new LocalMatrixAssembler(grid, stiffnessMatrixTemplatesProvider, massMatrixTemplatesProvider),
        materialFactory);

var inserter = new Inserter();
var globalAssembler = new GlobalAssembler<Node2D>(grid, new MatrixPortraitBuilder(), localAssembler, inserter,
    new GaussExcluder(), localBasisFunctionsProvider);

var firstBoundaryProvider = new FirstBoundaryProvider(grid, u);
var secondBoundaryProvider = new SecondBoundaryProvider(grid, materialFactory, u, derivativeCalculator,
    massMatrixTemplatesProvider);

var firstConditions = firstBoundaryProvider.GetConditions(10, 40);
var secondConditions = secondBoundaryProvider.GetConditions(10, 40);

var firstSource = new Node2D(1.05, -11.5d);

var equation = globalAssembler
    .AssembleEquation()
    .ApplySecondConditions(secondConditions)
    .ApplySources()
    .ApplyFirstConditions(firstConditions)
    .BuildEquation();

var preconditionMatrix = globalAssembler.BuildPreconditionMatrix();

var lltPreconditioner = new LLTPreconditioner();

var mcg = new MCG(lltPreconditioner, new LLTSparse());
var solution = mcg.Solve(equation, preconditionMatrix);

var femSolution = new FEMSolution(grid, solution, localBasisFunctionsProvider);

//var error = femSolution.CalcError(u);
//Console.WriteLine(error);

var potentialDifference = femSolution.CalculatePotentialDifference(new Node2D(1.05, -10d), new Node2D(1.05, -13d));
Console.WriteLine(potentialDifference);