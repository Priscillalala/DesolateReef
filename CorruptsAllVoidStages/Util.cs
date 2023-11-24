using System;
using UnityEngine;
using BepInEx;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using RoR2;
using HG;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CorruptsAllVoidStages
{
    public static class Util
    {
        public static bool TryFind(this Transform transform, string n, out Transform child)
        {
            return child = transform.Find(n);
        }

        public static IEnumerable<Transform> AllChildren(this Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                yield return transform.GetChild(i);
            }
        }

        public static TaskAwaiter<TObject> GetAwaiter<TObject>(this AsyncOperationHandle<TObject> handle)
        {
            return handle.Task.GetAwaiter();
        }

        [ConCommand(commandName = "calculate_jump_velocity")]
        public static void CCCalculateJumpVelocity(ConCommandArgs args)
        {
            Vector3 startPosition = new Vector3(args.GetArgFloat(0), args.GetArgFloat(1), args.GetArgFloat(2));
            Vector3 endPosition = new Vector3(args.GetArgFloat(3), args.GetArgFloat(4), args.GetArgFloat(5));
            float targetHeight = args.GetArgFloat(6);
            CalculateJumpVelocity(startPosition, endPosition, targetHeight, out Vector3 jumpVelocity, out float time);
            args.Log(jumpVelocity.ToString());
            args.Log(time.ToString());
        }

        public static void CalculateJumpVelocity(Vector3 startPosition, Vector3 endPosition, float targetHeight, out Vector3 jumpVelocity, out float time)
        {
            float a = endPosition.x - startPosition.x;
            Debug.Log("a: " + a);
            float b = endPosition.z - startPosition.z;
            Debug.Log("b: " + b);
            float displacement_x = Mathf.Sqrt(a * a + b * b);
            Debug.Log("displacement_x: " + displacement_x);
            KinematicsBlock startToPeak_y = new KinematicsBlock
            {
                pos_0 = startPosition.y,
                pos = targetHeight,
                vel = 0,
                acc = Physics.gravity.y,
            };
            startToPeak_y.vel_0 = Mathf.Sqrt(startToPeak_y.vel * startToPeak_y.vel - 2 * startToPeak_y.acc * startToPeak_y.deltaPos);
            Debug.Log("startToPeak_y.vel_0: " + startToPeak_y.vel_0);
            KinematicsBlock startToEnd_y = new KinematicsBlock
            {
                pos_0 = startPosition.y,
                pos = endPosition.y,
                vel_0 = startToPeak_y.vel_0,
                acc = Physics.gravity.y,
            };
            startToEnd_y.vel = -Mathf.Sqrt(startToEnd_y.vel_0 * startToEnd_y.vel_0 + 2 * startToEnd_y.acc * startToEnd_y.deltaPos);
            Debug.Log("startToEnd_y.vel: " + startToEnd_y.vel);
            Debug.Log("startToEnd_y.deltaVel: " + startToEnd_y.deltaVel);
            startToEnd_y.time = startToEnd_y.deltaVel / startToEnd_y.acc;
            Debug.Log("startToEnd_y.time: " + startToEnd_y.time);
            KinematicsBlock startToEnd_x = new KinematicsBlock
            {
                pos_0 = 0,
                pos = displacement_x,
                acc = 0,
                time = startToEnd_y.time,
            };
            startToEnd_x.vel = startToEnd_x.vel_0 = startToEnd_x.deltaPos / startToEnd_x.time;
            Debug.Log("startToEnd_x.vel: " + startToEnd_x.vel);
            float scalar_x = startToEnd_x.vel_0 / displacement_x;
            Debug.Log("scalar_x: " + scalar_x);
            jumpVelocity = new Vector3(scalar_x * a, startToEnd_y.vel_0, scalar_x * b);
            time = startToEnd_x.time;
        }
    }
}