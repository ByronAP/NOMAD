namespace Nomad.Net.SizeReport;

/// <summary>
/// Represents size information for a single scenario.
/// </summary>
/// <param name="Scenario">The scenario name.</param>
/// <param name="JsonSize">The size of the JSON representation in bytes.</param>
/// <param name="NomadSize">The size of the NOMAD representation in bytes.</param>
internal sealed record ScenarioResult(string Scenario, long JsonSize, long NomadSize);

