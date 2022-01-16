using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StockFischer.Engine;
using System.Collections.Generic;
using System.IO;

namespace StockFischer;
public class AppSettings : ReactiveObject
{
    private static readonly string path = "appsettings.json";
    private readonly JObject _json;

    [Reactive]
    public string DefaultEngine { get; set; }
    
    [Reactive]
    public List<UCIEngineInfo> Engines { get; set; }

    public AppSettings(IConfiguration config)
    {
        _json = JObject.Parse(File.ReadAllText(path));

        config.Bind(this);

        PropertyChanged += AppSettings_PropertyChanged;
    }

    /// <summary>
    /// Update Json file when ever we change a value here.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AppSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        object newValue = GetType().GetProperty(e.PropertyName)?.GetValue(this);

        if(newValue is {})
        {
            _json.Property(e.PropertyName).Value = JToken.FromObject(newValue);
            var result = _json.ToString(Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(path, result);
        }
    }   
}
