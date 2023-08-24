using System;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Microsoft.Extensions.DependencyInjection;

namespace ICE_HOCKEY_SIMULATOR;

public partial class ChangeTeam : Window
{
    private StackPanel _addPlayerPanel;
    private StackPanel _editPlayerPanel;
    private StackPanel _deletePlayerPanel;
    private StackPanel _addTeamPanel;
    private StackPanel _editTeamPanel;
    private StackPanel _deleteTeamPanel;

    private Team? team1;
    private Team? team2;

    public delegate void TeamsHandler(Team? team1, Team? team2);
    public event TeamsHandler? SetTeamsEvent;

    MacAudioPlayer macAudioPlayer = new MacAudioPlayer();

    public ChangeTeam()
    {
        InitializeComponent();
        this.WindowState = WindowState.FullScreen;
        this.WindowState = WindowState.Maximized;

        _addPlayerPanel = this.FindControl<StackPanel>("addPlayerPanel");
        _editPlayerPanel = this.FindControl<StackPanel>("editPlayerPanel");
        _deletePlayerPanel = this.FindControl<StackPanel>("deletePlayerPanel");

        _addTeamPanel = this.FindControl<StackPanel>("addTeamPanel");
        _editTeamPanel = this.FindControl<StackPanel>("editTeamPanel");
        _deleteTeamPanel = this.FindControl<StackPanel>("deleteTeamPanel");

        var imageSource = new Bitmap("pastrnak_krejci.jpg");
        PastaKrejca.Source = imageSource;
        ShowDatabaseData();
    }

    public void ShowDatabaseData()
    {
        var (teams, players) = TeamsDbOperations.GetData();
        TextBlock playerColumns = new TextBlock();
        TextBlock teamColumns = new TextBlock();
        playerColumns.Text = "T-ID = TEAM_ID\nN = NUMBER\nSS = SKATING_SPEED\nSR = SHOOT_RATING\nPR = PASS_RATING\nDR = DEFENCE_RATING\nFOR = FACE_OFF_RATING\nPOS = POSITION\n\n\n";
        playerColumns.FontSize = 15;
        PlayersList.Children.Add(playerColumns);
        teamColumns.FontSize = 15;
        teamColumns.Text = "ID NAME CITY";
        TeamsList.Children.Add(teamColumns);

        foreach (Player player in players)
        {
            ShowData(player.ToString(), PlayersList);
        }
        foreach (Team team in teams)
        {
            ShowData(team.ToString(), TeamsList);
        }
    }

    private void ShowData(string text, StackPanel panel)
    {
        TextBlock item = new TextBlock();
        item.Text = text;
        item.FontSize = 15;
        panel.Children.Add(item);
    }

    public void ResetInputs()
    {        
        SolidColorBrush defaultColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#dbdbdb"));
        foreach (var child in Inputs.Children)
        {
            if (child is TextBox textBox)
            {
                textBox.Background = defaultColor;
                textBox.Text = string.Empty;
                textBox.RaiseEvent(new RoutedEventArgs(TextBox.TextInputEvent));
            }
        }
    }

    public void AddPlayerButton(object sender, RoutedEventArgs e)
    {
        _addPlayerPanel.IsVisible = !_addPlayerPanel.IsVisible;
    }

    public void EditPlayerButton(object sender, RoutedEventArgs e)
    {
        _editPlayerPanel.IsVisible = !_editPlayerPanel.IsVisible;
    }

    public void DeletePlayerButton(object sender, RoutedEventArgs e)
    {
        _deletePlayerPanel.IsVisible = !_deletePlayerPanel.IsVisible;
    }

    public void AddTeamButton(object sender, RoutedEventArgs e)
    {
        _addTeamPanel.IsVisible = !_addTeamPanel.IsVisible;
    }

    public void EditTeamButton(object sender, RoutedEventArgs e)
    {
        _editTeamPanel.IsVisible = !_editTeamPanel.IsVisible;
    }

    public void DeleteTeamButton(object sender, RoutedEventArgs e)
    {
        _deleteTeamPanel.IsVisible = !_deleteTeamPanel.IsVisible;
    }

