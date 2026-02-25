// =====    Старт программы   =====

// Ищем активный путь по умолчанию при старте программы, и показываем содержимое
document.addEventListener('DOMContentLoaded', function () {

  // вывод в окно активного таба
  printToActivTab(null, null, "All", 2000)

  // отправка запроса для получения актуального пути к папке с контентом
  const payload = { type: 'get-path-content' };
  chrome.webview.postMessage(payload);

});

let pendingGalleryEl = null;
// получение результата (списка картинок с сервера)
window.chrome.webview.addEventListener('message', (e) => {
  const msg = typeof e.data === 'string' ? JSON.parse(e.data) : e.data;
  // отображение картинок превью контента
  if (msg.type == 'images') {
    let images = msg.data;
    console.log(images); // проверка контента

    let gallery = pendingGalleryEl;
    // запуск отображения галереи
    loadGallery(images, gallery);
    // тултипы на новых элементах
    refreshTooltips(gallery);
    // установка отзывчивой сетки для размещения картинок
    gridApi = setupResponsiveGrid(gallery, () => images?.length ?? 0);
    // пересчёт после показа
    requestAnimationFrame(() => gridApi.recalc());       // ✅ must-have

    pendingGalleryEl = null;
  }
  // получение актуального пути к папке с контентом
  else if (msg.type == 'set-path-content') {
    let path = msg.pathContent;
    // прописывам актуальный путь 
    const content = document.querySelector('#path-content');
    content.textContent = path;
  }
  else return;


});


// печать в окно активного таба
function printToActivTab(typeModel, activeTab, typeSearch = "All", duration = 500) {
  // получаем активный элемент tab
  if (activeTab == null) activeTab = document.querySelector('.tabs .tab a.active');
  // получаем название выбранного типа модели
  if (typeModel == null) typeModel = document.querySelector('#files-count')?.textContent.trim() || '';

  const targetId = activeTab.getAttribute('href'); // "#female"

  // если открыто окно настроек и клик не по tab, то выходим
  if (targetId == "#settings") return;

  // получаем название выбранного tab
  const tabName = activeTab.textContent.trim();
  // получаем окно для размещения контента
  const targetWindow = document.querySelector(targetId);

  const path = typeModel + '/' + tabName;

  // очищаем окно от предыдущего контента

  targetWindow.innerHTML = '';
  fadeIn(targetWindow, duration);


  // создаём div галереи
  const gallery = document.createElement('div');
  gallery.className = 'gallery';

  // добавляем галерею в targetWindow
  targetWindow.appendChild(gallery);

  pendingGalleryEl = gallery;

  // отправка запроса для получения содержимого папки: path
  const payload = { type: 'get-path-images', path, searth: typeSearch };
  chrome.webview.postMessage(payload);





}



