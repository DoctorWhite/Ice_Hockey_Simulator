using System;
namespace ICE_HOCKEY_SIMULATOR
{
	public class Puck
	{
        public (int X, int Y) Position { get; set; }
        public Player? Holder { get; set; }

        private static Random random = new Random();

        public void ChangePosToRandom(int where)
        {
            if (where == 1)
            {
                Position = (random.Next(0, 22), random.Next(0, 30));
            }
            else
            {
                Position = (random.Next(38, 60), random.Next(0, 30));
            }
        }

        public Puck()
        {
        }
    }
}

