using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float movementSpeed;
    private Animator anim;
    private Rigidbody2D rigid2D;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        rigid2D = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update () {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(hAxis, vAxis);

        anim.SetFloat("Magnitude", movement.magnitude);

        //transform.position = transform.position + movement * movementSpeed * Time.deltaTime;
        if (movement.magnitude > 0)
        {
            anim.SetFloat("MoveX", movement.x);
            anim.SetFloat("MoveY", movement.y);
            rigid2D.velocity = movement * movementSpeed;
            ////transform.Translate(movement * movementSpeed * Time.deltaTime);
        }
        else
        {
            rigid2D.velocity = Vector2.zero;
        }

        //if ( Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f )
        //    transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * movementSpeed * Time.deltaTime, Input.GetAxisRaw("Vertical") * movementSpeed * Time.deltaTime));
    }
}
