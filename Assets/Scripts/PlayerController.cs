using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 10.0f;
    [SerializeField]
    private float mouseSens = 10.0f;
    [SerializeField]
    private float maxCamAngle = 70.0f;
    [SerializeField]
    private float gravity = 1.0f;
    [SerializeField]
    private float jumpForce = 10.0f;
    [SerializeField]
    private float jumpDegress = 1.0f;

    private float currentJumpForce = 0;
    private CharacterController characterController;

    private void Start()
    {
        this.characterController = GetComponent<CharacterController>();
    }
    private void FixedUpdate()
    {
        float xMove = Input.GetAxis("Horizontal");
        float zMove = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 vectorMove;

        vectorMove = new Vector3(xMove, 0, zMove);

        vectorMove = vectorMove * movementSpeed;

        if (!characterController.isGrounded)
            vectorMove.y = -gravity;

        if(currentJumpForce >= 0)
        {
            currentJumpForce -= jumpDegress;
        }
        else
        {
            currentJumpForce += jumpDegress;
        }

        currentJumpForce = Mathf.Clamp(currentJumpForce, 0, jumpForce);

        if(Input.GetKey(KeyCode.Space))
        {
            currentJumpForce = jumpForce;
        }
        if(Input.GetKey(KeyCode.LeftShift))
        {
            currentJumpForce = -jumpForce;
        }

        if (currentJumpForce > 0)
            vectorMove.y = jumpForce;

        vectorMove = vectorMove * Time.deltaTime;

        characterController.Move(transform.rotation * vectorMove);


        transform.Rotate(Vector3.up, mouseX * mouseSens);

        Transform CamTransform = Camera.main.transform;
        Vector3 CamRotation = CamTransform.rotation.eulerAngles;

        CamRotation.x += -mouseY * mouseSens;

        CamRotation.x = Mathf.Clamp(CamRotation.x, 0, maxCamAngle);

        CamTransform.rotation = Quaternion.Euler(CamRotation);

    }
}
