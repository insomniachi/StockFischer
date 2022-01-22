namespace Lichess.Board;

public class WriteInChatRequest
{
    private readonly string _apiEndPoint;
    private readonly string _apiKey;
    private readonly WriteInChatParameters _parameters;

    public WriteInChatRequest(string gameId, WriteInChatParameters parameters, string token)
    {
        _apiEndPoint = $"https://lichess.org/api/board/game/{gameId}/chat";
        _apiKey = token;
        _parameters = parameters;
    }

    public async Task<Response> PostAsync() => await Request.PostAsync(_apiEndPoint, _parameters, _apiKey);
}
