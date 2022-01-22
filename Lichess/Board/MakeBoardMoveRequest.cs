namespace Lichess.Board;

public class MakeBoardMoveRequest
{
    private readonly string _apiEndPoint;
    private readonly string _apiToken;

    public MakeBoardMoveRequest(string gameId, string move, string token)
    {
        _apiEndPoint = $"https://lichess.org/api/board/game/{gameId}/move/{move}";
        _apiToken = token;
    }

    public async Task<Response> PostAsync() => await Request.PostAsync(_apiEndPoint, _apiToken);
}
