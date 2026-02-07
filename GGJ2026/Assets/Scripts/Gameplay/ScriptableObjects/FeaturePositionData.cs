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
        

        public PositionType GetCurPositionType(FeatureType featureType, Vector3 curPos)
        {
            var minDis = float.MaxValue;
            var result = PositionType.None;
            var faceDis = Vector3.Distance(curPos, FacePosition);
            var mouthDis = Vector3.Distance(curPos, MouthPosition);
            var noseDis = Vector3.Distance(curPos, NosePosition);
            var leftEyeDis = Vector3.Distance(curPos, LeftEyePosition);
            var rightEyeDis = Vector3.Distance(curPos, RightEyePosition);
            var leftEarDis = Vector3.Distance(curPos, LeftEarPosition);
            var rightEarDis = Vector3.Distance(curPos, RightEarPosition);

            if(featureType != FeatureType.Face)
            {
                minDis = Math.Min(minDis, noseDis);
                minDis = Math.Min(minDis, mouthDis);
                minDis = Math.Min(minDis, leftEyeDis);
                minDis = Math.Min(minDis, rightEyeDis);
                minDis = Math.Min(minDis, leftEarDis);
                minDis = Math.Min(minDis, rightEarDis);

                result = minDis == noseDis && noseDis <= NoseDetectionRadius ? PositionType.Nose : result;
                result = minDis == mouthDis && mouthDis <= MouthDetectionRadius ? PositionType.Mouth : result;
                result = minDis == leftEyeDis && leftEyeDis <= LeftEyeDetectionRadius ? PositionType.LeftEye : result;
                result = minDis == rightEyeDis && rightEyeDis <= RightEyeDetectionRadius ? PositionType.RightEye : result;
                result = minDis == leftEarDis && leftEarDis <= LeftEarDetectionRadius ? PositionType.LeftEar : result;
                result = minDis == rightEarDis && rightEarDis <= RightEarDetectionRadius ? PositionType.RightEar : result;

                return result;
            }
            else
            {
                return faceDis <= FaceDetectionRadius ? PositionType.Face : result;
            }
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
