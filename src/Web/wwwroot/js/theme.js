// Theme Management System for eShopOnWeb
(function() {
    'use strict';
    
    const THEME_STORAGE_KEY = 'eshop-theme-preference';
    const THEME_LIGHT = 'light';
    const THEME_DARK = 'dark';
    const THEME_AUTO = 'auto';
    
    // Initialize theme system
    function initThemeSystem() {
        // Get saved theme preference or default to auto
        const savedTheme = localStorage.getItem(THEME_STORAGE_KEY) || THEME_AUTO;
        applyTheme(savedTheme);
        
        // Listen for system theme changes when in auto mode
        if (window.matchMedia) {
            const darkModeQuery = window.matchMedia('(prefers-color-scheme: dark)');
            darkModeQuery.addEventListener('change', function(e) {
                const currentTheme = localStorage.getItem(THEME_STORAGE_KEY) || THEME_AUTO;
                if (currentTheme === THEME_AUTO) {
                    applyTheme(THEME_AUTO);
                }
            });
        }
    }
    
    // Apply theme based on preference
    function applyTheme(preference) {
        let actualTheme = preference;
        
        // If auto, detect system preference
        if (preference === THEME_AUTO) {
            if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
                actualTheme = THEME_DARK;
            } else {
                actualTheme = THEME_LIGHT;
            }
        }
        
        // Apply theme to document
        if (actualTheme === THEME_DARK) {
            document.documentElement.setAttribute('data-theme', 'dark');
        } else {
            document.documentElement.removeAttribute('data-theme');
        }
        
        // Update UI state if toggle exists
        updateThemeToggleUI(preference);
    }
    
    // Update theme toggle UI to reflect current state
    function updateThemeToggleUI(preference) {
        const themeToggles = document.querySelectorAll('.theme-toggle');
        themeToggles.forEach(function(themeToggle) {
            if (themeToggle) {
                themeToggle.setAttribute('data-theme-preference', preference);
                
                // Update button text/icon based on preference
                const icon = themeToggle.querySelector('.theme-icon');
                const label = themeToggle.querySelector('.theme-label');
                
                if (icon && label) {
                    switch(preference) {
                        case THEME_LIGHT:
                            icon.textContent = '☀️';
                            label.textContent = 'Light';
                            break;
                        case THEME_DARK:
                            icon.textContent = '🌙';
                            label.textContent = 'Dark';
                            break;
                        case THEME_AUTO:
                            icon.textContent = '🔄';
                            label.textContent = 'Auto';
                            break;
                    }
                }
            }
        });
    }
    
    // Cycle through theme options: Light -> Dark -> Auto -> Light
    function cycleTheme() {
        const currentTheme = localStorage.getItem(THEME_STORAGE_KEY) || THEME_AUTO;
        let nextTheme;
        
        switch(currentTheme) {
            case THEME_LIGHT:
                nextTheme = THEME_DARK;
                break;
            case THEME_DARK:
                nextTheme = THEME_AUTO;
                break;
            case THEME_AUTO:
                nextTheme = THEME_LIGHT;
                break;
            default:
                nextTheme = THEME_AUTO;
        }
        
        localStorage.setItem(THEME_STORAGE_KEY, nextTheme);
        applyTheme(nextTheme);
    }
    
    // Expose theme functions globally
    window.themeManager = {
        init: initThemeSystem,
        cycle: cycleTheme,
        apply: applyTheme,
        get: function() {
            return localStorage.getItem(THEME_STORAGE_KEY) || THEME_AUTO;
        },
        set: function(theme) {
            if ([THEME_LIGHT, THEME_DARK, THEME_AUTO].includes(theme)) {
                localStorage.setItem(THEME_STORAGE_KEY, theme);
                applyTheme(theme);
            }
        }
    };
    
    // Auto-initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initThemeSystem);
    } else {
        initThemeSystem();
    }
})();
