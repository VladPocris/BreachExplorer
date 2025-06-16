function copyToClipboard(text) {
    navigator.clipboard.writeText(text).then(function () {
        console.log('Text copied to clipboard');
    }).catch(function (error) {
        console.error('Error copying text: ', error);
    });
}

document.addEventListener('DOMContentLoaded', function () {
    // Make sure Bootstrap’s Dropdown plugin is loaded
    if (window.bootstrap && bootstrap.Dropdown) {
        // Grab the existing default popperConfig
        const origPopperConfig = bootstrap.Dropdown.Default.popperConfig;

        // Override it with our own function
        bootstrap.Dropdown.Default.popperConfig = function (defaultBsPopperConfig) {
            // Allow the original config (if it’s a function) to run first
            const cfg = typeof origPopperConfig === 'function'
                ? origPopperConfig(defaultBsPopperConfig)
                : { ...origPopperConfig };

            // Force it to open “top-start”
            cfg.placement = 'top-start';

            // Disable any flipping so it never goes back down
            cfg.modifiers = (cfg.modifiers || []).map(mod => {
                if (mod.name === 'flip') {
                    return { name: 'flip', options: { fallbackPlacements: [] } };
                }
                return mod;
            });

            return cfg;
        };
    }
});
