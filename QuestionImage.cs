using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleSearchConsole
{
    class QuestionImage
    {
        public QuestionImage()
        {
            //takes the screenshot of question
            var smallX = Math.Min(498, 887);
            var largeX = Math.Max(498, 887);
            var smallY = Math.Min(199, 346);
            var largeY = Math.Max(199, 346);

            using (var bitmap = new Bitmap((int)(largeX - smallX), (int)(largeY - smallY)))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(System.Drawing.Color.HotPink);
                    graphics.CopyFromScreen((int)smallX, (int)smallY, 0, 0, new Size((int)(largeX - smallX), (int)(largeY - smallY)));
                }

                bitmap.Save(@"E:\foo\screencap1.png");
            }
        }
    }
}
