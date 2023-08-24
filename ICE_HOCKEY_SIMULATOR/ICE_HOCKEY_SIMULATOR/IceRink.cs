using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using System.Diagnostics;

namespace ICE_HOCKEY_SIMULATOR
{
    public class IceRink
    {
        public int Length { get; private set; } = 60;
        public int Width { get; private set; } = 30;

        public FaceOffRink CenterFaceOffRing { get; private set; }
        public FaceOffRink LeftUpFaceOffRing { get; private set; }
        public FaceOffRink LeftDownFaceOffRing { get; private set; }
        public FaceOffRink RightUpFaceOffRing { get; private set; }
        public FaceOffRink RightDownFaceOffRing { get; private set; }

        public Net LeftNet { get; private set; }
        public Net RightNet { get; private set; }

        // TEAM 1
        public Player LeftAttack1 { get; set; }
        public Player RightAttack1 { get; set; }
        public Player Center1 { get; set; }
        public Player LeftDef1 { get; set; }
        public Player RightDef1 { get; set; }
        public Player Goalkeeper1 { get; set; }
        public List<Player> PlayersTeam1 { get; private set; }

        // TEAM 2
        public Player LeftAttack2 { get; set; }
        public Player RightAttack2 { get; set; }
        public Player Center2 { get; set; }
        public Player LeftDef2 { get; set; }
        public Player RightDef2 { get; set; }
        public Player Goalkeeper2 { get; set; }
        public List<Player> PlayersTeam2 { get; private set; }

        public Puck Puck { get; private set; } = new Puck();

        private static Random random = new Random();

        public StackPanel Comments { get; private set; }

        public FaceOffRink NextFaceOff { get; private set; }

        public event Action<Player>? IncreaseTeam1Score;
        public event Action<Player>? IncreaseTeam2Score;

        public Stopwatch Time { get; private set; }
        public TextBlock TimeSign { get; private set; }

        public MacAudioPlayer macAudioPlayer = new MacAudioPlayer();

        public IceRink(Team team1, Team team2, StackPanel comment, TextBlock timeSign)
        {
            CenterFaceOffRing = new FaceOffRink((30, 15),
                (29, 15), (29, 11), (29, 19), (26, 11), (26, 19),
                (31, 15), (31, 19), (31, 11), (34, 19), (34, 11));
            LeftUpFaceOffRing = new FaceOffRink((10, 8),
                (9, 8), (9, 4), (9, 12), (6, 4), (6, 12),
                (11, 8), (11, 12), (11, 4), (14, 12), (14, 4));
            LeftDownFaceOffRing = new FaceOffRink((10, 22),
                (9, 22), (9, 19), (9, 26), (6, 18), (6, 26),
                (11, 22), (11, 26), (11, 19), (14, 26), (14, 19));
            RightUpFaceOffRing = new FaceOffRink((50, 8),
                (49, 8), (49, 4), (49, 12), (46, 4), (46, 12),
                (51, 8), (51, 12), (51, 4), (54, 12), (54, 4));
            RightDownFaceOffRing = new FaceOffRink((50, 22),
                (49, 22), (49, 19), (49, 26), (46, 19), (46, 26),
                (51, 22), (51, 26), (51, 19), (54, 26), (54, 19));

            NextFaceOff = CenterFaceOffRing;

            PlayersTeam1 = team1.GetPlayers();
            PlayersTeam2 = team2.GetPlayers();

            LeftAttack1 = team1.Left_Attackers.First();
            RightAttack1 = team1.Right_Attackers.First();
            Center1 = team1.Centers.First();
            LeftDef1 = team1.Defenders.First();
            RightDef1 = team1.Defenders.Last();
            Goalkeeper1 = team1.Goalkeepers.First();
            RightDef1.Position = Position.DEFENDER_2;

            LeftAttack2 = team2.Left_Attackers.First();
            RightAttack2 = team2.Right_Attackers.First();
            Center2 = team2.Centers.First();
            LeftDef2 = team2.Defenders.First();
            RightDef2 = team2.Defenders.Last();
            Goalkeeper2 = team2.Goalkeepers.First();
            LeftDef2.Position = Position.DEFENDER_2;

            PlayersTeam2.ForEach(player => player.Team = 2);

            LeftNet = new Net((4, 16), (4, 14), Goalkeeper1);
            RightNet = new Net((56, 16), (56, 14), Goalkeeper2);

            Goalkeeper1.Coordinates = ((LeftNet.DownPosition.Y + LeftNet.UpPosition.Y) / 2, LeftNet.DownPosition.X + 1);
            Goalkeeper2.Coordinates = ((RightNet.DownPosition.Y + RightNet.UpPosition.Y) / 2, RightNet.DownPosition.X - 1);

            Comments = comment;
            Time = new Stopwatch();
            TimeSign = timeSign;
        }

