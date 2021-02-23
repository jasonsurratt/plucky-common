using UnityEngine;

namespace Plucky.Common
{

    public static class TerrainExtensions
    {
        /// <summary>
        /// Get TerrainData returns the terrain data below or above a given point.
        /// </summary>
        public static Terrain GetTerrain(Vector3 pos)
        {
            Terrain result = null;

            // I so want to use NonAlloc here but it just doesn't make sense. Sadly I can't
            // allocate the array on the heap w/o unsafe and if I persist the result then the
            // array will still hold pointers to colliders and such which could keep object in
            // ram unnecessarily. Ugh.
            RaycastHit[] tmpHits = Physics.RaycastAll(new Ray(pos, Vector3.down));

            for (int i = 0; i < tmpHits.Length; i++)
            {
                if (tmpHits[i].collider is TerrainCollider tc)
                {
                    if (tc.bounds.Contains(tmpHits[i].point))
                    {
                        result = tc.GetComponent<Terrain>();
                    }
                }
            }

            return result;
        }

        public static float GetTerrainHeight(Vector3 pos)
        {
            float result = 0;

            // I so want to use NonAlloc here but it just doesn't make sense. Sadly I can't
            // allocate the array on the heap w/o unsafe and if I persist the result then the
            // array will still hold pointers to colliders and such which could keep object in
            // ram unnecessarily. Ugh.
            RaycastHit[] tmpHits = Physics.RaycastAll(new Ray(pos, Vector3.down));

            for (int i = 0; i < tmpHits.Length; i++)
            {
                if (tmpHits[i].collider is TerrainCollider)
                {
                    result = tmpHits[i].point.y;
                    break;
                }
            }

            return result;
        }

    }
}
