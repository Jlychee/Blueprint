fetch('resources/components/filters.html')
    .then(r => r.text())
    .then(html => {
        document.getElementById('filters-container').innerHTML = html
    })