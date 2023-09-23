using DirectProblem.Core.GridComponents;

namespace DirectProblem.Core;

public class Grid<TPoint>
{
    public TPoint[] Nodes { get; }
    public Element[] Elements { get; }
    public Area[]? Areas { get; }
    public Source[]? Sources { get; }

    public IEnumerator<Element> GetEnumerator() => ((IEnumerable<Element>)Elements).GetEnumerator();

    public Grid(TPoint[] nodes, Element[] elements)
    {
        Nodes = nodes;
        Elements = elements;
    }

    public Grid(TPoint[] nodes, Element[] elements, Area[] areas) : this(nodes, elements)
    {
        Areas = areas;
    }

    public Grid(TPoint[] nodes, Element[] elements, Area[] areas, Source[] sources) : this(nodes, elements, areas)
    {
        Sources = sources;
    }
}