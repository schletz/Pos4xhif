function toggleSidebar() {
    const navbar = document.querySelector(".sidebar");
    navbar.style.display = window.getComputedStyle(navbar).display != "block" ? "block" : "none";
}

for (const item of document.querySelectorAll(".sidebar-item")) {
    const link = item.querySelector("a");
    if (!(link instanceof HTMLAnchorElement)) { continue; }
    if (window.location.href == link.href) {
        item.classList.add("sidebar-active-item");
    }
}
