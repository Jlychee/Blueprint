function getCookie(name) {
    let matches = document.cookie.match(new RegExp(
        "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
    ));
    return matches ? decodeURIComponent(matches[1]) : undefined;
}

function setCookie(name, value, days = 365) {
    const expires = new Date(Date.now() + days * 24 * 60 * 60 * 1000).toUTCString();
    document.cookie = `${name}=${encodeURIComponent(value)}; expires=${expires}; path=/; SameSite=Lax`;
}

function generateId() {
    return crypto.randomUUID();
}

function getOrCreateUserMetricId() {
    let id = getCookie('metric_user_id');

    if (!id) {
        id = generateId();
        setCookie('metric_user_id', id, 365);
    }

    return id;
}

function deleteFilterSessionId(name) {
    document.cookie = `${name}=; max-age=0; path=/`;
}

function getOrCreateFilterSessionId(needReset = false) {
    if (needReset) {
        deleteFilterSessionId('filter_session_id')
    } else {
        let id = getCookie('filter_session_id');

        if (needReset || !id) {
            id = generateId();
            setCookie('filter_session_id', id, 1);
        }

        return id;
    }

}


document.addEventListener('DOMContentLoaded', () => {
    getOrCreateUserMetricId();
});
