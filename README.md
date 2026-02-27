# LISTENER — Punk Audio Machine

> A cross-platform CLI music player with an old-school cassette deck aesthetic.
> Built with .NET 8 + Spectre.Console + LibVLCSharp.

```
╔══════════════════════════════════════════════════════════════════════════════╗
║ ╔═══════╗  · ★ · * · ★ · * · ★ · * · ✦ ·                      ╔═══════╗  ║
║ ║▐▌▐▌▐▌▐║  ██    ██████ ███████████ ████████ ██    ██ ████████  ║▌▐▌▐▌▐▌║  ║
║ ║▌▐▌▐▌▐▌║  ██    ██       ██    ██  ██    ██ ████████ ██    ██  ║▐▌▐▌▐▌▐║  ║
║ ║▐▌▐▌▐▌▐║  ██    ██████   ██    ██████    ██ ██    ██ ██████    ║▌▐▌▐▌▐▌║  ║
║ ║▌▐▌▐▌▐▌║  ██    ██       ██    ██  ██    ██ ██    ██ ██    ██  ║▐▌▐▌▐▌▐║  ║
║ ╠═══════╣  · ✦ · * · ★ · * · ✦ · * · ★ · *                    ╠═══════╣  ║
║ ║ ╭───╮ ║  ╭───╮══════════════════════════════════════╭───╮     ║ ╭───╮ ║  ║
║ ║ │ ● │ ║  │─●─│══════════════╪═══════════════════════│─●─│     ║ │ ● │ ║  ║
║ ║ ╰───╯ ║  ╰───╯══════════════════════════════════════╰───╯     ║ ╰───╯ ║  ║
║ ║  ───  ║  ▶   VOL ████████████▒▒▒▒▒▒▒▒  080%                  ║  ───  ║  ║
║ ╚═══════╝                                                        ╚═══════╝  ║
╠══════════ QUEUE ════════════════════════════════════════════════════════════╣
║  01 ▶ ANARCHY IN THE UK.MP3                                                 ║
║  02   white riot.flac                                                        ║
╠══════════ NOW PLAYING ══════════════════════════════════════════════════════╣
║         ▶  ANARCHY IN THE UK.MP3                                            ║
║  01:14  ████████████░░░░░░░░░░░░░░░░░░░░░░░░  02:48                         ║
╠═════════════════════════════════════════════════════════════════════════════╣
║  [SPACE] PLAY/PAUSE  [N] NEXT  [B] PREV  [←] -10s  [→] +10s  [↑/↓] VOL   ║
╚═════════════════════════════════════════════════════════════════════════════╝
```

[![CI](https://github.com/jacshuo/listener-sharp/actions/workflows/ci.yml/badge.svg)](https://github.com/jacshuo/listener-sharp/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/ListenerSharp.svg)](https://www.nuget.org/packages/ListenerSharp)
[![License: MIT](https://img.shields.io/badge/License-MIT-red.svg)](LICENSE)

---

## Features

- **Punk aesthetics** — Fat block-letter title, retro cassette deck UI, cosmic starfield header
- **Animated tape reels** — Left reel spins clockwise, right counter-clockwise; tape scrolls in real-time
- **Twin speaker cabinets** — Old-school woofer grille + tweeter on each side
- **Live progress + timer** — Elapsed time, progress bar, remaining time, all updated every 200 ms
- **Volume control** — Keyboard-driven ↑/↓, displayed as a live bar
- **Seek** — Jump ±10 seconds with ←/→
- **Cross-platform** — Windows, macOS, Linux (requires system VLC on Linux)

---

## Supported formats

`.mp3` `.aac` `.flac` `.wav` `.ogg` `.m4a` `.opus` `.wma`

---

## Installation

### Option 1 — dotnet global tool (needs .NET 8 SDK/runtime)

```bash
dotnet tool install -g ListenerSharp
```

Then from anywhere:

```bash
listen /path/to/music
```

### Option 2 — Self-contained binary (no .NET required)

Download the archive for your platform from the [latest release](https://github.com/YOUR_USERNAME/listener-sharp/releases/latest), extract it, and run the `listen` executable directly.

### Option 3 — Build from source

```bash
git clone https://github.com/YOUR_USERNAME/listener-sharp.git
cd listener-sharp
dotnet build ListenerSharp.csproj -c Release
dotnet run --project ListenerSharp.csproj -- /path/to/music
```

---

## Linux prerequisite

LibVLC native libraries are **not** bundled on Linux. Install them via your package manager before running `listen`:

| Distro              | Command                                  |
|---------------------|------------------------------------------|
| Debian / Ubuntu     | `sudo apt install vlc libvlc-dev`        |
| Fedora / RHEL       | `sudo dnf install vlc-devel`             |
| Arch                | `sudo pacman -S vlc`                     |
| openSUSE            | `sudo zypper install vlc-devel`          |

---

## Usage

```
listen <path>
```

| Argument | Description |
|----------|-------------|
| `<path>` | Directory containing audio files **or** a single audio file path |

Files in a directory are sorted alphabetically and played in order.

---

## Key bindings

| Key            | Action                                  |
|----------------|-----------------------------------------|
| `Space` / `P`  | Play / Pause                            |
| `N`            | Next track                              |
| `B`            | Previous track (restart if > 3 s in)   |
| `←`            | Seek –10 seconds                        |
| `→`            | Seek +10 seconds                        |
| `↑`            | Volume +5                               |
| `↓`            | Volume –5                               |
| `Q` / `Esc`    | Quit                                    |

---

## Publishing a release

Create a version tag and push it — GitHub Actions will build all platform binaries and publish to NuGet automatically:

```bash
git tag v1.0.0
git push origin v1.0.0
```

For NuGet publishing, add your API key as a repository secret named **`NUGET_API_KEY`** (Settings → Secrets and variables → Actions).

---

## Tech stack

| Library | Use |
|---------|-----|
| [Spectre.Console](https://spectreconsole.net/) | Terminal UI — panels, markup, live rendering, grid |
| [LibVLCSharp](https://github.com/videolan/libvlcsharp) | Cross-platform audio decoding & playback |
| [VideoLAN.LibVLC.Windows](https://www.nuget.org/packages/VideoLAN.LibVLC.Windows) | Native VLC binaries — Windows |
| [VideoLAN.LibVLC.Mac](https://www.nuget.org/packages/VideoLAN.LibVLC.Mac) | Native VLC binaries — macOS |

---

## License

[MIT](LICENSE) © 2026 listener-sharp contributors
