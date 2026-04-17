function openOverlay() {
    document.querySelector('.overlay').style.display = 'block';
    document.querySelector('.overlay-bg').style.display = 'block';
    document.body.style.overflow = 'hidden';
}

function closeOverlay() {
    document.querySelector('.overlay').style.display = 'none';
    document.querySelector('.overlay-bg').style.display = 'none';
    document.body.style.overflow = '';
}

function copyLink(id) {
    const input = document.getElementById(id);
    input.select();
    input.setSelectionRange(0, 99999);
    navigator.clipboard.writeText(input.value).then(() => {
        alert("Ссылка скопирована!");
    });
}

document.addEventListener("DOMContentLoaded", function () {
    fetch('linkOverlay.html')
        .then(response => response.text())
        .then(html => {
            const container = document.createElement('div');
            container.innerHTML = html;
            document.body.appendChild(container);
        })
        .catch(err => console.error('Ошибка загрузки оверлея:', err));
});