using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Top Canvas")]
    [SerializeField] private Text goldTxt;
    [SerializeField] private Button pauseBtn;

    [Header("Bottom Canvas")] 
    [SerializeField] private Text levelTxt;
    [SerializeField] private Button upgradeLevelBtn;
    [SerializeField] private Button addNewCircleBtn;
    [SerializeField] private Button upgradeCircleBtn;

    private GameController _parent;

    public void Initialize(GameController parent)
    {
        this._parent = parent;
        
        SetGold(parent.gold);
        SetLevel(parent.currentLevel);
        
        pauseBtn.onClick.AddListener(OnPause);
        upgradeLevelBtn.onClick.AddListener(OnUpgradeLevel);
        addNewCircleBtn.onClick.AddListener(OnAddNewCircle);
        upgradeCircleBtn.onClick.AddListener(OnUpgradeCircle);

        UpdateUpgradeCost();
        UpdateCircleCost();
        UpdateUpgradeCircleCost();
    }

    public void SetUpgradeCircleButtonState(bool status)
    {
        this.upgradeCircleBtn.interactable = status;
    }
    
    public void UpgradeLevelComplete()
    {
        SetGold(_parent.gold);
        SetLevel(_parent.currentLevel);
        UpdateUpgradeCost();
    }

    public void OnAddNewCircleComplete()
    {
        SetGold(_parent.gold);
        UpdateCircleCost();
    }

    public void UpdateGold()
    {
        SetGold(_parent.gold);
    }
    
    private void UpdateUpgradeCost()
    {
        upgradeLevelBtn.GetComponentInChildren<Text>().text = GameExtension.ConvertGold(_parent.GetUpgradeCost());
    }
    private void UpdateCircleCost()
    {
        addNewCircleBtn.GetComponentInChildren<Text>().text = GameExtension.ConvertGold(_parent.GetNextCircleCost);
    }
    public void UpdateUpgradeCircleCost()
    {
        upgradeCircleBtn.GetComponentInChildren<Text>().text = GameExtension.ConvertGold(_parent.GetUpgradeCircleCost);
    }
    private void OnUpgradeCircle()
    {
        _parent.OnUpgradeCircle();
    }

    private void OnAddNewCircle()
    {
        _parent.AddNewCircle();
    }

    private void OnUpgradeLevel()
    {
        _parent.Upgrade();
    }

    private void OnPause()
    {
        
    }

    private void SetGold(ulong value)
    {
        goldTxt.text = GameExtension.ConvertGold(value);
    }
    
    public void AddToGold(ulong total)
    {
        SetGold(total);
    }

    private void SetLevel(int lev)
    {
        this.levelTxt.text = lev >= 10 ? lev.ToString() : "0" + lev;
    }
}
