using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Core
{
    public class PatrolPath : MonoBehaviour
    {
        const float waypointradius = 0.4f;
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWayPointPosition(i), waypointradius);
                Gizmos.DrawLine(GetWayPointPosition(i), GetWayPointPosition(j));
            }
        }

        public int GetNextIndex(int i)
        {
            if(i + 1 == transform.childCount)
            {
                return 0;
            }
            return i + 1;
        }

        public Vector3 GetWayPointPosition(int i)
        {
            return transform.GetChild(i).position;
        }
    }
    
}
