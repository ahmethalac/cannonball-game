using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] bool toLeft = true;
    [SerializeField] bool toUp = true;

    [SerializeField] bool move = true;
    [SerializeField] bool movementInY = false;
    [SerializeField] bool stopCompletely = false;

    float center;
    float amplitude;
    [SerializeField] float stopAtMaximum;
    // Start is called before the first frame update
    void Start()
    {
        stopCompletely = false;
    }

    bool leftCorner = false;
    bool rightCorner = false;
    bool upperCorner = false;
    bool downCorner = false;
    // Update is called once per frame
    void Update()
    {
        if (movementInY) {
            MoveY();
        }
        else {
            MoveX();
        }
    }

    private void MoveX() {
        if (gameObject.transform.position.x == GetComponent<SpriteRenderer>().bounds.size.x / 2 + center - amplitude) {
            toLeft = false;
            if (!leftCorner) {
                StartCoroutine(stop());
            }
            leftCorner = true;
        }
        else {
            leftCorner = false;
        }
        if (gameObject.transform.position.x == center + amplitude - GetComponent<SpriteRenderer>().bounds.size.x / 2) {
            toLeft = true;
            if (!rightCorner) {
                StartCoroutine(stop());
            }
            rightCorner = true;
        }
        else {
            rightCorner = false;
        }

        if (move && !stopCompletely) {
            if (toLeft) {
                transform.position = Vector3.MoveTowards(transform.position,
                                                         new Vector3(GetComponent<SpriteRenderer>().bounds.size.x / 2 + center - amplitude,
                                                                     transform.position.y,
                                                                     0),
                                                         speed);
            }
            else {
                transform.position = Vector3.MoveTowards(transform.position,
                                             new Vector3(center + amplitude - GetComponent<SpriteRenderer>().bounds.size.x / 2,
                                                         transform.position.y,
                                                         0),
                                             speed);
            }
        }
    }

    private void MoveY() {
        if (gameObject.transform.position.y == GetComponent<SpriteRenderer>().bounds.size.y / 2 + center - amplitude) {
            toUp = false;
            if (!upperCorner) {
                StartCoroutine(stop());
            }
            upperCorner = true;
        }
        else {
            upperCorner = false;
        }
        if (gameObject.transform.position.y == center + amplitude - GetComponent<SpriteRenderer>().bounds.size.y / 2) {
            toUp = true;
            if (!downCorner) {
                StartCoroutine(stop());
            }
            downCorner = true;
        }
        else {
            downCorner = false;
        }

        if (move) {
            if (toUp) {
                transform.position = Vector3.MoveTowards(transform.position,
                                                         new Vector3(transform.position.x,
                                                                     GetComponent<SpriteRenderer>().bounds.size.y / 2 + center - amplitude,
                                                                     0),
                                                         speed);
            }
            else {
                transform.position = Vector3.MoveTowards(transform.position,
                                             new Vector3(transform.position.x,
                                                         center + amplitude - GetComponent<SpriteRenderer>().bounds.size.y / 2,
                                                         0),
                                             speed);
            }
        }
    }
    public IEnumerator stop() {
        move = false;
        yield return new WaitForSeconds(stopAtMaximum);
        move = true;
    }
    public void SetSpeed(float speed) {
        this.speed = speed;
    }

    public void SlowSpeed() {
        speed = speed / 3;
    }

    public float GetSpeed() {
        return speed;
    }
    public void SetCenter(float center) {
        this.center = center;
    }

    public void SetAmplitude(float amplitude) {
        this.amplitude = amplitude;
    }
    
    public void SetStopAtMaximum(float seconds) {
        stopAtMaximum = seconds;
    }

    public void SetMovementInY(bool x) {
        movementInY = x;
    }

    public void SetToLeft(bool x) {
        if (movementInY) {
            toUp = x;
        }
        else {
            toLeft = x;
        }
    }

    public void StartMoving() {
        stopCompletely = false;
    }
    public void Stop() {
        stopCompletely = true;
    }
}
