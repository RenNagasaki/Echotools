namespace Echotools.Logging.Enums;

/// <summary>
/// Identifies the origin of a log entry. Plugins use whichever values apply;
/// unused values are simply never written to and cost nothing.
/// Extend with new values if a future plugin needs additional sources.
/// </summary>
public enum TextSource
{
    None,
    Chat,
    AddonTalk,
    AddonBattleTalk,
    AddonSelectString,
    AddonCutsceneSelectString,
    AddonBubble,
    VoiceTest,
    Backend,
    Sync,
    CrowdGeneral,
    CrowdSync,
    CrowdAdmin,
}
