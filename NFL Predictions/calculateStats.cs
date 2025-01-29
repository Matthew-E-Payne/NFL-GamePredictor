using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string defensePassingFile = "NFL-defense-passing.xml";
        string offensePassingFile = "NFL-offense-passing.xml";

        List<TeamStats> defensePassingTeams = FetchTeamStats<DefensePassingTeamStats>(defensePassingFile);
        List<TeamStats> offensePassingTeams = FetchTeamStats<OffensePassingTeamStats>(offensePassingFile);

        Console.WriteLine("NFL Team Passing Stats:");
        var lionsOffense = offensePassingTeams.FirstOrDefault(team => team.Name == "Bills");
        var offensiveYards = lionsOffense.Yards;

        var lionsDefense = defensePassingTeams.FirstOrDefault(team => team.Name == "Bills");
        var defensiveYards = lionsDefense.Yards;

        Console.WriteLine($"Lions TD: {lionsOffense.TD}");
        Console.WriteLine($"Lions TD: {lionsDefense.TD}");
    }

    // Fetch Offensive TeamStats
    static List<TeamStats> FetchTeamStats<T>(string fileName) where T : TeamStats, new()
    {
        string filePath = GetFilePath(fileName);
        XDocument xmlDoc = XDocument.Load(filePath);

        return xmlDoc.Descendants("Team")
            .Select(team => new T
            {
                Name = team.Element("Name")?.Value ?? "Unknown",
                Attempts = team.Element("Att")?.Value ?? "0",
                Completions = team.Element("Cmp")?.Value ?? "0",
                CompletionPercentage = team.Element("CmpPercent")?.Value ?? "0%",
                YardsPerAttempt = team.Element("YdsPerAtt")?.Value ?? "0",
                Yards = team.Element(typeof(T) == typeof(OffensePassingTeamStats) ? "PassYds" : "Yds")?.Value ?? "0",
                TD = team.Element("TD")?.Value ?? "0",
                INT = team.Element("INT")?.Value ?? "0",
                Rating = team.Element("Rate")?.Value ?? "0",
                First = team.Element("First")?.Value ?? "0",
                FirstPercentage = team.Element("FirstPercent")?.Value ?? "0%",
                TwentyPlus = team.Element("TwentyPlus")?.Value ?? "0",
                FortyPlus = team.Element("FortyPlus")?.Value ?? "0",
                Longest = team.Element("Lng")?.Value ?? "0",
                Sacks = team.Element("Sck")?.Value ?? "0"
            }).Cast<TeamStats>().ToList();
    }

    static string GetFilePath(string fileName)
    {
        string projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
        return Path.Combine(projectRoot, "data", fileName);
    }
}

// Base class to avoid duplicating properties
abstract class TeamStats
{
    public string Name { get; set; }
    public string Attempts { get; set; }
    public string Completions { get; set; }
    public string CompletionPercentage { get; set; }
    public string YardsPerAttempt { get; set; }
    public string Yards { get; set; }
    public string TD { get; set; }
    public string INT { get; set; }
    public string Rating { get; set; }
    public string First { get; set; }
    public string FirstPercentage { get; set; }
    public string TwentyPlus { get; set; }
    public string FortyPlus { get; set; }
    public string Longest { get; set; }
    public string Sacks { get; set; }
}

// Classes to inherit teamStats but store either defense or offense
class DefensePassingTeamStats : TeamStats { }
class OffensePassingTeamStats : TeamStats { }
