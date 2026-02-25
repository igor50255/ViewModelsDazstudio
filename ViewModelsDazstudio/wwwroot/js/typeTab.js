// обработчик клика по табам
document.querySelectorAll('.tabs .tab a').forEach(tab => {
  tab.addEventListener('click', function () {

    // возвращаем фильтрацию в исходное положение
    document.querySelector('#files-countSearch').textContent = "All";

    // получаем id таба (female / male)
    const tabId = this.getAttribute('href').substring(1);

    // кроме таба настроек (если кликнули по вызову настроек)
    if (tabId != "settings") {
      // вывод в окно активного таба
      printToActivTab(null, this);
    }

  });
});

