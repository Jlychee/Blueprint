const buttons = document.querySelectorAll(".tab-btn")
const tabs = document.querySelectorAll(".tab")

buttons.forEach(button => {

    button.addEventListener("click", () => {

        const tabId = button.dataset.tab

        buttons.forEach(btn => btn.classList.remove("active"))
        tabs.forEach(tab => tab.classList.remove("active"))

        button.classList.add("active")
        document.getElementById(tabId).classList.add("active")

    })

})