using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class that generates the content of an scroll view
/// </summary>
public class PopulateGrid : MonoBehaviour
{
    //Instance of PopulateGrid
    public static PopulateGrid GM;
    //prefab from where the button will be created
    public GameObject prefab;

    //array of GameObjects where buttons crerated will be saved
    private GameObject[] buttonsList;


    void Awake()
    {
        if (GM != null)
            GameObject.Destroy(GM);
        else
            GM = this;

        //DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Creates the content of the scroll view
    /// </summary>
    /// <param name="buttons">Number of buttons to generate (number of .csv files)</param>
    /// <param name="names">Name of the .csv files</param>
    public void Populate(int buttons, string[] names)
    {
        buttonsList = new GameObject[buttons];
        for (int i = 0; i < buttons; i++)
        {
            GameObject gObj;
            gObj = Instantiate(prefab, transform);
            gObj.GetComponent<ButtonLevel>().SetButtonIndex(i);
            gObj.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = names[i];
            buttonsList[i] = gObj;
            int index = gObj.GetComponent<ButtonLevel>().GetIndex();
            buttonsList[i].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => TaskOnClick(index, names[index]));   
        }
    }

    /// <summary>
    /// Listener for each scroll view button (Adding or removing file)
    /// </summary>
    /// <param name="index">Index of the clicked button</param>
    /// <param name="item">File name to add or remove</param>
    public void TaskOnClick(int index, string item)
    {
        if (!buttonsList[index].GetComponent<ButtonLevel>().GetLocked())
        {
            if (!InputFieldManager.GM.GetGenerationMode() && InputFieldManager.GM.GetFilesSelectedLength() < 1)
            {
                buttonsList[index].GetComponent<ButtonLevel>().SetLocked(true);
                InputFieldManager.GM.AddFile(item);
            }
            else if(InputFieldManager.GM.GetGenerationMode())
            {
                buttonsList[index].GetComponent<ButtonLevel>().SetLocked(true);
                InputFieldManager.GM.AddFile(item);
            }
        }
        else
        {
                buttonsList[index].GetComponent<ButtonLevel>().SetLocked(false);
                InputFieldManager.GM.RemoveFile(item);
        }
    }
}
