namespace Lichess.Board;

public class ResignGameRequest
{
    private readonly string _apiEndPoint;
    private readonly string _apiKey;

    public ResignGameRequest(string gameId, string apiKey)
    {
        _apiEndPoint = $"https://lichess.org/api/board/game/{gameId}/resign";
        _apiKey = apiKey;
    }

    public async Task<Response> PostAsync() => await Request.PostAsync(_apiEndPoint, _apiKey);
}
