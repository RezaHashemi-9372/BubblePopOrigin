using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    #region MemberFields
    [SerializeField]
    private Ball ballPrefab;
    [SerializeField, Range(0, 1500)]
    private int shootForce = 350;
    [SerializeField]
    private Material lrMaterial;
    [SerializeField]
    private Color lrColor = Color.white;

    private GameObject laserObj;
    private Ball sampleObject = null;
    public static bool isShooting = false;
    private List<Vector2> vectorList = new List<Vector2>();
    private LineRenderer lr;
    private List<Color> sampleColor = new List<Color>();
    #endregion MemberFields

    #region MonoBehaviour Methods
    private void Awake()
    {
        lrMaterial.color = lrColor;
        lr = this.GetComponent<LineRenderer>();
        lr.positionCount = 2;
    }

    private void Update()
    {
        if (sampleObject == null)
        {
            BallCreator();
        }
    }

    #endregion MonoBehaviour Methods

    #region Private Methods
    //Creating ball for shoot
    private void BallCreator()
    {
        if (sampleObject != null)
        {
            return;
        }
        sampleObject = Instantiate(ballPrefab,
            this.transform.position,
            Quaternion.identity).SetColor(sampleColor[Random.Range(0, sampleColor.Count)]);
        sampleObject.transform.name = "Sample Objects";
        sampleObject.GetComponent<Ball>().moveState = Ball.MoveState.move;
        sampleObject.GetComponent<Ball>().isAttach = false;
        sampleObject.GetComponent<Rigidbody2D>().isKinematic = false;
        sampleObject.GetComponent<CircleCollider2D>().enabled = false;
        
    }

    //Update line renderer everyframe
    private void UpdateLaser()
    {
        int count = 0;
        lr.positionCount = vectorList.Count;

        foreach (Vector2 vec in vectorList)
        {
            lr.SetPosition(count, vec);
            count++;
        }
    }


    private void Raycast(Vector3 pos, Vector3 dir, LineRenderer lineRen)
    {
        if (vectorList.Count == 4)
        {
            UpdateLaser();
            return;
        }
        lr.startWidth = .07f;
        lr.endWidth = .07f;
        vectorList.Add(pos);
        Ray2D ray = new Ray2D(pos, dir);
        RaycastHit2D hit;
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 5.0f);
        if (hit = Physics2D.Raycast(ray.origin, ray.direction, 10.0f))
        {
            CheckHit(hit, dir, lineRen);
        }
        else
        {
            vectorList.Add(ray.GetPoint(10.0f));
            UpdateLaser();
        }
    }

    //check the raycast to reflection of the linerenderer
    private void CheckHit(RaycastHit2D hit, Vector3 direction, LineRenderer lineRen)
    {

        if (hit.collider.gameObject.tag == "Wall")
        {
            vectorList.Add(hit.point);
            Vector3 temPos = hit.point;
            if (hit.point.x < 0)
            {
                temPos.x += .1f;
            }
            else if (hit.point.x > 0)
            {
                temPos.x -= .1f;
            }
            Vector3 pos = temPos;
            Vector3 dir = Vector2.Reflect(direction, hit.normal);
            Raycast(pos, dir, lineRen);
        }
        else
        {
            vectorList.Add(hit.point);
            UpdateLaser();
        }
    }

    #endregion Private Methods

    #region Public Methods

    //start laset process from here
    public void Laser(Vector3 pos)
    {
        if (!GameMode.hasChance)
        {
            return;
        }
        Destroy(GameObject.Find("Laser Beam"));
        lr = new LineRenderer();
        this.laserObj = new GameObject();
        this.laserObj.name = "Laser Beam"; 
        vectorList = new List<Vector2>();

        this.lr = this.laserObj.AddComponent(typeof(LineRenderer)) as LineRenderer;
        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(pos);
        Vector3 lookDir = mousePos - (Vector2)this.transform.position;
        lr.startColor = Color.red;
        lr.startWidth = .1f;
        lr.endWidth = .1f;
        Raycast(this.transform.position, lookDir, lr);

    }

    //recieve listof color to set the color which relative to color
    public void SetListColor(List<Color> colors)
    {
        sampleColor = new List<Color>();
        for (int i = 0; i < colors.Count; i++)
        {
            sampleColor.Add(colors[i]);
        }
    }


    //shoot te ball in the direction of mouse
    public void Shooot()
    {
        if (!GameMode.hasChance)
        {
            return;
        }
        isShooting = true;
        GameMode.staList = new List<GameObject>();
        sampleObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        Vector2 temp = vectorList[1];
        Vector2 dir = temp - (Vector2)sampleObject.transform.position;
        dir = dir.normalized;
        sampleObject.GetComponent<Rigidbody2D>().AddForce(dir * shootForce);
        sampleObject.GetComponent<CircleCollider2D>().enabled = true;
        GameMode.staList.Add(sampleObject.gameObject);
        sampleObject = null;
        GameMode.ShootChance -= 1;
    }

    #endregion Public Methods
}
