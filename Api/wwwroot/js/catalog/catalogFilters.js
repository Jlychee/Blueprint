import {getTags} from "../api/projectApi.js";

import {
    AVAILABLE_YEARS,
    isFiltersInitialized,
    markFiltersInitialized,
    state
} from "./catalogState.js";

import {
    syncUiWithState
} from "./catalogRender.js";

import {
    refreshCatalog
} from "./catalogLoader.js";

function renderTagsFilters(tagGroups) {
    const tagsContainer = document.getElementById("filter-tags-list");
    if (!tagsContainer) return;

    tagsContainer.innerHTML = "";

    tagGroups.forEach((group) => {
        if (!group || !Array.isArray(group.tags) || group.tags.length === 0) {
            return;
        }

        const section = document.createElement("div");
        section.className = "filter-group filter-subsection";

        const title = document.createElement("button");
        title.type = "button";
        title.className = "filter-subsection-title";
        title.dataset.filterToggle = "";

        const heading = document.createElement("h3");
        heading.textContent = group.type || "Теги";

        const icon = document.createElement("img");
        icon.src = "resources/images/chevron.svg";
        icon.className = "filter-toggle-icon";
        icon.alt = "";

        title.appendChild(icon);
        title.appendChild(heading);
        section.appendChild(title);

        const content = document.createElement("div");
        content.className = "filter-subsection-content";

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
            content.appendChild(row);
        });

        section.appendChild(content);
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

    if (typeof window.initSearchAndFilter === "function") {
        window.initSearchAndFilter();
    }

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

export async function initFiltersUi() {
    const tagsContainer = document.getElementById("filter-tags-list");
    const yearsContainer = document.getElementById("filter-years-list");

    if (!tagsContainer || !yearsContainer || isFiltersInitialized()) return;

    markFiltersInitialized();

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