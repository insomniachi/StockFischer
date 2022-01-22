namespace Lichess.TV;

public class GetCurrentTvGamesRequest
{
    private readonly string _apiEndPoint = "https://lichess.org/api/tv/channels";
    public async Task<TvGames> GetAsync() => await Request.GetAsync<TvGames>(_apiEndPoint);
}
