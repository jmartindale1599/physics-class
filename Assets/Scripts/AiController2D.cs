using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Search;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class AiController2D : MonoBehaviour, IDamagable{

	[SerializeField] Animator animator;

	[SerializeField] float speed;
	
	[SerializeField] float jumpHeight;

	[SerializeField] float doubleJumpHeight;

    [SerializeField, Range(1, 5)] float fallRateMultiplier;

    [SerializeField, Range(1, 5)] float lowJumpRateMultiplier;

    [Header("Ground")]

    [SerializeField] Transform groundTransform;
    
	[SerializeField] LayerMask groundLayerMask;

	[SerializeField] LayerMask raycastLayerMask;

    Rigidbody2D rb;

	[SerializeField] SpriteRenderer spriteRenderer;

	[SerializeField] Transform[] waypoints;

	[SerializeField] string enemyTag;

	[Header("AI")]

	[SerializeField, Range(1,5)] float rayDistance;

	[SerializeField] float Health;

	Vector2 velocity = Vector2.zero;

	bool faceRight = true;

	GameObject enemy;

	enum State { Idle, Patrol, Chase ,Attack}

	State state = State.Idle;

	float stateTimer = 1;

	Transform targetPoint = null;

	void Start(){

		rb = GetComponent<Rigidbody2D>();
		
	}

	void Update(){

		CheckEnemySeen();

        Vector2 direction = Vector2.zero;

		//direction.x = Input.GetAxis("Horizontal");

		//Do Ai things

		switch (state){

			case State.Idle:

                if (enemy != null) state = State.Chase;

                stateTimer -= Time.deltaTime;

				if (stateTimer <= 0){

					setNewPoint();

					state = State.Patrol;

				}

					break;

			case State.Patrol:
				
				{

					if (enemy != null) state = State.Chase;

					direction.x = Mathf.Sign(targetPoint.position.x - transform.position.x);

					float dx = Mathf.Abs(transform.position.x - targetPoint.position.x);

					if (dx <= 0.25f){

						state = State.Idle;

						stateTimer = 1;

					}
				
				}

                break;

			case State.Chase:
				
				{

					if (enemy == null){

						state = State.Idle;

						stateTimer = 1;

						break;

					}

					float dx = Mathf.Abs(enemy.transform.position.x - transform.position.x);

					if (dx <= 1f){

						state = State.Attack;

						animator.SetTrigger("Attack");

					}else{

						direction.x = Mathf.Sign(enemy.transform.position.x - transform.position.x);

					}

				}

                break;
			
			case State.Attack:

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0)){

                    state = State.Chase;
                
				}

                break;

			default:

			break;
		
		}

		bool onGround = Physics2D.OverlapCircle(groundTransform.position, 0.1f, groundLayerMask) != null;
		
		// set velocity

        velocity.x = direction.x * speed;
		
		if (onGround){

            if (velocity.y < 0) velocity.y = 0;

			if (Input.GetButtonDown("Jump")){ 
			
				velocity.y += Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);

				animator.SetTrigger("jump");

                StartCoroutine(DoubleJump());

            }

		}

		velocity.y += Physics.gravity.y * Time.deltaTime;

        // adjust gravity for jump

        float gravityMultiplier = 1;
        
		if (!onGround && velocity.y < 0) gravityMultiplier = fallRateMultiplier;

		if (!onGround && velocity.y > 0 && !Input.GetButton("Jump")) gravityMultiplier = lowJumpRateMultiplier;

		velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

        // move character

		rb.velocity = velocity;

		//flip character to right dir

		if (velocity.x > 0 && !faceRight) flip();

		if (velocity.x < 0 && faceRight) flip();

		//update animator

		animator.SetFloat("speed", Mathf.Abs(velocity.x));

	}

    IEnumerator DoubleJump(){

        // wait a little after the jump to allow a double jump
        
		yield return new WaitForSeconds(0.01f);
        
		// allow a double jump while moving up
        
		while (velocity.y > 0){

            // if "jump" pressed add jump velocity
            
			if (Input.GetButtonDown("Jump")){

                velocity.y += Mathf.Sqrt(doubleJumpHeight * -2 * Physics.gravity.y);

				animator.SetTrigger("jump");

                break;
            
			}

            yield return null;
        
		}

    }

	private void flip(){

		faceRight = !faceRight;

		spriteRenderer.flipX = !faceRight;	
	
	}

	void setNewPoint() {

		Transform wp = null;

		do{

			wp = waypoints[Random.Range(0, waypoints.Length)];

		} while (wp == targetPoint);

		targetPoint = wp;
	
	}

	private bool seesPlayer(){

        RaycastHit2D rayHit = Physics2D.Raycast(transform.position,((faceRight) ? Vector2.right : Vector2.left), rayDistance);

        Debug.DrawRay(transform.position, ((faceRight) ? Vector2.right : Vector2.left), Color.red);

		return rayHit.collider != null && rayHit.collider.CompareTag("Player");

    }

    private void CheckEnemySeen(){

        enemy = null;
        
		RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, ((faceRight) ? Vector2.right : Vector2.left), rayDistance, raycastLayerMask);
        
		if (raycastHit.collider != null && raycastHit.collider.gameObject.CompareTag(enemyTag)){

            enemy = raycastHit.collider.gameObject;
            
			Debug.DrawRay(transform.position, ((faceRight) ? Vector2.right : Vector2.left) * rayDistance, Color.red);
        
		}

    }

    public void Damage(int damage){

        Health-= damage;

		print(Health);

    }

}
