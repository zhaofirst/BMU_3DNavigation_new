using System.Linq;
using System.Numerics;

namespace Ultrasonics3DReconstructor
{
    public class Rect3
    {
        public Vector3[] v = new Vector3[4];

        /// <summary>
        /// 平均矩形四个点的XYZ，得到中心点坐标
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCenter()
        {
            Vector3 vector3Df = new Vector3(0f, 0f, 0f);
            for (int i = 0; i < 4; i++)
            {
                vector3Df.X += this.v[i].X / 4f;
                vector3Df.Y += this.v[i].Y / 4f;
                vector3Df.Z += this.v[i].Z / 4f;
            }
            return vector3Df;
        }

        public Rect3 Inflate(float offset)
        {
            Rect3 rect3Df = new Rect3();
            Vector3 center = this.GetCenter();

            rect3Df.v = this.v.Select(delegate (Vector3 v)
            {
                Vector3 vector3Df = v - center;

                return center + vector3Df + Vector3.Normalize(vector3Df) * offset;
            }).ToArray();
            return rect3Df;
        }

        public Vector3 Get3DCoordFrom2DNormalizedCoord(float x, float y)
        {
            Vector3 v = this.v[1] - this.v[0];
            Vector3 v2 = this.v[3] - this.v[0];
            return this.v[0] + v * x + v2 * y;
        }
    }
}
