namespace Lichess.Board;

public class CreateSeekRequest
{
    private readonly CreateSeekRequestParameters _parameters;
    private readonly string _token;
    private readonly string _apiEndPoint = "https://lichess.org/api/board/seek";

    public CreateSeekRequest(string apiToken, CreateSeekRequestParameters parameters)
    {
        _parameters = parameters;
        _token = apiToken;
    }

    public async Task<Response> PostAsync() => await Request.PostAsync(_apiEndPoint, _parameters, _token);
}
