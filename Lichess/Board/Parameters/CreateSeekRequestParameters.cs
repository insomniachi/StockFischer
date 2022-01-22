namespace Lichess.Board;

public class CreateSeekRequestParameters : Parameters
{
    /// <summary>
    /// Whether the game is rated and impacts players ratings.
    /// </summary>
    public bool Rated { get; set; }

    /// <summary>
    /// Clock initial time in minutes. Required for real-time seeks.
    /// </summary>
    public int Time { get; set; }

    /// <summary>
    /// Clock increment in seconds. Required for real-time seeks.
    /// </summary>
    public int Increment { get; set; }

    /// <summary>
    /// Days per turn. Required for correspondence seeks.
    /// </summary>
    public int? Days { get; set; }

    /// <summary>
    /// Enum: "standard" "chess960" "crazyhouse" "antichess" "atomic" "horde" "kingOfTheHill" "racingKings" "threeCheck"
    /// </summary>
    public string Variant { get; set; }

    /// <summary>
    /// The color to play.Better left empty to automatically get 50% white.
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// The rating range of potential opponents. Better left empty. Example: 1500-1800
    /// </summary>
    public string RatingRange { get; set; }
}
