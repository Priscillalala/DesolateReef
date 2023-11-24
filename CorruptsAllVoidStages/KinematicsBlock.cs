using System;
using UnityEngine;
using RoR2;

namespace CorruptsAllVoidStages
{
    public struct KinematicsBlock
    {
        public float pos_0;
        public float pos;
        public float vel_0;
        public float vel;
        public float acc;
        public float time;

        public float deltaPos => pos - pos_0;
        public float deltaVel => vel - vel_0;
    }
}