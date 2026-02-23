
// Действия при клике на выбор модели DAZ - (в окне справа)
const collapsibleType = document.querySelector('#list-files');
const collection = collapsibleType.querySelector('.collection');
// действия при клине на выбор модели DAZ
collection.addEventListener('click', (e) => {
  if (e.target.classList.contains('collection-item')) {
    const typeModel = e.target.textContent.trim(); // получаем имя кликнутотого типа модели DAZ

    // возвращаем фильтрацию в исходное положение
    document.querySelector('#files-countSearch').textContent = "All";

    // вывод в окно активного таба
    printToActivTab(typeModel, null)
  }
});


