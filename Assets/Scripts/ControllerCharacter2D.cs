using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class ControllerCharacter2D : MonoBehaviour{

	[SerializeField] Animator animator;

	[SerializeField] float speed;
	
	[SerializeField] float turnRate;
	
	[SerializeField] float jumpHeight;

	[SerializeField] float doubleJumpHeight;

	//[SerializeField, Range(0,1)] float waitJumpTime;

	[SerializeField] float hitForce;

    [SerializeField, Range(1, 5)] float fallRateMultiplier;

    [SerializeField, Range(1, 5)] float lowJumpRateMultiplier;

    [Header("Ground")]

    [SerializeField] Transform groundTransform;
    
	[SerializeField] LayerMask groundLayerMask;

	Rigidbody2D rb;

	[SerializeField] SpriteRenderer spriteRenderer;
	
	Vector2 velocity = Vector2.zero;

	bool faceRight = true;

	void Start(){

		rb = GetComponent<Rigidbody2D>();
	
	}

	void Update(){

        bool onGround = Physics2D.OverlapCircle(groundTransform.position, 0.02f, groundLayerMask) != null;

        // get direction input

        Vector2 direction = Vector2.zero;
		
		direction.x = Input.GetAxis("Horizontal");
		
		// set velocity
		
		if (onGround){

            velocity.x = direction.x * speed;

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

}
