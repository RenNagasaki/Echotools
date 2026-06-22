using System.Collections.Generic;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;

namespace Echotools.UI.Nodes;

/// <summary>
/// Convenience wrapper that combines a <see cref="ScrollingNode{T}"/> with a
/// <see cref="TreeListNode"/> for easy scrollable journal/quest-log style tree layout.
///
/// This re-creates the <c>KamiToolKit.Nodes.ScrollingTreeNode</c> that was removed when
/// KamiToolKit replaced <c>ScrollingAreaNode&lt;T&gt;</c> with the generic
/// <see cref="ScrollingNode{T}"/>. The public surface matches the old type so consuming
/// plugins keep working unchanged.
/// </summary>
public class ScrollingTreeNode : SimpleComponentNode
{
    private readonly ReservedScrollingNode<TreeListNode> listNode;

    public ScrollingTreeNode()
    {
        listNode = new ReservedScrollingNode<TreeListNode>();

        // TreeListNode accumulates the total category height on an internal childContainer but
        // never updates its OWN Height. The new ScrollingNode<T> derives the scroll range from
        // ContentNode.Height (vs the viewport), so mirror the layout height onto the node via
        // OnLayoutUpdate — otherwise the height stays 0 and no scrollbar shows / scrolling does
        // nothing. (The removed ScrollingAreaNode<T> tracked this via a separate ContentHeight.)
        listNode.ContentNode.OnLayoutUpdate = height => listNode.ContentNode.Height = height;

        listNode.AttachNode(this);
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        listNode.Size = Size;
        RecalculateLayout();
    }

    public float CategoryVerticalSpacing
    {
        get => listNode.ContentNode.CategoryVerticalSpacing;
        set => listNode.ContentNode.CategoryVerticalSpacing = value;
    }

    public bool AutoHideScrollBar
    {
        get => listNode.AutoHideScrollBar;
        set => listNode.AutoHideScrollBar = value;
    }

    public int ScrollSpeed
    {
        get => listNode.ScrollSpeed;
        set => listNode.ScrollSpeed = value;
    }

    public IReadOnlyList<TreeListCategoryNode> CategoryNodes => listNode.ContentNode.CategoryNodes;

    public void RecalculateLayout()
    {
        listNode.ContentNode.RefreshLayout();
        listNode.RecalculateSizes();
    }

    public void AddCategoryNode(TreeListCategoryNode node) => listNode.ContentNode.AddCategoryNode(node);

    public TreeListNode TreeListNode => listNode.ContentNode;
}
