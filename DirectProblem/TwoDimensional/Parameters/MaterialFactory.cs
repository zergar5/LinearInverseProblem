﻿using DirectProblem.Core.GridComponents;

namespace DirectProblem.TwoDimensional.Parameters;

public class MaterialFactory
{
    private readonly Dictionary<int, double> _sigmas;

    public MaterialFactory(IEnumerable<double> sigmas)
    {
        _sigmas = sigmas.Select((value, index) => new KeyValuePair<int, double>(index, value))
            .ToDictionary(index => index.Key, value => value.Value);
    }

    public Material GetById(int id)
    {
        return new Material(
            _sigmas[id]
        );
    }
}