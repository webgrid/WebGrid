/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/


#region Header

/*
Copyright ©  Olav Christian Botterli. 

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/

#endregion Header

namespace WebGrid.Util
{
    using System;
    using System.Drawing;
    using System.Drawing.Text;

    /// <summary>
    /// This class contains methods and properties to create a headline for a chart.
    /// This class is used by <see cref="WebGrid.Chart">WebGrid.Chart</see> when generating charts
    /// for the <see cref="WebGrid.Grid">WebGrid.Grid</see> web control.
    /// </summary>
    public class ChartHeadline
    {
        #region Fields

        internal ChartAlignTypes Align = ChartAlignTypes.Center | ChartAlignTypes.Top; // Left,Center,Right
        internal string FontName = "Arial";
        internal int FontSize = 16;
        internal string Headline = string.Empty;
        internal Color HeadlineColor = Color.White;
        internal Color HeadlineShadowColor = Color.Black;
        internal bool UseShadow = true;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Headline object.
        /// </summary>
        /// <param name="headline">The text for the headline. Default settings: Font=Arial, Color=White , fontsize=16, alignment = Center | Top.</param>
        public ChartHeadline(string headline)
        {
            Headline = headline;
        }

        /// <summary>
        /// Headline object.
        /// </summary>
        /// <param name="headline">The text for the headline. Default settings: Font=Arial, Color=White , fontsize=16, alignment = Center | Top.</param>
        /// <param name="align">Defined with ChartHeadline.AlignTypes. Defaullt is Center | Top.</param>
        public ChartHeadline(string headline, ChartAlignTypes align)
        {
            Headline = headline;
            Align = align;
        }

        /// <summary>
        /// Headline object.
        /// </summary>
        /// <param name="headline">The text for the headline. Default settings: Font=Arial, Color=White , fontsize=16, alignment = Center | Top.</param>
        /// <param name="fontSize">Font size for the headline. Default is 16.</param>
        /// <param name="headlineColor">Color for the headline. Default is white.</param>
        public ChartHeadline(string headline, int fontSize, Color headlineColor)
        {
            Headline = headline;
            FontSize = fontSize;
            HeadlineColor = headlineColor;
        }

        /// <summary>
        /// Headline object.
        /// </summary>
        /// <param name="headline">The text for the headline. Default settings: Font=Arial, Color=White , fontsize=16, alignment = Center | Top.</param>
        /// <param name="fontSize">Font size for the headline. Default is 16.</param>
        /// <param name="headlineColor">Color for the headline. Default is white.</param>
        /// <param name="align">Defined with ChartHeadline.AlignTypes. Defaullt is Top | Center.</param>
        /// <param name="useShadow">Toggle the shadow. Default is true (on).</param>
        public ChartHeadline(string headline, int fontSize, Color headlineColor, ChartAlignTypes align, bool useShadow)
        {
            Headline = headline;
            FontSize = fontSize;
            HeadlineColor = headlineColor;
            Align = align;
            UseShadow = useShadow;
        }

        /// <summary>
        /// Creates a headline / title for the chart
        /// </summary>
        /// <param name="headline">The text for the headline. Default settings: Font=Arial, Color=White , fontsize=16, alignment = Center | Top.</param>
        /// <param name="fontname">Font name  for the headline (Default is "Arial") </param>
        /// <param name="fontSize">Font size (Default is 16) </param>
        /// <param name="headlineColor">Color for the headline. (Default is white)</param>
        /// <param name="align">Defind with ChartHeadline.AlignTypes (defaullt is Top | Center </param>
        /// <param name="useShadow">Toogle the shadow (Defaullt is true (on)</param>
        public ChartHeadline(string headline, String fontname, int fontSize, Color headlineColor, ChartAlignTypes align,
            bool useShadow)
        {
            Headline = headline;
            FontSize = fontSize;
            FontName = fontname;
            HeadlineColor = headlineColor;
            Align = align;
            UseShadow = useShadow;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Draws the headline and returns the new area for the chart to be drawn in.
        /// </summary>
        /// <param name="bitmap"></param>
        public Rectangle Render(Bitmap bitmap)
        {
            Graphics gfx = Graphics.FromImage(bitmap);

            int width = bitmap.Width;
            int height = bitmap.Height;

            gfx.TextRenderingHint = TextRenderingHint.AntiAlias;

            Rectangle newDrawArea = new Rectangle(0, 0, width, height);

            Font headlineFont = new Font(FontName, FontSize);
            // Set format of string.
            StringFormat drawFormat = new StringFormat();

            float x = 0.0f;
            float y = 0.0f;

            SizeF stringSize = gfx.MeasureString(Headline, headlineFont);

            float spaceingPixels = stringSize.Height/4;

            switch (Align & (ChartAlignTypes.Left | ChartAlignTypes.Center | ChartAlignTypes.Right))
            {
                case ChartAlignTypes.Left:
                    x = spaceingPixels;
                    break;
                case ChartAlignTypes.Center:
                    x = (float)width / 2 - (stringSize.Width / 2);
                    break;
                case ChartAlignTypes.Right:
                    x = width - (spaceingPixels + stringSize.Width);
                    break;
            }

            switch (Align & (ChartAlignTypes.Top | ChartAlignTypes.Middle | ChartAlignTypes.Bottom))
            {
                case ChartAlignTypes.Top:
                    y = spaceingPixels;
                    newDrawArea =
                        new Rectangle(0, (int) (y + stringSize.Height), width - 1,
                                      (int) (height - (stringSize.Height + (spaceingPixels*2))));
                    break;
                case ChartAlignTypes.Middle:
                    y = (float)height/2 - (stringSize.Height/2);

                    if ((Align & (ChartAlignTypes.Left | ChartAlignTypes.Center | ChartAlignTypes.Right)) ==
                        ChartAlignTypes.Left)
                    {
                        int textX2 = (int) (x + stringSize.Width + (spaceingPixels*2));
                        newDrawArea = new Rectangle(textX2, 0, (width - textX2) - 1, height - 1);
                    }
                    if ((Align & (ChartAlignTypes.Left | ChartAlignTypes.Center | ChartAlignTypes.Right)) ==
                        ChartAlignTypes.Right)
                    {
                        newDrawArea =
                            new Rectangle(0, 0, (int) (width - (stringSize.Width + ((spaceingPixels*1)))), height - 1);
                    }

                    break;
                case ChartAlignTypes.Bottom:
                    y = height - (spaceingPixels + stringSize.Height);
                    newDrawArea =
                        new Rectangle(0, 0, width - 1, (int) (height - (stringSize.Height + ((spaceingPixels*1)))));
                    break;
            }

            if (UseShadow)
            {
                // (Set same Alpha for shadow)
                gfx.DrawString(
                    Headline,
                    headlineFont,
                    new SolidBrush(
                        Color.FromArgb(
                            HeadlineColor.A,
                            HeadlineShadowColor.R,
                            HeadlineShadowColor.G,
                            HeadlineShadowColor.B)
                        ),
                    x + (headlineFont.Size/10),
                    y + (headlineFont.Size/10),
                    drawFormat);
            }

            gfx.DrawString(
                Headline, headlineFont,
                new SolidBrush(HeadlineColor),
                x,
                y,
                drawFormat);

            return newDrawArea;
        }

        #endregion Methods
    }
}