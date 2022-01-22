namespace Lichess.Board;

public class AbortGameRequest
{
    private readonly string _apiEndPoint;
    private readonly string _apiKey;

    public AbortGameRequest(string gameId, string apiToken)
    {
        _apiEndPoint = $"https://lichess.org/api/board/game/{gameId}/abort";
        _apiKey = apiToken;
    }

    public async Task<Response> PostAsync() => await Request.PostAsync(_apiEndPoint, _apiKey);
}
