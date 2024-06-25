using System;
using UnityEngine;

namespace Utils
{
    public static class GizmosExtensions
    {
        public static void DrawWireArc(Vector3 position, Vector3 dir, float anglesRange, float radius, float maxSteps = 20)
        {
            // Get Angles From Dir
            var forwardLimitPos = position + dir;
            var srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);
            
            var initialPos = position;
            var posA = initialPos;
            var stepAngles = anglesRange / maxSteps;
            var angle = srcAngles - anglesRange / 2;

            float rad = 0f;
            Vector3 posB;
            
            for (var i = 0; i <= maxSteps; i++)
            {
                rad = Mathf.Deg2Rad * angle;
                posB = initialPos;
                posB += new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));

                Gizmos.DrawLine(posA, posB);

                angle += stepAngles;
                posA = posB;
            }
            
            Gizmos.DrawLine(posA, initialPos);
        }
    }
}