    public void SubmitAddPlayerButton(object sender, RoutedEventArgs e)
    {
        string firstName = FirstNameText.Text;
        string surname = SurnameText.Text;
        int number, skatingSpeed, shootRating, passRating, defenceRating, faceOffRating, teamId;
        Position? pos = null;
        SolidColorBrush errorColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#a10b29"));
        SolidColorBrush correctColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#dbdbdb"));

        NumberText.Background = int.TryParse(NumberText.Text, out number) ? correctColor : errorColor;
        SkatingSpeedText.Background = int.TryParse(SkatingSpeedText.Text, out skatingSpeed) ? correctColor : errorColor;
        ShootRatingText.Background = int.TryParse(ShootRatingText.Text, out shootRating) ? correctColor : errorColor;
        PassRatingText.Background = int.TryParse(PassRatingText.Text, out passRating) ? correctColor : errorColor;
        DefenceRatingText.Background = int.TryParse(DefenceRatingText.Text, out defenceRating) ? correctColor : errorColor;
        FaceOffRatingText.Background = int.TryParse(FaceOffRatingText.Text, out faceOffRating) ? correctColor : errorColor;
        TeamIdText.Background = int.TryParse(TeamIdText.Text, out teamId) ? correctColor : errorColor;

        if (Array.Exists(Enum.GetNames(typeof(Position)), element => element == PositionText.Text))
        {
            PositionText.Background = correctColor;
            pos = (Position)Enum.Parse(typeof(Position), PositionText.Text);
        }
        else
        {
            PositionText.Background = errorColor;
        }        
        if (number != 0 && skatingSpeed != 0 && shootRating != 0 &&
            passRating != 0 && defenceRating != 0 && faceOffRating != 0 && pos != null)
        {
            Player player = new Player(teamId, firstName, surname, number, skatingSpeed, shootRating, passRating, defenceRating, faceOffRating, (Position)pos);
            TeamsDbOperations.AddRow(player);
            ResetInputs();
            _addPlayerPanel.IsVisible = !_addPlayerPanel.IsVisible;
            RefreshWindow(this);
        }
    }

    public void SubmitEditPlayerButton(object sender, RoutedEventArgs e)
    {
        int id;
        int newValue;
        UpdateAttr? updateAttr = null;

        SolidColorBrush errorColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#a10b29"));
        SolidColorBrush correctColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#dbdbdb"));
        PlayerIdUpdate.Background = int.TryParse(PlayerIdUpdate.Text, out id) ? correctColor : errorColor;
        PlayerIdUpdate.Background = int.TryParse(PlayerNewValue.Text, out newValue) ? correctColor : errorColor;
        if (Array.Exists(Enum.GetNames(typeof(UpdateAttr)), element => element == PlayerWhatToUpdate.Text))
        {
            PlayerWhatToUpdate.Background = correctColor;
            updateAttr = (UpdateAttr)Enum.Parse(typeof(UpdateAttr), PlayerWhatToUpdate.Text);
        }
        else
        {
            PlayerWhatToUpdate.Background = errorColor;
        }
        if (id != 0 && newValue != 0 && updateAttr != null)
        {
            TeamsDbOperations.UpdateRow(id, newValue, (UpdateAttr) updateAttr, Entity.Players);
            _editPlayerPanel.IsVisible = !_editPlayerPanel.IsVisible;
            RefreshWindow(this);
        }
    }

    public void SubmitDeletePlayerButton(object sender, RoutedEventArgs e)
    {
        int id;
        SolidColorBrush errorColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#a10b29"));
        SolidColorBrush correctColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#dbdbdb"));
        PlayerIdDelete.Background = int.TryParse(PlayerIdDelete.Text, out id) ? correctColor : errorColor;

        if (id != 0)
        {
            TeamsDbOperations.DeleteRow(id, Entity.Players);
            _deletePlayerPanel.IsVisible = !_deletePlayerPanel.IsVisible;
            RefreshWindow(this);
        }
    }

    public void SubmitAddTeamButton(object sender, RoutedEventArgs e)
    {
        TeamsDbOperations.AddRow(new Team(TeamNameAdd.Text, TeamLogoAdd.Text, TeamCityAdd.Text));
        _addTeamPanel.IsVisible = !_addTeamPanel.IsVisible;
        RefreshWindow(this);
    }

    public void SubmitEditTeamButton(object sender, RoutedEventArgs e)
    {
        int id;
        string newValue = TeamNewValue.Text;
        UpdateAttr? updateAttr = null;

        SolidColorBrush errorColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#a10b29"));
        SolidColorBrush correctColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#dbdbdb"));
        PlayerIdUpdate.Background = int.TryParse(TeamIdUpdate.Text, out id) ? correctColor : errorColor;
        if (Array.Exists(Enum.GetNames(typeof(UpdateAttr)), element => element == TeamWhatToUpdate.Text))
        {
            TeamWhatToUpdate.Background = correctColor;
            updateAttr = (UpdateAttr)Enum.Parse(typeof(UpdateAttr), TeamWhatToUpdate.Text);
        }
        else
        {
            TeamWhatToUpdate.Background = errorColor;
        }
        if (id != 0 && updateAttr != null)
        {
            TeamsDbOperations.UpdateRow(id, newValue, (UpdateAttr)updateAttr, Entity.Teams);
            _editTeamPanel.IsVisible = !_editTeamPanel.IsVisible;
            RefreshWindow(this);
        }
    }

