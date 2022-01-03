using OpenPGN;

public class AttackResult
{
    public bool IsAttacked { get; set; }
    public List<Attack> Attacks { get; private init; } = new();

    public static AttackResult operator |(AttackResult a, AttackResult b)
    {
        var attacks = new List<Attack>();
        attacks.AddRange(a.Attacks);
        attacks.AddRange(b.Attacks);

        return new AttackResult
        {
            IsAttacked = a.IsAttacked || b.IsAttacked,
            Attacks = attacks
        };
    }

    public static implicit operator bool(AttackResult a) => a.IsAttacked;
}

