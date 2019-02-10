using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public GameObject HUDParent;
    public Text levelNameLabel;
    public Text timeLeftLabel;

    public static UIController instance;

    void Awake()
    {

        if (instance != null)
        {
            Debug.Log("am i hitting this");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            HUDParent.SetActive(false); //set inactive by default and actually using gui will make it active
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LevelChanged(int levelNum, string newlevelName)
    {
        HUDParent.SetActive(true);
        levelNameLabel.text = "Level " + levelNum + ": " + newlevelName;
    }

    public void UpdateTimeLabel(string newTime)
    {
        timeLeftLabel.text = "Time Left: " + newTime;
    }
}
