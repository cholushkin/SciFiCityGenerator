using System.Collections;
using GameLib.Random;
using TowerGenerator;
using UnityEngine;

public class RandomizationCoroutine : MonoBehaviour
{
    public int RanomizationIterationCount;
    private IPseudoRandomNumberGenerator _rnd;
    private ChunkControllerBase _chunkController;
    private bool _isCoroutineRunning;

    public void StartRandomization(int seed)
    {
        _rnd = RandomHelper.CreateRandomNumberGenerator(seed);
        _chunkController = GetComponentInChildren<ChunkControllerBase>();
        StopAllCoroutines();
        StartCoroutine(StartRandomizingAnimation());
    }

    public bool IsRunning()
    {
        return _isCoroutineRunning;
    }

    private IEnumerator StartRandomizingAnimation()
    {
        _isCoroutineRunning = true;
        int rndIterationCounter = RanomizationIterationCount;
        float delay = 0.1f;
        const float stepDelta = 0.02f;
        while (rndIterationCounter-- > 0)
        {
            _rnd.Next();
            _chunkController.Seed = _rnd.ValueInt();
            _chunkController.SetConfiguration();
            delay += stepDelta;
            yield return new WaitForSeconds(delay);
        }
        _isCoroutineRunning = false;
        //Debug.Log($"Seed = {_rnd.GetState().AsNumber()} Chunk = {_curMeta.ChunkName}");
    }
}
