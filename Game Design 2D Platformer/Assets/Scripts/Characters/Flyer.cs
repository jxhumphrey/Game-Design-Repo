using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*The functionality for flying enemies*/
public class Flyer : MonoBehaviour {

    [Header ("References")]
    private Rigidbody2D rigidbody2D;
    [SerializeField] private GameObject bomb;
    [System.NonSerialized] public EnemyBase enemyBase;
    private Transform lookAtTarget; //If I'm a bomb, I will point to a transform, like the player

    [Header ("Ground Avoidance")]
    [SerializeField] private float rayCastWidth = 5;
    [SerializeField] private float rayCastOffsetX = 1;
    [SerializeField] private float rayCastOffsetY = 1;
    [SerializeField] private LayerMask layerMask; //What will I be looking to avoid?
    private RaycastHit2D rayCastHit;
    private float distanceFromRightWall;
    private float distanceFromGround;

    [Header ("Flight")]
    [SerializeField] private float maxPositionY; //Flyer should not go past this position
    [SerializeField] private bool avoidGround; //Should I steer away from the ground?
    private Vector3 distanceFromPlayer;
    [SerializeField] private float maxSpeedDeviation;
    [SerializeField] private float easing = 1; //How intense should we ease when changing speed? The higher the number, the less air control!
    private float bombCounter = 0;
    [SerializeField] private float bombCounterMax = 2; //How many seconds before shooting another bomb?
    [SerializeField] private float bombCounterMaxCoinDecrement; //After how many coins will the Flyer shoot more frequently?
    [SerializeField] private float bombCounterMaxCoinDecrementAmount; //How much should bombCounterMax decrease after set amount of coins collected?
    public float attentionRange; //How far can I see?
    public float lifeSpan; //Keep at zero if you don't want to explode after a certain period of time.
    [System.NonSerialized] public float lifeSpanCounter;
    private bool sawPlayer = false; //Have I seen the player?
    [SerializeField] private float speedMultiplier;
    [System.NonSerialized] public Vector3 speed;
    [System.NonSerialized] public Vector3 speedEased;
    [SerializeField] private bool shootsBomb;
    [SerializeField] private Vector2 targetOffset = new Vector2(0, 2);

    // Use this for initialization
    void Start() {
        enemyBase = GetComponent<EnemyBase>();
        rigidbody2D = GetComponent<Rigidbody2D>();

        //Player is set as the target
        if (enemyBase.isBomb) {
            lookAtTarget = NewPlayer.Instance.gameObject.transform;
        }
        speed.x = 6;
        speed.y = 2;

        speedMultiplier += Random.Range(-maxSpeedDeviation, maxSpeedDeviation);
    }

    void OnDrawGizmosSelected() {
        // Draw a yellow sphere at the transform's position indicating the attentionRange
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attentionRange);
    }

    // Update is called once per frame
    void Update() {
      distanceFromPlayer.x = (NewPlayer.Instance.transform.position.x + targetOffset.x) - transform.position.x;
      distanceFromPlayer.y = (NewPlayer.Instance.transform.position.y + targetOffset.y) - transform.position.y;
      speedEased += (speed - speedEased) * Time.deltaTime * easing;
      transform.position += speedEased * Time.deltaTime;

      if (enemyBase.isBomb) {
        speed.x = (Mathf.Abs(distanceFromPlayer.x) / distanceFromPlayer.x) * speedMultiplier;
        speed.y = (Mathf.Abs(distanceFromPlayer.y) / distanceFromPlayer.y) * speedMultiplier;
      } else {
        if(NewPlayer.Instance.coins != 0 && NewPlayer.Instance.coins % 8 == 0 && bombCounterMax != 2) {
          bombCounterMax -= bombCounterMaxCoinDecrementAmount;
        }

        if (transform.position.y >= maxPositionY) {
          speed.y = -2;
        }

        if (Mathf.Abs(distanceFromPlayer.x) <= attentionRange && Mathf.Abs(distanceFromPlayer.y) <= attentionRange || lookAtTarget != null) {
              sawPlayer = true;
              speed.x = 6;

              if (!NewPlayer.Instance.frozen) {
                  if (shootsBomb) {
                      if (bombCounter > bombCounterMax) {
                          ShootBomb();
                          bombCounter = 0;
                      } else {
                          bombCounter += Time.deltaTime;
                      }
                  }
              } else {
                  speedEased = Vector3.zero;
              }
        } else {
          speed.x = 3;
        }

        // Check for walls and ground, adjust speed so always same distance away from groud/walls
        if (avoidGround) {
            rayCastHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right, rayCastWidth, layerMask);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y), Vector2.right * rayCastWidth, Color.yellow, 10f);

            //If object is blocking path to the right
            if (rayCastHit.collider != null) {
                //distanceFromRightWall = rayCastHit.distance;
                speed.x = speed.x / 2;
            }
            //If object is blocking path down
            rayCastHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.down, rayCastWidth, layerMask);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y), Vector2.down * rayCastWidth, Color.red, 10f);

            if (rayCastHit.collider != null) {
                speed.y = Mathf.Abs(speed.x);
            }

            //If object is blocking path to the left (may not need as is moving to constantly to the right)
            rayCastHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.left, rayCastWidth, layerMask);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y), Vector2.left * rayCastWidth, Color.blue, 10f);

            if (rayCastHit.collider != null) {
                speed.x = Mathf.Abs(speed.x);
            }
        }

        if (lookAtTarget != null) {
            LookAt2D();
        }

        if (lifeSpan != 0) {
            if (lifeSpanCounter < lifeSpan) {
                lifeSpanCounter += Time.deltaTime;
            } else {
                enemyBase.Die();
            }
        }
      }
    }

    void LookAt2D() {
        float angle = Mathf.Atan2(speedEased.y, speedEased.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
    }

    public void ShootBomb() {
        GameObject bombClone;
        bombClone = Instantiate(bomb, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, null);
    }
}
