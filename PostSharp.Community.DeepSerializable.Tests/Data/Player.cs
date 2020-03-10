using System;

namespace PostSharp.Community.DeepSerializable.Tests.Data
{
    public class Player
    {
        public Creature Hero;

        public Creature[] ControlledCreatures;

        public Player()
        {
            Console.WriteLine("New player created.");
        }
    }
    
    public class NonSerializablePlayer
    {
        public Creature Hero;

        public Creature[] ControlledCreatures;

        public NonSerializablePlayer()
        {
            Console.WriteLine("New player created.");
        }
    }
}