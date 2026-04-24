import {getAllProjects, getTags} from "./api/projectApi.js";

const PAGE_SIZE = 9;
const AVAILABLE_YEARS = [2021, 2022, 2023, 2024, 2025, 2026];
const STORAGE_KEY = "projectBlueprintCatalogState";

const state = {
    search: "",
    tagIds: [],
    year: null,
    page: 1,
    pageSize: PAGE_SIZE,
    totalPages: 1,
    totalCount: 0
};

let baseInitialized = false;
let filtersInitialized = false;
let lastRequestId = 0;

function saveCatalogState() {
    sessionStorage.setItem(
        STORAGE_KEY,
        JSON.stringify({
            search: state.search,
            tagIds: state.tagIds,
            year: state.year,
            page: state.page,
            pageSize: state.pageSize
        })
    );
}

function restoreCatalogState() {
    try {
        const raw = sessionStorage.getItem(STORAGE_KEY);
        if (!raw) return;

        const saved = JSON.parse(raw);

        state.search = typeof saved.search === "string" ? saved.search : "";
        state.tagIds = Array.isArray(saved.tagIds)
            ? saved.tagIds.map(Number).filter(Number.isFinite)
            : [];
        state.year = saved.year != null && saved.year !== ""
            ? Number(saved.year)
            : null;
        state.page = saved.page != null
            ? Math.max(Number(saved.page), 1)
            : 1;
        state.pageSize = saved.pageSize != null
            ? Math.max(Number(saved.pageSize), 1)
            : PAGE_SIZE;
    } catch (error) {
        console.error("Failed to restore catalog state:", error);
        sessionStorage.removeItem(STORAGE_KEY);
    }
}

function syncUiWithState() {
    const searchInput = document.getElementById("search-input");
    if (searchInput) {
        searchInput.value = state.search || "";
    }

    document.querySelectorAll('#filter-tags-list input[name="tagIds"]').forEach((checkbox) => {
        checkbox.checked = state.tagIds.includes(Number(checkbox.value));
    });

    document.querySelectorAll('#filter-years-list input[name="year"]').forEach((checkbox) => {
        checkbox.checked = Number(checkbox.value) === state.year;
    });
}

function escapeHtml(value) {
    return String(value)
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}

function renderSearchStub(query) {
    const container = document.getElementById("projects-grid");
    const pagination = document.getElementById("projects-pagination");

    if (!container) return;

    container.innerHTML = `
        <div class="search-stub">
            <p class="search-stub__text">
                По запросу ничего не найдено.
                <br>
                Запрос: <span>${escapeHtml(query)}</span>
            </p>
            <img
                class="search-stub__image"
                src="resources/images/hlopin.png"
                alt="Ничего не найдено"
                width="180"
                height="221"
            >
        </div>
    `;

    if (pagination) {
        pagination.style.display = "none";
    }
}

function refreshCatalog() {
    saveCatalogState();
    loadProjects();
}

function scrollToProjectsTop() {
    const target = document.querySelector(".search-section") || document.querySelector(".projects");
    const headerOffset = 110;

    requestAnimationFrame(() => {
        if (!target) {
            window.scrollTo({
                top: 0,
                behavior: "smooth"
            });
            return;
        }

        const top = target.getBoundingClientRect().top + window.scrollY - headerOffset;

        window.scrollTo({
            top: Math.max(top, 0),
            behavior: "smooth"
        });
    });
}

function renderPagination() {
    const pagination = document.getElementById("projects-pagination");
    const prevBtn = document.getElementById("pagination-prev");
    const nextBtn = document.getElementById("pagination-next");
    const info = document.getElementById("pagination-info");

    if (!pagination || !prevBtn || !nextBtn || !info) return;

    const totalPages = Math.max(state.totalPages || 1, 1);
    const currentPage = Math.min(state.page, totalPages);

    info.textContent = `Страница ${currentPage} из ${totalPages}`;
    prevBtn.disabled = currentPage <= 1;
    nextBtn.disabled = currentPage >= totalPages;
    pagination.style.display = totalPages > 1 ? "flex" : "none";
}

