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
    public GameObject login;
    public GameObject accountCreation;
    public GameObject characterSelect;
    public GameObject characterCreation;
    public SceneReference startingScene;


    readonly char[] gender = new char[] { 'M', 'F' };

    static readonly string constr = "Database=lovelyvh_main;Server=217.182.207.227;Uid=lovelyvh_user;Password=Lance1991;pooling=true;CharSet=utf8;port=3306";
    MySqlConnection connection = null;
    MySqlCommand command = null;
    MySqlDataReader dataReader = null;

    public struct LoginInfo
    {
        public int account_id, character_slots, account_privilege, logincount;
        public string userid, user_pass, email, last_ip;
        public DateTime lastlogin;
    }

    public struct Character
    {
        public int char_id, account_id, char_num, level, base_exp, money, str, agi, vit, intel, dex, luk, max_hp, hp, status_point, hair, hair_color, clothes_color, online;
        public int last_x, last_y, save_x, save_y;
        public string name, last_map, save_map;
        public char gender;
    }

    void Awake()
    {
        if (Application.isBatchMode)
        {
            SceneManager.LoadScene("LoadServer");
        }
        if (connection == null)
        {
            try
            {
                // Connection Element.
                connection = new MySqlConnection(constr);

                // Try to open a connection.
                connection.Open();
                Debug.Log("Connection: " + connection.State);
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

    public void Login()
    {
        // Get text elements and other components.
        InputField[] inputs = login.GetComponentsInChildren<InputField>();
        Text[] textElements = login.GetComponentsInChildren<Text>();

        // No username entered.
        if (inputs[0].text == string.Empty)
        {
            textElements[7].text = "Enter a username.";
            return;
        }

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
                        textElements[7].text = "No User Found!";
                    dataReader.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }

        if (savedPasswordHash == string.Empty)
        {
            textElements[7].text = "This account has no password?";
            return;
        }

        byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);

        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        var pbkdf2 = new Rfc2898DeriveBytes(inputs[1].text, salt, 10000);
        byte[] hash = pbkdf2.GetBytes(20);

        for (int i = 0; i < 20; i++)
            if (hashBytes[i + 16] != hash[i])
            {
                textElements[7].text = "Invalid Password!";
                return;
            }

        LoginInfo accountData = new LoginInfo();

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

        PlayerPrefs.SetString("userid", accountData.userid);
        PlayerPrefs.SetInt("account_id", accountData.account_id);
        PlayerPrefs.SetString("email", accountData.email);
        PlayerPrefs.SetInt("character_slots", accountData.character_slots);
        PlayerPrefs.SetInt("account_privilege", accountData.account_privilege);
        PlayerPrefs.SetInt("logincount", accountData.logincount);
        //PlayerPrefs.SetString("lastlogin", accountData.lastlogin.ToShortDateString());
        PlayerPrefs.SetString("last_ip", accountData.last_ip);

        //Switching Windows
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
            query = "SELECT `name` FROM `character` WHERE `account_id` = " + PlayerPrefs.GetInt("account_id") + "";
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
                                name = dataReader["name"].ToString()
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
        Text[] textElements = characterSelect.GetComponentsInChildren<Text>();

        if (PlayerPrefs.GetInt("char_count") >= PlayerPrefs.GetInt("character_slots"))
        {
            textElements[4].text = "Maximum Characters Reached!";
            return; //Maximum Amount of characters reached.
        }
        characterSelect.GetComponentInParent<Canvas>().enabled = false;
        characterCreation.GetComponent<Canvas>().enabled = true;
    }

    public void CharacterPlay()
    {
        Text[] textElements = characterSelect.GetComponentsInChildren<Text>();
        if (characterSelect.GetComponentInChildren<Dropdown>().options.Count <= 0)
        {
            textElements[4].text = "No Character Selected!";
            return;
        }
        string character = characterSelect.GetComponentInChildren<Dropdown>().options[characterSelect.GetComponentInChildren<Dropdown>().value].text;
        if (character == string.Empty) return;
        Character characterData = new Character();
        string query = string.Empty;

        Debug.Log("We got this far.");
        dataReader.Dispose();
        try
        {
            query = "SELECT `characterid`, `name`, `max_hp`, `hp` FROM `character` WHERE `account_id` = " + PlayerPrefs.GetInt("account_id") + " AND `name` = '" + character + "'";
            if (connection.State.ToString() != "Open")
                connection.Open();
            using (connection)
            {
                using (command = new MySqlCommand(query, connection))
                {
                    Debug.Log("Now here.");
                    dataReader = command.ExecuteReader();
                    if (dataReader.Read())
                    {
                        characterData = new Character();
                        characterData.char_id = int.Parse(dataReader["characterid"].ToString());
                        characterData.name = dataReader["name"].ToString();
                        characterData.last_x = int.Parse(dataReader["max_hp"].ToString());
                        characterData.last_y = int.Parse(dataReader["hp"].ToString());
                    }
                    else
                    {
                        textElements[4].text = "No data for that character found.";
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
        Debug.Log("Ok Wtf");

        PlayerPrefs.SetInt("char_id", characterData.char_id);
        PlayerPrefs.SetString("name", characterData.name);
        PlayerPrefs.SetInt("last_x", characterData.last_x);
        PlayerPrefs.SetInt("last_y", characterData.last_y);

        Debug.Log(characterData.name);
        Debug.Log(characterData.last_map);

        SceneManager.LoadScene("LoadServer");
        connection.Close();
        connection.Dispose();
    }

    public void CreateCharacter()
    {
        InputField[] inputs = characterCreation.GetComponentsInChildren<InputField>();
        Slider[] sliders = characterCreation.GetComponentsInChildren<Slider>();
        Text[] textElements = characterCreation.GetComponentsInChildren<Text>();

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
                        textElements[6].text = "Character name already exists!";
                        dataReader.Dispose();
                        return;
                    }
                }
                dataReader.Dispose();

                query = "INSERT INTO `character` (`account_id`, `char_num`, `name`, `gamemap`, `gender`) VALUES (?account_id, ?char_num, ?name, ?gamemap, ?gender)";
                using (command = new MySqlCommand(query, connection))
                {
                    MySqlParameter oParam = command.Parameters.Add("?account_id", MySqlDbType.UInt64);
                    oParam.Value = PlayerPrefs.GetInt("account_id");
                    MySqlParameter oParam1 = command.Parameters.Add("?char_num", MySqlDbType.UInt64);
                    oParam1.Value = PlayerPrefs.GetInt("char_count");
                    MySqlParameter oParam2 = command.Parameters.Add("?name", MySqlDbType.VarChar);
                    oParam2.Value = inputs[0].text;
                    MySqlParameter oParam3 = command.Parameters.Add("?gamemap", MySqlDbType.VarChar);
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

        BuildCharacterList();
        characterCreation.GetComponent<Canvas>().enabled = false;
        characterSelect.GetComponent<Canvas>().enabled = true;

        Debug.Log("Character Created?");
    }

    static public void UpdateCharacter( GameObject player, int characterid )
    {
        string query = string.Empty;
        Debug.Log((int)player.transform.position.x + " " + (int)player.transform.position.y + " " + (int)characterid);
        string x = Math.Truncate(player.transform.position.x) + "";
        string y = Math.Truncate(player.transform.position.y) + "";
        MySqlConnection connect = new MySqlConnection(constr);
        MySqlCommand cmd;
        connect.Open();

        try
        {
            using (connect)
            {
                query = "UPDATE `character` SET `max_hp` = ?max_hp, `hp` = ?hp WHERE `characterid` = ?characterid";
                using (cmd = new MySqlCommand(query, connect))
                {
                    MySqlParameter oParam = cmd.Parameters.Add("?max_hp", MySqlDbType.VarChar);
                    oParam.Value = x;
                    MySqlParameter oParam1 = cmd.Parameters.Add("?hp", MySqlDbType.VarChar);
                    oParam1.Value = y;
                    MySqlParameter oParam2 = cmd.Parameters.Add("?characterid", MySqlDbType.UInt32);
                    oParam2.Value = characterid;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }

        connect.Close();
        connect.Dispose();

        Debug.Log("Character Updated?");
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
        Text[] textElements = characterSelect.GetComponentsInChildren<Text>();

        if (characterSelect.GetComponentInChildren<Dropdown>().options.Count <= 0)
        {
            textElements[4].text = "No character selected!";
            return;
        }
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

        PlayerPrefs.SetInt("char_count", PlayerPrefs.GetInt("char_count") - 1);
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
        Text[] textElements = accountCreation.GetComponentsInChildren<Text>();

        if (inputs[0].text == string.Empty || inputs[1].text == string.Empty)
        {
            textElements[13].text = "Username and Password cannot be blank!";
            return; //Empty Values
        }

        if (inputs[1].text != inputs[2].text)
        {
            textElements[13].text = "Passwords do not match!";
            return; // Passwords Don't match.
        }

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
                        textElements[13].text = "Account Already Exists!";
                        dataReader.Dispose();
                        return;
                    }
                }
                dataReader.Dispose();

                query = "INSERT INTO `login` (`userid`, `user_pass`, `email`) VALUES (?userid, ?user_pass, ?email)";
                using (command = new MySqlCommand(query, connection))
                {
                    MySqlParameter oParam = command.Parameters.Add("?userid", MySqlDbType.VarChar);
                    oParam.Value = inputs[0].text;
                    MySqlParameter oParam1 = command.Parameters.Add("?user_pass", MySqlDbType.VarChar);
                    oParam1.Value = savedPasswordHash;
                    MySqlParameter oParam2 = command.Parameters.Add("?email", MySqlDbType.VarChar);
                    oParam2.Value = inputs[3].text;
                    //MySqlParameter oParam3 = command.Parameters.Add("?lastlogin", MySqlDbType.DateTime);
                    //oParam3.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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

        Debug.Log("Account Created?");
        accountCreation.GetComponent<Canvas>().enabled = false;
        login.GetComponent<Canvas>().enabled = true;
    }
}