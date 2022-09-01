using Assets.Plugins.Alg;
using DG.Tweening;
using UnityEngine;

namespace TowerGenerator
{
    public class OrtoCameraSizeFitter : MonoBehaviour
    {
        public Camera Camera;
        [Tooltip("Duration of zooming in/ zooming out effect in seconds")]
        public float AdjustDuration;
        public float SizeMultiplier; // Offset

        public void Reset()
        {
            Camera = GetComponent<Camera>();
            AdjustDuration = 1f;
            SizeMultiplier = 1f;
        }

        public void DoFit(GameObject gameObj)
        {
            var bbs = gameObj.BoundBox().size;
            var maxDimension = Mathf.Max(Mathf.Max(bbs.x, bbs.y), bbs.z);
            Camera.DOOrthoSize(maxDimension * SizeMultiplier, AdjustDuration);
        }
    }
}