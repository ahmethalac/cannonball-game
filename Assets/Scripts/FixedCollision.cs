using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCollision : MonoBehaviour
{
    [SerializeField] private bool specificTargetOrFixedAngle;
    [SerializeField] private Vector2 targetOrAngle;

    private void Start() {
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "FixedTargetObstacle";
    }
    public void SetSpecificTarget(bool x) {
        specificTargetOrFixedAngle = x;
    }

    public void SetTargetOrAngle(Vector2 target) {
        targetOrAngle = target;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Ball")) {
            var rigidBody = collision.gameObject.GetComponent<Rigidbody2D>();
            if (specificTargetOrFixedAngle) {
                Debug.Log("specifictarget");
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = rigidBody.velocity.magnitude * ( targetOrAngle - (Vector2)collision.transform.position ).normalized;
            }
            else {
                Debug.Log("fixedangle");
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = rigidBody.velocity.magnitude * targetOrAngle.normalized;
            }
        }
    }

    public bool GetSpecificTargetOrFixedAngle() {
        return specificTargetOrFixedAngle;
    }

    public Vector2 GetTargetOrAngle() {
        return targetOrAngle;
    }
}
