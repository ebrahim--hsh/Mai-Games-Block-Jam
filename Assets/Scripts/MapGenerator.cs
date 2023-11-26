using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;
    [SerializeField] private string[] map;
    [SerializeField] private int sizeX = 20;
    [SerializeField] private int sizeY = 20;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject hiddenPrefab;
    [SerializeField] private GameObject dispenserPrefab;
    [SerializeField] private int[] dispensersCount;

    [SerializeField] private int agentsCount = 9;
    [SerializeField] private GameObject[] agents;
    [SerializeField] private Transform tilesParent;

    private int _agentIndex;
    private int _dispenserIndex;

    private Tile[,] _grid;


    private Dictionary<Tile, Tile[]> _neighborDictionary;
    private List<GameObject> _randomizedAgents = new();

    public List<Tile> FirstRow { get; } = new();

    public int AgentsCount => agentsCount;

    private void Awake()
    {
        Instance = this;
        _grid = new Tile[sizeX, sizeY];
        _neighborDictionary = new Dictionary<Tile, Tile[]>();
        _randomizedAgents = GetRandomizeColorFulItems();
        GenerateMap();
    }

    private void Start()
    {
        LevelManager.Instance.CheckNewPath?.Invoke();
    }

    public Tile[] Neighbors(Tile tile)
    {
        return _neighborDictionary[tile];
    }

    private void GenerateMap()
    {
        //Generate Map
        for (var y = 0; y < sizeY; y++)
        for (var x = 0; x < sizeX; x++)
        {
            _grid[x, y] = Instantiate(tilePrefab, new Vector3(x - 4, -0.499f, y - 2), Quaternion.identity, tilesParent).GetComponent<Tile>();
            _grid[x, y].Init(x, y);
            if (y == 0)
                FirstRow.Add(_grid[x, y]);
        }

        //Build Graph from map
        for (var y = 0; y < sizeY; y++)
        for (var x = 0; x < sizeX; x++)
        {
            var neighbors = new List<Tile>();
            if (y < sizeY - 1)
                neighbors.Add(_grid[x, y + 1]);
            if (x < sizeX - 1)
                neighbors.Add(_grid[x + 1, y]);
            if (y > 0)
                neighbors.Add(_grid[x, y - 1]);
            if (x > 0)
                neighbors.Add(_grid[x - 1, y]);

            _neighborDictionary.Add(_grid[x, y], neighbors.ToArray());
        }

        GenerateLevel();
    }

    private void GenerateLevel()
    {
        for (var x = 0; x < sizeX; x++)
        for (var y = 0; y < sizeY; y++)
            switch (map[y][x])
            {
                case 'W':
                    var wallObj = Instantiate(wallPrefab, Vector3.zero, Quaternion.identity, _grid[x, sizeY - y - 1].transform);
                    wallObj.transform.localPosition = new Vector3(0, 1, 0);
                    break;
                case 'A':
                    var item = Instantiate(_randomizedAgents[_agentIndex], Vector3.zero, Quaternion.identity, _grid[x, sizeY - y - 1].transform);
                    item.transform.localPosition = new Vector3(0, 0.3f, 0);
                    _agentIndex++;
                    break;
                case 'H':
                    var agent = Instantiate(_randomizedAgents[_agentIndex], Vector3.zero, Quaternion.identity, _grid[x, sizeY - y - 1].transform);
                    agent.transform.localPosition = new Vector3(0, 0.3f, 0);
                    _agentIndex++;

                    var hiddenObj = Instantiate(hiddenPrefab, Vector3.zero, Quaternion.identity, _grid[x, sizeY - y - 1].transform);
                    hiddenObj.transform.localPosition = new Vector3(0, -0.8f, 0);
                    break;
                case 'D':
                    var dis = Instantiate(dispenserPrefab, Vector3.zero, Quaternion.identity, _grid[x, sizeY - y - 1].transform).GetComponent<Dispenser>();
                    dis.transform.localPosition = new Vector3(0, 0.5f, 0);
                    dis.Init(dispensersCount[_dispenserIndex]);
                    _dispenserIndex++;
                    break;
            }
    }

    public GameObject GetRandomAgent(Transform parent)
    {
        var item = Instantiate(_randomizedAgents[_agentIndex], Vector3.zero, Quaternion.identity, parent);
        _agentIndex++;
        return item;
    }

    private List<GameObject> GetRandomizeColorFulItems()
    {
        var selectedColors = new List<GameObject>();
        var index = 0;
        for (var i = 0; i < agentsCount / 3; i++)
        {
            for (var j = 0; j < 3; j++) selectedColors.Add(agents[index]);

            index++;
            if (index == agents.Length)
                index = 0;
        }

        var randomized = new List<GameObject>();
        for (var i = 0; i < agentsCount; i++)
        {
            var random = Random.Range(0, selectedColors.Count - 1);
            randomized.Add(selectedColors[random]);
            selectedColors.RemoveAt(random);
        }

        return randomized;
    }
}