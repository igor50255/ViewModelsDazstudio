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
  const collapsibleEl = document.querySelector('#list-files');
  M.Collapsible.init(collapsibleEl, {
    inDuration: 500,   // скорость открытия
    outDuration: 500,  // скорость закрытия
    accordion: false
  });

  const headerText = document.querySelector('#files-count');


  collapsibleEl.querySelectorAll('.collection-item').forEach(item => {
    item.addEventListener('click', () => {
      // только меняем заголовок
      headerText.textContent = item.textContent.trim();

      // закрываем collapsible
      const instance = M.Collapsible.getInstance(collapsibleEl);
      instance.close(0);
    });
  });
});