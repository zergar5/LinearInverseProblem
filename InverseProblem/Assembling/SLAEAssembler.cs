using System.Numerics;
using System.Runtime.Intrinsics;
using DirectProblem.Core.Base;
using DirectProblem.Core.Global;
using DirectProblem.Core.GridComponents;
using Electrostatics.Calculus;
using Vector = DirectProblem.Core.Base.Vector;

namespace InverseProblem.Assembling;

public class SLAEAssembler
{
    private readonly PotentialDifferenceFunctionProvider _potentialDifferenceFunctionProvider;
    private readonly DerivativeCalculator _derivativeCalculator;
    private readonly (Source, Source)[] _sourcesLines;
    private readonly (Receiver, Receiver)[] _receiversLines;
    private readonly double _sigma;
    private double[] _truePotentialDifferences;
    private double[] _weightsSquares;
    private readonly Equation<Matrix> _equation;

    public double[] TrueCurrents { get; private set; }

    public SLAEAssembler
    (
        PotentialDifferenceFunctionProvider potentialDifferenceFunctionProvider,
        DerivativeCalculator derivativeCalculator,
        (Source, Source)[] sourcesLines,
        (Receiver, Receiver)[] receiversLines,
        double sigma
    )
    {
        _potentialDifferenceFunctionProvider = potentialDifferenceFunctionProvider;
        _derivativeCalculator = derivativeCalculator;
        _sourcesLines = sourcesLines;
        _receiversLines = receiversLines;
        _sigma = sigma;
        _equation = new Equation<Matrix>(new Matrix(sourcesLines.Length), new Vector(sourcesLines.Length),
            new Vector(sourcesLines.Length));
    }

    public SLAEAssembler SetInitialValues(double initialValue)
    {
        _equation.Solution.Fill(initialValue);

        return this;
    }

    public SLAEAssembler SetInitialValues(double[] initialValues)
    {
        if (initialValues.Length != _equation.Solution.Count)
        {
            throw new ArgumentOutOfRangeException(
                $"{nameof(_equation.Solution)} and {nameof(initialValues)} must have same size");
        }
        for (var i = 0; i < _equation.Solution.Count; i++)
        {
            _equation.Solution[i] = initialValues[i];
        }

        return this;
    }

    public SLAEAssembler CalculateTrueValues()
    {
        TrueCurrents = _sourcesLines.Select(l => l.Item1.Current).ToArray();
        CalculateTruePotentialDifferences();
        CalculateWeights();

        return this;
    }

    public Equation<Matrix> BuildEquation()
    {
        AssembleSLAE();
        return _equation;
    }

    private void CalculateTruePotentialDifferences()
    {
        _truePotentialDifferences = new double[_receiversLines.Length];

        for (var i = 0; i < _receiversLines.Length; i++)
        {
            _truePotentialDifferences[i] =
                _potentialDifferenceFunctionProvider.CreateForSeveralSources(_sourcesLines, _receiversLines[i], _sigma)
                (TrueCurrents);
        }
    }

    private void CalculateWeights()
    {
        _weightsSquares = new double[_truePotentialDifferences.Length];

        for (var i = 0; i < _receiversLines.Length; i++)
        {
            _weightsSquares[i] = Math.Pow(1d / _truePotentialDifferences[i], 2);
        }
    }

    private void AssembleSLAE()
    {
        AssembleMatrix();
        AssembleRightPart();
    }

    private void AssembleMatrix()
    {
        for (var q = 0; q < _equation.Matrix.CountRows; q++)
        {
            for (var s = 0; s < _equation.Matrix.CountColumns; s++)
            {
                _equation.Matrix[q, s] = 0;

                for (var i = 0; i < _receiversLines.Length; i++)
                {
                    var potentialDifferenceFunction =
                        _potentialDifferenceFunctionProvider.CreateForSeveralSources(_sourcesLines, _receiversLines[i], _sigma);
                    var derivativeByQ =
                        _derivativeCalculator.Calculate(potentialDifferenceFunction, _equation.Solution.Values, q);
                    var derivativeByS =
                        _derivativeCalculator.Calculate(potentialDifferenceFunction, _equation.Solution.Values, s);

                    _equation.Matrix[q, s] += _weightsSquares[i] * derivativeByQ * derivativeByS;
                }
            }
        }
    }

    private void AssembleRightPart()
    {
        for (var q = 0; q < _equation.Matrix.CountRows; q++)
        {
            _equation.RightPart[q] = 0;

            for (var i = 0; i < _receiversLines.Length; i++)
            {
                var potentialDifferenceFunction =
                    _potentialDifferenceFunctionProvider.CreateForSeveralSources(_sourcesLines, _receiversLines[i], _sigma);
                var derivativeByQ =
                    _derivativeCalculator.Calculate(potentialDifferenceFunction, _equation.Solution.Values, q);

                _equation.RightPart[q] -= _weightsSquares[i] *
                                          (potentialDifferenceFunction(_equation.Solution.Values) - _truePotentialDifferences[i]) *
                                          derivativeByQ;
            }
        }
    }
}