namespace Lichess.Board;

public class HandleTakebackRequest
{
    private readonly string _apiEndPoint;
    private readonly string _apiKey;

    public HandleTakebackRequest(string gameId, bool accept, string apiKey)
    {
        _apiEndPoint = $"https://lichess.org/api/board/game/{gameId}/takeback/{accept}";
        _apiKey = apiKey;
    }

    public async Task<Response> PostAsync() => await Request.PostAsync(_apiEndPoint, _apiKey);
}
