﻿
using AppLanches.Services;
using AppLanches.Validations;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Validator = AppLanches.Validations.Validator;

namespace AppLanches
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
            
#endif
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<IValidator, Validator>();
            //builder.Services.AddSingleton<FavoritosService>();
           
            return builder.Build();
        }
    }
}
