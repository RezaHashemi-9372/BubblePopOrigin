using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPhoneController : MonoBehaviour
{
    private LaserBeam laserBeam;

    private void Awake()
    {
        laserBeam = this.GetComponent<LaserBeam>();
    }
    void Update()
    {
        if (Input.touchCount > 0)
        {
            TouchProcess(Input.GetTouch(0));
        }
        
        /*//just for editor test 
        if (Input.GetMouseButton(0))
        {
            TouchProcess(new Touch() { position = Input.mousePosition, phase = TouchPhase.Stationary });
        }*/
    }

    public void TouchProcess(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                Destroy(GameObject.Find("Laser Beam"));
                laserBeam.Laser(touch.position);
                break;

            case TouchPhase.Ended:
                Destroy(GameObject.Find("Laser Beam"));
                break;
            /*case TouchPhase.Canceled:
                break;
            default:
                break;*/
        }
    }
}
