using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genesis.UI;

/// <summary>Lightweight bitmap font drawn from a Content texture atlas.</summary>
public sealed class BitmapFont
{
    public readonly struct Glyph
    {
        public readonly Rectangle Source;
        public readonly int Advance;
        public Glyph(int x, int y, int w, int h, int advance)
        {
            Source = new Rectangle(x, y, w, h);
            Advance = advance;
        }
    }

    private readonly Texture2D _texture;
    private readonly Dictionary<char, Glyph> _glyphs;
    public int LineHeight { get; }

    public BitmapFont(Texture2D texture, Dictionary<char, Glyph> glyphs, int lineHeight)
    {
        _texture = texture;
        _glyphs = glyphs;
        LineHeight = lineHeight;
    }

    public Vector2 MeasureString(string text)
    {
        float w = 0, lineW = 0, h = LineHeight;
        foreach (var ch in text)
        {
            if (ch == '\n')
            {
                w = MathHelper.Max(w, lineW);
                lineW = 0;
                h += LineHeight;
                continue;
            }
            if (_glyphs.TryGetValue(ch, out var g))
                lineW += g.Advance;
            else if (_glyphs.TryGetValue('?', out var q))
                lineW += q.Advance;
        }
        w = MathHelper.Max(w, lineW);
        return new Vector2(w, h);
    }

    public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
    {
        var pen = position;
        foreach (var ch in text)
        {
            if (ch == '\n')
            {
                pen.X = position.X;
                pen.Y += LineHeight;
                continue;
            }
            if (!_glyphs.TryGetValue(ch, out var g))
            {
                if (!_glyphs.TryGetValue('?', out g))
                    continue;
            }
            spriteBatch.Draw(_texture, pen, g.Source, color);
            pen.X += g.Advance;
        }
    }

