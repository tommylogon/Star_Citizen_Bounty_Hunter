using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform shipTransform;
    public float forwardForce = 10f;
    public float strafeForce = 5f;
    private float brakingForce = 5f;
    private float maxSpeed = 10f;
    public float rotationSpeed = 2f;
    private bool isBraking = false;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        if (shipTransform == null)
        {
            shipTransform = transform.Find("PlayerShip");
        }

        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        AimShip();
        HandleMovement();

    }

    private void HandleMovement()
    {
        if (!isBraking)
        {

            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(shipTransform.right * forwardForce);
            }


            if (Input.GetKey(KeyCode.A))
            {
                rb.AddForce(shipTransform.up * strafeForce);
            }


            if (Input.GetKey(KeyCode.D))
            {
                rb.AddForce(-shipTransform.up * strafeForce);
            }


            if (Input.GetKey(KeyCode.S))
            {
                rb.AddForce(-shipTransform.right * strafeForce);
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            isBraking = true;
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            isBraking = false;
        }
    }

    private void FixedUpdate()
    {
        // Limit velocity
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);

        // Braking
        if (isBraking)
        {
            rb.AddForce(-rb.velocity.normalized * brakingForce);
        }
    }

    private void AimShip()
    {
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
        Vector3 aimDirectoin = (mousePosition - transform.position).normalized;

        float targetAngle = Mathf.Atan2(aimDirectoin.y, aimDirectoin.x) * Mathf.Rad2Deg;
        float deltaAngle = Mathf.DeltaAngle(shipTransform.eulerAngles.z, targetAngle);
        float maxRotation = rotationSpeed * Time.deltaTime;

        float rotation = Mathf.Clamp(deltaAngle, -maxRotation, maxRotation);
        shipTransform.Rotate(Vector3.forward * rotation);
    }
    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {

        }
    }

}
