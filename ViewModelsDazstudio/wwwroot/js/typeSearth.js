
// Действия при клике на выбор модели DAZ - (в окне справа)
const collapsibleSearch = document.querySelector('#list-search');
const collectionSearch = collapsibleSearch.querySelector('.collection');
// действия при клине на выбор модели DAZ
collectionSearch.addEventListener('click', (e) => {
  if (e.target.classList.contains('collection-item')) {
    const typeSearch = e.target.textContent.trim(); // получаем имя кликнутотого типа модели DAZ
    console.log(typeSearch);
  // вывод в окно активного таба
  printToActivTab(null, null, typeSearch)
  }
});