    public static class UiData
    {
        // Auto-generated glyph metrics for Ui
        public const string TextureAsset = "Fonts/UiBitmapFont";
        public const int LineHeight = 30;
        public const int Size = 28;
        public static readonly Dictionary<char, Glyph> Glyphs = new()
        {
            [' '] = new Glyph(2, 2, 12, 30, 10),
            ['!'] = new Glyph(14, 2, 13, 30, 11),
            ['"'] = new Glyph(27, 2, 17, 30, 15),
            ['#'] = new Glyph(44, 2, 20, 30, 18),
            ['$'] = new Glyph(64, 2, 20, 30, 18),
            ['%'] = new Glyph(84, 2, 29, 30, 27),
            ['&'] = new Glyph(113, 2, 24, 30, 22),
            ['\''] = new Glyph(137, 2, 11, 30, 9),
            ['('] = new Glyph(148, 2, 14, 30, 12),
            [')'] = new Glyph(162, 2, 13, 30, 11),
            ['*'] = new Glyph(175, 2, 15, 30, 13),
            ['+'] = new Glyph(190, 2, 20, 30, 18),
            [','] = new Glyph(210, 2, 12, 30, 10),
            ['-'] = new Glyph(222, 2, 13, 30, 11),
            ['.'] = new Glyph(235, 2, 12, 30, 10),
            ['/'] = new Glyph(247, 2, 12, 30, 10),
            ['0'] = new Glyph(259, 2, 20, 30, 18),
            ['1'] = new Glyph(279, 2, 20, 30, 18),
            ['2'] = new Glyph(299, 2, 20, 30, 18),
            ['3'] = new Glyph(319, 2, 20, 30, 18),
            ['4'] = new Glyph(339, 2, 20, 30, 18),
            ['5'] = new Glyph(359, 2, 20, 30, 18),
            ['6'] = new Glyph(379, 2, 20, 30, 18),
            ['7'] = new Glyph(399, 2, 20, 30, 18),
            ['8'] = new Glyph(419, 2, 20, 30, 18),
            ['9'] = new Glyph(439, 2, 20, 30, 18),
            [':'] = new Glyph(459, 2, 13, 30, 11),
            [';'] = new Glyph(472, 2, 13, 30, 11),
            ['<'] = new Glyph(485, 2, 20, 30, 18),
            ['='] = new Glyph(2, 32, 20, 30, 18),
            ['>'] = new Glyph(22, 32, 20, 30, 18),
            ['?'] = new Glyph(42, 32, 21, 30, 19),
            ['@'] = new Glyph(63, 32, 31, 30, 29),
            ['A'] = new Glyph(94, 32, 24, 30, 22),
            ['B'] = new Glyph(118, 32, 24, 30, 22),
            ['C'] = new Glyph(142, 32, 24, 30, 22),
            ['D'] = new Glyph(166, 32, 24, 30, 22),
            ['E'] = new Glyph(190, 32, 23, 30, 21),
            ['F'] = new Glyph(213, 32, 21, 30, 19),
            ['G'] = new Glyph(234, 32, 26, 30, 24),
            ['H'] = new Glyph(260, 32, 24, 30, 22),
            ['I'] = new Glyph(284, 32, 12, 30, 10),
            ['J'] = new Glyph(296, 32, 20, 30, 18),
            ['K'] = new Glyph(316, 32, 24, 30, 22),
            ['L'] = new Glyph(340, 32, 21, 30, 19),
            ['M'] = new Glyph(361, 32, 27, 30, 25),
            ['N'] = new Glyph(388, 32, 24, 30, 22),
            ['O'] = new Glyph(412, 32, 26, 30, 24),
            ['P'] = new Glyph(438, 32, 23, 30, 21),
            ['Q'] = new Glyph(461, 32, 26, 30, 24),
            ['R'] = new Glyph(2, 62, 24, 30, 22),
            ['S'] = new Glyph(26, 62, 23, 30, 21),
            ['T'] = new Glyph(49, 62, 21, 30, 19),
            ['U'] = new Glyph(70, 62, 24, 30, 22),
            ['V'] = new Glyph(94, 62, 23, 30, 21),
            ['W'] = new Glyph(117, 62, 31, 30, 29),
            ['X'] = new Glyph(148, 62, 23, 30, 21),
            ['Y'] = new Glyph(171, 62, 23, 30, 21),
            ['Z'] = new Glyph(194, 62, 21, 30, 19),
            ['['] = new Glyph(215, 62, 13, 30, 11),
            ['\\'] = new Glyph(228, 62, 12, 30, 10),
            [']'] = new Glyph(240, 62, 13, 30, 11),
            ['^'] = new Glyph(253, 62, 20, 30, 18),
            ['_'] = new Glyph(273, 62, 21, 30, 19),
            ['`'] = new Glyph(294, 62, 13, 30, 11),
            ['a'] = new Glyph(307, 62, 20, 30, 18),
            ['b'] = new Glyph(327, 62, 21, 30, 19),
            ['c'] = new Glyph(348, 62, 20, 30, 18),
            ['d'] = new Glyph(368, 62, 21, 30, 19),
            ['e'] = new Glyph(389, 62, 20, 30, 18),
            ['f'] = new Glyph(409, 62, 14, 30, 12),
            ['g'] = new Glyph(423, 62, 21, 30, 19),
            ['h'] = new Glyph(444, 62, 21, 30, 19),
            ['i'] = new Glyph(465, 62, 12, 30, 10),
            ['j'] = new Glyph(477, 62, 13, 30, 11),
            ['k'] = new Glyph(490, 62, 20, 30, 18),
            ['l'] = new Glyph(2, 92, 12, 30, 10),
            ['m'] = new Glyph(14, 92, 29, 30, 27),
            ['n'] = new Glyph(43, 92, 21, 30, 19),
            ['o'] = new Glyph(64, 92, 21, 30, 19),
            ['p'] = new Glyph(85, 92, 21, 30, 19),
            ['q'] = new Glyph(106, 92, 21, 30, 19),
            ['r'] = new Glyph(127, 92, 15, 30, 13),
            ['s'] = new Glyph(142, 92, 20, 30, 18),
            ['t'] = new Glyph(162, 92, 13, 30, 11),
            ['u'] = new Glyph(175, 92, 21, 30, 19),
            ['v'] = new Glyph(196, 92, 20, 30, 18),
            ['w'] = new Glyph(216, 92, 27, 30, 25),
            ['x'] = new Glyph(243, 92, 20, 30, 18),
            ['y'] = new Glyph(263, 92, 20, 30, 18),
            ['z'] = new Glyph(283, 92, 18, 30, 16),
            ['{'] = new Glyph(301, 92, 15, 30, 13),
            ['|'] = new Glyph(316, 92, 12, 30, 10),
            ['}'] = new Glyph(328, 92, 15, 30, 13),
            ['~'] = new Glyph(343, 92, 20, 30, 18),
        };
    }

