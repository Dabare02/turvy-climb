using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelProgressManager : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    private Player _player;

    private float levelProgress;

    public UnityEvent<float> onProgressChanged;

    void Awake()
    {
        if (startPoint == null || endPoint == null) Debug.LogError("Please set a start and endpoint for the level in LevelProgressManager.");
        _player = FindObjectOfType<Player>();
        if (_player == null)
        {
            Debug.LogError("No player detected in the scene. Can't calculate progress.");
            gameObject.SetActive(false);
        }

        // Eventos
        if (onProgressChanged == null) onProgressChanged = new UnityEvent<float>();
    }

    void OnEnable()
    {
        LevelManager lvlManager = GetComponent<LevelManager>();
        if (lvlManager != null)
        {
            onProgressChanged.AddListener(lvlManager.UpdateProgress);
        }

        ProgressBar progressBar = FindObjectOfType<ProgressBar>();
        if (progressBar != null)
        {
            onProgressChanged.AddListener(progressBar.SetValue);
        }
    }

    void Update()
    {
        UpdateLevelProgress();
    }

    void OnDisable()
    {
        onProgressChanged.RemoveAllListeners();
    }

    private void UpdateLevelProgress()
    {
        Vector2 playerPos = _player.playerTorso.transform.position - startPoint.position;
        Vector2 endPos = endPoint.position - startPoint.position;

        levelProgress = playerPos.y / endPos.y;
        if (levelProgress < 0f) levelProgress = 0f;
        else if (levelProgress > 1f) levelProgress = 1f;
        onProgressChanged.Invoke(levelProgress);
    }
}
