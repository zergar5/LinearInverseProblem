﻿using Electrostatics.Core.GridComponents;

namespace Electrostatics.Core;

public class Grid<TPoint>
{
    public TPoint[] Nodes { get; }
    public Element[] Elements { get; }

    public IEnumerator<Element> GetEnumerator() => ((IEnumerable<Element>)Elements).GetEnumerator();

    public Grid(IEnumerable<TPoint> nodes, IEnumerable<Element> elements)
    {
        Nodes = nodes.ToArray();
        Elements = elements.ToArray();
    }
}