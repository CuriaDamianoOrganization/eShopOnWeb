(function () {
    var key = "eshop-theme";
    var root = document.documentElement;
    var preference = window.matchMedia("(prefers-color-scheme: dark)");
    var currentSessionTheme = null;

    function selectedTheme() {
        if (currentSessionTheme) return currentSessionTheme;
        var saved;
        try {
            saved = localStorage.getItem(key);
        } catch (error) {
            saved = null;
        }
        return saved === "light" || saved === "dark"
            ? saved
            : (preference.matches ? "dark" : "light");
    }

    function applyTheme() {
        var theme = selectedTheme();
        root.setAttribute("data-theme", theme);
        root.style.colorScheme = theme;
        document.querySelectorAll("[data-theme-toggle]").forEach(function (button) {
            var label = theme === "dark" ? "Light theme" : "Dark theme";
            if (button.textContent !== label) button.textContent = label;
            button.setAttribute("aria-label", "Switch to " + (theme === "dark" ? "light" : "dark") + " theme");
        });
    }

    applyTheme();
    document.addEventListener("DOMContentLoaded", applyTheme);
    document.addEventListener("click", function (event) {
        if (!event.target.closest("[data-theme-toggle]")) return;
        currentSessionTheme = selectedTheme() === "dark" ? "light" : "dark";
        try {
            localStorage.setItem(key, currentSessionTheme);
        } catch (error) {
            // Storage can be unavailable in private browsing.
        }
        applyTheme();
    });
    if (preference.addEventListener) preference.addEventListener("change", applyTheme);
    else preference.addListener(applyTheme);
    new MutationObserver(function () {
        if (document.querySelector("[data-theme-toggle]")) applyTheme();
    }).observe(document.documentElement, { childList: true, subtree: true });
}());
