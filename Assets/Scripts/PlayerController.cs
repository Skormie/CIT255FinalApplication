using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

    public float movementSpeed;
    private Animator anim;
    private Rigidbody2D rigid2D;
    private CinemachineVirtualCamera vCam;
    public GameObject camFab;
    public bool mainPlayer;

    [SyncVar(hook = "OnChangeName")]
    public string playerName;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        rigid2D = GetComponent<Rigidbody2D>();
    }

    private void OnApplicationQuit()
    {
        MySQLCS.UpdateCharacter(transform.gameObject, PlayerPrefs.GetInt("char_id"));
    }

    public override void OnStartLocalPlayer()
    {
        mainPlayer = true;
        transform.position = new Vector2(PlayerPrefs.GetInt("last_x"), PlayerPrefs.GetInt("last_y"));
        SceneManager.activeSceneChanged += ChangedActiveScene;
        GetComponent<SpriteRenderer>().color = Color.blue;
        vCam = Instantiate<GameObject>(camFab).GetComponent<CinemachineVirtualCamera>();
        GameObject camConfiner = GameObject.FindGameObjectWithTag("Base");
        if (camConfiner != null)
            vCam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = camConfiner.GetComponent<CompositeCollider2D>().GetComponent<Collider2D>();
        vCam.Follow = transform;
        DontDestroyOnLoad(vCam);
        DontDestroyOnLoad(transform.gameObject);
        GetComponentInChildren<Text>().text = PlayerPrefs.GetString("name");
        playerName = PlayerPrefs.GetString("name");
        CmdOnServerNameChange(PlayerPrefs.GetString("name"));
    }

    [Command]
    void CmdOnServerNameChange(string name)
    {
        playerName = name;
        RpcOnClientUpdateNameChange(playerName);
    }

    [ClientRpc]
    void RpcOnClientUpdateNameChange(string name)
    {
        GetComponentInChildren<Text>().text = name;

        foreach (var clientPlayer in GameObject.FindGameObjectsWithTag("Player"))
        {
            string text = clientPlayer.GetComponentInChildren<Text>().text;
            string playerName = clientPlayer.GetComponent<PlayerController>().playerName;
            if (text != playerName && playerName != "")
                clientPlayer.GetComponentInChildren<Text>().text = clientPlayer.GetComponent<PlayerController>().playerName;
        }
    }

    public void OnChangeName(string name)
    {
        GetComponentInChildren<Text>().text = name;
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

        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Trying to save this shit.");
            MySQLCS.UpdateCharacter(transform.gameObject, PlayerPrefs.GetInt("char_id"));
        }

        //if ( Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f )
        //    transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * movementSpeed * Time.deltaTime, Input.GetAxisRaw("Vertical") * movementSpeed * Time.deltaTime));
    }
}
