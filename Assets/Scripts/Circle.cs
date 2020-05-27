using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Circle : MonoBehaviour
{
   public delegate void CircleEvent(Circle circle);
   public static event CircleEvent OnCircleAttack;
   
   [SerializeField] private Electric electric;
   [SerializeField] private float attackDuration = .5f;
   public int level;
   public ulong goldPerAttack = 0;
   [SerializeField] private GameObject selectMark;
   [SerializeField] private TextMesh levelTxt;
   [SerializeField] private MeshRenderer meshRenderer;
   [SerializeField]private string sortingLayerName;
   [SerializeField] private int sortingLayer;
  
   private Transform target;
   private GameController _parent;
   private float _attackTime;
   private Material _material;
   private CircleSpawnPos _position;
   private bool _canAttack;
   [Button("Attack")]
   private void Attack()
   {
      electric.SetVisibility(true);
   }
[Button("Stop Attack")]
   private void StopAttack()
   {
      electric.SetVisibility(false);
   }

   private void Start()
   {
      electric.Initialize(transform, target);
   }

   public Vector2 GetWorldPos()
   {
      return this._position.worldPos;
   }
   
   public void Initialize(GameController parent, float attackTime, Material material, CircleSpawnPos position)
   {
      _canAttack = false;
      this._position = position;
      this._material = material;
      this._material = GetComponent<SpriteRenderer>().material;
      this._attackTime = attackTime;
      this._parent = parent;
      
      selectMark.SetActive(false);
      level = 0;
      Upgrade();
      this.target = this._parent.block.transform;
      electric.Initialize(transform, target);
      UpdateFromResource();
      MoveToPosition();
   }

   private void MoveToPosition()
   {
      var startScale = transform.localScale;
      transform.position = this._position.start;
      transform.localScale = Vector3.zero;
      transform.DOMove(this._position.target, _parent.circleMovementSpeed);
      transform.DOScale(startScale, _parent.circleMovementSpeed).OnComplete(() =>
      {
         _canAttack = true;
         InvokeRepeating("OnAttack",_attackTime, _attackTime);
      });
   }

   private void OnAttack()
   {
      if(!_canAttack) return;
      OnCircleAttack?.Invoke(this);
      this.electric.Show(attackDuration);
   }
   
   private void UpdateFromResource()
   {
      var resource = _parent.circleRes[GetResourceIndex()];
      this._material.SetFloat("_rotationSpeed", GetRandomSpeed(resource.twirlSpeed));
      this._material.SetFloat("_Twirl", resource.twirlAmount);
      this._material.SetColor("_Tint", resource.color);
      this._material.SetVector("_Offset", resource.offset);
      electric.UpdateColor(resource.color);
   }
   
   private void SetLevel(int lev)
   {
      level = lev;
      levelTxt.text = lev.ToString();
      meshRenderer.sortingLayerName = this.sortingLayerName;
      meshRenderer.sortingOrder = this.sortingLayer;
   }

   private void OnMouseUpAsButton()
   {
      _parent.OnSelectCircle(this);
   }

   public void Upgrade()
   {
      SetLevel(level+1);
      goldPerAttack = (ulong)(Mathf.Pow((_parent.gameData.goldPerTapScaling.value1 * this.level), _parent.gameData.goldPerTapScaling.value2));
      UpdateFromResource();
   }
   
   public void Select()
   {
      selectMark.SetActive(true);
   }

   public void Unselect()
   {
      selectMark.SetActive(false);
   }

   private int GetResourceIndex()
   {
      if (level == 0) return level;
      if (level < 7) return level - 1;
      return 6;
   }

   private float GetRandomSpeed(float defaultSpeed)
   {
      return Random.Range((defaultSpeed - 0.5f), (defaultSpeed + 0.5f));
   }
}
