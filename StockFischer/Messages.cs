using OpenPGN.Models;

namespace StockFischer.Messages;

public class GameOpenedMessage
{
    public Game Game { get; set; }
}

public class StartAutoPlayMessage { }

public class StopEngineMessage { }
