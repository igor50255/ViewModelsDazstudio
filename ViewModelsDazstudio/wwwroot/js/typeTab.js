// обработчик клика по табам
document.querySelectorAll('.tabs .tab a').forEach(tab => {
  tab.addEventListener('click', function () {

    // возвращаем фильтрацию в исходное положение
    document.querySelector('#files-countSearch').textContent = "All";
    
    // вывод в окно активного таба
    printToActivTab(null, this);
  });
});

