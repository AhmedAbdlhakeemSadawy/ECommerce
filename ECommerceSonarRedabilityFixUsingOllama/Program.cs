using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using OllamaSharp;

class Program
{
    static async Task Main(string[] args)
    {
        // === CONFIG ===
        string jsonPath = @"C:\Projects\ECommerce\maintainability.json";   // ← change this
        string ollamaUrl = "http://localhost:11434";                    // default Docker
        string model = "deepseek-coder:6.7b";                // or your model
        string outputFile = "suggested_fixes.md";                        // where results go

        // === Load SonarQube JSON ===
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"File not found: {jsonPath}");
            return;
        }

        string json = await File.ReadAllTextAsync(jsonPath);
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        JsonElement issues = root.GetProperty("issues");

        // === Connect to Ollama ===
        var uri = new Uri(ollamaUrl);
        var ollama = new OllamaApiClient(uri) { SelectedModel = model };

        Console.WriteLine($"Connected to Ollama → Model: {model}\nProcessing {issues.GetArrayLength()} issues...\n");

        // Prepare markdown file
        await File.WriteAllTextAsync(outputFile, $"# SonarQube Readability Fixes ({DateTime.Now:yyyy-MM-dd})\n\n");

        int count = 0;
        foreach (JsonElement issue in issues.EnumerateArray())
        {
            count++;
            string? file = issue.TryGetProperty("component", out var c) ? c.GetString()?.Split(':').Last() : null;
            string? line = issue.TryGetProperty("line", out var l) ? l.GetInt32().ToString() : "—";
            string? message = issue.TryGetProperty("message", out var m) ? m.GetString() : null;
            string? rule = issue.TryGetProperty("rule", out var r) ? r.GetString() : null;

            // Optional: only process readability / complexity issues
            // Only process maintainability issues (code smells)
            string issueType = issue.TryGetProperty("type", out var typ) ? typ.GetString() : null;
            if (file.Contains("result-reports"))
                continue;


            if (!string.Equals(issueType, "CODE_SMELL", StringComparison.OrdinalIgnoreCase))
                continue;

            // Optional: skip very low-severity if you want
            string severity = issue.TryGetProperty("severity", out var sev) ? sev.GetString() : null;
            if (severity == "INFO")  // or "MINOR" if too noisy
                continue;

            Console.WriteLine($"[{count}] {rule} → {file}:{line}  {message}");


            // Build strong prompt for C# refactoring
            string prompt = $@"You are a senior .NET / C# architect focused on **maintainability**, low technical debt, and clean, long-term sustainable code.

SonarQube flagged this maintainability issue (code smell):
Rule:     {rule ?? "unknown"}
File:     {file ?? "unknown file"}
Line:     {line ?? "—"}
Message:  {message ?? "No message available"}

This issue contributes to higher technical debt, harder future changes, increased risk of bugs during maintenance, and slower onboarding.

Your task:
1. In 3–5 sentences, explain:
   - Why this issue harms maintainability (e.g., high cognitive complexity → brain strain during changes, long method → single responsibility violation, deep nesting → hard to test/debug).
   - How it increases SonarQube's technical debt or affects the maintainability rating.

2. Provide a refactored version of the affected code/block that:
   - Significantly improves maintainability
   - Reduces Cognitive Complexity / lines / nesting / duplication
   - Follows modern C# best practices (.NET 6–9 style: records, primary constructors, pattern matching, minimal APIs where appropriate, guard clauses, early returns, extracted small focused methods)
   - Keeps (or improves) the original behavior and performance characteristics
   - Is realistic to apply in an existing .NET Core / .NET project

3. Strictly use this output format — nothing else:

### Maintainability Impact
[Your 3–5 sentence explanation]

### Refactored Code
```csharp
// the improved method(s) / class fragment / full block
// include using directives / namespace / class context ONLY if truly needed";

            try
            {
                string fullResponse = string.Empty;

                await foreach (var chunk in ollama.GenerateAsync(prompt))
                {
                    fullResponse += chunk.Response ?? string.Empty;
                    // Optional: show progress in console
                    // Console.Write(chunk.Response);  // streams token-by-token to console if you want live feedback
                }

                string result = $"## Issue {count}: {rule} – {file}:{line}\n**Message:** {message}\n\n{fullResponse}\n\n---\n\n";

                await File.AppendAllTextAsync(outputFile, result);
                Console.WriteLine("   → Suggestion saved\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   → Error: {ex.Message}\n");
            }
        }
        Console.WriteLine($"\nDone! Check file: {Path.GetFullPath(outputFile)}");
    }
}