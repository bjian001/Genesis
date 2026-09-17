using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genesis.UI;

/// <summary>Thematic plant-growth style progress bar used for boot and New Game start.</summary>
public sealed class LoadingBar
{
    private readonly Texture2D _pixel;
    private float _progress;
    private float _displayProgress;
    private float _durationSeconds;
    private float _elapsed;
    private bool _running;
    private bool _completed;
    private string _statusText = "Awakening...";

    public LoadingBar(Texture2D pixel)
    {
        _pixel = pixel;
    }

    public float Progress => MathHelper.Clamp(_displayProgress, 0f, 1f);
    public bool IsRunning => _running;
    public bool IsCompleted => _completed;
    public string StatusText => _statusText;

    public void Begin(float durationSeconds, string statusText)
    {
        _durationSeconds = Math.Max(0.35f, durationSeconds);
        _elapsed = 0f;
        _progress = 0f;
        _displayProgress = 0f;
        _running = true;
        _completed = false;
        _statusText = statusText;
    }

    public void Update(GameTime gameTime)
    {
        if (!_running)
            return;

        _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
        var t = MathHelper.Clamp(_elapsed / _durationSeconds, 0f, 1f);
        _progress = 1f - MathF.Pow(1f - t, 3f);
        _displayProgress = MathHelper.Lerp(_displayProgress, _progress, 0.35f);

        if (t >= 1f)
        {
            _displayProgress = 1f;
            _running = false;
            _completed = true;
        }
    }

    public void Draw(SpriteBatch spriteBatch, BitmapFont? font, Rectangle bounds)
    {
        DrawRect(spriteBatch, bounds, UiTheme.BarTrack);
        var border = new Rectangle(bounds.X - 2, bounds.Y - 2, bounds.Width + 4, bounds.Height + 4);
        DrawRect(spriteBatch, border, UiTheme.PanelBorder * 0.55f);

        var fillWidth = (int)(bounds.Width * Progress);
        if (fillWidth > 0)
        {
            var fill = new Rectangle(bounds.X, bounds.Y, fillWidth, bounds.Height);
            DrawGradientFill(spriteBatch, fill);
            var highlight = new Rectangle(fill.X, fill.Y, fill.Width, Math.Max(2, fill.Height / 3));
            DrawRect(spriteBatch, highlight, new Color(255, 255, 220, 70));
        }

        if (font != null)
        {
            var label = $"{_statusText}  {(int)(Progress * 100f)}%";
            var size = font.MeasureString(label);
            var pos = new Vector2(
                bounds.X + (bounds.Width - size.X) * 0.5f,
                bounds.Y - size.Y - 10f);
            font.DrawString(spriteBatch, label, pos + new Vector2(1, 1), new Color(0, 0, 0, 120));
            font.DrawString(spriteBatch, label, pos, UiTheme.TitleGlow);
        }
    }

    private void DrawGradientFill(SpriteBatch spriteBatch, Rectangle fill)
    {
        const int slices = 24;
        var sliceW = Math.Max(1, fill.Width / slices);
        for (var i = 0; i < slices; i++)
        {
            var x = fill.X + i * sliceW;
            var w = (i == slices - 1) ? fill.Right - x : sliceW;
            if (w <= 0)
                break;
            var t = i / (float)(slices - 1);
            var color = Color.Lerp(UiTheme.BarFillStart, UiTheme.BarFillEnd, t);
            DrawRect(spriteBatch, new Rectangle(x, fill.Y, w, fill.Height), color);
        }
    }

    private void DrawRect(SpriteBatch spriteBatch, Rectangle rect, Color color)
    {
        spriteBatch.Draw(_pixel, rect, color);
    }
}
