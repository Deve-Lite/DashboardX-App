using System.Text.Json.Serialization;

namespace Infrastructure.Models;

internal class UpdateResponse
{
    [JsonPropertyName("updatedAt")]
    public DateTime EditedAt { get; set; }
}
