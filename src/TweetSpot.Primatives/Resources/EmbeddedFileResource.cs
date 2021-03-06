using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace TweetSpot.Resources
{
    /// <summary>
    /// Provides a wrapper around a file embedded in a DLL to do the file-related things you typically need to do with it.
    /// Though frequently used in test assemblies to obtain test files, it is sometimes used to load default state elsewhere.
    /// </summary>
    public class EmbeddedFileResource
    {
        private readonly Assembly _assemblyContainingResource;
        private readonly string _filePath;

        public EmbeddedFileResource(Type classProvidingNamespaceAndAssembly, string relativePath)
        {
            _assemblyContainingResource = classProvidingNamespaceAndAssembly?.Assembly ?? throw new ArgumentNullException(nameof(classProvidingNamespaceAndAssembly));
            if (relativePath == null) throw new ArgumentNullException(nameof(relativePath));
            _filePath = $"{classProvidingNamespaceAndAssembly.Namespace}.{relativePath}";
        }

        /// <summary>
        /// Open the file resource as a readable stream throwing an exception if not found.
        /// </summary>
        /// <returns>Readable stream.</returns>
        /// <exception cref="FileNotFoundException">If the file contents cannot be located in the assembly.</exception>
        public Stream OpenReadStream()
        {
            return _assemblyContainingResource.GetManifestResourceStream(_filePath)
                   ?? throw new FileNotFoundException($"Unable to find resource {_filePath} in assembly {_assemblyContainingResource.GetName().Name}.");
        }

        public Task<Stream> OpenReadStreamAsync()
        {
            return Task<Stream>.FromResult(OpenReadStream());
        }

        public TextReader GetReader()
        {
            return new StreamReader(OpenReadStream());
        }
    }
}
