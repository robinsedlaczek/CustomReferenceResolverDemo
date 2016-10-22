using System;
using System.IO;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace CustomReferenceResolverDemo
{
    internal class CustomSourceReferenceResolver : SourceReferenceResolver
    {
        public override bool Equals(object other)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string NormalizePath(string path, string baseFilePath)
        {
            throw new NotImplementedException();
        }

        public override Stream OpenRead(string resolvedPath)
        {
            var stream = File.OpenRead(resolvedPath);

            return stream;
        }

        public override string ResolveReference(string path, string baseFilePath)
        {
            var scriptsFolder = Path.Combine(baseFilePath, "Scripts");
            var files = Directory.GetFiles(scriptsFolder);
            var scriptFilePath = files.FirstOrDefault(file => file == Path.Combine(scriptsFolder, path));

            return scriptFilePath;
        }
    }
}