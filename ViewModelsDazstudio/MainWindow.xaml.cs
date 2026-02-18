using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace ViewModelsDazstudio;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Обязательно: создаёт окружение/профиль и гарантирует готовность движка
        await Browser.EnsureCoreWebView2Async();

        // Подписываемся на сообщения из веб-страницы
        Browser.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

        string webRoot = System.IO.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "wwwroot"
        );

        Browser.CoreWebView2.SetVirtualHostNameToFolderMapping(
            "app",
            webRoot,
            CoreWebView2HostResourceAccessKind.Allow
        );

        Browser.NavigationCompleted += async (_, __) =>
        {
            // фокус WPF -> WebView2
            Browser.Focus();
            Keyboard.Focus(Browser);

            // фокус внутри страницы (иногда нужен дополнительно)
            await Browser.ExecuteScriptAsync("window.focus();");
        };

        Browser.CoreWebView2.Navigate("https://app/index.html");
    }

    private void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        var json = e.WebMessageAsJson;

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!root.TryGetProperty("type", out var typeProp))
            return;

        var type = typeProp.GetString();

        switch (type)
        {
            case "text.append":
                {
                    var payload = root.GetProperty("payload");
                    var value = payload.GetProperty("value").GetString() ?? "";

                    var response = new
                    {
                        type = "text.set",
                        payload = new
                        {
                            id = "myText",
                            value = value + " + C#"
                        }
                    };

                    string finalJson = JsonSerializer.Serialize(response);
                    Browser.CoreWebView2.PostWebMessageAsJson(finalJson);
                    break;
                }
            case "text.new":
                {
                    var payload = root.GetProperty("payload");
                    var value = payload.GetProperty("value").GetString() ?? "";

                    var response = new
                    {
                        type = "text.set",
                        payload = new
                        {
                            id = "myText",
                            value = value
                        }
                    };

                    string finalJson = JsonSerializer.Serialize(response);
                    Browser.CoreWebView2.PostWebMessageAsJson(finalJson);
                    break;
                }
            case "toggle-window-fullscreen": // раскрыть окно на весь экран
                {
                    ToggleWpfFullscreen();
                    break;
                }
            case "exit-window-fullscreen": // выход из полноэкранного режима
                {
                    if (_isWpfFullscreen) ToggleWpfFullscreen();
                    break;
                }
        }
    }

    // развернуть и свернуть окно WPF в полноэкранный режим (без рамки) двойным кликом по окну
    private bool _isWpfFullscreen;
    private WindowStyle _prevWindowStyle;
    private WindowState _prevWindowState;
    private ResizeMode _prevResizeMode;
    private bool _prevTopmost;
    private void ToggleWpfFullscreen()
    {
        if (!_isWpfFullscreen)
        {
            // сохраняем текущее состояние
            _prevWindowStyle = this.WindowStyle;
            _prevWindowState = this.WindowState;
            _prevResizeMode = this.ResizeMode;
            _prevTopmost = this.Topmost;

            // включаем fullscreen без рамки
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.Topmost = true; // опционально, чтобы поверх панели задач
            this.WindowState = WindowState.Maximized;

            _isWpfFullscreen = true;
        }
        else
        {
            // возвращаем как было
            this.Topmost = _prevTopmost;
            this.WindowStyle = _prevWindowStyle;
            this.ResizeMode = _prevResizeMode;
            this.WindowState = _prevWindowState;

            _isWpfFullscreen = false;
        }

        // после смены стиля фокус иногда теряется
        Browser.Focus();
        Keyboard.Focus(Browser);
    }
}