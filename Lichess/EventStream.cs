﻿using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Lichess;

public abstract class EventStream
{
    private readonly string _token;

    public string ApiEndPoint { get; protected set; }
    public static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public EventStream()
    {

    }

    public EventStream(string apiToken)
    {
        _token = apiToken;
    }

    public CancellationTokenSource StartStream()
    {
        var cts = new CancellationTokenSource();
        Task.Run(() => StartStreamInternal(cts.Token), cts.Token);
        return cts;
    }

    protected abstract void ProcessJson(string json);

    private async Task StartStreamInternal(CancellationToken token)
    {
        using var client = new HttpClient();

        if (!string.IsNullOrEmpty(_token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        var stream = await client.GetStreamAsync(ApiEndPoint, token).ConfigureAwait(false);
        var sb = new StringBuilder();

        while (!token.IsCancellationRequested)
        {
            var b = stream.ReadByte();

            if (b == -1)
            {
                break;
            }

            var c = (char)b;

            if (c == '\n')
            {
                var json = sb.ToString();
                sb.Clear();

                if (string.IsNullOrEmpty(json))
                {
                    continue;
                }

                ProcessJson(json);
            }
            else
            {
                sb.Append(c);
            }

        }
    }
}
