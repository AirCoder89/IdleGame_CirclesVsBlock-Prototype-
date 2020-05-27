using UnityEngine;
using System.Collections;

public class Electric : MonoBehaviour
{
    private LineRenderer lRend;
    public Transform transformPointA;
    public Transform transformPointB;
    
    private readonly int pointsCount = 5;
    private readonly int half = 2;
    private float randomness;
    private Vector3[] points;

    private readonly int pointIndexA = 0;
    private readonly int pointIndexB = 1;
    private readonly int pointIndexC = 2;
    private readonly int pointIndexD = 3;
    private readonly int pointIndexE = 4;

    private readonly string mainTexture = "_MainTex";
    private Vector2 mainTextureScale = Vector2.one;
    private Vector2 mainTextureOffset = Vector2.one;

    private float timer;
    private float timerTimeOut = 0.05f;
    private bool _isVisible;

    public void Initialize(Transform parent,Transform target)
    {
        _isVisible = false;
       
        transformPointA = parent;
        transformPointB = target;
        lRend = GetComponent<LineRenderer>();
        points = new Vector3[pointsCount];
        lRend.positionCount = pointsCount;
        SetVisibility(_isVisible);
    }

    public void UpdateColor(Color color)
    {
        if(lRend == null)  lRend = GetComponent<LineRenderer>();
        lRend.startColor = color;
        lRend.endColor = color;
    }
    public void Show(float showTime)
    {
        SetVisibility(true);
        StopAllCoroutines();
        StartCoroutine(WaitAndHide(showTime));
    }

    IEnumerator WaitAndHide(float t)
    {
        yield return new WaitForSeconds(t);
        SetVisibility(false);
    }
    public void SetVisibility(bool status)
    {
        _isVisible = status;
        lRend.enabled = _isVisible;
    }
    
    private void Update()
    {
        if(transformPointA == null || transformPointB == null || !_isVisible) return;
        CalculatePoints();
    }

    private void CalculatePoints()
    {
        timer += Time.deltaTime;

        if (timer > timerTimeOut)
        {
            timer = 0;

            points[pointIndexA] = transformPointA.position;
            points[pointIndexE] = transformPointB.position;
            points[pointIndexC] = GetCenter(points[pointIndexA], points[pointIndexE]);
            points[pointIndexB] = GetCenter(points[pointIndexA], points[pointIndexC]);
            points[pointIndexD] = GetCenter(points[pointIndexC], points[pointIndexE]);

            float distance = Vector3.Distance(transformPointA.position, transformPointB.position) / points.Length;
            mainTextureScale.x = distance;
            mainTextureOffset.x = Random.Range(-randomness, randomness);
            lRend.material.SetTextureScale(mainTexture, mainTextureScale);
            lRend.material.SetTextureOffset(mainTexture, mainTextureOffset);

            randomness = distance / (pointsCount * half);

            SetRandomness();

            lRend.SetPositions(points);
        }
    }

    private void SetRandomness()
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (i != pointIndexA && i != pointIndexE)
            {
                points[i].x += Random.Range(-randomness, randomness);
                points[i].y += Random.Range(-randomness, randomness);
                points[i].z += Random.Range(-randomness, randomness);
            }
        }
    }

    private Vector3 GetCenter(Vector3 a, Vector3 b)
    {
        return (a + b) / half;
    }
}
