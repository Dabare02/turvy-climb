using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class BackgroundLoop : MonoBehaviour
{
    // Indica la cantidad de veces se repite el elemento de fondo, SIN INCLUIR la copia original.
    private const int VERTICAL_CLONES = 1;
    private const int HORIZONTAL_CLONES = 1;

    public EnvironmentElement[] layers;
    private Vector2 screenBounds;
    private Vector3 lastScreenPos;

    void Awake()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        lastScreenPos = Camera.main.transform.position;
    }

    void Start()
    {
        foreach (EnvironmentElement env in layers)
        {
            env.Init();
            LoadChildObjects(env);
        }
    }

    void LateUpdate()
    {
        foreach (EnvironmentElement env in layers)
        {
            RepositionChildObjects(env);
            HorizontalScroll(env);

            float parallaxSpeed = 1 - Mathf.Clamp(Mathf.Abs(Camera.main.transform.position.z / env.envObject.transform.position.z), 0f, 1f);
            float diff = Camera.main.transform.position.y - lastScreenPos.y;
            env.envObject.transform.Translate(Vector3.up * diff * parallaxSpeed);
        }

        lastScreenPos = Camera.main.transform.position;
    }

    private void LoadChildObjects(EnvironmentElement env)
    {
        // Realizarlo de esta forma tiene varias ventajas:
        //  1. Se puede posicionar el fondo donde se quiera en el editor, y el código
        //     respetará dicha posición mientras realiza su función.
        //  2. Haciendo que solo los hijos tengan sprites renderer permite una mejor gestión
        //     de las copias del fondo, especialmente para cosas como parallax vertical.

        // Calcular cantidad de copias a realizar
        float objectH = env.envSpriteBounds.size.y;
        float objectW = env.envSpriteBounds.size.x;

        // Usar clon del GameObject para instanciar para evitar duplicación de hijos.
        GameObject clone = Instantiate(env.envObject);

        // Realizar copias
        for (int i = 0; i <= VERTICAL_CLONES; i++)
        {
            GameObject o = new GameObject(env.envObject.name + i);
            o.transform.SetParent(env.envObject.transform);
            o.transform.position = new Vector3(env.envObject.transform.position.x, objectH * i, env.envObject.transform.position.z);
            
            for (int j = 0; j <= HORIZONTAL_CLONES; j++)
            {
                GameObject c = Instantiate(clone);
                c.transform.SetParent(o.transform);
                c.transform.position = new Vector3(objectW * j, o.transform.position.y, o.transform.position.z);
                c.name = o.name + "-" + j;
            }
        }

        // Eliminar sprite renderer del padre, pues los hijos actuarán como fondo.
        Destroy(clone);
        Destroy(env.envObject.GetComponent<SpriteRenderer>());
    }

    // Coloca la primera copia por detrás de la última, o la última copia por delante de la
    // primera, para conseguir el efecto de fondo que se repite.
    private void RepositionChildObjects(EnvironmentElement env)
    {
        // Obtener copias del fondo.
        Transform[] children = Utilities.GetComponetsInDirectChildren<Transform>(env.envObject);

        if (children.Length > 1)
        {
            GameObject firstChild = children[0].gameObject;
            GameObject lastChild = children[children.Length - 1].gameObject;

            float halfObjH = env.envSpriteBounds.extents.y;

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

    private void HorizontalScroll(EnvironmentElement env)
    {
        GameObject obj = env.envObject;
        Bounds bounds = env.envSpriteBounds;

        obj.transform.Translate(Vector3.right * env.horizontalScrollSpeed * Time.deltaTime);

        if (obj.transform.position.x > 0f)
        {
            obj.transform.position = new Vector3(-bounds.size.x * HORIZONTAL_CLONES,  obj.transform.position.y, obj.transform.position.z);
        }
    }
}

[Serializable]
public class EnvironmentElement
{
    public GameObject envObject;
    public float horizontalScrollSpeed;
    public Bounds envSpriteBounds
    {
        get; private set;
    }

    public void Init()
    {
        if (envObject != null)
        {
            envSpriteBounds = envObject.GetComponent<SpriteRenderer>().bounds;
        }
    }
}