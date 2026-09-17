using Microsoft.Xna.Framework;

namespace Genesis.UI;

/// <summary>Cozy sci-fi palette matching the cover art (pastel purple/pink, gold seedling).</summary>
public static class UiTheme
{
    public static readonly Color BackdropTint = new(28, 18, 42);
    public static readonly Color PanelFill = new(48, 32, 72, 210);
    public static readonly Color PanelBorder = new(210, 170, 230, 180);
    public static readonly Color PanelBorderHover = new(255, 220, 245, 230);
    public static readonly Color ButtonFill = new(72, 48, 98, 220);
    public static readonly Color ButtonFillHover = new(108, 72, 140, 240);
    public static readonly Color ButtonFillDisabled = new(55, 45, 70, 160);
    public static readonly Color ButtonText = new(255, 240, 250);
    public static readonly Color ButtonTextDisabled = new(160, 145, 170);
    public static readonly Color TitleGlow = new(255, 214, 120);
    public static readonly Color TitleMain = new(255, 236, 250);
    public static readonly Color Subtitle = new(220, 190, 230, 220);
    public static readonly Color AccentGold = new(255, 200, 90);
    public static readonly Color AccentLeaf = new(140, 210, 130);
    public static readonly Color BarTrack = new(40, 28, 55, 200);
    public static readonly Color BarFillStart = new(255, 196, 80);
    public static readonly Color BarFillEnd = new(120, 200, 110);
    public static readonly Color OverlayDim = new(12, 8, 22, 180);
    public static readonly Color PlayingBg = new(36, 28, 52);
}
