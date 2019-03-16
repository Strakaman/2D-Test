using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

//alot of coded borrowed and derived from brackeys 2d tutorial, tutorial on how to make a 2d platformer,
//and  unity's 3d platformer example

public class GMScript : MonoBehaviour
{
    public AudioClip[] backgroundMusics;
    private short numOfBGMs;
    public KeyCode nkey = new KeyCode();
    public KeyCode rkey = new KeyCode();
    private int currLevel; //used to store current level instead of constantly calling application.loadedlevel

    public int LASTLEVEL = 1; //to prevent loading past last level
    List<Stage> Stages = new List<Stage>(); //hold stages metadata
    public TimerScript timer; //game object is attached to a timer class that calculates and handles remaining time
    //public int timeleft; //used to determine if player should die based on the calculated time of OnGUI
    public struct Springer //used for spring so that spring properties can be passed to player spring collision function
    {
        public Ray elRay;
        public float intensity;
    }

    public enum Context //used to determine if player can perform certain actions and the properties of those actions
    {
        notstarted = 0,
        normal = 1, //p1 is running or jumping, not interacting with any object except ground
        walling = 2, //p1 is touching a wall...not yet implemented, implementation might not be needed
        zipping = 3, //p1 is on zipline...not yet implemented
        rappelling = 4, //p1 is rappelling...not yet implemented
        ringing = 5,
        dead = 8, //as in not alive
        finished = 9 //stage beat
    }
    public static UnityEvent OnPaused = new UnityEvent();
    public static UnityEvent OnUnPaused = new UnityEvent();
    public static UnityEvent OnLevelLoaded = new UnityEvent();
    public static UnityEvent OnDeath = new UnityEvent();
    public static UnityEvent OnStageClear = new UnityEvent();
    public static GMScript instance;

