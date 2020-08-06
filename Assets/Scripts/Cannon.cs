using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Cannon : MonoBehaviour {
    [Header("Cannon Config")]
    [Range(0,600)] [Tooltip("Movement Speed(Angle per Second)")] [SerializeField] float movementSpeed = 60;
    [Range(15,80)] [Tooltip("Minimum angle between the cannon and bottom")] [SerializeField] float minAngleWithBottom = 45;

    [Header("Ball Config")]
    [SerializeField] GameObject ballPrefab;
    [Tooltip("Minimum time interval between the balls")] [SerializeField] float minTimeInterval = 0.1f;
    [Tooltip("Maximum time interval between the balls")] [SerializeField] float maxTimeInterval = 0.5f;
    [Tooltip("Decrease rate for the time interval in every ball throw")] [SerializeField] float firstDecreaseRate = 0.02f;
    [Tooltip("Decrease rate for the time interval in every ball throw")] [SerializeField] float secondDecreaseRate = 0.02f;
    [Tooltip("Speed of the ball")] [SerializeField] float ballSpeed = 15f;
    [SerializeField] public int ballCount = 40;

    Coroutine coroutine;
    float angle;
    Text text;
    [SerializeField] bool move = true;
    Game game;
    float barSecond;
    TimerBar timerBar;
    bool tbInitiated;
    float shortInterval;
    [SerializeField] bool fixedAngle;
    // Start is called before the first frame update
    void Start() {
        shortInterval = 0;
        tbInitiated = false;
        game = FindObjectOfType<Game>();
        barSecond = game.GetBarSecond();
        timerBar = FindObjectOfType<TimerBar>();
        InitiateText();
        fixedAngle = false;
        move = false;
        if (GameOver.checkpoint != 1) {
            ballCount = 120 - GameOver.checkpoint * 4;
        }
    }

    private void InitiateText() {
        foreach (Text element in FindObjectsOfType<Text>()) {
            if (element.name == "Remaining Balls") {
                text = element;
            }
        }
        text.enabled = true;
        text.text = ballCount.ToString();
        text.transform.position = transform.position + new Vector3(0.8f,0.3f,0);
    }

    // Update is called once per frame
    void Update() {
        Shoot2();
        text.text = ballCount.ToString();

        InitiateText();
        if (gameObject.transform.position.y != -8) {
            if (coroutine != null) {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            move = false;
        }
        else {
            if (Input.GetMouseButtonDown(0)) {
                shortInterval = 0;
            }
            if (Input.GetMouseButton(0) && !fixedAngle) {
                shortInterval += Time.deltaTime;
                move = true;
                if (shortInterval > 0.1f) {
                    fixedAngle = false;
                    if (!tbInitiated) {
                        timerBar.Initiate();
                        tbInitiated = true;
                    }
                    if (coroutine == null) {
                        coroutine = StartCoroutine(StartShooting());
                    }
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                if (shortInterval < 0.1f && ballCount > 0) {
                    GameObject ball = Instantiate(ballPrefab,GetTipPosition(),Quaternion.identity);
                    ball.GetComponent<Rigidbody2D>().velocity = transform.up.normalized * ballSpeed;
                    ball.GetComponent<Rigidbody2D>().freezeRotation = true;
                    ballCount--;
                }
                move = false;
                timerBar.Stop();
                tbInitiated = false;
                if (coroutine != null) {
                    StopCoroutine(coroutine);
                    coroutine = null;
                }
            }
        }

    }

    private void Shoot2() {
        if (move) {

            angle = Mathf.Atan(( Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y ) /
                         ( Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x )) * 360 / ( 2 * Mathf.PI );

            if (transform.rotation.eulerAngles.z < 90 && transform.rotation.eulerAngles.z >= 0) {
                if (angle > 0) {
                    transform.Rotate(0,0,-( 90 - angle + transform.rotation.eulerAngles.z ));
                }
                else {
                    transform.Rotate(0,0,-( -( 90 + angle ) + transform.rotation.eulerAngles.z ));
                }
            }
            else if (transform.rotation.eulerAngles.z < 360 && transform.rotation.eulerAngles.z > 270) {
                if (angle > 0) {
                    transform.Rotate(0,0,( angle - ( transform.rotation.eulerAngles.z - 270 ) ));
                }
                else {
                    transform.Rotate(0,0,( 360 - transform.rotation.eulerAngles.z + ( 90 + angle ) ));
                }
            }

        }

        float rotation = 0;
        if (transform.rotation.eulerAngles.z < 90 && transform.rotation.eulerAngles.z >= 0) {
            rotation = transform.rotation.eulerAngles.z;
        }
        else if (transform.rotation.eulerAngles.z > 270 && transform.rotation.eulerAngles.z < 360) {
            rotation = transform.rotation.eulerAngles.z - 360f;
        }
        foreach (Vector2 angles in game.correctAngles) {
            if (rotation > angles.x - game.angleTolerance && rotation < angles.x) {
                move = false;
                fixedAngle = true;
                transform.rotation = Quaternion.Euler(new Vector3(0,0,angles.x));
                //StartCoroutine(Wait());
                break;
            }
            else if (rotation < angles.y + game.angleTolerance && rotation > angles.y) {
                move = false;
                fixedAngle = true;
                transform.rotation = Quaternion.Euler(new Vector3(0,0,angles.y));
                //StartCoroutine(Wait());
                break;
            }
            else {
                fixedAngle = false;
            }
        }



    }

    public void StopMoving() {
        move = false;
    }

    public IEnumerator StartShooting() {
        yield return new WaitForSeconds(barSecond);
        coroutine = StartCoroutine(Shoot());
    }

    public IEnumerator Shoot() {
        float timeInterval = maxTimeInterval;
        while (true) {
            if (ballCount > 0) {
                GameObject ball = Instantiate(ballPrefab,GetTipPosition(),Quaternion.identity);
                ball.GetComponent<Rigidbody2D>().velocity = transform.up.normalized * ballSpeed;
                ball.GetComponent<Rigidbody2D>().freezeRotation = true;
                ballCount--;
            }
            yield return new WaitForSeconds(Mathf.Clamp(timeInterval,minTimeInterval,maxTimeInterval));
            Debug.Log(Mathf.Clamp(timeInterval,minTimeInterval,maxTimeInterval));
            if (timeInterval > ( minTimeInterval + maxTimeInterval ) / 2) {
                timeInterval -= firstDecreaseRate;
            }
            else {
                timeInterval -= secondDecreaseRate;
            }
        }
    }
    private Vector3 GetTipPosition() {
        float xPosition = transform.up.x + transform.position.x;
        float yPosition = transform.up.y + transform.position.y;
        return new Vector3(xPosition,yPosition,0);
    }

    private void OnDestroy() {
        if (text) {
            text.enabled = false;
        }
    }

    public float GetBallSpeed() {
        return ballSpeed;
    }

    public void SetBallCount(int count) {
        ballCount = count;
        move = true;
    }

    public int GetBallCount() {
        return ballCount;
    }

    public float GetBarSecond() {
        return barSecond;
    }
}
