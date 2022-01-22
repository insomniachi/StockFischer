using System.Text.Json;

namespace Lichess.Games;

public class LiveGameStream : EventStream
{
    private string lastMove;
    private bool lastMoveEncountered;

    public event EventHandler<LiveGameStatus> StreamStarted;
    public event EventHandler<GameMove> MovePlayed;
    public event EventHandler<LiveGameStatus> StreamEnded;

    public LiveGameStream(string id)
    {
        ApiEndPoint = $"https://lichess.org/api/stream/game/{id}";
    }

    protected override void ProcessJson(string json)
    {
        if (json.Contains("wc") && JsonSerializer.Deserialize<GameMove>(json, Options) is { } move)
        {
            MovePlayed?.Invoke(this, move);
        }
        else if (json.Contains("id") && JsonSerializer.Deserialize<LiveGameStatus>(json, Options) is { } gs)
        {
            if (string.IsNullOrEmpty(gs.Winner))
            {
                lastMove = gs.LastMove;
                StreamStarted?.Invoke(this, gs);
            }
            else
            {
                StreamEnded?.Invoke(this, gs);
            }

        }

    }
}
