using System;
using System.IO;

namespace TestReading
{
    public class TestReader
    {
        string basePath;

        public TestReader(string filePath, params string[] filePaths)
        {
            string workingDirectory = Environment.CurrentDirectory;
            basePath = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            basePath = Path.Combine(basePath, filePath);
            foreach (var pathPart in filePaths)
            {
                basePath = Path.Combine(basePath, pathPart);
            }
        }

        public string ReadTest(string filePath, params string[] filePaths)
        {
            var path = Path.Combine(basePath, filePath);
            foreach (var pathPart in filePaths)
            {
                path = Path.Combine(path, pathPart);
            }
            return File.ReadAllText(path);
        }
    }
}
