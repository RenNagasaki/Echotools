using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Simple;

namespace Echotools.UI.Nodes;

/// <summary>
/// Beast-tribe reputation style progress bar with background track, fill bar,
/// a right-aligned count label, and an optional left-aligned action label.
/// Uses Character.tex NineGrid textures.
/// </summary>
public class StatusProgressBar : ResNode
{
    public readonly NineGridNode BackgroundNode;
    public readonly NineGridNode FillNode;
    public readonly TextNode CountLabel;
    public readonly TextNode ActionLabel;

    private float _maxFillWidth;

    public StatusProgressBar()
    {
        BackgroundNode = new SimpleNineGridNode
        {
            TexturePath = "ui/uld/img01/Character.tex",
            TextureCoordinates = new Vector2(128, 160),
            TextureSize = new Vector2(12, 12),
            LeftOffset = 5,
            RightOffset = 5,
        };
        BackgroundNode.AttachNode(this);

        FillNode = new SimpleNineGridNode
        {
            Size = new Vector2(0, 8),
            TexturePath = "ui/uld/img01/Character.tex",
            TextureCoordinates = new Vector2(240, 94),
            TextureSize = new Vector2(16, 8),
            LeftOffset = 5,
            RightOffset = 5,
        };
        FillNode.AttachNode(this);

        CountLabel = new TextNode
        {
            FontType = FontType.Axis,
            FontSize = 12,
            AlignmentType = AlignmentType.Right,
            String = "",
        };
        CountLabel.AttachNode(this);

        ActionLabel = new TextNode
        {
            FontType = FontType.Axis,
            FontSize = 12,
            AlignmentType = AlignmentType.Left,
            String = "",
            IsVisible = false,
        };
        ActionLabel.AttachNode(this);
    }

    /// <summary>
    /// Set the progress (0.0 to 1.0) and count text.
    /// </summary>
    public void SetProgress(float fraction, string countText)
    {
        fraction = Math.Clamp(fraction, 0f, 1f);
        FillNode.Size = new Vector2(_maxFillWidth * fraction, 8);
        CountLabel.String = countText;
    }

    /// <summary>
    /// Show or hide the action label (e.g., "Generating voice clips...").
    /// </summary>
    public string ActionText
    {
        get => ActionLabel.String.ExtractText();
        set
        {
            ActionLabel.String = value;
            ActionLabel.IsVisible = !string.IsNullOrEmpty(value);
        }
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        var barHeight = 12f;
        var fillHeight = 8f;
        var labelY = barHeight + 2;

        _maxFillWidth = Width - 4;

        BackgroundNode.Position = new Vector2(0, 0);
        BackgroundNode.Size = new Vector2(Width, barHeight);

        FillNode.Position = new Vector2(2, 0);
        // Keep current fill width ratio
        FillNode.Size = new Vector2(FillNode.Width, fillHeight);

        CountLabel.Position = new Vector2(0, labelY);
        CountLabel.Size = new Vector2(Width, 14);

        ActionLabel.Position = new Vector2(0, labelY);
        ActionLabel.Size = new Vector2(Width, 14);
    }
}
