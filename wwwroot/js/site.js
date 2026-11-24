(() => {
    const storageKey = "moto-mobi-theme";
    const root = document.documentElement;
    const button = document.querySelector("[data-theme-toggle]");
    const storedTheme = localStorage.getItem(storageKey);
    root.dataset.theme = storedTheme === "light" || storedTheme === "dark" ? storedTheme : "dark";

    const updateButton = () => {
        if (!button) return;
        const isDark = root.dataset.theme === "dark";
        button.textContent = isDark ? "☼" : "☾";
        button.setAttribute("aria-label", isDark ? "Activer le thème clair" : "Activer le thème sombre");
        button.setAttribute("aria-pressed", String(isDark));
    };

    updateButton();
    button?.addEventListener("click", () => {
        root.dataset.theme = root.dataset.theme === "dark" ? "light" : "dark";
        localStorage.setItem(storageKey, root.dataset.theme);
        updateButton();
    });
})();
