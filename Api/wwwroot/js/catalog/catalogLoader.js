import {getAllProjects} from "../api/projectApi.js";

import {
    getLastRequestId,
    getNextRequestId,
    hasActiveFilters,
    state
} from "./catalogState.js";

import {
    saveCatalogState
} from "./catalogUrl.js";

import {
    renderPagination,
    renderProjects,
    renderSearchStub
} from "./catalogRender.js";

export function refreshCatalog() {
    saveCatalogState();
    loadProjects();
}

function syncFilterSessionId() {
    if (typeof window.getOrCreateFilterSessionId !== "function") return;

    if (hasActiveFilters()) {
        window.getOrCreateFilterSessionId();
    } else {
        window.getOrCreateFilterSessionId(true);
    }
}

export async function loadProjects() {
    const container = document.getElementById("projects-grid");
    const requestId = getNextRequestId();

    if (container) {
        container.classList.add("projects-grid--loading");

        if (!container.children.length) {
            container.innerHTML = `<div class="projects-empty">Загрузка проектов...</div>`;
        }
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

        if (requestId !== getLastRequestId()) return;

        state.page = data.page ?? state.page;
        state.totalPages = data.totalPages ?? 1;
        state.totalCount = data.totalCount ?? 0;

        saveCatalogState();

        const items = data.items || [];

        if (container) {
            container.classList.remove("projects-grid--loading");
        }

        if (state.search && items.length === 0) {
            renderSearchStub(state.search);
        } else {
            await renderProjects(items);
            renderPagination();
        }
    } catch (error) {
        if (requestId !== getLastRequestId()) return;

        if (container) {
            container.classList.remove("projects-grid--loading");
        }

        console.error("Error loading projects:", error);

        if (container) {
            container.innerHTML = `<div class="projects-error">Не удалось загрузить проекты.</div>`;
        }

        state.totalPages = 1;
        renderPagination();
    }
}