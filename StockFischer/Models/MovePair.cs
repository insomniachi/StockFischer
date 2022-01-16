﻿using OpenPGN.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace StockFischer.Models;

public class MovePair : ReactiveObject
{
    public int MoveNumber { get; set; }

    [Reactive]
    public MoveModel White { get; set; }

    [Reactive]
    public MoveModel Black { get; set; }

    public override string ToString()
    {
        return $"{White} - {Black}";
    }
}

public class MoveModel
{
    public Move Move { get; set; }
    public string Fen { get; set; }
    public Color Color => LivePiece.Color;
    public LivePiece LivePiece { get; set; }
    public LivePiece CapturedPiece { get; set; }
    public Square TargetSquare { get; set; }
    public Square OriginSquare { get; set; }
    public PieceType PieceType => LivePiece.Type;

    public MoveModel(Move m)
    {
        Move = m;
    }

    public override string ToString()
    {
        var algebraic = Move.ToString();

        if (LivePiece.Type == PieceType.Pawn)
        {
            return algebraic;
        }
        if (Move.Type is MoveType.CastleKingSide or MoveType.CastleQueenSide)
        {
            return algebraic;
        }

        return $"{LivePiece.Glyph}{algebraic[1..]}";
    }
}

public class MoveCollection : ObservableCollection<MovePair>
{
    private MovePair _currentPair;
    public MovePair CurrentPair
    {
        get => _currentPair;
        set
        {
            _currentPair = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentPair)));
        }
    }

    private MoveModel _current;
    public MoveModel Current
    {
        get => _current;
        set
        {
            _current = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Current)));
        }
    }

    public void AddMove(MoveModel move)
    {
        if (move.Color == Color.White)
        {
            CurrentPair = new MovePair { MoveNumber = Count + 1 };
            Current = CurrentPair.White = move;
            Add(CurrentPair);
        }
        else
        {
            if (CurrentPair is null)
            {
                CurrentPair = new MovePair { MoveNumber = Count + 1 };
                Add(CurrentPair);
            }

            Current = CurrentPair.Black = move;
        }
    }

    public bool GoBack()
    {
        if (CurrentPair is null || Current is null) return false;

        if (Current.Color == Color.Black)
        {
            Current = CurrentPair.White;
            return true;
        }

        int index = IndexOf(CurrentPair);

        if (index <= 0) return false;

        CurrentPair = this[index - 1];

        Current = CurrentPair.Black;

        return true;
    }

    public bool GoForward()
    {
        if (CurrentPair is null) return false;
        if (Current is null) return false;

        if (Current.Color == Color.White)
        {
            Current = CurrentPair.Black;
            return true;
        }

        var index = IndexOf(CurrentPair);

        if (index == Count - 1) return false;

        CurrentPair = this[index + 1];

        Current = CurrentPair.White;

        return true;
    }

    public bool GoToStart()
    {
        if (Count == 0) return false;
        CurrentPair = this.First();
        Current = CurrentPair.White ?? CurrentPair.Black;
        return true;
    }

    public bool GoToEnd()
    {
        if (Count == 0) return false;
        CurrentPair = this.Last();
        Current = CurrentPair.Black ?? CurrentPair.White;
        return true;
    }

    protected override event PropertyChangedEventHandler PropertyChanged;
}
