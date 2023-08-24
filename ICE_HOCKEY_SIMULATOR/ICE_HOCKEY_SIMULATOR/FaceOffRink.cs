using System;
namespace ICE_HOCKEY_SIMULATOR
{
	public class FaceOffRink
	{
        public (int X, int Y) Position { get; private set; }
        public (int X, int Y) PositionCenter1 { get; private set; }
        public (int X, int Y) PositionLeftWing1 { get; private set; }
        public (int X, int Y) PositionRightWing1 { get; private set; }
        public (int X, int Y) PositionLeftDef1 { get; private set; }
        public (int X, int Y) PositionRightDef1 { get; private set; }

        public (int X, int Y) PositionCenter2 { get; private set; }
        public (int X, int Y) PositionLeftWing2 { get; private set; }
        public (int X, int Y) PositionRightWing2 { get; private set; }
        public (int X, int Y) PositionLeftDef2 { get; private set; }
        public (int X, int Y) PositionRightDef2 { get; private set; }

        public FaceOffRink((int X, int Y) position,
            (int X, int Y) positionCenter1,
            (int X, int Y) positionLeftWing1,
            (int X, int Y) positionRightWing1,
            (int X, int Y) positionLeftDef1,
            (int X, int Y) positionRightDef1,
            (int X, int Y) positionCenter2,
            (int X, int Y) positionLeftWing2,
            (int X, int Y) positionRightWing2,
            (int X, int Y) positionLeftDef2,
            (int X, int Y) positionRightDef2)
        {
            Position = position;
            PositionCenter1 = positionCenter1;
            PositionLeftWing1 = positionLeftWing1;
            PositionRightWing1 = positionRightWing1;
            PositionLeftDef1 = positionLeftDef1;
            PositionRightDef1 = positionRightDef1;
            PositionCenter2 = positionCenter2;
            PositionLeftWing2 = positionLeftWing2;
            PositionRightWing2 = positionRightWing2;
            PositionLeftDef2 = positionLeftDef2;
            PositionRightDef2 = positionRightDef2;
        }
    }
}

