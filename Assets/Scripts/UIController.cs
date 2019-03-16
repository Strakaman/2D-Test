using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject HUDParent;
    public GameObject PauseMenu;
    public GameObject DeathMenu;
    public Text PressEnterMessage;
    public Text levelNameLabel;
    public Text timeLeftLabel;
    public EventSystem pauseMenuEventSystem;
    public EventSystem deathMenuEventSystem;

    public static UIController instance;

    void Awake()
    {
        if (instance != null)
        {
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
        GMScript.OnPaused.AddListener(new UnityAction(OnGamePaused));
        GMScript.OnUnPaused.AddListener(new UnityAction(OnGameUnPaused));
        GMScript.OnDeath.AddListener(new UnityAction(OnStageFailed));
        GMScript.OnLevelLoaded.AddListener(new UnityAction(OnLevelLoaded));
        GMScript.OnStageClear.AddListener(new UnityAction(OnStageClear));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GMScript.instance.UnPause();
        }
        //pauseMenuEventSystem.SetSelectedGameObject(pauseMenuEventSystem.currentSelectedGameObject);

    }

    public void LevelChanged(int levelNum, string newlevelName)
    {
        HUDParent.SetActive(true);
        PauseMenu.SetActive(false);
        levelNameLabel.text = "Level " + levelNum + ": " + newlevelName;
    }

    public void UpdateTimeLabel(string newTime)
    {
        timeLeftLabel.text = "Time Left: " + newTime;
    }

    public void OnGamePaused()
    {
        PauseMenu.SetActive(true);
        PressEnterMessage.transform.parent.gameObject.SetActive(true);
        PressEnterMessage.text = "Press Enter//A To Choose";
        pauseMenuEventSystem.SetSelectedGameObject(pauseMenuEventSystem.firstSelectedGameObject);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnGameUnPaused()
    {
        PauseMenu.SetActive(false);
        PressEnterMessage.transform.parent.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnStageFailed()
    {
        DeathMenu.SetActive(true);
        PressEnterMessage.transform.parent.gameObject.SetActive(true);
        PressEnterMessage.text = "Press Enter//A To Choose";
        deathMenuEventSystem.SetSelectedGameObject(deathMenuEventSystem.firstSelectedGameObject);
    }

    public void OnLevelLoaded()
    {
        PauseMenu.SetActive(false);
        DeathMenu.SetActive(false);
        PressEnterMessage.transform.parent.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnStageClear()
    {
        PressEnterMessage.transform.parent.gameObject.SetActive(true);
        PressEnterMessage.text = "Press Enter//A To Continue";
    }

}
