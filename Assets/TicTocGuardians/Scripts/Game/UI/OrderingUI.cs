using UnityEngine;
using UnityEngine.UI;

namespace TicTocGuardians.Scripts.Game.UI
{
    public class OrderingUI : MonoBehaviour
    {
        public CharacterSelectButton[] selectButtons = new CharacterSelectButton[3];

        public Sprite[] orderSprites = new Sprite[3];
        public Button submitButton;
        public Button cancelButton;
        public Image submitText;

        public void Start()
        {
            ResetUI();
        }

        public void ResetUI()
        {
            foreach (var button in selectButtons) button.ResetUI();
            SubmitUnavilable();
        }

        public void SubmitAvailable()
        {
            submitButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(true);
            submitText.gameObject.SetActive(true);
        }

        public void SubmitUnavilable()
        {
            submitButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            submitText.gameObject.SetActive(false);
        }
    }
}