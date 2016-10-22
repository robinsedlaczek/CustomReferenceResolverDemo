using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Collections.Immutable;
using System.IO;

namespace CustomReferenceResolverDemo
{
    internal class CustomMetadataReferenceResolver : MetadataReferenceResolver
    {
        public override bool Equals(object other) => Equals(other as CustomMetadataReferenceResolver);

        public bool Equals(CustomMetadataReferenceResolver other)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override bool ResolveMissingAssemblies
        {
            get
            {
                return true;
            }
        }

        public override PortableExecutableReference ResolveMissingAssembly(MetadataReference definition, AssemblyIdentity referenceIdentity)
        {            
            return base.ResolveMissingAssembly(definition, referenceIdentity);
        }

        public override ImmutableArray<PortableExecutableReference> ResolveReference(string reference, string baseFilePath, MetadataReferenceProperties properties)
        {
            var path = Path.Combine(baseFilePath, "SharedLibraries");
            var files = Directory.GetFiles(path);
            var filePath = files.FirstOrDefault(file => file == Path.Combine(path, reference));
            var metadataReference = MetadataReference.CreateFromFile(filePath);

            return ImmutableArray.Create(metadataReference);
        }
    }
}
