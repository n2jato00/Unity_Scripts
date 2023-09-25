using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowItem : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField]
    private float maxThrowForce = 50f;
    [SerializeField]
    private float minSwipeThreshold = 10f; // Minimum swipe distance to count as a throw

    [Header("Drag Settings")]
    [SerializeField]
    private float followSpeed = 5f;

    private Vector2 startSwipePos;
    private Vector2 endSwipePos;
    private bool isDragging = false;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        HandleDrag();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startSwipePos = Input.mousePosition;
            isDragging = true;
            rb.useGravity = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endSwipePos = Input.mousePosition;
            isDragging = false;
            rb.useGravity = true;
            if (Vector2.Distance(startSwipePos, endSwipePos) > minSwipeThreshold)
            {
                ThrowBall();
            }
        }
    }

    private void HandleDrag()
    {
        if (isDragging)
        {
            float distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);
            Vector3 mousePosOnScreen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToCamera);
            Vector3 mousePosInWorld = Camera.main.ScreenToWorldPoint(mousePosOnScreen);
            mousePosInWorld.z = transform.position.z;

            // Check if the ball is close enough to the mouse position
            if (Vector3.Distance(transform.position, mousePosInWorld) < 0.05f) // You can adjust the 0.1f threshold if needed
            {
                rb.MovePosition(mousePosInWorld);
            }
            else
            {
                rb.MovePosition(Vector3.Lerp(transform.position, mousePosInWorld, Time.deltaTime * followSpeed));
            }
        }
    }



    private void ThrowBall()
    {
        Vector2 swipeDir = endSwipePos - startSwipePos;
        float throwForce = Mathf.Clamp(swipeDir.magnitude, 0, maxThrowForce);
        rb.AddForce(new Vector3(0, swipeDir.normalized.y, 1) * throwForce, ForceMode.Impulse);
    }
}
