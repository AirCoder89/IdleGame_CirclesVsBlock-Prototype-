using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PathologicalGames;
using UnityEngine;
using UnityEngine.UI;

public class GoldVFX : MonoBehaviour
{
    [SerializeField] private string poolName;
    [SerializeField] private float upAmount;
    [SerializeField] private Text goldTxt;

    [Header("Animation Settings")]
    [SerializeField] Vector3 targetPos = Vector3.zero;
    [SerializeField] private Vector2 startScale = Vector2.one;
    [SerializeField] private Vector2 scaleRange;
    [SerializeField] private Vector2 shakeStrength;
    [SerializeField] private float duration;
    [SerializeField] private Ease ease;
    
    
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Vector2 _startScale;
    
    private RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }
    
    private CanvasGroup canvasGroup
    {
        get
        {
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
            return _canvasGroup;
        }
    }

    public void Play(ulong amount, Vector3 position, bool isCircle)
    {
        var finalDuration = isCircle ? duration * 1.5f : duration;
        var durationSection = finalDuration / 4;
        transform.localScale = startScale;
        rectTransform.anchoredPosition = position;
        canvasGroup.alpha = 0f;
        goldTxt.text = GameExtension.ConvertGold(amount);
        
        
        var targetScale = startScale + scaleRange;
        transform.DOScale(targetScale, finalDuration).SetEase(ease).OnComplete(Remove);
        transform.DOShakeScale(finalDuration, shakeStrength);

        canvasGroup.DOFade(1, durationSection);
        canvasGroup.DOFade(0, durationSection).SetDelay(durationSection * 3);

        if (targetPos != Vector3.zero)
        {
            rectTransform.DOAnchorPos(targetPos, finalDuration).SetEase(ease);
        }
        else
        {
            var targetYPos = transform.position.y + upAmount;
            rectTransform.DOAnchorPosY(targetYPos, finalDuration).SetEase(ease);
        }
    }
   
    private void Remove()
    {
        PoolManager.Pools[poolName].Despawn(this.transform);
    }
}
