// Старт программы

// Ищем активный путь по умолчанию, и показываем содержимое
document.addEventListener('DOMContentLoaded', function () {

  // вывод в окно активного таба
  printToActivTab(null, null)
  
});

// печать в окно активного таба
function printToActivTab(typeModel, activeTab){
  // получаем активный элемент tab
  if(activeTab == null) activeTab = document.querySelector('.tabs .tab a.active');
  // получаем название выбранного типа модели
  if(typeModel == null) typeModel = document.querySelector('#files-count')?.textContent.trim() || '';

  const targetId = activeTab.getAttribute('href');
  // получаем название выбранного tab
  const tabName = activeTab.textContent.trim();
  // получаем окно для размещения контента
  const targetWindow = document.querySelector(targetId);

  const result = typeModel + '/' + tabName;
  console.log(result);

  if (targetWindow) {
    targetWindow.textContent = result;
  }
}