using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class ControllerCharacter2D : MonoBehaviour, IDamagable{

	[SerializeField] Animator animator;

	[SerializeField] float speed;
	
	[SerializeField] float jumpHeight;

	[SerializeField] float doubleJumpHeight;

	[SerializeField] float groundAngle;

	[SerializeField] float groundRadius;

    [SerializeField, Range(1, 5)] float fallRateMultiplier;

    [SerializeField, Range(1, 5)] float lowJumpRateMultiplier;

    [Header("Ground")]

    [SerializeField] Transform groundTransform;
    
	[SerializeField] LayerMask groundLayerMask;

	Rigidbody2D rb;

	[SerializeField] SpriteRenderer spriteRenderer;

	[Header("Attack")]

	[SerializeField] Transform attackTransfrom;
	
	[SerializeField] float attackRad;

	int Hits = 3; // how long until it dies

	Vector2 velocity = Vector2.zero;

	bool faceRight = true;

    enum State { NoHit, Hit1, Hit2, Death }

    State state = State.NoHit;

    void Start(){

		rb = GetComponent<Rigidbody2D>();

	}

	void Update(){

		switch (state){ 
		
			case State.Hit1:

				//lives -- for screen or whatever i physically and mentally don't give a damn

				break;

			case State.Hit2:

				//idk increase speed and attack or something ffs

				break;

			case State.Death:

                //death

                animator.SetTrigger("Death");

                break;

		
		}

		bool onGround = Physics2D.OverlapCircle(groundTransform.position, 0.1f, groundLayerMask) != null;

        // get direction input

        Vector2 direction = Vector2.zero;
		
		direction.x = Input.GetAxis("Horizontal");
		
		// set velocity

        velocity.x = direction.x * speed;
		
		if (onGround){

            if (velocity.y < 0) velocity.y = 0;

			if (Input.GetButtonDown("Jump")){ 
			
				velocity.y += Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);

				animator.SetTrigger("Jump");

                StartCoroutine(DoubleJump());

            }

			if (Input.GetMouseButtonDown(0)){

				animator.SetTrigger("Attack");

				CheckAttack();

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

		animator.SetFloat("Speed", Mathf.Abs(velocity.x));

	}

    private bool UpdateGroundCheck(){

        // check if the character is on the ground
        
		Collider2D collider = Physics2D.OverlapCircle(groundTransform.position, groundRadius, groundLayerMask);
        
		if (collider != null){

            RaycastHit2D raycastHit = Physics2D.Raycast(groundTransform.position, Vector2.down, groundRadius, groundLayerMask);
            
			if (raycastHit.collider != null){

                // get the angle of the ground (angle between up vector and ground normal)
                
				groundAngle = Vector2.SignedAngle(Vector2.up, raycastHit.normal);
                
				Debug.DrawRay(raycastHit.point, raycastHit.normal, Color.red);

            }

        }

        return (collider != null);

    }

    IEnumerator DoubleJump(){

        // wait a little after the jump to allow a double jump
        
		yield return new WaitForSeconds(0.01f);
        
		// allow a double jump while moving up
        
		while (velocity.y > 0){

            // if "jump" pressed add jump velocity
            
			if (Input.GetButtonDown("Jump")){

                velocity.y += Mathf.Sqrt(doubleJumpHeight * -2 * Physics.gravity.y);

                animator.SetTrigger("Jump");

                break;
            
			}

            yield return null;
        
		}

    }

	private void flip(){

		faceRight = !faceRight;

		spriteRenderer.flipX = !faceRight;	
	
	}

    private void CheckAttack(){

        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackTransfrom.position, attackRad);
        
		foreach (Collider2D collider in colliders){

            if (collider.gameObject == gameObject) continue;

            if (collider.gameObject.TryGetComponent<IDamagable>(out var damagable)){

                damagable.Damage(10);

            }

        }

    }

    public void Damage(int damage){

		Hits--;

		if (Hits == 2) { state = State.Hit1; }

		if (Hits == 1) { state = State.Hit2; }
		
		if (Hits == 0) { state = State.Death; }
    
	}

}
