namespace ListenerSharp;

/// <summary>
/// Renders text as fat 5×4 block letters using Unicode full-block (█) characters.
/// Returns 5 markup strings: rows 0-3 are the letter body, row 4 is a drop-shadow.
/// </summary>
public static class BigFont
{
    // Each letter: exactly 5 chars wide, 4 rows tall
    private static readonly Dictionary<char, string[]> Letters = new()
    {
        ['L'] = ["█    ", "█    ", "█    ", "█████"],
        ['I'] = ["█████", "  █  ", "  █  ", "█████"],
        ['S'] = ["████ ", "███  ", "  ███", " ████"],
        ['T'] = ["█████", "  █  ", "  █  ", "  █  "],
        ['E'] = ["█████", "████ ", "█    ", "█████"],
        ['N'] = ["█   █", "██  █", "█  ██", "█   █"],
        ['R'] = ["████ ", "█   █", "████ ", "█   █"],
        [' '] = ["     ", "     ", "     ", "     "],
        ['A'] = [" ███ ", "█   █", "█████", "█   █"],
        ['C'] = [" ████", "█    ", "█    ", " ████"],
        ['D'] = ["████ ", "█   █", "█   █", "████ "],
        ['G'] = [" ████", "█    ", "█  ██", " ████"],
        ['H'] = ["█   █", "█████", "█   █", "█   █"],
        ['K'] = ["█   █", "████ ", "████ ", "█   █"],
        ['M'] = ["█   █", "█████", "█ █ █", "█   █"],
        ['O'] = [" ███ ", "█   █", "█   █", " ███ "],
        ['P'] = ["████ ", "█   █", "████ ", "█    "],
        ['U'] = ["█   █", "█   █", "█   █", " ███ "],
        ['0'] = [" ███ ", "█   █", "█   █", " ███ "],
        ['1'] = [" ██  ", " ██  ", " ██  ", "████ "],
        ['2'] = ["████ ", "  ██ ", " ██  ", "█████"],
        ['3'] = ["████ ", "  ██ ", "  ██ ", "████ "],
    };

    /// <summary>
    /// Returns 5 Spectre markup strings (rows 0-3 = letter body, row 4 = drop shadow).
    /// </summary>
    public static string[] GetMarkupRows(
        string text,
        string topColor    = "bold yellow",
        string bodyColor   = "bold red",
        string shadowColor = "grey23")
    {
        text = text.ToUpperInvariant();
        var result = new string[5];

        for (int row = 0; row < 4; row++)
        {
            string color = (row == 0) ? topColor : bodyColor;
            var sb = new System.Text.StringBuilder();
            foreach (char c in text)
            {
                string[] art = Letters.GetValueOrDefault(c, Letters[' ']);
                foreach (char ch in art[row])
                    sb.Append(ch == '█' ? $"[{color}]█[/]" : " ");
                sb.Append(' '); // 1-char gap between letters
            }
            result[row] = sb.ToString().TrimEnd();
        }

        // Drop-shadow row: bottom row shifted 1 right, rendered as ▓
        var shsb = new System.Text.StringBuilder(" "); // 1-char right offset
        foreach (char c in text)
        {
            string[] art = Letters.GetValueOrDefault(c, Letters[' ']);
            foreach (char ch in art[3]) // use bottom row as shadow source
                shsb.Append(ch == '█' ? $"[{shadowColor}]▓[/]" : " ");
            shsb.Append(' ');
        }
        result[4] = shsb.ToString().TrimEnd();

        return result;
    }
}
