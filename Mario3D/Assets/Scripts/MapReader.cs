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
    private List<Transform> _secretZonePos;
    private List<GameObject> _exitSecretZones;
    private List<GameObject> _platforms;
    private Vector3 _marioPosition;
    private bool _notSet = false;
    private int _platformIndex = -1;

    private struct PipeCoords
    {

        public int x, y;

        public GameObject tileL, tileR;

        public void SetTileL(GameObject tile) { tileL = tile; }
        public void SetTileR(GameObject tile) { tileR = tile; }

    }

    private struct PlatformSt
    {
        public GameObject tileL, tileR, tileM;

        public void SetTileL(GameObject tile) { tileL = tile; }
        public void SetTileR(GameObject tile) { tileR = tile; }
        public void SetTileM(GameObject tile) { tileM = tile; }

    }

    void Awake()
    {
        if (GM != null)
            GameObject.Destroy(GM);
        else
            GM = this;

        DontDestroyOnLoad(this);
    }

    public void InitMap(string mapLevel, bool generationMode = true)
    {
        _stringList = new List<string>();
        _parsedList = new List<string[]>();
        _secretZonePos = new List<Transform>();
        _exitSecretZones = new List<GameObject>();
        _tiles = new Dictionary<string, GameObject>();
        _pipes = new List<PipeCoords>();
        _prefabs = Resources.LoadAll("Tiles").Cast<GameObject>().ToArray();
        string fileToRead = "";
        if (generationMode)
        {
            fileToRead = "1-" + mapLevel;
        }
        else
        {
            fileToRead = mapLevel;
        }

        ReadTextFile(Application.dataPath + "/Resources/Maps/" + fileToRead + ".csv");
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
        int pipeIdx = 0;

        DestroyMap();
        int SIZEX = _parsedList.Count;
        int SIZEY = _parsedList[0].Length;

        for (int i = 0; i < SIZEY; i++)
        {
            for (int j = 0; j < SIZEX; j++)
            {
                Debug.Log(_parsedList[j][i]);
                if (_parsedList[j][i] != "-1" && _parsedList[j][i] != null)
                {
                    GameObject tile = null;
                    if (_parsedList[j][i] != "265")
                    {
                        tile = Instantiate(_tiles[_parsedList[j][i]], new Vector3(this.gameObject.transform.position.x + i, this.gameObject.transform.position.y - j, this.gameObject.transform.position.z),
                       this.gameObject.transform.rotation, this.gameObject.transform);
                    }

                    if (_parsedList[j][i] == "24")
                    {
                        tile.AddComponent<MagicBlock>();
                    }
                    else if (_parsedList[j][i] == "313")
                    {
                        tile.GetComponent<BoxCollider>().isTrigger = true;
                    }
                    else if (_parsedList[j][i] == "123" || _parsedList[j][i] == "57")
                    {
                        tile.GetComponent<Coin>().SetMove(false);
                    }
                    else if (_parsedList[j][i] == "264")
                    {
                        if (_parsedList[j][i + 1] != null && _parsedList[j][i + 1] == "265")
                        {
                            PipeCoords coords;
                            coords.x = j;
                            coords.y = i;
                            coords.tileL = tile;
                            coords.tileR = null;
                            GameObject tileR = Instantiate(_tiles[_parsedList[j][i + 1]], new Vector3(this.gameObject.transform.position.x + i + 1, this.gameObject.transform.position.y - j, this.gameObject.transform.position.z),
                            this.gameObject.transform.rotation, this.gameObject.transform);

                            coords.tileR = tileR;
                            _pipes.Add(coords);
                        }
                    }
                    else if (_parsedList[j][i] == "922")
                    {
                        _secretZonePos.Add(tile.transform);
                    }
                    else if (_parsedList[j][i] == "923")
                    {
                        _marioPosition = tile.transform.position;
                    }
                    else if (_parsedList[j][i] == "266" && j > 32)
                    {
                        _exitSecretZones.Add(tile);
                    }
                    else if (_parsedList[j][i] == "292")
                    {
                        //tile.AddComponent<Platform>();
                        /* Debug.Log("PLATFORM: " + "J " + j + "    I " + i);
                         GameObject platform = new GameObject();
                         platform.transform.SetParent(GameObject.Find("Platforms").transform);
                         GameObject tile1 = Instantiate(_tiles[_parsedList[j][i]], new Vector3(this.gameObject.transform.position.x + i, this.gameObject.transform.position.y - j, this.gameObject.transform.position.z),
                        platform.transform.rotation);
                         if(_parsedList[j][i+1] == "292")
                         {
                             GameObject tile2 = Instantiate(_tiles[_parsedList[j][i]], new Vector3(this.gameObject.transform.position.x + i, this.gameObject.transform.position.y - j + 1, this.gameObject.transform.position.z),
                        platform.transform.rotation);
                         if (_parsedList[j][i + 2] == "292")
                         {
                             GameObject tile3 = Instantiate(_tiles[_parsedList[j][i]], new Vector3(this.gameObject.transform.position.x + i, this.gameObject.transform.position.y - j + 2, this.gameObject.transform.position.z),
                    platform.transform.rotation, platform.transform);
                         }
                     }
                             // j += 3;
                         platform.AddComponent<Platform>();
                         if(platform.transform.childCount == 3)
                         /*if(j != _platformIndex)
                         {
                             _platformIndex = j;
                             gen.GetComponent<Platform>().SetDirection(false);
                         }
                             _platforms.Add(platform);*/
                    }
                    else if (_parsedList[j][i] == "148")
                    {

                    }
                    
                }
            }
        }
        //Debug.Log("FINISH");
        //Debug.Log("PLATFORMS: " + _platforms.Count);
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
        int _exitIndex = 0;
        int _enterIndex = 0;
        Debug.Log("CHECK PIPES");
        for (int i = 0; i < _pipes.Count; i++)
        {
            int j = _pipes[i].x;
            Debug.Log("INICIAL: " + j);
            while (_parsedList[j][_pipes[i].y] != "0")
            {
                Debug.Log("WHILE: " + _parsedList[j][_pipes[i].y]);
                j++;
                if (_parsedList[j][_pipes[i].y] == "66")
                {
                    _pipes.RemoveAt(i);
                    i--;
                    break;
                }
            }
            if (_parsedList[j][_pipes[i].y] != "0")
            {
                continue;
            }

            Debug.Log("ALTURA: " + j);
            if (_parsedList[j][_pipes[i].y] == "0")
            {
                int aux = j;
                while (aux < j + 29 && _parsedList[aux][_pipes[i].y] != "266")
                {
                    aux++;
                    Debug.Log("AUX: " + aux);
                }
                //j += 13;
                //j += 25;
                j = aux;
                Debug.Log("SECRET: " + _parsedList[j][_pipes[i].y]);
                if (_parsedList[j][_pipes[i].y] == "266")
                {
                    //Debug.Log("SECRET: " + i + "      " + _parsedList[j][_pipes[i].y - 2]);
                    if (i > 0)
                    {

                        _pipes[i - 1].tileL.AddComponent<EnterSecretZone>().SetEnterZoneIndex(_enterIndex);
                        _pipes[i - 1].tileR.AddComponent<EnterSecretZone>().SetEnterZoneIndex(_enterIndex);
                        Debug.Log("TUBERIA Nº: " + (i - 1));
                        //Debug.Log("1: " + _parsedList[_pipes[i - 1].x][_pipes[i - 1].y] + "   2: " + _parsedList[_pipes[i - 1].x][_pipes[i - 1].y + 1]);
                        //_parsedList[_pipes[i-1].x][_pipes[i - 1].y]  addcomponent(entrada zona secreta)
                        //_parsedList[_pipes[i - 1].x][_pipes[i - 1].y]  addcomponent(entrada zona secreta)
                        /*_pipes[i - 1].tileL.AddComponent<AudioSource>();
                        _pipes[i - 1].tileR.AddComponent<AudioSource>();*/
                    }

                    _exitSecretZones[_exitIndex].AddComponent<ExitSecretZone>().SetExitPipe(_pipes[i].tileL);
                    _exitIndex++;
                    _enterIndex++;
                    /* else
                     {
                         _pipes[i].tileL.AddComponent<AudioSource>();
                         _pipes[i].tileR.AddComponent<AudioSource>();
                     }*/
                    // _parsedList[j ][_pipes[i].y - 2]. addcomponent(salida zona secreta)
                    //Debug.Log("PIPE FOUND!");

                }
            }
            Debug.Log("PIPES: ");
        }
    }

    public Vector3 GetInitialPosition()
    {
        return _marioPosition;
    }

    public Transform GetSecretZonePos(int i)
    {
        return _secretZonePos[i];
    }
}