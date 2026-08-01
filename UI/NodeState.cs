using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.BaseTypes.ComponentNode;

namespace Echotools.UI;

/// <summary>
/// Enabled/disabled state for KamiToolKit nodes — the one place that makes "greyed out" mean
/// "unusable".
///
/// <para><b>Why this exists.</b> Lowering a node's alpha is purely cosmetic: ATK keeps delivering
/// hover, press animation and click to it, so a control that only *looks* disabled still fires its
/// handler. Every plugin that dims controls hit this, so the rule lives here rather than in one
/// plugin's UI helpers.</para>
///
/// <para><b>How it disables.</b> Three mechanisms, in this order:</para>
/// <list type="number">
/// <item>A node implementing <see cref="INodeEnableState"/> disables itself — the escape hatch for
/// input paths neither of the following covers.</item>
/// <item>Component nodes (buttons, checkboxes, sliders, text inputs, dropdowns — anything deriving
/// from <see cref="ComponentNode"/>) get the game's own disabled state via
/// <c>AtkComponentBase.SetEnabledState</c>. That is what the game itself uses, so the control
/// greys out in the native style, stops reacting to the mouse and drops keyboard focus.</item>
/// <item>Plain nodes (an <c>ImageNode</c> with a collision area, a hand-built clickable row) have no
/// component, so instead the flags that make them react to the mouse are taken away and later given
/// back — <b>exactly</b> the ones that were present, never a superset, so a node never gains
/// interactivity it did not have.</item>
/// </list>
///
/// <para><b>Writes happen only on a state change.</b> Callers drive this from <c>OnUpdate</c> for
/// dozens of nodes per window, and repeating native ATK writes every frame is the per-frame node
/// mutation the Dalamud conventions warn about (setting node flags every frame can crash the game).
/// The last applied state is therefore remembered per node here, deliberately <i>not</i> read back
/// off the node: the native disabled state may change the node's own appearance, which would make an
/// alpha-derived "did it change?" comparison flip on every frame.</para>
/// </summary>
public static unsafe class NodeState
{
    /// <summary>Alpha of an enabled node.</summary>
    public const float EnabledAlpha = 1.0f;

    /// <summary>Alpha of a disabled node. Matches the shade the plugins already used for dimming.</summary>
    public const float DisabledAlpha = 0.4f;

    /// <summary>
    /// The flags that make a node without a component react to the mouse. Only these are ever taken
    /// away when such a node is disabled.
    /// </summary>
    public const NodeFlags InteractiveFlags =
        NodeFlags.RespondToMouse | NodeFlags.HasCollision | NodeFlags.EmitsEvents;

    /// <summary>
    /// Which of <see cref="InteractiveFlags"/> a node currently carries — i.e. what disabling it has
    /// to remove and enabling it has to restore. Pure, so the rule is testable without the game.
    /// </summary>
    public static NodeFlags InteractiveFlagsOf(NodeFlags current) => current & InteractiveFlags;

    /// <summary>
    /// Enables or disables <paramref name="node"/>: dims it <b>and</b> makes it actually unusable.
    /// Null-safe and idempotent — repeated calls with an unchanged value write nothing.
    /// </summary>
    /// <param name="node">The node to update. Null is ignored.</param>
    /// <param name="enabled">Whether the node should be usable.</param>
    /// <param name="disabledAlpha">Alpha to dim to. Pass <see cref="EnabledAlpha"/> for a control
    /// that must stay fully visible while being inert.</param>
    public static void SetEnabled(NodeBase? node, bool enabled, float disabledAlpha = DisabledAlpha)
    {
        if (node is null) return;

        // Nodes are created enabled and fully opaque, so that is the assumed starting state.
        var state = States.GetValue(node, _ => new NodeDimState());
        if (state.Enabled == enabled) return;
        state.Enabled = enabled;

        node.Alpha = enabled ? EnabledAlpha : disabledAlpha;

        if (node is INodeEnableState self)
        {
            self.ApplyEnabled(enabled);
            return;
        }

        if (node is ComponentNode component)
        {
            component.ComponentBase->SetEnabledState(enabled);
            return;
        }

        if (enabled)
        {
            if (state.RemovedFlags == 0) return;
            node.AddNodeFlags(state.RemovedFlags);
            state.RemovedFlags = 0;
        }
        else
        {
            state.RemovedFlags = InteractiveFlagsOf(node.NodeFlags);
            if (state.RemovedFlags != 0) node.RemoveNodeFlags(state.RemovedFlags);
        }
    }

    /// <summary>
    /// Whether <see cref="SetEnabled"/> last left this node enabled. A node it has never touched
    /// counts as enabled, which is how KamiToolKit creates them.
    /// </summary>
    public static bool IsEnabled(NodeBase? node)
        => node is null || !States.TryGetValue(node, out var state) || state.Enabled;

    /// <summary>Last state applied per node. Weak keys — a disposed node drops out on its own, so
    /// this never keeps an ATK node alive.</summary>
    private static readonly ConditionalWeakTable<NodeBase, NodeDimState> States = new();

    private sealed class NodeDimState
    {
        public bool Enabled = true;

        /// <summary>What was taken away from a plain node, so enabling restores exactly that.</summary>
        public NodeFlags RemovedFlags;
    }
}
