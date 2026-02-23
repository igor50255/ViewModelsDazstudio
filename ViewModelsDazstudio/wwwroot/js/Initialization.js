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

// 1) Вешаем глобальный клик
document.addEventListener(
  "pointerdown", // лучше чем click: срабатывает раньше (до focus/blur)
  (e) => {
    // 2) Игнорируем клики внутри "открытых вещей"
    const insidePopup = e.target.closest(".popup, .dropdown, .modal");

    if (!insidePopup) {
      closeAllOpenThings();
    }
  },
  true // <-- capture: ловим клик на захвате
);

function closeAllOpenThings() {
  // закрываем меню выбора модели по начальной букве
  const collapsibleSearch = document.querySelector('#list-search');
  const instance1 = M.Collapsible.getInstance(collapsibleSearch);
      instance1.close(0);
  // зкрываем меню выбора покаления модели
  const collapsibleType = document.querySelector('#list-files');
  const instance2 = M.Collapsible.getInstance(collapsibleType);
      instance2.close(0);
}




