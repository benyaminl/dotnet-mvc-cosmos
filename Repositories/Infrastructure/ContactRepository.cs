using System.Text.Json;
using coba.Models.Database;
using coba.Repositories.Domain;
using Microsoft.Azure.Cosmos;

namespace coba.Repositories.Infrastructure;

public class ContactRepository : IContactRepository
{
    private CosmosClient _client;
    private Container _container;

    public ContactRepository(
        CosmosClient client,
        string databaseName = "Contact"
    )
    {
        _client = client;
        _container = client.GetContainer("Contact", databaseName);
    }

    public async Task DeleteMessage(string id)
    {
        await _container.DeleteItemAsync<ContactMessage>(id, new PartitionKey(id));
    }

    public async Task<List<ContactMessage>> GetListMessageAsync(string query)
    {
        var q = _container.GetItemQueryIterator<ContactMessage>(new QueryDefinition(query));
        List<ContactMessage> results = new List<ContactMessage>();
        while (q.HasMoreResults)
        {
            var response = await q.ReadNextAsync();
            
            results.AddRange(response.ToList());
        }

        return results;
    }

    public async Task<ContactMessage> GetMessageAsync(string id)
    {
        try 
        {
            ItemResponse<ContactMessage> resp = await _container.ReadItemAsync<ContactMessage>(id, new PartitionKey(id));
            return resp.Resource;
        }
        catch(CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        { 
            return null;
        }
    }

    public async Task SaveMessage(ContactMessage msg)
    {
        var obj = JsonSerializer.Serialize(msg);
        var result = await _container.CreateItemAsync<ContactMessage>(msg, new PartitionKey(msg.Id));
        var code = result.StatusCode;
    }
}