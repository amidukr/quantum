using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Quantum.Quantum.Renders
{
    class MotionBlurFilter
    {
        private readonly Image blurImage;

        public MotionBlurFilter(int width, int height)
        {
            blurImage = new Bitmap(width, height);
        }

        public void Draw(GameEvent gameEvent, Graphics userOutput, Image mainImage)
        {
            Graphics blurG = Graphics.FromImage(blurImage);

            ColorMatrix matrix = new ColorMatrix(new float[][]{
                new float[] {1F, 0, 0, 0,    0},
                new float[] {0, 1F, 0, 0,    0},
                new float[] {0, 0, 1F, 0,    0},
                new float[] {0, 0, 0, 0.8F,  0},
                new float[] {0, 0, 0, 0,    1F}});
            
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(matrix);

            blurG.CompositingMode = CompositingMode.SourceOver;

            blurG.DrawImage(mainImage,
                        new Rectangle(0, 0, mainImage.Width, mainImage.Height),
                        0,
                        0,
                        mainImage.Width,
                        mainImage.Height,
                        GraphicsUnit.Pixel,
                        imageAttributes);

            userOutput.DrawImage(blurImage, 0, 0, gameEvent.screenWidth, gameEvent.screenHeight);
        }
    }
}
