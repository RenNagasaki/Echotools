using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Plugin.Services;
using Echotools.Logging.DataClasses;
using Echotools.Logging.Enums;

namespace Echotools.Logging.Services;

/// <summary>
/// Thread-safe logging service with a dual-buffer design:
/// worker threads write to <see cref="ConcurrentBag{T}"/>s,
/// the main thread drains them via <see cref="UpdateMainThreadLogs"/>.
/// </summary>
public class LogService : ILogService
{
    private readonly IPluginLog _log;

    // One concurrent bag per TextSource — worker threads write here
    private readonly Dictionary<TextSource, ConcurrentBag<LogMessage>> _bags = new();

    // One list per TextSource — main thread reads here
    private readonly Dictionary<TextSource, List<LogMessage>> _mainThreadLists = new();

    public bool Updating { get; private set; }

    public event Action<TextSource>? LogUpdated;

    public LogService(IPluginLog log)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));

        foreach (var source in Enum.GetValues<TextSource>())
        {
            _bags[source] = new ConcurrentBag<LogMessage>();
            _mainThreadLists[source] = new List<LogMessage>();
        }
    }

    public EKEventId Start(string method, TextSource source)
    {
        var eventId = new EKEventId(EKEventId.CurrentId++, source);
        Info(method, "---------------------------Start----------------------------------", eventId);
        return eventId;
    }

    public void End(string method, EKEventId eventId)
    {
        Info(method, "---------------------------End------------------------------------", eventId);
    }

    public void Info(string method, string message, EKEventId eventId)
        => Log(LogType.Info, method, message, eventId);

    public void Debug(string method, string message, EKEventId eventId)
        => Log(LogType.Debug, method, message, eventId);

    public void Error(string method, string message, EKEventId eventId)
        => Log(LogType.Error, method, message, eventId);

    public void Warning(string method, string message, EKEventId eventId)
        => Log(LogType.Warning, method, message, eventId);

    private void Log(LogType logType, string method, string message, EKEventId eventId)
    {
        var logMessage = new LogMessage
        {
            Type = logType,
            Method = method,
            Message = message,
            EventId = eventId,
            TimeStamp = DateTime.Now,
            Color = logType switch
            {
                LogType.Debug   => LogConstants.DebugLogColor,
                LogType.Warning => LogConstants.WarningLogColor,
                LogType.Error   => LogConstants.ErrorLogColor,
                _               => LogConstants.InfoLogColor,
            }
        };

        _bags[eventId.TextSource].Add(logMessage);

        var formatted = $"[{eventId.Id}:{eventId.TextSource}] {method}: {message}";
        switch (logType)
        {
            case LogType.Debug:   _log.Debug(formatted);   break;
            case LogType.Info:    _log.Info(formatted);     break;
            case LogType.Warning: _log.Warning(formatted);  break;
            case LogType.Error:   _log.Error(formatted);    break;
        }

        LogUpdated?.Invoke(eventId.TextSource);
    }

    public void UpdateMainThreadLogs()
    {
        Updating = true;
        foreach (var source in Enum.GetValues<TextSource>())
            DrainBag(source);
        Updating = false;
    }

    private void DrainBag(TextSource source)
    {
        var bag = _bags[source];
        var list = _mainThreadLists[source];
        while (bag.TryTake(out var message))
            list.Add(message);
    }

    public void ClearLogs(TextSource source)
        => _mainThreadLists[source].Clear();

    public List<LogMessage> GetLogsForSource(TextSource source)
        => _mainThreadLists[source];
}
