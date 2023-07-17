using System.Collections.Generic;
using coba.Models.Database;

namespace coba.Repositories.Domain;

public interface IContactRepository
{
    public Task SaveMessage(ContactMessage msg);
    public Task DeleteMessage(string id);
    public Task<ContactMessage?> GetMessageAsync(string id);
    public Task<List<ContactMessage>> GetListMessageAsync(string query);
}