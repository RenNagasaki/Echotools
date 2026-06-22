using System;
using KamiToolKit.BaseTypes;
using KamiToolKit.Nodes;

namespace Echotools.UI.Nodes;

/// <summary>
/// A <see cref="ScrollingNode{T}"/> that reserves horizontal space on the right for the
/// scrollbar, so list/tree content is not rendered underneath it.
///
/// The removed <c>ScrollingAreaNode&lt;T&gt;</c> sized its content area to <c>Width - 16</c>;
/// the new <see cref="ScrollingNode{T}"/> instead sizes <c>ContentNode</c> to the full width and
/// draws the 8px scrollbar on top of the right edge. This subclass restores the old behaviour by
/// shrinking <c>ContentNode.Width</c> after each size change, so FitWidth items / tree categories
/// stop short of the scrollbar.
/// </summary>
public class ReservedScrollingNode<T> : ScrollingNode<T> where T : NodeBase, new()
{
    /// <summary>Horizontal space reserved on the right for the scrollbar (matches the old 16px).</summary>
    public const float ScrollBarReserve = 16f;

    /// <inheritdoc />
    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();
        ContentNode.Width = Math.Max(0f, Width - ScrollBarReserve);
    }
}
