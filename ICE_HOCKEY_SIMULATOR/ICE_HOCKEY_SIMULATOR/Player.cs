using System;
using Avalonia.X11;
using System.Threading.Tasks;
using System.Data;

namespace ICE_HOCKEY_SIMULATOR
{
	public class Player
	{
        public int Id { get; set; }
        public int TeamId { get; private set; }

        public string FirstName { get; private set; }
        public string Surname { get; private set; }
        public int Number { get; private set; }

        public int SkatingSpeed { get; private set; }
        public int ShootRating { get; private set; }
        public int PassRating { get; private set; }
        public int DefenceRating { get; private set; }
        public int FaceOffRating { get; private set; }

        public Position Position { get; set; }

        public bool HasPuck { get; set; } = false;
        public (int X, int Y) Coordinates { get; set; }
        public (int X, int Y) WantedCoordinates { get; set; } = (-1, -1);

        public Player? PlayerToDefend;
        public bool Stop { get; set; } = false;
        public int Team { get; set; } = 1;

        public EPlayerStatus playerStatus { get; set; } = EPlayerStatus.IDLE;
        private static Random random = new Random();

        public event Func<Player, Task<EShotResult>>? ShootEvent;
        public event Func<Player, Task<bool>>? PassEvent;
        public event Func<Player, bool>? TakePuck;

        public static int team1Attack { get; set; } = 3;
        public static int team2Attack { get; set; } = 3;

        public Player(int teamId, string firstName, string surname, int number, int skatingSpeed, int shootRating, int passRating, int defenceRating, int faceOffRating, Position position)
        {
            TeamId = teamId;
            FirstName = firstName;
            Surname = surname;
            Number = number;
            SkatingSpeed = skatingSpeed;
            ShootRating = shootRating;
            PassRating = passRating;
            DefenceRating = defenceRating;
            FaceOffRating = faceOffRating;
            Position = position;
        }

        public override string ToString()
        {
            return $"{Id} {FirstName} {Surname} T-ID: {TeamId} N: {Number} SS: {SkatingSpeed} SR: {ShootRating} PR: {PassRating} DR: {DefenceRating} FOR: {FaceOffRating} POS: {Position}";
        }

        private (int, int) nextPosition((int X, int Y) coordinates)
        {
            int newX = Coordinates.X;
            int newY = Coordinates.Y;

            if (coordinates.Y < Coordinates.Y)
            {
                newY = Coordinates.Y - 1;
            }
            else if (coordinates.Y > Coordinates.Y)
            {
                newY = Coordinates.Y + 1;
            }
            if (coordinates.X < Coordinates.X)
            {
                newX = Coordinates.X - 1;
            }
            else if (coordinates.X > Coordinates.X)
            {
                newX = Coordinates.X + 1;
            }
            return (newX, newY);
        }

        public async Task OneMove((int X, int Y) position)
        {
            switch (playerStatus)
            {
                case EPlayerStatus.DEFENCE:
                    await Task.Delay(1000 / (5 * SkatingSpeed / 100));
                    break;
                case EPlayerStatus.COUNTERATTACK:
                    await Task.Delay(1000 / (8 * SkatingSpeed / 100));
                    break;
                case EPlayerStatus.ATTACK:
                    await Task.Delay(1000 / (5 * SkatingSpeed / 100));
                    break;
                case EPlayerStatus.GOING_FOR_PUCK:
                    await Task.Delay(1000 / (8 * SkatingSpeed / 100));
                    break;
            }
            Coordinates = nextPosition(position);
        }

        private void wantedPositionInDefence()
        {
            WantedCoordinates = (random.Next(0, 3) - 1 + PlayerToDefend.Coordinates.X,
                random.Next(0, 3) - 1 + PlayerToDefend.Coordinates.Y);
        }

        private void wantedPositionToAttack()
        {
            switch (Position)
            {
                case Position.LEFT_ATTACKER:
                    WantedCoordinates = Team == 1 ? (random.Next(44, 56), random.Next(0, 10)) : (random.Next(4, 16), random.Next(20, 30));
                    break;
                case Position.CENTER:
                    WantedCoordinates = Team == 1 ? (random.Next(44, 56), random.Next(10, 20)) : (random.Next(4, 16), random.Next(10, 20));
                    break;
                case Position.RIGHT_ATTACKER:
                    WantedCoordinates = Team == 1 ? (random.Next(44, 56), random.Next(20, 30)) : (random.Next(4, 16), random.Next(0, 10));
                    break;
                case Position.DEFENDER:
                    WantedCoordinates = Team == 1 ? (random.Next(38, 44), random.Next(0, 15)) : (random.Next(16, 22), random.Next(15, 30));
                    break;
                case Position.DEFENDER_2:
                    WantedCoordinates = Team == 1 ? (random.Next(38, 44), random.Next(15, 30)) : (random.Next(16, 22), random.Next(0, 15));
                    break;
            }
        }

        private int ShootOrPass(int value)
        {
            int zeroOrOne = random.Next(3);
            if (value == 5)
            {
                return 0;
            }
            if (value == 2 || value == 1)
            {
                return zeroOrOne == 0 ? 0 : 1;
            }
            if (value == 4)
            {
                return zeroOrOne == 0 ? 1 : 0;
            }
            return random.Next(2);
        }

        public async Task<bool> Play()
        {
            while (!Stop)
            {
                if (WantedCoordinates == (-1, -1) || playerStatus == EPlayerStatus.DEFENCE || playerStatus == EPlayerStatus.ATTACK)
                {
                    switch (playerStatus)
                    {
                        case EPlayerStatus.DEFENCE:
                            wantedPositionInDefence();
                            break;
                        case EPlayerStatus.COUNTERATTACK:
                            wantedPositionToAttack();
                            break;
                        case EPlayerStatus.ATTACK:
                            wantedPositionToAttack();
                            break;
                    }
                }
                if (Stop)
                {
                    break;
                }
                await OneMove(WantedCoordinates);
                if (playerStatus == EPlayerStatus.GOING_FOR_PUCK)
                {
                    TakePuck.Invoke(this);
                }
                else if (Coordinates == WantedCoordinates && HasPuck)
                {
                    int whatToDo;
                    if (Team == 1)
                    {
                        whatToDo = ShootOrPass(team1Attack);
                    }
                    else
                    {
                        whatToDo = ShootOrPass(team2Attack);
                    }
                    if (whatToDo == 0)
                    {
                        EShotResult eShotResult = await ShootEvent!.Invoke(this);
                        if (eShotResult == EShotResult.GOAL)
                        {
                            return true;
                        }
                        else if (eShotResult == EShotResult.SAVE_STOP)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        await PassEvent!.Invoke(this);
                    }
                }
            }
            return false;
        }
    }
}

