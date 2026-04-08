const projects = [
    {
        id: 'vibik',
        title: "Vibik",
        description: "Приложение с фото-заданиями, которое мотивирует выйти из рутины и прогуляться.",
        stack: [
            "python",
            "csharp",
            "postgresql",
            "docker"
        ]
    },

    {
        id: 'matmeh-maps',
        title: "Матмех мэпс",
        description: "Приложение — навигатор по матмеху",
        stack: [
            "csharp",
            "docker",
            "javascript",
            "css3"
        ]
    },

    {
        id: 'stud-compas',
        title: "СтудКомпас",
        description: "Приложение — навигатор по матмеху",
        stack: [
            "csharp",
            "docker",
            "javascript",
            "css3"
        ]
    },
    {
        id: 'vibik',
        title: "Vibik",
        description: "Приложение с фото-заданиями, которое мотивирует выйти из рутины и прогуляться.",
        stack: [
            "python",
            "csharp",
            "postgresql",
            "docker"
        ]
    },

    {
        id: 'matmeh-maps',
        title: "Матмех мэпс",
        description: "Приложение — навигатор по матмеху",
        stack: [
            "csharp",
            "docker",
            "javascript",
            "css3"
        ]
    },

    {
        id: 'stud-compas',
        title: "СтудКомпас",
        description: "Приложение — навигатор по матмеху",
        stack: [
            "csharp",
            "docker",
            "javascript",
            "css3"
        ]
    },
];

const container = document.getElementById("projects-grid");
const template = document.getElementById("project-card-template");

projects.forEach(project => {

    const clone = template.content.cloneNode(true);
    const card = clone.querySelector(".card");

    card.href = `project.html?id=${project.id}`;

    clone.querySelector(".project-title").textContent = project.title;
    clone.querySelector(".project-description").textContent = project.description;

    const iconsContainer = clone.querySelector(".icons");

    project.stack.forEach(tech => {

        const img = document.createElement("img");

        img.src = `https://cdn.jsdelivr.net/gh/devicons/devicon/icons/${tech}/${tech}-original.svg`;

        iconsContainer.appendChild(img);

    });

    container.appendChild(clone);

});