        public void ResetPlayers()
        {
            PlayersTeam1.ForEach(player =>
            {
                player.Team = 1;
                player.HasPuck = false;
                player.WantedCoordinates = (-1, -1);
                player.Stop = false;
            });
            PlayersTeam2.ForEach(player =>
            {
                player.Team = 2;
                player.HasPuck = false;
                player.WantedCoordinates = (-1, -1);
                player.Stop = false;
            });
        }

	    public async Task<EShotResult> Shoot(Player player)
        {
            Player goalkeeper = player.Team == 1 ? RightNet.GoalKeeper : LeftNet.GoalKeeper;
            AddComment($"{player.Surname} shots!");
            int tipSport = (100 - goalkeeper.DefenceRating + random.Next(-5, 6)) * player.ShootRating / 100;
            await Task.Delay(500);
            await Task.Delay((int) (100 / (50000 / Distance(player.Coordinates, goalkeeper.Coordinates) * (100 / player.ShootRating))));
            if (random.Next(1, 101) < tipSport)
            {
                player.HasPuck = false;
                Puck.Holder = null;
                AddComment($"SCOOOOOOOOOOOOOORES! {player.Surname}!");
                if (player.Team == 1)
                {
                    IncreaseTeam1Score!.Invoke(player);
                }
                else
                {
                    IncreaseTeam2Score!.Invoke(player);
                }
                NextFaceOff = CenterFaceOffRing;
                return EShotResult.GOAL;
            }
            else
            {
                Puck.Position = goalkeeper.Coordinates;
                if (random.Next(2) == 1)
                {
                    PuckSomewhere(goalkeeper, player);
                    AddComment($"{goalkeeper.Surname} made a save!");                    
                    return EShotResult.SAVE_CONTINUE;
                }
                if (player.Team == 1)
                    NextFaceOff = random.Next(2) == 0 ? RightDownFaceOffRing : RightUpFaceOffRing;
                else
                    NextFaceOff = random.Next(2) == 0 ? LeftDownFaceOffRing : LeftUpFaceOffRing;
                AddComment($"{goalkeeper.Surname} saved the puk!");
                return EShotResult.SAVE_STOP;
            }
        }

        private double Distance((int X, int Y) pos1, (int X, int Y) pos2)
        {
            int x = Math.Abs(pos1.X - pos2.X);
            int y = Math.Abs(pos1.Y - pos2.Y);
            return Math.Sqrt(x * x + y * y);
        }

        public void PuckSomewhere(Player goalkeeper, Player player)
        {
            Puck.ChangePosToRandom(goalkeeper.Team);
            Puck.Holder = null;
            player.HasPuck = false;
            setStatusToPlayers(EPlayerStatus.GOING_FOR_PUCK, EPlayerStatus.GOING_FOR_PUCK);
            PlayersTeam1.ForEach(p => p.WantedCoordinates = Puck.Position);
            PlayersTeam2.ForEach(p => p.WantedCoordinates = Puck.Position);
        }

