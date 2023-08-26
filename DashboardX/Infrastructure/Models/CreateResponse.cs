
using System.Text.Json.Serialization;

namespace Infrastructure.Models;

internal class CreateResponse : UpdateResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}

