using UnityEngine;

namespace F1Racer
{
    public class WaypointPath : MonoBehaviour
    {
        public Transform[] waypoints;

        private void Reset()
        {
            waypoints = GetComponentsInChildren<Transform>();
            if (waypoints != null && waypoints.Length > 0)
            {
                var list = new System.Collections.Generic.List<Transform>(waypoints);
                list.RemoveAt(0);
                waypoints = list.ToArray();
            }
        }

        public Transform GetClosestWaypoint(Vector3 position)
        {
            if (waypoints == null || waypoints.Length == 0)
            {
                return null;
            }

            Transform best = waypoints[0];
            float bestDistance = float.MaxValue;

            for (int i = 0; i < waypoints.Length; i++)
            {
                float d = Vector3.Distance(position, waypoints[i].position);
                if (d < bestDistance)
                {
                    bestDistance = d;
                    best = waypoints[i];
                }
            }

            return best;
        }
    }
}
