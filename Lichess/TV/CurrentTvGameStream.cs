using System.Text.Json;

namespace Lichess.TV;

public class CurrentTvGameStream : EventStream
{
    public event EventHandler<CurrentTvGame> StreamStarted;
    public event EventHandler<CurrentTvGameMove> MovePlayed;

    public CurrentTvGameStream()
    {
        ApiEndPoint = "https://lichess.org/api/tv/feed";
    }

    protected override void ProcessJson(string json)
    {
        if(json.Contains("players") && JsonSerializer.Deserialize<CurrentTvGame>(json, Options) is { } ctvg)
        {
            StreamStarted?.Invoke(this, ctvg);
        }
        else if(json.Contains("lm") && JsonSerializer.Deserialize<CurrentTvGameMove>(json, Options) is { } move)
        {
            MovePlayed?.Invoke(this, move);
        }
    }
}
