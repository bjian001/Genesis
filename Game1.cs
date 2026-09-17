using Genesis.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Genesis;

public class Game1 : Game
{
    private const int BaseWidth = 1280;
    private const int BaseHeight = 720;

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _pixel = null!;
    private Texture2D _cover = null!;
    private BitmapFont _uiFont = null!;
    private BitmapFont _titleFont = null!;

    private StartScreen _startScreen = null!;
    private LoadingBar _loadingBar = null!;
    private SettingsOverlay _settings = null!;

    private GameState _state = GameState.BootLoading;
    private bool _graphicsDirty;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = BaseWidth;
        _graphics.PreferredBackBufferHeight = BaseHeight;
        _graphics.SynchronizeWithVerticalRetrace = true;
        Window.Title = "Genesis";
        Window.AllowUserResizing = false;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        _cover = Content.Load<Texture2D>("cover");
        var uiAtlas = Content.Load<Texture2D>(BitmapFont.UiData.TextureAsset);
        var titleAtlas = Content.Load<Texture2D>(BitmapFont.TitleData.TextureAsset);

        _uiFont = new BitmapFont(uiAtlas, BitmapFont.UiData.Glyphs, BitmapFont.UiData.LineHeight);
        _titleFont = new BitmapFont(titleAtlas, BitmapFont.TitleData.Glyphs, BitmapFont.TitleData.LineHeight);

        _startScreen = new StartScreen(_pixel);
        _startScreen.SetAssets(_cover, _uiFont, _titleFont);
        _startScreen.SetContinueEnabled(HasSaveData());
        _startScreen.Layout(BaseWidth, BaseHeight);

        _loadingBar = new LoadingBar(_pixel);
        _settings = new SettingsOverlay(_pixel);
        _settings.SetFont(_uiFont);
        _settings.Layout(BaseWidth, BaseHeight);

        // Brief boot loading, then title.
        _loadingBar.Begin(1.2f, "Growing...");
        _state = GameState.BootLoading;
    }

    protected override void Update(GameTime gameTime)
    {
        // Global quick-exit from title with Back / Esc handled per-state.
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
                    Exit();
                    break;
                }

                var action = _startScreen.Update(gameTime);
                switch (action)
                {
                    case StartScreen.MenuAction.NewGame:
                        _loadingBar.Begin(2.0f, "Awakening...");
                        _state = GameState.Starting;
                        break;
                    case StartScreen.MenuAction.Continue:
                        // No save yet — button is disabled, but guard anyway.
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
                        Exit();
                        break;
                }
                break;
            }

            case GameState.Settings:
                _settings.Update();
                if (_settings.RequestClose || Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    ApplySettings();
                    _state = GameState.Title;
                }
                break;

            case GameState.Starting:
                _loadingBar.Update(gameTime);
                if (_loadingBar.IsCompleted)
                    _state = GameState.Playing;
                break;

            case GameState.Playing:
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    _startScreen.SetContinueEnabled(HasSaveData());
                    _state = GameState.Title;
                }
                break;
        }

        if (_graphicsDirty)
        {
            _graphics.ApplyChanges();
            _graphicsDirty = false;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        switch (_state)
        {
            case GameState.BootLoading:
                GraphicsDevice.Clear(UiTheme.BackdropTint);
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                DrawCenteredLoadingBar();
                _spriteBatch.End();
                break;

            case GameState.Title:
                GraphicsDevice.Clear(UiTheme.BackdropTint);
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                _startScreen.Draw(_spriteBatch, BaseWidth, BaseHeight);
                _spriteBatch.End();
                break;

            case GameState.Settings:
                GraphicsDevice.Clear(UiTheme.BackdropTint);
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                _startScreen.Draw(_spriteBatch, BaseWidth, BaseHeight);
                _settings.Draw(_spriteBatch, BaseWidth, BaseHeight);
                _spriteBatch.End();
                break;

            case GameState.Starting:
                GraphicsDevice.Clear(UiTheme.BackdropTint);
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                // Dimmed cover behind the starting bar.
                _startScreen.Draw(_spriteBatch, BaseWidth, BaseHeight);
                _spriteBatch.Draw(_pixel, new Rectangle(0, 0, BaseWidth, BaseHeight), UiTheme.OverlayDim);
                DrawCenteredLoadingBar();
                _spriteBatch.End();
                break;

            case GameState.Playing:
                GraphicsDevice.Clear(UiTheme.PlayingBg);
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                DrawPlayingPlaceholder();
                _spriteBatch.End();
                break;
        }

        base.Draw(gameTime);
    }

    private void DrawCenteredLoadingBar()
    {
        var bar = new Rectangle((BaseWidth - 480) / 2, BaseHeight / 2 + 20, 480, 28);
        _loadingBar.Draw(_spriteBatch, _uiFont, bar);
    }

    private void DrawPlayingPlaceholder()
    {
        const string hud = "In Progress";
        const string hint = "Press Esc to return to title";
        var size = _titleFont.MeasureString(hud);
        var pos = new Vector2((BaseWidth - size.X) * 0.5f, BaseHeight * 0.4f);
        _titleFont.DrawString(_spriteBatch, hud, pos + new Vector2(2, 2), new Color(0, 0, 0, 100));
        _titleFont.DrawString(_spriteBatch, hud, pos, UiTheme.TitleMain);

        var hintSize = _uiFont.MeasureString(hint);
        _uiFont.DrawString(
            _spriteBatch,
            hint,
            new Vector2((BaseWidth - hintSize.X) * 0.5f, pos.Y + size.Y + 18),
            UiTheme.Subtitle);
    }

    private void ApplySettings()
    {
        // Volume value is kept on the overlay for a future audio bus.

        var wantFullscreen = _settings.Fullscreen;
        if (_graphics.IsFullScreen != wantFullscreen)
        {
            _graphics.IsFullScreen = wantFullscreen;
            if (wantFullscreen)
            {
                _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                _graphics.PreferredBackBufferWidth = BaseWidth;
                _graphics.PreferredBackBufferHeight = BaseHeight;
            }
            _graphicsDirty = true;
        }
    }

    /// <summary>No save system yet — Continue stays disabled until one exists.</summary>
    private static bool HasSaveData() => false;

    protected override void UnloadContent()
    {
        _pixel?.Dispose();
        base.UnloadContent();
    }
}
