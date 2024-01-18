using Newtonsoft.Json.Linq;

namespace final_work;

public partial class GamePage : ContentPage
{
    MainPage mainPage = new MainPage();                                        
    TimeOnly time = new();                                                              
    ImageButton[] cells, symbolsPictures;                                           //cells - soluja pelissä, symbolsPictures - symbolin kuva pelaajan vieressä
    string[] playground = new string[9],                                            //"virtuaalinen" kenttä, johon askelit on merkitty
    player1data, player2data, symbols = { "tik.png", "tak.png" };                   
    string curentPlayer;                                                            //pelaajan symboli, jonka on pitää tehdä askel
    bool timerOnOff, ai, aiStep = false;                                            //ai - tekoälyn läsnäolo, aiStep - tekoälyn askel
    int stepCount;                                                                  //pelin askelin määrä

    public GamePage(string[] player1Data, string[] player2Data, bool AI)
    {
        InitializeComponent();
        symbolsAssign();
        gameTimer();
        cells = new ImageButton[] { cell1, cell2, cell3, cell4, cell5, cell6, cell7, cell8, cell9 };
        player1data = player1Data;
        player2data = player2Data;
        ai = AI;
        labelPlayer1Name.Text = player1Data[0];
        labelPlayer2Name.Text = player2Data[0];
        checkIfAIstepFirst();
    }

    private void OnCell1Clicked(object sender, EventArgs e)
    {
        stepHandling(0);                                                            
    }

    private void OnCell2Clicked(object sender, EventArgs e)
    {
        stepHandling(1);
    }

    private void OnCell3Clicked(object sender, EventArgs e)
    {
        stepHandling(2);
    }

    private void OnCell4Clicked(object sender, EventArgs e)
    {
        stepHandling(3);
    }

    private void OnCell5Clicked(object sender, EventArgs e)
    {
        stepHandling(4);
    }

    private void OnCell6Clicked(object sender, EventArgs e)
    {
        stepHandling(5);
    }

    private void OnCell7Clicked(object sender, EventArgs e)
    {
        stepHandling(6);
    }

    private void OnCell8Clicked(object sender, EventArgs e)
    {
        stepHandling(7);
    }

    private void OnCell9Clicked(object sender, EventArgs e)
    {
        stepHandling(8);
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        MainPage mainPage = new MainPage();
        await Navigation.PushModalAsync(mainPage);
    }

    private async void OnPlayAgainClicked(object sender, EventArgs e)
    {
        GamePage gamePage = new GamePage(player1data, player2data, ai);
        await Navigation.PushModalAsync(gamePage);
    }

    private void stepHandling(int cellNumber)
    {
        if (ai == true && timerOnOff == true)                               //peli tekoälyn kanssa
        {
            if(aiStep == false)                                             //tarkistus - onko tekoälyn askel tehty
            {
                aiStep = true;
                checkStepAndWIn(cellNumber, playground);
                if (aiStep == true)
                {
                    doAIstep();
                }
            }
        }
        else if (ai == false && timerOnOff == true)                         //peli henkilön kanssa
        {
            checkStepAndWIn(cellNumber, playground);
        }
    }

    private void checkStepAndWIn(int cellNumber, string[] arr)              
    {
        if (playground[cellNumber] == null)                                             //tarkistaa onko solu vapaa
        {
            stepCount++;
            cells[cellNumber].Source = curentPlayer;
            playground[cellNumber] = curentPlayer;

            if (arr[0] != null && arr[0] == arr[1] && arr[1] == arr[2] ||               //tarkistaa voito
            arr[3] != null && arr[3] == arr[4] && arr[4] == arr[5] ||
            arr[6] != null && arr[6] == arr[7] && arr[7] == arr[8] ||
            arr[0] != null && arr[0] == arr[3] && arr[3] == arr[6] ||
            arr[1] != null && arr[1] == arr[4] && arr[4] == arr[7] ||
            arr[2] != null && arr[2] == arr[5] && arr[5] == arr[8] ||
            arr[0] != null && arr[0] == arr[4] && arr[4] == arr[8] ||
            arr[6] != null && arr[6] == arr[4] && arr[4] == arr[2]
            )
            {
                if (player1Symbol.Source.ToString() == "File: " + curentPlayer)         //tarkistaa, kumpi pelaaja omisti voiton symboli
                {
                    labelWinner.Text = player1data[0] + " wins !";
                    saveGameResults(1, 0, 0, 0, 1, 0);
                }
                else if (player2Symbol.Source.ToString() == "File: " + curentPlayer)
                {
                    labelWinner.Text = player2data[0] + " wins !";
                    saveGameResults(0, 1, 0, 1, 0, 0);
                }
                timerOnOff = false;
                aiStep = false;
            }
            else if (stepCount == 9)                                                    //tarkistaa tasapeli
            {
                labelWinner.Text = "Draw !";
                timerOnOff = false;
                saveGameResults(0, 0, 1, 0, 0, 1);
                aiStep = false;
            }
            else                                                                        //vahtaa pelajaan jono
            {
                switch (curentPlayer)
                {
                    case "tik.png":
                        curentPlayer = symbols[1];
                        symbolsPictures[1].IsEnabled = true;
                        symbolsPictures[0].IsEnabled = false;
                        break;
                    case "tak.png":
                        curentPlayer = symbols[0];
                        symbolsPictures[0].IsEnabled = true;
                        symbolsPictures[1].IsEnabled = false;
                        break;
                }
            }
        }
    }

