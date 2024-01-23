using UnityEngine;

namespace Util
{
    public static class Util
    {
        public static float RandomInRange(this Vector2 vec)
        {
            return Random.Range(vec.x, vec.y);
        }
    }
}
