function getBaseProjectLink() {
    return `${window.location.origin}${window.location.pathname}${window.location.search}`;
}

function getActiveTabId() {
    const activeButton = document.querySelector(".tab-btn.active");
    if (activeButton?.dataset.tab) {
        return activeButton.dataset.tab;
    }

    const hashTab = window.location.hash.replace("#", "");
    return hashTab || "short";
}

function updateOverlayLinks() {
    const fileLinkInput = document.getElementById("fileLink");
    const projectLinkInput = document.getElementById("projectLink");

    if (!fileLinkInput || !projectLinkInput) return;

    const projectLink = getBaseProjectLink();
    const activeTab = getActiveTabId();
    const fileLink = `${projectLink}#${activeTab}`;

    fileLinkInput.value = fileLink;
    projectLinkInput.value = projectLink;
}

function openOverlay() {
    updateOverlayLinks();

    const overlay = document.querySelector(".overlay");
    const overlayBg = document.querySelector(".overlay-bg");

    if (!overlay || !overlayBg) return;

    overlay.style.display = 'block';
    overlayBg.style.display = 'block';
    document.body.style.overflow = 'hidden';
}

function closeOverlay() {
    const overlay = document.querySelector('.overlay');
    const overlayBg = document.querySelector('.overlay-bg');

    if (!overlay || !overlayBg) return;

    overlay.style.display = 'none';
    overlayBg.style.display = 'none';
    document.body.style.overflow = '';
}

function copyLink(id) {
    const input = document.getElementById(id);
    if (!input) return;

    navigator.clipboard.writeText(input.value).then(() => {
        alert("Ссылка скопирована!");
    });
}
function bindOverlayEvents(root = document) {
    root.querySelectorAll('[data-close-overlay]').forEach((element) => {
        if (element.dataset.bound === 'true') return;

        element.dataset.bound = 'true';
        element.addEventListener('click', closeOverlay);
    });

    root.querySelectorAll('[data-open-overlay]').forEach((button) => {
        if (button.dataset.bound === 'true') return;

        button.dataset.bound = 'true';
        button.addEventListener('click', (event) => {
            event.preventDefault();
            openOverlay();
        });
    });

    root.querySelectorAll('[data-copy-link]').forEach((button) => {
        if (button.dataset.bound === 'true') return;

        button.dataset.bound = 'true';
        button.addEventListener('click', () => {
            copyLink(button.dataset.copyLink);
        });
    });
}
async function loadOverlay() {
    if (document.querySelector(".overlay")) {
        updateOverlayLinks();
        return;
    }

    try {
        const response = await fetch('linkOverlay.html');
        const html = await response.text();

        const container = document.createElement('div');
        container.innerHTML = html;
        document.body.appendChild(container);
        bindOverlayEvents(container);
        updateOverlayLinks();
    } catch (err) {
        console.error('Ошибка загрузки оверлея:', err);
    }
}

window.openOverlay = openOverlay;
window.closeOverlay = closeOverlay;
window.copyLink = copyLink;
window.updateOverlayLinks = updateOverlayLinks;

document.addEventListener("DOMContentLoaded", () => {
    bindOverlayEvents();
    loadOverlay();
});