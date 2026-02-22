using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using ViewModelsDazstudio.Services;

namespace ViewModelsDazstudio;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    string hostGallery = "gallery";
    string rootContent = @"D:\Content";
    string defaultImg = @"Resources\default.png";
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
        Browser.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceivedAsync;

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

        // ВАЖНО: поставить gallery-host заранее (на любую существующую папку)
        string initialGalleryFolder = rootContent; // или пустая папка-заглушка
        Browser.CoreWebView2.SetVirtualHostNameToFolderMapping(hostGallery, initialGalleryFolder,
            CoreWebView2HostResourceAccessKind.Allow);

        Browser.CoreWebView2.Navigate("https://app/index.html");
    }

    private async void CoreWebView2_WebMessageReceivedAsync(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
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
            case "get-path-images":
                {
                    var folder = root.GetProperty("path").GetString() ?? "";

                    //var folder = @"Genesis 9\Female";

                    if (!Directory.Exists(rootContent + '/' + folder))
                        break;

                    // устанавливаем версию файла для измежания кеширования одинаковых имён из разных папок
                    long ver = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                    // получаем список путей к картинкам 0.* в папке "preview" каждого персонажа (в заданной папке)
                    // если нет такой папки или такого файла, то создаёт. Файл берёт первый из папки персонажа или дефолтный, если в папке нет картинок

                    var list = PreviewFixer.BuildList(rootContent + '/' + folder, defaultImg);

                    if (list.Count == 0) return;// если в заданной папке нет контента

                    // получаем список объектов для дальнейшей работы в js. Пример (это поля):
                    //Name = "Aelwen For Genesis 8 Female",
                    //PathFolder = $"https://{hostGallery}/{folder}/{urlName}", - https://gallery/Genesis 8 - 8.1\Female\Adeline for Genesis 8 Female
                    //PathFile = $"https://{hostGallery}/{folder}/{urlNameFile}?v={ver}" - https://gallery/Genesis 8 - 8.1\Female\Adeline for Genesis 8 Female\preview\0.jpg?v=123456789

                    var images = GalleryMapper.BuildImageDtos(list, hostGallery, folder, ver);

                    Browser.CoreWebView2.PostWebMessageAsJson(
                        System.Text.Json.JsonSerializer.Serialize(new { type = "images", data = images })
                    );
                    break;
                }
            case "open-model-folder":
                {
                    var url = root.GetProperty("path").GetString() ?? "";

                    // открытие папки с выбранной моделью в проводнике Windows
                    ModelFolder.Open(url, rootContent, hostGallery);

                    break;
                }
            case "toggle-window-fullscreen": // раскрыть окно на весь экранvar 
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