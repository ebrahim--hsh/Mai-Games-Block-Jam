using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private string nextScene;
    [SerializeField] private Transform[] pickedSlots;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject gameOverPanel;

    public Colors.AgentColor[] slotsColor;

    private int _matched;
    public Action CheckDispensers;
    public Action CheckNewPath;

    private void Awake()
    {
        Time.timeScale = 1;
        Instance = this;
        Application.targetFrameRate = 60;
    }


    public Transform GetFirstAvailableSlot(Colors.AgentColor color)
    {
        for (var i = slotsColor.Length - 1; i >= 0; i--)
            if (slotsColor[i] == color)
            {
                for (var j = slotsColor.Length - 2; j > i; j--)
                {
                    slotsColor[j + 1] = slotsColor[j];
                    if (slotsColor[j] != Colors.AgentColor.None)
                        AgentsDictionary.GetAgent(pickedSlots[j].GetChild(0).gameObject.GetInstanceID()).MoveToNextSlot(pickedSlots[j + 1]);
                }

                slotsColor[i + 1] = color;
                return pickedSlots[i + 1];
            }

        for (var i = 0; i < slotsColor.Length; i++)
            if (slotsColor[i] == Colors.AgentColor.None)
            {
                slotsColor[i] = color;
                return pickedSlots[i];
            }

        return null;
    }

    public void CheckForMatch()
    {
        for (var i = 0; i < slotsColor.Length - 2; i++)
            if (slotsColor[i] == slotsColor[i + 1] && slotsColor[i + 1] == slotsColor[i + 2] && slotsColor[i] != Colors.AgentColor.None)
            {
                slotsColor[i] = slotsColor[i + 1] = slotsColor[i + 2] = Colors.AgentColor.None;
                Destroy(pickedSlots[i].GetChild(0).gameObject);
                Destroy(pickedSlots[i + 1].GetChild(0).gameObject);
                Destroy(pickedSlots[i + 2].GetChild(0).gameObject);

                for (var j = pickedSlots.Length - 1; j > i + 2; j--)
                    if (slotsColor[j] != Colors.AgentColor.None)
                        AgentsDictionary.GetAgent(pickedSlots[j].GetChild(0).gameObject.GetInstanceID()).MoveToNextSlot(pickedSlots[j - 3]);

                StartCoroutine(UpdateSlotsColor());

                _matched += 3;
                if (_matched == MapGenerator.Instance.AgentsCount)
                    winPanel.SetActive(true);

                return;
            }


        var isGameOver = slotsColor.All(a => a != Colors.AgentColor.None);
        if (!isGameOver) return;

        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    private IEnumerator UpdateSlotsColor()
    {
        yield return new WaitForNextFrameUnit();
        for (var i = 0; i < pickedSlots.Length; i++)
            if (pickedSlots[i].childCount == 0)
                slotsColor[i] = Colors.AgentColor.None;
            else
                slotsColor[i] = AgentsDictionary.GetAgent(pickedSlots[i].GetChild(0).gameObject.GetInstanceID()).agentColor;
    }

    public void NextLevelPressed()
    {
        SceneManager.LoadScene(nextScene);
    }

    public void RetryPressed()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}