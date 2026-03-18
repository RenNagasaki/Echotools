using System;
using System.Numerics;
using Echotools.Logging.Enums;

namespace Echotools.Logging.DataClasses;

public class LogMessage
{
    public DateTime TimeStamp { get; set; }
    public string Method { get; set; } = null!;
    public string Message { get; set; } = null!;
    public Vector4 Color { get; set; }
    public EKEventId EventId { get; set; } = null!;
    public LogType Type { get; set; }
}
