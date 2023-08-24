using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace ICE_HOCKEY_SIMULATOR
{
    public static class TeamsDbOperations
    {
        public static string DBfile { get; private set; } = "URI=file:TeamsDB.db";

        public static void CreateTable()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBfile))
            {
                connection.Open();
                string createTeamTableQuery = "CREATE TABLE IF NOT EXISTS Teams (ID INTEGER PRIMARY KEY AUTOINCREMENT, NAME TEXT NOT NULL, CITY TEXT NOT NULL, LOGO TEXT NOT NULL);";
                string createPlayerTableQuery = "CREATE TABLE IF NOT EXISTS Players (ID INTEGER PRIMARY KEY AUTOINCREMENT, FIRSTNAME TEXT NOT NULL, SURNAME TEXT NOT NULL, NUMBER INTEGER, SKATING_SPEED INTEGER, SHOOT_RATING INTEGER, PASS_RATING INTEGER, DEFENCE_RATING INTEGER, FACE_OFF_RATING INTEGER, TEAM_ID INTEGER, POSITION TEXT NOT NULL ,FOREIGN KEY(TEAM_ID) REFERENCES Teams(ID));";

                using (SQLiteCommand cmd = new SQLiteCommand(createTeamTableQuery, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                using (SQLiteCommand cmd = new SQLiteCommand(createPlayerTableQuery, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void AddRow(Team team)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBfile))
            {
                connection.Open();
                string addPerson = $"insert into Teams (NAME, CITY, LOGO) values ('{team.Name}', '{team.City}', '{team.Logo}');";

                using (SQLiteCommand cmd = new SQLiteCommand(addPerson, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void AddRow(Player player)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBfile))
            {
                connection.Open();
                string addPerson = $"insert into Players (FIRSTNAME, SURNAME, NUMBER, SKATING_SPEED, SHOOT_RATING, PASS_RATING, DEFENCE_RATING, FACE_OFF_RATING, TEAM_ID, POSITION) " +
                    $"values ('{player.FirstName}', '{player.Surname}', {player.Number}, {player.SkatingSpeed}, {player.ShootRating}, {player.PassRating}, {player.DefenceRating}, {player.FaceOffRating}, {player.TeamId}, '{player.Position.ToString()}');";

                using (SQLiteCommand cmd = new SQLiteCommand(addPerson, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateRow(int id, object newValue, UpdateAttr updateAttr, Entity entity)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBfile))
            {
                connection.Open();
                string update;
                if (entity == Entity.Players)
                {
                    update = $"update {entity.ToString()} set {updateAttr.ToString()}={newValue} where ID={id}";
                }
                else
                {
                    update = $"update {entity.ToString()} set {updateAttr.ToString()}='{newValue.ToString()}' where ID={id}";
                }

                using (SQLiteCommand cmd = new SQLiteCommand(update, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteRow(int id, Entity entity)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBfile))
            {
                connection.Open();
                string delPerson = $"delete from {entity.ToString()} where id={id};";
                using (SQLiteCommand cmd = new SQLiteCommand(delPerson, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static Team? GetTeam(int id)
        {
            Team? team = null;
            using (SQLiteConnection connection = new SQLiteConnection(DBfile))
            {
                connection.Open();
                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT * FROM teams WHERE id = {id};", connection))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            team = new Team(reader.GetString(1), reader.GetString(3), reader.GetString(2));
                            team.Id = reader.GetInt32(0);
                            return team;
                        }
                    }
                }
            }
            return team;
        }

        public static List<Player> GetTeamPlayers(int id)
        {
            List<Player> players = new List<Player>();
            using (SQLiteConnection connection = new SQLiteConnection(DBfile))
            {
                connection.Open();
                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT * FROM Players WHERE TEAM_ID = {id};", connection))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Position pos = (Position)Enum.Parse(typeof(Position), reader.GetString(10));
                            Player player = new Player(reader.GetInt32(9), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6), reader.GetInt32(7), reader.GetInt32(8), pos);
                            player.Id = reader.GetInt32(0);
                            players.Add(player);
                        }
                    }
                }
            }
            return players;
        }

        public static (List<Team>, List<Player>) GetData()
        {
            List<Team> teams = new List<Team>();
            List<Player> players = new List<Player>();
            using (SQLiteConnection connection = new SQLiteConnection(DBfile))
            {
                connection.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Teams;", connection))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Team team = new Team(reader.GetString(1), reader.GetString(3), reader.GetString(2));
                            team.Id = reader.GetInt32(0);
                            teams.Add(team);
                        }
                    }
                }
            }
            using (SQLiteConnection connection = new SQLiteConnection(DBfile))
            {
                connection.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Players;", connection))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Position pos = (Position)Enum.Parse(typeof(Position), reader.GetString(10));
                            Player player = new Player(reader.GetInt32(9), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6), reader.GetInt32(7), reader.GetInt32(8), pos);
                            player.Id = reader.GetInt32(0);
                            players.Add(player);
                        }
                    }
                }
            }
            return (teams, players);
        }
    }
}