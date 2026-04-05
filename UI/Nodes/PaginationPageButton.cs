using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace Echotools.UI.Nodes;

/// <summary>
/// Page number button for pagination, styled to match FFXIV's native pagination.
/// Uses PaginationLabel for consistent text styling.
/// Active (current) page is visually distinct and not clickable.
/// </summary>
public unsafe class PaginationPageButton : ButtonBase
{
    public readonly PaginationLabel LabelNode;

    private bool _isCurrentPage;

    private static readonly Vector4 ActiveTextColor = new(1f, 1f, 1f, 1f);
    private static readonly Vector4 ActiveEdgeColor = new(0x9C / 255f, 0x61 / 255f, 0x16 / 255f, 1f);

    public PaginationPageButton()
    {
        LabelNode = new PaginationLabel
        {
            Size = new Vector2(24, 20),
            AlignmentType = AlignmentType.Center,
        };
        LabelNode.AttachNode(this);

        LoadTwoPartTimelines(this, LabelNode);
        InitializeComponentEvents();
    }

    public int PageIndex { get; set; }

    public bool IsCurrentPage
    {
        get => _isCurrentPage;
        set
        {
            _isCurrentPage = value;
            if (value)
            {
                LabelNode.TextColor = ActiveTextColor;
                LabelNode.TextOutlineColor = ActiveEdgeColor;
            }
            else
            {
                // Restore default PaginationLabel colors
                LabelNode.TextColor = new Vector4(0x9C / 255f, 0x61 / 255f, 0x16 / 255f, 1f);
                LabelNode.TextOutlineColor = new Vector4(0xFF / 255f, 0xB1 / 255f, 0x7D / 255f, 1f);
            }
            IsEnabled = !value;
        }
    }

    public string String
    {
        get => LabelNode.String.ToString();
        set => LabelNode.String = value;
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();
        LabelNode.Size = Size;
    }
}
