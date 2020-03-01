using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class MapReader : MonoBehaviour
{

    private List<string> _stringList;
    private List<string[]> _parsedList;
    private GameObject[] _prefabs;
    private Dictionary<string, GameObject> _tiles;

    // Start is called before the first frame update
    void Start()
    {
        _stringList = new List<string>();
        _parsedList = new List<string[]>();
        _tiles = new Dictionary<string, GameObject>();

        _prefabs = Resources.LoadAll("Tiles").Cast<GameObject>().ToArray();

        ReadTextFile("Assets/Resources/Maps/1-1.csv");
        LoadTiles();

        CreateMap();
    }


    private void ReadTextFile(string path)
    {
        StreamReader map = new StreamReader(path);

        while (!map.EndOfStream)
        {
            string inp_ln = map.ReadLine();

            _stringList.Add(inp_ln);
        }

        map.Close();

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
        for (int i = 0; i < _parsedList.Count; i++)
        {
            for (int j = 0; j < _parsedList[i].Length; j++)
            {
                if (_parsedList[i][j] != "-1")
                {
                    GameObject tile = Instantiate(_tiles[_parsedList[i][j]], new Vector3(this.gameObject.transform.position.x + j, this.gameObject.transform.position.y - i, this.gameObject.transform.position.z),
                        this.gameObject.transform.rotation, this.gameObject.transform);

                    if(_parsedList[i][j] == "0" || _parsedList[i][j] == "1" || _parsedList[i][j] == "2" || _parsedList[i][j] == "3" ||
                       _parsedList[i][j] == "24" || _parsedList[i][j] == "33" || _parsedList[i][j] == "66" || _parsedList[i][j] == "68" ||
                       _parsedList[i][j] == "264" || _parsedList[i][j] == "265" || _parsedList[i][j] == "266" || _parsedList[i][j] == "267" ||
                       _parsedList[i][j] == "268" || _parsedList[i][j] == "280" || _parsedList[i][j] == "297" || _parsedList[i][j] == "298" ||
                       _parsedList[i][j] == "299" || _parsedList[i][j] == "300" || _parsedList[i][j] == "301" || _parsedList[i][j] == "313")
                    {
                        tile.gameObject.tag = "Solid";
                    }
                }
            }
        }
    }
}
