using Markdig;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace ContactManager.Models
{
    // This is all just me having fun now
    
    // Class for implicit casting from known file extensions for easier readability and modularity
    public abstract class Extension
    {
        public string Value { get; protected set; }

        public static implicit operator string(Extension value) => value.Value;
        public override string ToString() => Value;

        protected Extension(string extension) => Value = extension;

        public static Extension NewExtension(string filepath)
        {
            var value = System.IO.Path.GetExtension(filepath);

            // Reflect through to get the file extension if it is valid, throw otherwise
            // Reflection is so cool, I love it and never get a good chance to use it
            var match = System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOfRawGeneric(typeof(Extension<,>)))
                .Select(t => (Extension)Activator.CreateInstance(t))
                .FirstOrDefault(ext => ext.Value == value);

            if (match == null)
                throw new FormatException($"File type {value} is not supported.");

            return match;
        }

        public abstract object Content(string filepath);
    }

    // Just checks if the extension class is an attribute on the generic
    public static class TypeExtensions
    {
        public static bool IsSubclassOfRawGeneric(this Type type, Type generic)
        {
            while (type != null && type != typeof(object))
            {
                var cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (cur == generic) return true;
                type = type.BaseType;
            }
            return false;
        }
    }

    // Allows us to use CRTP which is funky but fun and lets us do a bunch of type casting nicely
    public abstract class Extension<T, ContentType>(string ext) : Extension(ext) where T : Extension<T, ContentType>, new()
    {
        public override object Content(string filepath) => GetContent(filepath);
        public abstract ContentType GetContent(string filepath);
    }

    // Concrete types
    // When implementing, you specify  decoded type, file extension, and how to get from the path to the decoded content
    public class MD() : Extension<MD, string>(".md")
    {
        public override string GetContent(string filepath)
        {
            return File.ReadAllText(filepath);
        }
    }

    // For storing any custom document MetaData
    public class Document
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set;  }
        public Extension Extension { get; set; }
        public object Content { get => Extension.Content(Path);  }

        public Document(string path)
        {
            Id = Guid.NewGuid();
            if (!File.Exists(path)) throw new FileNotFoundException($"File {path} either doesn't exist, or is inaccessible.");
            Path = System.IO.Path.GetFullPath(path);
            Name = System.IO.Path.GetFileName(path);
            Extension = Extension.NewExtension(path);
        }
    }
}
