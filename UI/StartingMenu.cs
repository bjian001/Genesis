using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Genesis.UI;

/// <summary>
/// Owns the boot / title / settings / starting-bar flow.
/// Game1 only loads this and reacts when the player enters gameplay or quits.
/// </summary>
public sealed class StartingMenu
{
    public enum Result
    {
        None,
        EnterGameplay,
        Quit
    }

    private readonly int _width;
    private readonly int _height;
    private readonly GraphicsDeviceManager _graphics;
    private readonly ScreenScaler _scaler;

    private Texture2D _pixel = null!;
    private Texture2D _cover = null!;
    private BitmapFont _uiFont = null!;
    private BitmapFont _titleFont = null!;

    private StartScreen _startScreen = null!;
    private LoadingBar _loadingBar = null!;
    private SettingsOverlay _settings = null!;
    private readonly GameAudio _audio = new();

    private GameState _state = GameState.BootLoading;
    private bool _graphicsDirty;
    private bool _ownsPixel;

    public StartingMenu(GraphicsDeviceManager graphics, ScreenScaler scaler)
    {
        _graphics = graphics;
        _scaler = scaler;
        _width = scaler.VirtualWidth;
        _height = scaler.VirtualHeight;
    }

    public bool IsActive =>
        _state is GameState.BootLoading or GameState.Title or GameState.Settings or GameState.Starting;

    public GameState State => _state;

    public void Load(GraphicsDevice device, ContentManager content)
    {
        _pixel = new Texture2D(device, 1, 1);
        _pixel.SetData(new[] { Color.White });
        _ownsPixel = true;

        _cover = content.Load<Texture2D>("cover");
        var uiAtlas = content.Load<Texture2D>(BitmapFont.UiData.TextureAsset);
        var titleAtlas = content.Load<Texture2D>(BitmapFont.TitleData.TextureAsset);

        _uiFont = new BitmapFont(uiAtlas, BitmapFont.UiData.Glyphs, BitmapFont.UiData.LineHeight);
        _titleFont = new BitmapFont(titleAtlas, BitmapFont.TitleData.Glyphs, BitmapFont.TitleData.LineHeight);

        _startScreen = new StartScreen(_pixel);
        _startScreen.SetAssets(_cover, _uiFont, _titleFont);
        _startScreen.SetContinueEnabled(HasSaveData());
        _startScreen.Layout(_width, _height);

        _loadingBar = new LoadingBar(_pixel);
        _settings = new SettingsOverlay(_pixel);
        _settings.SetFont(_uiFont);
        _settings.Layout(_width, _height);

        _audio.Load(content);
        _audio.SetVolume(_settings.Volume);
        _audio.EnsurePlaying();

        _loadingBar.Begin(1.2f, "Growing...");
        _state = GameState.BootLoading;
        _scaler.Refresh(device);
    }

    /// <summary>Call when returning from gameplay to the title screen.</summary>
    public void ShowTitle()
    {
        _startScreen.SetContinueEnabled(HasSaveData());
        _state = GameState.Title;
    }

    public Result Update(GameTime gameTime, GraphicsDevice device)
    {
        _scaler.Refresh(device);
        var result = Result.None;

        switch (_state)
        {
            case GameState.BootLoading:
                _loadingBar.Update(gameTime);
                if (_loadingBar.IsCompleted)
                    _state = GameState.Title;
                break;

            case GameState.Title:
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    return Result.Quit;
                }

                switch (_startScreen.Update(gameTime, _scaler))
                {
                    case StartScreen.MenuAction.NewGame:
                        _loadingBar.Begin(2.0f, "Awakening...");
                        _state = GameState.Starting;
                        break;
                    case StartScreen.MenuAction.Continue:
                        if (HasSaveData())
                        {
                            _loadingBar.Begin(1.6f, "Returning...");
                            _state = GameState.Starting;
                        }
                        break;
                    case StartScreen.MenuAction.Settings:
                        _state = GameState.Settings;
                        break;
                    case StartScreen.MenuAction.Quit:
                        return Result.Quit;
                }
                break;
            }

            case GameState.Settings:
                _settings.Update(_scaler);
                _audio.SetVolume(_settings.Volume);
                if (_settings.RequestClose || Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    ApplySettings(device);
                    _state = GameState.Title;
                }
                break;

            case GameState.Starting:
                _loadingBar.Update(gameTime);
                if (_loadingBar.IsCompleted)
                {
                    _state = GameState.Playing;
                    result = Result.EnterGameplay;
                }
                break;
        }

        if (_graphicsDirty)
        {
            _graphics.ApplyChanges();
            _graphicsDirty = false;
            _scaler.Refresh(device);
        }

        return result;
    }

    public void Draw(GraphicsDevice device, SpriteBatch spriteBatch)
    {
        _scaler.Refresh(device);

        switch (_state)
        {
            case GameState.BootLoading:
                device.Clear(Color.Black);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                    null, null, null, _scaler.Transform);
                FillVirtual(spriteBatch, UiTheme.BackdropTint);
                DrawCenteredLoadingBar(spriteBatch);
                spriteBatch.End();
                break;

            case GameState.Title:
                device.Clear(Color.Black);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                    null, null, null, _scaler.Transform);
                _startScreen.Draw(spriteBatch, _width, _height);
                spriteBatch.End();
                break;

            case GameState.Settings:
                device.Clear(Color.Black);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                    null, null, null, _scaler.Transform);
                _startScreen.Draw(spriteBatch, _width, _height);
                _settings.Draw(spriteBatch, _width, _height);
                spriteBatch.End();
                break;

            case GameState.Starting:
                device.Clear(Color.Black);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                    null, null, null, _scaler.Transform);
                _startScreen.Draw(spriteBatch, _width, _height);
                spriteBatch.Draw(_pixel, new Rectangle(0, 0, _width, _height), UiTheme.OverlayDim);
                DrawCenteredLoadingBar(spriteBatch);
                spriteBatch.End();
                break;
        }
    }

    public BitmapFont UiFont => _uiFont;
    public BitmapFont TitleFont => _titleFont;
    public float MusicVolume => _audio.Volume;

    public void Unload()
    {
        _audio.Stop();
        if (_ownsPixel)
        {
            _pixel?.Dispose();
            _ownsPixel = false;
        }
    }

    private void FillVirtual(SpriteBatch spriteBatch, Color color)
    {
        spriteBatch.Draw(_pixel, new Rectangle(0, 0, _width, _height), color);
    }

    private void DrawCenteredLoadingBar(SpriteBatch spriteBatch)
    {
        var bar = new Rectangle((_width - 480) / 2, _height / 2 + 20, 480, 28);
        _loadingBar.Draw(spriteBatch, _uiFont, bar);
    }

    private void ApplySettings(GraphicsDevice device)
    {
        _audio.SetVolume(_settings.Volume);

        var wantFullscreen = _settings.Fullscreen;
        if (_graphics.IsFullScreen == wantFullscreen)
            return;

        _graphics.IsFullScreen = wantFullscreen;
        // Prefer soft fullscreen so we can letterbox-scale the full virtual frame.
        _graphics.HardwareModeSwitch = false;

        if (wantFullscreen)
        {
            var mode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            _graphics.PreferredBackBufferWidth = mode.Width;
            _graphics.PreferredBackBufferHeight = mode.Height;
        }
        else
        {
            _graphics.PreferredBackBufferWidth = _width;
            _graphics.PreferredBackBufferHeight = _height;
        }

        _graphicsDirty = true;
    }

    private static bool HasSaveData() => false;
}

