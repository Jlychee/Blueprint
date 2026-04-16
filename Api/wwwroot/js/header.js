async function loadHeader() {
    const headerContainer = document.getElementById('header');
    if (!headerContainer) return;

    const headerPaths = [
        'resources/components/header.html',
        'header.html'
    ];

    let headerHtml = '';

    for (const path of headerPaths) {
        try {
            const response = await fetch(path);
            if (response.ok) {
                headerHtml = await response.text();
                break;
            }
        } catch (error) {
            console.error(`Не удалось загрузить header по пути ${path}:`, error);
        }
    }

    if (!headerHtml) {
        console.error('Не удалось загрузить header.html');
        return;
    }

    headerContainer.innerHTML = headerHtml;

    initHeaderSearch();

    if (typeof window.initSearchAndFilter === 'function') {
        window.initSearchAndFilter();
    }
}

function focusIndexSearch() {
    const searchSection = document.querySelector('.search-section');
    const searchInput = document.getElementById('search-input');

    if (!searchInput) return;

    if (searchSection) {
        searchSection.classList.add('active');
        searchSection.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }

    requestAnimationFrame(() => {
        searchInput.focus();
        searchInput.select();
    });
}

function initHeaderSearch() {
    const searchBtn = document.getElementById('search-btn');

    if (!searchBtn) return;

    searchBtn.addEventListener('click', focusIndexSearch);
}

loadHeader();