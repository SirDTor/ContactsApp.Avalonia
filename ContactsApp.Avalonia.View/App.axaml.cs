using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Remote.Protocol;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using ContactsApp.Avalonia.View.ViewModels;
using ContactsApp.Avalonia.View.Views;
using System;
using System.Linq;
using System.Reflection;

namespace ContactsApp.Avalonia.View;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new ContactsListView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Переключает тему приложения на Light/Dark.
    /// Метод сначала пытается установить FluentTheme.Mode через reflection,
    /// если это невозможно — использует Application.RequestedThemeVariant (ThemeVariant).
    /// </summary>
    public void SetDarkTheme(bool isDark)
    {
        // 1) Попытка найти FluentTheme в Application.Styles по полному имени типа
        var fluentTheme = Styles.FirstOrDefault(s => s.GetType().FullName == "Avalonia.Themes.Fluent.FluentTheme");

        if (fluentTheme != null)
        {
            try
            {
                var modeProp = fluentTheme.GetType().GetProperty("Mode", BindingFlags.Public | BindingFlags.Instance);
                if (modeProp != null && modeProp.CanWrite)
                {
                    // Попробуем установить значение enum-подобного свойства "Mode" по имени "Dark"/"Light"
                    var enumType = modeProp.PropertyType;
                    var enumName = isDark ? "Dark" : "Light";

                    // Если тип действительно enum-подобный, парсим
                    if (enumType.IsEnum)
                    {
                        var enumValue = Enum.Parse(enumType, enumName, ignoreCase: true);
                        modeProp.SetValue(fluentTheme, enumValue);
                        return; // успешно установили FluentTheme.Mode
                    }
                    else
                    {
                        // Иногда тип может быть ThemeVariant/другой — попробуем задать через конверсию
                        // Пытаемся создать значение через Enum.Parse на имя типа (на всякий случай)
                        var tryVal = Enum.Parse(enumType, enumName, ignoreCase: true);
                        modeProp.SetValue(fluentTheme, tryVal);
                        return;
                    }
                }
            }
            catch
            {
                // если reflection не сработал — просто перейдём к fallback
            }
        }

        // 2) Fallback: установить RequestedThemeVariant приложения (работает универсально)
        // ThemeVariant имеет статические свойства Dark/Light
        try
        {
            Application.Current.RequestedThemeVariant = isDark ? ThemeVariant.Dark : ThemeVariant.Light;
        }
        catch
        {
            // если и это не работает — ничего не делаем (не фатально)
        }
    }

    /// <summary>
    /// Удобный метод-переключатель: если isDark==null — переключает на противоположную текущей.
    /// </summary>
    public void ToggleTheme()
    {
        // попробуем прочитать текущий FluentTheme.Mode (через reflection), иначе прочитаем RequestedThemeVariant
        var fluentTheme = Styles.FirstOrDefault(s => s.GetType().FullName == "Avalonia.Themes.Fluent.FluentTheme");
        bool currentIsDark = false;
        if (fluentTheme != null)
        {
            var modeProp = fluentTheme.GetType().GetProperty("Mode", BindingFlags.Public | BindingFlags.Instance);
            if (modeProp != null && modeProp.CanRead)
            {
                var modeVal = modeProp.GetValue(fluentTheme);
                if (modeVal != null)
                {
                    var name = modeVal.ToString() ?? "";
                    currentIsDark = name.Equals("Dark", StringComparison.OrdinalIgnoreCase);
                }
            }
        }
        else
        {
            var rv = Application.Current.RequestedThemeVariant;
            if (rv != null)
            {
                // ThemeVariant has Key "Dark"/"Light" -> ToString gives key
                currentIsDark = rv == ThemeVariant.Dark;
            }
        }

        SetDarkTheme(!currentIsDark);
    }
}
