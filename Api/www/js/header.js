async function loadHeader() {
    const response = await fetch("resources/components/header.html");
    document.getElementById("header").innerHTML = await response.text();
}

loadHeader();