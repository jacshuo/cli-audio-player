using System.Text;
using ListenerSharp;
using Spectre.Console;

// Force UTF-8 output so Unicode block/box chars render correctly on Windows
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// ─── Argument handling ────────────────────────────────────────────────────────
if (args.Length == 0)
{
    AnsiConsole.MarkupLine("[bold red]LISTENER[/] [grey46]// punk audio machine[/]");
    AnsiConsole.MarkupLine("[grey46]usage:[/]  [white]listen [bold]<path>[/][/]");
    AnsiConsole.MarkupLine("[grey46]example:[/] [white]listen ~/Music/punk[/]");
    return 1;
}

string inputPath = args[0];

if (!Directory.Exists(inputPath) && !File.Exists(inputPath))
{
    AnsiConsole.MarkupLine($"[bold red]ERROR:[/] Path not found: [white]{Markup.Escape(inputPath)}[/]");
    return 1;
}

// ─── Collect audio files ─────────────────────────────────────────────────────
string[] supportedExtensions = [".mp3", ".aac", ".flac", ".wav", ".ogg", ".m4a", ".opus", ".wma"];

List<string> tracks;

if (File.Exists(inputPath))
{
    string ext = Path.GetExtension(inputPath).ToLowerInvariant();
    if (!supportedExtensions.Contains(ext))
    {
        AnsiConsole.MarkupLine($"[bold red]ERROR:[/] Unsupported file type: [white]{Markup.Escape(ext)}[/]");
        return 1;
    }
    tracks = [inputPath];
}
else
{
    tracks = Directory
        .EnumerateFiles(inputPath, "*.*", SearchOption.TopDirectoryOnly)
        .Where(f => supportedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
        .OrderBy(f => f)
        .ToList();
}

if (tracks.Count == 0)
{
    AnsiConsole.MarkupLine($"[bold red]ERROR:[/] No audio files found in [white]{Markup.Escape(inputPath)}[/]");
    AnsiConsole.MarkupLine($"[grey46]Supported: {string.Join(", ", supportedExtensions)}[/]");
    return 1;
}

// ─── Launch player ────────────────────────────────────────────────────────────
Console.CursorVisible = false;
Console.Clear();

using var player = new Player(tracks);
var ui = new PlayerUI(player);

// Start first track
player.Play(0);

// ─── Key input loop ───────────────────────────────────────────────────────────
var cts = new CancellationTokenSource();

// Key handler runs on a dedicated thread so it never blocks the live update
var keyTask = Task.Run(() =>
{
    while (!cts.Token.IsCancellationRequested)
    {
        if (!Console.KeyAvailable)
        {
            Thread.Sleep(20);
            continue;
        }
        var key = Console.ReadKey(intercept: true);
        switch (key.Key)
        {
            case ConsoleKey.Spacebar:
            case ConsoleKey.P:
                player.PlayPause();
                break;
            case ConsoleKey.N:
                player.Next();
                break;
            case ConsoleKey.B:
                player.Previous();
                break;
            case ConsoleKey.UpArrow:
                player.Volume = Math.Min(player.Volume + 5, 100);
                break;
            case ConsoleKey.DownArrow:
                player.Volume = Math.Max(player.Volume - 5, 0);
                break;
            case ConsoleKey.RightArrow:
                player.Seek(10_000);
                break;
            case ConsoleKey.LeftArrow:
                player.Seek(-10_000);
                break;
            case ConsoleKey.Q:
            case ConsoleKey.Escape:
                cts.Cancel();
                break;
        }
    }
});

// ─── Live UI update loop ──────────────────────────────────────────────────────
await AnsiConsole.Live(ui.Render())
    .AutoClear(false)
    .Overflow(VerticalOverflow.Ellipsis)
    .StartAsync(async ctx =>
    {
        while (!cts.Token.IsCancellationRequested)
        {
            ctx.UpdateTarget(ui.Render());
            await Task.Delay(200, CancellationToken.None);
        }
    });

player.Stop();
Console.CursorVisible = true;
Console.Clear();
AnsiConsole.MarkupLine("[bold red]LISTENER[/] [grey46]// session ended. stay punk.[/]");

await keyTask;
return 0;
