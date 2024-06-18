using DG.Tweening;
using TicTocGuardians.Scripts.Game.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SynopsisUI : MonoBehaviour
{
    public Image image;
    public Image description;
    public Button previousButton;
    public TMP_Text page;
    public Button nextButton;

    public Sprite[] imageSprites = new Sprite[4];
    public Sprite[] descriptionSprites = new Sprite[4];
    public int currentIndex;

    public void Start()
    {
        previousButton.onClick.AddListener(PreviousScene);
        nextButton.onClick.AddListener(NextScene);
    }

    public void NextScene()
    {
        if (currentIndex < 3)
        {
            currentIndex++;
            image.sprite = imageSprites[currentIndex];
            description.sprite = descriptionSprites[currentIndex];
            description.SetNativeSize();
            page.SetText((currentIndex + 1) + "/4");
            if (currentIndex == 3)
                transform.DOScale(0, 1f).SetDelay(2.0f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    LobbyManager.Instance.PlayerActive();
                });
        }
    }

    public void PreviousScene()
    {
        if (currentIndex > 1)
        {
            currentIndex--;
            image.sprite = imageSprites[currentIndex];
            description.sprite = descriptionSprites[currentIndex];
            description.SetNativeSize();
            page.SetText((currentIndex + 1) + "/4");
        }
    }
}