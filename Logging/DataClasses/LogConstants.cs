using System.Numerics;

namespace Echotools.Logging.DataClasses;

public static class LogConstants
{
    public static readonly Vector4 DebugLogColor = new(0.4f, 0.8f, 0.4f, 1f);
    public static readonly Vector4 ErrorLogColor = new(1.0f, 0.35f, 0.35f, 1f);
    public static readonly Vector4 WarningLogColor = new(1.0f, 0.8f, 0.2f, 1f);
    public static readonly Vector4 InfoLogColor = Vector4.Zero;
}
