using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Search;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class AiController2D : MonoBehaviour{

	[SerializeField] Animator animator;

	[SerializeField] float speed;
	
	[SerializeField] float jumpHeight;

	[SerializeField] float doubleJumpHeight;

    [SerializeField, Range(1, 5)] float fallRateMultiplier;

    [SerializeField, Range(1, 5)] float lowJumpRateMultiplier;

    [Header("Ground")]

    [SerializeField] Transform groundTransform;
    
	[SerializeField] LayerMask groundLayerMask;

	Rigidbody2D rb;

	[SerializeField] SpriteRenderer spriteRenderer;

	[SerializeField] Transform[] waypoints;

	[Header("AI")]

	[SerializeField, Range(1,5)] float rayDistance;

	Vector2 velocity = Vector2.zero;

	bool faceRight = true;

	enum State { Idle, Patrol, Chase ,Attack}

	State state = State.Idle;

	float stateTimer = 5;

	Transform targetPoint = null;

	void Start(){

		rb = GetComponent<Rigidbody2D>();

	}

	void Update(){

        Vector2 direction = Vector2.zero;

		//direction.x = Input.GetAxis("Horizontal");

		//Do Ai things

		switch (state){

			case State.Idle:

                if (seesPlayer()) state = State.Chase;

                stateTimer += Time.deltaTime;

				if (stateTimer > 0.5f){

					setNewPoint();

					state = State.Patrol;

				}

					break;

			case State.Patrol:

				if (seesPlayer()) state = State.Chase;

				direction.x = Mathf.Sign(targetPoint.position.x - transform.position.x);

				float dx = Mathf.Abs(transform.position.x - targetPoint.position.x);

				if(dx <= 0.25f){

					state = State.Idle;

					stateTimer = 5;
														
				}

                break;

			case State.Chase:



				break;
			
			case State.Attack:

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

}
