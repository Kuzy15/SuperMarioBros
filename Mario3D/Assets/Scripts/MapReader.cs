using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;


public class MapReader : MonoBehaviour
{
    public static MapReader GM;

    private List<string> _stringList;
    private List<string[]> _parsedList;
    private GameObject[] _prefabs;
    private Dictionary<string, GameObject> _tiles;
    private List<PipeCoords> _pipes;

    private struct PipeCoords {

        public int x, y;
    }

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
        _pipes = new List<PipeCoords>();
        _prefabs = Resources.LoadAll("Tiles").Cast<GameObject>().ToArray();

        ReadTextFile(Application.dataPath + "/Resources/Maps/1-" + mapLevel.ToString() + ".csv");
        LoadTiles();
    }

    public void GenerateMap()
    {
        CreateMap();
    }

    private void ReadTextFile(string path, bool parse = true)
    {
        if (_stringList.Count > 0)
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
        if (parse)
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
                if (_parsedList[i][j] != "-1" && _parsedList[i][j] != null)
                {
                    //Debug.Log(_parsedList[i][j]);
                    GameObject tile = Instantiate(_tiles[_parsedList[i][j]], new Vector3(this.gameObject.transform.position.x + j, this.gameObject.transform.position.y - i, this.gameObject.transform.position.z),
                        this.gameObject.transform.rotation, this.gameObject.transform);
                  

                    if (_parsedList[i][j] == "24")
                    {
                        tile.AddComponent<MagicBlock>();
                    }
                    else if (_parsedList[i][j] == "313")
                    {
                        tile.GetComponent<BoxCollider>().isTrigger = true;
                    }
                    else if (_parsedList[i][j] == "264")
                    {
                        PipeCoords coords;
                        coords.x = i;
                        coords.y = j;
                        _pipes.Add(coords);
                    }
                }
            }
        }
        //Debug.Log("FINISH");
        CheckPipes();
    }

    public void DestroyMap()
    {

            Transform[] tilesToDelete = gameObject.GetComponentsInChildren<Transform>();
            for (int i = 1; i < tilesToDelete.Length; i++)
            {
                Destroy(tilesToDelete[i].gameObject);
            }
    }

    public void CheckPipes()
    {
        Debug.Log("CHECK PIPES");
        for (int i = 0; i < _pipes.Count; i++)
        {
            int j = _pipes[i].x;
            while (_parsedList[j][_pipes[i].y] != "0")
            {
                j++;
            }

            if (_parsedList[j][_pipes[i].y] == "0")
            {
                j += 13;
                if (_parsedList[j][_pipes[i].y - 2] == "266")
                {
                    if(i > 0)
                    {
                        Debug.Log("1: " + _parsedList[_pipes[i - 1].x][_pipes[i - 1].y] + "   2: " + _parsedList[_pipes[i - 1].x][_pipes[i - 1].y + 1]);
                        //_parsedList[_pipes[i-1].x][_pipes[i - 1].y]  addcomponent(entrada zona secreta)
                        //_parsedList[_pipes[i - 1].x][_pipes[i - 1].y]  addcomponent(entrada zona secreta)
                    }
                    // _parsedList[j ][_pipes[i].y - 2]. addcomponent(salida zona secreta)
                   // Debug.Log("PIPE FOUND!");
                }
            }
        }
    }
}
