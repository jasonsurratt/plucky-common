using UnityEngine;

namespace Plucky.Common
{
    public static class TerrainDataExtensions
    {
        /// <summary>
        /// GetNormalizedLocation returns a point in 2d space that corresponds to 0..1 on the
        /// terrain data. This is handy for functions like GetInterpolatedHeight. The result will
        /// be clamped to 0..1.
        /// 
        /// This assumes the terrain is not scaled/rotated
        /// </summary>
        /// <param name="td"></param>
        /// <param name="pos">pos is the location in world space to transform. The y value is ignored.</param>
        /// <returns></returns>
        public static Vector2 GetNormalizedLocation(this Terrain t, Vector3 pos)
        {
            Vector2 result;
            var bounds = t.GetComponent<TerrainCollider>().bounds;
            result.x = Mathf.InverseLerp(bounds.min.x, bounds.max.x, pos.x);
            result.x = Mathf.Clamp01(result.x);
            result.y = Mathf.InverseLerp(bounds.min.z, bounds.max.z, pos.z);
            result.y = Mathf.Clamp01(result.y);

            return result;
        }
    }
}
