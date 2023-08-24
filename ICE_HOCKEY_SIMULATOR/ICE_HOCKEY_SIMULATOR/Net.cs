using System;
namespace ICE_HOCKEY_SIMULATOR
{
	public class Net
	{
        public (int X, int Y) DownPosition { get; private set; }
        public (int X, int Y) UpPosition { get; private set; }

        public Player GoalKeeper { get; set; }

        public Net((int X, int Y) downPosition, (int X, int Y) upPosition, Player goalkeeper)
        {
            DownPosition = downPosition;
            UpPosition = upPosition;
            GoalKeeper = goalkeeper;
        }
    }
}

