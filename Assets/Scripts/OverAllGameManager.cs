using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class OverAllGameManager : MonoBehaviour
{
    //DoTweening Objects 
    [SerializeField] GameObject BgImage,PlayButtonParent,InstructionsPanel;



    public LevelInfo[] levelinfo;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelDescriptionText;
    public Button StartLevelButton;

    public GameObject GameStartinfoPanel;
    public GameObject ButtonsMainMenus;   //All 5 game buttons
  //  public RectTransform PlaneTransform;


    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }
    private void Start()
    {
      //  BgImage.transform.DOScale(1f, 0.5f).SetEase(Ease.OutQuart);
        GameStartinfoPanel.SetActive(false);
        ButtonsMainMenus.SetActive(true);
        //    ButtonsTransform.transform.DOMoveX(200, 1f).SetEase(Ease.OutQuart);
        //    PlaneTransform.transform.DOLocalMove(new Vector2(0, -389f), 2f).SetEase(Ease.OutQuart);
    }

    public void SceneChanage(int SceneNumber)
    {

        ButtonsMainMenus.SetActive(false);

    //    BgImage.transform.DOScale(1.5f, 0.5f).SetEase(Ease.OutQuart);
    //    InstructionsPanel.transform.DOScale(1, 0.5f).SetEase(Ease.OutQuart);
        PlayButtonParent.transform.DOMoveY(150, 0.5f).SetEase(Ease.OutQuart);
     //   ButtonsTransform.transform.DOMoveX(-200, 0.1f).SetEase(Ease.Linear);
     //   PlaneTransform.transform.DOLocalMove(new Vector2(675, 193), 1f).SetEase(Ease.OutQuart);

        GameStartinfoPanel.SetActive(true);
        levelText.text = levelinfo[SceneNumber - 1].levelTextName;
        string description = levelinfo[SceneNumber - 1].levelTextDescription;
        string formattedText = description.Replace(". ", ".\n");
        levelDescriptionText.text = formattedText;
        StartLevelButton.onClick.AddListener(() => LoadScene(SceneNumber));
    }

    public void LoadScene(int SceneNumber)
    {
        SceneManager.LoadScene(SceneNumber);
    }

    public void BackButton()
    {
        ButtonsMainMenus.SetActive(true);
    //    InstructionsPanel.transform.DOScale(0, 0.5f).SetEase(Ease.OutQuart);
        //this code is implemented on MainMenu button in 2nd Panel 
     //   BgImage.transform.DOScale(1f, 0.5f).SetEase(Ease.OutQuart);
        PlayButtonParent.transform.DOMoveY(-400, 0.5f).SetEase(Ease.OutQuart);
     //   ButtonsTransform.transform.DOMoveX(200, 1f).SetEase(Ease.OutQuart);
     //   PlaneTransform.transform.DOLocalMove(new Vector2(0, -389f), 2f).SetEase(Ease.OutQuart);


    }
}

[System.Serializable]
public struct LevelInfo
{
    public int levelNumber;
    public string levelTextName;
    public string levelTextDescription;
}


