using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GGJ2026.Gameplay.ScriptableObjects
{
    /// <summary>
    /// 面部位置标准数据配置
    /// </summary>
    [CreateAssetMenu(fileName = "New Feature Position Data", menuName = "GGJ2026/Feature Position Data")]
    public class FeaturePositionData : ScriptableObject
    {
        [Header("面部位置")]
        public Vector3 FacePosition;
        [Header("面部检测半径")]
        public float FaceDetectionRadius = 1f;
        [Header("嘴部位置")]
        public Vector3 MouthPosition;
        [Header("嘴部检测半径")]
        public float MouthDetectionRadius = 1f;
        [Header("鼻子位置")]
        public Vector3 NosePosition;
        [Header("鼻子检测半径")]
        public float NoseDetectionRadius = 1f;
        [Header("左眼位置")]
        public Vector3 LeftEyePosition;
        [Header("左眼检测半径")]
        public float LeftEyeDetectionRadius = 1f;
        [Header("右眼位置")]
        public Vector3 RightEyePosition;
        [Header("右眼检测半径")]
        public float RightEyeDetectionRadius = 1f;
        [Header("左耳位置")]
        public Vector3 LeftEarPosition;
        [Header("左耳检测半径")]
        public float LeftEarDetectionRadius = 1f;
        [Header("右耳位置")]
        public Vector3 RightEarPosition;
        [Header("右耳检测半径")]
        public float RightEarDetectionRadius = 1f;
        

        public bool CheckInRange(Vector3 curPos, out PositionType nearestPos)
        {
            var minDis = float.MaxValue;
            nearestPos = PositionType.None;
            var faceDis = Vector3.Distance(curPos, FacePosition);
            var mouthDis = Vector3.Distance(curPos, MouthPosition);
            var noseDis = Vector3.Distance(curPos, NosePosition);
            var leftEyeDis = Vector3.Distance(curPos, LeftEyePosition);
            var rightEyeDis = Vector3.Distance(curPos, RightEyePosition);
            var leftEarDis = Vector3.Distance(curPos, LeftEarPosition);
            var rightEarDis = Vector3.Distance(curPos, RightEarPosition);

            minDis = Math.Min(minDis, faceDis);
            minDis = Math.Min(minDis, noseDis);
            minDis = Math.Min(minDis, mouthDis);
            minDis = Math.Min(minDis, leftEyeDis);
            minDis = Math.Min(minDis, rightEyeDis);
            minDis = Math.Min(minDis, leftEarDis);
            minDis = Math.Min(minDis, rightEarDis);

            nearestPos = minDis == faceDis ? PositionType.Face : nearestPos;
            nearestPos = minDis == noseDis? PositionType.Nose : nearestPos;
            nearestPos = minDis == mouthDis ? PositionType.Mouth : nearestPos;
            nearestPos = minDis == leftEyeDis ? PositionType.LeftEye : nearestPos;
            nearestPos = minDis == rightEyeDis ? PositionType.RightEye : nearestPos;
            nearestPos = minDis == leftEarDis ? PositionType.LeftEar : nearestPos;
            nearestPos = minDis == rightEarDis ? PositionType.RightEar : nearestPos;

            return nearestPos switch
            {
                PositionType.Face => minDis <= FaceDetectionRadius,
                PositionType.Mouth => minDis <= MouthDetectionRadius,
                PositionType.Nose => minDis <= NoseDetectionRadius,
                PositionType.LeftEye => minDis <= LeftEyeDetectionRadius,
                PositionType.RightEye => minDis <= RightEyeDetectionRadius,
                PositionType.LeftEar => minDis <= LeftEarDetectionRadius,
                PositionType.RightEar => minDis <= RightEarDetectionRadius,
                _ => false,
            };
        }

        public Vector3 Convert2Pos(PositionType positionType)
        {
            return positionType switch 
            { 
                PositionType.Face => FacePosition, 
                PositionType.Mouth => MouthPosition, 
                PositionType.Nose => NosePosition, 
                PositionType.LeftEye => LeftEyePosition, 
                PositionType.RightEye => RightEyePosition, 
                PositionType.LeftEar => LeftEarPosition, 
                PositionType.RightEar => RightEarPosition, 
                _ => Vector3.zero 
            };
        }
    }
}
