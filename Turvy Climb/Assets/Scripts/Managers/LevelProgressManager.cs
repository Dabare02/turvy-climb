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

    void Awake()
    {
        if (startPoint == null || endPoint == null) Debug.LogError("Please set a start and endpoint for the level in LevelProgressManager.");
        _player = FindObjectOfType<Player>();
        if (_player == null)
        {
            Debug.LogError("No player detected in the scene. Can't calculate progress.");
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        UpdateLevelProgress();
    }

    private void UpdateLevelProgress()
    {
        Vector2 playerPos = _player.playerTorso.transform.position - startPoint.position;
        Vector2 endPos = endPoint.position - startPoint.position;

        levelProgress = playerPos.y / endPos.y;
        if (levelProgress < 0f) levelProgress = 0f;
        else if (levelProgress > 1f) levelProgress = 1f;
        EventManager.OnLevelProgressChanged(levelProgress);
    }
}
