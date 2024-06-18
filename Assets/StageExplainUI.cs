using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageExplainUI : MonoBehaviour
{
    public TMP_Text title;
    public Image previewImage;
    public TMP_Text explain;

    public void OnActive()
    {
        gameObject.SetActive(true);

        transform.localScale = new Vector3(0, 0, 0);
        transform.DORewind();
        transform.DOScale(1, 0.2f);
    }

    public void OnInActive()
    {
        transform.DOScale(0, 0.2f).OnComplete(() => { gameObject.SetActive(false); });
    }
}