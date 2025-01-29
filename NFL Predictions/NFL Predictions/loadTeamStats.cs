using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

class loadTeamStats
{
    static void Main()
    {
        List<DefensePassingTeamStats> defensePassingTeams = FetchTeamStats<DefensePassingTeamStats>();
        List<OffensePassingTeamStats> offensePassingTeams = FetchTeamStats<OffensePassingTeamStats>();
        List<DefenseRushingTeamStats> defenseRushingTeams = FetchTeamStats<DefenseRushingTeamStats>();
        List<OffenseRushingTeamStats> offenseRushingTeams = FetchTeamStats<OffenseRushingTeamStats>();

        Console.WriteLine("NFL Team Stats:");

        var billsOffensePassing = offensePassingTeams.FirstOrDefault(team => team.Name == "Bills");
        var billsDefensePassing = defensePassingTeams.FirstOrDefault(team => team.Name == "Bills");
        var billsOffenseRushing = offenseRushingTeams.FirstOrDefault(team => team.Name == "Bills");
        var billsDefenseRushing = defenseRushingTeams.FirstOrDefault(team => team.Name == "Bills");

        Console.WriteLine($"Bills Passing TD (Offense): {billsOffensePassing?.TD}");
        Console.WriteLine($"Bills Passing TD (Defense): {billsDefensePassing?.TD}");
        Console.WriteLine($"Bills Rushing TD (Offense): {billsOffenseRushing?.TD}");
        Console.WriteLine($"Bills Rushing TD (Defense): {billsDefenseRushing?.TD}");
    }

    // Generic function to fetch team stats
    public static List<T> FetchTeamStats<T>() where T : TeamStats, new()
    {
        string fileName = GetFileName<T>(); // Get the file name based on type
        string filePath = GetFilePath(fileName);
        XDocument xmlDoc = XDocument.Load(filePath);

        return xmlDoc.Descendants("Team")
            .Select(team =>
            {
                var t = new T
                {
                    Name = team.Element("Name")?.Value ?? "Unknown",
                    Attempts = team.Element("Att")?.Value ?? "0",
                    TD = team.Element("TD")?.Value ?? "0",
                    TwentyPlus = team.Element("TwentyPlus")?.Value ?? "0",
                    FortyPlus = team.Element("FortyPlus")?.Value ?? "0",
                    Longest = team.Element("Lng")?.Value ?? "0"
                };

                if (t is OffensePassingTeamStats offensePassing)
                {
                    offensePassing.Yards = team.Element("PassYds")?.Value ?? "0";
                    offensePassing.Completions = team.Element("Cmp")?.Value ?? "0";
                    offensePassing.CompletionPercentage = team.Element("CmpPercent")?.Value ?? "0%";
                    offensePassing.YardsPerAttempt = team.Element("YdsPerAtt")?.Value ?? "0";
                    offensePassing.INT = team.Element("INT")?.Value ?? "0";
                    offensePassing.Rating = team.Element("Rate")?.Value ?? "0";
                    offensePassing.First = team.Element("First")?.Value ?? "0";
                    offensePassing.FirstPercentage = team.Element("FirstPercent")?.Value ?? "0%";
                    offensePassing.Sacks = team.Element("Sck")?.Value ?? "0";
                }
                else if (t is DefensePassingTeamStats defensePassing)
                {
                    defensePassing.Yards = team.Element("Yds")?.Value ?? "0";
                    defensePassing.Completions = team.Element("Cmp")?.Value ?? "0";
                    defensePassing.CompletionPercentage = team.Element("CmpPercent")?.Value ?? "0%";
                    defensePassing.YardsPerAttempt = team.Element("YdsPerAtt")?.Value ?? "0";
                    defensePassing.INT = team.Element("INT")?.Value ?? "0";
                    defensePassing.Rating = team.Element("Rate")?.Value ?? "0";
                    defensePassing.First = team.Element("First")?.Value ?? "0";
                    defensePassing.FirstPercentage = team.Element("FirstPercent")?.Value ?? "0%";
                    defensePassing.Sacks = team.Element("Sck")?.Value ?? "0";
                }
                else if (t is OffenseRushingTeamStats offenseRushing)
                {
                    offenseRushing.Yards = team.Element("RushYds")?.Value ?? "0";
                    offenseRushing.YardsPerAttempt = team.Element("YPC")?.Value ?? "0";
                    offenseRushing.First = team.Element("RushFirst")?.Value ?? "0";
                    offenseRushing.FirstPercentage = team.Element("RushFirstPercent")?.Value ?? "0%";
                    offenseRushing.Fumbles = team.Element("RushFUM")?.Value ?? "0";
                }
                else if (t is DefenseRushingTeamStats defenseRushing)
                {
                    defenseRushing.Yards = team.Element("RushYds")?.Value ?? "0";
                    defenseRushing.YardsPerAttempt = team.Element("YPC")?.Value ?? "0";
                    defenseRushing.First = team.Element("RushFirst")?.Value ?? "0";
                    defenseRushing.FirstPercentage = team.Element("RushFirstPercent")?.Value ?? "0%";
                    defenseRushing.Fumbles = team.Element("RushFUM")?.Value ?? "0";
                }

                return t;
            })
            .ToList();
    }

    private static string GetFileName<T>()
    {
        string projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
        string dataFolder = Path.Combine(projectRoot, "data");

        if (typeof(T) == typeof(DefensePassingTeamStats))
            return Path.Combine(dataFolder, "NFL-defense-passing.xml");
        if (typeof(T) == typeof(OffensePassingTeamStats))
            return Path.Combine(dataFolder, "NFL-offense-passing.xml");
        if (typeof(T) == typeof(DefenseRushingTeamStats))
            return Path.Combine(dataFolder, "NFL-defense-rushing.xml");
        if (typeof(T) == typeof(OffenseRushingTeamStats))
            return Path.Combine(dataFolder, "NFL-offense-rushing.xml");

        throw new InvalidOperationException("Unknown team stats type");
    }

    private static string GetFilePath(string fileName)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), fileName);
    }

    // Base class for stats
    public abstract class TeamStats
    {
        public string Name { get; set; }
        public string Attempts { get; set; }
        public string Completions { get; set; }
        public string Yards { get; set; }
        public string TD { get; set; }
        public string TwentyPlus { get; set; }
        public string FortyPlus { get; set; }
        public string Longest { get; set; }
        public string CompletionPercentage { get; set; }
        public string YardsPerAttempt { get; set; }
        public string INT { get; set; }
        public string Rating { get; set; }
        public string First { get; set; }
        public string FirstPercentage { get; set; }
        public string Sacks { get; set; }
        public string Fumbles { get; set; }
    }

    // Derived classes for different stat types
    public class DefensePassingTeamStats : TeamStats { }
    public class OffensePassingTeamStats : TeamStats { }
    public class DefenseRushingTeamStats : TeamStats { }
    public class OffenseRushingTeamStats : TeamStats { }
}
