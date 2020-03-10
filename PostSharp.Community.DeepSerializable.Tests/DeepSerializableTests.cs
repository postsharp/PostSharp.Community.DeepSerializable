using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using PostSharp.Community.DeepSerializable.Tests.Data;
using Xunit;

namespace PostSharp.Community.DeepSerializable.Tests
{
    public class DeepSerializableTests
    {
        [Fact]
        public void MainTest()
        {
            Player p1 = new Player()
            {
                Hero = new Creature(),
                ControlledCreatures = new []{ new Creature()}
            };
            GameState state = new GameState()
            {
                Players = new List<Player>()
                {
                    p1
                },
                MasterPlayer = p1,
                TurnsElapsed = 3,
                Weather = new Weather()
            };
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream serializationStream = new MemoryStream();
            bf.Serialize(serializationStream, state);
            serializationStream.Close();
        }
        
        [Fact]
        public void MainControlTest()
        {
            NonSerializablePlayer p1 = new NonSerializablePlayer()
            {
                Hero = new Creature(),
                ControlledCreatures = new []{ new Creature()}
            };
            NonDeepGameState state = new NonDeepGameState()
            {
                Players = new List<NonSerializablePlayer>()
                {
                    p1
                },
                MasterPlayer = p1,
                TurnsElapsed = 3,
                Weather = new Weather()
            };
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream serializationStream = new MemoryStream();
            Assert.Throws<SerializationException>(() =>
            {
                bf.Serialize(serializationStream, state);
                serializationStream.Close();
            });
        }
    }
}