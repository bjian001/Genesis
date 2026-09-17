using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Genesis.UI;

public sealed class StartScreen
{
    public enum MenuAction
    {
        None,
        NewGame,
        Continue,
        Settings,
        Quit
    }

    private readonly Texture2D _pixel;
    private Texture2D? _cover;
    private BitmapFont? _font;
    private BitmapFont? _titleFont;

    private Rectangle _newGameBtn;
    private Rectangle _continueBtn;
    private Rectangle _settingsBtn;
    private Rectangle _quitBtn;
    private int _hovered = -1;
    private bool _continueEnabled;
    private MouseState _prevMouse;

    public StartScreen(Texture2D pixel)
    {
        _pixel = pixel;
    }

    public void SetAssets(Texture2D cover, BitmapFont font, BitmapFont? titleFont = null)
    {
        _cover = cover;
        _font = font;
        _titleFont = titleFont ?? font;
    }

    public void SetContinueEnabled(bool enabled) => _continueEnabled = enabled;

    public void Layout(int screenWidth, int screenHeight)
    {
        const int btnW = 280;
        const int btnH = 48;
        const int gap = 14;
        var startY = (int)(screenHeight * 0.52f);
        var x = (screenWidth - btnW) / 2;

        _newGameBtn = new Rectangle(x, startY, btnW, btnH);
        _continueBtn = new Rectangle(x, startY + (btnH + gap), btnW, btnH);
        _settingsBtn = new Rectangle(x, startY + 2 * (btnH + gap), btnW, btnH);
        _quitBtn = new Rectangle(x, startY + 3 * (btnH + gap), btnW, btnH);
    }

    public MenuAction Update(GameTime gameTime, ScreenScaler scaler)
    {
        var mouse = Mouse.GetState();
        var pos = scaler.ToVirtual(mouse.Position);
        _hovered = -1;

        if (_newGameBtn.Contains(pos)) _hovered = 0;
        else if (_continueBtn.Contains(pos) && _continueEnabled) _hovered = 1;
        else if (_settingsBtn.Contains(pos)) _hovered = 2;
        else if (_quitBtn.Contains(pos)) _hovered = 3;

        var clicked = mouse.LeftButton == ButtonState.Pressed &&
                      _prevMouse.LeftButton == ButtonState.Released;
        _prevMouse = mouse;

        if (!clicked)
            return MenuAction.None;

        return _hovered switch
        {
            0 => MenuAction.NewGame,
            1 => MenuAction.Continue,
            2 => MenuAction.Settings,
            3 => MenuAction.Quit,
            _ => MenuAction.None
        };
    }

    public void Draw(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
    {
        DrawCover(spriteBatch, screenWidth, screenHeight);
        DrawRect(spriteBatch, new Rectangle(0, 0, screenWidth, screenHeight), new Color(20, 10, 35, 90));
        DrawTitle(spriteBatch, screenWidth, screenHeight);

        DrawButton(spriteBatch, _newGameBtn, "New Game", _hovered == 0, enabled: true);
        DrawButton(spriteBatch, _continueBtn, "Continue", _hovered == 1, enabled: _continueEnabled);
        DrawButton(spriteBatch, _settingsBtn, "Settings", _hovered == 2, enabled: true);
        DrawButton(spriteBatch, _quitBtn, "Quit", _hovered == 3, enabled: true);
    }

    private void DrawCover(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
    {
        if (_cover == null)
        {
            DrawRect(spriteBatch, new Rectangle(0, 0, screenWidth, screenHeight), UiTheme.BackdropTint);
            return;
        }

        var texW = _cover.Width;
        var texH = _cover.Height;
        var scale = Math.Max(screenWidth / (float)texW, screenHeight / (float)texH);
        var drawW = texW * scale;
        var drawH = texH * scale;
        var dest = new Rectangle(
            (int)((screenWidth - drawW) * 0.5f),
            (int)((screenHeight - drawH) * 0.5f),
            (int)drawW,
            (int)drawH);
        spriteBatch.Draw(_cover, dest, Color.White);
    }

    private void DrawTitle(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
    {
        if (_titleFont == null || _font == null)
            return;

        const string title = "GENESIS";
        const string subtitle = "Bring life back";

        var titleSize = _titleFont.MeasureString(title);
        var titlePos = new Vector2(
            (screenWidth - titleSize.X) * 0.5f,
            screenHeight * 0.18f);

        var glow = new Color(255, 200, 120, 80);
        _titleFont.DrawString(spriteBatch, title, titlePos + new Vector2(0, 3), glow);
        _titleFont.DrawString(spriteBatch, title, titlePos + new Vector2(2, 2), new Color(0, 0, 0, 100));
        _titleFont.DrawString(spriteBatch, title, titlePos, UiTheme.TitleMain);

        var lineW = (int)(titleSize.X * 0.55f);
        var lineRect = new Rectangle(
            (screenWidth - lineW) / 2,
            (int)(titlePos.Y + titleSize.Y + 6),
            lineW,
            3);
        DrawRect(spriteBatch, lineRect, UiTheme.AccentGold * 0.85f);

        var subSize = _font.MeasureString(subtitle);
        var subPos = new Vector2(
            (screenWidth - subSize.X) * 0.5f,
            lineRect.Bottom + 14);
        _font.DrawString(spriteBatch, subtitle, subPos + new Vector2(1, 1), new Color(0, 0, 0, 100));
        _font.DrawString(spriteBatch, subtitle, subPos, UiTheme.Subtitle);
    }

    private void DrawButton(SpriteBatch spriteBatch, Rectangle bounds, string label, bool hovered, bool enabled)
    {
        var fill = !enabled
            ? UiTheme.ButtonFillDisabled
            : hovered ? UiTheme.ButtonFillHover : UiTheme.ButtonFill;
        var border = hovered && enabled ? UiTheme.PanelBorderHover : UiTheme.PanelBorder;
        var textColor = enabled ? UiTheme.ButtonText : UiTheme.ButtonTextDisabled;

        if (hovered && enabled)
            bounds = new Rectangle(bounds.X, bounds.Y - 1, bounds.Width, bounds.Height);

        DrawRect(spriteBatch, new Rectangle(bounds.X - 2, bounds.Y - 2, bounds.Width + 4, bounds.Height + 4), border * 0.65f);
        DrawRect(spriteBatch, bounds, fill);
        DrawRect(spriteBatch, new Rectangle(bounds.X + 2, bounds.Y + 2, bounds.Width - 4, 2),
            new Color(255, 255, 255, enabled ? 35 : 15));

        if (_font == null)
            return;

        var size = _font.MeasureString(label);
        var pos = new Vector2(
            bounds.X + (bounds.Width - size.X) * 0.5f,
            bounds.Y + (bounds.Height - size.Y) * 0.5f);
        _font.DrawString(spriteBatch, label, pos + new Vector2(1, 1), new Color(0, 0, 0, 90));
        _font.DrawString(spriteBatch, label, pos, textColor);
    }

    private void DrawRect(SpriteBatch spriteBatch, Rectangle rect, Color color)
    {
        spriteBatch.Draw(_pixel, rect, color);
    }
}

