using System;
using System.IO;
using Microsoft.CodeAnalysis;

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
            throw new NotImplementedException();
        }

        public override string ResolveReference(string path, string baseFilePath)
        {
            throw new NotImplementedException();
        }
    }
}