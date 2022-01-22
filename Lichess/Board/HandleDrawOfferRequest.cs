namespace Lichess.Board;

public class HandleDrawOfferRequest
{
    private readonly string _apiEndPoint;
    private readonly string _apiKey;

    public HandleDrawOfferRequest(string gameId, bool accept, string apiKey)
    {
        _apiEndPoint = $"https://lichess.org/api/board/game/{gameId}/draw/{accept}";
        _apiKey = apiKey;
    }

    public async Task<Response> PostAsync() => await Request.PostAsync(_apiEndPoint, _apiKey);
}
