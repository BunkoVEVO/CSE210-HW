using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

abstract class Goal
{
    public string Name { get; set; }
    public int Points { get; set; }
    public bool Completed { get; protected set; }

    public abstract int RecordEvent();
    public virtual bool IsCompleted() => Completed;

    public override string ToString()
    {
        string status = Completed ? "[X]" : "[ ]";
        return $"{status} {Name} - {Points} points";
    }
}

class SimpleGoal : Goal
{
    public override int RecordEvent()
    {
        Completed = true;
        return Points;
    }
}

class EternalGoal : Goal
{
    public override int RecordEvent()
    {
        return Points;
    }
}

class ChecklistGoal : Goal
{
    public int TargetCount { get; set; }
    public int CurrentCount { get; set; }
    public int BonusPoints { get; set; }

    public override int RecordEvent()
    {
        CurrentCount++;
        int totalPoints = Points;

        if (CurrentCount >= TargetCount)
        {
            Completed = true;
            totalPoints += BonusPoints;
        }

        return totalPoints;
    }

    public override string ToString()
    {
        string status = Completed ? "[X]" : "[ ]";
        return $"{status} {Name} - {Points} points (Completed {CurrentCount}/{TargetCount})";
    }
}

class EternalQuest
{
    public List<Goal> Goals { get; set; } = new List<Goal>();
    public int Score { get; set; } = 0;
    public int Level { get; set; } = 1;

    public void AddGoal(Goal goal)
    {
        Goals.Add(goal);
    }

    public void RecordEvent(int goalIndex)
    {
        if (goalIndex < 0 || goalIndex >= Goals.Count) return;

        int points = Goals[goalIndex].RecordEvent();
        Score += points;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        int[] levels = { 100, 500, 1000, 2000 };
        foreach (int level in levels)
        {
            if (Score >= level)
            {
                Level = Array.IndexOf(levels, level) + 2;
            }
        }
    }

    public void ShowGoals()
    {
        for (int i = 0; i < Goals.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {Goals[i]}");
        }
    }

    public void ShowScore()
    {
        Console.WriteLine($"Total Score: {Score}, Level: {Level}");
    }

    public void SaveProgress(string filename)
    {
        var data = new
        {
            Score,
            Goals
        };

        File.WriteAllText(filename, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
    }

    public void LoadProgress(string filename)
    {
        if (!File.Exists(filename)) return;

        var data = JsonSerializer.Deserialize<dynamic>(File.ReadAllText(filename));
        Score = data.GetProperty("Score").GetInt32();

        Goals.Clear();
        foreach (var goalData in data.GetProperty("Goals").EnumerateArray())
        {
            string name = goalData.GetProperty("Name").GetString();
            int points = goalData.GetProperty("Points").GetInt32();
            bool completed = goalData.GetProperty("Completed").GetBoolean();

            if (goalData.TryGetProperty("CurrentCount", out var currentCount))
            {
                int targetCount = goalData.GetProperty("TargetCount").GetInt32();
                int bonusPoints = goalData.GetProperty("BonusPoints").GetInt32();
                var goal = new ChecklistGoal
                {
                    Name = name,
                    Points = points,
                    Completed = completed,
                    TargetCount = targetCount,
                    CurrentCount = currentCount.GetInt32(),
                    BonusPoints = bonusPoints
                };
                Goals.Add(goal);
            }
            else if (points == 100)
            {
                var goal = new EternalGoal
                {
                    Name = name,
                    Points = points,
                    Completed = completed
                };
                Goals.Add(goal);
            }
            else
            {
                var goal = new SimpleGoal
                {
                    Name = name,
                    Points = points,
                    Completed = completed
                };
                Goals.Add(goal);
            }
        }
    }
}

class Program
{
    static void Main()
    {
        var quest = new EternalQuest();

        // Example goals
        quest.AddGoal(new SimpleGoal { Name = "Run a marathon", Points = 1000 });
        quest.AddGoal(new EternalGoal { Name = "Read scriptures", Points = 100 });
        quest.AddGoal(new ChecklistGoal { Name = "Attend temple", Points = 50, TargetCount = 10, BonusPoints = 500 });

        // Main loop
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nEternal Quest");
            quest.ShowGoals();
            quest.ShowScore();
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1. Record Goal Event");
            Console.WriteLine("2. Add New Goal");
            Console.WriteLine("3. Save Progress");
            Console.WriteLine("4. Load Progress");
            Console.WriteLine("5. Exit");

            Console.Write("\nSelect an option: ");
            int choice = int.Parse(Console.ReadLine() ?? "0");

            switch (choice)
            {
                case 1:
                    Console.Write("Enter the goal number to record an event: ");
                    int goalIndex = int.Parse(Console.ReadLine() ?? "0") - 1;
                    quest.RecordEvent(goalIndex);
                    break;
                case 2:
                    AddNewGoal(quest);
                    break;
                case 3:
                    quest.SaveProgress("progress.json");
                    Console.WriteLine("Progress saved!");
                    break;
                case 4:
                    quest.LoadProgress("progress.json");
                    Console.WriteLine("Progress loaded!");
                    break;
                case 5:
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option, please try again.");
                    break;
            }
        }
    }

    static void AddNewGoal(EternalQuest quest)
    {
        Console.WriteLine("\n1. Simple Goal");
        Console.WriteLine("2. Eternal Goal");
        Console.WriteLine("3. Checklist Goal");
        Console.Write("Choose goal type: ");
        int type = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Enter goal name: ");
        string name = Console.ReadLine() ?? "";
        Console.Write("Enter points: ");
        int points = int.Parse(Console.ReadLine() ?? "0");

        switch (type)
        {
            case 1:
                quest.AddGoal(new SimpleGoal { Name = name, Points = points });
                break;
            case 2:
                quest.AddGoal(new EternalGoal { Name = name, Points = points });
                break;
            case 3:
                Console.Write("Enter target count: ");
                int targetCount = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Enter bonus points: ");
                int bonusPoints = int.Parse(Console.ReadLine() ?? "0");
                quest.AddGoal(new ChecklistGoal { Name = name, Points = points, TargetCount = targetCount, BonusPoints = bonusPoints });
                break;
            default:
                Console.WriteLine("Invalid goal type.");
                break;
        }
    }
}
