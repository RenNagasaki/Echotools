namespace Echotools.Logging.DataClasses;

/// <summary>
/// Per-source filter preferences. Serializable — embed one per <see cref="Echotools.Logging.Enums.TextSource"/>
/// your plugin uses inside your plugin's Configuration class.
/// </summary>
public class LogSourceConfig
{
    public bool ShowDebugLog { get; set; } = true;
    public bool ShowErrorLog { get; set; } = true;
    public bool ShowId0 { get; set; } = true;
    public bool JumpToBottom { get; set; } = true;
}
