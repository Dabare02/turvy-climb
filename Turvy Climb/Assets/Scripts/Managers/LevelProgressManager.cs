using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelProgressManager : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    private float levelProgress;

    public UnityEvent<float> onProgressChanged;

    void Awake()
    {
        if (startPoint == null || endPoint == null) Debug.LogError("Please set a start and endpoint for the level in LevelProgressManager.");
        levelProgress = 0f;

        // Eventos
        if (onProgressChanged == null) onProgressChanged = new UnityEvent<float>();
    }

    void OnEnable()
    {
        LevelManager lvlManager = GetComponent<LevelManager>();
        if (lvlManager != null) {
            onProgressChanged.AddListener(lvlManager.UpdateProgress);
        }
    }

    void Update()
    {
        UpdateLevelProgress();
    }

    private void UpdateLevelProgress()
    {
        Player player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogWarning("No player detected in the scene. Can't calculate progress.");
            gameObject.SetActive(false);
        }

        Vector2 playerPos = player.playerTorso.transform.position - startPoint.position;
        Vector2 endPos = endPoint.position - startPoint.position;

        levelProgress = playerPos.y / endPos.y;
        onProgressChanged.Invoke(levelProgress);
    }
}
