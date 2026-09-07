(function () {
    var storageKey = "eshop-theme";
    var validThemes = ["default", "light", "dark"];

    function getTheme() {
        var theme = window.localStorage.getItem(storageKey);
        return validThemes.indexOf(theme) >= 0 ? theme : "default";
    }

    function applyTheme(theme) {
        theme = validThemes.indexOf(theme) >= 0 ? theme : "default";
        if (theme === "dark" ||
            (theme === "default" && window.matchMedia("(prefers-color-scheme: dark)").matches)) {
            document.documentElement.setAttribute("data-theme", theme);
        } else {
            document.documentElement.removeAttribute("data-theme");
        }
        window.localStorage.setItem(storageKey, theme);
    }

    document.addEventListener("DOMContentLoaded", function () {
        var selector = document.querySelector("[data-theme-selector]");
        if (!selector) {
            return;
        }

        selector.value = getTheme();
        selector.addEventListener("change", function () {
            applyTheme(selector.value);
        });

        window.matchMedia("(prefers-color-scheme: dark)").addEventListener("change", function () {
            if (getTheme() === "default") {
                applyTheme("default");
            }
        });
    });
})();
