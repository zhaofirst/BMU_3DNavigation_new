using System.Runtime.InteropServices;

namespace Ultrasonics3DReconstructor
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct US_FILE_HEADER
    {

        public int type;

        public int frames;      //frameCounts

        public int w;           //frameWidth

        public int h;           //frameHeight

        public int ss;

        public int ulx;

        public int uly;

        public int urx;

        public int ury;

        public int brx;

        public int bry;

        public int blx;

        public int bly;

        public int probe;

        public int txf;

        public int sf;

        public int dr;

        public int ld;

        public int extra;

    }
}
