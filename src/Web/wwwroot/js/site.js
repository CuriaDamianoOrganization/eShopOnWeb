// Theme management
(function () {
    'use strict';

    function getStoredTheme() {
        return localStorage.getItem('eshop-theme');
    }

    function getPreferredTheme() {
        var stored = getStoredTheme();
        if (stored) {
            return stored;
        }
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }

    function setTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('eshop-theme', theme);
        updateToggleIcon(theme);
    }

    function updateToggleIcon(theme) {
        var btn = document.getElementById('theme-toggle');
        if (btn) {
            btn.setAttribute('aria-label', theme === 'dark' ? 'Switch to light theme' : 'Switch to dark theme');
            btn.textContent = theme === 'dark' ? '\u2600\uFE0F' : '\uD83C\uDF19';
        }
    }

    function toggleTheme() {
        var current = document.documentElement.getAttribute('data-theme') || 'light';
        var next = current === 'dark' ? 'light' : 'dark';
        setTheme(next);
    }

    // Apply theme on DOM ready
    document.addEventListener('DOMContentLoaded', function () {
        var theme = getPreferredTheme();
        setTheme(theme);

        var btn = document.getElementById('theme-toggle');
        if (btn) {
            btn.addEventListener('click', toggleTheme);
        }
    });

    // Listen for OS theme changes
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function (e) {
        if (!getStoredTheme()) {
            setTheme(e.matches ? 'dark' : 'light');
        }
    });
})();
