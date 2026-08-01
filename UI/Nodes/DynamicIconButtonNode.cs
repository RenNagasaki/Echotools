using System;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace Echotools.UI.Nodes;

/// <summary>
/// A circle icon button whose icon, tooltip, and click handler can be swapped at runtime.
/// Extends KamiToolKit's CircleButtonNode with convenience properties for dynamic state changes.
/// Uses MouseClick on the ImageNode for reliable click detection in both NativeAddon windows
/// and addon-attached contexts (e.g., Talk addon).
/// </summary>
public class DynamicIconButtonNode : CircleButtonNode, INodeEnableState
{
    private Action? _onClick;

    /// <summary>What makes the <see cref="ImageNode"/> receive the click this button runs on.</summary>
    private const NodeFlags ClickFlags =
        NodeFlags.RespondToMouse | NodeFlags.HasCollision | NodeFlags.EmitsEvents;

    public DynamicIconButtonNode()
    {
        ImageNode.AddNodeFlags(ClickFlags);
        ImageNode.AddEvent(AtkEventType.MouseClick, () => _onClick?.Invoke());
    }

    /// <summary>
    /// Disables the button for <see cref="NodeState"/>. The component's own disabled state is not
    /// enough here: clicks arrive as a raw <c>MouseClick</c> on the <see cref="ImageNode"/> (see
    /// <see cref="OnClick"/>), which the component knows nothing about — so that node's mouse flags
    /// have to go too, or a "disabled" button would keep firing.
    /// </summary>
    public void ApplyEnabled(bool enabled)
    {
        IsEnabled = enabled;
        if (enabled) ImageNode.AddNodeFlags(ClickFlags);
        else ImageNode.RemoveNodeFlags(ClickFlags);
    }

    /// <summary>
    /// Click handler. Shadows ButtonBase.OnClick to route through MouseClick on the ImageNode,
    /// which fires reliably in all contexts (ButtonClick only works in component-owned addons).
    /// </summary>
    public new Action? OnClick
    {
        get => _onClick;
        set => _onClick = value;
    }

    public string Tooltip
    {
        get => TextTooltip.ToString();
        set => TextTooltip = value;
    }

    /// <summary>
    /// Atomically swap icon, tooltip, and click handler in one call.
    /// </summary>
    public void SetState(CircleButtonIcon icon, string tooltip, Action? onClick)
    {
        Icon = icon;
        Tooltip = tooltip;
        OnClick = onClick;
    }
}
