const buttons = document.querySelectorAll(".tab-btn");

buttons.forEach(button => {

    button.addEventListener("click", () => {

        const tabId = button.dataset.tab;

        const tabs = document.querySelectorAll(".tab");

        buttons.forEach(btn => btn.classList.remove("active"));
        tabs.forEach(tab => tab.classList.remove("active"));

        button.classList.add("active");

        const targetTab = document.getElementById(tabId);
        if (targetTab) {
            targetTab.classList.add("active");
        }

    });

});