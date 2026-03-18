using System;
using System.Collections.Generic;
using Echotools.Logging.DataClasses;
using Echotools.Logging.Enums;

namespace Echotools.Logging.Services;

public interface ILogService
{
    /// <summary>True while <see cref="UpdateMainThreadLogs"/> is draining concurrent bags.</summary>
    bool Updating { get; }

    /// <summary>Fired (on any thread) whenever a new log entry is added for a given TextSource.</summary>
    event Action<TextSource>? LogUpdated;

    /// <summary>Creates a tracked event with an auto-incremented ID and logs a Start marker.</summary>
    EKEventId Start(string method, TextSource source);

    /// <summary>Logs an End marker for a previously started event.</summary>
    void End(string method, EKEventId eventId);

    void Info(string method, string message, EKEventId eventId);
    void Debug(string method, string message, EKEventId eventId);
    void Error(string method, string message, EKEventId eventId);
    void Warning(string method, string message, EKEventId eventId);

    /// <summary>
    /// Drains all concurrent bags into the main-thread lists. Call once per frame
    /// from your UI update method (e.g. <c>OnUpdate</c>).
    /// </summary>
    void UpdateMainThreadLogs();

    /// <summary>Clears the main-thread log list for the given source.</summary>
    void ClearLogs(TextSource source);

    /// <summary>Returns the main-thread log list for the given source (safe to read on main thread).</summary>
    List<LogMessage> GetLogsForSource(TextSource source);
}
