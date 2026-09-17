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
        // Bellota Bold + stroke, shared baseline
        public const string TextureAsset = "Fonts/UiBitmapFont";
        public const int LineHeight = 49;
        public const int Size = 28;
        public static readonly Dictionary<char, Glyph> Glyphs = new()
        {
            [' '] = new Glyph(2, 2, 11, 49, 11),
            ['!'] = new Glyph(15, 2, 10, 49, 10),
            ['"'] = new Glyph(27, 2, 13, 49, 14),
            ['#'] = new Glyph(42, 2, 21, 49, 21),
            ['$'] = new Glyph(65, 2, 20, 49, 20),
            ['%'] = new Glyph(87, 2, 26, 49, 26),
            ['&'] = new Glyph(115, 2, 24, 49, 24),
            ['\''] = new Glyph(141, 2, 10, 49, 11),
            ['('] = new Glyph(153, 2, 15, 49, 15),
            [')'] = new Glyph(170, 2, 16, 49, 16),
            ['*'] = new Glyph(188, 2, 16, 49, 16),
            ['+'] = new Glyph(206, 2, 18, 49, 18),
            [','] = new Glyph(226, 2, 10, 49, 10),
            ['-'] = new Glyph(238, 2, 15, 49, 16),
            ['.'] = new Glyph(255, 2, 10, 49, 10),
            ['/'] = new Glyph(267, 2, 15, 49, 15),
            ['0'] = new Glyph(284, 2, 22, 49, 22),
            ['1'] = new Glyph(308, 2, 14, 49, 14),
            ['2'] = new Glyph(324, 2, 19, 49, 19),
            ['3'] = new Glyph(345, 2, 20, 49, 20),
            ['4'] = new Glyph(367, 2, 20, 49, 20),
            ['5'] = new Glyph(389, 2, 19, 49, 19),
            ['6'] = new Glyph(410, 2, 20, 49, 20),
            ['7'] = new Glyph(432, 2, 20, 49, 20),
            ['8'] = new Glyph(454, 2, 20, 49, 20),
            ['9'] = new Glyph(476, 2, 20, 49, 20),
            [':'] = new Glyph(498, 2, 10, 49, 10),
            [';'] = new Glyph(2, 53, 11, 49, 11),
            ['<'] = new Glyph(15, 53, 17, 49, 17),
            ['='] = new Glyph(34, 53, 17, 49, 20),
            ['>'] = new Glyph(53, 53, 17, 49, 17),
            ['?'] = new Glyph(72, 53, 19, 49, 19),
            ['@'] = new Glyph(93, 53, 23, 49, 24),
            ['A'] = new Glyph(118, 53, 27, 49, 27),
            ['B'] = new Glyph(147, 53, 24, 49, 24),
            ['C'] = new Glyph(173, 53, 25, 49, 25),
            ['D'] = new Glyph(200, 53, 27, 49, 27),
            ['E'] = new Glyph(229, 53, 23, 49, 23),
            ['F'] = new Glyph(254, 53, 23, 49, 23),
            ['G'] = new Glyph(279, 53, 25, 49, 25),
            ['H'] = new Glyph(306, 53, 24, 49, 24),
            ['I'] = new Glyph(332, 53, 10, 49, 11),
            ['J'] = new Glyph(344, 53, 18, 49, 18),
            ['K'] = new Glyph(364, 53, 25, 49, 25),
            ['L'] = new Glyph(391, 53, 19, 49, 19),
            ['M'] = new Glyph(412, 53, 34, 49, 34),
            ['N'] = new Glyph(448, 53, 26, 49, 26),
            ['O'] = new Glyph(476, 53, 26, 49, 26),
            ['P'] = new Glyph(2, 104, 23, 49, 23),
            ['Q'] = new Glyph(27, 104, 26, 49, 26),
            ['R'] = new Glyph(55, 104, 26, 49, 26),
            ['S'] = new Glyph(83, 104, 21, 49, 21),
            ['T'] = new Glyph(106, 104, 23, 49, 23),
            ['U'] = new Glyph(131, 104, 25, 49, 25),
            ['V'] = new Glyph(158, 104, 24, 49, 24),
            ['W'] = new Glyph(184, 104, 36, 49, 36),
            ['X'] = new Glyph(222, 104, 23, 49, 23),
            ['Y'] = new Glyph(247, 104, 23, 49, 23),
            ['Z'] = new Glyph(272, 104, 21, 49, 21),
            ['['] = new Glyph(295, 104, 14, 49, 14),
            ['\\'] = new Glyph(311, 104, 15, 49, 15),
            [']'] = new Glyph(328, 104, 14, 49, 14),
            ['^'] = new Glyph(344, 104, 18, 49, 18),
            ['_'] = new Glyph(364, 104, 19, 49, 20),
            ['`'] = new Glyph(385, 104, 13, 49, 13),
            ['a'] = new Glyph(400, 104, 22, 49, 22),
            ['b'] = new Glyph(424, 104, 19, 49, 19),
            ['c'] = new Glyph(445, 104, 19, 49, 19),
            ['d'] = new Glyph(466, 104, 22, 49, 22),
            ['e'] = new Glyph(490, 104, 19, 49, 19),
            ['f'] = new Glyph(2, 155, 16, 49, 16),
            ['g'] = new Glyph(20, 155, 19, 49, 19),
            ['h'] = new Glyph(41, 155, 21, 49, 21),
            ['i'] = new Glyph(64, 155, 13, 49, 13),
            ['j'] = new Glyph(79, 155, 13, 49, 13),
            ['k'] = new Glyph(94, 155, 20, 49, 20),
            ['l'] = new Glyph(116, 155, 12, 49, 12),
            ['m'] = new Glyph(130, 155, 29, 49, 29),
            ['n'] = new Glyph(161, 155, 22, 49, 22),
            ['o'] = new Glyph(185, 155, 20, 49, 20),
            ['p'] = new Glyph(207, 155, 20, 49, 20),
            ['q'] = new Glyph(229, 155, 19, 49, 19),
            ['r'] = new Glyph(250, 155, 16, 49, 16),
            ['s'] = new Glyph(268, 155, 18, 49, 18),
            ['t'] = new Glyph(288, 155, 15, 49, 15),
            ['u'] = new Glyph(305, 155, 23, 49, 23),
            ['v'] = new Glyph(330, 155, 20, 49, 20),
            ['w'] = new Glyph(352, 155, 28, 49, 28),
            ['x'] = new Glyph(382, 155, 21, 49, 21),
            ['y'] = new Glyph(405, 155, 19, 49, 19),
            ['z'] = new Glyph(426, 155, 20, 49, 20),
            ['{'] = new Glyph(448, 155, 15, 49, 15),
            ['|'] = new Glyph(465, 155, 9, 49, 12),
            ['}'] = new Glyph(476, 155, 15, 49, 15),
            ['~'] = new Glyph(2, 206, 18, 49, 19),
        };
    }

    public static class TitleData
    {
        // Sniglet ExtraBold + stroke, shared baseline
        public const string TextureAsset = "Fonts/TitleBitmapFont";
        public const int LineHeight = 92;
        public const int Size = 60;
        public static readonly Dictionary<char, Glyph> Glyphs = new()
        {
            [' '] = new Glyph(2, 2, 31, 92, 31),
            ['!'] = new Glyph(35, 2, 31, 92, 32),
            ['"'] = new Glyph(68, 2, 33, 92, 33),
            ['#'] = new Glyph(103, 2, 52, 92, 52),
            ['$'] = new Glyph(157, 2, 40, 92, 40),
            ['%'] = new Glyph(199, 2, 49, 92, 50),
            ['&'] = new Glyph(250, 2, 54, 92, 54),
            ['\''] = new Glyph(306, 2, 21, 92, 21),
            ['('] = new Glyph(329, 2, 33, 92, 33),
            [')'] = new Glyph(364, 2, 33, 92, 33),
            ['*'] = new Glyph(399, 2, 39, 92, 43),
            ['+'] = new Glyph(440, 2, 43, 92, 43),
            [','] = new Glyph(485, 2, 26, 92, 26),
            ['-'] = new Glyph(513, 2, 38, 92, 38),
            ['.'] = new Glyph(553, 2, 27, 92, 27),
            ['/'] = new Glyph(582, 2, 33, 92, 33),
            ['0'] = new Glyph(617, 2, 42, 92, 42),
            ['1'] = new Glyph(661, 2, 37, 92, 37),
            ['2'] = new Glyph(700, 2, 42, 92, 42),
            ['3'] = new Glyph(744, 2, 45, 92, 45),
            ['4'] = new Glyph(791, 2, 46, 92, 46),
            ['5'] = new Glyph(839, 2, 42, 92, 42),
            ['6'] = new Glyph(883, 2, 43, 92, 43),
            ['7'] = new Glyph(928, 2, 42, 92, 42),
            ['8'] = new Glyph(972, 2, 44, 92, 44),
            ['9'] = new Glyph(2, 96, 43, 92, 43),
            [':'] = new Glyph(47, 96, 24, 92, 25),
            [';'] = new Glyph(73, 96, 24, 92, 25),
            ['<'] = new Glyph(99, 96, 34, 92, 34),
            ['='] = new Glyph(135, 96, 37, 92, 37),
            ['>'] = new Glyph(174, 96, 34, 92, 34),
            ['?'] = new Glyph(210, 96, 43, 92, 44),
            ['@'] = new Glyph(255, 96, 49, 92, 49),
            ['A'] = new Glyph(306, 96, 51, 92, 51),
            ['B'] = new Glyph(359, 96, 47, 92, 47),
            ['C'] = new Glyph(408, 96, 50, 92, 50),
            ['D'] = new Glyph(460, 96, 47, 92, 47),
            ['E'] = new Glyph(509, 96, 46, 92, 46),
            ['F'] = new Glyph(557, 96, 46, 92, 46),
            ['G'] = new Glyph(605, 96, 54, 92, 54),
            ['H'] = new Glyph(661, 96, 53, 92, 53),
            ['I'] = new Glyph(716, 96, 33, 92, 33),
            ['J'] = new Glyph(751, 96, 40, 92, 40),
            ['K'] = new Glyph(793, 96, 50, 92, 50),
            ['L'] = new Glyph(845, 96, 46, 92, 46),
            ['M'] = new Glyph(893, 96, 66, 92, 66),
            ['N'] = new Glyph(961, 96, 57, 92, 57),
            ['O'] = new Glyph(2, 190, 52, 92, 52),
            ['P'] = new Glyph(56, 190, 49, 92, 49),
            ['Q'] = new Glyph(107, 190, 55, 92, 55),
            ['R'] = new Glyph(164, 190, 49, 92, 49),
            ['S'] = new Glyph(215, 190, 44, 92, 44),
            ['T'] = new Glyph(261, 190, 51, 92, 51),
            ['U'] = new Glyph(314, 190, 53, 92, 53),
            ['V'] = new Glyph(369, 190, 57, 92, 57),
            ['W'] = new Glyph(428, 190, 67, 92, 67),
            ['X'] = new Glyph(497, 190, 55, 92, 55),
            ['Y'] = new Glyph(554, 190, 53, 92, 53),
            ['Z'] = new Glyph(609, 190, 50, 92, 50),
            ['['] = new Glyph(661, 190, 43, 92, 43),
            ['\\'] = new Glyph(706, 190, 33, 92, 35),
            [']'] = new Glyph(741, 190, 43, 92, 43),
            ['^'] = new Glyph(786, 190, 44, 92, 44),
            ['_'] = new Glyph(832, 190, 46, 92, 46),
            ['`'] = new Glyph(880, 190, 27, 92, 28),
            ['a'] = new Glyph(909, 190, 51, 92, 51),
            ['b'] = new Glyph(962, 190, 47, 92, 47),
            ['c'] = new Glyph(2, 284, 42, 92, 42),
            ['d'] = new Glyph(46, 284, 52, 92, 52),
            ['e'] = new Glyph(100, 284, 43, 92, 43),
            ['f'] = new Glyph(145, 284, 45, 92, 45),
            ['g'] = new Glyph(192, 284, 48, 92, 48),
            ['h'] = new Glyph(242, 284, 50, 92, 50),
            ['i'] = new Glyph(294, 284, 30, 92, 30),
            ['j'] = new Glyph(326, 284, 36, 92, 36),
            ['k'] = new Glyph(364, 284, 51, 92, 51),
            ['l'] = new Glyph(417, 284, 31, 92, 31),
            ['m'] = new Glyph(450, 284, 66, 92, 66),
            ['n'] = new Glyph(518, 284, 50, 92, 50),
            ['o'] = new Glyph(570, 284, 44, 92, 44),
            ['p'] = new Glyph(616, 284, 50, 92, 50),
            ['q'] = new Glyph(668, 284, 51, 92, 51),
            ['r'] = new Glyph(721, 284, 45, 92, 45),
            ['s'] = new Glyph(768, 284, 40, 92, 40),
            ['t'] = new Glyph(810, 284, 50, 92, 50),
            ['u'] = new Glyph(862, 284, 47, 92, 47),
            ['v'] = new Glyph(911, 284, 48, 92, 48),
            ['w'] = new Glyph(961, 284, 60, 92, 60),
            ['x'] = new Glyph(2, 378, 45, 92, 45),
            ['y'] = new Glyph(49, 378, 50, 92, 50),
            ['z'] = new Glyph(101, 378, 43, 92, 43),
            ['{'] = new Glyph(146, 378, 33, 92, 39),
            ['|'] = new Glyph(181, 378, 23, 92, 23),
            ['}'] = new Glyph(206, 378, 34, 92, 36),
            ['~'] = new Glyph(242, 378, 36, 92, 36),
        };
    }
}
