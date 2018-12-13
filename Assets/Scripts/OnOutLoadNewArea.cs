using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class OnOutLoadNewArea : NetworkBehaviour {

    public SceneReference targetLevel;

    void OnTriggerExit2D(Collider2D other)
    {
        NetworkIdentity networkIdentity = other.gameObject.GetComponent<NetworkIdentity>();
        if (other.gameObject.tag == "Player" && networkIdentity.isLocalPlayer && !networkIdentity.isServer && networkIdentity.isClient)
        {
            SceneManager.LoadScene(targetLevel);
        }
    }
}
