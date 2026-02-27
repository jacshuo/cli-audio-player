using Spectre.Console;
using Spectre.Console.Rendering;

namespace ListenerSharp;

public class PlayerUI
{
    private static readonly string[] LeftSpokes  = ["─", "╱", "│", "╲"];
    private static readonly string[] RightSpokes = ["─", "╲", "│", "╱"];

    // 9-char wide, 11-line tall speaker cabinet
    private static readonly string SpeakerMarkup = string.Join("\n",
    [
        "[red]╔═══════╗[/]",
        "[red]║[/][grey46]▐▌▐▌▐▌▐[/][red]║[/]",
        "[red]║[/][grey46]▌▐▌▐▌▐▌[/][red]║[/]",
        "[red]║[/][grey46]▐▌▐▌▐▌▐[/][red]║[/]",
        "[red]║[/][grey46]▌▐▌▐▌▐▌[/][red]║[/]",
        "[red]╠═══════╣[/]",
        "[red]║[/][white] ╭───╮ [/][red]║[/]",
        "[red]║[/][yellow] │ ● │ [/][red]║[/]",
        "[red]║[/][white] ╰───╯ [/][red]║[/]",
        "[red]║[/][grey46]  ───  [/][red]║[/]",
        "[red]╚═══════╝[/]",
    ]);

    private int _tick;
    private readonly Player _player;

    public PlayerUI(Player player) { _player = player; }

    public IRenderable Render()
    {
        _tick++;
        var root = new Table()
            .Border(TableBorder.Heavy)
            .BorderColor(Color.Red)
            .HideHeaders()
            .Expand();
        root.AddColumn(new TableColumn("content").Padding(0, 0));
        root.AddRow(BuildHeader());
        root.AddRow(BuildPlaylist());
        root.AddRow(BuildNowPlaying());
        root.AddRow(BuildControls());
        return root;
    }

    private IRenderable BuildHeader()
    {
        var tbl = new Table().Border(TableBorder.None).HideHeaders().Expand();
        tbl.AddColumn(new TableColumn("l").Width(11).Padding(0, 0));
        tbl.AddColumn(new TableColumn("c").Padding(0, 0));
        tbl.AddColumn(new TableColumn("r").Width(11).Padding(0, 0));
        tbl.AddRow(new Markup(SpeakerMarkup), BuildCenterGrid(), new Markup(SpeakerMarkup));
        return new Panel(tbl).Border(BoxBorder.Heavy).BorderColor(Color.Red).Expand().Padding(0, 0);
    }

    private IRenderable BuildCenterGrid()
    {
        int frame = _tick % 4;
        string ls = LeftSpokes[frame];
        string rs = RightSpokes[frame];

        int tapeW   = Math.Max(8, Math.Min(60, Console.WindowWidth - 62));
        string tapE = new string('─', tapeW);
        string tapM = BuildScrollingTape(tapeW, _tick, '═', '╪');

        int vol       = _player.Volume;
        int volFilled = (int)Math.Round(vol / 5.0);
        string volBar = new string('█', volFilled) + new string('░', 20 - volFilled);
        string state  = _player.State switch
        {
            PlayerState.Playing => "▶",
            PlayerState.Paused  => "II",
            _                   => "■"
        };

        string[] big = BigFont.GetMarkupRows("LISTENER");

        var grid = new Grid().Expand();
        grid.AddColumn(new GridColumn());

        // Row  1 — starfield top
        grid.AddRow(new Markup("[grey39]· [/][yellow]★[/][grey39] · [/][white]*[/][grey39] · [/][yellow]★[/][grey39] · [/][white]*[/][grey39] · [/][yellow]★[/][grey39] · [/][white]*[/][grey39] · [/][yellow]✦[/][grey39] ·[/]").Centered());

        // Rows 2-5 — fat LISTENER (yellow top row, red body)
        foreach (string row in big[..4])
            grid.AddRow(new Markup(row).Centered());

        // Row  6 — drop-shadow
        grid.AddRow(new Markup(big[4]).Centered());

        // Row  7 — starfield bottom
        grid.AddRow(new Markup("[white]*[/][grey39] · [/][yellow]✦[/][grey39] · [/][white]*[/][grey39] · [/][yellow]★[/][grey39] · [/][white]*[/][grey39] · [/][yellow]✦[/][grey39] · [/][white]*[/][grey39] · [/][yellow]★[/][grey39] ·[/]").Centered());

        // Row  8 — reel top + tape top edge
        grid.AddRow(new Markup($"[red]╭───╮[/][grey46]{tapE}[/][red]╭───╮[/]").Centered());

        // Row  9 — reel spokes + animated tape body
        grid.AddRow(new Markup($"[red]│[/][yellow]{ls}●{ls}[/][red]│[/][red]{tapM}[/][red]│[/][yellow]{rs}●{rs}[/][red]│[/]").Centered());

        // Row 10 — reel bottom + tape bottom edge
        grid.AddRow(new Markup($"[red]╰───╯[/][grey46]{tapE}[/][red]╰───╯[/]").Centered());

        // Row 11 — state + VOL bar (centered)
        grid.AddRow(new Markup($"[bold red]{state}[/]   [grey46]VOL[/] [red]{volBar}[/] [bold white]{vol:D3}%[/]").Centered());

        return grid;
    }

