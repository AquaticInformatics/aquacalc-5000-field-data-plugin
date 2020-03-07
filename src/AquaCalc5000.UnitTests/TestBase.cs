using System.IO;
using System.Reflection;
using System.Text;

namespace AquaCalc5000.UnitTests
{
    public abstract class TestBase
    {
        private readonly Assembly _testAssembly = Assembly.GetExecutingAssembly();

        protected Stream GetEmbeddedFileStream(string partialFileName)
        {
            var resourceFileName = MakeFullEmbeddedResourceFileName(partialFileName);

            return _testAssembly.GetManifestResourceStream(resourceFileName);
        }

        private static string MakeFullEmbeddedResourceFileName(string partialName)
        {
            return $"AquaCalc5000.UnitTests.TestData.{partialName}";
        }

        protected string GetEmbeddedFileFullText(string partialFileName)
        {
            using (var streamReader = new StreamReader(GetEmbeddedFileStream(partialFileName), Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: false))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
