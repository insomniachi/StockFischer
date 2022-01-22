using System.Text.Json.Serialization;

namespace Lichess.TV;

public class TvGames
{
    public TvGame Bot { get; set; }
    public TvGame Blitz { get; set; }
    public TvGame UltraBullet { get; set; }
    public TvGame Bullet { get; set; }
    public TvGame Classical { get; set; }
    public TvGame Antichess { get; set; }
    public TvGame Computer { get; set; }
    public TvGame Horde { get; set; }
    public TvGame Rapid { get; set; }
    public TvGame Atomic { get; set; }
    public TvGame Crazyhouse { get; set; }
    public TvGame Chess960 { get; set; }

    [JsonPropertyName("Top Rated")]
    public TvGame TopRated { get; set; }

    [JsonPropertyName("King of the Hill")]
    public TvGame KingOfTheHill { get; set; }

    [JsonPropertyName("Three-Check")]
    public TvGame ThreeCheck { get; set; }

    [JsonPropertyName("Racing Kings")]
    public TvGame RacingKing { get; set; }
}
