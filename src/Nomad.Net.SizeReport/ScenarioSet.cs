namespace Nomad.Net.SizeReport;

using System.Collections.Generic;

/// <summary>
/// Represents a group of scenarios with their size comparison results.
/// </summary>
/// <param name="Name">The scenario set name.</param>
/// <param name="Results">The results within the set.</param>
internal sealed record ScenarioSet(string Name, IReadOnlyList<ScenarioResult> Results);
