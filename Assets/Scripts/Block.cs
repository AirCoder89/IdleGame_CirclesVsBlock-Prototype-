using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Block : MonoBehaviour
{
    [SerializeField][Range(0,1)] private float shakeDuration;
    [SerializeField] private Vector2 shakeStrength;
    [SerializeField] private Vector2 scaleShakeStrength;
    
    private GameController _controller;
    private bool _isShaking;
    private Sequence _shakeAnim;
    private Vector3 _startPosition;
    private Vector3 _startScale;
    public void Initialize(GameController controller)
    {
        this._controller = controller;
        _startPosition = transform.position;
        _startScale = transform.localScale;
    }

    private void OnMouseUpAsButton()
    {
        this._controller.OnPlayerAttack();
        Shake();
    }

    private void Shake()
    {
        if(_shakeAnim != null) _shakeAnim.Kill();
        _shakeAnim = DOTween.Sequence();
        transform.position = _startPosition;
        transform.localScale = _startScale;
        _shakeAnim.Append(transform.DOShakePosition(shakeDuration, shakeStrength))
            .Append(transform.DOShakeScale(shakeDuration, scaleShakeStrength)).OnComplete(() =>
        {
            transform.localScale = _startScale;
            transform.position = _startPosition;
        });
        _shakeAnim.Play();
        
    }

    
}
