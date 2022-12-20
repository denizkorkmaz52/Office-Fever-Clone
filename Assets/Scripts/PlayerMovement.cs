using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerMovement : MonoBehaviour
{

    public int moveSpeed;
    public int rotationSpeed;
    public Transform orientation;
    public FloatingJoystick floatingJoystick;
    
    private Player playerSC;
    //[SerializeField] private float RotationSmoothTime = 0.12f;

    private bool isMoving = false;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        playerSC = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        //TakeMovementInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MovePlayer()
    {        
        Vector3 direction = -Vector3.right * floatingJoystick.Vertical + Vector3.forward * floatingJoystick.Horizontal;
        //rb.AddForce(moveSpeed * Time.fixedDeltaTime * direction, ForceMode.VelocityChange);
        float animationSpeed = Mathf.Abs(Mathf.Sqrt(Mathf.Pow(floatingJoystick.Vertical, 2) + Mathf.Pow(floatingJoystick.Horizontal, 2)));
        if (direction != new Vector3(0, 0, 0))
        {
            //Debug.Log(direction.x + "--" + direction.y + "--" + direction.z);
            rb.velocity = (moveSpeed * direction * Time.deltaTime);
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed);

            //transform.forward = direction;
            //Debug.Log(rb.velocity);
            isMoving = true;
        }
        else
            isMoving = false;
        
        AnimatePlayer(animationSpeed);

    }
    private void OnCollisionEnter(Collision collision)
    {
        DOTween.KillAll();
    }

    private void AnimatePlayer(float animationSpeed)
    {
        playerSC.SetAnimVar("SpeedMultiplier", animationSpeed);
        if (isMoving && playerSC.ShowPaperCount() == 0)
        {
            playerSC.SetAnimVar("Walking", true);
            playerSC.SetAnimVar("Carrying", false);
            playerSC.SetAnimVar("Idle", false);
        }
        else if (isMoving && playerSC.ShowPaperCount() != 0)
        {
            playerSC.SetAnimVar("Walking", true);
            playerSC.SetAnimVar("Carrying", true);
            playerSC.SetAnimVar("Idle", false);
        }
        else if (!isMoving && playerSC.ShowPaperCount() == 0)
        {
            playerSC.SetAnimVar("Idle", true);
            playerSC.SetAnimVar("Carrying", false);
            playerSC.SetAnimVar("Walking", false);
        }
        else if (!isMoving && playerSC.ShowPaperCount() != 0)
        {
            playerSC.SetAnimVar("Idle", true);
            playerSC.SetAnimVar("Carrying", true);
            playerSC.SetAnimVar("Walking", false);
        }

    }

    


}
