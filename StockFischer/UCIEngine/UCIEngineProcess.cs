using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace StockFischer.Engine;

internal class UCIEngineProcess
{
    /// <summary>
    /// Default process info for engine process
    /// </summary>
    private readonly ProcessStartInfo _processStartInfo;

    /// <summary>
    /// engine process
    /// </summary>
    private readonly Process _process;


    public event EventHandler<PotentialVariation> PotentialVariationCalculated;

    /// <summary>
    /// engine process constructor
    /// </summary>
    /// <param name="path">Path to usable binary file from stockfish site</param>
    public UCIEngineProcess(string path)
    {
        //TODO: need add method which should be depended on os version
        _processStartInfo = new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
        };

        _process = new Process 
        {
            StartInfo = _processStartInfo,
            EnableRaisingEvents = true,
        };

        _process.OutputDataReceived += OutputRecieved;
        _process.ErrorDataReceived += ErrorDataReceived;
    }

    private void OutputRecieved(object sender, DataReceivedEventArgs e)
    {
        ProcessEngineOutput(e.Data);
    }

    private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        ProcessEngineOutput(e.Data);
    }

    private void ProcessEngineOutput(string data)
    {
        if (string.IsNullOrEmpty(data)) return;

        if (PotentialVariation.TryParseFromEngineOutput(data) is { } pv)
        {
            PotentialVariationCalculated?.Invoke(this, pv);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="millisecond"></param>
    public void Wait(int millisecond)
    {
        _process.WaitForExit(millisecond);
    }

    /// <summary>
    /// This method is writing in stdin of Stockfish process
    /// </summary>
    /// <param name="command"></param>
    public void WriteLine(string command)
    {
        if (_process.StandardInput == null)
        {
            throw new NullReferenceException();
        }
        _process.StandardInput.WriteLine(command);
        _process.StandardInput.Flush();
    }

    /// <summary>
    /// This method is allowing to read stdout of Stockfish process
    /// </summary>
    /// <returns></returns>
    public string ReadLine()
    {
        if (_process.StandardOutput == null)
        {
            throw new NullReferenceException();
        }

        return _process.StandardOutput.ReadLine();
    }
    /// <summary>
    /// Start stockfish process
    /// </summary>
    public void Start()
    {
        _process.Start();
        _process.BeginOutputReadLine();
    }
    /// <summary>
    /// This method is allowing to close Stockfish process
    /// </summary>
    ~UCIEngineProcess()
    {
        //When process is going to be destructed => we are going to close stockfish process
        _process.Close();
    }
}
