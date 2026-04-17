function initSearchAndFilter() {
    const input = document.getElementById('search-input');
    const clearBtn = document.querySelector('.clear');

    if (!input || !clearBtn) return;
    if (input.dataset.searchInitialized === 'true') return;

    input.dataset.searchInitialized = 'true';

    function toggleClear() {
        clearBtn.classList.toggle('active', input.value.length > 0);
    }

    input.addEventListener('input', toggleClear);

    clearBtn.addEventListener('click', () => {
        input.value = '';
        toggleClear();
        input.focus();
    });

    toggleClear();
}

window.initSearchAndFilter = initSearchAndFilter;

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initSearchAndFilter);
} else {
    initSearchAndFilter();
}