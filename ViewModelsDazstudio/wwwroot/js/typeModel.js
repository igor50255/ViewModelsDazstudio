
// Действия при клике на выбор модели DAZ - (в окне справа)

const collection = document.querySelector('.collection');
// действия при клине на выбор модели DAZ
collection.addEventListener('click', (e) => {
  if (e.target.classList.contains('collection-item')) {
    const typeModel = e.target.textContent.trim(); // получаем имя кликнутотого типа модели DAZ
  // вывод в окно активного таба
  printToActivTab(typeModel, null)
  }
});


