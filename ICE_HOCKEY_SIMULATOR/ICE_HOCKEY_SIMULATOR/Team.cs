using System;
using System.Collections.Generic;

namespace ICE_HOCKEY_SIMULATOR
{
	public class Team
	{
		public string Name { get; private set; }
		public int Id { get; set; }
		public string Logo { get; private set; }
		public string City { get; private set; }

		public List<Player> Left_Attackers { get; private set; }
		public List<Player> Right_Attackers { get; private set; }
		public List<Player> Centers { get; private set; }
		public List<Player> Defenders { get; private set; }
		public List<Player> Goalkeepers { get; private set; }

		public Team(string name, string logo, string city)
		{
            Name = name;
			City = city;
			Logo = logo;
			Left_Attackers = new List<Player>();
			Right_Attackers = new List<Player>();
			Centers = new List<Player>();
			Defenders = new List<Player>();
			Goalkeepers = new List<Player>();
        }

		public void AddPlayer(Player player)
		{
			switch (player.Position)
			{
				case Position.CENTER:
					Centers.Add(player);
					break;
				case Position.LEFT_ATTACKER:
					Left_Attackers.Add(player);
					break;
				case Position.RIGHT_ATTACKER:
					Right_Attackers.Add(player);
					break;
				case Position.DEFENDER:
					Defenders.Add(player);
					break;
				case Position.GOALKEEPER:
					Goalkeepers.Add(player);
					break;
			}
		}

        public override string ToString()
        {
            return $"{Id} {Name} {City}";
		}

		public bool CanPlay()
		{
			return Left_Attackers.Count > 0 &&
				Right_Attackers.Count > 0 &&
				Centers.Count > 0 &&
				Defenders.Count > 0 &&
				Goalkeepers.Count > 0;
        }

		public List<Player> GetPlayers()
		{
			List<Player> players = new List<Player>();
			players.AddRange(Left_Attackers);
            players.AddRange(Right_Attackers);
            players.AddRange(Centers);
            players.AddRange(Defenders);
            players.AddRange(Goalkeepers);
			return players;
        }
    }
}

