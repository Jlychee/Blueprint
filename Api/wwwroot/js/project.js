import {getProject} from "/js/api/projectApi.js";

const params = new URLSearchParams(window.location.search);
const projectId = params.get("id");

function generateTabs(files) {

    const container = document.getElementById("tabs-content");

    if (!container) return;

    Object.entries(files).forEach(([key, value]) => {

        const tab = document.createElement("div");
        tab.className = "tab";
        tab.id = key.toLowerCase();

        let content = "";

        if (Array.isArray(value)) {

            content += "<h3>Ссылки:</h3><ul>";

            value.forEach(link => {
                content += `
                    <li>
                        <a href="${link}" target="_blank">${link}</a>
                    </li>
                `;
            });

            content += "</ul>";

        } else if (value) {

            content += `
                <h3>Ссылка:</h3>
                <p>
                    <a href="${value}" target="_blank">${value}</a>
                </p>
            `;

        }

        tab.innerHTML = content;

        container.appendChild(tab);

    });

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

function renderProject(project) {

    console.log(project);
    const title = document.getElementById("project-title");
    const shortDescription = document.getElementById("short-description");
    const description = document.getElementById("description");
    const stackContainer = document.getElementById("stack-icons");
    const year = document.getElementById("year");
    const semester = document.getElementById("semester");

    if (title) {
        title.textContent = project.name;
    }

    if (shortDescription) {
        shortDescription.textContent = project.shortDescription || "";
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

    if (stackContainer && project.tags && project.tags.length > 0) {

        stackContainer.innerHTML = "";

        project.tags.forEach(tag => {

            const img = document.createElement("img");

            img.src = `https://cdn.jsdelivr.net/gh/devicons/devicon/icons/${tag}/${tag}-original.svg`;
            img.alt = tag;
            img.title = tag;

            stackContainer.appendChild(img);

        });


    }
    console.log(project.files);
    generateTabs(project.files);
}

loadProject();