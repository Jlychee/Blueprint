

export async function getProject(id) {

    const response = await fetch(`http://localhost:5100/api/projects/project/${id}`);

    if (!response.ok) {
        throw new Error("Failed to fetch project");
    }

    return await response.json();
}

export async function getAllProjects() {
    const response = await fetch(`http://localhost:5100/api/projects/projects`);
    
    if (!response.ok) {
        throw new Error("Failed to fetch projects");
    }
    
    return await response.json();

}

export async function getTags() {
    const response = await fetch(`http://localhost:5100/api/projects/tags`);
    if (!response.ok) {
        throw new Error("Failed to fetch tags");
    }
    return await response.json();
}