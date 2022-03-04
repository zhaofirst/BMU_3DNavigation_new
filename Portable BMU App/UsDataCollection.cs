using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultrasonics3DReconstructor
{
    public class USDataCollection2
    {
        public int framesCount;

        public byte[][,] frames;        // Source frame in sonixTablet[height,width] [480,640]

        public byte[][,] changeFrames;  // The frames after resolution changed

        public string fileName;         // file name: us29

        public string filePath;         // filePath example: D:\Users\Chenhb\Documents\G4Data\2021-01-19-16-37-49

        public static short[,] corolnalMatrixs; // 3D reconstruction volume projection on corolnal plane

        //Voxel Size default
        public float voxelWidth = 0.5f;
        public float voxelHeight = 0.5f;
        public float voxelDepth = 1f;

        public string selectedMethod = "VNN";   // default selected method is VNN in radioButton Group

        public string savedPath = null;

        public Rect3[] calibratedRects; // coordinate of rectangle after transformation

        public bool isHoleFillingUsed = false;

        public USDataCollection2(int m_framesCount, byte[][,] m_frames, byte[][,] m_changeFrames, Rect3[] m_rects, string m_fileName, string m_filePath)
        {
            framesCount = m_framesCount;
            frames = m_frames;
            calibratedRects = m_rects;
            changeFrames = m_changeFrames;
            fileName = m_fileName;
            filePath = m_filePath;
        }


        public USDataCollection2 Clone()
        {
            USDataCollection2 usDataCollection = new USDataCollection2(framesCount, frames, changeFrames,calibratedRects,fileName, filePath);

            usDataCollection.framesCount = framesCount;
            usDataCollection.frames = frames;
            usDataCollection.changeFrames = changeFrames;
            usDataCollection.calibratedRects = calibratedRects;
            usDataCollection.fileName = fileName;
            usDataCollection.filePath = filePath;

            return usDataCollection;
        }

        /// <summary>
        /// A void constructor of UsDataCollecction
        /// </summary>
        public USDataCollection2()
        {

        }

    }
}
