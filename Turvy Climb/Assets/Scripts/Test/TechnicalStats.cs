using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TechnicalStats : MonoBehaviour
{
    public enum DeltaTimeType
    {
        Smooth,
        Unscaled
    }

    private string fps;
    [Tooltip("Unscaled is more accurate, but jumpy, or if your game modifies Time.timeScale. Use Smooth for smoothDeltaTime.")]
    public DeltaTimeType DeltaType = DeltaTimeType.Smooth;

    private Dictionary<int, string> CachedNumberStrings = new();

    private int[] _frameRateSamples;
    private int _cacheNumbersAmount = 300;
    private int _averageFromAmount = 30;
    private int _averageCounter;
    private int _currentAveraged;

    private string sceneLoadTime;
    private string prevScene;
    private string newScene;
    private bool _hasSceneChanged;

    void Awake()
    {
        // Cache strings and create array
        {
            for (int i = 0; i < _cacheNumbersAmount; i++)
            {
                CachedNumberStrings[i] = i.ToString();
            }

            _frameRateSamples = new int[_averageFromAmount];
        }
    }

    void Update()
    {
        // Sample
        {
            var currentFrame = (int)Math.Round(1f / DeltaType switch
            {
                DeltaTimeType.Smooth => Time.smoothDeltaTime,
                DeltaTimeType.Unscaled => Time.unscaledDeltaTime,
                _ => Time.unscaledDeltaTime
            });
            _frameRateSamples[_averageCounter] = currentFrame;
        }

        // Average
        {
            var average = 0f;

            foreach (var frameRate in _frameRateSamples)
            {
                average += frameRate;
            }

            _currentAveraged = (int)Math.Round(average / _averageFromAmount);
            _averageCounter = (_averageCounter + 1) % _averageFromAmount;
        }

        // Assign to UI
        {
            fps = _currentAveraged switch
            {
                var x when x >= 0 && x < _cacheNumbersAmount => CachedNumberStrings[x],
                var x when x >= _cacheNumbersAmount => $"> {_cacheNumbersAmount}",
                var x when x < 0 => "< 0",
                _ => "?"
            };
        }
    }

    public void SceneLoadDone(string prevScene, string newScene, float sceneLoadTime)
    {
        _hasSceneChanged = true;
        this.prevScene = prevScene;
        this.newScene = newScene;
        this.sceneLoadTime = sceneLoadTime.ToString("F10");
    }

    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 400, 80), "Debug info");

        GUI.Label(new Rect(20, 30, 360, 50), "FPS: " + fps);
        if (_hasSceneChanged)
        {
            GUI.Label(new Rect(20, 50, 360, 50), "Load time between scenes "
                + prevScene + " and " + newScene + ": " + sceneLoadTime + " s");
        }
    }
}