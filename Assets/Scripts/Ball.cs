using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public enum MoveState
    {
        none = 0,
        stay,
        move
    }

    #region MemberFields

    [SerializeField, Range(1, 40)]
    private int score = 5;

    //List<GameObject> lRemove = new List<GameObject>();
    private float diameter = 0;
    private bool isSetted = false;
    public MoveState moveState = MoveState.none;
    public bool isMatched = false;
    public bool isAttach = false;
    public bool couldShift = false;
    public bool isChecked = false;
    public bool isSmall = false;
    private Rigidbody2D rb;
    private GameMode gameMode;
    private Vector2 nextPosition = Vector2.zero;
    private Vector2[] rayDirection = new Vector2[4] { Vector2.down, Vector2.up, Vector2.left, Vector2.right};
    private Vector2[] rayForAlone = new Vector2[4] { Vector2.down, Vector2.right, Vector2.left , Vector3.up};

    #endregion MemberFields


    #region Properties

    public Color Color
    {
        get
        {
            return this.GetComponent<SpriteRenderer>().color;
        }
        set
        {
            this.GetComponent<SpriteRenderer>().color = value;
        }
    }
    #endregion Properties


    #region MonoBehavior Methods

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
        diameter = this.transform.localScale.x;
        gameMode = FindObjectOfType<GameMode>();
        Physics2D.queriesStartInColliders = false;
    }
    void Update()
    {
        //for disapearing individually for any ball
        if (isMatched)
        {
            this.GetComponent<CircleCollider2D>().enabled = false;
            this.transform.localScale = Vector3.Lerp(this.transform.localScale,
                new Vector3(diameter + .1f, diameter + .1f), 25.0f * Time.deltaTime);

            if (this.transform.localScale.x >= diameter + .09f)
            {
                isMatched = false;
                Destroy(this.gameObject);
                GameMode.allObjects.Remove(this.gameObject);
                gameMode.AddScore(1);
                gameMode.CountingGoal();
            }
        }

        //shifting with the help of transform
        if (couldShift)
        {
            this.transform.position = Vector3.Lerp(this.transform.position,
                nextPosition,
                50.0f * Time.deltaTime);
            if (this.transform.position.y == nextPosition.y )
            {
                couldShift = false;
            }
        }
    }

    private void LateUpdate()
    {
        //Make bigger the top and small ball with Lerp effect
        if (isSmall)
        {
            this.transform.localScale = Vector3.Lerp(this.transform.localScale,
                new Vector2(.4f, .4f),
                35f * Time.deltaTime);

            if (this.transform.localScale.x >= .4f)
            {
                isSmall = false;
            }
        }
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (moveState == MoveState.move)
        {
            //LaserBeam.isShooting = false;
            if (collision.transform.tag == "Ball")
            {
                //Destroy(this.rb);
                this.rb.isKinematic = true;
                rb.angularVelocity = 0.0f;
                rb.velocity = Vector2.zero;
                this.moveState = MoveState.stay;
                this.isChecked = true;
                this.Raycast();
                if (collision.gameObject.GetComponent<SpriteRenderer>().color == this.Color)
                {
                    if (!GameMode.staList.Contains(collision.gameObject))
                    {
                        GameMode.staList.Add(collision.gameObject);
                    }
                    collision.gameObject.GetComponent<Ball>().Raycast();
                }
                if (!GameMode.allObjects.Contains(this.gameObject))
                {
                    GameMode.allObjects.Add(this.gameObject);
                }
                gameMode.RemoveSameColorBall();
                gameMode.ShiftDown();
            }
            if (collision.transform.name == "Top")
            {
                Destroy(this.rb);
                moveState = MoveState.stay;
            }
        }
    }

    #endregion MonoBehaviour Methods


    #region Public Methods
    //It was for Releasing alone ball
    public void Release()
    {
        Debug.Log("Release is working bro.");
        this.GetComponent<Rigidbody2D>().isKinematic = false;
        this.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 125.0f);
        this.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        this.GetComponent<CircleCollider2D>().radius = .1f;
        this.couldShift = false;
        //Destroy(gameObject, .3f);
    }

    //setting for shifting down
    public void NextPosition()
    {
        couldShift = true;
        nextPosition = new Vector2(this.transform.position.x, this.transform.position.y - .5f);
        
    }

    //check up for attaching any ball from above
    public bool CheckUp()
    {
        Debug.Log("CHeck Up is working.");
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Vector2.up, diameter + .1f);

        if (hit && hit.collider.CompareTag("TopWall"))
        {
            return true;
        }

        return false;
    }

    public void SpreadAround()
    {
        RaycastHit2D hit;

        for (int i = 0; i < rayForAlone.Length; i++)
        {
            hit = Physics2D.Raycast(this.transform.position, rayForAlone[i], diameter + .1f);
            if (hit)
            {
                if (hit.collider.GetComponent<Ball>() && !hit.collider.GetComponent<Ball>().isChecked)
                {
                    //this.isAttach = true;
                    this.isChecked = true;
                    hit.collider.GetComponent<Ball>().isChecked = true;
                    hit.collider.GetComponent<Ball>().SpreadAround();
                }
            }
        }

    }

    //setting color with the help of the SpriteRenderer
    public Ball SetColor(Color color)
    {
        this.Color = color;
        return this;
    }
    #endregion Public Methods

    #region Private Methods
    // Raycsting after colliding in four direction 
    private void Raycast()
    {
        RaycastHit2D hit;

        for (int i = 0; i < rayDirection.Length; i++)
        {
            hit = Physics2D.Raycast(this.transform.position, rayDirection[i], diameter );
            if (hit && hit.collider.CompareTag ("Ball"))
            {
                //Debug.Log("HIt color is: " + hit.collider.GetComponent<Ball>().Color);
                if (hit.collider.GetComponent<SpriteRenderer>().color == this.Color &&
                !GameMode.staList.Contains(hit.collider.gameObject))
                {
                    GameMode.staList.Add(hit.collider.gameObject);
                    hit.collider.GetComponent<Ball>().Raycast();
                }
            }
        }
    }

    #endregion Private Methods
}
