namespace Nomad.Net.SizeReport;

using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Entry point for the NOMAD size reporting tool.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Generates size comparison tables for each example set.
    /// </summary>
    /// <param name="args">The first argument is the root directory containing example sets. The second argument is an optional output file.</param>
    public static void Main(string[] args)
    {
        string inputDirectory = args.Length > 0 ? args[0] : Path.Combine("..", "examples");
        string? outputFile = args.Length > 1 ? args[1] : null;

        var lines = new List<string>();

        foreach (string setDirectory in Directory.GetDirectories(inputDirectory))
        {
            string setName = Path.GetFileName(setDirectory);
            IReadOnlyList<ScenarioResult> results = ReportGenerator.GetScenarioResults(setDirectory);
            if (results.Count == 0)
            {
                continue;
            }

            lines.Add($"### {setName}");
            lines.Add("| Scenario | JSON Size | NOMAD Size | Reduction |");
            lines.Add("|---------|----------|-----------|----------|");

            foreach (ScenarioResult result in results)
            {
                double reduction = 1d - ((double)result.NomadSize / result.JsonSize);
                lines.Add($"| {result.Scenario} | {result.JsonSize} B | {result.NomadSize} B | {reduction:P0} |");
            }

            lines.Add(string.Empty);
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
