using System;
using UnityEngine;
using RoR2;

namespace CorruptsAllVoidStages
{
    public struct SimpleBoxZone : IZone
    {
        public Vector3 cornerA;
        public Vector3 cornerB;

        public bool IsInBounds(Vector3 position) => InRange(cornerA.x, cornerB.x, position.x) 
            && InRange(cornerA.y, cornerB.y, position.y) 
            && InRange(cornerA.z, cornerB.z, position.z);

        private static bool InRange(float a, float b, float value)
        {
            if (a > b)
            {
                return value <= a && value >= b;
            }
            else if (a < b)
            {
                return value >= a && value <= b;
            }
            else
            {
                return value == a;
            }
        }
    }
}