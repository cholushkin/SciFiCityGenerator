using UnityEngine;

namespace TowerGenerator
{
    public class CameraEntShowroomController : MonoBehaviour
    {
        public OrtoCameraSizeFitter CamSizeFitter1;
        public OrtoCameraSizeFitter CamSizeFitter2;

        public void FitView(GameObject current, MetaBase meta)
        {
            CamSizeFitter1.DoFit(current, meta);
            CamSizeFitter2.DoFit(current, meta);
        }
    }
}