using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genesis.UI;

/// <summary>
/// Keeps a fixed virtual resolution (e.g. 1280x720) and scales the whole game
/// to fit the current backbuffer while showing everything (letterbox / pillarbox).
/// </summary>
public sealed class ScreenScaler
{
    public int VirtualWidth { get; }
    public int VirtualHeight { get; }

    public float Scale { get; private set; } = 1f;
    public Point Offset { get; private set; }
    public Matrix Transform { get; private set; } = Matrix.Identity;
    public Rectangle Destination { get; private set; }

    public ScreenScaler(int virtualWidth, int virtualHeight)
    {
        VirtualWidth = virtualWidth;
        VirtualHeight = virtualHeight;
        Refresh(virtualWidth, virtualHeight);
    }

    public void Refresh(GraphicsDevice device) =>
        Refresh(device.Viewport.Width, device.Viewport.Height);

    public void Refresh(int backBufferWidth, int backBufferHeight)
    {
        var scaleX = backBufferWidth / (float)VirtualWidth;
        var scaleY = backBufferHeight / (float)VirtualHeight;
        Scale = Math.Min(scaleX, scaleY);

        var drawW = (int)Math.Round(VirtualWidth * Scale);
        var drawH = (int)Math.Round(VirtualHeight * Scale);
        var x = (backBufferWidth - drawW) / 2;
        var y = (backBufferHeight - drawH) / 2;

        Offset = new Point(x, y);
        Destination = new Rectangle(x, y, drawW, drawH);
        Transform = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(x, y, 0f);
    }

    public Point ToVirtual(Point screenPoint)
    {
        if (Scale <= 0f)
            return Point.Zero;

        return new Point(
            (int)Math.Floor((screenPoint.X - Offset.X) / Scale),
            (int)Math.Floor((screenPoint.Y - Offset.Y) / Scale));
    }
}
