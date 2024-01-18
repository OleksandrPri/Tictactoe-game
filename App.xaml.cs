namespace final_work;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}

    protected override Window CreateWindow(IActivationState activationState)
    {
		var window = base.CreateWindow(activationState);

		const int newWidth = 960;
        const int newHeight = 540;

		window.X = 200;
		window.Y = 200;

		window.Width = newWidth;
		window.Height = newHeight;

		window.MinimumWidth = newWidth;
		window.MinimumHeight = newHeight;

		

        return window;
    }
}
