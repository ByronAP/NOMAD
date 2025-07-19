using System;
using System.Collections.Generic;
using System.IO;

namespace Nomad.Net.SizeReport;

/// <summary>
/// Entry point for the NOMAD size reporting tool.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Generates a table comparing NOMAD file sizes with their JSON equivalents.
    /// </summary>
    /// <param name="args">The first argument is the input directory. The second argument is an optional output file.</param>
    public static void Main(string[] args)
    {
        string inputDirectory = args.Length > 0 ? args[0] : Path.Combine("..", "examples");
        string? outputFile = args.Length > 1 ? args[1] : null;

        List<ScenarioResult> results = new();
        foreach (string jsonPath in Directory.EnumerateFiles(inputDirectory, "*.json", SearchOption.AllDirectories))
        {
            string nomadPath = Path.ChangeExtension(jsonPath, ".nmd");
            if (!File.Exists(nomadPath))
            {
                continue;
            }

            long jsonSize = new FileInfo(jsonPath).Length;
            long nomadSize = new FileInfo(nomadPath).Length;
            string scenarioName = Path.GetFileNameWithoutExtension(jsonPath);
            results.Add(new ScenarioResult(scenarioName, jsonSize, nomadSize));
        }

        var lines = new List<string>
        {
            "| Scenario | JSON Size | NOMAD Size | Reduction |",
            "|---------|----------|-----------|----------|",
        };

        foreach (ScenarioResult result in results)
        {
            double reduction = 1d - ((double)result.NomadSize / result.JsonSize);
            lines.Add($"| {result.Scenario} | {result.JsonSize} B | {result.NomadSize} B | {reduction:P0} |");
        }

        foreach (string line in lines)
        {
            Console.WriteLine(line);
        }

        if (!string.IsNullOrEmpty(outputFile))
        {
            File.WriteAllLines(outputFile, lines);
        }
    }
}
