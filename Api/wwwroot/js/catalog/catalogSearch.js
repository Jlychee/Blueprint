import {state} from "./catalogState.js";
import {saveCatalogState} from "./catalogUrl.js";
import {loadProjects, refreshCatalog} from "./catalogLoader.js";

export function clearCatalogSearch() {
    state.search = "";
    state.page = 1;

    const searchInput = document.getElementById("search-input");
    if (searchInput) {
        searchInput.value = "";
    }

    refreshCatalog();
}

export function bindSearch() {
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