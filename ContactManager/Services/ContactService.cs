using ContactManager.Models;
using System.Collections.Concurrent;
using ContactManager.Utilities;
using System.Linq;

namespace ContactManager.Services
{
    public class ContactService : IContactService
    {
        // In place of DB, readonly ConcurrentDictionary so that multiple async requests aren't messing with each other
        private readonly ConcurrentDictionary<Guid, Contact> _contacts = new ConcurrentDictionary<Guid, Contact>();
        public IEnumerable<Contact> GetAll()
        {
            return _contacts.Values;
        }
        public Contact? GetById(Guid id)
        {
            _contacts.TryGetValue(id, out Contact? contact);    // Defaults to null
            return contact;
        }
        public Contact? Add(Contact contact)
        {
            contact.Id = Guid.NewGuid();
            if (!_contacts.TryAdd(contact.Id, contact)) return null;    // If we failed, we wanna signal that a new contact was not created
            return contact;
        }
        public bool Update(Contact contact)
        {
            _contacts.TryGetValue(contact.Id, out Contact? value);
            if (value == null) return false;
            return _contacts.TryUpdate(contact.Id, contact, value);
        }
        public bool Delete(Guid id)
        {
            if (!_contacts.TryGetValue(id, out Contact? contact)) return false;
            return _contacts.TryRemove(KeyValuePair.Create(id, contact));
        }
        public IEnumerable<Contact> Search(string query, int page, int size)
        {
            // TODO: Add this setup to the trade-offs and explanations
            // Returns all values ordered by the best matching field using the Levenshtein ratio
            return _contacts.Values.OrderByDescending(c =>
            MathF.Max(Levenshtein.PercentDiff(c.FirstName, query),
            MathF.Max((c.LastName != null ? Levenshtein.PercentDiff(c.LastName, query) : 0.0f),
            MathF.Max((c.Email != null ? Levenshtein.PercentDiff(c.Email, query) : 0.0f),
            Levenshtein.PercentDiff($"{c.FirstName} {c.LastName}", query)
            )))).Skip((page-1)*size).Take(size);  // Paginated so that we aren't always getting all
        }
    }
}
