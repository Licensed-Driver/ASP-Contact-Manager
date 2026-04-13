using ContactManager.Models;
using ContactManager.Services;

namespace Tests
{
    public class DocumentServiceUnitTests
    {
        // Same temp md file creation for te tests
        private static string CreateTempMdFile(string content = "# Test")
        {
            var tmp = Path.GetTempFileName();
            var md = Path.ChangeExtension(tmp, ".md");
            File.Move(tmp, md);
            File.WriteAllText(md, content);
            return md;
        }

        [Fact]
        public void GetAll_NoDocuments_ReturnsEmpty()
        {
            var service = new DocumentService();

            Assert.Empty(service.GetAll());
        }

        [Fact]
        public void AddDocument_ValidPath_ReturnsTrue()
        {
            var service = new DocumentService();
            var path = CreateTempMdFile();
            try
            {
                Assert.True(service.AddDocument(path));
            }
            finally { File.Delete(path); }
        }

        [Fact]
        public void AddDocument_DuplicatePath_ReturnsFalse()
        {
            var service = new DocumentService();
            var path = CreateTempMdFile();
            try
            {
                service.AddDocument(path);

                Assert.False(service.AddDocument(path));
            }
            finally { File.Delete(path); }
        }

        [Fact]
        public void AddDocument_NonExistentFile_ThrowsFileNotFoundException()
        {
            var service = new DocumentService();

            Assert.Throws<FileNotFoundException>(() => service.AddDocument("/nonexistent/file.md"));
        }

        [Fact]
        public void GetByPath_AfterAdd_ReturnsDocument()
        {
            var service = new DocumentService();
            var path = CreateTempMdFile();
            try
            {
                service.AddDocument(path);

                var result = service.GetByPath(path);

                Assert.NotNull(result);
                Assert.Equal(Path.GetFullPath(path), result.Path);
                Assert.Equal(Path.GetFileName(path), result.Name);
            }
            finally { File.Delete(path); }
        }

        [Fact]
        public void GetByPath_PathNotAdded_ReturnsNull()
        {
            var service = new DocumentService();

            Assert.Null(service.GetByPath("/a/path/that/was/never/added.md"));
        }

        [Fact]
        public void GetAll_WithDocuments_ReturnsAll()
        {
            var service = new DocumentService();
            var path1 = CreateTempMdFile("# Doc 1");
            var path2 = CreateTempMdFile("# Doc 2");
            try
            {
                service.AddDocument(path1);
                service.AddDocument(path2);

                Assert.Equal(2, service.GetAll().Count());
            }
            finally { File.Delete(path1); File.Delete(path2); }
        }

        [Fact]
        public void GetAll_AfterAdd_ContainsAddedDocument()
        {
            var service = new DocumentService();
            var path = CreateTempMdFile();
            try
            {
                service.AddDocument(path);

                Assert.Contains(service.GetAll(), d => d.Path == Path.GetFullPath(path));
            }
            finally { File.Delete(path); }
        }
    }
}
