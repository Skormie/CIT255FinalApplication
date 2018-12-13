using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UNETScript : NetworkManager
{
    [SerializeField]
    public bool editorIsServer;

    private void Awake()
    {
        if (editorIsServer || Application.isBatchMode)
        {
            StartServer();
        }
        else
        {
            StartClient();
        }
    }

    void OnApplicationQuit()
    {
        if (Application.isBatchMode)
        {
            StopServer();
        }
        else if(client != null)
        {
            StopClient();
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log("Scene Changed to " + sceneName + ".");
        var mob = NetworkManager.Instantiate(NetworkManager.singleton.spawnPrefabs[0], Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(mob);
    }

    public override void OnClientDisconnect(NetworkConnection connection)
    {
        Debug.Log(PlayerPrefs.GetFloat("last_x") + " " + PlayerPrefs.GetFloat("last_y") + " " + PlayerPrefs.GetInt("char_id") + "");
        MySQLCS.UpdateCharacter( connection.playerControllers[0].gameObject, PlayerPrefs.GetInt("char_id") );
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("OnPlayerConnected");
    }
}