        private static readonly object lockObject = new object();

        public bool GivePuckToPlayer(Player player)
        {
            if (player.Coordinates == Puck.Position)
            {
                lock (lockObject)
                {
                    if (Puck.Holder == null)
                    {
                        player.HasPuck = true;
                        Puck.Holder = player;
                        if (player.Team == 1)
                        {
                            setStatusToPlayers(EPlayerStatus.COUNTERATTACK, EPlayerStatus.DEFENCE);
                        }
                        else
                        {
                            setStatusToPlayers(EPlayerStatus.DEFENCE, EPlayerStatus.COUNTERATTACK);
                        }
                        AddComment($"{player.Surname} takes the puck!");
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        public async Task<bool> Pass(Player player)
        {
            Player toPlayer;
            if (player.Team == 1)
            {
                switch (random.Next(5))
                {
                    case 1:
                        toPlayer = LeftAttack1;
                        break;
                    case 2:
                        toPlayer = RightAttack1;
                        break;
                    case 3:
                        toPlayer = Center1;
                        break;
                    case 4:
                        toPlayer = LeftDef1;
                        break;
                    default:
                        toPlayer = RightDef1;
                        break;
                }
            }
            else
            {
                switch (random.Next(5))
                {
                    case 1:
                        toPlayer = LeftAttack2;
                        break;
                    case 2:
                        toPlayer = RightAttack2;
                        break;
                    case 3:
                        toPlayer = Center2;
                        break;
                    case 4:
                        toPlayer = LeftDef2;
                        break;
                    default:
                        toPlayer = RightDef2;
                        break;
                }
            }
            await Task.Delay(1000);
            player.HasPuck = false;
            toPlayer.HasPuck = true;
            Puck.Holder = toPlayer;
            await Task.Delay((int)(100 / (30000 / Distance(player.Coordinates, toPlayer.Coordinates) * (100 / player.PassRating))));
            //await Task.Delay((int) (30000 / Distance(player.Coordinates, toPlayer.Coordinates) + 1));
            AddComment($"{player.Surname} to {toPlayer.Surname}");
            return true;
        }

        public void SetCheckers()
        {
            LeftAttack1.PlayerToDefend = RightDef2;
            Center1.PlayerToDefend = Center2;
            RightAttack1.PlayerToDefend = LeftDef2;
            LeftDef1.PlayerToDefend = RightAttack2;
            RightDef1.PlayerToDefend = LeftAttack2;

            LeftAttack2.PlayerToDefend = RightDef1;
            Center2.PlayerToDefend = Center1;
            RightAttack2.PlayerToDefend = LeftDef1;
            LeftDef2.PlayerToDefend = RightAttack1;
            RightDef2.PlayerToDefend = LeftAttack1;
        }

        public void SetEvents()
        {
            PlayersTeam1.ForEach(player => {
                player.ShootEvent += Shoot;
                player.PassEvent += Pass;
                player.TakePuck += GivePuckToPlayer;
                });
            PlayersTeam2.ForEach(player => {
                player.ShootEvent += Shoot;
                player.PassEvent += Pass;
                player.TakePuck += GivePuckToPlayer;
                });
        }

        private void setStatusToPlayers(EPlayerStatus status, EPlayerStatus status2)
        {
            PlayersTeam1.ForEach(player => player.playerStatus = status);
            PlayersTeam2.ForEach(player => player.playerStatus = status2);
        }

        public void PrepareFaceOff(FaceOffRink faceOffRink)
        {
            Center1!.Coordinates = faceOffRink.PositionCenter1;
            LeftAttack1!.Coordinates = faceOffRink.PositionLeftWing1;
            RightAttack1!.Coordinates = faceOffRink.PositionRightWing1;
            LeftDef1!.Coordinates = faceOffRink.PositionLeftDef1;
            RightDef1!.Coordinates = faceOffRink.PositionRightDef1;

            Center2!.Coordinates = faceOffRink.PositionCenter2;
            LeftAttack2!.Coordinates = faceOffRink.PositionLeftWing2;
            RightAttack2!.Coordinates = faceOffRink.PositionRightWing2;
            LeftDef2!.Coordinates = faceOffRink.PositionLeftDef2;
            RightDef2!.Coordinates = faceOffRink.PositionRightDef2;
        }

        public void FaceOff()
        {
            int a = random.Next(100) + Center1!.FaceOffRating;
            int b = random.Next(100) + Center2!.FaceOffRating;

            Player foWinner = a >= b ? Center1 : Center2;

            int where = a >= b ? random.Next(4) : random.Next(4, 8);

            switch (where)
            {
                case 0:
                    Puck.Holder = LeftAttack1;
                    break;
                case 1:
                    Puck.Holder = RightAttack1;
                    break;
                case 2:
                    Puck.Holder = LeftDef1;
                    break;
                case 3:
                    Puck.Holder = RightDef1;
                    break;
                case 4:
                    Puck.Holder = LeftAttack2;
                    break;
                case 5:
                    Puck.Holder = RightAttack2;
                    break;
                case 6:
                    Puck.Holder = LeftDef2;
                    break;
                case 7:
                    Puck.Holder = RightDef2;
                    break;
            }
            Puck.Holder!.HasPuck = true;
            if (Center2 == foWinner)
                setStatusToPlayers(EPlayerStatus.DEFENCE, EPlayerStatus.COUNTERATTACK);
            if (Center1 == foWinner)
                setStatusToPlayers(EPlayerStatus.COUNTERATTACK, EPlayerStatus.DEFENCE);
            AddComment($"{foWinner.Surname} won the face-off, {Puck.Holder!.Surname} has a puck!");
        }

        public void AddComment(string text)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.FontSize = 20;
            Comments.Children.Insert(0, textBlock);
        }

        public async Task PlayPart(FaceOffRink faceOffRink)
        {
            ResetPlayers();
            PrepareFaceOff(faceOffRink);
            FaceOff();
            Task[] tasks = new Task[10];
            tasks[0] = LeftAttack1!.Play();
            tasks[1] = RightAttack1!.Play();
            tasks[2] = Center1!.Play();
            tasks[3] = LeftDef1!.Play();
            tasks[4] = RightDef1!.Play();
            tasks[5] = LeftAttack2!.Play();
            tasks[6] = RightAttack2!.Play();
            tasks[7] = Center2!.Play();
            tasks[8] = LeftDef2!.Play();
            tasks[9] = RightDef2!.Play();

            await Task.WhenAny(tasks);
            PlayersTeam1.ForEach(player => player.Stop = true);
            PlayersTeam2.ForEach(player => player.Stop = true);
            await Task.WhenAll(tasks);
        }

        public async Task TimeChecker()
        {
            while (true)
            {
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => TimeSign.Text = $"{Time.Elapsed.Minutes}:{Time.Elapsed.Seconds}");
                await Task.Delay(1000);                
                if (Time.ElapsedMilliseconds > 100000)
                {
                    PlayersTeam1.ForEach(player => player.Stop = true);
                    PlayersTeam2.ForEach(player => player.Stop = true);
                    break;
                }
            }
        }


        public void SectorE()
        {
            macAudioPlayer.PlaySound("borci_ze_statlu.wav");
        }

        public async Task Play()
        {
            SectorE();
            Task time = Task.Run(async () => TimeChecker());
            while (Time.ElapsedMilliseconds < 100000)
            {
                Time.Start();
                await PlayPart(NextFaceOff);
                AddComment("");
                Time.Stop();
                await Task.Delay(5000);
            }
            await time;
            AddComment("FINISH OF THE MATCH");
            macAudioPlayer.Stop();
            macAudioPlayer.PlaySound("Soundtrack_Oda_na_Kometu.wav");
        }
    }
}

