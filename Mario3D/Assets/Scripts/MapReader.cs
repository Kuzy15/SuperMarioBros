using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

using System.Threading.Tasks;
using IronPython;

public class MapReader : MonoBehaviour
{
    public static MapReader GM;
    private List<string> _stringList;
    private List<string[]> _parsedList;
    private GameObject[] _prefabs;
    private Dictionary<string, GameObject> _tiles;

    //public int mapLevel/* = 1*/;

    void Awake()
    {
        if (GM != null)
            GameObject.Destroy(GM);
        else
            GM = this;

        DontDestroyOnLoad(this);
    }

    public void InitMap(int mapLevel)
    {
        _stringList = new List<string>();
        _parsedList = new List<string[]>();
        _tiles = new Dictionary<string, GameObject>();
        _prefabs = Resources.LoadAll("Tiles").Cast<GameObject>().ToArray();
        //Debug.Log("Ngrams: " + InputFieldManager.GM.GetNGramsInput() + "   NFiles: " + InputFieldManager.GM.GetNFilesInput() + "   Files: " + InputFieldManager.GM.GetFilesToConcatInput().Length);
        ReadTextFile("D:/Curso19-20/TFG/SM-Master/SuperMarioBros/Mario3D/Assets/Scripts/1-" + mapLevel.ToString() + ".csv");
        LoadTiles();
    }

    public void GenerateMap() {

        CreateMap();
    }




    private void ReadTextFile(string path, bool parse = true)
    {
        if(_stringList.Count > 0)
        {
            _stringList.Clear();
        }
        StreamReader map = new StreamReader(path);

        while (!map.EndOfStream)
        {
            string inp_ln = map.ReadLine();

            _stringList.Add(inp_ln);
        }

        map.Close();
        if(parse)
            ParseList();
    }


    private void ParseList()
    {
        for (int i = 0; i < _stringList.Count; i++)
        {
            string[] temp = _stringList[i].Split(',');
            for (int j = 0; j < temp.Length; j++)
            {
                temp[j] = temp[j].Trim();  //removed the blank spaces
            }
            _parsedList.Add(temp);
        }
    }


    private void LoadTiles()
    {
        for (int i = 0; i < _prefabs.Length; i++)
        {
            _tiles.Add(_prefabs[i].name, _prefabs[i]);
        }
    }


    void CreateMap()
    {
        DestroyMap();
        for (int i = 0; i < _parsedList.Count; i++)
        {
            for (int j = 0; j < _parsedList[i].Length; j++)
            {
                if (_parsedList[i][j] != "-1")
                {
                    //Debug.Log(_parsedList[i][j]);
                    GameObject tile = Instantiate(_tiles[_parsedList[i][j]], new Vector3(this.gameObject.transform.position.x + j, this.gameObject.transform.position.y - i, this.gameObject.transform.position.z),
                        this.gameObject.transform.rotation, this.gameObject.transform);

                    if(_parsedList[i][j] == "0" || _parsedList[i][j] == "1" || _parsedList[i][j] == "2" || _parsedList[i][j] == "3" ||
                       _parsedList[i][j] == "24" || _parsedList[i][j] == "30" || _parsedList[i][j] == "33" || _parsedList[i][j] == "66" || _parsedList[i][j] == "67" ||
                       _parsedList[i][j] == "68" || _parsedList[i][j] == "69" || _parsedList[i][j] == "90" || _parsedList[i][j] == "99" ||
                       _parsedList[i][j] == "167" || _parsedList[i][j] == "264" || _parsedList[i][j] == "265" || _parsedList[i][j] == "266" ||
                       _parsedList[i][j] == "267" || _parsedList[i][j] == "268" || _parsedList[i][j] == "269" || _parsedList[i][j] == "270" ||
                       _parsedList[i][j] == "271" || _parsedList[i][j] == "280" || _parsedList[i][j] == "297" || _parsedList[i][j] == "298" ||
                       _parsedList[i][j] == "299" || _parsedList[i][j] == "300" || _parsedList[i][j] == "301" || _parsedList[i][j] == "313" ||
                       _parsedList[i][j] == "795" || _parsedList[i][j] == "796" || _parsedList[i][j] == "797" ||

                       _parsedList[i][j] == "292" /*es del spritesheet de objetos*/)
                    {
                        tile.gameObject.tag = "Solid";
                    }

                    if (_parsedList[i][j] == "24")
                    {
                        tile.AddComponent<MagicBlock>();
                    }
                    else if (_parsedList[i][j] == "313")
                    {
                        tile.GetComponent<BoxCollider>().isTrigger = true;
                    }             
                }
            }
        }
    }

    public void DestroyMap()
    {
        if (this.gameObject != null)
        {
            Destroy(this.gameObject.transform);
        }
    }
}
