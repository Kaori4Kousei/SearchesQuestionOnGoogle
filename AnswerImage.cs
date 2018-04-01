using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleSearchConsole
{
    class AnswerImage
    {
        public AnswerImage()
        {
            //takes the screenshot of question
            var smallX = Math.Min(489, 891);
            var largeX = Math.Max(489, 891);
            var smallY = Math.Min(337, 561);
            var largeY = Math.Max(337, 561);

            using (var bitmap = new Bitmap((int)(largeX - smallX), (int)(largeY - smallY)))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(System.Drawing.Color.HotPink);
                    graphics.CopyFromScreen((int)smallX, (int)smallY, 0, 0, new Size((int)(largeX - smallX), (int)(largeY - smallY)));
                }

                bitmap.Save(@"E:\foo\screencap2.png");
            }
        }
    }
}
