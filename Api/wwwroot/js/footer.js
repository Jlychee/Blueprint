async function loadFooter() {
    const footerContainer = document.getElementById('footer');
    if (!footerContainer) return;

    const footerPaths = [
        'resources/components/footer.html',
        'footer.html'
    ];

    let footerHtml = '';

    for (const path of footerPaths) {
        try {
            const response = await fetch(path);
            if (response.ok) {
                footerHtml = await response.text();
                break;
            }
        } catch (error) {
            console.error(`Не удалось загрузить footer по пути ${path}:`, error);
        }
    }

    if (!footerHtml) {
        console.error('Не удалось загрузить footer.html');
        return;
    }

    footerContainer.innerHTML = footerHtml;
}

loadFooter();