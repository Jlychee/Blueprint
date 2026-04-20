function initSearchAndFilter() {
    const input = document.getElementById('search-input');
    const clearBtn = document.querySelector('.clear');
    const layout = document.getElementById('catalog-layout');

    if (input && clearBtn && input.dataset.searchInitialized !== 'true') {
        input.dataset.searchInitialized = 'true';

        function toggleClear() {
            clearBtn.classList.toggle('active', input.value.length > 0);
        }

        input.addEventListener('input', toggleClear);

        clearBtn.addEventListener('click', () => {
            input.value = '';
            toggleClear();
            input.dispatchEvent(new Event('input', { bubbles: true }));
            input.focus();
        });

        toggleClear();
    }

    if (!layout) return;

    if (!document.body.dataset.filtersBound) {
        document.body.dataset.filtersBound = 'true';

        document.addEventListener('click', (event) => {
            if (event.target.closest('.filter-btn') || event.target.closest('#filter')) {
                layout.classList.toggle('filters-open');
            }

            if (event.target.closest('#filters-close')) {
                layout.classList.remove('filters-open');
            }
        });
    }
}

window.initSearchAndFilter = initSearchAndFilter;

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initSearchAndFilter);
} else {
    initSearchAndFilter();
}