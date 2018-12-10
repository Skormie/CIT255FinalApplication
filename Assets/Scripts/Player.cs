using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerApplication
{
    [Serializable]
    public class Player
    {
        public string Username;
        public string Password;
        public int Level;
        public byte Access;
        public byte FirstTime;

        public byte[] inventory;
    }
}
