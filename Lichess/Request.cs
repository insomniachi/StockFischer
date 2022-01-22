using System.Text.Json;

namespace Lichess;

public class Request
{
    public static async Task<Response> PostAsync(string endPoint, Parameters parameter, string token = null)
    {
        using var client = new HttpClient();
        if (string.IsNullOrEmpty(token) == false)
        {
            client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        }

        using var content = new FormUrlEncodedContent(parameter.ToPostContent());
        content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

        var response = await client.PostAsync(endPoint, content);

        return await GetResponse(response);
    }

    public static async Task<Response> PostAsync(string endPoint, string token = null)
    {
        using var client = new HttpClient();
        if (string.IsNullOrEmpty(token) == false)
        {
            client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        }

        var response = await client.PostAsync(endPoint, null);

        return await GetResponse(response);
    }

    public static async ValueTask<Response> GetResponse(HttpResponseMessage message)
    {
        if (message.IsSuccessStatusCode)
        {
            return new() { Success = true, Error = string.Empty };
        }
        else
        {
            var err = await message.Content.ReadAsStringAsync();
            var error = JsonSerializer.Deserialize<Error>(err);
            return new() { Success = false, Error = error.ErrorMessage };
        }
    }

}
