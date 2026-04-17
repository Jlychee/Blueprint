import { getAllProjects, getTags } from "./api/projectApi.js";

const PAGE_SIZE = 9;
const AVAILABLE_YEARS = [2021, 2022, 2023, 2024, 2025, 2026];
const state = {
    search: '',
    tagIds: [],
    year: null,
    page: 1,
    pageSize: PAGE_SIZE,
    totalPages: 1,
    totalCount: 0
};

let baseInitialized = false;
let filtersInitialized = false;
let searchDebounce = null;

console.log('projects.js loaded');
console.log('window.location.origin =', window.location.origin);
function renderPagination() {
    const pagination = document.getElementById('projects-pagination');
    const prevBtn = document.getElementById('pagination-prev');
    const nextBtn = document.getElementById('pagination-next');
    const info = document.getElementById('pagination-info');

    if (!pagination || !prevBtn || !nextBtn || !info) return;

    const totalPages = Math.max(state.totalPages || 1, 1);
    const currentPage = Math.min(state.page, totalPages);

    info.textContent = `Страница ${currentPage} из ${totalPages}`;

    prevBtn.disabled = currentPage <= 1;
    nextBtn.disabled = currentPage >= totalPages;

    pagination.style.display = totalPages > 1 ? 'flex' : 'none';
}
function createTagChip(tag) {
    if (tag.icon) {
        const img = document.createElement('img');
        img.src = tag.icon;
        img.alt = tag.title || 'tag';
        img.title = tag.title || '';
        if (tag.color) {
            img.style.backgroundColor = tag.color;
        }
        return img;
    }

    const chip = document.createElement('span');
    chip.className = 'tag-chip';
    chip.textContent = tag.title || 'tag';
    return chip;
}

function renderProjects(items) {
    const container = document.getElementById('projects-grid');
    const template = document.getElementById('project-card-template');

    if (!container || !template) return;

    container.innerHTML = '';

    if (!Array.isArray(items) || items.length === 0) {
        container.innerHTML = `<div class="projects-empty">По этим фильтрам пока ничего не найдено.</div>`;
        return;
    }

    items.forEach((project) => {
        const clone = template.content.cloneNode(true);
        const card = clone.querySelector('.card');
        const title = clone.querySelector('.project-title');
        const description = clone.querySelector('.project-description');
        const iconsContainer = clone.querySelector('.icons');

        card.href = `project.html?id=${project.id}`;
        title.textContent = project.name || 'Без названия';
        description.textContent =
            project.shortDescriptionAi ||
            project.shortDescription ||
            'Описание пока не добавлено.';

        iconsContainer.innerHTML = '';

        if (Array.isArray(project.tags) && project.tags.length > 0) {
            project.tags.forEach((tag) => {
                iconsContainer.appendChild(createTagChip(tag));
            });
        } else {
            const empty = document.createElement('span');
            empty.className = 'stack-empty';
            empty.textContent = 'Стек не указан';
            iconsContainer.appendChild(empty);
        }

        container.appendChild(clone);
    });
}
function hasActiveFilters() {
    return Boolean(
        state.search ||
        (Array.isArray(state.tagIds) && state.tagIds.length > 0) ||
        state.year
    );
}

async function loadProjects() {
    const container = document.getElementById('projects-grid');

    if (container) {
        container.innerHTML = `<div class="projects-empty">Загрузка проектов...</div>`;
    }

    try {
        let filterSessionId;

        if (hasActiveFilters()) {
            getOrCreateFilterSessionId();
        } else {
            getOrCreateFilterSessionId(true); 
        }
        
        const data = await getAllProjects({
            search: state.search,
            tagIds: state.tagIds,
            year: state.year,
            page: state.page,
            pageSize: state.pageSize
        });

        state.page = data.page ?? state.page;
        state.totalPages = data.totalPages ?? 1;
        state.totalCount = data.totalCount ?? 0;

        renderProjects(data.items || []);
        renderPagination();
    } catch (error) {
        console.error('Error loading projects:', error);

        if (container) {
            container.innerHTML = `<div class="projects-error">Не удалось загрузить проекты.</div>`;
        }

        state.totalPages = 1;
        renderPagination();
    }
}

function bindPagination() {
    const prevBtn = document.getElementById('pagination-prev');
    const nextBtn = document.getElementById('pagination-next');

    if (prevBtn && prevBtn.dataset.bound !== 'true') {
        prevBtn.dataset.bound = 'true';

        prevBtn.addEventListener('click', () => {
            if (state.page <= 1) return;

            state.page -= 1;
            loadProjects();
        });
    }

    if (nextBtn && nextBtn.dataset.bound !== 'true') {
        nextBtn.dataset.bound = 'true';

        nextBtn.addEventListener('click', () => {
            if (state.page >= state.totalPages) return;

            state.page += 1;
            loadProjects();
        });
    }
}

