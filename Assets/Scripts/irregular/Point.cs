using UnityEngine;

namespace Utils
{
    public static class Point {
        public static bool IsPointInTriangles(ushort[] triangles, Vector2[] vertices, Vector2 p)
        {
            int tri_count = triangles.Length / 3;
            for (int i = 0; i < tri_count; i++)
            {
                var idx = i * 3;
                var a = triangles[idx];
                var b = triangles[idx + 1];
                var c = triangles[idx + 2];

                var v1 = vertices[a];
                var v2 = vertices[b];
                var v3 = vertices[c];
                if (PointInTriangle(p, v1,v2,v3))
                    return true;
            }
            return false;
        }

        static float Sign(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return (v1.x - v3.x) * (v2.y - v3.y) - (v2.x - v3.x) * (v1.y - v3.y);
        }

        //https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
        public static bool PointInTriangle(Vector2 point, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = Sign(point, v1, v2);
            d2 = Sign(point, v2, v3);
            d3 = Sign(point, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }

        //point in polygon
        public static bool IsPointInPolygon(Vector2[] polyPoints, Vector2 p) 
        {
            var j = polyPoints.Length - 1;
            int cn = 0;
            for (int i = 0; i < polyPoints.Length; j = i++)
            {
                var pi = polyPoints[i];
                var pj = polyPoints[j];
                if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                    (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                {
                    ++cn;
                }
            }
            return (cn & 1) == 1;
            //return cn > 0;
        }

        //Return: >0 for P2 left of the line through P0 and P1
        //            =0 for P2  on the line
        //            <0 for P2  right of the line
        public static float IsLeft(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            return ((p1.x - p0.x) * (p2.y - p0.y) - (p2.x - p0.x) * (p1.y * p0.y));
        }

        /// <summary>
        /// winding number
        /// </summary>
        /// <param name="polyPoints"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsPointInPolygonByWindingNumber(Vector2[] polyPoints, Vector2 p)
        {
            int wn = 0;
            for (int i = 0; i < polyPoints.Length - 1; i++)
            {
                if (polyPoints[i].y <= p.y)  // edge from p[i] to  p[i+1]
                {
                    if (polyPoints[i + 1].y > p.y) // an upward crossing
                    {
                        if (IsLeft(polyPoints[i], polyPoints[i + 1], p) > 0f) // P left of  edge
                            ++wn;           // have  a valid up intersect
                    }
                }
                else   // start y > P.y (no test needed)
                {
                    if (polyPoints[i + 1].y <= p.y)     // a downward crossing
                        if (IsLeft(polyPoints[i], polyPoints[i + 1], p) < 0f) // P right of  edge
                            --wn;           // have  a valid down intersect
                }
            }
            return (wn & 1) == 1;
        }
    }
}
