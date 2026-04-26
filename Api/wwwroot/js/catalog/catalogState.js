export const PAGE_SIZE = 9;
export const AVAILABLE_YEARS = [2021, 2022, 2023, 2024, 2025, 2026];

export const state = {
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

export function isBaseInitialized() {
    return baseInitialized;
}

export function markBaseInitialized() {
    baseInitialized = true;
}

export function isFiltersInitialized() {
    return filtersInitialized;
}

export function markFiltersInitialized() {
    filtersInitialized = true;
}

export function getNextRequestId() {
    lastRequestId += 1;
    return lastRequestId;
}

export function getLastRequestId() {
    return lastRequestId;
}

export function hasActiveFilters() {
    return Boolean(
        state.search ||
        (Array.isArray(state.tagIds) && state.tagIds.length > 0) ||
        state.year
    );
}