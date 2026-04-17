function initSearchAndFilter() {
    const input = document.getElementById('search-input');
    const clearBtn = document.querySelector('.clear');
    const layout = document.getElementById('catalog-layout');
    const filterButtons = document.querySelectorAll('.filter-btn, #filter');

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

    if (layout) {
        filterButtons.forEach((button) => {
            if (button.dataset.filtersBound === 'true') return;

            button.dataset.filtersBound = 'true';
            button.addEventListener('click', () => {
                layout.classList.toggle('filters-open');
            });
        });
    }

    const closeBtn = document.getElementById('filters-close');
    if (closeBtn && layout && closeBtn.dataset.filtersBound !== 'true') {
        closeBtn.dataset.filtersBound = 'true';
        closeBtn.addEventListener('click', () => {
            layout.classList.remove('filters-open');
        });
    }
}

window.initSearchAndFilter = initSearchAndFilter;

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initSearchAndFilter);
} else {
    initSearchAndFilter();
}