    private void symbolsAssign()                                                        //antaa pelaajalle satunnaisen symböli pelin alussa
    {
        Random random = new Random();
        curentPlayer = symbols[0];

        if (random.Next(2) == 0)
        {
            player1Symbol.Source = symbols[0];
            player2Symbol.Source = symbols[1];
            symbolsPictures = new ImageButton[] { player1Symbol, player2Symbol };
        }
        else
        {
            player1Symbol.Source = symbols[1];
            player2Symbol.Source = symbols[0];
            symbolsPictures = new ImageButton[] { player2Symbol, player1Symbol };
        }

        symbolsPictures[1].IsEnabled = false;
    }

    private async void gameTimer()                                                       //käynnistää ajastimen
    {
        timerOnOff = !timerOnOff;

        while (timerOnOff)
        {   
            time = time.Add(TimeSpan.FromSeconds(1));
            gameTime.Text = $"{time.Minute:00}:{time.Second:00}";
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }

    private async void doAIstep()                                                         //tekoälyn askelin tekeminen
    {
        await Task.Delay(1000);
        Random r = new Random();
        int AIcell = r.Next(0, 9);
        while (playground[AIcell] != null)
        {
            AIcell = r.Next(0, 9);
        }
        checkStepAndWIn(AIcell, playground);
        aiStep = false;
    }

    private void checkIfAIstepFirst()                                                     //tekoäly tekee ensimmäinen askel pelin alussa, jos hän on X 
    {
        if(ai == true)
        {
            picturePlayer2.Source = "computer.jpg";
            if (player2Symbol.Source.ToString() == "File: " + symbols[0] && ai == true)
            {
                aiStep = true;
                doAIstep();
            }
        }
    }

    private void saveGameResults(int player1Win, int player1Loss, int player1Draw, int player2Win, int player2Loss, int player2Draw)    //tallentaa pelin tiedot
    {
        try
        {
            string JSONfileText = File.ReadAllText(mainPage.path);                   //avaa tiedosto
            JArray jsonArray = JArray.Parse(JSONfileText);                           //muuntaa taulukoksi
            JObject player1Object = null, player2Object = null;

            setNewCounts(jsonArray, player1Object, player1data, player1Win, player1Loss, player1Draw);
            setNewCounts(jsonArray, player2Object, player2data, player2Win, player2Loss, player2Draw);

            string modifiedJson = jsonArray.ToString();                              //talenna tiedosto
            File.WriteAllText(mainPage.path, modifiedJson);
        }
        catch (Exception ex)
        {
            DisplayAlert("Virhe", ex.Message, "OK");
        }

    }

    private void setNewCounts(JArray jsonArray, JObject playerObject, string[] playerData, int playerWin, int playerLoss, int playerDraw)   //tekee muutokset pelaajan tiedossa
    {
        playerObject = jsonArray.Children<JObject>().FirstOrDefault(o => o["firstName"]?.ToString() +
        o["secondName"]?.ToString() + o["birthDay"]?.ToString() == playerData[0] + playerData[1] + playerData[2]);

        if (playerObject != null)
        {
            playerObject["countWin"] = Convert.ToString(Convert.ToInt32(playerObject["countWin"]) + playerWin);
            playerObject["countLoss"] = Convert.ToString(Convert.ToInt32(playerObject["countLoss"]) + playerLoss);
            playerObject["countDraw"] = Convert.ToString(Convert.ToInt32(playerObject["countDraw"]) + playerDraw);
            playerObject["countTime"] = Convert.ToString(TimeOnly.Parse(playerObject["countTime"].ToString()).Add(TimeSpan.Parse(gameTime.Text)));
        }
    }
}
