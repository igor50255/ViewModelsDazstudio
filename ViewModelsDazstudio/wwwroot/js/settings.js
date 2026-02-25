// Вызвать окно со справкой и изменением пути к папке с контентом
document.getElementById("open-settings").addEventListener("click", () => {

  const tabs = M.Tabs.getInstance(document.querySelector('.tabs'));
  console.log(tabs);
  tabs.select('settings');
});

// Изменить путь к папке с контентом
document.getElementById("chengePathContent").addEventListener("click", () => {

  const payload = { type: 'restarting-application' };
  chrome.webview.postMessage(payload);
});