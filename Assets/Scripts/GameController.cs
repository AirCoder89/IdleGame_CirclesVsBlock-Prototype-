using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public struct CircleResource
{
    [ColorPalette] public Color color;
    [Range(0,20)] public float twirlSpeed;
    [Range(0,100)] public float twirlAmount;
    public Vector2 offset;
}

[System.Serializable]
public struct CircleSpawnPos
{
    public Vector2 start;
    public Vector2 target;
    public Vector2 worldPos;
}

public class GameController : MonoBehaviour
{
    [TabGroup("Elements")][SerializeField] private Loading loading;
    [TabGroup("Elements")][SerializeField] private UIManager uiManager;
    [TabGroup("Elements")] public Block block;
    [TabGroup("Elements")][SerializeField] private GameObject circlePrefab;
    [TabGroup("Elements")][SerializeField] private Transform circleHolder;
    [TabGroup("Elements")][SerializeField] private Transform UIVFXHolder;
    
    [TabGroup("Initialize")] public float transitionSpeed = 1f;
    [TabGroup("Initialize")] public GameData gameData;
    [TabGroup("Initialize")] public string poolName;
    [TabGroup("Initialize")] public string goldVfxName;
    [TabGroup("Initialize")] public ulong gold = 0;
    [TabGroup("Initialize")] public int currentLevel = 1;
    [TabGroup("Initialize")] public Circle selectedCircle;
    [TabGroup("Initialize")] public float circleAttackTime = 1f;


    [TabGroup("Circles & Block")] public float circleMovementSpeed;
    [TabGroup("Circles & Block")] public List<Material> circlesMaterials;

    [TabGroup("Circles & Block")] [TableList]
    public List<CircleResource> circleRes;
    
    [TabGroup("Circles & Block")] [TableList][SerializeField] private List<CircleSpawnPos> circlePos;
    public int NbCircle
    {
        get
        {
            if (_allCircles == null || _allCircles.Count == 0) return 0;
            return _allCircles.Count;
        }
    }

    [Button("Convert Gold")]
    private void ConvertGold(ulong amount)
    {
        print(GameExtension.ConvertGold(amount));
    }
    
    public ulong GetNextCircleCost
    {
        get
        {
            if (_allCircles == null || _allCircles.Count == 0)
            {
                return (ulong)gameData.firstCircleCost;
            }
            else
            {
                return ((ulong)_lastCircleCost * 100);
            }
        }
    }

    public ulong GetUpgradeCircleCost
    {
        get
        {
            if (selectedCircle == null) return 0;
            return (ulong)(Mathf.Pow((gameData.upgradeCostScaling.value1 * gameData.upgradeCostScaling.value2), selectedCircle.level));
        }
    }

    private ulong _lastCircleCost;
    private List<Circle> _allCircles;
   
    void Start()
    {
        Loading.OnComplete += OnLoadingComplete;
        Loading.OnError += OnLoadingError;
        loading.StartLoading();
    }

    private void OnLoadingComplete(GameData data)
    {
        Loading.OnComplete -= OnLoadingComplete;
        Loading.OnError -= OnLoadingError;
        this.gameData = data;

        TransitionScript.OnComplete += () =>
        {
            TransitionScript.RemoveEvents();
            loading.Hide();
            InitializeGame();
            TransitionScript.Instance.FadeOut(transitionSpeed);
        };
        TransitionScript.Instance.FadeIn(transitionSpeed);
          
    }

    private void OnLoadingError()
    {
        Loading.OnComplete -= OnLoadingComplete;
        Loading.OnError -= OnLoadingError;
        
        //here we will start with default game values!
        TransitionScript.OnComplete += () =>
        {
            TransitionScript.RemoveEvents();
            loading.Hide();
            InitializeGame();
            TransitionScript.Instance.FadeOut(transitionSpeed);
        };
        TransitionScript.Instance.FadeIn(transitionSpeed);
    }


