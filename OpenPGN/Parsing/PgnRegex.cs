using OpenPGN.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenPGN.Parsing
{
    internal class PgnRegex
    {
        public const string MoveNumber = @"(?'MoveNumber'\d+)\.(?'Continued'\.\.)?\s*?";
        public const string Space = @"\s*";
        public static string CheckOrMate(Color color) => @$"(?'{color}CheckOrMate'\+|\+\+|\#)?";
        public static string Annotation(Color color) => $@"(?'{color}Annotation'\!|\!\!|\?|\?\?)?";
        public static string Result(Color color) => @$"(?'{color}Result'(1\/2-1\/2)|(1-0)|(0-1))?";
        public static string Piece(Color color) => $@"(?'{color}Piece'[KQNBR])?";
        public static string OriginFile(Color color) => $@"(?'{color}OriginFile'[a-h])?";
        public static string OriginRank(Color color) => $@"(?'{color}OriginRank'[1-8])?";
        public static string Capture(Color color) => $@"(?'{color}Capture'x)?";
        public static string TargetFile(Color color) => $@"(?'{color}TargetFile'[a-h])";
        public static string TargetRank(Color color) => $@"(?'{color}TargetRank'[1-8])";
        public static string EnPassant(Color color) => $@"(?'{color}EnPassant'\se\.p\.\s)?";
        public static string Promotion(Color color) => $@"(?'{color}Promotion'=[QRNB])?";
        public static string Castle(Color color) => $@"(?'{color}Castle'O-O(-O)?)";

        public static string Comment(Color color) => color == Color.White ? @"(\{(?'WhiteComment'[^}]+)\})?" : @"(\{(?'BlackComment'[^}]+)\})?";

        public static string MoveText = new StringBuilder()
            .Append(MoveNumber)
            .Append(HalfMove(Color.White))
            .Append($"({HalfMove(Color.Black)})?")
            .ToString();

        public static Regex GameInfoRegex => new(@$"\[(?'Name'\w+)\s""(?'Value'.*)""\]{Space}", RegexOptions.Compiled);
        public static Regex MoveTextRegex => new(MoveText, RegexOptions.Compiled);
        public static Regex RAV => new(@"\((?>\((?<c>)|[^()]+|\)(?<-c>))*(?(c)(?!))\)", RegexOptions.Compiled);
        public static Regex MoveNumberRegex => new(MoveNumber, RegexOptions.Compiled);

        public static string HalfMove(Color color)
        {
            var simpleMoveOrCapture = new StringBuilder()
                .Append(Piece(color))
                .Append(OriginFile(color))
                .Append(OriginRank(color))
                .Append(Capture(color))
                .Append(TargetFile(color))
                .Append(TargetRank(color))
                .Append(Promotion(color))
                .Append(EnPassant(color))
                .ToString();

            return @$"({simpleMoveOrCapture}|{Castle(color)}){CheckOrMate(color)}{Space}{Annotation(color)}{Comment(color)}{Space}";
        }
    }

    public static class GroupExtensions
    {
        public static MoveTextEntry? GetMoveText(this Match match)
        {
            int moveNumber = int.Parse(match.Groups["MoveNumber"].Value);

            if (match.IsMovePair())
            {
                return new MovePairEntry(match.GetHalfMove(Color.White), match.GetHalfMove(Color.Black))
                {
                    MoveNumber = moveNumber,
                };
            }
            else
            {
                return new HalfMoveEntry(match.GetHalfMove())
                {
                    MoveNumber = moveNumber,
                    IsContinued = match.Groups["Continued"].Success
                };
            }

        }

        public static Move GetHalfMove(this Match match)
        {
            return match.GetHalfMove(Color.White);
        }

        public static bool IsMovePair(this Match match)
        {
            IEnumerable<KeyValuePair<string, Group>> groups = match.Groups;

            return groups.Any(x =>
            {
                bool hasWhiteMove = groups.Any(x => x.Key.Contains("White") && x.Value.Success);
                bool hasBlackMove = groups.Any(x => x.Key.Contains("Black") && x.Value.Success);
                return hasWhiteMove && hasBlackMove;
            });
        }

        public static Move GetHalfMove(this Match match, Color color)
        {
            var move = new Move
            {
                Piece = GetPieceType(match.Groups[$"{color}Piece"]),
                OriginFile = GetFile(match.Groups[$"{color}OriginFile"]),
                OriginRank = GetRank(match.Groups[$"{color}OriginRank"]),
                TargetFile = GetFile(match.Groups[$"{color}TargetFile"])
            };

            var rank = GetRank(match.Groups[$"{color}TargetRank"]);
            if (move.TargetFile.HasValue && rank.HasValue)
            {
                move.TargetSquare = new Square(move.TargetFile.Value, rank.Value);
            }

            move.Type = GetMoveType(match, color);

            if (match.Groups[$"{color}CheckOrMate"].Success)
            {
                string value = match.Groups[$"{color}CheckOrMate"].Value;

                move.IsCheck = value.Contains('+');
                move.IsDoubleCheck = value.Contains("++");
                move.IsCheckMate = value.Contains('#');
            }

            if (match.Groups[$"{color}Promotion"].Success)
            {
                string value = match.Groups[$"{color}Promotion"].Value;
                move.PromotedPiece = (PieceType)value[^1];
            }

            if (match.Groups[$"{color}Comment"].Success)
            {
                move.Comment = match.Groups[$"{color}Comment"].Value;
            }

            return move;
        }

        private static MoveType GetMoveType(Match match, Color color)
        {
            var type = MoveType.Simple;

            if (match.Groups[$"{color}Castle"].Success)
            {
                type = match.Groups[$"{color}Castle"].Value switch
                {
                    "O-O" => MoveType.CastleKingSide,
                    "O-O-O" => MoveType.CastleQueenSide,
                    _ => throw new Exception("Invalid Castling move")
                };
            }

            else if (match.Groups[$"{color}Capture"].Success)
            {
                type = MoveType.Capture;

                if (match.Groups[$"{color}EnPassant"].Success)
                {
                    type = MoveType.CaptureEnPassant;
                }
            }

            return type;
        }

        private static Models.File? GetFile(Group group)
        {
            return group.Success ? (Models.File)group.Value[0] : null;
        }

        private static int? GetRank(Group group)
        {
            return group.Success ? int.Parse(group.Value) : null;
        }

        private static PieceType GetPieceType(Group group)
        {
            return group.Success ? (PieceType)group.Value[0] : PieceType.Pawn;
        }

    }
}


