using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace Echotools.UI.Nodes;

/// <summary>
/// Page navigation arrow button using the RecipeNoteBook texture (crafting log style).
/// Extends ButtonBase for proper click handling and timeline-based disabled state.
/// UV coordinates: (0, 36), Size: 36x26 from ui/uld/img01/RecipeNoteBook.tex.
/// </summary>
public unsafe class PaginationArrowButton : ButtonBase
{
    public readonly ImageNode ArrowImage;

    public PaginationArrowButton(PaginationArrowDirection direction)
    {
        ArrowImage = new ImageNode
        {
            Size = new Vector2(36.0f, 26.0f),
        };
        ArrowImage.AddPart(new Part
        {
            TexturePath = "ui/uld/img01/RecipeNoteBook.tex",
            TextureCoordinates = new Vector2(0.0f, 36.0f),
            Size = new Vector2(36.0f, 26.0f),
            Id = 0,
        });
        ArrowImage.PartId = 0;

        if (direction == PaginationArrowDirection.Right)
            ArrowImage.ImageNodeFlags = ImageNodeFlags.FlipH;

        ArrowImage.AttachNode(this);

        LoadTwoPartTimelines(this, ArrowImage);

        InitializeComponentEvents();
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();
        ArrowImage.Size = Size;
    }
}

public enum PaginationArrowDirection
{
    Left,
    Right
}
