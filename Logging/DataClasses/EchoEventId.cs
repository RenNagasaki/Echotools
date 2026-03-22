using Echotools.Logging.Enums;

namespace Echotools.Logging.DataClasses;

public class EchoEventId
{
    public static int CurrentId { get; set; } = 1;
    public int Id { get; set; }
    public TextSource TextSource { get; set; }

    public EchoEventId(int id, TextSource textSource)
    {
        Id = id;
        TextSource = textSource;
    }
}
