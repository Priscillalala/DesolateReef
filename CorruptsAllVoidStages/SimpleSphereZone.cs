using System;
using UnityEngine;
using RoR2;

namespace CorruptsAllVoidStages
{
    public struct SimpleSphereZone : IZone
    {
        public Vector3 center;
        public float radius;

        public bool IsInBounds(Vector3 position) => (position - center).sqrMagnitude <= radius * radius;
    }
}