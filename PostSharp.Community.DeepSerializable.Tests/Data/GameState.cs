using System;
using System.Collections.Generic;

namespace PostSharp.Community.DeepSerializable.Tests.Data
{
    [DeepSerializable]
    public class GameState
    {
        public List<Player> Players = new List<Player>();

        public Player MasterPlayer;

        public Weather Weather;
        
        public int TurnsElapsed;
    }
    
    [Serializable]
    public class NonDeepGameState
    {
        public List<NonSerializablePlayer> Players = new List<NonSerializablePlayer>();

        public NonSerializablePlayer MasterPlayer;

        public Weather Weather;
        
        public int TurnsElapsed;
    }
}