window.toggleSection = function (el) {
    el.classList.toggle("active");

    const inner = el.closest(".toggle")?.querySelector(".toggle-inner");
    if (!inner) return;

    inner.style.overflow = "hidden";
    inner.style.transition = "max-height 0.25s ease";

    if (!inner.style.maxHeight || inner.style.maxHeight === "0%") {
        inner.style.display = "block";
        inner.style.maxHeight = "100%";
    } else {
        inner.style.maxHeight = "0%";
        inner.style.display = "none";
    }
};