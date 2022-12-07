using System;
using Assets.Plugins.Alg;
using DG.Tweening;
using GameLib.Random;
using TowerGenerator;
using UnityEngine;
using UnityEngine.Assertions;

[ScriptExecutionOrderDependsOn(typeof(MetaProvider))]
public class Showroom2Controller : MonoBehaviour
{
    public MetaProvider MetaProvider;
    public long Seed;
    public bool UseLeveling;
    public float[] Levels;
    public Transform[] Pivots;
    public bool RandomizeOnStart;
    public float OneShowcaseDuration;
    public bool IsDrawDebugInfo;

    private IPseudoRandomNumberGenerator _rnd;
    private const int XMax = 3;
    private const int ZMax = 3;
    private readonly float[] _targetLevels = new float[XMax*ZMax];
    private RandomizationCoroutine[] _randCoroutines;
    private float _duration;


  void Start()
    {
        _randCoroutines = GetComponentsInChildren<RandomizationCoroutine>();
        if (RandomizeOnStart)
        {
            _duration = OneShowcaseDuration;
            Randomize();
        }
    }

    void Update()
    {
        _duration -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Randomize();
        }

        if (_duration < 0f && IsAnimationOfRandomizationFinished())
        {
            Randomize();
            _duration = OneShowcaseDuration;
        }
    }

    public void Randomize()
    {
        // Use previous seed or create new
        _rnd ??= RandomHelper.CreateRandomNumberGenerator(Seed);
        Seed = _rnd.GetState().AsNumber(); // Show current seed to inspector

        int index = 0;
        for (int z = 0; z < ZMax; ++z)
        {
            for (int x = 0; x < XMax; ++x)
            {
                var pivot = Pivots[index++];

                // Delete previous chunk if exists
                if (pivot.childCount != 0)
                    DestroyImmediate(pivot.GetChild(0).gameObject);

                // Create new random chunk from MetaProvider
                var rndMeta = _rnd.FromList(MetaProvider.Metas);
                var rndChunk = ChunkFactory.CreateChunkRnd(rndMeta, _rnd.GetState(), pivot, pivot.position,
                    ChunkFactory.Positioning.ChunkPivot);
                if (IsDrawDebugInfo)
                    rndChunk.AddComponent<ChunkControllerDebug>();

                // Set random level
                _targetLevels[XMax * z + x] = 0f;
                if (UseLeveling)
                {
                    _targetLevels[XMax * z + x] = _rnd.FromArray(Levels);
                    pivot.GetChild(0).DOMove(pivot.position + Vector3.up * _targetLevels[XMax * z + x], 2f)
                        .SetEase(Ease.InOutCubic);
                }
            }
        }

        // Disable pillars
        for (int z = 0; z < ZMax; ++z)
        {
            for (int x = 0; x < XMax; ++x)
            {
                DisablePillars(x, z);
            }
        }

        // Randomize chunk
        foreach (var c in _randCoroutines)
        {
            c.StartRandomization(_rnd.ValueInt());
        }
    }

    private bool IsAnimationOfRandomizationFinished()
    {
        foreach (var c in _randCoroutines)
        {
            if (c.IsRunning())
            {
                return false;
            }
        }
        return true;
    }

    private void DisablePillars(int x, int z)
    {
        var center = Pivots[XMax * z + x].GetChild(0).transform.position;
        var pillars = Pivots[XMax * z + x].GetChild(0).GetComponentsInChildren<Pillar>();
        Assert.IsTrue(pillars.Length == 3*4);

        void DisablePillarIfLower(int neighbourPillarIndex, Pillar pillar)
        {
            var neighborHeight = _targetLevels[neighbourPillarIndex];
            var needDisable = _targetLevels[XMax * z + x] < neighborHeight;

            if (Mathf.Approximately(neighborHeight, _targetLevels[XMax * z + x]))
            {
                needDisable = ((XMax * z + x) % 2 ) == 0;
            }

            if (needDisable)
                pillar.gameObject.SetActive(false);
        }


        foreach (var pillar in pillars)
        {
            var offset = center - pillar.transform.position;

            // left pillar?
            if (Mathf.Approximately(offset.x, 10.0f))
            {
                if (x == 0) // left edge of m33
                    continue;
                // if neighbor chunk on the left side is higher then disable pillar
                DisablePillarIfLower(XMax * z + x - 1, pillar);
            }
            // right pillar?
            else if (Mathf.Approximately(offset.x, -10.0f))
            {
                if (x == XMax - 1) // right edge of m33
                    continue;
                // if neighbor chunk on the right side is higher then disable pillar
                DisablePillarIfLower(XMax * z + x + 1, pillar);
            }
            // top pillar?
            else if (Mathf.Approximately(offset.z, -10.0f))
            {
                if (z == ZMax - 1) // top edge of m33
                    continue;
                // if neighbor chunk on the top side is higher then disable pillar
                DisablePillarIfLower(XMax * (z+1) + x, pillar);
            }
            // bottom pillar?
            else if (Mathf.Approximately(offset.z, 10.0f))
            {
                if (z == 0) // bottom edge of m33
                    continue;
                // if neighbor chunk on the bottom side is higher then disable pillar
                DisablePillarIfLower(XMax * (z - 1) + x, pillar);
            }
        }
    }
}
