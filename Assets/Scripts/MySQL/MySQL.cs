//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using ADODB;
//using System;

//namespace ServerApplication
//{
//    public class MySQL
//    {
//        public Recordset databaseRecordset;
//        public Connection clientConnection;

//        public static MySQL mysql = new MySQL();

//        // Use this for initialization
//        public void MySQLInit()
//        {
//            try
//            {
//                databaseRecordset = new Recordset();
//                clientConnection = new Connection();

//                clientConnection.ConnectionString = "";
//                clientConnection.CursorLocation = CursorLocationEnum.adUseServer;
//                clientConnection.Open();
//                Console.WriteLine("Connection to MYSQL Server was successful.");

//                var db = databaseRecordset;
//                {
//                    db.Open("SELECT * FROM accounts WHERE ID = 1", clientConnection, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockOptimistic);
//                    db.Close();
//                }
//            }
//            catch(Exception ex)
//            {
//                Console.WriteLine(ex);
//                Console.ReadLine();
//            }
//        }
//    }
//}
