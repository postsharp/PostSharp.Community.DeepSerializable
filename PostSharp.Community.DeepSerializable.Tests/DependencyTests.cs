using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using PostSharp.Community.DeepSerializable.Tests.Data;
using PostSharp.Community.DeepSerializable.Tests.Dependency;
using Xunit;

namespace PostSharp.Community.DeepSerializable.Tests
{
    public class DependencyTests
    {
        [Fact]
        public void OutsiderNotSerializable()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream serializationStream = new MemoryStream();
            Assert.Throws<SerializationException>(() =>
            {
                bf.Serialize(serializationStream, new ContainsOutsider());
                serializationStream.Close();
            });
        }

        [Fact]
        public void NonSerializedOutsiderIsOk()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream serializationStream = new MemoryStream();
            bf.Serialize(serializationStream, new ContainsNonSerializedOutsider());
            serializationStream.Close();
        }
        
    }

    [DeepSerializable]
    public class ContainsOutsider
    {
        public GameState GameState { get; set; } = new GameState();
        public Outsider Outsider { get; set; } = new Outsider();
    }
    [DeepSerializable]
    public class ContainsNonSerializedOutsider
    {
        public GameState GameState { get; set; } = new GameState();
        [field:NonSerialized]
        public Outsider Outsider { get; set; } = new Outsider();
    }
}