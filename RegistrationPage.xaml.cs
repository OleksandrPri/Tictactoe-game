namespace final_work;

public partial class RegistrationPage : ContentPage
{
    MainPage mainPage = new MainPage();

    public RegistrationPage()
    {
        InitializeComponent();
    }

    private void OnAddUserClicked(object sender, EventArgs e)
    {
        try
        {
            if (newFirstName.Text != null && newSecondName.Text != null &&               //Tarkista, ovatko kentät tyhjiä
            newFirstName.Text != string.Empty && newSecondName.Text != string.Empty)
            {
                mainPage.addNewUser(newFirstName.Text, newSecondName.Text, DateOnly.FromDateTime(dateBirthday.Date).ToString(), "0", "0", "0", "0:00");   //Uuden käyttäjän lisääminen

                newUserConfirm.Text = newFirstName.Text + " on lisätty.";               //Vahvistus että uusi pelaaja in lisätty

                newFirstName.Text = string.Empty;                                       //Tekstikenttien tyhjentäminen
                newSecondName.Text = string.Empty;
                dateBirthday.Date = DateTime.Now;
            }
            else
            {
                DisplayAlert("Virhe", "Täytä kaikki kentät", "OK");
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Virhe", ex.Message, "OK");
        }

    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(mainPage);
    }
}
