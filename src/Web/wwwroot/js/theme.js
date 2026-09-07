(function () {
    var storageKey = "eshop-theme";
    var validThemes = ["default", "light", "dark"];

    function getTheme() {
        var theme = window.localStorage.getItem(storageKey);
        return validThemes.indexOf(theme) >= 0 ? theme : "default";
    }

    function applyTheme(theme) {
        theme = validThemes.indexOf(theme) >= 0 ? theme : "default";
        if (theme === "default") {
            document.documentElement.removeAttribute("data-theme");
        } else {
            document.documentElement.setAttribute("data-theme", theme);
        }
        window.localStorage.setItem(storageKey, theme);
    }

    applyTheme(getTheme());

    document.addEventListener("DOMContentLoaded", function () {
        var selector = document.querySelector("[data-theme-selector]");
        if (!selector) {
            return;
        }

        selector.value = getTheme();
        selector.addEventListener("change", function () {
            applyTheme(selector.value);
        });
    });
})();
