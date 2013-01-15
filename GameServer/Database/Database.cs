﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
namespace GameServer.Database
{
    public class DatabaseManager
    {
        private SQLiteConnection Connection;
        private CharacterDataCtrl CharacterCtrl;
        private LocationDataCtrl LocationCtrl;
        private EquipmentCtrl EquipmentCtrl;

        public DatabaseManager()
        {
            Connection = new SQLiteConnection("Data Source=..\\..\\..\\ConquerDatabase.db;Version=3");
            Connection.Open();

            CharacterCtrl = new CharacterDataCtrl(Connection);
            LocationCtrl = new LocationDataCtrl(Connection);
            EquipmentCtrl = new EquipmentCtrl(Connection);

        }
        public void LoadEquipment(GameClient Client)
        {
            EquipmentCtrl.LoadEquipment(Client);
        }
        public void DropCharacterTable()
        {
            CharacterCtrl.DestroyTable();
        }
        public void SaveCharacter(GameClient Client)
        {
            CharacterCtrl.UpdateCharacter(Client);
            LocationCtrl.UpdateLocation(Client);
        }
        public void CreateCharacter(GameClient Client, ushort Model, ushort Class, string Name)
        {
            CharacterCtrl.CreateCharacter(Client, Model, Class, Name);
            LocationCtrl.AddLocation(Client);
        }
        public bool GetCharacterData(GameClient Client)
        {
            bool Result = CharacterCtrl.GetCharacterData(Client);
            Result &= LocationCtrl.GetLocation(Client);
            return Result;
        }
    }
}
