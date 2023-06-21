
namespace DashboardXModels;

public class DeleteItemDTO 
{
    public DeleteItemDTO() { }
    public DeleteItemDTO(string id)
    {
        Id = id;
    }
    public string Id { get; set; } = string.Empty;
}
