namespace Echotools.UI;

/// <summary>
/// Implemented by a node that knows better than <see cref="NodeState"/> how to make itself inert.
///
/// <para><see cref="NodeState.SetEnabled"/> handles the two general cases — a component node gets the
/// game's disabled state, a plain node loses its mouse flags. A node whose input does not arrive
/// through either of those paths (for example a button component that listens for a raw
/// <c>MouseClick</c> on a child node) must say so itself, or it would look disabled and keep firing.
/// Implement this instead of adding the special case to <see cref="NodeState"/>.</para>
///
/// <para><see cref="NodeState"/> already dimmed the node and only calls this on an actual state
/// change, so implementations handle input only and need no idempotence guard of their own.</para>
/// </summary>
public interface INodeEnableState
{
    /// <summary>Make this node accept or reject input. Alpha is not this method's business.</summary>
    void ApplyEnabled(bool enabled);
}
