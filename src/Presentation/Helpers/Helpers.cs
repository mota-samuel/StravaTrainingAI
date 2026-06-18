namespace Presentation.Helpers;
public static class Helpers
{
    public static string GetEnvRequired(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if(string.IsNullOrEmpty(value))
        {
           Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Variável '{name}' não configurada.");
            Console.ResetColor();
            Environment.Exit(1);
        }
        return value!;
    }

    public static void OpenBrowser(string url)
    {
        try
        {
           if (OperatingSystem.IsWindows())
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
            else if (OperatingSystem.IsMacOS())
                System.Diagnostics.Process.Start("open", url);
            else 
                System.Diagnostics.Process.Start("xdg-open", url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Não foi possível abrir o navegador: {ex.Message}");
            Console.WriteLine($"Cole a URL no navegador manualmente: {url}");
        }
    }

    public static void PrintBanner()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("""
    ┌──────────────────────────────────────────────────┐
    │       🏃 STRAVA TRAINING AI  🤖 Gemini           │
    │  Planos de treino personalizados — 100% gratuito  │
    └──────────────────────────────────────────────────┘
    """);
        Console.ResetColor();

    }
}
