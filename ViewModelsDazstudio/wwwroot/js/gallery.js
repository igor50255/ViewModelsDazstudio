
// В будущем эти пути к картинкам могут не работать, можно поставить путь к картинкам на пк
// window.images = [
//   { Name: "794.jpg", Path: "file:///C:/Users/igorNik/Desktop/aaaaaa/597605.jpg" },
//   { Name: "5770.jpg", Path: "https://picsum.photos/id/1025/800/450" },
//   { Name: "794.jpg", Path: "https://picsum.photos/id/1015/800/450" },
//   { Name: "5770.jpg", Path: "https://picsum.photos/id/1025/800/450" },
//   { Name: "794.jpg", Path: "https://picsum.photos/id/1015/800/450" },
//   { Name: "5770.jpg", Path: "https://picsum.photos/id/1025/800/450" },
//   { Name: "794.jpg", Path: "https://picsum.photos/id/1015/800/450" },
//   { Name: "5770id/1025/800/450id/1025/800/450.jpg", Path: "https://picsum.photos/id/1025/800/450" },
//   { Name: "cute.webp", Path: "https://picsum.photos/id/1035/800/450" }
// ];

function loadGallery(images, gallery) {
  gallery.innerHTML = "";

  for (const img of images) {
    const card = document.createElement("div");
    card.className = "card";

    const image = document.createElement("img");
    image.src = img.PathFile;
    image.alt = img.Name ?? "";
    image.loading = "lazy";

    const caption = document.createElement("div");
    caption.className = "caption tooltipped";
    caption.textContent = img.Name ?? "";

    // сохраняем путь прямо в DOM
    caption.dataset.path = img.PathFolder ?? "";

    // // Полный текст — в data-tooltip
    caption.setAttribute("data-tooltip", img.Name ?? "");

    card.append(image, caption);
    gallery.appendChild(card);
  }
}

function setupResponsiveGrid(galleryEl, getItemCount) {
  const rootStyles = getComputedStyle(document.documentElement);
  const minW = parseFloat(rootStyles.getPropertyValue("--min")) || 200;
  const maxW = parseFloat(rootStyles.getPropertyValue("--max")) || 250;

  function recalc() {
    const styles = getComputedStyle(galleryEl);

    const padL = parseFloat(styles.paddingLeft) || 0;
    const padR = parseFloat(styles.paddingRight) || 0;
    const gap = parseFloat(styles.columnGap || styles.gap) || 0;

    const count = Math.max(0, getItemCount());
    const innerW = galleryEl.clientWidth - padL - padR;

    if (count === 0 || innerW <= 0) {
      galleryEl.style.setProperty("--cols", 1);
      galleryEl.style.setProperty("--w", minW);
      galleryEl.style.setProperty("--jc", "start");
      return;
    }

    const maxColsByMin = Math.max(1, Math.floor((innerW + gap) / (minW + gap)));
    const colsUpper = Math.min(count, maxColsByMin);

    let foundFill = false;
    let cols = colsUpper;
    let w = minW;

    for (let c = colsUpper; c >= 1; c--) {
      const ww = (innerW - gap * (c - 1)) / c;
      if (ww >= minW && ww <= maxW) {
        cols = c;
        w = ww;
        foundFill = true;
        break;
      }
    }

    if (foundFill) {
      galleryEl.style.setProperty("--cols", cols);
      galleryEl.style.setProperty("--w", w);
      galleryEl.style.setProperty("--jc", "start");
      return;
    }

    galleryEl.style.setProperty("--cols", colsUpper);
    galleryEl.style.setProperty("--w", maxW);
    galleryEl.style.setProperty("--jc", "center");
  }

  const ro = new ResizeObserver(recalc);
  ro.observe(galleryEl);

  recalc();

  return { recalc, destroy: () => ro.disconnect() };
}

// запуск - перенесли в в app.js
// loadGallery(window.images);

// setupResponsiveGrid(
//   document.getElementById("gallery"),
//   () => window.images?.length ?? 0
// );

// клик по картинки (по блоку с именем) возвращает путь к картинке
document.addEventListener("click", (e) => {
  const cap = e.target.closest(".caption");
  if (!cap) return;

  console.log(cap.dataset.path); // здесь можно открыть папку с этой картинкой
});

// инициализация Tooltipe - всплывающей подсказки
function refreshTooltips(scope = document) {
  const elems = scope.querySelectorAll('.tooltipped');

  elems.forEach(el => {
    const inst = M.Tooltip.getInstance(el);
    if (inst) inst.destroy();
  });

  M.Tooltip.init(elems, {
    exitDelay: 200,      // задержка перед скрытием (мс)
    enterDelay: 1000,     // задержка перед показом (мс)
    transitionMovement: 10, // «подпрыгивание» тултипа
    margin: 5,           // отступ от элемента
    position: 'bottom',     // top | right | bottom | left
  });
}




