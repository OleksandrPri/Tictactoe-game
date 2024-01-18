using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Text;

namespace final_work;

public partial class MainPage : ContentPage
{
    public ObservableCollection<Users> User { get; set; }
    public string path = FileSystem.Current.AppDataDirectory + @"\userScores.json", 
    player1PictureSource = "player1.jpg", player2PictureSource = "player2.jpg", AIPictureSource = "computer.jpg", emptyPictureSource = "choose.png";
    public string[] player1Data = new string[7], player2Data = new string[7],
    Properties = { "firstName", "secondName", "birthDay", "countWin", "countLoss", "countDraw", "countTime" };   //tiedostosta taulukkoon kirjoitetut pelaajan tiedot
    bool playerChoose = true, AI;                                                                                //playerChoose - true: valita pelaaja 1 / false: valita pelaaja 2, AI - ai - tekoälyn läsnäolo;

    public MainPage()
	{
		InitializeComponent();
        User = new ObservableCollection<Users>();
        activatePlayer1Choice();
        BindingContext = this;
        checkFileExisting();
        refreshListView();
        pickerPlayerType.SelectedIndex = 0;
    }

    private void OnFirstPlayerClicked(object sender, EventArgs e)
    {
        activatePlayer1Choice();
    }

    private void OnSecondPlayerClicked(object sender, EventArgs e)
    {
        activatePlayer2Choice();
    }

