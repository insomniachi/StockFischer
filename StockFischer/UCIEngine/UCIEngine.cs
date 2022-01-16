using OpenPGN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace StockFischer.Engine;


public class UCIEngine
{
    private const int MAX_TRIES = 200;
    private int _skillLevel;
    private readonly UCIEngineProcess _process;
    private string _currentFen;

    public event EventHandler<PotentialVariation> PotentialVariationCalculated; 
    
    public UCIEngineOptions Settings { get; set; }
    public int Depth { get; set; }
    public int SkillLevel
    {
        get => _skillLevel;
        set
        {
            _skillLevel = value;
            Settings.SkillLevel = value;
            SetOption("Skill level", SkillLevel.ToString());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="maxDepth"></param>
    /// <param name="settings"></param>
    public UCIEngine(string path, int maxDepth = 27, UCIEngineOptions settings = null)
    {
        Depth = maxDepth;
        _process = new UCIEngineProcess(path);
        _process.Start();
        _process.PotentialVariationCalculated += (_, e) =>
        {
            PotentialVariationCalculated?.Invoke(this, e);
        };

        if (settings == null)
        {
            Settings = new UCIEngineOptions();
        }
        else
        {
            Settings = settings;
        }

        SkillLevel = Settings.SkillLevel;
        
        foreach (var property in Settings.GetPropertiesAsDictionary())
        {
            SetOption(property.Key, property.Value);
        }

        StartNewGame();
    }

    public UCIEngine(UCIEngineInfo info, int maxDepth = 27) : this(info.Path, maxDepth, info.Options) { }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="estimatedTime"></param>
    private void Send(string command)
    {
        _process.WriteLine(command);
    }

    public void Stop() => Send("stop");


    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <exception cref="ApplicationException"></exception>
    private void SetOption(string name, string value)
    {
        Send($"setoption name {name} value {value}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="moves"></param>
    /// <returns></returns>
    private static string MovesToString(string[] moves)
    {
        return string.Join(" ", moves);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="ApplicationException"></exception>
    private void StartNewGame()
    {
        Send("ucinewgame");
    }

    /// <summary>
    /// 
    /// </summary>
    public void Go()
    {
        Send($"go depth {Depth}");
    }


    public void SetNewGamePosition(params string[] moves)
    {
        StartNewGame();

        if(moves is null)
        {
            Send($"position startpos");
        }
        else
        {
            Send($"position startpos moves {MovesToString(moves)}");
        }

        _currentFen = BoardSetup.StartingPosition;
    }

    /// <summary>
    /// Set position in fen format
    /// </summary>
    /// <param name="fenPosition"></param>
    public void SetFenPosition(string fenPosition)
    {
        StartNewGame();
        Send($"position fen {fenPosition}");
        _currentFen = fenPosition;
    }


}
