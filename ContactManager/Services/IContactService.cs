using ContactManager.Models;

namespace ContactManager.Services
{
    public interface IContactService
    {
        IEnumerable<Contact> GetAll();  // IEnumerable for read only view
        Contact? GetById(Guid id);
        Contact? Add(Contact contact);
        bool Update(Contact contact);
        bool Delete(Guid id);
        IEnumerable<Contact> Search(string query, int page, int size);  // IEnumerable for read only view
    }
}
