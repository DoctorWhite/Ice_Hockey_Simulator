using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ICE_HOCKEY_SIMULATOR;

public partial class MainWindow : Window
{
    public Team? Team1 { get; set; } = null;
    public Team? Team2 { get; set; } = null;

    public MainWindow()
    {
        InitializeComponent();
        this.WindowState = WindowState.FullScreen;
        TeamsDbOperations.CreateTable();
        this.FindControl<Button>("ChangeTeamsButton").Click += ChangeTeamsButtonClick!;
        ShowImage("DominikFurch.jpg", Furch);
    }

    private void ShowImage(string path, Image image)
    {
        try
        {
            var imageSource = new Bitmap(path);
            image.Source = imageSource;
        }
        catch (Exception) { }
    }

    private void StartButton(object sender, RoutedEventArgs e)
    {
        if (Team1 != null && Team2 != null && Team1.CanPlay() && Team2.CanPlay())
        {
            var simulationWindow = new Simulation();
            simulationWindow.Team1 = Team1;
            simulationWindow.Team2 = Team2;
            simulationWindow.SetView();
            simulationWindow.Show();    
        }
    }

    private void ShowPlayers()
    {
        Team1Players.Children.Clear();
        Team2Players.Children.Clear();
        ShowPLayersFromTeam(Team1, Team1Players);
        ShowPLayersFromTeam(Team2, Team2Players);
    }

    private void ShowPLayersFromTeam(Team? team, StackPanel stackPanel)
    {
        if (team != null)
        {
            TextBlock playerColumns = new TextBlock();
            TextBlock teamColumns = new TextBlock();
            playerColumns.Text = "T-ID = TeamID\nN = Number\nSS = SkatingSpeed\nSR = ShootRating\nPR = PassRating\nDR = DefenceRating\nFOR = FaceOffRating\nPOS = Position\n\n\n";
            playerColumns.FontSize = 15;
            stackPanel.Children.Add(playerColumns);

            ShowPlayers(team.Left_Attackers, stackPanel);
            ShowPlayers(team.Right_Attackers, stackPanel);
            ShowPlayers(team.Centers, stackPanel);
            ShowPlayers(team.Defenders, stackPanel);
        }
    }

    private void ShowPlayers(List<Player> players, StackPanel stackPanel)
    {
        foreach (Player player in players)
        {
            ShowPlayer(player, stackPanel);
        }
    }

    private void ShowPlayer(Player player, StackPanel stackPanel)
    {
        TextBlock playerData = new TextBlock();
        playerData.FontSize = 15;
        playerData.Text = player.ToString();
        stackPanel.Children.Add(playerData);
    }

    private async void ChangeTeamsButtonClick(object sender, RoutedEventArgs e)
    {
        var changeTeamsWindow = new ChangeTeam();
        changeTeamsWindow.SetTeamsEvent += SetTeams;
        await changeTeamsWindow.ShowDialog(this);
        ShowPlayers();
    }

    private void ExitButton(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    public void SetTeams(Team? team1, Team? team2)
    {
        Team1 = team1;
        Team2 = team2;
        if (Team1 != null)
        {
            TeamCity1.Text = Team1.City;
            TeamName1.Text = Team1.Name;
            ShowImage(Team1.Logo, LogoTeam1);
        }
        if (Team2 != null)
        {
            TeamCity2.Text = Team2.City;
            TeamName2.Text = Team2.Name;
            ShowImage(Team2.Logo, LogoTeam2);
        }
    }
}
