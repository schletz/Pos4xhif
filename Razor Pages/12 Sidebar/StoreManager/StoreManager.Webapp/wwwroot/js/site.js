function toggleSidebar() {
    const navbar = document.querySelector(".sidebar");
    navbar.style.display = window.getComputedStyle(navbar).display != "block" ? "block" : "none";
}
const items = Array.from(document.querySelectorAll(".sidebar-item"));
const active = items.find(i => window.location.href.includes(i.querySelector("a").href));
if (active) {
    active.classList.add("sidebar-active-item");
}
