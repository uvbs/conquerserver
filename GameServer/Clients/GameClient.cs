﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkLibrary;
namespace GameServer
{
    public class PacketQueue
    {
        private List<byte> Queue;
        private byte[] Data;
        
        public PacketQueue()
        {
            Queue = new List<byte>();
        }
        public void Push(byte[] Packet)
        {
            lock (Queue)
            {
                Queue.AddRange(Packet);
                Data = Queue.ToArray();
            }
        }
        public byte[] Pop()
        {
            lock (Queue)
            {
                if (Data.Length < 2) return null;
                ushort Size = BitConverter.ToUInt16(Data, 0);
                if (Size > Data.Length)
                {
                    // Framentation, rest of packet should follow in the next transmission(s)
                    return null;
                }
                else
                {
                    byte[] Packet = new byte[Size];
                    Buffer.BlockCopy(Data, 0, Packet, 0, Size);

                    Queue.RemoveRange(0, Size);
                    Data = Queue.ToArray();

                    return Packet;
                }
            }
        }
        public int Size
        {
            get { return Queue == null ? -1 : Queue.Count; }
        }
    }
    public unsafe class GameClient
    {
        private WinsockClient Connection;
        private GameCryptography Crypto;
        public Entity Entity;
        public Screen Screen;
        public uint UID;

        private PacketQueue Queue;

        public GameClient(WinsockClient Connection)
        {
            this.Connection = Connection;
   
            Crypto = new GameCryptography();
            Crypto.Initialize();
            Entity = new Entity(this);

            Queue = new PacketQueue();
            Screen = new Screen(this);
        }

        public PacketQueue Packets
        {
            get { return Queue; }
            set { Queue = value; }
        }
       
   
        public void Send(void* Packet, ushort Size)
        {
            byte[] tmp = new byte[Size];
            fixed (byte* pPacket = tmp)
            {
                Memory.Copy(Packet, pPacket, Size);
            }
            Send(tmp);
        }
        public void SendScreen(void* Packet, ushort Size)
        {
            Entity[] Entities = Screen.Players.Values.ToArray();
            foreach (Entity entity in Entities)
            {
                entity.Owner.Send(Packet, Size);
            }
        }
        public void Send(byte[] Packet)
        {
            Crypto.Encrypt(Packet);
            Connection.Send(Packet);
        }
        public void Decrypt(byte[] Packet)
        {
            Crypto.Decrypt(Packet);
        }
        public void GenerateKeys(uint Key1, uint Key2)
        {
            Crypto.GenerateKeys(Key1, Key2);
        }
        public void Disconnect()
        {
            Connection.Disconnect();
        }
    }
}
