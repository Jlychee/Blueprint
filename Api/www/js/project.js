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
    }
];


const params = new URLSearchParams(window.location.search);
const projectId = params.get("id");


const project = projects.find(p => p.id === projectId);


if(project){

    document.getElementById("project-title").textContent = project.title;

    document.getElementById("short-description").textContent = project.description;

    const stackContainer = document.getElementById("stack-icons");

    project.stack.forEach(tech => {

        const img = document.createElement("img");

        img.src = `https://cdn.jsdelivr.net/gh/devicons/devicon/icons/${tech}/${tech}-original.svg`;

        stackContainer.appendChild(img);

    });

}