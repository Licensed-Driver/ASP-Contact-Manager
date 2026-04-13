using System.Reflection.Metadata;

namespace ContactManager.Services
{
    public interface IDocumentService
    {
        public Models.Document? GetByPath(string filepath);
        public IEnumerable<Models.Document> GetAll();
        public bool AddDocument(string path);
    }
}
