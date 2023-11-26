using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class AgentBase : MonoBehaviour, IPointerClickHandler
{
    public Colors.AgentColor agentColor;

    [SerializeField] private float speed = 1;
    private readonly List<Queue<Tile>> _paths = new();
    private bool _isPathOpen;
    private bool _isPicked;
    private Queue<Tile> _optimumPath;

    private Tile _tile;

    private void Awake()
    {
        _tile = transform.parent.GetComponent<Tile>();
        AgentsDictionary.Add(gameObject.GetInstanceID(), this);
    }

    private IEnumerator Start()
    {
        LevelManager.Instance.CheckNewPath += CheckPathIsOpen;
        yield return new WaitForSeconds(0.5f);
        CheckPathIsOpen();
    }

    private void OnDisable()
    {
        LevelManager.Instance.CheckNewPath -= CheckPathIsOpen;
        AgentsDictionary.Remove(gameObject.GetInstanceID());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Touched();
    }

    private void CheckPathIsOpen()
    {
        foreach (var tile in MapGenerator.Instance.FirstRow)
            _paths.Add(Pathfinding.Instance.FindPath(_tile, tile));

        var route = 9999;
        foreach (var path in _paths.Where(path => path != null))
        {
            _isPathOpen = true;
            if (path.Count < route)
            {
                route = path.Count;
                _optimumPath = path;
            }
        }

        if (_isPathOpen)
            ActiveAgent();
    }

    private void ActiveAgent()
    {
        transform.DOLocalMoveY(1.15f, 0.2f).SetEase(Ease.InOutQuint);
        if (transform.parent == null) return;
        if (transform.parent.childCount == 2)
            Destroy(transform.parent.GetChild(1).gameObject);
    }

    private void Touched()
    {
        if (!_isPathOpen || _isPicked) return;

        _isPicked = true;

        var parent = transform.parent;
        transform.parent = null;
        parent.GetComponent<Tile>().CheckIfTileIsEmpty();
        LevelManager.Instance.CheckNewPath?.Invoke();
        StartCoroutine(MoveAgent());
    }

    private IEnumerator MoveAgent()
    {
        var target = Vector3.zero;
        float dis;
        foreach (var step in _optimumPath)
        {
            target.Set(step.transform.position.x, 1.15f, step.transform.position.z);
            dis = Vector3.Distance(transform.position, target);
            transform.DOMove(target, dis / speed).SetEase(Ease.Linear);
            yield return new WaitForSeconds(dis / speed);
        }

        var availablePickedSlot = LevelManager.Instance.GetFirstAvailableSlot(agentColor);
        transform.parent = availablePickedSlot;
        target = new Vector3(0, 1.15f, 0);
        dis = Vector3.Distance(transform.position, target);
        transform.DOLocalMove(target, dis / speed).SetEase(Ease.Linear).OnComplete(() => { LevelManager.Instance.CheckForMatch(); });
        LevelManager.Instance.CheckDispensers?.Invoke();
    }

    public void MoveToNextSlot(Transform slot)
    {
        transform.parent = slot;
        transform.DOLocalMoveX(0, 0.1f);
    }
}