import {getProject} from "./api/projectApi.js";

const params = new URLSearchParams(window.location.search);
const projectId = params.get("id");

function generateTabs(files) {
    const container = document.getElementById("tabs-content");
    if (!container) return;

    container.querySelectorAll(".tab[data-generated='true']").forEach(tab => tab.remove());

    Object.entries(files).forEach(([key, value]) => {
        const tab = document.createElement("div");
        tab.className = "tab";
        tab.id = String(key).toLowerCase();
        tab.dataset.generated = "true";

        if (isEmptyValue(value)) {
            tab.innerHTML = getEmptyStateMarkup(key);
            container.appendChild(tab);
            return;
        }

        if (Array.isArray(value)) {
            tab.innerHTML = renderLinksList(key, value);
            container.appendChild(tab);
            return;
        }

        tab.innerHTML = renderSingleLink(value);
        container.appendChild(tab);
    });
}

function isEmptyValue(value) {
    return value == null || (typeof value === "string" && value.trim() === "");
}

function getEmptyStateMarkup(key) {
    return `
        <div class="product-empty-state">
            <p class="product-empty-state__text">
                У этого продукта, к сожалению, нет данных о <span>${escapeHtml(key)}</span>
            </p>
            <img
                class="product-empty-state__image"
                src="resources/images/sad_bunny.svg"
                width="308"
                height="380"
                alt="Грустный кролик"
            >
        </div>
    `;
}

function renderLinksList(key, links) {
    if (links.length === 0) {
        return getEmptyStateMarkup(key);
    }

    const items = links
        .map(link => {
            const safeLink = escapeHtml(link);
            return `
                <li>
                    <a href="${safeLink}" target="_blank" rel="noopener noreferrer">
                        ${safeLink}
                    </a>
                </li>
            `;
        })
        .join("");

    return `
        <h3>Ссылки:</h3>
        <ul>
            ${items}
        </ul>
    `;
}

function renderSingleLink(link) {
    const safeLink = escapeHtml(link);

    return `
        <h3>Ссылка:</h3>
        <p>
            <a href="${safeLink}" target="_blank" rel="noopener noreferrer">
                ${safeLink}
            </a>
        </p>
    `;
}

function escapeHtml(value) {
    return String(value)
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}

async function loadProject() {

    if (!projectId) {
        console.error("Project id not found in URL");
        return;
    }

    try {

        const project = await getProject(projectId);

        renderProject(project);

    } catch (error) {

        console.error("Error loading project:", error);

        const container = document.getElementById("project-container");
        if (container) {
            container.innerHTML = "<p>Не удалось загрузить проект</p>";
        }

    }
}

function renderStack(tags) {
    const stackContainer = document.getElementById("icons");
    if (!stackContainer) return;

    stackContainer.innerHTML = "";

    if (!Array.isArray(tags) || tags.length === 0) {
        stackContainer.innerHTML = `
            <p class="stack-empty">Стек не указан</p>
        `;
        return;
    }

    for (const tag of tags) {
        const backgroundStyle = tag.color
            ? `style="filter: brightness(0) saturate(100%) invert(70%);
                      box-shadow: none;"`
            : "";
        stackContainer.innerHTML += `<div class="icon-item">
                <img
                    src="${tag.icon}"
                    alt="${tag.title}"
                    ${backgroundStyle}>
                <span class="icon-label">${formatTagName(tag.title)}</span>
            </div>`
    }
}

function formatTagName(tag) {
    const labels = {
        python: "Python",
        csharp: "C#",
        postgresql: "PostgreSQL",
        docker: "Docker",
        javascript: "JavaScript",
        typescript: "TypeScript",
        java: "Java",
        kotlin: "Kotlin",
        go: "Go",
        php: "PHP",
        html5: "HTML5",
        css3: "CSS3",
        react: "React",
        vuejs: "Vue",
        angularjs: "Angular",
        nodejs: "Node.js",
        express: "Express",
        dotnetcore: ".NET",
        mysql: "MySQL",
        mongodb: "MongoDB",
        redis: "Redis",
        git: "Git",
        linux: "Linux"
    };

    return labels[tag] || tag.charAt(0).toUpperCase() + tag.slice(1);
}

function renderProject(project) {

    const title = document.getElementById("project-title");
    const shortDescription = document.getElementById("short-description");
    const description = document.getElementById("description");
    const stackContainer = document.getElementById("stack-icons");
    const year = document.getElementById("year");
    const semester = document.getElementById("semester");
    const membersSection = document.querySelector(".members-section");
    const membersGrid = document.getElementById("members-grid");


    if (title) {
        title.textContent = project.name;
    }

    if (shortDescription) {
        shortDescription.textContent = project.description || "";
    }

    if (description) {
        description.textContent = project.description || "";
    }

    if (year) {
        year.textContent = project.year || "Год не указан";
    }

    if (semester) {
        semester.textContent = project.semester || "";
    }
    if (membersSection && membersGrid) {
        const members = project.teamMembers || [];
        const memberImages = [
            "resources/images/bow_blush.svg",
            "resources/images/sad_tear.svg",
            "resources/images/wink_star.svg",
            "resources/images/sleepy_moon.svg",
            "resources/images/sparkle_heart.svg",
            "resources/images/glasses_sad.svg",
        ];

        membersGrid.innerHTML = members.map((member, index) => {
            const memberName = member.userName || "Без имени";
            const avatar = memberImages[index % memberImages.length];

            return `
            <div class="member-card">
                <img class="member-avatar" src="${avatar}" alt="Аватарка для ${memberName}">
                <span class="member-name">${memberName}</span>
            </div>
        `;
        }).join("");
    }
    console.log(project.tags);
    renderStack(project.tags);

    generateTabs(project.files);
    if (typeof window.activateTabFromHash === "function") {
        window.activateTabFromHash(false);
    }
}

loadProject();