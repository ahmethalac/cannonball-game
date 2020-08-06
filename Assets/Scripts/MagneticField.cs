using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticField : MonoBehaviour
{
    [SerializeField][Range(0,10)] private float power = 1;

    float turnSpeed;

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.tag == "Ball") {
            collision.gameObject.GetComponent<Rigidbody2D>().velocity += power * (Vector2)(transform.position - collision.transform.position).normalized;
        }
    }
    
    public void SetPower(float power) {
        this.power = power;
    }

    public void SetTurnSpeed(float speed) {
        turnSpeed = speed;
    }
    private void Update() {
        transform.Rotate(Vector3.forward, turnSpeed);
    }
}
