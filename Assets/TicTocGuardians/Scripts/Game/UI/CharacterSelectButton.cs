using TicTocGuardians.Scripts.Game.Manager;
using TicTocGuardians.Scripts.Game.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TicTocGuardians.Scripts.Game.UI
{
    public class CharacterSelectButton : MonoBehaviour
    {
        [HideInInspector] public Button button;

        [HideInInspector] public PlayerPreview playerPreview;

        public PlayerType type;
        public Image order;
        public Sprite normalSprite;
        public Sprite highlightSprite;
        private Image _image;

        private bool _selected;

        public void Awake()
        {
            _image = GetComponent<Image>();
            button = GetComponent<Button>();
            playerPreview = GetComponentInChildren<PlayerPreview>();
        }

        public void Start()
        {
            button.onClick.AddListener(OnClick);
            order.gameObject.SetActive(false);
        }

        public void AddListener(UnityAction action)
        {
            button.onClick.AddListener(action);
        }

        public void OnClick()
        {
            GlobalSoundManager.Instance.PlaySFX("SFX_PickNumber");
            if (!_selected)
            {
                playerPreview.Pick();
                order.gameObject.SetActive(true);
                _image.sprite = highlightSprite;
            }
            else
            {
                playerPreview.Wait();
                order.gameObject.SetActive(false);
                _image.sprite = normalSprite;
            }
        }

        public void Select()
        {
            _selected = true;
        }

        public void UnSelect()
        {
            _selected = false;
        }

        public bool IsSelected()
        {
            return _selected;
        }

        public void ResetUI()
        {
            playerPreview.Wait();
            order.gameObject.SetActive(false);
            _selected = false;
        }
    }
}