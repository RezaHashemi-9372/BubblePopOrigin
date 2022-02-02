using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    #region MonoBehaviour Methods

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Ball>() && collision.gameObject.GetComponent<Ball>().moveState == Ball.MoveState.stay)
        {
            GameMode.allObjects.Remove(collision.gameObject);
            Destroy(collision.gameObject);
        }
        /*if (!collision.GetComponent<Rigidbody2D>())
        {
            Debug.Log("You lose this game thanks more bro.");
        }*/
    }

    #endregion MonoBehaviour Methods
}
