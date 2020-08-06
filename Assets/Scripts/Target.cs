using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    Text text;
    int count = 0;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<Ball>()) {
            collision.gameObject.GetComponent<Rigidbody2D>().velocity /= 10;
        }
    }
    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.GetComponent<Ball>()) {
            collision.gameObject.GetComponent<Rigidbody2D>().velocity += (Vector2)( transform.position - collision.transform.position ).normalized ;
        }
        if (Vector3.Distance(collision.gameObject.transform.position,transform.position) < 0.2f) {
            Destroy(collision.gameObject);
            count++;
        }
    }
    private void Start() {
        InitiateText();
    }

    private void InitiateText() {
        foreach (Text element in FindObjectsOfType<Text>()) {
            if (element.name == "ScoreText") {
                text = element;
            }
        }
        text.enabled = true;
        text.text = count.ToString();
        text.transform.position = transform.position - new Vector3(0,1f,0);
    }

    private void Update() {
        text.text = count.ToString();
        if (FindObjectOfType<Cannon>().GetBallCount() == 0 && !FindObjectOfType<Ball>()) {
            if ( count == 0) {
                FindObjectOfType<Game>().Finish();
            }
            else {
                FindObjectOfType<Game>().LoadNextLevel();
            }            
        }
    }

    private void OnDestroy() {
        if (FindObjectOfType<Cannon>()) {
            FindObjectOfType<Cannon>().SetBallCount(count);
        }
        if ( text) {
            text.enabled = false;
        }
    }

    public int GetCount() {
        return count;
    }
}
