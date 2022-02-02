using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMode : MonoBehaviour
{
    #region MemberFields
    [SerializeField]
    private Text shootText;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text levelText;
    [SerializeField]
    private Text goalText;
    [SerializeField]
    private GameObject pnlWin;
    [SerializeField]
    private GameObject pnlLose;
    [SerializeField]
    private GameObject pnlPause;
    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private int numForColor = 0;
    [SerializeField]
    private List<Color> ConstColor = new List<Color>();
    [SerializeField, Range(1, 15)]
    private int row = 9;
    [SerializeField, Range(1, 15)]
    private int column = 10;
    [SerializeField]
    private Vector2 startPoint;

    private float diameter = 0;
    private List<Color> ChosenColor = new List<Color>();
    private LaserBeam laserBeam;
    public static List<GameObject> allObjects = new List<GameObject>();
    public static List<GameObject> staList = new List<GameObject>();
    private List<GameObject> regenerateObj = new List<GameObject>();
    public int Score { get; set; }
    public int LevelCounter { get; set; }
    public int GoalCounter { get; set; }
    public static int ShootChance { get; set; }
    public static bool hasChance = true;

    #endregion MemberFields


    #region MonoBehaviour Methods
    private void Awake()
    {
        PlayerPrefs.DeleteKey("Level");
        PlayerPrefs.DeleteKey("Score");
        pnlPause.SetActive(false);
        pnlLose.SetActive(false);
        pnlWin.SetActive(false);
        if (PlayerPrefs.GetInt("Level") > 1 && PlayerPrefs.GetInt("Level") != 0)
        {
            LevelCounter = PlayerPrefs.GetInt("Level");
        }
        else
        {
            PlayerPrefs.SetInt("Level", 1);
            LevelCounter = 1;
        }
        Score = PlayerPrefs.GetInt("Score");
        scoreText.text = string.Format("Score: {0}", Score);
        laserBeam = FindObjectOfType<LaserBeam>();
        diameter = ballPrefab.transform.localScale.x;
        GenerateLevel(LevelCounter);
    }
    void Update()
    {
        shootText.text = string.Format("{0}", ShootChance <= 0 ? 0 : ShootChance);

    }
    #endregion MonoBehaviour Methods


    #region Public Methods

    public void CountingGoal()
    {
        GoalCounter -= 1;
        goalText.text = string.Format("{0}", GoalCounter > 0 ? GoalCounter : 0);

        if (ShootChance <= 0 && GoalCounter > 0)
        {
            Invoke("ShowLosePanel", 1.0f);
        }
        if (GoalCounter <= 0)
        {
            Invoke("ShowWinPanel", 1.0f);
        }
    }

    public void RemoveSameColorBall()
    {
        if (staList.Count < 3)
        {
            return;
        }
        for (int i = 0; i < staList.Count; i++)
        {
            staList[i].GetComponent<Ball>().isMatched = true;
        }
        staList.Clear();
        CheckGoalAndWinOrLose();
        Debug.Log("Static list count is: " + staList.Count);
    }

    //Restart level via UI-button
    public void Restart()
    {
        DeleteScene();
        GenerateLevel(LevelCounter);
        LaserBeam.isShooting = false;
        pnlLose.SetActive(false);
        hasChance = true;
    }

    //load next level via UI-button
    public void NextLevel()
    {
        LevelCounter++;
        PlayerPrefs.SetInt("Level", LevelCounter);
        DeleteScene();
        GenerateLevel(LevelCounter);
        pnlWin.SetActive(false);
        LaserBeam.isShooting = false;
        hasChance = true;
    }

    //used for pause game to show pause panel
    public void Pause()
    {

        pnlPause.SetActive(true);
    }

    //Continue via UI-button
    public void Continue()
    {
        pnlPause.SetActive(false);
    }

    //ShiftDown all objects after any shoot
    public void ShiftDown()
    {
        for (int i = 0; i < allObjects.Count; i++)
        {
            if (allObjects.Contains(allObjects[i]))
            {
                allObjects[i].GetComponent<Ball>().NextPosition();
            }
        }
        ReGenerate();
        Invoke("CheckForRelease", .3f);
        //CheckForRelease();
    }

    //Exit game via UI-button
    public void QuitGame()
    {
        Debug.Log("You quit game");
        Application.Quit();
    }

    public void AddScore(int scr)
    {
        Score += scr;
        PlayerPrefs.SetInt("Score", Score);
        scoreText.text = string.Format("Score: {0}", Score);
    }

    public void CheckForRelease()
    {
        //Debug.Log("THe generated list count is: " + regenerateObj.Count);
        LaserBeam.isShooting = true;
        List<GameObject> tempList = new List<GameObject>();
        for (int i = 0; i < allObjects.Count; i++)
        {
            allObjects[i].GetComponent<Ball>().isChecked = false;
        }

        for (int i = 0; i < allObjects.Count; i++)
        {
            if (allObjects[i].GetComponent<Ball>().CheckUp())
            {
                tempList.Add(allObjects[i]);
            }
        }

        for (int i = 0; i < tempList.Count; i++)
        {
            tempList[i].GetComponent<Ball>().SpreadAround();
        }

        Invoke("ReleaseALL", .3f);
        

        LaserBeam.isShooting = false;
    }

#endregion Public Methods


    #region Private Methods

    private void ShowWinPanel()
    {
        hasChance = false;
        pnlWin.SetActive(true);
    }

    private void ShowLosePanel()
    {
        hasChance = false;
        pnlLose.SetActive(true);
    }
    private void ReleaseALL()
    {
        for (int i = 0; i < allObjects.Count; i++)
        {
            if (!allObjects[i].GetComponent<Ball>().isChecked)
            {
                allObjects[i].GetComponent<Ball>().Release();
            }
        }
    }

    private void CheckGoalAndWinOrLose()
    {
        if (ShootChance <= 0 && GoalCounter > 0 )
        {
            hasChance = false;
            Debug.Log("You loose the game bro");
        }

        levelText.text = string.Format("Level: {0}", LevelCounter);
        //goalText.text = string.Format("{0}", GoalCounter > 0 ? GoalCounter : 0 );
    }

    //level generator 
    private void GenerateLevel(int lvl)
    {
        LevelSetter(lvl);

        while (ChosenColor.Count < numForColor)
        {
            int n = Random.Range(0, ConstColor.Count);

            if (!ChosenColor.Contains(ConstColor[n]))
            {
                ChosenColor.Add(ConstColor[n]);
            }
        }

        laserBeam.SetListColor(ChosenColor);
        Vector3 pos = startPoint;
        for (int i = 0; i < row; i++)
        {
            pos.x = startPoint.x;
            for (int j = 0; j < column; j++)
            {
                GameObject temp = Instantiate(ballPrefab, pos, Quaternion.identity);
                temp.GetComponent<SpriteRenderer>().color = ChosenColor[Random.Range(0, ChosenColor.Count)];
                temp.GetComponent<Ball>().moveState = Ball.MoveState.stay;
                allObjects.Add(temp);
                pos.x += diameter + .1f;
            }
            pos.y += diameter + .1f;

        }

        shootText.text = string.Format("{0}", ShootChance <= 0 ? 0 : ShootChance);
        levelText.text = string.Format("Level: {0}", LevelCounter);
        goalText.text = string.Format("{0}", GoalCounter);
    }

    

    //Refill from the Top row with lerp 
    private void ReGenerate()
    {
        regenerateObj.Clear();
        LaserBeam.isShooting = true;
        Vector2 pos = new Vector2();

        pos.x = -2.35f;
        pos.y = 3.5f;

        for (int i = 0; i < column; i++)
        {
            GameObject temp = Instantiate(ballPrefab, pos, Quaternion.identity);
            temp.GetComponent<SpriteRenderer>().color = ChosenColor[Random.Range(0, ChosenColor.Count)];
            temp.GetComponent<Ball>().moveState = Ball.MoveState.stay;
            temp.transform.localScale = new Vector2(.1f, .1f);
            temp.GetComponent<Ball>().isSmall = true;
            temp.GetComponent<Ball>().isChecked = true;
            allObjects.Add(temp);
            regenerateObj.Add(temp);
            pos.x += diameter + .1f;
        }
        //LaserBeam.isShooting = false;
    }
    
    

    //Recieve level number and set the number of goal,color,shoot
    private void LevelSetter(int lvl)
    {
        int temp = new int();
        ShootChance = 0;
        numForColor = 0;
        GoalCounter = 0;
        ChosenColor.Clear();
        if (lvl < 5)
        {
            temp = 1;
        }
        else if (lvl >= 15)
        {
            temp = 3;
        }
        else
        {
            temp = lvl / 5;
        }

        ShootChance = temp * 2 + 3;
        numForColor = temp + 1;
        GoalCounter = temp * 5 + 20;
    }

    //Delete all the objects before creating new level or restarting
    private void DeleteScene()
    {
        Ball[] temp = FindObjectsOfType<Ball>();

        for (int i = 0; i < temp.Length; i++)
        {
            Destroy(temp[i].gameObject);
            allObjects.Remove(temp[i].gameObject);
        }
        allObjects.Clear();
    }


    #endregion Private Methods
    private void TestEverything()
    {
        
    }

}
