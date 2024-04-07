using UnityEngine;

namespace GamingMonks.Utils
{
    /// <summary>
	/// Extention class for Vector operations.
	/// </summary>
    public static class VectorUtils
    {
        // Replace X value of Vector3 with given value.
        public static Vector3 WithNewX(this Vector3 vector, float x)
        {
            vector.x = x;
            return vector;
        }

        // Replace Y value of Vector3 with given value.
        public static Vector3 WithNewY(this Vector3 vector, float y)
        {
            vector.y = y;
            return vector;
        }

        // Replace Z value of Vector3 with given value.
        public static Vector3 WithNewZ(this Vector3 vector, float z)
        {
            vector.z = z;
            return vector;
        }

        // Replace X value of Vector2 with given value.
        public static Vector2 WithNewX(this Vector2 vector, float x)
        {
            vector.x = x;
            return vector;
        }

        // Replace Y value of Vector2 with given value.
        public static Vector2 WithNewY(this Vector2 vector, float y)
        {
            vector.y = y;
            return vector;
        }

        public static Vector3 GetGlobalToLocalScaleFactor(Transform t)
        {
            Vector3 factor = Vector3.one;

            while (true)
            {
                Transform tParent = t.parent;

                if (tParent != null)
                {
                    factor.x *= tParent.localScale.x;
                    factor.y *= tParent.localScale.y;
                    factor.z *= tParent.localScale.z;

                    t = tParent;
                }
                else
                {
                    return factor;
                }
            }
        }
    }
}
