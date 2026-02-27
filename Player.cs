using LibVLCSharp.Shared;

namespace ListenerSharp;

public enum PlayerState
{
    Stopped,
    Playing,
    Paused
}

public class Player : IDisposable
{
    private readonly LibVLC _libVLC;
    private MediaPlayer _mediaPlayer;
    private bool _disposed;

    public List<string> Tracks { get; }
    public int CurrentIndex { get; private set; } = -1;
    public PlayerState State { get; private set; } = PlayerState.Stopped;

    public long CurrentTimeMs => _mediaPlayer.Time < 0 ? 0 : _mediaPlayer.Time;
    public long TotalTimeMs => _mediaPlayer.Length < 0 ? 0 : _mediaPlayer.Length;

    public bool IsPlaying => _mediaPlayer.IsPlaying;

    public event Action? TrackChanged;
    public event Action? StateChanged;

    public Player(List<string> tracks)
    {
        Core.Initialize();
        _libVLC = new LibVLC("--no-video");
        _mediaPlayer = new MediaPlayer(_libVLC);

        _mediaPlayer.Playing += (_, _) =>
        {
            State = PlayerState.Playing;
            StateChanged?.Invoke();
        };
        _mediaPlayer.Paused += (_, _) =>
        {
            State = PlayerState.Paused;
            StateChanged?.Invoke();
        };
        _mediaPlayer.Stopped += (_, _) =>
        {
            State = PlayerState.Stopped;
            StateChanged?.Invoke();
        };
        _mediaPlayer.EndReached += (_, _) =>
        {
            // Auto-advance to next track on a separate thread to avoid VLC deadlock
            Task.Run(() =>
            {
                Thread.Sleep(200);
                Next();
            });
        };

        Tracks = tracks;
    }

    public void Play(int index)
    {
        if (index < 0 || index >= Tracks.Count) return;
        CurrentIndex = index;

        var media = new Media(_libVLC, Tracks[index], FromType.FromPath);
        _mediaPlayer.Play(media);
        media.Dispose();

        TrackChanged?.Invoke();
    }

    public void PlayPause()
    {
        if (State == PlayerState.Stopped && CurrentIndex >= 0)
        {
            Play(CurrentIndex);
            return;
        }
        if (State == PlayerState.Stopped)
        {
            Play(0);
            return;
        }
        if (_mediaPlayer.IsPlaying)
            _mediaPlayer.Pause();
        else
            _mediaPlayer.Play();
    }

    public void Next()
    {
        if (Tracks.Count == 0) return;
        int next = (CurrentIndex + 1) % Tracks.Count;
        Play(next);
    }

    public void Previous()
    {
        if (Tracks.Count == 0) return;
        // If more than 3s in, restart current track; otherwise go back
        if (CurrentTimeMs > 3000)
        {
            _mediaPlayer.Time = 0;
            return;
        }
        int prev = CurrentIndex - 1;
        if (prev < 0) prev = Tracks.Count - 1;
        Play(prev);
    }

    public int Volume
    {
        get => _mediaPlayer.Volume;
        set => _mediaPlayer.Volume = Math.Clamp(value, 0, 100);
    }

    public void Seek(long offsetMs)
    {
        if (TotalTimeMs <= 0) return;
        long newTime = Math.Clamp(CurrentTimeMs + offsetMs, 0, TotalTimeMs - 1000);
        _mediaPlayer.Time = newTime;
    }

    public void Stop()
    {
        _mediaPlayer.Stop();
        State = PlayerState.Stopped;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _mediaPlayer.Stop();
        _mediaPlayer.Dispose();
        _libVLC.Dispose();
    }
}
