using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Level Config")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] bool dependsOnPrevBallCount;
    [Header("Walls")]
    [SerializeField] bool solidLeftWall;
    [SerializeField] PlatformConfig solidLeftWallConfig;
    [SerializeField] bool solidRightWall;
    [SerializeField] PlatformConfig solidRightWallConfig;
    [Tooltip("Bottom to Top")][SerializeField] List<PlatformConfig> leftWallConfigs = new List<PlatformConfig>(4);
    [Tooltip("Bottom to Top")] [SerializeField] List<PlatformConfig> rightWallConfigs = new List<PlatformConfig>(4);

    [Header("Obstacles")]
    [SerializeField] PlatformConfig[] obstacleConfigs;
    [SerializeField] List<Vector3> obstaclePositions;
    [SerializeField] List<Vector3> obstacleScales;
    [SerializeField] List<Vector3> obstacleRotations;
    [Tooltip("0 to indicate it is not breakable")][SerializeField] List<int> breakCount;

    [Header("Cannon")]
    [SerializeField] Vector3 cannonPosition;
    [SerializeField] List<Vector2> correctCannonAngles;
    [SerializeField] float angleTolerance;

    [Header("Target")]
    [SerializeField] Vector3 targetPosition;

    [Header("MagneticFields")]
    [SerializeField] List<Vector3> magneticFieldPositions;
    [SerializeField] List<float> magneticFieldScales;
    [SerializeField] List<float> magneticFieldPowers;

    [Header("Boosts")]
    [SerializeField] List<Bonus.boosts> boosts;
    [SerializeField] List<Vector3> boostPositions;

    public List<PlatformConfig> GetLeftWallConfigs() {
        return leftWallConfigs;
    }

    public List<PlatformConfig> GetRightWallConfigs() {
        return rightWallConfigs;
    }

    public PlatformConfig[] GetObstacleConfig() {
        return obstacleConfigs;
    }

    public List<Vector3> GetPositions() {
        return obstaclePositions;
    }

    public Vector3 GetCannonPosition() {
        return cannonPosition;
    }

    public Vector3 GetTargetPosition() {
        return targetPosition;
    }

    public List<Vector3> GetObstacleScales() {
        return obstacleScales;
    }

    public List<Vector3> GetObstacleRotations() {
        return obstacleRotations;
    }
    public List<Vector3> GetMagneticFieldPositions() {
        return magneticFieldPositions;
    }

    public List<float> GetMagneticFieldScales() {
        return magneticFieldScales;
    }

    public List<float> GetMagneticFieldPowers() {
        return magneticFieldPowers;
    }

    public List<int> GetBreakCount() {
        return breakCount;
    }

    public bool DependsOnPrevBallCount() {
        return dependsOnPrevBallCount;
    }

    public bool GetSolidLeftWall() {
        return solidLeftWall;
    }

    public bool GetSolidRightWall() {
        return solidRightWall;
    }

    public PlatformConfig GetSolidLeftWallConfig() {
        return solidLeftWallConfig;
    }

    public PlatformConfig GetSolidRightWallConfig() {
        return solidRightWallConfig;
    }

    public List<Bonus.boosts> GetBoosts() {
        return boosts;
    }

    public List<Vector3> GetBoostPositions() {
        return boostPositions;
    }

    public List<Vector2> GetCorrectCannonAngles() {
        return correctCannonAngles;
    }

    public float GetAngleTolerance() {
        return angleTolerance;
    }
}
