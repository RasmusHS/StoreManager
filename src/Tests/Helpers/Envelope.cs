using System.Text.Json.Serialization;

namespace Helpers;

public class Envelope : Envelope<string>
{
    protected internal Envelope(string errorMessage) : base(null, errorMessage)
    {
    }

    public static Envelope<T> Ok<T>(T result)
    {
        return new Envelope<T>(result, null);
    }

    public static Envelope Ok()
    {
        return new Envelope(null);
    }

    public static Envelope Error(string errormessage)
    {
        return new Envelope(errormessage);
    }
}

public class Envelope<T>
{
    [JsonPropertyName("result")]
    public T? Result { get; set; }

    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("timeGenerated")]
    public DateTime TimeGenerated { get; set; }

    protected internal Envelope(T result, string errorMessage)
    {
        this.Result = result;
        this.ErrorMessage = errorMessage;
        this.TimeGenerated = DateTime.UtcNow;
    }

    // Parameterless constructor required for JSON deserialization
    public Envelope()
    {
    }
}