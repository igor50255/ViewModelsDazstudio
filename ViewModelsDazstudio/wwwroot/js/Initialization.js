document.addEventListener('DOMContentLoaded', () => {
  // M.Sidenav.init(document.querySelectorAll('.sidenav'));
  const menu = document.querySelector('.sidenav');
  M.Sidenav.init(menu, {
    inDuration: 500,   // скорость открытия
    outDuration: 500  // скорость закрытия
  });

  const tabsEl = document.querySelector('.tabs');
  const dedicated = document.querySelector('.dedicatedTab');
  const sidenavEl = document.querySelector('#mobile-demo');

  const highlightSidenav = (id) => {
    document.querySelectorAll('.sidenav a').forEach(a => {
      a.classList.toggle(
        'active',
        a.getAttribute('href') === `#${id}`
      );
    });
  };

  const setDedicatedFromId = (id) => {
    const a = tabsEl.querySelector(`a[href="#${CSS.escape(id)}"]`);
    if (a) dedicated.textContent = a.textContent.trim();
  };

  const tabs = M.Tabs.init(tabsEl, {
    onShow: (contentEl) => {
      const id = contentEl.id;
      setDedicatedFromId(id);
      highlightSidenav(id);
    }
  });

  // начальная синхронизация
  const activeA = tabsEl.querySelector('a.active');
  if (activeA) {
    const id = activeA.getAttribute('href').slice(1);
    dedicated.textContent = activeA.textContent.trim();
    highlightSidenav(id);
  }

  // клики в sidenav
  document.querySelectorAll('.sidenav-tab').forEach(a => {
    a.addEventListener('click', e => {
      e.preventDefault();
      const id = a.getAttribute('href').slice(1);
      tabs.select(id);

      const sidenav = M.Sidenav.getInstance(sidenavEl);
      sidenav.close();
      // подсветка и dedicatedTab обновятся через onShow
    });
  });


  // инициализация collapsible - меню выбора типа модели 
  const collapsibleType = document.querySelector('#list-files');
  M.Collapsible.init(collapsibleType, {
    inDuration: 500,   // скорость открытия
    outDuration: 500,  // скорость закрытия
    accordion: false
  });
  // инициализация collapsible - меню выбора модели по начальной букве
  const collapsibleSearch = document.querySelector('#list-search');
  M.Collapsible.init(collapsibleSearch, {
    inDuration: 500,   // скорость открытия
    outDuration: 500,  // скорость закрытия
    accordion: false
  });

  // меняем заголовок у выбора поколения модели и закрываем collapsible при клике
  const headerTextType = collapsibleType.querySelector('#files-count');
  collapsibleType.querySelectorAll('.collection-item').forEach(item => {
    item.addEventListener('click', () => {
      // только меняем заголовок
      headerTextType.textContent = item.textContent.trim();

      // закрываем collapsible
      const instance = M.Collapsible.getInstance(collapsibleType);
      instance.close(0);
    });
  });

  // меняем заголовок у выбора поиска модели и закрываем collapsible при клике
  const headerTextSearch = collapsibleSearch.querySelector('#files-countSearch');
  collapsibleSearch.querySelectorAll('.collection-item').forEach(item => {
    item.addEventListener('click', () => {
      // только меняем заголовок
      headerTextSearch.textContent = item.textContent.trim();

      // закрываем collapsible
      const instance = M.Collapsible.getInstance(collapsibleSearch);
      instance.close(0);
    });
  });

});

// 1) Вешаем глобальный клик для для закрития выпадающих списков при клике где угодно
document.addEventListener("pointerdown", (e) => {
  const onFilesBtn   = e.target.closest("#files-count");
  const onSearchBtn  = e.target.closest("#files-countSearch");

  const inFilesList  = e.target.closest("#list-files");
  const inSearchList = e.target.closest("#list-search");

  // 1) Клик по одному триггеру -> закрываем другой список
  if (onFilesBtn) {
    closeCollapsible("#list-search");
    return;
  }
  if (onSearchBtn) {
    closeCollapsible("#list-files");
    return;
  }

  // 2) Клик внутри списков -> ничего не делаем
  if (inFilesList || inSearchList) return;

  // 3) Клик везде вне триггеров и списков -> закрыть оба
  closeCollapsible("#list-files");
  closeCollapsible("#list-search");
}); // <-- важно: без capture


function closeCollapsible(selector) {
  const el = document.querySelector(selector);
  if (!el) return;

  let inst = M.Collapsible.getInstance(el);
  if (!inst) inst = M.Collapsible.init(el);

  // закрыть первую секцию (если у тебя одна)
  inst.close(0);

  // если секций может быть несколько — так надёжнее:
  // el.querySelectorAll("li").forEach((_, i) => inst.close(i));
}




