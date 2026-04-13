using ContactManager.Models;
using System.Collections.Concurrent;

namespace ContactManager.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ConcurrentDictionary<string, Document> _documents = new ConcurrentDictionary<string, Document>();

        public Document? GetByPath(string filepath)
        {
            _documents.TryGetValue(filepath, out var document);
            return document;
        }

        public IEnumerable<Document> GetAll()
        {
            return _documents.Values;
        }

        public bool AddDocument(string filepath)
        {
            return _documents.TryAdd(filepath, new Document(filepath));
        }
    }
}
