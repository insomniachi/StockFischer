using Microsoft.Extensions.Configuration;
using StockFischer.Engine;
using System.Collections.Generic;

namespace StockFischer;
public class AppSettings
{
    public string DefaultEngine { get; set; }
    public List<UCIEngineInfo> Engines { get; set; }

    public AppSettings(IConfiguration config)
    {
        config.Bind(this);
    }
}
