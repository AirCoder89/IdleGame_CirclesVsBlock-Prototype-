using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TransitionScript : MonoBehaviour
{
    public static TransitionScript Instance;
    public delegate void TransitionEvents();
    public static event TransitionEvents OnComplete;

    public float duration = 2f;
    public bool fadeOpacity;
    
    private Material _material;
    private Image _image;
    private float _transitionAmount;
    
    private void Awake()
    {
        if(Instance != null)return;
        Instance = this;
        _image = GetComponent<Image>();
        _material = _image.material;
    }

    public static void RemoveEvents()
    {
        OnComplete = null;
    }
    public void FadeIn(float time = -1f)
    {
        var t = time > -1 ? time : duration;
        StopAllCoroutines();
        _transitionAmount = 0f;
        if (fadeOpacity)
        {
            _image.DOFade(0, 0);
            _image.DOFade(1, t/2f);
        }
        StartCoroutine(LerpTransition(1f,t));
    }

    public void FadeOut(float time = -1f)
    {
        var t = time > -1 ? time : duration;
        StopAllCoroutines();
        _transitionAmount = 1f;
        if (fadeOpacity)
        {
            _image.DOFade(1, 0);
            _image.DOFade(0, t/2f);
        }
        StartCoroutine(LerpTransition(0f,t));
    }

    private IEnumerator LerpTransition(float to,float t)
    {
        var startTime = Time.time;
        while (Time.time < (startTime + t))
        {
            var smooth = (Time.time - startTime) / t;
            _transitionAmount = Mathf.Clamp01(Mathf.Lerp(_transitionAmount, to, smooth));
            UpdateShader();
            yield return null;
        }

        _transitionAmount = to;
        UpdateShader();
        if (OnComplete != null) OnComplete();
        yield return null;
    }

    private void UpdateShader()
    {
        if(_material) _material.SetFloat("_transitionAmount",_transitionAmount);
    }
}
