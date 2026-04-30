using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Simple;

namespace Echotools.UI.Nodes;

/// <summary>
/// RecipeNote-style list item with an icon, title text, and subtitle text.
/// Uses GearSetList texture for hover/selected backgrounds.
/// Built on top of <see cref="ListButtonNode"/> for click/select behavior.
/// </summary>
public unsafe class IconListItemNode : ListButtonNode
{
    public readonly TextNode SubtitleNode;
    public readonly IconNode ItemIconNode;

    private const float IconAreaWidth = 42f;
    private const float TextLeftPadding = 48f;
    private const float TitleYOffset = 7f;
    private const float SubtitleY = 22f;
    private const float IconX = 2f;
    private const float IconY = -2f;
    private const float IconSize = 30f;

    // FFXIV journal-style text colors
    private static readonly Vector4 TitleColor = new(0.49f, 0.32f, 0.23f, 1f);       // 7D523B
    private static readonly Vector4 SubtitleColor = new(0.67f, 0.47f, 0.32f, 1f);     // AB7852
    private static readonly Vector4 TextOutlineColor = new(1f, 0.95f, 0.91f, 1f);     // FFF3E7

    public IconListItemNode()
    {
        // Restyle backgrounds to GearSetList texture
        if (HoverBackgroundNode is SimpleNineGridNode hoverBg)
        {
            hoverBg.TexturePath = "ui/uld/GearSetList.tex";
            hoverBg.TextureCoordinates = new Vector2(56, 28);
            hoverBg.TextureSize = new Vector2(28, 28);
            hoverBg.LeftOffset = 10;
            hoverBg.RightOffset = 10;
            hoverBg.TopOffset = 10;
            hoverBg.BottomOffset = 10;
        }

        if (SelectedBackgroundNode is SimpleNineGridNode selBg)
        {
            selBg.TexturePath = "ui/uld/GearSetList.tex";
            selBg.TextureCoordinates = new Vector2(84, 0);
            selBg.TextureSize = new Vector2(28, 28);
            selBg.LeftOffset = 10;
            selBg.RightOffset = 10;
            selBg.TopOffset = 10;
            selBg.BottomOffset = 10;
        }

        // Restyle the inherited LabelNode as title
        LabelNode.Position = new Vector2(TextLeftPadding, TitleYOffset);
        LabelNode.FontSize = 14;
        LabelNode.TextColor = TitleColor;
        LabelNode.TextOutlineColor = TextOutlineColor;

        // Add subtitle text
        SubtitleNode = new TextNode
        {
            Position = new Vector2(TextLeftPadding, SubtitleY),
            FontType = FontType.Axis,
            FontSize = 12,
            String = "",
            TextColor = SubtitleColor,
            TextOutlineColor = TextOutlineColor,
        };
        SubtitleNode.AttachNode(this);

        // Add icon (no border frame)
        ItemIconNode = new IconNode
        {
            Position = new Vector2(IconX, IconY),
            Size = new Vector2(IconSize, IconSize),
        };
        ItemIconNode.IconExtras.IsVisible = false;
        ItemIconNode.AttachNode(this);
    }

    /// <summary>Title text (top line).</summary>
    public string Title
    {
        get => LabelNode.String.ExtractText();
        set => LabelNode.String = value;
    }

    /// <summary>Subtitle text (bottom line, smaller).</summary>
    public string Subtitle
    {
        get => SubtitleNode.String.ExtractText();
        set => SubtitleNode.String = value;
    }

    /// <summary>Icon ID (FFXIV game icon).</summary>
    public uint IconId
    {
        get => ItemIconNode.IconId;
        set => ItemIconNode.IconId = value;
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        // Position backgrounds to the right of the icon area
        HoverBackgroundNode.Position = new Vector2(IconAreaWidth, 0);
        HoverBackgroundNode.Size = new Vector2(Width - IconAreaWidth, Height);
        SelectedBackgroundNode.Position = new Vector2(IconAreaWidth, 0);
        SelectedBackgroundNode.Size = new Vector2(Width - IconAreaWidth, Height);

        // Re-apply title position (base.OnSizeChanged resets LabelNode to default)
        var textWidth = Width - TextLeftPadding - 4;
        LabelNode.Position = new Vector2(TextLeftPadding, TitleYOffset);
        LabelNode.Size = new Vector2(textWidth, 16);
        SubtitleNode.Size = new Vector2(textWidth, 14);
    }
}
