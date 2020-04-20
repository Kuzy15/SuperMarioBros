using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class whose aim is to create the grid of buttons at SelectLevel, and give them a callback
/// </summary>
public class PopulateGrid : MonoBehaviour
{
    public static PopulateGrid GM;

    //array of GameObjects where buttons crerated will be saved
    private GameObject[] buttonsList;

    //number of buttons to generate
    private int numberOfButtons;

    //prefab from where the button will be created
    public GameObject prefab;

    /// <summary>
    /// This method just creates the "grid" of buttons
    /// </summary>

    /// <summary>
    /// Creates the array of buttons and gives an index and locked to each of them.
    /// If the current button index is lower than the max level of the current category
    /// we set it unlocked, and add to it the callback
    /// </summary>
    /// 
    void Awake()
    {
        if (GM != null)
            GameObject.Destroy(GM);
        else
            GM = this;

        //DontDestroyOnLoad(this);
    }

    public void Populate(int buttons, string[] names)
    {
        buttonsList = new GameObject[buttons];
        for (int i = 0; i < buttons; i++)
        {
            GameObject gObj;
            gObj = Instantiate(prefab, transform);
            gObj.GetComponent<ButtonLevel>().SetButtonIndex(i);
            gObj.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = names[i]; // Child 1 is the STAR image
            buttonsList[i] = gObj;
            int index = gObj.GetComponent<ButtonLevel>().GetIndex();
            buttonsList[i].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => TaskOnClick(index, names[index]));
            /*if (buttonsList[i].GetComponent<ButtonLevel>().GetIndex() <= GameManager.Instance.GetMaxLevel(GameManager.Instance.GetCurrentCategory()))
            {
                buttonsList[i].GetComponent<ButtonLevel>().SetLocked(false);
                buttonsList[i].transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().enabled = true; // Child 1 is the STAR image
                int index = buttonsList[i].GetComponent<ButtonLevel>().GetIndex();
                buttonsList[i].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => TaskOnClick(index));
            }*/
        }
    }

    /// <summary>
    /// Set the current level with the given index
    /// and loads Gameplay scene
    /// </summary>
    /// <param name="buttonIndex">index of the level that will be loaded

    public void TaskOnClick(int index, string item)
    {
        if (!buttonsList[index].GetComponent<ButtonLevel>().GetLocked())
        {
            buttonsList[index].GetComponent<ButtonLevel>().SetLocked(true);
            InputFieldManager.GM.AddFile(item);
        }
        else
        {
            buttonsList[index].GetComponent<ButtonLevel>().SetLocked(false);
            InputFieldManager.GM.RemoveFile(item);
        }
    }
}
