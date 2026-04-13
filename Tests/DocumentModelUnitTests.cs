using ContactManager.Models;

namespace Tests
{
    public class DocumentModelUnitTests
    {
        // Make a temp md file for a test
        private static string CreateTempMdFile(string content = "# Test")
        {
            var tmp = Path.GetTempFileName();
            var md = Path.ChangeExtension(tmp, ".md");
            File.Move(tmp, md);
            File.WriteAllText(md, content);
            return md;
        }

        [Fact]
        public void Document_ValidMdFile_SetsProperties()
        {
            var path = CreateTempMdFile();
            try
            {
                var doc = new Document(path);

                // Make sure all the fields were set properly
                Assert.NotEqual(Guid.Empty, doc.Id);
                Assert.Equal(Path.GetFileName(path), doc.Name);
                Assert.Equal(Path.GetFullPath(path), doc.Path);
                Assert.IsType<MD>(doc.Extension);   // Especially this one cuz it does a lot of reflected and more complex ops
            }
            finally { File.Delete(path); }
        }

        [Fact]
        public void Document_NonExistentFile_ThrowsFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => new Document("/nonexistent/path/file.md"));
        }

        [Fact]
        public void Document_UnsupportedExtension_ThrowsFormatException()
        {
            // Give it a .tmp file that we don't support
            var tmp = Path.GetTempFileName();
            try
            {
                Assert.Throws<FormatException>(() => new Document(tmp));
            }
            finally { File.Delete(tmp); }
        }

        [Fact]
        public void Document_Content_ReturnsFileContents()
        {
            var path = CreateTempMdFile("# Hello World");
            try
            {
                var doc = new Document(path);

                Assert.Equal("# Hello World", (string)doc.Content);
            }
            finally { File.Delete(path); }
        }

        [Fact]
        public void Document_Id_IsUniquePerInstance()
        {
            var path1 = CreateTempMdFile();
            var path2 = CreateTempMdFile();
            try
            {
                var doc1 = new Document(path1);
                var doc2 = new Document(path2);

                Assert.NotEqual(doc1.Id, doc2.Id);
            }
            finally { File.Delete(path1); File.Delete(path2); }
        }

        // Testing Extension since it was really complicated in the implementation and I wanna make sure it's chill
        [Fact]
        public void Extension_NewExtension_MdPath_ReturnsMdInstance()
        {
            // NewExtension doesn't validate the file exists, it just extracts it from the path and provides a translater (content)
            var ext = Extension.NewExtension("any/path/file.md");

            Assert.IsType<MD>(ext);
            Assert.Equal(".md", (string)ext);
        }

        [Fact]
        public void Extension_NewExtension_UnknownExtension_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => Extension.NewExtension("file.xyz"));
        }

        [Fact]
        public void Extension_NewExtension_NoExtension_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => Extension.NewExtension("README"));
        }

        [Fact]
        public void MD_ImplicitStringCast_ReturnsDotMd()
        {
            string value = new MD();

            Assert.Equal(".md", value);
        }

        [Fact]
        public void MD_GetContent_ReturnsFileText()
        {
            var path = CreateTempMdFile("## Section");
            try
            {
                var md = new MD();

                Assert.Equal("## Section", md.GetContent(path));
            }
            finally { File.Delete(path); }
        }
    }
}
