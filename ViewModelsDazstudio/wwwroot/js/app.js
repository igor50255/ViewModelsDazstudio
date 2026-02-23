// =====    Старт программы   =====

// Ищем активный путь по умолчанию при старте программы, и показываем содержимое
document.addEventListener('DOMContentLoaded', function () {

  // вывод в окно активного таба
  printToActivTab(null, null)

});

let pendingGalleryEl = null;
// получение результата (списка картинок с сервера)
window.chrome.webview.addEventListener('message', (e) => {
  const msg = typeof e.data === 'string' ? JSON.parse(e.data) : e.data;
  if (msg.type !== 'images') return;

  let images = msg.data;
 
  console.log(images);

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
});


// печать в окно активного таба
function printToActivTab(typeModel, activeTab, typeSearch = "All") {
  // получаем активный элемент tab
  if (activeTab == null) activeTab = document.querySelector('.tabs .tab a.active');
  // получаем название выбранного типа модели
  if (typeModel == null) typeModel = document.querySelector('#files-count')?.textContent.trim() || '';

  const targetId = activeTab.getAttribute('href'); // "#female"
  // получаем название выбранного tab
  const tabName = activeTab.textContent.trim();
  // получаем окно для размещения контента
  const targetWindow = document.querySelector(targetId);

  const path = typeModel + '/' + tabName;

  // очищаем окно от предыдущего контента
  targetWindow.innerHTML = '';

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



