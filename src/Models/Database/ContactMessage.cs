using System.Text.Json;
using System.Text.Json.Serialization;

namespace coba.Models.Database;

public class ContactMessage
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;
}