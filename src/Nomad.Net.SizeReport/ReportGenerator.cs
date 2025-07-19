namespace Nomad.Net.SizeReport;

using System.Collections.Generic;
using System.IO;

/// <summary>
/// Provides helper methods to generate size comparison results.
/// </summary>
internal static class ReportGenerator
{
    /// <summary>
    /// Gets the size comparison results for the specified directory.
    /// </summary>
    /// <param name="inputDirectory">The directory containing paired JSON and NOMAD files.</param>
    /// <returns>A list of scenario results.</returns>
    public static IReadOnlyList<ScenarioResult> GetScenarioResults(string inputDirectory)
    {
        var results = new List<ScenarioResult>();

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

        return results;
    }
}
