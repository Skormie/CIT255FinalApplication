using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using UnityEngine.UI;

public class MySQLCS : MonoBehaviour
{
    // In truth, the only things you want to save to the database are dynamic objects
    // static objects in the scene will always exist, so make sure to set your Tag
    // based on the documentation for this demo

    // values to match the database columns
    private readonly string ID, Name, levelname, objectType;
    private readonly float posx, posy, posz, tranx, trany, tranz;

    public GameObject login;
    public GameObject accountCreation;
    public GameObject characterSelect;
    public GameObject characterCreation;
    public SceneReference startingScene;


    readonly char[] gender = new char[] { 'M', 'F' };

    // MySQL instance specific items
    readonly string constr = "Database=lovelyvh_main;Server=217.182.207.227;Uid=lovelyvh_user;Password=Lance1991;pooling=true;CharSet=utf8;port=3306";
    // connection object
    MySqlConnection connection = null;
    // command object
    MySqlCommand command = null;
    // reader object
    MySqlDataReader dataReader = null;
    // object collection array
    GameObject[] bodies;
    // object definitions
    public struct Data
    {
        public int UID;
        public string ID, Name, levelname, objectType;
        public float posx, posy, posz, tranx, trany, tranz;
    }

    public struct LoginInfo
    {
        public int account_id, character_slots, account_privilege, logincount;
        public string userid, user_pass, email, last_ip;
        public DateTime lastlogin;
    }

    public struct Character
    {
        public int char_id, account_id, char_num, level, base_exp, money, str, agi, vit, intel, dex, luk, max_hp, hp, status_point, hair, hair_color, clothes_color, last_x, last_y, save_x, save_y, online;
        public string name, last_map, save_map;
        public char gender;
    }

