console.log('projectApi.js loaded');
console.log('project window.location.origin =', window.location.origin);

function buildUrl(path, params = new URLSearchParams()) {
    const query = params.toString();
    return `${path}${query ? `?${query}` : ''}`;
}

async function fetchJson(path, params = new URLSearchParams(), errorText = 'Request failed') {
    const url = buildUrl(path, params);
    console.log('API FETCH =>', url);

    const response = await fetch(url, {
        headers: {
            Accept: '*/*'
        }
    });

    if (!response.ok) {
        throw new Error(errorText);
    }

    return await response.json();
}

export async function getProject(id) {
    return fetchJson(
        `/api/projects/project/${id}`,
        new URLSearchParams(),
        'Failed to fetch project'
    );
}

export async function getAllProjects(filters = {}) {
    const params = new URLSearchParams();

    if (filters.search?.trim()) {
        params.append('Search', filters.search.trim());
    }

    if (Array.isArray(filters.tagIds)) {
        filters.tagIds.forEach((tagId) => {
            params.append('TagIds', String(tagId));
        });
    }

    if (filters.teamMemberCount != null) {
        params.append('TeamMemberCount', String(filters.teamMemberCount));
    }

    if (filters.year != null) {
        params.append('Year', String(filters.year));
    }

    if (filters.semester != null) {
        params.append('Semester', String(filters.semester));
    }

    params.append('Page', String(filters.page ?? 1));
    params.append('PageSize', String(filters.pageSize ?? 9));

    return fetchJson(
        '/api/projects/projects',
        params,
        'Failed to fetch projects'
    );
}

export async function getTags() {
    return fetchJson(
        '/api/projects/tags',
        new URLSearchParams(),
        'Failed to fetch tags'
    );
}