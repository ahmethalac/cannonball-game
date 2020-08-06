using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    private float scale;
    private bool increaseScale;
    [SerializeField] float scaleSpeed;
    public enum boosts {
        DoubleBall,
        StopObstacles,
        Bomb
    };

    [SerializeField] boosts boost;
    [SerializeField] List<Color> colors;
    [SerializeField] List<Sprite> sprites;

    int count;
    float bombRange;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<Ball>()) {
            if (boost == boosts.StopObstacles) {
                Debug.Log("again");
                FindObjectOfType<Game>().CreateAgain(transform.position,boost);
            }
            ActivateBoost();
            FindObjectOfType<Game>().RemoveObstacle(gameObject);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    public void Start() {
        increaseScale = true;
        scale = 5;
        ConfigureColor();
        count = 10;
        bombRange = 5f;
        GetComponent<CircleCollider2D>().radius = 0.07f;
    }

    private void Update() {
        if ( increaseScale) {
            scale += scaleSpeed * Time.deltaTime;
        }
        else {
            scale -= scaleSpeed * Time.deltaTime;
        }

        if (scale < 5) {
            increaseScale = true;
        }else if(scale > 6) {
            increaseScale = false;
        }

        transform.localScale = new Vector2(scale,scale);
    }

    private void ConfigureColor() {
        switch (boost) {
            case boosts.DoubleBall:
                gameObject.GetComponent<SpriteRenderer>().color = colors[0];
                gameObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case boosts.StopObstacles:
                gameObject.GetComponent<SpriteRenderer>().color = colors[1];
                gameObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case boosts.Bomb:
                gameObject.GetComponent<SpriteRenderer>().color = colors[2];
                gameObject.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
        }
    }

    public void SetBoost(boosts boost) {
        this.boost = boost;
        ConfigureColor();
    }
    private void ActivateBoost() {
        switch (boost) {
            case boosts.DoubleBall:
                FindObjectOfType<Cannon>().ballCount += 25;
                break;
            case boosts.StopObstacles:
                FindObjectOfType<Game>().StopObstacles(count);
                FindObjectOfType<CanvasManipulator>().CreateCountDown(transform.position,count);
                break;
            case boosts.Bomb:
                FindObjectOfType<Game>().Explode(transform.position,bombRange);
                break;
        }
    }
}
