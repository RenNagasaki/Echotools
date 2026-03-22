using Echotools.Logging.Enums;

namespace Echotools.Logging.DataClasses;

public class EKEventId : EchoEventId
{
    public EKEventId(int id, TextSource textSource) : base(id, textSource) { }
}
