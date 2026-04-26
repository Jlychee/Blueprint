import {
    isBaseInitialized,
    markBaseInitialized
} from "./catalog/catalogState.js";

import {
    readCatalogStateFromUrl
} from "./catalog/catalogUrl.js";

import {
    syncUiWithState
} from "./catalog/catalogRender.js";

import {
    loadProjects,
    refreshCatalog
} from "./catalog/catalogLoader.js";

import {
    bindSearch,
    clearCatalogSearch
} from "./catalog/catalogSearch.js";

import {
    bindPagination
} from "./catalog/catalogPagination.js";

import {
    initFiltersUi
} from "./catalog/catalogFilters.js";

window.clearCatalogSearch = clearCatalogSearch;

function initProjectCatalog() {
    if (!isBaseInitialized()) {
        markBaseInitialized();

        readCatalogStateFromUrl();
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

window.addEventListener("popstate", () => {
    readCatalogStateFromUrl();
    syncUiWithState();
    loadProjects();
});