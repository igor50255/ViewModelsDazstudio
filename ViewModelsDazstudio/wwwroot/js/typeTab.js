// обработчик клика по табам
document.querySelectorAll('.tabs .tab a').forEach(tab => {
  tab.addEventListener('click', function () {
    // вывод в окно активного таба
    printToActivTab(null, this);
  });
});

