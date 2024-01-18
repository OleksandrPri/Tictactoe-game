using Microsoft.Maui.Controls.Compatibility.Platform.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace final_work;

public partial class InstructionsPage : ContentPage
{


    public InstructionsPage()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        MainPage mainPage = new MainPage();
        await Navigation.PushModalAsync(mainPage);
    }
}
