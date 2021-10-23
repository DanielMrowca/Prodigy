using System.IO;
using Figgle;

namespace Prodigy.Figlet
{
    public static class FiggleFonts
    {
        public static FiggleFont ANSI_Shadow => FiggleFontParser.Parse(new MemoryStream(Resource1.ANSI_Shadow));
    }
}
