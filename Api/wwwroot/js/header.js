async function loadHeader() {
    const headerContainer = document.getElementById('header');
    if (!headerContainer) {
        console.error('Элемент #header не найден');
        return;
    }

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

    if (!searchSection) {
        console.error('.search-section не найден');
        return;
    }

    searchSection.classList.toggle('active');

    if (searchSection.classList.contains('active')) {
        searchSection.scrollIntoView({behavior: 'smooth', block: 'center'});

        requestAnimationFrame(() => {
            if (searchInput) {
                searchInput.focus();
                searchInput.select();
            }
        });
    }
}

function initHeaderSearch() {
    const searchBtn = document.getElementById('search-btn');
    if (!searchBtn) {
        console.error('#search-btn не найден');
        return;
    }

    searchBtn.addEventListener('click', focusIndexSearch);
}

document.addEventListener('DOMContentLoaded', loadHeader);