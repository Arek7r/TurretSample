using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleMove : MonoBehaviour
{
    #region Variables

    public CharacterController controller;
    public float playerSpeed = 2.0f;
    private float gravityValue = -9.81f;
   
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private string horizontal = "Horizontal";
    private string vertical = "Vertical";
    
    #endregion
    
    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(Input.GetAxis(horizontal), 0, Input.GetAxis(vertical));
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