    void PickerSelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            switch (pickerPlayerType.SelectedIndex)
            {
                case 0:                                                                                 //tekoäly vastustajana
                    AI = true;
                    activatePlayer1Choice();
                    picturePlayer2.Source = AIPictureSource;
                    labelPlayer2Name.Text = "Tietokone";
                    string JSONfileText = File.ReadAllText(path);
                    JArray jsonArray = JArray.Parse(JSONfileText);
                    JObject AIObject = jsonArray.Children<JObject>().FirstOrDefault(o => o["birthDay"]?.ToString() == "0000-00-00");

                    if (AIObject != null)
                    {
                        for (int i = 0; i < Properties.Length; i++)                                     //tallentaa AI-tiedot pelaajaksi 2
                        {
                            player2Data[i] = AIObject[Properties[i]].ToString();
                        }
                    }
                    break;
                case 1:                                                                                  //henkilö vastustajana
                    AI = false;
                    activatePlayer2Choice();
                    picturePlayer2.Source = emptyPictureSource;                                          //tyhjentaa AI-tiedot 
                    labelPlayer2Name.Text = string.Empty;
                    player2Data = new string[7];
                    break;
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Virhe", ex.Message, "OK");
        }
    }

    private void OnNewGameClicked(object sender, EventArgs e)
    {
        if (player1Data[0] != null && player2Data[0] != null)                                   //tarkistaa onko molemmat pelaajat valittu
        {
            GamePage newGame = new GamePage(player1Data, player2Data, AI);
            Navigation.PushModalAsync(newGame);
        }
        else
        {
            DisplayAlert("Virhe", "Valitse pelaaja !", "ok");
        }
    }

    private async void OnCreateAccountClicked(object sender, EventArgs e)
	{
        RegistrationPage registration = new RegistrationPage();
        await Navigation.PushModalAsync(registration);
    }

    private async void OnInstructionsClicked(object sender, EventArgs e)
    {
        InstructionsPage Instructions = new InstructionsPage();
        await Navigation.PushModalAsync(Instructions);
    }

    private void OnExitClicked(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }

    private void activatePlayer1Choice()                                                //valita pelaaja 1
    {
        borderPlayer1.StrokeThickness = 5;
        borderPlayer2.StrokeThickness = 1;
        playerChoose = true;
    }

    private void activatePlayer2Choice()                                                //valita pelaaja 2
    {
        if (pickerPlayerType.SelectedIndex == 1)
        {
            borderPlayer1.StrokeThickness = 1;
            borderPlayer2.StrokeThickness = 5;
            playerChoose = false;
        }
    }

    public void checkFileExisting()                                                     //tarkistaa onko tiedosto olemassa
    {
        try
        {
            if (File.Exists(path) != true)
            {
                File.Create(path).Close();                                              //luo tiedosto ja AI-tiedot
                addNewUser("Tietokone", "", "0000-00-00", "0", "0", "0", "0:00");
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Virhe", ex.Message, "OK");
        }
    }

    public void refreshListView()                                                                   //päivita ListView
    {
        try
        {
            User.Clear();                                                                           //tyhjentaa ListView
            string JSONfileText = File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<List<Users>>(JSONfileText).ToList();
            var sb = new StringBuilder();

            data.ForEach(player =>                                                                  //kirjoittaa pelaajaa tiedostosta
            {
                User.Add(new Users
                {
                    firstName = player.firstName,
                    secondName = player.secondName,
                    birthDay = player.birthDay,
                    countWin = player.countWin,
                    countLoss = player.countLoss,
                    countDraw = player.countDraw,
                    countTime = player.countTime
                });
            });
        }
        catch (Exception ex)
        {
            DisplayAlert("Virhe", ex.Message, "OK");
        }

    }

    public void addNewUser(string firstName, string secondName, string birthDay, string countWin, string countLoss, string countDraw, string countTime)      //luo uusi pelaaja
    {
        try
        {
            User.Add(new Users
            {
                firstName = firstName,
                secondName = secondName,
                birthDay = birthDay,
                countWin = countWin,
                countLoss = countLoss,
                countDraw = countDraw,
                countTime = countTime
            });
            string JsonObject = JsonConvert.SerializeObject(User);
            File.WriteAllText(path, JsonObject);
        }
        catch (Exception ex)
        {
            DisplayAlert("Virhe", ex.Message, "OK");
        }
    }

    public void OnItemSelectedChanged(object sender, SelectedItemChangedEventArgs e)
    {
        switch (playerChoose)                                                                               //tarkistaa kumpi pelaajan solu valitetaal
        {
            case true:
                setPlayer(player1PictureSource, player1Data, labelPlayer1Name, picturePlayer1, e);
                break;
            case false:
                setPlayer(player2PictureSource, player2Data, labelPlayer2Name, picturePlayer2, e);
                break;
        }
    }

    private void setPlayer(string playerPictureSource, string[] playerData, Label labelPlayerName, ImageButton playerImage, SelectedItemChangedEventArgs e)         //valitsee pelaajan luettelosta
    {
        string selectedUser = string.Empty, player1 = string.Empty, player2 = string.Empty;

        try
        {
            if (player1Data != null)                                                //määrittää valittujen pelaajien tiedot
            {
                player1 = player1Data[0] + player1Data[1] + player1Data[2];
            }
            if (player2Data != null)
            {
                player2 = player1Data[0] + player1Data[1] + player1Data[2];
            }

            for (int i = 0; i < 3; i++)                                             //määrittää tiedot luettelosta valitulle uudelle käyttäjälle
            {
                System.Reflection.PropertyInfo pi = e.SelectedItem.GetType().GetProperty(Properties[i]);
                selectedUser += (string)pi.GetValue(e.SelectedItem, null);
            }

            if (selectedUser == player1 && pickerPlayerType.SelectedIndex == 1 ||   //tarkistaa, oliko luettelosta valittu käyttäjä jo määritetty
                selectedUser == player2 && pickerPlayerType.SelectedIndex == 1)
            {
                DisplayAlert("Virhe", "Pelaaja on jo valittu.", "ok");
            }
            else if (selectedUser == "Tietokone0000-00-00")                         //tarkistaa, oliko luettelosta valittu käyttäjä - tietokone
            {
                DisplayAlert("Virhe", "Tietokonetta ei voi valita pelaajaksi.", "ok");
            }
            else if (selectedUser != player1 && selectedUser != "Tietokone0000-00-00" ||    //määrittää uusi valittu pelaaja soluun
                selectedUser != player2 && selectedUser != "Tietokone0000-00-00")
            {
                for (int i = 0; i < Properties.Length; i++)
                {
                    System.Reflection.PropertyInfo pi =
                    e.SelectedItem.GetType().GetProperty(Properties[i]);
                    playerData[i] = Convert.ToString(pi.GetValue(e.SelectedItem, null));
                }

                playerImage.Source = playerPictureSource;
                labelPlayerName.Text = playerData[0] + " " + playerData[1];
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Virhe", ex.Message, "OK");
        }
    }
}
public struct Users
{
    public string firstName { get; set; }
    public string secondName { get; set; }
    public string birthDay { get; set; }
    public string countWin { get; set; }
    public string countLoss { get; set; }
    public string countDraw { get; set; }
    public string countTime { get; set; }

    public Users(string firstName, string secondName, string countWin, string countLoss, string countDraw, string countTime, string birthDay)
    {
        this.firstName = firstName;
        this.secondName = secondName;
        this.birthDay = birthDay;
        this.countWin = countWin;
        this.countLoss = countLoss;
        this.countDraw = countDraw;
        this.countTime = countTime;
    }
}

