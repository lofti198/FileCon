using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq; // LINQ extension methods

namespace FileConcatenator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string[] fileExtensions = new string[] { "*.cs", "*.md" }; // Support for both C# and Markdown files
            string folderPath = Directory.GetCurrentDirectory(); // Current directory

            SearchOption searchOption = SearchOption.AllDirectories;
            if (args.Length > 0 && args[0].Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                searchOption = SearchOption.TopDirectoryOnly;
            }

            string combinedContent = "";

            foreach (var extension in fileExtensions)
            {
                string[] files = Directory.GetFiles(folderPath, extension, searchOption);
                files = files.Where(file => !file.Contains(@"\obj\")).ToArray();

                foreach (var file in files)
                {
                    combinedContent += $"File: {file}{Environment.NewLine}{Environment.NewLine}";
                    combinedContent += await File.ReadAllTextAsync(file) + Environment.NewLine;
                    combinedContent += $"{Environment.NewLine}--- End of {file} ---{Environment.NewLine}{Environment.NewLine}";
                }
            }

            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string outputFile = Path.Combine(myDocumentsPath, "joined_files.txt");

            await File.WriteAllTextAsync(outputFile, combinedContent);
            Process.Start(new ProcessStartInfo(outputFile) { UseShellExecute = true });

            Console.WriteLine($"Content written to {outputFile} and opened in default editor.");
        }
    }
}
