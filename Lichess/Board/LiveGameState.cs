using System.Text.Json;

namespace Lichess;

public class LiveGameState : EventStream
{
    public event EventHandler<GameFull> StreamStarted;
    public event EventHandler<GameState> StateChanged;
    public event EventHandler<ChatLine> ChatLineChanged;

    public LiveGameState(string gameId)
    {
        ApiEndPoint = $"https://lichess.org/api/board/game/stream/{gameId}";
    }

    protected override void ProcessJson(string json)
    {
        if(json.Contains("gameFull") && JsonSerializer.Deserialize<GameFull>(json, Options) is { } gf)
        {
            StreamStarted?.Invoke(this, gf);
        }
        else if(json.Contains("gameState") && JsonSerializer.Deserialize<GameState>(json, Options) is { } gs)
        {
            StateChanged?.Invoke(this, gs);
        }
        else if(json.Contains("chatLine") && JsonSerializer.Deserialize<ChatLine>(json, Options) is { } cl)
        {
            ChatLineChanged?.Invoke(this, cl);
        }
    }
}
