using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyController : MonoBehaviour {

    public float moveSpeed;
    public float movementDelay; // Delay in seconds.
    private float movementClock;
    private Rigidbody2D rbody;
    private Vector2 moveDirection;
    private Animator anim;

    // Use this for initialization
    void Start () {
        rbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeMovementDirection();
    }

	// Update is called once per frame
	void Update () {
        if (Time.fixedTime < movementClock)
        {
            anim.SetFloat("MoveX", moveDirection.x);
            anim.SetFloat("MoveY", moveDirection.y);
            rbody.velocity = moveDirection;
        }
        else
            ChangeMovementDirection();
	}

    void ChangeMovementDirection ()
    {
        rbody.velocity = Vector2.zero;
        movementClock = Time.fixedTime + movementDelay;
        moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * moveSpeed;
    }
}
