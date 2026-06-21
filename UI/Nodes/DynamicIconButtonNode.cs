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
public class DynamicIconButtonNode : CircleButtonNode
{
    private Action? _onClick;

    public DynamicIconButtonNode()
    {
        ImageNode.AddNodeFlags(NodeFlags.RespondToMouse | NodeFlags.HasCollision | NodeFlags.EmitsEvents);
        ImageNode.AddEvent(AtkEventType.MouseClick, () => _onClick?.Invoke());
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
