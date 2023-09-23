namespace DirectProblem.Core.GridComponents;

public class Area
{
    public List<int> ElementsIndexes { get; }

    public int MaterialId { get; private set; }
    public Node2D LowerLeftCorner { get; private set; }
    public Node2D UpperRightCorner { get; private set; }

    public Area(int materialId, Node2D lowerLeftCorner, Node2D upperRightCorner)
    {
        MaterialId = materialId;
        LowerLeftCorner = lowerLeftCorner;
        UpperRightCorner = upperRightCorner;
    }

    public Area(List<int> elementsIndexes, Node2D lowerLeftCorner, Node2D upperRightCorner)
    {
        ElementsIndexes = elementsIndexes;
        LowerLeftCorner = lowerLeftCorner;
        UpperRightCorner = upperRightCorner;
    }

    public void Add(int index)
    {
        ElementsIndexes.Add(index);
    }

    public bool AreaHas(Node2D elementLowerLeftCorner, Node2D elementUpperRightCorner)
    {
        return elementLowerLeftCorner.R >= LowerLeftCorner.R && elementLowerLeftCorner.Z >= LowerLeftCorner.Z &&
               elementUpperRightCorner.R <= UpperRightCorner.R && elementUpperRightCorner.Z <= UpperRightCorner.Z;
    }
}