    public void SubmitDeleteTeamButton(object sender, RoutedEventArgs e)
    {
        int id;
        SolidColorBrush errorColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#a10b29"));
        SolidColorBrush correctColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#dbdbdb"));
        TeamIdDelete.Background = int.TryParse(TeamIdDelete.Text, out id) ? correctColor : errorColor;
        if (id != 0)
        {
            TeamsDbOperations.DeleteRow(id, Entity.Teams);
            _deleteTeamPanel.IsVisible = !_deleteTeamPanel.IsVisible;
            RefreshWindow(this);
        }
    }

    public void ExitChangeTeamsButton(object sender, RoutedEventArgs e)
    {
        if (team1 != null)
        {
            foreach (Player player in TeamsDbOperations.GetTeamPlayers(team1.Id))
            {
                team1.AddPlayer(player);
            }
        }
        if (team2 != null)
        {
            foreach (Player player in TeamsDbOperations.GetTeamPlayers(team2.Id))
            {
                team2.AddPlayer(player);
            }
        }
        SetTeamsEvent?.Invoke(team1, team2);
        macAudioPlayer.Stop();
        this.Close();
    }

    //public void FindTeamButton(object sender, RoutedEventArgs e)
    //{
    //    int id;
    //    Team? team = null;
    //    SolidColorBrush errorColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#a10b29"));
    //    SolidColorBrush correctColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#dbdbdb"));
    //    FindTeam1id.Background = int.TryParse(FindTeam1id.Text, out id) ? correctColor : errorColor;
    //    if (id != 0)
    //    {
    //        team = TeamsDbOperations.GetTeam(id);
    //    }
    //    if (team != null)
    //    {
    //        NameTeam1.Text = team.Name;
    //        CityTeam1.Text = team.City;
    //        team1 = team;
    //        try
    //        {
    //            var imageSource = new Bitmap(team.Logo);
    //            LogoTeam1.Source = imageSource;
    //        }
    //        catch (Exception) { }
    //        if (team.Name.ToLower() == "kometa")
    //        {
    //            MacAudioPlayer macAudioPlayer = new MacAudioPlayer();
    //            macAudioPlayer.PlaySound("Kometa_clap.wav");
    //        }
    //    }
    //}

    public void FindTeamButton(object sender, RoutedEventArgs e)
    {
        FindTeam(FindTeam1id, CityTeam1, NameTeam1, ref team1, LogoTeam1);
    }

    public void FindTeam2Button(object sender, RoutedEventArgs e)
    {
        FindTeam(FindTeam2id, CityTeam2, NameTeam2, ref team2, LogoTeam2);
    }

    public void FindTeam(TextBox input, TextBlock city, TextBlock name, ref Team? realTeam, Image image)
    {
        int id;
        Team? team = null;
        SolidColorBrush errorColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#a10b29"));
        SolidColorBrush correctColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#dbdbdb"));
        input.Background = int.TryParse(input.Text, out id) ? correctColor : errorColor;
        if (id != 0)
        {
            team = TeamsDbOperations.GetTeam(id);
        }
        if (team != null)
        {
            name.Text = team.Name;
            city.Text = team.City;
            realTeam = team;
            try
            {
                var imageSource = new Bitmap(team.Logo);
                image.Source = imageSource;
            }
            catch (Exception) { }
            if (team.Name.ToLower() == "kometa")
            {
                macAudioPlayer.PlaySound("Kometa_clap.wav");
            }
        }
    }

    //public void FindTeam2Button(object sender, RoutedEventArgs e)
    //{
    //    int id;
    //    Team? team = null;
    //    SolidColorBrush errorColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#a10b29"));
    //    SolidColorBrush correctColor = new SolidColorBrush((Avalonia.Media.Color)Color.Parse("#dbdbdb"));
    //    FindTeam2id.Background = int.TryParse(FindTeam2id.Text, out id) ? correctColor : errorColor;
    //    if (id != 0)
    //    {
    //        team = TeamsDbOperations.GetTeam(id);
    //    }
    //    if (team != null)
    //    {
    //        NameTeam2.Text = team.Name;
    //        CityTeam2.Text = team.City;
    //        team2 = team;
    //        try
    //        {
    //            var imageSource = new Bitmap(team.Logo);
    //            LogoTeam2.Source = imageSource;
    //        }
    //        catch (Exception ex)
    //        {

    //        }
    //        if (team.Name.ToLower() == "kometa")
    //        {
    //            MacAudioPlayer macAudioPlayer = new MacAudioPlayer();
    //            macAudioPlayer.PlaySound("Kometa_clap.wav");
    //        }
    //    }
    //}

    public void OpenNewWindow()
    {
        Window newWindow = new ChangeTeam();
        newWindow.Show();
    }

    public void RefreshWindow(Window window)
    {
        if (window != null)
        {
            var currentPosition = window.Position;
            var newWindow = new ChangeTeam();
            {
                Position = currentPosition;
            }
            window.Close();
            newWindow.Show();
        }
    }
}