function createTagChip(tag) {
    if (tag.icon) {
        const img = document.createElement("img");
        img.src = tag.icon;
        img.alt = tag.title || "tag";
        img.title = tag.title || "";

        if (tag.color) {
            img.style.filter = "brightness(0) saturate(100%) invert(70%)";
            img.style.boxShadow = "none";
        }

        return img;
    }

    const chip = document.createElement("span");
    chip.className = "tag-chip";
    chip.textContent = tag.title || "tag";
    return chip;
}

function renderProjects(items) {
    const container = document.getElementById("projects-grid");
    const template = document.getElementById("project-card-template");

    if (!container || !template) return;

    container.innerHTML = "";

    if (!Array.isArray(items) || items.length === 0) {
        container.innerHTML = `<div class="projects-empty">По этим фильтрам пока ничего не найдено.</div>`;
        return;
    }

    items.forEach((project) => {
        const clone = template.content.cloneNode(true);
        const card = clone.querySelector(".card");
        const title = clone.querySelector(".project-title");
        const description = clone.querySelector(".project-description");
        const iconsContainer = clone.querySelector(".icons");

        card.href = `project.html?id=${project.id}`;
        title.textContent = project.name || "Без названия";
        description.textContent =
            project.shortDescriptionAi ||
            "Описание пока не добавлено.";

        iconsContainer.innerHTML = "";

        if (Array.isArray(project.tags) && project.tags.length > 0) {
            project.tags.forEach((tag) => {
                iconsContainer.appendChild(createTagChip(tag));
            });
        } else {
            const empty = document.createElement("span");
            empty.className = "stack-empty";
            empty.textContent = "Стек не указан";
            iconsContainer.appendChild(empty);
        }

        container.appendChild(clone);
    });
}

function syncFilterSessionId() {
    if (typeof window.getOrCreateFilterSessionId !== "function") return;

    if (hasActiveFilters()) {
        window.getOrCreateFilterSessionId();
    } else {
        window.getOrCreateFilterSessionId(true);
    }
}

async function loadProjects() {
    const container = document.getElementById("projects-grid");
    const requestId = ++lastRequestId;

    if (container) {
        container.innerHTML = `<div class="projects-empty">Загрузка проектов...</div>`;
    }

    try {
        syncFilterSessionId();

        const data = await getAllProjects({
            search: state.search,
            tagIds: state.tagIds,
            year: state.year,
            page: state.page,
            pageSize: state.pageSize
        });

        if (requestId !== lastRequestId) return;

        state.page = data.page ?? state.page;
        state.totalPages = data.totalPages ?? 1;
        state.totalCount = data.totalCount ?? 0;

        saveCatalogState();

        const items = data.items || [];

        if (state.search && items.length === 0) {
            renderSearchStub(state.search);
        } else {
            renderProjects(items);
            renderPagination();
        }
    } catch (error) {
        if (requestId !== lastRequestId) return;

        console.error("Error loading projects:", error);

        if (container) {
            container.innerHTML = `<div class="projects-error">Не удалось загрузить проекты.</div>`;
        }

        state.totalPages = 1;
        renderPagination();
    }
}

function bindPagination() {
    const prevBtn = document.getElementById("pagination-prev");
    const nextBtn = document.getElementById("pagination-next");

    if (prevBtn && prevBtn.dataset.bound !== "true") {
        prevBtn.dataset.bound = "true";

        prevBtn.addEventListener("click", async () => {
            if (state.page <= 1) return;

            state.page -= 1;
            saveCatalogState();
            await loadProjects();
            scrollToProjectsTop();
        });
    }

    if (nextBtn && nextBtn.dataset.bound !== "true") {
        nextBtn.dataset.bound = "true";

        nextBtn.addEventListener("click", async () => {
            if (state.page >= state.totalPages) return;

            state.page += 1;
            saveCatalogState();
            await loadProjects();
            scrollToProjectsTop();
        });
    }
}