    public static class TitleData
    {
        // Auto-generated glyph metrics for Title
        public const string TextureAsset = "Fonts/TitleBitmapFont";
        public const int LineHeight = 57;
        public const int Size = 56;
        public static readonly Dictionary<char, Glyph> Glyphs = new()
        {
            [' '] = new Glyph(2, 2, 20, 57, 20),
            ['!'] = new Glyph(22, 2, 23, 57, 23),
            ['"'] = new Glyph(45, 2, 31, 57, 31),
            ['#'] = new Glyph(76, 2, 35, 57, 35),
            ['$'] = new Glyph(111, 2, 35, 57, 35),
            ['%'] = new Glyph(146, 2, 54, 57, 54),
            ['&'] = new Glyph(200, 2, 44, 57, 44),
            ['\''] = new Glyph(244, 2, 17, 57, 17),
            ['('] = new Glyph(261, 2, 23, 57, 23),
            [')'] = new Glyph(284, 2, 23, 57, 23),
            ['*'] = new Glyph(307, 2, 26, 57, 26),
            ['+'] = new Glyph(333, 2, 37, 57, 37),
            [','] = new Glyph(370, 2, 20, 57, 20),
            ['-'] = new Glyph(390, 2, 23, 57, 23),
            ['.'] = new Glyph(413, 2, 20, 57, 20),
            ['/'] = new Glyph(433, 2, 20, 57, 20),
            ['0'] = new Glyph(453, 2, 35, 57, 35),
            ['1'] = new Glyph(2, 59, 35, 57, 35),
            ['2'] = new Glyph(37, 59, 35, 57, 35),
            ['3'] = new Glyph(72, 59, 35, 57, 35),
            ['4'] = new Glyph(107, 59, 35, 57, 35),
            ['5'] = new Glyph(142, 59, 35, 57, 35),
            ['6'] = new Glyph(177, 59, 35, 57, 35),
            ['7'] = new Glyph(212, 59, 35, 57, 35),
            ['8'] = new Glyph(247, 59, 35, 57, 35),
            ['9'] = new Glyph(282, 59, 35, 57, 35),
            [':'] = new Glyph(317, 59, 23, 57, 23),
            [';'] = new Glyph(340, 59, 23, 57, 23),
            ['<'] = new Glyph(363, 59, 37, 57, 37),
            ['='] = new Glyph(400, 59, 37, 57, 37),
            ['>'] = new Glyph(437, 59, 37, 57, 37),
            ['?'] = new Glyph(2, 116, 38, 57, 38),
            ['@'] = new Glyph(40, 116, 59, 57, 59),
            ['A'] = new Glyph(99, 116, 44, 57, 44),
            ['B'] = new Glyph(143, 116, 44, 57, 44),
            ['C'] = new Glyph(187, 116, 44, 57, 44),
            ['D'] = new Glyph(231, 116, 44, 57, 44),
            ['E'] = new Glyph(275, 116, 41, 57, 41),
            ['F'] = new Glyph(316, 116, 38, 57, 38),
            ['G'] = new Glyph(354, 116, 48, 57, 48),
            ['H'] = new Glyph(402, 116, 44, 57, 44),
            ['I'] = new Glyph(446, 116, 20, 57, 20),
            ['J'] = new Glyph(466, 116, 35, 57, 35),
            ['K'] = new Glyph(2, 173, 44, 57, 44),
            ['L'] = new Glyph(46, 173, 38, 57, 38),
            ['M'] = new Glyph(84, 173, 51, 57, 51),
            ['N'] = new Glyph(135, 173, 44, 57, 44),
            ['O'] = new Glyph(179, 173, 48, 57, 48),
            ['P'] = new Glyph(227, 173, 41, 57, 41),
            ['Q'] = new Glyph(268, 173, 48, 57, 48),
            ['R'] = new Glyph(316, 173, 44, 57, 44),
            ['S'] = new Glyph(360, 173, 41, 57, 41),
            ['T'] = new Glyph(401, 173, 38, 57, 38),
            ['U'] = new Glyph(439, 173, 44, 57, 44),
            ['V'] = new Glyph(2, 230, 41, 57, 41),
            ['W'] = new Glyph(43, 230, 57, 57, 57),
            ['X'] = new Glyph(100, 230, 41, 57, 41),
            ['Y'] = new Glyph(141, 230, 41, 57, 41),
            ['Z'] = new Glyph(182, 230, 38, 57, 38),
            ['['] = new Glyph(220, 230, 23, 57, 23),
            ['\\'] = new Glyph(243, 230, 20, 57, 20),
            [']'] = new Glyph(263, 230, 23, 57, 23),
            ['^'] = new Glyph(286, 230, 37, 57, 37),
            ['_'] = new Glyph(323, 230, 37, 57, 37),
            ['`'] = new Glyph(360, 230, 23, 57, 23),
            ['a'] = new Glyph(383, 230, 36, 57, 36),
            ['b'] = new Glyph(419, 230, 38, 57, 38),
            ['c'] = new Glyph(457, 230, 35, 57, 35),
            ['d'] = new Glyph(2, 287, 38, 57, 38),
            ['e'] = new Glyph(40, 287, 35, 57, 35),
            ['f'] = new Glyph(75, 287, 23, 57, 23),
            ['g'] = new Glyph(98, 287, 38, 57, 38),
            ['h'] = new Glyph(136, 287, 38, 57, 38),
            ['i'] = new Glyph(174, 287, 20, 57, 20),
            ['j'] = new Glyph(194, 287, 21, 57, 21),
            ['k'] = new Glyph(215, 287, 36, 57, 36),
            ['l'] = new Glyph(251, 287, 20, 57, 20),
            ['m'] = new Glyph(271, 287, 54, 57, 54),
            ['n'] = new Glyph(325, 287, 38, 57, 38),
            ['o'] = new Glyph(363, 287, 38, 57, 38),
            ['p'] = new Glyph(401, 287, 38, 57, 38),
            ['q'] = new Glyph(439, 287, 38, 57, 38),
            ['r'] = new Glyph(477, 287, 26, 57, 26),
            ['s'] = new Glyph(2, 344, 35, 57, 35),
            ['t'] = new Glyph(37, 344, 23, 57, 23),
            ['u'] = new Glyph(60, 344, 38, 57, 38),
            ['v'] = new Glyph(98, 344, 35, 57, 35),
            ['w'] = new Glyph(133, 344, 49, 57, 49),
            ['x'] = new Glyph(182, 344, 35, 57, 35),
            ['y'] = new Glyph(217, 344, 35, 57, 35),
            ['z'] = new Glyph(252, 344, 32, 57, 32),
            ['{'] = new Glyph(284, 344, 26, 57, 26),
            ['|'] = new Glyph(310, 344, 20, 57, 20),
            ['}'] = new Glyph(330, 344, 26, 57, 26),
            ['~'] = new Glyph(356, 344, 37, 57, 37),
        };
    }
}
