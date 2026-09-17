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
    private readonly ScreenScaler _scaler;
    private SpriteBatch _spriteBatch = null!;
    private StartingMenu _startingMenu = null!;
    private Texture2D _pixel = null!;

    private bool _inGameplay;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = BaseWidth;
        _graphics.PreferredBackBufferHeight = BaseHeight;
        _graphics.SynchronizeWithVerticalRetrace = true;
        _graphics.HardwareModeSwitch = false;
        Window.Title = "Genesis";
        Window.AllowUserResizing = false;

        _scaler = new ScreenScaler(BaseWidth, BaseHeight);
        _startingMenu = new StartingMenu(_graphics, _scaler);
    }

    protected override void Initialize()
    {
        base.Initialize();
        _scaler.Refresh(GraphicsDevice);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
        _startingMenu.Load(GraphicsDevice, Content);
    }

    protected override void Update(GameTime gameTime)
    {
        if (!_inGameplay)
        {
            switch (_startingMenu.Update(gameTime, GraphicsDevice))
            {
                case StartingMenu.Result.EnterGameplay:
                    _inGameplay = true;
                    break;
                case StartingMenu.Result.Quit:
                    Exit();
                    return;
            }
        }
        else
        {
            _scaler.Refresh(GraphicsDevice);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _inGameplay = false;
                _startingMenu.ShowTitle();
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        if (!_inGameplay)
        {
            _startingMenu.Draw(GraphicsDevice, _spriteBatch);
        }
        else
        {
            _scaler.Refresh(GraphicsDevice);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                null, null, null, _scaler.Transform);
            DrawPlayingPlaceholder();
            _spriteBatch.End();
        }

        base.Draw(gameTime);
    }

    private void DrawPlayingPlaceholder()
    {
        _spriteBatch.Draw(_pixel, new Rectangle(0, 0, BaseWidth, BaseHeight), UiTheme.PlayingBg);

        const string hud = "In Progress";
        const string hint = "Press Esc to return to title";
        var titleFont = _startingMenu.TitleFont;
        var uiFont = _startingMenu.UiFont;

        var size = titleFont.MeasureString(hud);
        var pos = new Vector2((BaseWidth - size.X) * 0.5f, BaseHeight * 0.4f);
        titleFont.DrawString(_spriteBatch, hud, pos + new Vector2(2, 2), new Color(0, 0, 0, 100));
        titleFont.DrawString(_spriteBatch, hud, pos, UiTheme.TitleMain);

        var hintSize = uiFont.MeasureString(hint);
        uiFont.DrawString(
            _spriteBatch,
            hint,
            new Vector2((BaseWidth - hintSize.X) * 0.5f, pos.Y + size.Y + 18),
            UiTheme.Subtitle);
    }

    protected override void UnloadContent()
    {
        _startingMenu.Unload();
        _pixel?.Dispose();
        base.UnloadContent();
    }
}
