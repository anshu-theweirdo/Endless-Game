using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed;
    public float maxSpeed;

    private int desiredLane = 1; //0:left 1:middle 2:right
    public float laneDistance = 4; //the distance between two lanes

    public float jumpForce;
    public float Gravity = -20;
    public Transform groundCheck;
    public LayerMask groundLayer;

    public Animator animator;
    private bool isSliding = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
      if (!PlayerManager.isGameStarted)
          return;

      //increse speed
      if (forwardSpeed < maxSpeed)    
      forwardSpeed += 0.1f * Time.deltaTime;

      animator.SetBool("isGameStarted", true);    

      direction.z = forwardSpeed;

      bool isGrounded = Physics.CheckSphere(groundCheck.position, 0.3f, groundLayer);
      animator.SetBool("isGrounded", isGrounded);
      
      if (controller.isGrounded)
      {
        
        if (SwipeManager.swipeUp)
        
          Jump();
      }
      else
      {
        direction.y += Gravity * Time.deltaTime; 
      }
      if (SwipeManager.swipeDown && !isSliding)
      {
        StartCoroutine(Slide());
      }

      //Gather the input on which lane we should be

      if (SwipeManager.swipeRight)
      {
          desiredLane++;
          if (desiredLane == 3)
              desiredLane = 2;
      } 

      if (SwipeManager.swipeLeft)
      {
          desiredLane--;
          if (desiredLane == -1)
              desiredLane = 0;
      } 

      //Calculate where we should be in the future
      Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
      if (desiredLane == 0)
      {
        targetPosition += Vector3.left * laneDistance;
      }
      else if (desiredLane == 2)
      {
        targetPosition += Vector3.right * laneDistance;
      }

      if (transform.position != targetPosition)
          {
            Vector3 diff = targetPosition - transform.position;
      Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
      if (moveDir.sqrMagnitude < diff.sqrMagnitude)
          controller.Move(moveDir);
      else
          controller.Move(diff);
          }
    
    //Move Player
        controller.Move(direction * Time.deltaTime);

    }
    private void Jump()
    {

         direction.y = jumpForce;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
      if (hit.transform.tag == "Obstacle")
      {
        PlayerManager.gameOver = true;
        Object.FindFirstObjectByType<AudioManager>().PlaySound("GameOver");
      }
    }

    private IEnumerator Slide()
    {
      isSliding = true;
      animator.SetBool("isSliding", true);
      controller.center = new Vector3(0, -0.5f, 0);
      controller.height = 1;

      yield return new WaitForSeconds(1.3f);
      controller.center = new Vector3(0, 0, 0);
      controller.height = 2;

      animator.SetBool("isSliding", false);
      isSliding = false;

    }
}
