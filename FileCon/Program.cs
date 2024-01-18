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
            string[] fileExtensions = new string[] { "*.cs", "*.md","*.js", "*.jsx","*.ts" }; // Supported extensions
            string folderPath = Directory.GetCurrentDirectory(); // Current directory

            SearchOption searchOption = SearchOption.AllDirectories;
            if (args.Length > 0 && args[0].Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                searchOption = SearchOption.TopDirectoryOnly;
            }
            // Define a list of folder names to exclude
            List<string> excludedFolderNames = new List<string>
            {
                "obj", // Add other folder names to exclude here
                "bin",  // Example: exclude "bin" folder
                "migrations",
                "dist",
                "node_modules"
            };


            // Define a list of substrings to exclude files based on folder name
            List<string> excludedFolderNameSubstrings = new List<string>
            {
                "jquery", // Add substrings to exclude here
                "backup"  // Example: exclude folders containing "temp" or "backup"
            };

            string combinedContent = "";

            foreach (var extension in fileExtensions)
            {
                string[] files = Directory.GetFiles(folderPath, extension, searchOption);

                foreach (var file in files)
                {
                    string parentFolderPath = Path.GetDirectoryName(file);

                    string parentFolderName = Path.GetFileName(parentFolderPath);

                    // Check if any part of the folder path contains an excluded folder name
                    if (excludedFolderNames.Any(excludedFolder => parentFolderName.Equals(excludedFolder, StringComparison.OrdinalIgnoreCase)) ||
                        excludedFolderNameSubstrings.Any(substring => parentFolderName.Contains(substring, StringComparison.OrdinalIgnoreCase)))
                    {
                        Console.WriteLine($"Skipping {file} because it's in an excluded folder.");
                        continue;
                    }

                    // Check if any part of the folder path contains an excluded folder name
                    if (excludedFolderNames.Any(excludedFolder => parentFolderPath.Contains(excludedFolder, StringComparison.OrdinalIgnoreCase)))
                    {
                        Console.WriteLine($"Skipping {file} because it's in an excluded folder.");
                        continue;
                    }

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
