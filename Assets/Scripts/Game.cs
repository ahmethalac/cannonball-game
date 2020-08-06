using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour {
    [SerializeField] List<LevelConfig> levels = new List<LevelConfig>();
    [SerializeField] float movementSpeedOfPlatforms = 0.01f;
    [SerializeField] Cannon cannonPrefab;
    [SerializeField] Target targetPrefab;
    [SerializeField] GameObject bonusPrefab;
    [SerializeField] GameObject magneticFieldPrefab;
    [SerializeField] Dot dotPrefab;
    [SerializeField] GameObject hintArrow;
    [Range(0,3)] [Tooltip("How many seconds cannon will wait to start shooting")] [SerializeField] float barSecond;

    [SerializeField] List<GameObject> platforms = new List<GameObject>();
    bool prevPlatformsDestroyed = false;
    [SerializeField] int currentLevel;
    Text levelText;
    private readonly int segmentCount = 15;
    private Collider2D hitObject;
    private readonly float segmentScale = 0.5f;
    private readonly Vector2 gravity = new Vector3(0,-9.81f);
    private Coroutine cannonCoroutine;

    [SerializeField] public List<Vector2> correctAngles;
    [SerializeField] public float angleTolerance;

    private void Start() {
        LoadLevel(levels[GameOver.checkpoint - 1]);
        currentLevel = GameOver.checkpoint;
        InitiateText();
    }

    private GameObject InstantiatePlatform(PlatformConfig config,Vector3 position,Vector3 scale) {
        GameObject platform = new GameObject(config.GetName());

        platform.AddComponent(typeof(SpriteRenderer));
        platform.GetComponent<SpriteRenderer>().sprite = config.GetSprite();
        platform.GetComponent<SpriteRenderer>().color = new Color(config.GetColor()[0],config.GetColor()[1],config.GetColor()[2]);
        platform.transform.localScale = scale;

        platform.AddComponent(typeof(Rigidbody2D));
        platform.GetComponent<Rigidbody2D>().isKinematic = true;

        platform.AddComponent(typeof(BoxCollider2D));

        if (config.GetFixedCollisionEffect()) {
            platform.AddComponent(typeof(FixedCollision));
            platform.GetComponent<FixedCollision>().SetSpecificTarget(config.IsSpecificTarget());
            platform.GetComponent<FixedCollision>().SetTargetOrAngle(config.GetTargetOrAngle());
            platform.GetComponent<BoxCollider2D>().isTrigger = true;
        }
        else {
            platform.GetComponent<Rigidbody2D>().sharedMaterial = new PhysicsMaterial2D {
                bounciness = config.GetBounceFactor(),
                friction = 0
            };
        }

        platform.transform.position = position;

        if (config.GetMovement()) {
            platform.AddComponent(typeof(MovingObstacle));
            platform.GetComponent<MovingObstacle>().Stop();
            platform.GetComponent<MovingObstacle>().SetSpeed(config.GetSpeed());
            platform.GetComponent<MovingObstacle>().SetCenter(config.GetCenter());
            platform.GetComponent<MovingObstacle>().SetAmplitude(config.GetAmplitude());
            platform.GetComponent<MovingObstacle>().SetStopAtMaximum(config.GetStopAtMaximum());
            platform.GetComponent<MovingObstacle>().SetMovementInY(config.GetMovementInY());
            platform.GetComponent<MovingObstacle>().SetToLeft(config.GetToLeft());
        }

        return platform;
    }

    private void CreateAllWalls(LevelConfig config) {
        if (!config.GetSolidRightWall()) {
            for (float i = 2 ; i < 6 ; i++) {
                var temp = InstantiatePlatform(config.GetRightWallConfigs()[(int)i - 2],
                                                  new Vector3(4.375f,i * 4,0),
                                                  Vector3.one);
                temp.GetComponent<SpriteRenderer>().sortingLayerName = "Wall";
                platforms.Add(temp);
            }
        }
        else {
            var temp = InstantiatePlatform(config.GetSolidRightWallConfig(),
                                           new Vector3(4.375f,16,0),
                                           Vector3.one);
            temp.GetComponent<SpriteRenderer>().sortingLayerName = "Wall";
            platforms.Add(temp);
        }

        if (!config.GetSolidLeftWall()) {
            for (float i = 2 ; i < 6 ; i++) {
                var temp = InstantiatePlatform(config.GetLeftWallConfigs()[(int)i - 2],
                                                  new Vector3(-4.375f,i * 4,0),
                                                  Vector3.one);
                temp.GetComponent<SpriteRenderer>().sortingLayerName = "Wall";
                platforms.Add(temp);
            }
        }
        else {
            var temp = InstantiatePlatform(config.GetSolidLeftWallConfig(),
                               new Vector3(-4.375f,16,0),
                               Vector3.one);
            temp.GetComponent<SpriteRenderer>().sortingLayerName = "Wall";
            platforms.Add(temp);
        }
    }

    private void CreateObstacles(LevelConfig config) {
        for (int i = 0 ; i < config.GetPositions().Count ; i++) {
            var temp = InstantiatePlatform(config.GetObstacleConfig()[i],
                                              config.GetPositions()[i] + new Vector3(0,16,0),
                                              config.GetObstacleScales()[i]);
            temp.GetComponent<SpriteRenderer>().sortingLayerName = "Obstacle";
            temp.transform.rotation = Quaternion.Euler(config.GetObstacleRotations()[i].x,
                                                       config.GetObstacleRotations()[i].y,
                                                       config.GetObstacleRotations()[i].z);
            if (config.GetBreakCount()[i] != 0) {
                temp.AddComponent(typeof(Breakable));
                if (!config.DependsOnPrevBallCount()) {
                    temp.GetComponent<Breakable>().SetBreakCount(config.GetBreakCount()[i]);
                }
                else {
                    temp.GetComponent<Breakable>().SetBreakCount(( FindObjectOfType<Target>().GetCount() / 3 ) + 1);
                }
            }
            platforms.Add(temp);
        }
    }

    private IEnumerator Move(GameObject gameObject,int prevPlatformCount,LevelConfig level) {

        if (gameObject == null) {
            yield return new WaitForEndOfFrame();
        }
        else {
            var expectedYPosition = gameObject.transform.position.y - 2 * Camera.main.orthographicSize + 0.01f;
            while (gameObject.transform.position.y >= expectedYPosition) {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position,
                                                                     gameObject.transform.position - new Vector3(0,2 * Camera.main.orthographicSize,0),
                                                                     movementSpeedOfPlatforms / 2);
                yield return new WaitForEndOfFrame();
                if (gameObject == null) {
                    break;
                }
            }
        }

        if (prevPlatformCount != 0) {
            DestroyPrevPlatforms(prevPlatformCount);
            prevPlatformsDestroyed = true;
        }
        foreach (GameObject platform in platforms) {
            if (platform.GetComponent<MovingObstacle>()) {
                platform.GetComponent<MovingObstacle>().StartMoving();
            }
        }

        if (FindObjectOfType<Target>() == null) {
            Instantiate(targetPrefab,level.GetTargetPosition(),Quaternion.identity);
        }

    }

    private void DestroyPrevPlatforms(int count) {
        if (!prevPlatformsDestroyed) {
            for (int i = 0 ; i < count ; i++) {
                Destroy(platforms[i]);
            }
            platforms.RemoveRange(0,count);
        }
    }

    private IEnumerator MoveCannon() {
        var cannon = FindObjectOfType<Cannon>();
        while (cannon.transform.position.y >= -8) {
            cannon.transform.position = Vector3.MoveTowards(cannon.transform.position,
                                                                 new Vector3(cannon.transform.position.x,-8,0),
                                                                 movementSpeedOfPlatforms / 2);
            yield return new WaitForEndOfFrame();
        }
    }
    private void LoadLevel(LevelConfig level) {
        correctAngles = level.GetCorrectCannonAngles();
        angleTolerance = level.GetAngleTolerance();

        FindObjectOfType<CanvasManipulator>().NewLevel();

        if (cannonCoroutine != null) {
            StopCoroutine(cannonCoroutine);
        }

        if (FindObjectsOfType<Ball>().Length != 0) {
            foreach (Ball ball in FindObjectsOfType<Ball>()) {
                Destroy(ball.gameObject);
            }
        }

        if (FindObjectOfType<Cannon>() == null) {
            Instantiate(cannonPrefab,level.GetCannonPosition(),Quaternion.identity);
        }


        prevPlatformsDestroyed = false;
        int prevPlatforms = platforms.Count;

        var magneticFieldPositions = level.GetMagneticFieldPositions();
        if (magneticFieldPositions.Count != 0) {
            for (int i = 0 ; i < magneticFieldPositions.Count ; i++) {
                var mf = Instantiate(magneticFieldPrefab,level.GetMagneticFieldPositions()[i] + new Vector3(0,16,0),Quaternion.identity);
                mf.transform.localScale *= level.GetMagneticFieldScales()[i];
                mf.GetComponent<MagneticField>().SetPower(level.GetMagneticFieldPowers()[i]);
                mf.GetComponent<MagneticField>().SetTurnSpeed(level.GetMagneticFieldScales()[i]);
                platforms.Add(mf);
            }
        }

        var boostPositions = level.GetBoostPositions();
        if (boostPositions.Count != 0) {
            for (int i = 0 ; i < boostPositions.Count ; i++) {
                var boost = Instantiate(bonusPrefab,level.GetBoostPositions()[i] + new Vector3(0,16,0),Quaternion.identity);
                boost.GetComponent<Bonus>().SetBoost(level.GetBoosts()[i]);
                platforms.Add(boost);
            }
        }
        CreateAllWalls(level);

        CreateObstacles(level);

        if (FindObjectOfType<Target>()) {
            if (FindObjectOfType<Cannon>()) {
                FindObjectOfType<Cannon>().gameObject.transform.position = level.GetCannonPosition() + new Vector3(0,16,0);
            }
            Destroy(FindObjectOfType<Target>().gameObject);
        }

        foreach (GameObject element in platforms) {
            StartCoroutine(Move(element,prevPlatforms,level));
        }

        if (FindObjectOfType<Cannon>().gameObject.transform.position.y > -4) {
            cannonCoroutine = StartCoroutine(MoveCannon());
        }

        FindObjectOfType<Cannon>().StopMoving();

    }

    public void CreateAgain(Vector3 position,Bonus.boosts bonus) {
        StartCoroutine(CreateBonusAgain(position,bonus));
    }

    public IEnumerator CreateBonusAgain(Vector3 position,Bonus.boosts bonus) {
        int currLevel = currentLevel;
        yield return new WaitForSeconds(10f);
        if (currLevel == currentLevel) {
            var boost = Instantiate(bonusPrefab,position,Quaternion.identity);
            boost.GetComponent<Bonus>().SetBoost(bonus);
            platforms.Add(boost);
        }
    }
    public void LoadNextLevel() {
        if (currentLevel == levels.Count) {
            Score.score = FindObjectOfType<Target>().GetCount();
            GameOver.checkpoint = 1;
            SceneManager.LoadScene("Congratulations");
        }
        else {
            LoadLevel(levels[currentLevel]);
            currentLevel++;
            if (currentLevel % 5 == 0 && currentLevel != 30) {
                FindObjectOfType<CanvasManipulator>().CreateText("Checkpoint " + ( currentLevel / 5 ) + "!");
                GameOver.checkpoint = currentLevel;
            }
        }
    }

    private void Update() {
        //levelText.text = "Level " + ( currentLevel );
        if (Input.GetKeyDown(KeyCode.Space)) {
            LoadNextLevel();
        }
        if (FindObjectOfType<Cannon>() && FindObjectOfType<Cannon>().gameObject.transform.position.y != -8) {

            foreach (Dot dot in FindObjectsOfType<Dot>()) {
                Destroy(dot.gameObject);
            }
        }
    }

    private void FixedUpdate() {
        if (FindObjectOfType<Cannon>()) {
            GetComponent<LineRenderer>().enabled = true;
            SimulatePath();
        }
        else {
            if (FindObjectOfType<Dot>()) {
                foreach (Dot element in FindObjectsOfType<Dot>()) {
                    Destroy(element.gameObject);
                }
            }
        }
    }

    private void InitiateText() {
        foreach (Text element in FindObjectsOfType<Text>()) {
            if (element.name == "Level") {
                levelText = element;
            }
        }
        levelText.enabled = true;
        levelText.text = "Level " + ( currentLevel );
    }

    public void StopObstacles(int count) {
        foreach (GameObject element in platforms) {
            if (element.GetComponent<MovingObstacle>()) {
                StartCoroutine(Stop(element,count));
            }
        }
    }

    private IEnumerator Stop(GameObject obj,int count) {
        var temp = obj.GetComponent<MovingObstacle>().GetSpeed();
        obj.GetComponent<MovingObstacle>().SetSpeed(0f);
        yield return new WaitForSeconds(count);
        if (obj != null) {
            obj.GetComponent<MovingObstacle>().SetSpeed(temp);
        }
    }

    public void Explode(Vector3 position,float bombRange) {
        List<GameObject> objects = new List<GameObject>();

        foreach (GameObject element in platforms) {
            if (element.GetComponent<Breakable>()) {
                if (Vector3.Distance(element.transform.position,position) <= bombRange) {
                    objects.Add(element);
                }
            }
        }

        foreach (GameObject element in objects) {
            Destroy(element);
            platforms.Remove(element);
        }
    }

    public void CreateHintArrow() {
        GameObject temp;
        if (currentLevel == 18) {
            temp = Instantiate(hintArrow,new Vector3(-3.02f,-3.47f,0),Quaternion.identity);
        }
        else if (currentLevel == 23) {
            temp = Instantiate(hintArrow,new Vector3(-1.572f,0.483f,0),Quaternion.identity);
        }
        else {
            temp = new GameObject();
        }
        platforms.Add(temp);
    }
    private void SimulatePath() {
        Vector2[] segments = new Vector2[segmentCount];

        // The first line point is wherever the player's cannon, etc is
        segments[0] = FindObjectOfType<Cannon>().transform.up + FindObjectOfType<Cannon>().transform.position;

        // The initial velocity
        Vector2 segVelocity = FindObjectOfType<Cannon>().transform.up * FindObjectOfType<Cannon>().GetBallSpeed();
        // reset our hit object
        hitObject = null;
        for (int i = 1 ; i < segmentCount ; i++) {
            // Time it takes to traverse one segment of length segScale (careful if velocity is zero)
            float segTime = ( segVelocity.sqrMagnitude != 0 ) ? segmentScale / segVelocity.magnitude : 0;

            // Add velocity from gravity for this segment's timestep
            segVelocity += gravity * segTime;

            // Check to see if we're going to hit a physics object
            if (Physics2D.Raycast(segments[i - 1],segVelocity,segmentScale) &&
                Physics2D.Raycast(segments[i - 1],segVelocity,segmentScale).collider.gameObject.tag != "Ball" &&
                Physics2D.Raycast(segments[i - 1],segVelocity,segmentScale).collider.gameObject.tag != "Target" &&
                Physics2D.Raycast(segments[i - 1],segVelocity,segmentScale).collider.gameObject.tag != "Shredder") {
                var hit = Physics2D.Raycast(segments[i - 1],segVelocity,segmentScale);

                // remember who we hit
                hitObject = hit.collider;

                // set next position to the position where we hit the physics object
                segments[i] = segments[i - 1] + segVelocity.normalized * ( hit.distance - 0.01f );
                // correct ending velocity, since we didn't actually travel an entire segment
                segVelocity = segVelocity - gravity * ( segmentScale - hit.distance ) / segVelocity.magnitude;
                // flip the velocity to simulate a bounce
                if (hitObject.gameObject.GetComponent<FixedCollision>()) {
                    bool specificOrAngle = hitObject.gameObject.GetComponent<FixedCollision>().GetSpecificTargetOrFixedAngle();
                    Vector2 targetOrAngle = hitObject.gameObject.GetComponent<FixedCollision>().GetTargetOrAngle();
                    if (specificOrAngle) {
                        segVelocity = segVelocity.magnitude * ( targetOrAngle - (Vector2)hitObject.transform.position ).normalized;
                    }
                    else {
                        segVelocity = segVelocity.magnitude * targetOrAngle.normalized;
                    }
                }
                else if (hitObject.gameObject.GetComponent<Rigidbody2D>().sharedMaterial == null) {
                    segVelocity = Vector3.Reflect(segVelocity,hit.normal);
                }
                else if (hitObject.gameObject.GetComponent<Rigidbody2D>().sharedMaterial.bounciness != 0) {
                    segVelocity = Vector3.Reflect(segVelocity,hit.normal);
                }
                else {
                    segVelocity = new Vector2(0,segVelocity.y);
                }

                /*
				 * Here you could check if the object hit by the Raycast had some property - was 
				 * sticky, would cause the ball to explode, or was another ball in the air for 
				 * instance. You could then end the simulation by setting all further points to 
				 * this last point and then breaking this for loop.
				 */
            }
            // If our raycast hit no objects, then set the next position to the last one plus v*t
            else {
                segments[i] = segments[i - 1] + segVelocity * segTime;
            }
        }

        segments[0] = segments[1];
        if (FindObjectOfType<Dot>()) {
            foreach (Dot element in FindObjectsOfType<Dot>()) {
                Destroy(element.gameObject);
            }
        }
        for (int i = 0 ; i < segments.Length ; i++) {
            Dot dot = Instantiate(dotPrefab,segments[i],Quaternion.identity);
            dot.GetComponent<SpriteRenderer>().color = new Color(0,0,0,1 - ( i * 1f / segments.Length ));
        }

    }

    public void Finish() {
        SceneManager.LoadScene("GameOver");
    }

    public void DestroyPlatform(GameObject gameObject) {
        platforms.Remove(gameObject);
    }

    public void RemoveObstacle(GameObject item) {
        platforms.Remove(item);
    }

    public float GetBarSecond() {
        return barSecond;
    }
}
