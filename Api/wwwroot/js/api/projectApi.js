export async function getProject(id) {

    const response = await fetch(`http://localhost:5100/api/projects/${id}`);

    if (!response.ok) {
        throw new Error("Failed to fetch project");
    }

    return await response.json();
}