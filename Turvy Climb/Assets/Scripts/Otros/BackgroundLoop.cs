using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    public GameObject[] levels;
    private Vector2 screenBounds;
    private Vector3 lastScreenPos;

    void Awake()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        lastScreenPos = Camera.main.transform.position;
    }

    void Start()
    {
        foreach (GameObject obj in levels)
        {
            LoadChildObjects(obj);
        }
    }

    void LateUpdate()
    {
        foreach (GameObject obj in levels)
        {
            RepositionChildObjects(obj);

            float parallaxSpeed = 1 - Mathf.Clamp(Mathf.Abs(Camera.main.transform.position.z / obj.transform.position.z), 0f, 1f);
            float diff = Camera.main.transform.position.y - lastScreenPos.y;
            obj.transform.Translate(Vector3.up * diff * parallaxSpeed);
        }

        lastScreenPos = Camera.main.transform.position;
    }

    void LoadChildObjects(GameObject obj)
    {
        float objectH = obj.GetComponent<SpriteRenderer>().bounds.size.y;
        int childsNeeded = (int)Mathf.Ceil(screenBounds.y * 2 / objectH);
        GameObject clone = Instantiate(obj);
        for (int i = 0; i <= childsNeeded; i++)
        {
            GameObject c = Instantiate(clone);
            c.transform.SetParent(obj.transform);
            c.transform.position = new Vector3(obj.transform.position.x, objectH * i, obj.transform.position.z);
            c.name = obj.name + i;
        }
        Destroy(clone);
        Destroy(obj.GetComponent<SpriteRenderer>());
    }

    void RepositionChildObjects(GameObject obj)
    {
        Transform[] children = obj.GetComponentsInChildren<Transform>();
        if (children.Length > 1)
        {
            GameObject firstChild = children[1].gameObject;
            GameObject lastChild = children[children.Length - 1].gameObject;
            float halfObjH = lastChild.GetComponent<SpriteRenderer>().bounds.extents.y;

            // Comprobar si la cámara se encuentra mirando fuera del fondo.
            if (Camera.main.transform.position.y + screenBounds.y > lastChild.transform.position.y + halfObjH)
            {
                firstChild.transform.SetAsLastSibling();
                firstChild.transform.position = new Vector3(lastChild.transform.position.x, lastChild.transform.position.y + halfObjH * 2, lastChild.transform.position.z);
            }
            else if (Camera.main.transform.position.y - screenBounds.y < firstChild.transform.position.y - halfObjH)
            {
                lastChild.transform.SetAsFirstSibling();
                lastChild.transform.position = new Vector3(firstChild.transform.position.x, firstChild.transform.position.y - halfObjH * 2, firstChild.transform.position.z);
            }
        }
    }
}
