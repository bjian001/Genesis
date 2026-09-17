using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Genesis.UI;

/// <summary>Simple settings panel with working stubs for Volume and Fullscreen, plus Close.</summary>
public sealed class SettingsOverlay
{
    private readonly Texture2D _pixel;
    private BitmapFont? _font;
    private Rectangle _panel;
    private Rectangle _volumeDown;
    private Rectangle _volumeUp;
    private Rectangle _fullscreenBtn;
    private Rectangle _closeBtn;
    private int _hovered = -1;
    private MouseState _prevMouse;

    public float Volume { get; private set; } = 0.8f;
    public bool Fullscreen { get; private set; }
    public bool RequestClose { get; private set; }

    public SettingsOverlay(Texture2D pixel)
    {
        _pixel = pixel;
    }

    public void SetFont(BitmapFont font) => _font = font;

    public void Layout(int screenWidth, int screenHeight)
    {
        const int w = 420;
        const int h = 280;
        _panel = new Rectangle((screenWidth - w) / 2, (screenHeight - h) / 2, w, h);

        _volumeDown = new Rectangle(_panel.X + 40, _panel.Y + 100, 48, 40);
        _volumeUp = new Rectangle(_panel.X + 100, _panel.Y + 100, 48, 40);
        _fullscreenBtn = new Rectangle(_panel.X + 40, _panel.Y + 160, 200, 40);
        _closeBtn = new Rectangle(_panel.Right - 140, _panel.Bottom - 56, 100, 40);
    }

    public void Update(ScreenScaler scaler)
    {
        RequestClose = false;
        var mouse = Mouse.GetState();
        var pos = scaler.ToVirtual(mouse.Position);
        _hovered = -1;

        if (_volumeDown.Contains(pos)) _hovered = 0;
        else if (_volumeUp.Contains(pos)) _hovered = 1;
        else if (_fullscreenBtn.Contains(pos)) _hovered = 2;
        else if (_closeBtn.Contains(pos)) _hovered = 3;

        var clicked = mouse.LeftButton == ButtonState.Pressed &&
                      _prevMouse.LeftButton == ButtonState.Released;
        _prevMouse = mouse;

        if (!clicked)
            return;

        switch (_hovered)
        {
            case 0:
                Volume = MathHelper.Clamp(Volume - 0.1f, 0f, 1f);
                break;
            case 1:
                Volume = MathHelper.Clamp(Volume + 0.1f, 0f, 1f);
                break;
            case 2:
                Fullscreen = !Fullscreen;
                break;
            case 3:
                RequestClose = true;
                break;
        }
    }

    public void Draw(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
    {
        spriteBatch.Draw(_pixel, new Rectangle(0, 0, screenWidth, screenHeight), UiTheme.OverlayDim);

        DrawRect(spriteBatch, new Rectangle(_panel.X - 3, _panel.Y - 3, _panel.Width + 6, _panel.Height + 6), UiTheme.PanelBorder);
        DrawRect(spriteBatch, _panel, UiTheme.PanelFill);

        if (_font == null)
            return;

        DrawCentered(spriteBatch, "Settings", _panel.X, _panel.Y + 24, _panel.Width, UiTheme.TitleMain);
        DrawCentered(spriteBatch, $"Volume: {(int)(Volume * 100)}%", _panel.X, _panel.Y + 64, _panel.Width, UiTheme.Subtitle);

        DrawSmallButton(spriteBatch, _volumeDown, "-", _hovered == 0);
        DrawSmallButton(spriteBatch, _volumeUp, "+", _hovered == 1);
        DrawSmallButton(spriteBatch, _fullscreenBtn, Fullscreen ? "Fullscreen: On" : "Fullscreen: Off", _hovered == 2);
        DrawSmallButton(spriteBatch, _closeBtn, "Close", _hovered == 3);
    }

    private void DrawSmallButton(SpriteBatch spriteBatch, Rectangle bounds, string label, bool hovered)
    {
        var fill = hovered ? UiTheme.ButtonFillHover : UiTheme.ButtonFill;
        DrawRect(spriteBatch, new Rectangle(bounds.X - 1, bounds.Y - 1, bounds.Width + 2, bounds.Height + 2), UiTheme.PanelBorder * 0.7f);
        DrawRect(spriteBatch, bounds, fill);

        if (_font == null)
            return;

        var size = _font.MeasureString(label);
        var pos = new Vector2(
            bounds.X + (bounds.Width - size.X) * 0.5f,
            bounds.Y + (bounds.Height - size.Y) * 0.5f);
        _font.DrawString(spriteBatch, label, pos, UiTheme.ButtonText);
    }

    private void DrawCentered(SpriteBatch spriteBatch, string text, int x, int y, int width, Color color)
    {
        if (_font == null)
            return;
        var size = _font.MeasureString(text);
        _font.DrawString(spriteBatch, text, new Vector2(x + (width - size.X) * 0.5f, y), color);
    }

    private void DrawRect(SpriteBatch spriteBatch, Rectangle rect, Color color)
    {
        spriteBatch.Draw(_pixel, rect, color);
    }
}

