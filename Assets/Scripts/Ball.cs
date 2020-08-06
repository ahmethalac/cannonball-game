using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    float time;
    [SerializeField] float maxTime;
    [SerializeField] List<Color> colorPalette;

    Rigidbody2D rigidBody;
    private void Start() {
        GetComponent<SpriteRenderer>().color = colorPalette[Random.Range(0,colorPalette.Count)];
        rigidBody = GetComponent<Rigidbody2D>();
        time = Time.time;
        maxTime = 7f;
    }
    private void Update() {
        if ( rigidBody.velocity.magnitude > FindObjectOfType<Cannon>().GetBallSpeed()) {
            rigidBody.velocity = rigidBody.velocity.normalized * FindObjectOfType<Cannon>().GetBallSpeed();
        }
        if (Time.time - time > maxTime) {
            Destroy(gameObject);
        }
    }
}
