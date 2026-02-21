// Старт программы

// Ищем активный путь по умолчанию, и показываем содержимое
document.addEventListener('DOMContentLoaded', function () {

  // вывод в окно активного таба
  printToActivTab(null, null)

});

// печать в окно активного таба
function printToActivTab(typeModel, activeTab) {
  // получаем активный элемент tab
  if (activeTab == null) activeTab = document.querySelector('.tabs .tab a.active');
  // получаем название выбранного типа модели
  if (typeModel == null) typeModel = document.querySelector('#files-count')?.textContent.trim() || '';

  const targetId = activeTab.getAttribute('href'); // "#female"
  // получаем название выбранного tab
  const tabName = activeTab.textContent.trim();
  // получаем окно для размещения контента
  const targetWindow = document.querySelector(targetId);

  const result = typeModel + '/' + tabName;

  // очищаем окно от предыдущего контента
  targetWindow.innerHTML = '';

  // создаём div галереи
  const gallery = document.createElement('div');
  gallery.className = 'gallery';

  // добавляем галерею в targetWindow
  targetWindow.appendChild(gallery);

  if (targetId == "#female") {
    // запуск отображения галереи
    loadGallery(window.images, gallery);
    // тултипы на новых элементах
    refreshTooltips(gallery);

    gridApi = setupResponsiveGrid(gallery, () => window.images?.length ?? 0);
  }
  else {
    // запуск отображения галереи
    loadGallery(window.images2, gallery);
    // тултипы на новых элементах
    refreshTooltips(gallery);

    gridApi = setupResponsiveGrid(gallery, () => window.images2?.length ?? 0);
  }


  // пересчёт после показа
  requestAnimationFrame(() => gridApi.recalc());       // ✅ must-have

}



