async function loadFilters() {
    const container = document.getElementById('filters-container');
    if (!container) return;

    const paths = [
        'resources/components/filters.html',
        'filters.html'
    ];

    let html = '';

    for (const path of paths) {
        try {
            const response = await fetch(path);
            if (response.ok) {
                html = await response.text();
                break;
            }
        } catch (error) {
            console.error(`Не удалось загрузить filters по пути ${path}:`, error);
        }
    }

    if (!html) {
        console.error('Не удалось загрузить filters.html');
        return;
    }

    container.innerHTML = html;

    if (typeof window.initSearchAndFilter === 'function') {
        window.initSearchAndFilter();
    }

    if (typeof window.initProjectCatalog === 'function') {
        window.initProjectCatalog();
    }
}

loadFilters();