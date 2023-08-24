using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;

namespace ICE_HOCKEY_SIMULATOR;

public partial class Simulation : Window
{
    public Team? Team1 { get; set; }
    public Team? Team2 { get; set; }

    private int team1Goals = 0;
    private int team2Goals = 0;

    private int team1Attack = 3;
    private int team2Attack = 3;

    private int team1ShotsPasses = 3;
    private int team2ShotsPasses = 3;

    private IceRink? iceRink;

    public Simulation()
    {
        InitializeComponent();
        this.WindowState = WindowState.FullScreen;
    }

    private void ScoreView()
    {
        ScoreTeam1.Text = team1Goals.ToString();
        ScoreTeam2.Text = team2Goals.ToString();
    }

    public void SetView()
    {
        SetLogo(Team1!.Logo, LogoTeam1);
        SetLogo(Team2!.Logo, LogoTeam2);
        ScoreView();
    }

    private void SetLogo(string path, Image image)
    {
        try
        {
            image.Source = new Bitmap(path);
        }
        catch (Exception) { }
    }

    private void IncValue(ref int value, TextBlock valueTextBlock)
    {
        if (value < 5)
        {
            value++;
            valueTextBlock.Text = value.ToString();
        }
    }

    private void DecValue(ref int value, TextBlock valueTextBlock)
    {
        if (value > 1)
        {
            value--;
            valueTextBlock.Text = value.ToString();
        }
    }

    private void ButtonIncAttackTeam1(object sender, RoutedEventArgs e)
    {
        IncValue(ref team1Attack, AttackValue);
    }

    private void ButtonDecAttackTeam1(object sender, RoutedEventArgs e)
    {
        DecValue(ref team1Attack, AttackValue);
    }

    private void ButtonIncShotsTeam1(object sender, RoutedEventArgs e)
    {
        IncValue(ref team1ShotsPasses, ShotPassValue);
        if (Player.team1Attack < 5)
        {
            Player.team1Attack++;
        }
    }

    private void ButtonDecShotsTeam1(object sender, RoutedEventArgs e)
    {
        DecValue(ref team1ShotsPasses, ShotPassValue);
        if (Player.team1Attack > 1)
        {
            Player.team1Attack--;
        }
    }

    private void ButtonFinishSimulation(object sender, RoutedEventArgs e)
    {
        if (iceRink != null)
        {
            iceRink!.macAudioPlayer.Stop();
        }
        this.Close();
    }

    private void ButtonIncAttackTeam2(object sender, RoutedEventArgs e)
    {
        IncValue(ref team2Attack, AttackValueTeam2);
    }

    private void ButtonDecAttackTeam2(object sender, RoutedEventArgs e)
    {
        DecValue(ref team2Attack, AttackValueTeam2);
    }

    private void ButtonIncShotsTeam2(object sender, RoutedEventArgs e)
    {
        IncValue(ref team2ShotsPasses, ShotPassValueTeam2);
        if (Player.team2Attack < 5)
        {
            Player.team2Attack++;
        }
    }

    private void ButtonDecShotsTeam2(object sender, RoutedEventArgs e)
    {
        DecValue(ref team2ShotsPasses, ShotPassValueTeam2);
        if (Player.team2Attack > 1)
        {
            Player.team2Attack--;
        }
    }

    private void ButtonStartSimulation(object sender, RoutedEventArgs e)
    {
        Play();
    }

    private async void Play()
    {
        IceRink iceRink = new IceRink(Team1!, Team2!, TextComments, TimeSign);
        this.iceRink = iceRink;
        iceRink.IncreaseTeam1Score += ScoreGoalTeam1;
        iceRink.IncreaseTeam2Score += ScoreGoalTeam2;
        iceRink.SetCheckers();
        iceRink.SetEvents();
        await iceRink.Play();
    }

    private void ScoreGoal(ref int goals, TextBlock goalsTextBlock, StackPanel goalsList, Player player)
    {
        goals++;
        goalsTextBlock.Text = goals.ToString();
        TextBlock line = new TextBlock();
        line.Text = $"{player.FirstName} {player.Surname}";
        goalsList.Children.Add(line);
    }

    private void ScoreGoalTeam1(Player player)
    {
        ScoreGoal(ref team1Goals, ScoreTeam1, GoalsTeam1, player);
    }

    private void ScoreGoalTeam2(Player player)
    {
        ScoreGoal(ref team2Goals, ScoreTeam2, GoalsTeam2, player);
    }
}
