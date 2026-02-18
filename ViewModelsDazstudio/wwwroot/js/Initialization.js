document.addEventListener('DOMContentLoaded', () => {
  M.Sidenav.init(document.querySelectorAll('.sidenav'));

  const tabs = M.Tabs.init(document.querySelector('.tabs'));

  document.querySelectorAll('.sidenav-tab').forEach(a => {
    a.addEventListener('click', e => {
      const id = a.getAttribute('href').replace('#', '');
      e.preventDefault();
      tabs.select(id);

      const sidenav = M.Sidenav.getInstance(
        document.querySelector('#mobile-demo')
      );
      sidenav.close();
    });
  });
});