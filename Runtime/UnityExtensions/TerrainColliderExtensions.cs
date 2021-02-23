using UnityEngine;

namespace Plucky.Common
{

    public static class TerrainColliderExtensions
    {

        public static Vector3 GetNearishPoint(this TerrainCollider tc, Vector3 pos)
        {
            var td = tc.terrainData;
            var tdBounds = td.bounds;
            tdBounds.center = tc.transform.position;

            Vector3 closestToBounds = tdBounds.ClosestPoint(pos);

            var terrainLocalPos = pos - tc.transform.position;
            var normalizedPos = new Vector2(Mathf.InverseLerp(0.0f, td.size.x, terrainLocalPos.x),
                                        Mathf.InverseLerp(0.0f, td.size.z, terrainLocalPos.z));
            var height = td.GetInterpolatedHeight(normalizedPos.x, normalizedPos.y);

            Vector3 result = closestToBounds;
            result.y = tc.transform.position.y + height;

            Debug.Log($"{result} {height} {normalizedPos.x} {normalizedPos.y}");

            return result;
        }
    }
}
