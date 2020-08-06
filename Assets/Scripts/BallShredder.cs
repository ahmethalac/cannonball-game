using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShredder : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if ( collision.GetComponent<Ball>()) {
            Destroy(collision.gameObject);
        }
    }
}
