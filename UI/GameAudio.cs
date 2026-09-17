using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Genesis.UI;

/// <summary>Loops the chill-warm hub BGM and syncs Settings volume.</summary>
public sealed class GameAudio
{
    private Song? _bgm;
    private bool _started;

    public float Volume { get; private set; } = 0.8f;

    public void Load(ContentManager content)
    {
        _bgm = content.Load<Song>("Audio/bgm_chill_warm");
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = Volume;
    }

    public void EnsurePlaying()
    {
        if (_bgm == null)
            return;

        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = Volume;

        if (!_started || MediaState.Stopped == MediaPlayer.State)
        {
            MediaPlayer.Play(_bgm);
            _started = true;
        }
        else if (MediaPlayer.State == MediaState.Paused)
        {
            MediaPlayer.Resume();
        }
    }

    public void SetVolume(float volume)
    {
        Volume = MathHelper.Clamp(volume, 0f, 1f);
        MediaPlayer.Volume = Volume;
    }

    public void Stop()
    {
        if (MediaPlayer.State != MediaState.Stopped)
            MediaPlayer.Stop();
        _started = false;
    }
}
