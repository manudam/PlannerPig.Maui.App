namespace PlannerPig.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("coolvetica rg.ttf", "MontserratSubrayada");
                fonts.AddFont("MontserratSubrayada-Regular.ttf", "MontserratSubrayada");
                fonts.AddFont("MyriadPro-Regular.otf", "MyriadProRegular");
                fonts.AddFont("MyriadPro-SemiboldIt.otf", "MyriadProSemiboldIt");
            });

        return builder.Build();
    }
}


