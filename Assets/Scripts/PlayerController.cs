using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cinemachine;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour {

    public float movementSpeed;
    private Animator anim;
    private Rigidbody2D rigid2D;
    private CinemachineVirtualCamera vCam;
    public GameObject camFab;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        rigid2D = GetComponent<Rigidbody2D>();
    }

    public override void OnStartLocalPlayer()
    {
        SceneManager.activeSceneChanged += ChangedActiveScene;
        GetComponent<SpriteRenderer>().color = Color.blue;
        vCam = Instantiate<GameObject>(camFab).GetComponent<CinemachineVirtualCamera>();
        vCam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = GameObject.FindGameObjectWithTag("Base").GetComponent<CompositeCollider2D>().GetComponent<Collider2D>();
        vCam.Follow = transform;
        DontDestroyOnLoad(vCam);
        DontDestroyOnLoad(transform.gameObject);
        //GetComponent<Material>().color = Color.blue;
    }

    private void ChangedActiveScene(Scene current, Scene next)
    {
        if (!isLocalPlayer) return;
        string currentName = current.name;

        if (currentName == null)
        {
            // Scene1 has been removed
            currentName = "Replaced";
        }

        Debug.Log("Scenes: " + currentName + ", " + next.name);
        vCam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = GameObject.FindGameObjectWithTag("Base").GetComponent<CompositeCollider2D>().GetComponent<Collider2D>();
        transform.position = GameObject.FindGameObjectWithTag("LoadPoint").transform.position;
    }

    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer) return;

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
