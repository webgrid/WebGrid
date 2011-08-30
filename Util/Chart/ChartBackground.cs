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
    using System.Net;

    /// <summary>
    /// This class contains methods and properties to create backgrounds for a chart.
    /// This class is used by <see cref="WebGrid.Chart">WebGrid.Chart</see> when generating charts
    /// for the <see cref="WebGrid.Grid">WebGrid.Grid</see> web control.
    /// Default background color is white.
    /// </summary>
    public class ChartBackground
    {
        #region Fields

        internal Color BackgroundBorderColor = Color.Black;
        internal Color BackgroundColor;
        internal Color[] BackgroundColorShade;
        internal string BackgroundImageUrl;
        internal BackgroundTypes BackgroundType = BackgroundTypes.None;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Single color background.
        /// </summary>
        /// <param name="backgroundColor">The color.</param>
        public ChartBackground(Color backgroundColor)
        {
            BackgroundColor = backgroundColor;
            BackgroundType = BackgroundTypes.BackgroundColor;
        }

        /// <summary>
        /// Creates a Shade background
        /// </summary>
        /// <param name="topRight">Shade alignment.</param>
        /// <param name="topLeft">Shade alignment.</param>
        /// <param name="bottomRight">Shade alignment.</param>
        /// <param name="bottomLeft">Shade alignment.</param>
        public ChartBackground(Color topRight, Color topLeft, Color bottomRight, Color bottomLeft)
        {
            BackgroundColorShade = new Color[4];
            BackgroundColorShade[0] = topRight;
            BackgroundColorShade[1] = topLeft;
            BackgroundColorShade[2] = bottomRight;
            BackgroundColorShade[3] = bottomLeft;
            BackgroundType = BackgroundTypes.BackgroundColorShade;
        }

        /// <summary>
        /// Uses an image at the given url as background and scales it to fit the generated graphics.
        /// </summary>
        /// <param name="backgroundImageUrl">URL to the image.</param>
        public ChartBackground(string backgroundImageUrl)
        {
            BackgroundImageUrl = backgroundImageUrl;
            BackgroundType = BackgroundTypes.BackgroundImageUrl;
        }

        #endregion Constructors

        #region Enumerations

        internal enum BackgroundTypes
        {
            None = 0,
            BackgroundColor,
            BackgroundColorShade,
            BackgroundImageUrl
        }

        #endregion Enumerations

        #region Methods

        /// <summary>
        /// Renders the specified bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to be rendered.</param>
        public void Render(Bitmap bitmap)
        {
            Graphics gfx = Graphics.FromImage(bitmap);

            int width = bitmap.Width;
            int height = bitmap.Height;

            switch (BackgroundType)
            {
                    // Create single color background
                case BackgroundTypes.BackgroundColor:
                    gfx.FillRectangle(new SolidBrush(BackgroundColor), 0, 0, width, height);
                    break;

                    // Create Shaded color background
                case BackgroundTypes.BackgroundColorShade:

                    #region BackgroundColorShade code

                    float rL = BackgroundColorShade[0].R;
                    float gL = BackgroundColorShade[0].G;
                    float bL = BackgroundColorShade[0].B;

                    float rsL = (BackgroundColorShade[2].R - rL)/height;
                    float gsL = (BackgroundColorShade[2].G - gL)/height;
                    float bsL = (BackgroundColorShade[2].B - bL)/height;

                    float rR = BackgroundColorShade[1].R;
                    float gR = BackgroundColorShade[1].G;
                    float bR = BackgroundColorShade[1].B;

                    float rsR = (BackgroundColorShade[3].R - rR)/height;
                    float gsR = (BackgroundColorShade[3].G - gR)/height;
                    float bsR = (BackgroundColorShade[3].B - bR)/height;

                    for (int j = 0; j < height; j++)
                    {
                        float r = rL;
                        float g = gL;
                        float b = bL;

                        float rs = (rR - rL)/width;
                        float gs = (gR - gL)/width;
                        float bs = (bR - bL)/width;

                        for (int i = 0; i < width; i++)
                        {
                            bitmap.SetPixel(i, j,
                                            Color.FromArgb(
                                                255,
                                                Math.Max(0, Math.Min((int) r, 255)),
                                                Math.Max(0, Math.Min((int) g, 255)),
                                                Math.Max(0, Math.Min((int) b, 255))
                                                )
                                );

                            // Horizontal shade
                            r += rs;
                            g += gs;
                            b += bs;
                        }

                        // Shade top to bottom, left side
                        rL += rsL;
                        gL += gsL;
                        bL += bsL;

                        // Shade top to bottom, right side
                        rR += rsR;
                        gR += gsR;
                        bR += bsR;
                    }

                    #endregion

                    break;

                    // Create image background
                case BackgroundTypes.BackgroundImageUrl:

                    #region Code

                    if (BackgroundImageUrl.ToUpperInvariant().StartsWith("HTTP://"))
                    {
                        WebRequest req = WebRequest.Create(BackgroundImageUrl);
                        WebResponse resp = req.GetResponse();

                        System.Drawing.Image background = new Bitmap(resp.GetResponseStream());

                        Rectangle toSize = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

                        gfx.DrawImage(
                            background,
                            toSize
                            );

                        background.Dispose();
                    }
                    else
                    {
                        System.Drawing.Image background = new Bitmap(BackgroundImageUrl);

                        Rectangle toSize = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

                        gfx.DrawImage(
                            background,
                            toSize
                            );

                        background.Dispose();
                    }

                    #endregion

                    break;

                default:
                    // Just white bakground. BackgroundTypes.None
                    // TESTING: Color.White -> Color.Transparent
                    gfx.FillRectangle(new SolidBrush(Color.Transparent), 0, 0, width, height);
                    break;
            }

            // If forced to be empty, dont draw a border.
            if (!BackgroundBorderColor.IsEmpty)
            {
                // Draws a black border around the image...
                gfx.DrawRectangle(new Pen(BackgroundBorderColor, 1), 0, 0, width - 1, height - 1);
            }
        }

        #endregion Methods
    }
}