    private void InitializeGame()
    {
        _lastCircleCost = (ulong)gameData.firstCircleCost;
        _allCircles = new List<Circle>();
        uiManager.Initialize(this);
        block.Initialize(this);
        Circle.OnCircleAttack += OnCircleAttack;
    }
    
    private void Update()
    {
        uiManager.SetUpgradeCircleButtonState(selectedCircle != null);
    }

    private void OnDestroy()
    {
        Circle.OnCircleAttack -= OnCircleAttack;
        Loading.OnComplete -= OnLoadingComplete;
        Loading.OnError -= OnLoadingError;
    }

    public void AddNewCircle()
    {
        if (NbCircle >= gameData.maxCircles)
        {
            print("you achieve max circle !");
            return;
        }
        if (gold < GetNextCircleCost)
        {
            print("insufficient gold !");
            return;
        }

        _lastCircleCost =  GetNextCircleCost;
        gold -= _lastCircleCost;

        var circle = Instantiate(circlePrefab, circleHolder).GetComponent<Circle>();
        var randomPosIndex = Random.Range(0, circlePos.Count);
        var randomPos = circlePos[randomPosIndex];
        circle.Initialize(this, this.circleAttackTime,this.circlesMaterials[_allCircles.Count],randomPos);
        circlePos.RemoveAt(randomPosIndex);
        this._allCircles.Add(circle);
        OnSelectCircle(circle);
        uiManager.OnAddNewCircleComplete();
    }

    public void OnSelectCircle(Circle circle)
    {
        if(selectedCircle == circle) return;
        if (selectedCircle != null)
        {
            selectedCircle.Unselect();
        }
        this.selectedCircle = circle;
        this.selectedCircle.Select();
        uiManager.UpdateUpgradeCircleCost();
    }
    
    public void Upgrade()
    {
        if (gold < GetUpgradeCost())
        {
            print("insufficient gold !");
            return;
        }

        gold -= GetUpgradeCost();
        currentLevel++;
        uiManager.UpgradeLevelComplete();
    }
    
    public void OnPlayerAttack()
    {
        var goldPerTap = (ulong)Mathf.Pow((gameData.goldPerTapScaling.value1 * currentLevel), gameData.goldPerTapScaling.value2) ;
        this.gold += goldPerTap;
        uiManager.AddToGold(this.gold);
        var goldVfx = PoolManager.Pools[this.poolName].Spawn(this.goldVfxName, this.UIVFXHolder).gameObject.GetComponent<GoldVFX>();
       
       goldVfx.Play(goldPerTap, block.transform.position, false);
        //goldVfx.Play(goldPerTap, new Vector3(Input.mousePosition.x,Input.mousePosition.y,0), false);
    }

    private void OnCircleAttack(Circle attacker)
    {
        print("Circle Pos: " + attacker.GetWorldPos());
        this.gold += attacker.goldPerAttack;
        uiManager.AddToGold(this.gold);
        var goldVfx = PoolManager.Pools[this.poolName].Spawn(this.goldVfxName, this.UIVFXHolder).gameObject.GetComponent<GoldVFX>();
        goldVfx.Play(attacker.goldPerAttack, attacker.GetWorldPos(), true);
    }

    public void OnUpgradeCircle()
    {
        if (selectedCircle == null)
        {
            Debug.LogError("There is no circle selected !!");
            return;
        }
        if (gold < GetUpgradeCircleCost)
        {
            print("insufficient gold !");
            return;
        }
        
        gold -= GetUpgradeCircleCost;
        
        uiManager.UpdateGold();
        selectedCircle.Upgrade();
        uiManager.UpdateUpgradeCircleCost();
    }
    
    public ulong GetUpgradeCost()
    {
        return (ulong)(Mathf.Pow((gameData.upgradeCostScaling.value1 * gameData.upgradeCostScaling.value2), currentLevel));
    }
}
