using System;
using UnityEngine;
using RoR2;

namespace CorruptsAllVoidStages
{
    public struct SimpleRadialZone : IZone
    {
        public Vector2 center;
        public RangeFloat height;
        public float radius;

        public bool IsInBounds(Vector3 position)
        {
            return (new Vector2(position.x, position.z) - center).sqrMagnitude <= radius * radius && position.y >= height.min && position.y <= height.max;
        }
    }
}