function renderTagsFilters(tagGroups) {
    const tagsContainer = document.getElementById('filter-tags-list');
    if (!tagsContainer) return;

    tagsContainer.innerHTML = '';

    tagGroups.forEach((group) => {
        if (!group || !Array.isArray(group.tags) || group.tags.length === 0) {
            return;
        }

        const section = document.createElement('div');
        section.className = 'filter-group';

        const title = document.createElement('div');
        title.className = 'filter-subsection-title';
        title.innerHTML = `<h3>${group.type}</h3>`;
        section.appendChild(title);

        group.tags.forEach((tag) => {
            const row = document.createElement('div');
            row.className = 'filter-row';

            row.innerHTML = `
                <label>
                    <input type="checkbox" name="tagIds" value="${tag.id}">
                    <span>${tag.title}</span>
                </label>
            `;

            section.appendChild(row);
        });

        tagsContainer.appendChild(section);
    });

    tagsContainer.querySelectorAll('input[name="tagIds"]').forEach((input) => {
        input.addEventListener('change', () => {
            state.tagIds = Array.from(
                tagsContainer.querySelectorAll('input[name="tagIds"]:checked')
            ).map((checkbox) => Number(checkbox.value));

            state.page = 1;
            loadProjects();
        });
    });
}

function renderYearFilters() {
    const yearsContainer = document.getElementById('filter-years-list');
    if (!yearsContainer) return;

    yearsContainer.innerHTML = '';

    AVAILABLE_YEARS.forEach((year) => {
        const row = document.createElement('div');
        row.className = 'filter-row';

        row.innerHTML = `
            <label>
                <input type="checkbox" name="year" value="${year}">
                <span>${year}</span>
            </label>
        `;

        yearsContainer.appendChild(row);
    });

    yearsContainer.querySelectorAll('input[name="year"]').forEach((input) => {
        input.addEventListener('change', () => {
            if (input.checked) {
                yearsContainer.querySelectorAll('input[name="year"]').forEach((checkbox) => {
                    if (checkbox !== input) {
                        checkbox.checked = false;
                    }
                });

                state.year = Number(input.value);
            } else {
                state.year = null;
            }

            state.page = 1;
            loadProjects();
        });
    });
}

function bindResetButton() {
    const resetButton = document.getElementById('filters-reset');
    if (!resetButton || resetButton.dataset.bound === 'true') return;

    resetButton.dataset.bound = 'true';

    resetButton.addEventListener('click', () => {
        state.search = '';
        state.tagIds = [];
        state.year = null;
        state.page = 1;

        const searchInput = document.getElementById('search-input');
        if (searchInput) {
            searchInput.value = '';
            searchInput.dispatchEvent(new Event('input', { bubbles: true }));
        }

        document.querySelectorAll(
            '#filter-tags-list input[type="checkbox"], #filter-years-list input[type="checkbox"]'
        ).forEach((checkbox) => {
            checkbox.checked = false;
        });

        loadProjects();
    });
}

async function initFiltersUi() {
    const tagsContainer = document.getElementById('filter-tags-list');
    const yearsContainer = document.getElementById('filter-years-list');

    if (!tagsContainer || !yearsContainer || filtersInitialized) return;

    filtersInitialized = true;

    renderYearFilters();

    try {
        const groups = await getTags();
        renderTagsFilters(Array.isArray(groups) ? groups : []);
    } catch (error) {
        console.error('Error loading tags:', error);
        tagsContainer.innerHTML = `<div class="projects-error">Не удалось загрузить теги.</div>`;
    }

    bindResetButton();
}

function bindSearch() {
    const input = document.getElementById('search-input');
    if (!input || input.dataset.catalogSearchBound === 'true') return;

    input.dataset.catalogSearchBound = 'true';

    input.addEventListener('input', () => {
        clearTimeout(searchDebounce);

        searchDebounce = setTimeout(() => {
            state.search = input.value.trim();
            state.page = 1;
            loadProjects();
        }, 300);
    });
}

function initProjectCatalog() {
    if (!baseInitialized) {
        baseInitialized = true;
        bindSearch();
        bindPagination();
        loadProjects();
    }

    initFiltersUi();
}

window.initProjectCatalog = initProjectCatalog;

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initProjectCatalog);
} else {
    initProjectCatalog();
}