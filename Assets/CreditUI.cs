using System;
using System.Collections;
using System.Collections.Generic;
using TicTocGuardians.Scripts.Game.Manager;
using UnityEngine;
using UnityEngine.UI;

public class CreditUI : MonoBehaviour
{
    public Sprite[] sprites = new Sprite[3];
    public Image image;
    public int index=0;
    void Start()
    {
        image = GetComponentInChildren<Image>();
    }

    public void NextSprite()
    {
        if (index < sprites.Length-1)
        {
            index++;
            image.sprite= sprites[index];
        }
        else
        {
            gameObject.SetActive(false);
            LobbyManager.Instance.controller.gameObject.SetActive(true);
            index = 0;
            image.sprite = sprites[index];
        }
    }
}
