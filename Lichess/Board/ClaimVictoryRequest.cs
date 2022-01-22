namespace Lichess.Board;

public class ClaimVictoryRequest
{
    private readonly string _apiEndPoint;
    private readonly string _apiKey;

    public ClaimVictoryRequest(string gameId, string apiKey)
    {
        _apiEndPoint = $"https://lichess.org/api/board/game/{gameId}/claim-victory";
        _apiKey = apiKey;
    }

    public async Task<Response> PostAsync() => await Request.PostAsync(_apiEndPoint, _apiKey);
}
