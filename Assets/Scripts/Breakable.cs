using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Breakable : MonoBehaviour
{
    private void Start() {
        if (breakCount != 1) {
            FindObjectOfType<CanvasManipulator>().CreateText(this,false);
        }
        else {
            FindObjectOfType<CanvasManipulator>().CreateText(this,true);
        }
    }
    private int breakCount;
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Ball") {
            if (breakCount != 0) {
                breakCount--;
            }
            if (breakCount == 0) {            
                Destroy(gameObject);
                FindObjectOfType<Game>().DestroyPlatform(gameObject);
            }
        }
    }

    private void OnDestroy() {
        if (FindObjectOfType<CanvasManipulator>()) {
            FindObjectOfType<CanvasManipulator>().RemoveObstacle(this);
        }
    }
    public void SetBreakCount(int count) {
        breakCount = count;
    }

    public int GetBreakCount() {
        return breakCount;
    }
}