function renderTagsFilters(tagGroups) {
    const tagsContainer = document.getElementById("filter-tags-list");
    if (!tagsContainer) return;

    tagsContainer.innerHTML = "";

    tagGroups.forEach((group) => {
        if (!group || !Array.isArray(group.tags) || group.tags.length === 0) {
            return;
        }

        const section = document.createElement("div");
        section.className = "filter-group";

        const title = document.createElement("div");
        title.className = "filter-subsection-title";

        const heading = document.createElement("h3");
        heading.textContent = group.type || "Теги";
        title.appendChild(heading);
        section.appendChild(title);

        group.tags.forEach((tag) => {
            const row = document.createElement("div");
            row.className = "filter-row";

            const label = document.createElement("label");
            const input = document.createElement("input");
            const text = document.createElement("span");

            input.type = "checkbox";
            input.name = "tagIds";
            input.value = String(tag.id);

            text.textContent = tag.title || "Без названия";

            label.appendChild(input);
            label.appendChild(text);
            row.appendChild(label);
            section.appendChild(row);
        });

        tagsContainer.appendChild(section);
    });

    tagsContainer.querySelectorAll('input[name="tagIds"]').forEach((input) => {
        input.addEventListener("change", () => {
            state.tagIds = Array.from(
                tagsContainer.querySelectorAll('input[name="tagIds"]:checked')
            ).map((checkbox) => Number(checkbox.value));

            state.page = 1;
            refreshCatalog();
        });
    });

    syncUiWithState();
}

function renderYearFilters() {
    const yearsContainer = document.getElementById("filter-years-list");
    if (!yearsContainer) return;

    yearsContainer.innerHTML = "";

    AVAILABLE_YEARS.forEach((year) => {
        const row = document.createElement("div");
        row.className = "filter-row";

        const label = document.createElement("label");
        const input = document.createElement("input");
        const text = document.createElement("span");

        input.type = "checkbox";
        input.name = "year";
        input.value = String(year);
        text.textContent = String(year);

        label.appendChild(input);
        label.appendChild(text);
        row.appendChild(label);
        yearsContainer.appendChild(row);
    });

    yearsContainer.querySelectorAll('input[name="year"]').forEach((input) => {
        input.addEventListener("change", () => {
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
            refreshCatalog();
        });
    });

    syncUiWithState();
}

function bindResetButton() {
    const resetButton = document.getElementById("filters-reset");
    if (!resetButton || resetButton.dataset.bound === "true") return;

    resetButton.dataset.bound = "true";

    resetButton.addEventListener("click", () => {
        state.search = "";
        state.tagIds = [];
        state.year = null;
        state.page = 1;

        const searchInput = document.getElementById("search-input");
        if (searchInput) {
            searchInput.value = "";
            searchInput.dispatchEvent(new Event("input", {bubbles: true}));
        }

        document.querySelectorAll(
            '#filter-tags-list input[type="checkbox"], #filter-years-list input[type="checkbox"]'
        ).forEach((checkbox) => {
            checkbox.checked = false;
        });

        refreshCatalog();
    });
}

async function initFiltersUi() {
    const tagsContainer = document.getElementById("filter-tags-list");
    const yearsContainer = document.getElementById("filter-years-list");

    if (!tagsContainer || !yearsContainer || filtersInitialized) return;

    filtersInitialized = true;

    renderYearFilters();

    try {
        const groups = await getTags();
        renderTagsFilters(Array.isArray(groups) ? groups : []);
    } catch (error) {
        console.error("Error loading tags:", error);
        tagsContainer.innerHTML = `<div class="projects-error">Не удалось загрузить теги.</div>`;
    }

    bindResetButton();
}

function hasActiveFilters() {
    return Boolean(
        state.search ||
        (Array.isArray(state.tagIds) && state.tagIds.length > 0) ||
        state.year
    );
}
function bindSearch() {
    const input = document.getElementById("search-input");
    if (!input || input.dataset.catalogSearchBound === "true") return;

    input.dataset.catalogSearchBound = "true";

    input.addEventListener("keydown", (event) => {
        if (event.key !== "Enter") return;

        event.preventDefault();

        state.search = input.value.trim();
        state.page = 1;

        refreshCatalog();
    });

    input.addEventListener("input", () => {
        if (input.value.trim()) return;
        if (!state.search) return;

        state.search = "";
        state.page = 1;
        saveCatalogState();
        loadProjects();
    });
}

function initProjectCatalog() {
    if (!baseInitialized) {
        baseInitialized = true;

        restoreCatalogState();
        syncUiWithState();
        bindSearch();
        bindPagination();
        refreshCatalog();
    }

    initFiltersUi().then(() => {
        syncUiWithState();
    });
}

window.initProjectCatalog = initProjectCatalog;

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initProjectCatalog);
} else {
    initProjectCatalog();
}
