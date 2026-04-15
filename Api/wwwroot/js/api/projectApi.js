export async function getProject(id) {

    const response = await fetch(`http://213.165.213.101:8080/api/projects/project/${id}`);

    if (!response.ok) {
        throw new Error("Failed to fetch project");
    }

    return await response.json();
}