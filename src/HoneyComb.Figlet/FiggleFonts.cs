using Figgle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HoneyComb.Figlet
{
    public static class FiggleFonts
    {
        public static FiggleFont ANSI_Shadow => FiggleFontParser.Parse(new MemoryStream(Resource1.ANSI_Shadow));
    }
}
