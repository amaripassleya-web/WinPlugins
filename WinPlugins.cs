using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        // Open a file dialog to select the .plugin file
        string filePath = OpenFileDialog();

        if (string.IsNullOrEmpty(filePath))
        {
            Console.WriteLine("No file selected. Exiting.");
            return;
        }

        Console.WriteLine($"Scanning: {filePath}\n");

        // Commands/keywords to flag
        string[] targetCommands = new string[] { 
            "cmd", "powershell", "pwsh", "cmd.exe", "powershell.exe", 
            "Invoke-Expression", "iex", "Start-Process" 
        };

        // Read lines starting from line 7 (index 6)
        string[] allLines = File.ReadAllLines(filePath);
        var linesFromSevenOnward = allLines.Skip(6);

        int currentLineNumber = 7;
        bool foundMatch = false;

        foreach (var line in linesFromSevenOnward)
        {
            foreach (var command in targetCommands)
            {
                if (line.IndexOf(command, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Console.WriteLine($"[FLAGGED] Line {currentLineNumber}: Found '{command}' -> {line.Trim()}");
                    foundMatch = true;
                }
            }
            currentLineNumber++;
        }

        if (!foundMatch)
        {
            Console.WriteLine("No targeted shell commands found on line 7 or below.");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static string OpenFileDialog()
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Plugin Files (*.plugin)|*.plugin|All Files (*.*)|*.*";
            openFileDialog.Title = "Select a .plugin File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
        }
        return null;
    }
}
