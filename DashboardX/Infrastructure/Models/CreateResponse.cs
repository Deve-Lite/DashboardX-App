
using System.Text.Json.Serialization;

namespace Infrastructure.Models;

internal class CreateResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}

