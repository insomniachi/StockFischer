using Lichess.Board;
using Lichess.Games;
using Lichess.TV;

namespace Lichess;

public class Program
{
    public static async Task Main(string[] args)
    {
        //var game = new LiveGameStream("hSP5aPPx");
        //game.StreamStarted += Game_StreamStarted1;
        //game.MovePlayed += Game_MovePlayed;

        //var game = new LiveGameState("oRdgU8ty");
        //game.StreamStarted += Game_StreamStarted;
        //game.StateChanged += Game_StateChanged;

        //var cts = game.StartStream();

        //var game = new CurrentTvGameStream();
        //game.StreamStarted += Game_StreamStarted2;
        //game.MovePlayed += Game_MovePlayed1;
        //game.StartStream();

        var test = await new GetCurrentTvGamesRequest().GetAsync();

        Console.ReadKey();
    }

    private static void Game_MovePlayed1(object sender, CurrentTvGameMove e)
    {
        Console.WriteLine(e.Details.LastMove);
    }

    private static void Game_StreamStarted2(object sender, CurrentTvGame e)
    {
        Console.WriteLine(e.Details.Id);
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
