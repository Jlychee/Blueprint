import {state} from "./catalogState.js";
import {saveCatalogState} from "./catalogUrl.js";
import {loadProjects} from "./catalogLoader.js";
import {scrollToProjectsTop} from "./catalogRender.js";

export function bindPagination() {
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