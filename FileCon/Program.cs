using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace FileConcatenator
{
    class Program
    {
        private const string OutputFileName = "joined_files.txt";
        private static readonly string[] SupportedExtensions = { "*.cs", "*.md", "*.js", "*.jsx", "*.ts", "*.sln", "*.html", "*.json" };
        private static readonly List<string> ExcludedFolderNames = new List<string>
        {
            "obj", "bin", "migrations", "dist", "node_modules"
        };
        private static readonly List<string> ExcludedFolderNameSubstrings = new List<string>
        {
            "jquery", "backup"
        };
        private static readonly List<string> ExcludedFileSubstrings = new List<string>
        {
            "package"
        };

        static async Task Main(string[] args)
        {
            string folderPath = Directory.GetCurrentDirectory();
            SearchOption searchOption = args.Length > 0 && args[0].Equals("false", StringComparison.OrdinalIgnoreCase)
                ? SearchOption.TopDirectoryOnly
                : SearchOption.AllDirectories;

            var combinedContent = new StringBuilder();

            foreach (var extension in SupportedExtensions)
            {
                string[] files = Directory.GetFiles(folderPath, extension, searchOption);

                foreach (var file in files)
                {
                    string parentFolderPath = Path.GetDirectoryName(file);
                    string parentFolderName = Path.GetFileName(parentFolderPath);
                    string fileName = Path.GetFileName(file);

                    if (IsFileExcluded(file, parentFolderName, fileName))
                    {
                        // Log skipped file here
                        continue;
                    }

                    Console.WriteLine($"Adding {file}");

                    combinedContent.AppendLine($"File: {file}");
                    combinedContent.AppendLine(await File.ReadAllTextAsync(file));
                    combinedContent.AppendLine($"--- End of {file} ---");
                }
            }

            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string outputFile = Path.Combine(myDocumentsPath, OutputFileName);

            await File.WriteAllTextAsync(outputFile, combinedContent.ToString());
            Process.Start(new ProcessStartInfo(outputFile) { UseShellExecute = true });

            Console.WriteLine($"Content written to {outputFile} and opened in default editor.");
        }

        private static bool IsFileExcluded(string filePath, string parentFolderName, string fileName)
        {
            return ExcludedFolderNames.Contains(parentFolderName, StringComparer.OrdinalIgnoreCase) ||
                   ExcludedFolderNameSubstrings.Any(substring => parentFolderName.Contains(substring, StringComparison.OrdinalIgnoreCase)) ||
                   ExcludedFileSubstrings.Any(substring => fileName.Contains(substring, StringComparison.OrdinalIgnoreCase)) ||
                   ExcludedFolderNames.Any(excludedFolder => filePath.Contains(excludedFolder, StringComparison.OrdinalIgnoreCase));
        }
    }
}
