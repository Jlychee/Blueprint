import {state} from "./catalogState.js";

export function syncUiWithState() {
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

export function escapeHtml(value) {
    return String(value)
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}

export function renderSearchStub(query) {
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

export function scrollToProjectsTop() {
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

export function renderPagination() {
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

export function renderProjects(items) {
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