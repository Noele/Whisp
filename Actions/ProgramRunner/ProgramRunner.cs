using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;
using Whisp.Actions.ProgramRunner.Models;

namespace Whisp.Actions.ProgramRunner;

public class ProgramRunner
{
    private const string FilePath = @"./apps.json";
    private List<UserProgram> _programs;
    public ProgramRunner()
    {
        if (!File.Exists(FilePath))
        {
            throw new FileNotFoundException("Could not find the applications file, this needs to be included to allow whisp to start programs.", FilePath);
        }
        var text = File.ReadAllText(FilePath);
        List<UserProgram>? json = JsonSerializer.Deserialize<List<UserProgram>>(text);
        if (json == null)
        {
            throw new FileLoadException("The apps.json file is malformed", FilePath);
        }
        this._programs = json;
    }
    
    public void Run(string programName)
    {
        // Extract program names from the list of programs
        var programNames = new List<string>();
        foreach (var pro in this._programs)
        {
            programNames.Add(pro.name);
        }
        
        var bestMatch = FuzzySharp.Process.ExtractOne(programName, programNames);

        if (bestMatch != null && bestMatch.Score > 70) // threshold, tweak if needed
        {
            var matchedProgram = this._programs.FirstOrDefault(p => p.name == bestMatch.Value);

            if (matchedProgram != null)
            {
                string pathToLaunch = matchedProgram.path;
                Console.WriteLine($"Launching {matchedProgram.name} at {pathToLaunch}");

                var psi = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"\"{pathToLaunch}\"",
                    UseShellExecute = true
                };

                System.Diagnostics.Process.Start(psi);
            }
        }
        else
        {
            Console.WriteLine("No close match found.");
        }
    }
}
