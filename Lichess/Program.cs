namespace Lichess;

public class Program
{
    public static void Main(string[] args)
    {
        //var game = new LiveGameStream("hSP5aPPx");
        //game.StreamStarted += Game_StreamStarted1;
        //game.MovePlayed += Game_MovePlayed;

        //var game = new LiveGameState("oRdgU8ty");
        //game.StreamStarted += Game_StreamStarted;
        //game.StateChanged += Game_StateChanged;

        //var cts = game.StartStream();

        var game = new ExportGameRequest("CCJbDIFL");
        Console.WriteLine(game.GetAsync().Result);

        Console.ReadKey();
    }

    private static void Game_MovePlayed(object sender, GameMove e)
    {
        Console.WriteLine(e.LastMove);
    }

    private static void Game_StreamStarted1(object sender, LiveGameStatus e)
    {
        Console.WriteLine("GameStarting... Last Move = {0}", e.LastMove);
    }

    private static void Game_StateChanged(object sender, GameState e)
    {
        Console.WriteLine(e.Moves);
    }

    private static void Game_StreamStarted(object sender, GameFull e)
    {
        Console.WriteLine(e.Status.Moves);
    }

}
