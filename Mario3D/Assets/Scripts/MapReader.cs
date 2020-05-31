using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

/// <summary>
/// Class that reads a map tile by tile and gives them "a representation"
/// </summary>
public class MapReader : MonoBehaviour
{
    //Instance of MapReader
    public static MapReader GM;

    public GameObject Mario;

    //List of tile values (string format)
    private List<string> _stringList;
    //Parsed list of the strings
    private List<string[]> _parsedList;
    //Init pos of Mario
    private List<string[]> _initPos;
    //Array with the gameobjects that can represent a single tile
    private GameObject[] _prefabs;
    //Dictionary to keep relationship between tile value and sprite
    private Dictionary<string, GameObject> _tiles;
    //List of all the pipes on the map
    private List<PipeCoords> _pipes;
    //List of all the secret zone positions of the map
    private List<Transform> _secretZonePos;
    //List of all the exit secret zones of the map
    private List<GameObject> _exitSecretZones;
    //Player position
    private Vector3 _marioPosition;
    private GameObject _mario;

    /// <summary>
    /// Struct that represents the enter of a pipe
    /// </summary>
    private struct PipeCoords
    {
        //X, Y coords of the pipe
        public int x, y;

        //Left and right sprites of the pipe
        public GameObject tileL, tileR;

        /// <summary>
        /// Set tileL sprite
        /// </summary>
        /// <param name="tile"></param>

        public void SetTileL(GameObject tile) { tileL = tile; }
        /// <summary>
        /// Set tileR sprite
        /// </summary>
        /// <param name="tile"></param>
        public void SetTileR(GameObject tile) { tileR = tile; }

    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (GM == null)
        {
            GM = this;
        }
        else
        {
            //DestroyMap();
            Destroy(gameObject);
        }

    }

    /// <summary>
    /// Method that allows us to load the specified map given a mapLevel sprite
    /// </summary>
    /// <param name="mapLevel"></param>
    /// <param name="generationMode"></param>
    public void InitMap(string mapLevel, bool generationMode = true)
    {
        _stringList = new List<string>();
        _parsedList = new List<string[]>();
        _secretZonePos = new List<Transform>();
        _exitSecretZones = new List<GameObject>();
        _tiles = new Dictionary<string, GameObject>();
        _pipes = new List<PipeCoords>();
        _prefabs = Resources.LoadAll("Tiles").Cast<GameObject>().ToArray();

        ReadTextFile(Application.streamingAssetsPath + "/Maps/" + mapLevel + ".csv");
        LoadTiles();
    }

    /// <summary>
    /// Creates a map
    /// </summary>
    public void GenerateMap()
    {
        CreateMap();
    }

    /// <summary>
    /// Reads a given file and fills a string with the tile values
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parse"></param>
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

    /// <summary>
    /// Parses the list filled when reading a file in order to use it when creating tiles.
    /// </summary>
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


    /// <summary>
    /// Loads the tiles dictionary with the relationship between name and sprite
    /// </summary>
    private void LoadTiles()
    {
        for (int i = 0; i < _prefabs.Length; i++)
        {
            _tiles.Add(_prefabs[i].name, _prefabs[i]);
        }
    }

