using System;
using System.IO;
using Xunit;

namespace TweetSpot.Resources
{
    public class EmbeddedFileResourceTesting
    {
        [Fact]
        public void ReadStream_Normal_NoExceptions()
        {
            var resource = new EmbeddedFileResource(GetType(),"HelloWorld.txt");
            using var stream = resource.OpenReadStream();
            using var reader = new StreamReader(stream);
            var contents = reader.ReadToEnd();
            Assert.Equal("Hello World!!!", contents);
        }

        [Fact]
        public void ReadStream_ResourceThatDoesNotExist_ThrowsException()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                new EmbeddedFileResource(GetType(), "This.File.Does.Not.Exist").OpenReadStream();
            });
        }
    }
}
