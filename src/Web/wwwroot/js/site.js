// Theme management
(function() {
    // Get theme from localStorage or default to 'auto'
    function getTheme() {
        return localStorage.getItem('theme') || 'auto';
    }

    // Save theme to localStorage
    function saveTheme(theme) {
        localStorage.setItem('theme', theme);
    }

    // Get effective theme (resolves 'auto' to 'light' or 'dark')
    function getEffectiveTheme(theme) {
        if (theme === 'auto') {
            // Check system preference
            return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
        }
        return theme;
    }

    // Apply theme to document
    function applyTheme(theme) {
        var effectiveTheme = getEffectiveTheme(theme);
        document.documentElement.setAttribute('data-theme', effectiveTheme);
    }

    // Set theme and save preference
    function setTheme(theme) {
        saveTheme(theme);
        applyTheme(theme);
        updateThemeButton(theme);
    }

    // Update theme button text
    function updateThemeButton(theme) {
        var button = document.getElementById('theme-toggle');
        if (button) {
            var themeText = theme.charAt(0).toUpperCase() + theme.slice(1);
            button.textContent = '🎨 ' + themeText;
        }
    }

    // Initialize theme on page load
    function initTheme() {
        var theme = getTheme();
        applyTheme(theme);
        updateThemeButton(theme);

        // Listen for system theme changes when in auto mode
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function() {
            var currentTheme = getTheme();
            if (currentTheme === 'auto') {
                applyTheme('auto');
            }
        });

        // Setup theme toggle button
        var button = document.getElementById('theme-toggle');
        if (button) {
            button.addEventListener('click', function() {
                var currentTheme = getTheme();
                var newTheme;
                
                // Cycle through: light -> dark -> auto -> light
                if (currentTheme === 'light') {
                    newTheme = 'dark';
                } else if (currentTheme === 'dark') {
                    newTheme = 'auto';
                } else {
                    newTheme = 'light';
                }
                
                setTheme(newTheme);
            });
        }
    }

    // Initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initTheme);
    } else {
        initTheme();
    }

    // Make setTheme available globally for programmatic use
    window.setTheme = setTheme;
})();
