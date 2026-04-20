const buttons = document.querySelectorAll(".tab-btn");

function activateTab(tabId, updateHash = true) {
    const tabs = document.querySelectorAll(".tab");

    let targetId = tabId || "short";

    const targetTab = document.getElementById(targetId);
    const targetButton = document.querySelector(`.tab-btn[data-tab="${targetId}"]`);

    if (!targetTab || !targetButton) {
        targetId = "short";
    }

    buttons.forEach(btn => {
        btn.classList.toggle("active", btn.dataset.tab === targetId);
    });

    document.querySelectorAll(".tab").forEach(tab => {
        tab.classList.toggle("active", tab.id === targetId);
    });

    if (updateHash) {
        history.replaceState(null, "", `${window.location.pathname}${window.location.search}#${targetId}`);
    }
}

function activateTabFromHash(updateHash = false) {
    const hash = window.location.hash.replace("#", "").trim();
    activateTab(hash || "short", updateHash);
}

buttons.forEach(button => {
    button.addEventListener("click", () => {
        activateTab(button.dataset.tab, true);
    });
});

document.addEventListener("DOMContentLoaded", () => {
    activateTabFromHash(false);
});

window.addEventListener("hashchange", () => {
    activateTabFromHash(false);
});

window.activateTabFromHash = activateTabFromHash;
window.activateTab = activateTab;