    public static Context state = Context.notstarted;
    //assume normal context initially, have to store here to allow access both by player and gamemanger
    public static bool paused = false;
    void Start()
    {
        numOfBGMs = (short)backgroundMusics.Length;
        //timer = GameObject.FindGameObjectWithTag("Timer");
        Stages.Add(new Stage("Title Screen", 0, 100, true, "", false));
        Stages.Add(new Stage("Life On the Line", 1, 30, true, "Does she know my life is on the line?"));
        Stages.Add(new Stage("Ramping Up", 2, 55, false, "It was a bad breakup."));
        Stages.Add(new Stage("Leap of Faith", 3, 30, false, "Just believe..."));
        Stages.Add(new Stage("High Up", 4, 45, false, "I hate sky deliveries."));
        Stages.Add(new Stage("Bouncy Zone", 5, 30, false, "Used to love coming here as a kid."));
        Stages.Add(new Stage("Down and Up", 6, 30, false, "To lose is to gain..."));
        Stages.Add(new Stage("The Climb To Freedom", 7, 60, false, "Use everything you learned to escape!"));
        Stages.Add(new Stage("FREEDOM", 8, 9000, false, "Congradulations!"));
        LASTLEVEL = Stages.Count-1; 
        currLevel = 0;
        timer = new TimerScript();
    }

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
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause") && state.Equals(Context.normal))
        {
            Debug.Log("pause please");
            Pause();
        }
        if ((Input.GetButtonDown("Submit")) && (SceneManager.GetActiveScene().buildIndex < LASTLEVEL) && state.Equals(Context.finished))
        {
            Debug.Log("hit");
            NextLevel(); //only here for testing, TODO - remove
        }
        if ((Input.GetKey(nkey)) && (SceneManager.GetActiveScene().buildIndex < LASTLEVEL))
        {
            NextLevel(); //only here for testing, TODO - remove
        }
        if ((Stages[currLevel].RealStage()) && ((int)state < 8) && (timer.TimeRemaining() <= 0))
        {
            StageFailed(); //if this is a real stage and the user is killable and has run out of time....kill him :)
        }
        if ((Input.GetKey(rkey)) && (state.Equals(Context.dead)))
        {
            RestartLevel(); //actually will use this code
        }
        else if ((Input.GetKey(rkey)) && (state.Equals(Context.finished)) && (SceneManager.GetActiveScene().buildIndex < LASTLEVEL))
        {
            NextLevel(); //actually will use this code
        }
        timer.UpdateTime(); //update timer
    }

    public void Pause()
    {
        if (!paused)
        {
            Debug.Log("pausing");
            paused = true;
            OnPaused.Invoke();
        }
    }

    public void UnPause()
    {
        if (paused)
        {
            Debug.Log("unpausing");
            paused = false;
            OnUnPaused.Invoke();
        }
    }

    void StartTimer()
    {   //only start timer if stage is a real playable scene
        if (Stages[currLevel].RealStage())
        {
            timer.RestartTimer(Stages[currLevel].levelTime());
        }
    }

    void StopTimer()
    {
        timer.StopTimer();
    }

    void StageClear()
    {   /*stage success - stop timer, unlock next stage, option to go to next level, restart stage, or quit
		  only really want to activate if player hasn't failed stage, just in case time is up but player falls into goal
		*/
        if (!state.Equals(Context.dead))
        {
            StopTimer();
            state = Context.finished;
            if (currLevel < LASTLEVEL)
            {
                Stages[currLevel + 1].unlock();
            }
        }
        OnStageClear.Invoke();
    }

    public void StageFailed()
    {
        StopTimer();
        //state = Context.dead;
        FindObjectOfType<PlayerMovementInput>().Die();
        OnDeath.Invoke();
    }

    public void NextLevel()
    {       //go to the next stage and start the timer for that level
        state = Context.normal;
        UnPause();
        OnLevelLoaded.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //Application.LoadLevel (Application.loadedLevel + 1);
        currLevel++;
        if (currLevel != 8)
        {//last level
            StartTimer();
        }
        AudioManager.instance.PlayMusic(backgroundMusics[currLevel / numOfBGMs]);
        UIController.instance.LevelChanged(currLevel, Stages[currLevel].levelName());

    }

    public void RestartLevel()
    {   //reload level and start timer, for user choosen restart
        state = Context.normal;
        UnPause();
        OnLevelLoaded.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //Application.LoadLevel (Application.loadedLevel);
        StartTimer();

    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GoToCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public class TimerScript
    {
        //private float startTime;
        private float restSeconds;
        private int roundedRestSeconds = 1; //have to default to 1 or else timer will initialize to 0 and it will be instant stage fail
        private float displaySeconds;
        private float displayMinutes;
        private int CountDownSeconds = 120;
        private float timeElapsed;
        private bool stopTime = false;
        private bool drawTime = true;
        string timetext;
        // Use this for initialization
        public TimerScript()
        {

        }

        void Start()
        {
            //startTime = Time.time;
            timeElapsed = 0;
        }

        public void UpdateTime()
        {
            if (paused) { return; }
            if (!stopTime)
                //Timeleft = Time.time - startTime;
                timeElapsed += Time.deltaTime;
            {
                //restSeconds= Time.time-startTime;
                restSeconds = CountDownSeconds - (timeElapsed);
                roundedRestSeconds = Mathf.CeilToInt(restSeconds);
                if (roundedRestSeconds > 0)
                {
                    displaySeconds = roundedRestSeconds % 60;
                    displayMinutes = (roundedRestSeconds / 60) % 60;
                    timetext = (displayMinutes.ToString() + ":");
                    if (displaySeconds > 9)
                    {
                        timetext = timetext + displaySeconds.ToString();
                    }
                    else
                    {
                        timetext = timetext + "0" + displaySeconds.ToString();
                    }
                }
                else
                {
                    timetext = "0:00";
                }
            }
            if (drawTime)
            {
                UIController.instance.UpdateTimeLabel(timetext);
                //GUI.Label(new Rect(750.0f, 0.0f, 100.0f, 75.0f), timetext);
            }
        }

        public void RestartTimer(int leveltime)
        {
            stopTime = false;
            drawTime = true;
            CountDownSeconds = leveltime;
            timeElapsed = 0;
            //startTime = Time.time;
        }

        public void StopTimer()
        {
            stopTime = true;
        }

        public int TimeRemaining()
        {
            return roundedRestSeconds;
        }
    }
}
