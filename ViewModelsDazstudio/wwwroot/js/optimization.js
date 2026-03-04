
// оптимизация файлов в папке: preview до высоты картинок: 1000px
document.getElementById("opntimazation1").addEventListener("click", () => {
  showLoader();
  const payload = { type: 'optimization', hieght: "1000" };
  chrome.webview.postMessage(payload);
});

// оптимизация файлов в папке: preview до высоты картинок: 500px
document.getElementById("opntimazation2").addEventListener("click", () => {
  showLoader();
  const payload = { type: 'optimization', hieght: "500" };
  chrome.webview.postMessage(payload);
});

// возвращение картинок превью в первозданное состояние
document.getElementById("opntimazation-refresh").addEventListener("click", () => {
  showLoader();
  const payload = { type: 'optimization-refresh' };
  chrome.webview.postMessage(payload);
});

// удаление всех папок: preview
document.getElementById("opntimazation-delete").addEventListener("click", () => {
  showLoader();
  const payload = { type: 'optimization-delete' };
  chrome.webview.postMessage(payload);
});