    // collection container
    List<Data> _GameItems;
    void Awake()
    {
        if(connection == null)
        {
            try
            {
                // setup the connection element
                connection = new MySqlConnection(constr);

                // lets see if we can open the connection
                connection.Open();
                Debug.Log("Connection State: " + connection.State);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("killing con");
        if (connection != null)
        {
            if (connection.State.ToString() != "Closed")
                connection.Close();
            connection.Dispose();
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    // gui event like a button, etc
    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(10, 70, 50, 30), "Save") && !saving)
    //    {
    //        saving = true;
    //        // first lets clean out the databae
    //        DeleteEntries();
    //        // now lets save the scene information
    //        InsertEntries();
    //        // you could also use the update if you know the ID of the item already saved

    //        saving = false;
    //    }
    //    if (GUI.Button(new Rect(10, 110, 50, 30), "Load") && !loading)
    //    {
    //        loading = true;
    //        // lets read the items from the database
    //        ReadEntries();
    //        // now display what is known about them to our log
    //        LogGameItems();
    //        loading = false;
    //    }
    //}

    // Insert new entries into the table
    public void InsertEntries()
    {
        PrepData();
        string query = string.Empty;
        // Error trapping in the simplest form
        try
        {
            query = "INSERT INTO demo_table (ID, Name, levelname, objectType, posx, posy, posz, tranx, trany, tranz) VALUES (?ID, ?Name, ?levelname, ?objectType, ?posx, ?posy, ?posz, ?tranx, ?trany, ?tranz)";
            if (connection.State.ToString() != "Open")
                connection.Open();
            using (connection)
            {
                foreach (Data itm in _GameItems)
                {
                    using (command = new MySqlCommand(query, connection))
                    {
                        MySqlParameter oParam = command.Parameters.Add("?ID", MySqlDbType.VarChar);
                        oParam.Value = itm.ID;
                        MySqlParameter oParam1 = command.Parameters.Add("?Name", MySqlDbType.VarChar);
                        oParam1.Value = itm.Name;
                        MySqlParameter oParam2 = command.Parameters.Add("?levelname", MySqlDbType.VarChar);
                        oParam2.Value = itm.levelname;
                        MySqlParameter oParam3 = command.Parameters.Add("?objectType", MySqlDbType.VarChar);
                        oParam3.Value = itm.objectType;
                        MySqlParameter oParam4 = command.Parameters.Add("?posx", MySqlDbType.Float);
                        oParam4.Value = itm.posx;
                        MySqlParameter oParam5 = command.Parameters.Add("?posy", MySqlDbType.Float);
                        oParam5.Value = itm.posy;
                        MySqlParameter oParam6 = command.Parameters.Add("?posz", MySqlDbType.Float);
                        oParam6.Value = itm.posz;
                        MySqlParameter oParam7 = command.Parameters.Add("?tranx", MySqlDbType.Float);
                        oParam7.Value = itm.tranx;
                        MySqlParameter oParam8 = command.Parameters.Add("?trany", MySqlDbType.Float);
                        oParam8.Value = itm.trany;
                        MySqlParameter oParam9 = command.Parameters.Add("?tranz", MySqlDbType.Float);
                        oParam9.Value = itm.tranz;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        finally
        {
        }
    }

    // Update existing entries in the table based on the iddemo_table
    public void UpdateEntries()
    {
        PrepData();
        string query = string.Empty;
        // Error trapping in the simplest form
        try
        {
            query = "UPDATE demo_table SET ID=?ID, Name=?Name, levelname=?levelname, objectType=?objectType, posx=?posx, posy=?posy, posz=?posz, tranx=?tranx, trany=?trany, tranz=?tranz WHERE iddemo_table=?UID";
            if (connection.State.ToString() != "Open")
                connection.Open();
            using (connection)
            {
                foreach (Data itm in _GameItems)
                {
                    using (command = new MySqlCommand(query, connection))
                    {
                        MySqlParameter oParam = command.Parameters.Add("?ID", MySqlDbType.VarChar);
                        oParam.Value = itm.ID;
                        MySqlParameter oParam1 = command.Parameters.Add("?Name", MySqlDbType.VarChar);
                        oParam1.Value = itm.Name;
                        MySqlParameter oParam2 = command.Parameters.Add("?levelname", MySqlDbType.VarChar);
                        oParam2.Value = itm.levelname;
                        MySqlParameter oParam3 = command.Parameters.Add("?objectType", MySqlDbType.VarChar);
                        oParam3.Value = itm.objectType;
                        MySqlParameter oParam4 = command.Parameters.Add("?posx", MySqlDbType.Float);
                        oParam4.Value = itm.posx;
                        MySqlParameter oParam5 = command.Parameters.Add("?posy", MySqlDbType.Float);
                        oParam5.Value = itm.posy;
                        MySqlParameter oParam6 = command.Parameters.Add("?posz", MySqlDbType.Float);
                        oParam6.Value = itm.posz;
                        MySqlParameter oParam7 = command.Parameters.Add("?tranx", MySqlDbType.Float);
                        oParam7.Value = itm.tranx;
                        MySqlParameter oParam8 = command.Parameters.Add("?trany", MySqlDbType.Float);
                        oParam8.Value = itm.trany;
                        MySqlParameter oParam9 = command.Parameters.Add("?tranz", MySqlDbType.Float);
                        oParam9.Value = itm.tranz;
                        MySqlParameter oParam10 = command.Parameters.Add("?UID", MySqlDbType.Int32);
                        oParam10.Value = itm.UID;

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        finally
        {
        }
    }

    // Delete entries from the table
    public void DeleteEntries()
    {
        string query = string.Empty;
        // Error trapping in the simplest form
        try
        {
            // optimally you will know which items you want to delete from the database
            // using the following code and the record ID, you can delete the entry
            //-----------------------------------------------------------------------
            // query = "DELETE FROM demo_table WHERE iddemo_table=?UID";
            // MySqlParameter oParam = cmd.Parameters.Add("?UID", MySqlDbType.Int32);
            // oParam.Value = 0;
            //-----------------------------------------------------------------------
            query = "DELETE FROM demo_table WHERE iddemo_table";
            if (connection.State.ToString() != "Open")
                connection.Open();
            using (connection)
            {
                using (command = new MySqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        finally
        {
        }
    }

    // Read all entries from the tablewas
    public void ReadEntries()
    {
        string query = string.Empty;
        if (_GameItems == null)
            _GameItems = new List<Data>();
        if (_GameItems.Count > 0)
            _GameItems.Clear();
        // Error trapping in the simplest form
        try
        {
            query = "SELECT * FROM view_demo";
            if (connection.State.ToString() != "Open")
                connection.Open();
            using (connection)
            {
                using (command = new MySqlCommand(query, connection))
                {
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                        while (dataReader.Read())
                        {
                            Data itm = new Data
                            {
                                UID = int.Parse(dataReader["iddemo_table"].ToString()),
                                ID = dataReader["ID"].ToString(),
                                levelname = dataReader["levelname"].ToString(),
                                Name = dataReader["Name"].ToString(),
                                objectType = dataReader["objectType"].ToString(),
                                posx = float.Parse(dataReader["posx"].ToString()),
                                posy = float.Parse(dataReader["posy"].ToString()),
                                posz = float.Parse(dataReader["posz"].ToString()),
                                tranx = float.Parse(dataReader["tranx"].ToString()),
                                trany = float.Parse(dataReader["trany"].ToString()),
                                tranz = float.Parse(dataReader["tranz"].ToString())
                            };
                            _GameItems.Add(itm);
                        }
                    dataReader.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        finally
        {
        }
    }

    public void Login()
    {
        InputField[] inputs = login.GetComponentsInChildren<InputField>();

        if (inputs[0].text == string.Empty) return;

        string savedPasswordHash = string.Empty;

        string query = string.Empty;

        try
        {
            query = "SELECT `user_pass` FROM `login` WHERE `userid` = '"+ inputs[0].text + "'";
            if (connection.State.ToString() != "Open")
                connection.Open();
            using (connection)
            {
                using (command = new MySqlCommand(query, connection))
                {
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                        while (dataReader.Read())
                        {
                            savedPasswordHash = dataReader["user_pass"].ToString();
                        }
                    else
                        Debug.Log("No rows found.");
                    dataReader.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        finally
        {
        }

        if (savedPasswordHash == string.Empty) return;

        byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);

        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        var pbkdf2 = new Rfc2898DeriveBytes(inputs[1].text, salt, 10000);
        byte[] hash = pbkdf2.GetBytes(20);

        for (int i = 0; i < 20; i++)
            if (hashBytes[i + 16] != hash[i])
                throw new UnauthorizedAccessException();

        LoginInfo accountData = new LoginInfo();

        Debug.Log("We made it this far.");

        Debug.Log(inputs[0].text);

        try
        {
            query = "SELECT `account_id`, `userid`, `email`, `character_slots`, `logincount`, `last_ip` FROM `login` WHERE `userid` = '" + inputs[0].text + "'";
            if (connection.State.ToString() != "Open")
                connection.Open();
            using (connection)
            {
                using (command = new MySqlCommand(query, connection))
                {
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                        while (dataReader.Read())
                        {
                            accountData = new LoginInfo
                            {
                                account_id = int.Parse(dataReader["account_id"].ToString()),
                                userid = dataReader["userid"].ToString(),
                                email = dataReader["email"].ToString(),
                                character_slots = int.Parse(dataReader["character_slots"].ToString()),
                                logincount = int.Parse(dataReader["logincount"].ToString()),
                                //lastlogin = DateTime.Parse(dataReader["lastlogin"].ToString()),
                                last_ip = dataReader["last_ip"].ToString()
                            };
                        }
                    dataReader.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
            return;
        }
        finally
        {
        }

        PlayerPrefs.SetString("userid", accountData.userid);
        PlayerPrefs.SetInt("account_id", accountData.account_id);
        PlayerPrefs.SetString("email", accountData.email);
        PlayerPrefs.SetInt("character_slots", accountData.character_slots);
        PlayerPrefs.SetInt("account_privilege", accountData.account_privilege);
        PlayerPrefs.SetInt("logincount", accountData.logincount);
        //PlayerPrefs.SetString("lastlogin", accountData.lastlogin.ToShortDateString());
        PlayerPrefs.SetString("last_ip", accountData.last_ip);

        //Switching Windows
        //GameObject characterSelect = GameObject.Find("Character Select");
        login.GetComponent<Canvas>().enabled = false;
        characterSelect.GetComponent<Canvas>().enabled = true;
        BuildCharacterList();
    }

    public void BuildCharacterList()
    {
        characterSelect.GetComponentInChildren<Dropdown>().ClearOptions();
        string query = string.Empty;
        List<Character> characterData = new List<Character>();

        try
        {
            query = "SELECT name FROM `character` WHERE `account_id` = " + PlayerPrefs.GetInt("account_id") + "";
            if (connection.State.ToString() != "Open")
                connection.Open();
            using (connection)
            {
                using (command = new MySqlCommand(query, connection))
                {
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                        while (dataReader.Read())
                        {
                            characterData.Add(new Character
                            {
                                //char_id = int.Parse(dataReader["char_id"].ToString()),
                                name = dataReader["name"].ToString()
                                //last_map = dataReader["last_map"].ToString(),
                                //char_num = int.Parse(dataReader["char_num"].ToString()),
                                //level = int.Parse(dataReader["level"].ToString())
                            });
                        }
                    dataReader.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
            return;
        }
        finally
        {
        }

        List<string> characters = new List<string>();
        int charCount = 0;

        foreach (var character in characterData)
        {
            characters.Add(character.name);
            charCount++;
        }

        PlayerPrefs.SetInt("char_count", charCount);

        characterSelect.GetComponentInChildren<Dropdown>().AddOptions(characters);

        Debug.Log("We made it!");
    }

    public void CharacterSelect()
    {
        Debug.Log(PlayerPrefs.GetInt("char_count") +">="+ PlayerPrefs.GetInt("character_slots"));
        if (PlayerPrefs.GetInt("char_count") >= PlayerPrefs.GetInt("character_slots")) return; //Maximum Amount of characters reached.
        Debug.Log("Test");
        //GameObject characterCreation = GameObject.FindGameObjectWithTag("CharacterCreator");
        characterSelect.GetComponentInParent<Canvas>().enabled = false;
        characterCreation.GetComponent<Canvas>().enabled = true;
    }

    public void CharacterPlay()
    {
        if (characterSelect.GetComponentInChildren<Dropdown>().options.Count <= 0) return;
        string character = characterSelect.GetComponentInChildren<Dropdown>().options[characterSelect.GetComponentInChildren<Dropdown>().value].text;
        if (character == string.Empty) return;
        Character characterData = new Character();
        string query = string.Empty;

        try
        {
            query = "SELECT `name`, `last_map`, `last_x`, `last_y` FROM `character` WHERE `account_id` = " + PlayerPrefs.GetInt("account_id") + " AND `name` = '" + character + "'";
            if (connection.State.ToString() != "Open")
                connection.Open();
            using (connection)
            {
                using (command = new MySqlCommand(query, connection))
                {
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                        while (dataReader.Read())
                        {
                            characterData = new Character()
                            {
                                //char_id = int.Parse(dataReader["char_id"].ToString()),
                                name = dataReader["name"].ToString(),
                                last_map = dataReader["last_map"].ToString(),
                                last_x = int.Parse(dataReader["last_x"].ToString()),
                                last_y = int.Parse(dataReader["last_y"].ToString())
                                //char_num = int.Parse(dataReader["char_num"].ToString()),
                                //level = int.Parse(dataReader["level"].ToString())
                            };
                        }
                    dataReader.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
            return;
        }
        finally
        {
        }



        int pFrom = characterData.last_map.IndexOf("Assets/Scenes/") + "Assets/Scenes/".Length;
        int pTo = characterData.last_map.LastIndexOf(".unity");
        string scene = characterData.last_map.Substring(pFrom, pTo - pFrom);


        Debug.Log(characterData.name);
        Debug.Log(characterData.last_map);
        Debug.Log(scene);

        SceneManager.LoadScene(scene);


    }

    public void CreateCharacter()
    {
        InputField[] inputs = characterCreation.GetComponentsInChildren<InputField>();
        Slider[] sliders = characterCreation.GetComponentsInChildren<Slider>();

        if (inputs[0].text == string.Empty) return; //Empty Values

        string query = string.Empty;

        Debug.Log(inputs[0].text);
        Debug.Log(gender[(int)sliders[0].value]);

        try
        {
            query = "SELECT `name` FROM `character` WHERE `name` = '" + inputs[0].text + "' AND `account_id` = "+ PlayerPrefs.GetInt("account_id") + "";
            if (connection.State.ToString() != "Open")
                connection.Open();
            using (connection)
            {
                using (command = new MySqlCommand(query, connection))
                {
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        Debug.Log("Character Already Exists");
                        dataReader.Dispose();
                        return;
                    }
                }
                dataReader.Dispose();

                query = "INSERT INTO `character` (`account_id`, `char_num`, `name`, `last_map`, `gender`) VALUES (?account_id, ?char_num, ?name, ?last_map, ?gender)";
                using (command = new MySqlCommand(query, connection))
                {
                    MySqlParameter oParam = command.Parameters.Add("?account_id", MySqlDbType.UInt64);
                    oParam.Value = PlayerPrefs.GetInt("account_id");
                    MySqlParameter oParam1 = command.Parameters.Add("?char_num", MySqlDbType.UInt64);
                    oParam1.Value = PlayerPrefs.GetInt("char_count");
                    MySqlParameter oParam2 = command.Parameters.Add("?name", MySqlDbType.VarChar);
                    oParam2.Value = inputs[0].text;
                    MySqlParameter oParam3 = command.Parameters.Add("?last_map", MySqlDbType.VarChar);
                    oParam3.Value = startingScene.ScenePath;
                    MySqlParameter oParam4 = command.Parameters.Add("?gender", MySqlDbType.VarChar);
                    oParam4.Value = gender[(int)sliders[0].value];
                    command.ExecuteNonQuery();
                }
                dataReader.Dispose();
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        finally
        {
        }

        BuildCharacterList();
        characterCreation.GetComponent<Canvas>().enabled = false;
        characterSelect.GetComponent<Canvas>().enabled = true;

        Debug.Log("Character Created?");
    }

    public void SelectRegister()
    {
        login.GetComponentInParent<Canvas>().enabled = false;
        accountCreation.GetComponent<Canvas>().enabled = true;
    }

    public void SelectBack()
    {
        accountCreation.GetComponentInParent<Canvas>().enabled = false;
        login.GetComponent<Canvas>().enabled = true;
    }

    public void DeleteCharacter()
    {
        if (characterSelect.GetComponentInChildren<Dropdown>().options.Count <= 0) return;
        string character = characterSelect.GetComponentInChildren<Dropdown>().options[characterSelect.GetComponentInChildren<Dropdown>().value].text;
        if (character == string.Empty) return;

        string query = string.Empty;

        try
        {
            query = "DELETE FROM `character` WHERE `name` = '" + character + "' AND `account_id` = ?account_id";
            if (connection.State.ToString() != "Open")
                connection.Open();
            using (connection)
            {
                using (command = new MySqlCommand(query, connection))
                {
                    MySqlParameter oParam = command.Parameters.Add("?account_id", MySqlDbType.Int32);
                    oParam.Value = PlayerPrefs.GetInt("account_id");
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        finally
        {
        }

        characterSelect.GetComponentInChildren<Dropdown>().options.RemoveAt(characterSelect.GetComponentInChildren<Dropdown>().value);
        characterSelect.GetComponentInChildren<Dropdown>().RefreshShownValue();
    }

    public void SelectBackCharacter()
    {
        characterCreation.GetComponentInParent<Canvas>().enabled = false;
        characterSelect.GetComponent<Canvas>().enabled = true;
    }

    public void CreateAccount()
    {
        // Storing Hashed Password

        //Verify Password Hash
        InputField[] inputs = accountCreation.GetComponentsInChildren<InputField>();

        if (inputs[0].text == string.Empty || inputs[1].text == string.Empty) return; //Empty Values

        if (inputs[1].text != inputs[2].text) return; // Passwords Don't match.

        /* Fetch the stored value */
        string savedPasswordHash = string.Empty;

        string query = string.Empty;

        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
        var pbkdf2 = new Rfc2898DeriveBytes(inputs[1].text, salt, 10000);
        byte[] hash = pbkdf2.GetBytes(20);
        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);
        savedPasswordHash = Convert.ToBase64String(hashBytes);

        Debug.Log(inputs[0].text);
        Debug.Log(inputs[1].text);
        Debug.Log(inputs[2].text);
        Debug.Log(inputs[3].text);
        Debug.Log(savedPasswordHash);

        try
        {
            query = "SELECT `userid` FROM `login` WHERE `userid` = '" + inputs[0].text + "'";
            if (connection.State.ToString() != "Open")
                connection.Open();
            using (connection)
            {
                using (command = new MySqlCommand(query, connection))
                {
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        Debug.Log("Account Already Exists");
                        dataReader.Dispose();
                        return;
                    }
                }

                query = "INSERT INTO `login` (`userid`, `user_pass`, `email`, `lastlogin`) VALUES (?userid, ?user_pass, ?email, ?lastlogin)";
                using (command = new MySqlCommand(query, connection))
                {
                    MySqlParameter oParam = command.Parameters.Add("?userid", MySqlDbType.VarChar);
                    oParam.Value = inputs[0].text;
                    MySqlParameter oParam1 = command.Parameters.Add("?user_pass", MySqlDbType.VarChar);
                    oParam1.Value = savedPasswordHash;
                    MySqlParameter oParam2 = command.Parameters.Add("?email", MySqlDbType.VarChar);
                    oParam2.Value = inputs[3].text;
                    MySqlParameter oParam3 = command.Parameters.Add("?lastlogin", MySqlDbType.DateTime);
                    oParam3.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    command.ExecuteNonQuery();
                }
                dataReader.Dispose();
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        finally
        {
        }

        accountCreation.GetComponent<Canvas>().enabled = false;
        login.GetComponent<Canvas>().enabled = true;

        Debug.Log("Account Created?");
    }

    /// <summary>
    /// Lets show what was read back to the log window
    /// </summary>
    void LogGameItems()
    {
        if (_GameItems != null)
        {
            if (_GameItems.Count > 0)
            {
                foreach (Data itm in _GameItems)
                {
                    Debug.Log("UID: " + itm.UID);
                    Debug.Log("ID: " + itm.ID);
                    Debug.Log("levelname: " + itm.levelname);
                    Debug.Log("Name: " + itm.Name);
                    Debug.Log("objectType: " + itm.objectType);
                    Debug.Log("posx: " + itm.posx);
                    Debug.Log("posy: " + itm.posy);
                    Debug.Log("posz: " + itm.posz);
                    Debug.Log("tranx: " + itm.tranx);
                    Debug.Log("trany: " + itm.trany);
                    Debug.Log("tranz: " + itm.tranz);
                }
            }
        }
    }

    /// <summary>
    /// This method prepares the data to be saved into our database
    ///
    /// </summary>
    void PrepData()
    {
        bodies = GameObject.FindGameObjectsWithTag("Savable");
        _GameItems = new List<Data>();
        Data itm;
        foreach (GameObject body in bodies)
        {
            itm = new Data
            {
                ID = body.name + "_" + body.GetInstanceID(),
                Name = body.name,
                levelname = SceneManager.GetActiveScene().name,
                objectType = body.name.Replace("(Clone)", ""),
                posx = body.transform.position.x,
                posy = body.transform.position.y,
                posz = body.transform.position.z,
                tranx = body.transform.rotation.x,
                trany = body.transform.rotation.y,
                tranz = body.transform.rotation.z
            };
            _GameItems.Add(itm);
        }
        Debug.Log("Items in collection: " + _GameItems.Count);
    }
}