    private static string BuildScrollingTape(int width, int tick, char main, char marker)
    {
        int pos = width - 1 - (tick * 2 % width);
        var chars = new char[width];
        for (int i = 0; i < width; i++) chars[i] = (i == pos) ? marker : main;
        return new string(chars);
    }

    private IRenderable BuildPlaylist()
    {
        var table = new Table().Border(TableBorder.None).HideHeaders().Expand();
        table.AddColumn(new TableColumn("#").Width(6).RightAligned());
        table.AddColumn(new TableColumn("title"));

        for (int i = 0; i < _player.Tracks.Count; i++)
        {
            bool isCurrent = i == _player.CurrentIndex;
            string fileName = Path.GetFileName(_player.Tracks[i]);
            string num      = $"{i + 1:D2}";
            if (isCurrent)
            {
                string g = _player.State switch { PlayerState.Playing => "▶", PlayerState.Paused => "II", _ => "■" };
                table.AddRow(new Markup($"[bold red]{num}[/]"),
                             new Markup($"[bold white on red] {g} {Markup.Escape(fileName.ToUpperInvariant())} [/]"));
            }
            else
            {
                table.AddRow(new Markup($"[grey46]{num}[/]"), new Markup($"[white]{Markup.Escape(fileName)}[/]"));
            }
        }
        if (_player.Tracks.Count == 0)
            table.AddRow(new Markup(""), new Markup("[grey46]  No audio files found.[/]"));

        return new Panel(table)
            .Header(new PanelHeader(" [bold red]QUEUE[/] ", Justify.Left))
            .Border(BoxBorder.Heavy).BorderColor(Color.Red).Expand().Padding(1, 0);
    }

    private IRenderable BuildNowPlaying()
    {
        long currentMs = _player.CurrentTimeMs;
        long totalMs   = _player.TotalTimeMs;
        string cur     = FormatTime(currentMs);
        string rem     = FormatTime(totalMs > currentMs ? totalMs - currentMs : 0);
        string track   = _player.CurrentIndex >= 0
                         ? Path.GetFileName(_player.Tracks[_player.CurrentIndex]).ToUpperInvariant()
                         : "── NO TRACK SELECTED ──";
        double frac    = (totalMs > 0) ? Math.Clamp((double)currentMs / totalMs, 0.0, 1.0) : 0.0;
        string g       = _player.State switch { PlayerState.Playing => "▶", PlayerState.Paused => "II", _ => "■" };
        const int bw   = 38;
        int filled     = (int)Math.Round(frac * bw);
        string bar     = new string('█', filled) + new string('░', bw - filled);

        var grd = new Grid().Expand();
        grd.AddColumn(new GridColumn());
        grd.AddRow(new Markup($"[bold yellow] {g}  {Markup.Escape(track)} [/]").Centered());
        grd.AddRow(new Markup($"[bold red]{cur}[/]  [red]{bar}[/]  [grey46]{rem}[/]").Centered());

        return new Panel(grd)
            .Header(new PanelHeader(" [bold red]NOW PLAYING[/] ", Justify.Left))
            .Border(BoxBorder.Heavy).BorderColor(Color.Red).Expand().Padding(1, 0);
    }

    private static IRenderable BuildControls() =>
        new Markup(
            "[grey46][[SPACE]][/] [white]PLAY/PAUSE[/]  " +
            "[grey46][[N]][/] [white]NEXT[/]  " +
            "[grey46][[B]][/] [white]PREV[/]  " +
            "[grey46][[←]][/] [white]-10s[/]  " +
            "[grey46][[→]][/] [white]+10s[/]  " +
            "[grey46][[↑]][/][grey46]/[/][grey46][[↓]][/] [white]VOL[/]  " +
            "[grey46][[Q]][/] [white]QUIT[/]").Centered();

    private static string FormatTime(long ms)
    {
        if (ms < 0) ms = 0;
        var ts = TimeSpan.FromMilliseconds(ms);
        return ts.Hours > 0
            ? $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}"
            : $"{ts.Minutes:D2}:{ts.Seconds:D2}";
    }
}