    /// <summary>
    /// Creates the map, reading the whole _parsedlist matrix, and instantiating in an specific position
    /// a tile with its own properties depending on the string value reades. (e.g. _parsedList[j][i] == "24" represents a magicblock)
    /// </summary>
    void CreateMap()
    {
        int pipeIdx = 0;
        bool _marioPosSet = false;

        DestroyMap();
        int SIZEX = _parsedList.Count;
        int SIZEY = _parsedList[0].Length;

        for (int i = 0; i < SIZEY; i++)
        {
            for (int j = 0; j < SIZEX; j++)
            {
                //Debug.Log(_parsedList[j][i]);
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
                    else if (!_marioPosSet && _parsedList[j][i] == "923"/* && !_marioPosSet*/)
                    {
                        _marioPosition = tile.transform.position;
                        _marioPosSet = true;
                        GameObject.Find("Mario").GetComponent<Player>().SetMarioPosition(_marioPosition);
                        //Instantiate(Mario, _marioPosition, this.gameObject.transform.rotation, this.gameObject.transform);
                    }
                    else if (_parsedList[j][i] == "299" && j > 32)
                    {
                        _exitSecretZones.Add(tile);
                    }
                    else if (_parsedList[j][i] == "255")
                    {
                        GameObject tileBackWater = Instantiate(_tiles["894"], new Vector3(this.gameObject.transform.position.x + i, this.gameObject.transform.position.y - j, this.gameObject.transform.position.z),
                        this.gameObject.transform.rotation, this.gameObject.transform);
                        tileBackWater.GetComponent<SpriteRenderer>().sortingOrder = -2;
                    }

                }
            }
        }
        //Debug.Log("FINISH");
        //Debug.Log("PLATFORMS: " + _platforms.Count);
        CheckPipes();
    }

    /// <summary>
    /// Destroys all the map tiles one  by one
    /// </summary>
    public void DestroyMap()
    {

        Transform[] tilesToDelete = gameObject.GetComponentsInChildren<Transform>();
        for (int i = 1; i < tilesToDelete.Length; i++)
        {
            Destroy(tilesToDelete[i].gameObject);
        }
    }

    /// <summary>
    /// Method to check where the pipes are. It check pipe by pipe if that pipe can be an exit pipe. This is done
    /// by looking down and seeing if it has an "exit secret zone pipe". If this is true, it gives the very previous pipe the enter secret zone component.
    /// Also checks if the secret zone is undergorund or underwater
    /// </summary>
    public void CheckPipes()
    {
        int _exitIndex = 0;
        int _enterIndex = 0;
        bool isUnderground = false;
        //Debug.Log("CHECK PIPES");
        for (int i = 0; i < _pipes.Count; i++)
        {
            int j = _pipes[i].x;
           // Debug.Log("INICIAL: " + j);
            while (_parsedList[j][_pipes[i].y] != "0")
            {
               // Debug.Log("WHILE: " + _parsedList[j][_pipes[i].y]);
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

            //Debug.Log("ALTURA: " + j);
            if (_parsedList[j][_pipes[i].y] == "0")
            {
                int aux = j;
                while (aux < j + 29 && _parsedList[aux][_pipes[i].y] != "299")
                {
                    aux++;
                 //   Debug.Log("AUX: " + aux);
                }
                //j += 13;
                //j += 25;
                j = aux;
               // Debug.Log("SECRET: " + _parsedList[j][_pipes[i].y]);
                if (_parsedList[j][_pipes[i].y] == "299")
                {
                    bool checkEnv = false;
                    if (!checkEnv)
                    {
                        if (_parsedList[j + 1][_pipes[i].y] == "232")
                        {
                            isUnderground = false;
                         //   Debug.Log("NOOOOOO");
                        }
                        if (_parsedList[j + 1][_pipes[i].y] == "66" || _parsedList[j + 1][_pipes[i].y] == "68")
                        {
                            isUnderground = true;
                          //  Debug.Log("YEEEEEEEESSSS");
                        }
                        checkEnv = true;
                    }
                    //Debug.Log("SECRET: " + i + "      " + _parsedList[j][_pipes[i].y - 2]);
                    if (i > 0)
                    {

                        _pipes[i - 1].tileL.AddComponent<EnterSecretZone>().SetEnterZoneIndex(_enterIndex);
                        _pipes[i - 1].tileR.AddComponent<EnterSecretZone>().SetEnterZoneIndex(_enterIndex);
                        _pipes[i - 1].tileL.GetComponent<EnterSecretZone>().EnteringUnderground(isUnderground);
                        _pipes[i - 1].tileR.GetComponent<EnterSecretZone>().EnteringUnderground(isUnderground);
                       // Debug.Log("TUBERIA Nº: " + (i - 1));
                        //Debug.Log("1: " + _parsedList[_pipes[i - 1].x][_pipes[i - 1].y] + "   2: " + _parsedList[_pipes[i - 1].x][_pipes[i - 1].y + 1]);
                        //_parsedList[_pipes[i-1].x][_pipes[i - 1].y]  addcomponent(entrada zona secreta)
                        //_parsedList[_pipes[i - 1].x][_pipes[i - 1].y]  addcomponent(entrada zona secreta)
                        /*_pipes[i - 1].tileL.AddComponent<AudioSource>();
                        _pipes[i - 1].tileR.AddComponent<AudioSource>();*/
                    }

                    _exitSecretZones[_exitIndex].AddComponent<ExitSecretZone>().SetExitPipe(_pipes[i].tileL);
                    _exitSecretZones[_exitIndex].GetComponent<Collider>().isTrigger = true;
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
           // Debug.Log("PIPES: ");
        }
    }

    /// <summary>
    /// Getter of initial mario position as a tile
    /// </summary>
    /// <returns></returns>
    public Vector3 GetInitialPosition()
    {
        return _marioPosition;
    }

    /// <summary>
    /// Getter of the secret zone position dpending on the given index
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public Transform GetSecretZonePos(int i)
    {
        return _secretZonePos[i];
    }
}