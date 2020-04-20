﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ButtonLevel class allows us to manage a button of SelectLevel scene, setting it lock or
/// unlocked depending on the levels passed of each category
/// </summary>
public class ButtonLevel : MonoBehaviour
{
    //bool for button (locked or not locked sprite)
    private bool _locked;

    //index of button
    private int _index;

    //Sprite to render depending on button's state
    public Sprite unlockedSprite;
    public Sprite unlockedPressedSprite;
    /// <summary>
    /// Set button to locked
    /// </summary>
    void Start()
    {
        _locked = false;
    }


    /// <summary>
    /// Method to set button locked or unlocked. If the button is unlocked
    /// we change the sprite to the unlocked one, and put below the "star" of the sprite, 
    /// its correspondant level
    /// </summary>
    /// <param name="newValue">value to set the button state
    public void SetLocked(bool newValue)
    {
        _locked = newValue;
        if (_locked)
        {
            this.gameObject.GetComponent<Image>().color = Color.red;
        }
        else
        {
            this.gameObject.GetComponent<Image>().color = Color.white;
        }
    }


    /// <summary>
    /// Getter for button state
    /// </summary>
    public bool GetLocked()
    {
        return _locked;
    }

    /// <summary>
    /// Setter for button index
    /// </summary>
    /// <param name="index">index the button will have
    public void SetButtonIndex(int index)
    {
        _index = index;
    }

    /// <summary>
    /// Getter for button index
    /// </summary>
    public int GetIndex()
    {
        return _index;
    }
}
