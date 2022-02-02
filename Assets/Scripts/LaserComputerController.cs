using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LaserComputerController : MonoBehaviour
{

    #region MemberFields
    private LaserBeam laserBeam;

    #endregion MemberFields

    #region MonoBehaviour Methods
    private void Awake()
    {
        laserBeam = this.GetComponent<LaserBeam>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.currentSelectedGameObject)
        {
            Destroy(GameObject.Find("Laser Beam"));
            laserBeam.Laser(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.currentSelectedGameObject)
        {
            laserBeam.Shooot();
           Destroy(GameObject.Find("Laser Beam"));
        }
    }

    #endregion MonoBehaviour Methods

    #region Private Methods

    #endregion Private Methods

    
    #region Public Methods

    #endregion Public Methods

}
