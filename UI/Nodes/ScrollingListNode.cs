using System;
using System.Collections.Generic;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;

namespace Echotools.UI.Nodes;

/// <summary>
/// Convenience wrapper that combines a <see cref="ScrollingNode{T}"/> with a
/// <see cref="VerticalListNode"/> for easy scrollable list layout.
///
/// This re-creates the <c>KamiToolKit.Nodes.ScrollingListNode</c> that was removed when
/// KamiToolKit replaced <c>ScrollingAreaNode&lt;T&gt;</c> with the generic
/// <see cref="ScrollingNode{T}"/>. The public surface matches the old type so consuming
/// plugins keep working unchanged.
/// </summary>
public class ScrollingListNode : SimpleComponentNode
{
    private readonly ReservedScrollingNode<VerticalListNode> listNode;

    public ScrollingListNode()
    {
        // FitContents lets the VerticalListNode size its own Height to fit its children.
        // The new ScrollingNode<T> derives the scroll range from ContentNode.Height (vs the
        // viewport height) — without this the content height stays 0, so no scrollbar shows
        // and scrolling does nothing. (The removed ScrollingAreaNode<T> tracked height via a
        // separate ContentHeight/FitToContentHeight path instead.) Matches KamiToolKit's own
        // ButtonListNode usage.
        listNode = new ReservedScrollingNode<VerticalListNode>
        {
            ContentNode = { FitContents = true },
        };
        listNode.AttachNode(this);
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        listNode.Size = Size;
        listNode.ContentNode.RecalculateLayout();
        listNode.RecalculateSizes();
    }

    public bool FitContents
    {
        get => listNode.ContentNode.FitContents;
        set => listNode.ContentNode.FitContents = value;
    }

    public bool FitWidth
    {
        get => listNode.ContentNode.FitWidth;
        set => listNode.ContentNode.FitWidth = value;
    }

    public VerticalListAnchor Anchor
    {
        get => listNode.ContentNode.Anchor;
        set => listNode.ContentNode.Anchor = value;
    }

    public VerticalListAlignment Alignment
    {
        get => listNode.ContentNode.Alignment;
        set => listNode.ContentNode.Alignment = value;
    }

    public bool ClipListContents
    {
        get => listNode.ContentNode.ClipListContents;
        set => listNode.ContentNode.ClipListContents = value;
    }

    public float ItemSpacing
    {
        get => listNode.ContentNode.ItemSpacing;
        set => listNode.ContentNode.ItemSpacing = value;
    }

    public float FirstItemSpacing
    {
        get => listNode.ContentNode.FirstItemSpacing;
        set => listNode.ContentNode.FirstItemSpacing = value;
    }

    public ICollection<NodeBase> InitialNodes
    {
        init => listNode.ContentNode.AddNode(value);
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

    public int ScrollPosition
    {
        get => listNode.ScrollBarNode.ScrollPosition;
        set => listNode.ScrollBarNode.ScrollPosition = value;
    }

    public float ContentWidth => listNode.ContentNode.Width;

    public IReadOnlyList<NodeBase> Nodes => listNode.ContentNode.Nodes;

    public IEnumerable<T> GetNodes<T>() where T : NodeBase => listNode.ContentNode.GetNodes<T>();

    public void RecalculateLayout()
    {
        listNode.ContentNode.RecalculateLayout();
        listNode.RecalculateSizes();
    }

    public void FitToContentHeight() => listNode.RecalculateSizes();

    public void AddNode(IEnumerable<NodeBase> nodes) => listNode.ContentNode.AddNode(nodes);

    public void AddNode(NodeBase? node) => listNode.ContentNode.AddNode(node);

    public void RemoveNode(params NodeBase[] nodes) => listNode.ContentNode.RemoveNode(nodes);

    public void RemoveNode(NodeBase node) => listNode.ContentNode.RemoveNode(node);

    public void AddDummy(float size = 0.0f) => listNode.ContentNode.AddDummy(size);

    public void Clear() => listNode.ContentNode.Clear();

    public void ReorderNodes(Comparison<NodeBase> comparison) => listNode.ContentNode.ReorderNodes(comparison);

    public VerticalListNode VerticalListNode => listNode.ContentNode;
}
