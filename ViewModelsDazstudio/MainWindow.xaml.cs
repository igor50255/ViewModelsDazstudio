using Microsoft.Web.WebView2.Core;
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
    string hostIndex = "app";
    string hostGallery = "gallery";
    string rootContent = @"D:\ContentDazStudio";
    string defaultImg = @"Resources\default.png";
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Устанавливаем папку для контента при первом запуске (или при изменении папки)
        rootContent = Configure.SetRootContent(rootContent, this);

        // Обязательно: создаёт окружение/профиль и гарантирует готовность движка
        await Browser.EnsureCoreWebView2Async();

        // Подписываемся на сообщения из веб-страницы
        Browser.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceivedAsync;

        string webRoot = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "wwwroot");

        // маппим хост для запуска index.html
        Browser.CoreWebView2.SetVirtualHostNameToFolderMapping(hostIndex, webRoot,
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

        // ВАЖНО: поставить gallery-host заранее (для работы с контентом галерееи)
        string initialGalleryFolder = rootContent;
        Browser.CoreWebView2.SetVirtualHostNameToFolderMapping(hostGallery, initialGalleryFolder,
            CoreWebView2HostResourceAccessKind.Allow);


        Browser.CoreWebView2.Navigate($"https://{hostIndex}/index.html");


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
            case "get-path-images":
                {
                    var folder = root.GetProperty("path").GetString() ?? "";

                    //var folder = @"Genesis 9\Female";

                    if (!Directory.Exists(rootContent + '/' + folder))
                        break;

                    // фильтрация по поиску
                    var search = root.GetProperty("searth").GetString() ?? ""; // или "Other", "All", или "S", "A", "IJ" и т.п.

                    // получаем список путей к картинкам 0.* в папке "preview" каждого персонажа (в заданной папке)
                    // если нет такой папки или такого файла, то создаёт. Файл берёт первый из папки персонажа или дефолтный, если в папке нет картинок

                    var list = PreviewFixer.BuildList(rootContent + '/' + folder, search, defaultImg);
                    
                    if (list.Count == 0) return;// если в заданной папке нет контента

                    // устанавливаем версию файла для измежания кеширования одинаковых имён из разных папок
                    long ver = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

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
            case "restarting-application": // перезапуск приложения
                {
                    // изменение пути к папке с контетом
                    Properties.Settings.Default.rootFolder = false;
                    Properties.Settings.Default.Save();

                    Restart.RestartApplication();
                    break;
                }
            case "get-path-content": // получить актуальный путь к папке с контентом
                {
                    var pathContent =  Properties.Settings.Default.rootFolderPath;
 
                    Browser.CoreWebView2.PostWebMessageAsJson(
                        System.Text.Json.JsonSerializer.Serialize(new { type = "set-path-content", pathContent = pathContent.ToString() })
                    );
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