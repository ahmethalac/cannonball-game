using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Platform Config")]
public class PlatformConfig : ScriptableObject {
    [SerializeField] float bounce;
    [SerializeField] Color color;
    [SerializeField] Sprite sprite;
    [SerializeField] string header;
    [Header("Movement")]
    [SerializeField] bool isMoving;
    [SerializeField] bool movementInY;
    [SerializeField] bool toLeft;
    [SerializeField] float center;
    [SerializeField] float amplitude;
    [SerializeField] float movementSpeed;
    [SerializeField] float stopAtMaximum;
    [Header("Fixing the after-collision velocity")]
    [SerializeField] bool isFixed;
    [SerializeField] bool specificTargetOrFixedAngle;
    [SerializeField] Vector2 targetOrAngle;


    public float GetBounceFactor() {
        return bounce;
    }

    public float[] GetColor() {
        float[] temp = new float[3];
        temp[0] = color.r;
        temp[1] = color.g;
        temp[2] = color.b;
        return temp;
    }

    public Sprite GetSprite() {
        return sprite;
    }

    public string GetName() {
        return header;
    }

    public bool GetMovement() {
        return isMoving;
    }

    public float GetSpeed() {
        return movementSpeed;
    }

    public float GetCenter() {
        return center;
    }

    public float GetAmplitude() {
        return amplitude;
    }

    public float GetStopAtMaximum() {
        return stopAtMaximum;
    }

    public bool GetMovementInY() {
        return movementInY;
    }

    public bool GetToLeft() {
        return toLeft;
    }

    public bool GetFixedCollisionEffect() {
        return isFixed;
    }

    public bool IsSpecificTarget() {
        return specificTargetOrFixedAngle;
    }

    public Vector2 GetTargetOrAngle() {
        return targetOrAngle;
    }
}
