# Echotools

Shared libraries for [Echotools](https://github.com/RenNagasaki) Dalamud plugins.

## Projects

### Logging (`Echotools.Logging`)

Thread-safe logging service designed for Dalamud plugins. Uses a dual-buffer pattern (ConcurrentBag → List) so worker threads can log freely while the UI reads safely on the main thread.

#### Features

- **Thread-safe** — log from any thread without locks
- **Per-source routing** — each `TextSource` gets its own log stream
- **Dual-buffer sync** — call `UpdateMainThreadLogs()` once per frame to drain worker-thread logs into main-thread lists
- **Event-driven UI updates** — subscribe to `LogUpdated` to know when new entries arrive
- **Dalamud integration** — all entries are also forwarded to Dalamud's `IPluginLog`
- **Tracked events** — `Start()`/`End()` pairs with auto-incrementing IDs for tracing operations

#### Setup

Add this repo as a git submodule in your plugin:

```bash
git submodule add https://github.com/RenNagasaki/Echotools.git Echotools
```

Reference the project in your `.csproj`:

```xml
<ProjectReference Include="..\Echotools\Logging\Logging.csproj" />
```

#### Usage

```csharp
using Echotools.Logging.DataClasses;
using Echotools.Logging.Enums;
using Echotools.Logging.Services;

// Create the service (pass Dalamud's IPluginLog)
var log = new LogService(pluginLog);

// Simple logging
var eventId = new EKEventId(0, TextSource.None);
log.Info(nameof(MyMethod), "Something happened", eventId);
log.Debug(nameof(MyMethod), "Debug details", eventId);
log.Error(nameof(MyMethod), "Something went wrong", eventId);

// Tracked events (auto-incrementing ID)
var eid = log.Start(nameof(MyMethod), TextSource.AddonTalk);
log.Info(nameof(MyMethod), "Processing dialogue", eid);
log.End(nameof(MyMethod), eid);

// In your UI update loop (once per frame)
log.UpdateMainThreadLogs();

// Read logs for display
List<LogMessage> entries = log.GetLogsForSource(TextSource.None);

// React to new log entries
log.LogUpdated += source => { /* mark UI dirty */ };

// Clear logs
log.ClearLogs(TextSource.None);
```

#### Log filter config

Use `LogSourceConfig` in your plugin's configuration to store per-source filter preferences:

```csharp
// In your Configuration class
public Dictionary<TextSource, LogSourceConfig> LogConfigs { get; set; } = new()
{
    [TextSource.None] = new LogSourceConfig(),
    [TextSource.Sync] = new LogSourceConfig(),
};
```

#### TextSource values

| Value | Used by |
|---|---|
| `None` | General / uncategorized logs |
| `Chat` | Chat message processing |
| `AddonTalk` | Dialogue addon hooks |
| `AddonBattleTalk` | Battle dialogue hooks |
| `AddonSelectString` | Selection menu hooks |
| `AddonCutsceneSelectString` | Cutscene selection hooks |
| `AddonBubble` | NPC bubble hooks |
| `VoiceTest` | Voice testing / preview |
| `Backend` | Backend / TTS service |
| `Sync` | Multiplayer synchronization |

Add new values to the enum as needed — unused sources cost nothing.

## License

[GNU Affero General Public License v3.0](LICENSE)
