using System.Collections;
using DG.Tweening;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class EntityPlace : MonoBehaviour
    {
        public CameraEntShowroomController CameraEntShowroomController;
        public float AppearingEffectDuration;
        public int RanomizationIterationCount;
        private IPseudoRandomNumberGenerator _rnd = RandomHelper.CreateRandomNumberGenerator();
        private GameObject _current;
        private ChunkControllerBase _chunkController;
        private MetaBase _curMeta;

        public void Place(MetaBase metaToPlace)
        {
            _curMeta = metaToPlace;
            // remove _current
            Destroy(_current);

            // spawn new
            _rnd.Next();
            _current = ChunkFactory.CreateChunkRnd(metaToPlace, _rnd.GetState(), transform, transform.position);

            CameraEntShowroomController.FitView(_current, _curMeta);
            _current.transform.DOScale(1.5f, AppearingEffectDuration).SetEase(Ease.OutElastic).From();
            _chunkController = _current.GetComponent<ChunkControllerBase>();

            StopAllCoroutines();
            StartCoroutine(StartRandomizingAnimation());
        }

        private IEnumerator StartRandomizingAnimation()
        {
            int rndIterationCounter = RanomizationIterationCount;
            float delay = 0.1f;
            const float stepDelta = 0.02f;
            while (rndIterationCounter-- >0)
            {
                var comp = _current.GetComponent<ChunkControllerBase>();
                _rnd.Next();
                comp.Seed = _rnd.ValueInt();
                comp.SetConfiguration();
                delay += stepDelta;
                yield return new WaitForSeconds(delay);
            }
            Debug.Log($"Seed = {_rnd.GetState().AsNumber()} Chunk = {_curMeta.ChunkName}");
        }

        void OnDrawGizmos()
        {
            if (_current != null)
            {
                //Gizmos.DrawLine(Vector3.zero, Vector3.zero+Vector3.right*100f);
                var bounds = _chunkController.CalculateCurrentAABB();
                Gizmos.DrawWireCube(
                    bounds.center,
                    bounds.size);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(bounds.center, 0.25f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(transform.position, 0.25f);
            }
        }
    }
}
