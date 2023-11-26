using DG.Tweening;
using TMPro;
using UnityEngine;

public class Dispenser : MonoBehaviour
{
    [SerializeField] private int count;
    [SerializeField] private TMP_Text countText;

    private Transform _frontNeighbor;

    private void Start()
    {
        LevelManager.Instance.CheckDispensers += OnCheckDispenser;
    }

    private void OnDisable()
    {
        LevelManager.Instance.CheckDispensers -= OnCheckDispenser;
    }

    public void Init(int _count)
    {
        count = _count;
        countText.text = count.ToString();
        var tile = transform.parent.GetComponent<Tile>();
        foreach (var n in MapGenerator.Instance.Neighbors(tile))
            if (n._Y < tile._Y)
            {
                _frontNeighbor = n.transform;
                break;
            }
    }

    private void OnCheckDispenser()
    {
        if (_frontNeighbor.childCount == 0 && count > 0)
            Dispense();
    }

    private void Dispense()
    {
        var agent = MapGenerator.Instance.GetRandomAgent(_frontNeighbor);
        agent.transform.parent = transform;
        agent.transform.localPosition = new Vector3(0, 1.15f, 0);
        agent.transform.parent = _frontNeighbor;
        agent.transform.DOLocalMoveZ(0, 0.1f);

        count--;
        countText.text = count.ToString();
    }
}