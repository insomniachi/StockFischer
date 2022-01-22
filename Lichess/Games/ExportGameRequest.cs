namespace Lichess.Games;

public class ExportGameRequest
{
    private readonly string _endPoint;
    public ExportGameRequest(string gameId, ExportGameQueryParameters parameters = null)
    {
        parameters ??= new();
        _endPoint = $"https://lichess.org/game/export/{gameId}?{parameters}";
    }

    public async Task<string> GetAsync()
    {
        using var client = new HttpClient();
        return await client.GetStringAsync(_endPoint).ConfigureAwait(false);
    }
}
