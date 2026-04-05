using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace Echotools.UI.Nodes;

/// <summary>
/// Pagination text label matching FFXIV's native style (e.g., crafting log "1-19").
/// Font: MiedingerMed, Size: 12, Alignment: Right.
/// Text Color: #9C6116 (warm brown), Edge Color: #FFB17D (light orange outline).
/// Uses TextFlags.Edge for outline rendering.
/// </summary>
public class PaginationLabel : TextNode
{
    public PaginationLabel()
    {
        Size = new Vector2(100, 20);
        FontType = FontType.MiedingerMed;
        FontSize = 12;
        AlignmentType = AlignmentType.Right;
        TextColor = new Vector4(0x9C / 255f, 0x61 / 255f, 0x16 / 255f, 1f);
        TextOutlineColor = new Vector4(0xFF / 255f, 0xB1 / 255f, 0x7D / 255f, 1f);
        RemoveTextFlags(TextFlags.Emboss);
        AddTextFlags(TextFlags.Edge);
    }
}
