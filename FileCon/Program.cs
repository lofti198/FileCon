using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FileConcatenator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string fileExtension = "*.cs"; // Change to your desired extension
            string folderPath = Directory.GetCurrentDirectory(); // Current directory

            string[] files = Directory.GetFiles(folderPath, fileExtension);
            string combinedContent = "";

            foreach (var file in files)
            {
                combinedContent += await File.ReadAllTextAsync(file) + Environment.NewLine;
            }

            // Define the output file path in the user's "My Documents" folder
            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string outputFile = Path.Combine(myDocumentsPath, "join.txt");

            // Save the combined content to the output file, overwriting if it already exists
            await File.WriteAllTextAsync(outputFile, combinedContent);

            // Open the file in the default editor
            Process.Start(new ProcessStartInfo(outputFile) { UseShellExecute = true });

            Console.WriteLine($"Content written to {outputFile} and opened in default editor.");
            Console.ReadKey();
        }
    }
}
