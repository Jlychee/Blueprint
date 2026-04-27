import {PAGE_SIZE, state} from "./catalogState.js";

export function saveCatalogState() {
    writeCatalogStateToUrl();
}

function getPositiveNumber(value, fallback) {
    const number = Number(value);
    return Number.isFinite(number) && number > 0 ? number : fallback;
}

export function readCatalogStateFromUrl() {
    const params = new URLSearchParams(window.location.search);

    state.search = (params.get("search") || "").trim();

    state.tagIds = params
        .getAll("tagIds")
        .flatMap((value) => value.split(","))
        .map(Number)
        .filter(Number.isFinite);

    const year = params.get("year");
    state.year = year ? Number(year) : null;

    state.page = getPositiveNumber(params.get("page"), 1);
    state.pageSize = getPositiveNumber(params.get("pageSize"), PAGE_SIZE);
}

export function writeCatalogStateToUrl() {
    const params = new URLSearchParams();

    if (state.search) {
        params.set("search", state.search);
    }

    state.tagIds.forEach((tagId) => {
        params.append("tagIds", String(tagId));
    });

    if (state.year) {
        params.set("year", String(state.year));
    }

    if (state.page > 1) {
        params.set("page", String(state.page));
    }

    if (state.pageSize !== PAGE_SIZE) {
        params.set("pageSize", String(state.pageSize));
    }

    const query = params.toString();
    const nextUrl = `${window.location.pathname}${query ? `?${query}` : ""}${window.location.hash}`;

    history.replaceState(null, "", nextUrl);
}