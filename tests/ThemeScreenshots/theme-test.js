const { chromium } = require('playwright');
const path = require('path');
const fs = require('fs');
const http = require('http');

const cssDir = path.join(__dirname, '..', '..', 'src', 'Web', 'wwwroot', 'css');
const screenshotDir = __dirname;

function readCSS(relativePath) {
    return fs.readFileSync(path.join(cssDir, relativePath), 'utf8').replace(/\uFEFF/g, '');
}

const allCSS = [
    readCSS('dark-theme.css'),
    readCSS('app.css'),
    readCSS('app.component.css'),
    readCSS('shared/components/header/header.css'),
    readCSS('shared/components/identity/identity.css'),
    readCSS('shared/components/pager/pager.css'),
    readCSS('basket/basket.component.css'),
    readCSS('basket/basket-status/basket-status.component.css'),
    readCSS('catalog/catalog.component.css'),
    readCSS('orders/orders.component.css'),
].join('\n');

const siteJS = fs.readFileSync(
    path.join(__dirname, '..', '..', 'src', 'Web', 'wwwroot', 'js', 'site.js'),
    'utf8'
);

function buildHTML(theme) {
    return `<!DOCTYPE html>
<html data-theme="${theme}">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>eShopOnWeb - ${theme} Theme</title>
    <style>${allCSS}</style>
    <style>
        .container { max-width: 1140px; margin: 0 auto; padding: 0 15px; }
        .row { display: flex; flex-wrap: wrap; }
        .col-lg-7, .col-md-6, .col-xs-12 { flex: 0 0 58%; }
        .col-lg-4, .col-md-5 { flex: 0 0 33%; }
        .col-lg-1 { flex: 0 0 8%; }
        .col-sm-6 { flex: 0 0 50%; }
        .col-md-4 { flex: 0 0 33.33%; }
        .col-md-3 { flex: 0 0 25%; }
        .col-md-2 { flex: 0 0 16.67%; }
        .col-md-9 { flex: 0 0 75%; }
        body { margin: 0; }
    </style>
</head>
<body>
    <div class="esh-app-wrapper">
        <header class="esh-app-header">
            <div class="container">
                <article class="row">
                    <section class="col-lg-7 col-md-6 col-xs-12">
                        <span style="font-size:1.5rem;font-weight:bold;color:var(--color-foreground);">eShop On Web</span>
                        <button id="theme-toggle" type="button" class="esh-theme-toggle" aria-label="Switch theme">${theme === 'dark' ? '☀️' : '🌙'}</button>
                    </section>
                    <section class="col-lg-4 col-md-5 col-xs-12">
                        <div class="esh-identity">
                            <section class="esh-identity-section">
                                <div class="esh-identity-item">
                                    <a href="#" class="esh-identity-name esh-identity-name--upper">Login</a>
                                </div>
                            </section>
                        </div>
                    </section>
                </article>
            </div>
        </header>

        <section class="esh-catalog-hero" style="background-color: var(--color-brand); display:flex; align-items:center;">
            <div class="container">
                <h1 style="color: #fff; font-size: 2rem;">Ready for a new adventure?</h1>
            </div>
        </section>

        <section class="esh-catalog-filters">
            <div class="container" style="display:flex;align-items:center;height:65px;">
                <label class="esh-catalog-label" data-title="brand">
                    <select class="esh-catalog-filter">
                        <option>All</option>
                        <option>.NET</option>
                    </select>
                </label>
                <label class="esh-catalog-label" data-title="type" style="margin-left:1rem;">
                    <select class="esh-catalog-filter">
                        <option>All</option>
                        <option>Mug</option>
                    </select>
                </label>
            </div>
        </section>

        <div class="container">
            <div class="esh-catalog-items row" style="margin-top:1rem;">
                <div class="esh-catalog-item col-md-4" style="padding:0.5rem;">
                    <div style="background:var(--color-background-dark);padding:1rem;border:1px solid var(--color-border);border-radius:4px;">
                        <div style="background:#ddd;height:180px;display:flex;align-items:center;justify-content:center;">
                            <span style="color:#888;">Product Image</span>
                        </div>
                        <div class="esh-catalog-name">.NET BOT BLACK SWEATSHIRT</div>
                        <div class="esh-catalog-price">19.50</div>
                        <button class="esh-catalog-button">[ ADD TO BASKET ]</button>
                    </div>
                </div>
                <div class="esh-catalog-item col-md-4" style="padding:0.5rem;">
                    <div style="background:var(--color-background-dark);padding:1rem;border:1px solid var(--color-border);border-radius:4px;">
                        <div style="background:#ddd;height:180px;display:flex;align-items:center;justify-content:center;">
                            <span style="color:#888;">Product Image</span>
                        </div>
                        <div class="esh-catalog-name">.NET BLUE HOODIE</div>
                        <div class="esh-catalog-price">12.00</div>
                        <button class="esh-catalog-button">[ ADD TO BASKET ]</button>
                    </div>
                </div>
                <div class="esh-catalog-item col-md-4" style="padding:0.5rem;">
                    <div style="background:var(--color-background-dark);padding:1rem;border:1px solid var(--color-border);border-radius:4px;">
                        <div style="background:#ddd;height:180px;display:flex;align-items:center;justify-content:center;">
                            <span style="color:#888;">Product Image</span>
                        </div>
                        <div class="esh-catalog-name">ROSLYN RED T-SHIRT</div>
                        <div class="esh-catalog-price">8.50</div>
                        <button class="esh-catalog-button">[ ADD TO BASKET ]</button>
                    </div>
                </div>
            </div>

            <div class="esh-pager-wrapper">
                <span class="esh-pager-item esh-pager-item--navigable">&lt; Previous</span>
                <span class="esh-pager-item">Showing 1 of 3 products - Page 1 - 1</span>
                <span class="esh-pager-item esh-pager-item--navigable">Next &gt;</span>
            </div>
        </div>

        <div class="container" style="margin-top:2rem;">
            <h3 style="color:var(--color-foreground);">Shopping Basket</h3>
            <div class="esh-basket">
                <div class="esh-basket-titles row">
                    <div class="col-md-4 esh-basket-title" style="color:var(--color-foreground);">Product</div>
                    <div class="col-md-2 esh-basket-title" style="color:var(--color-foreground);">Price</div>
                    <div class="col-md-2 esh-basket-title" style="color:var(--color-foreground);">Quantity</div>
                    <div class="col-md-2 esh-basket-title" style="color:var(--color-foreground);">Total</div>
                </div>
                <div class="esh-basket-items--border row" style="align-items:center;">
                    <div class="col-md-4 esh-basket-item" style="color:var(--color-foreground);">.NET BOT BLACK SWEATSHIRT</div>
                    <div class="col-md-2 esh-basket-item"><span class="esh-basket-item--mark">$19.50</span></div>
                    <div class="col-md-2 esh-basket-item" style="color:var(--color-foreground);">1</div>
                    <div class="col-md-2 esh-basket-item"><span class="esh-basket-item--mark">$19.50</span></div>
                </div>
                <a href="#" class="esh-basket-checkout">[ CHECKOUT ]</a>
            </div>
        </div>

        <div class="container" style="margin-top:2rem;">
            <h3 style="color:var(--color-foreground);">Form Elements</h3>
            <div class="alert alert-danger">This is a validation error message</div>
            <input type="text" class="form-input" placeholder="Text input field" style="background:var(--color-input-bg);color:var(--color-input-text);border:1px solid var(--color-input-border);" />
        </div>

        <footer class="esh-app-footer footer">
            <div class="container">
                <article class="row">
                    <section class="col-sm-6"></section>
                    <section class="col-sm-6">
                        <div class="esh-app-footer-text"> e-ShopOnWeb. All rights reserved </div>
                    </section>
                </article>
            </div>
        </footer>
    </div>
    <script>${siteJS}</script>
</body>
</html>`;
}

function startServer() {
    return new Promise((resolve) => {
        const server = http.createServer((req, res) => {
            const url = new URL(req.url, 'http://localhost');
            const rawTheme = url.searchParams.get('theme') || 'light';
            const theme = rawTheme === 'dark' ? 'dark' : 'light';
            res.writeHead(200, { 'Content-Type': 'text/html' });
            res.end(buildHTML(theme));
        });
        server.listen(0, '127.0.0.1', () => {
            resolve({ server, port: server.address().port });
        });
    });
}

(async () => {
    const { server, port } = await startServer();
    const baseUrl = `http://127.0.0.1:${port}`;
    console.log(`Test server running on ${baseUrl}`);

    const browser = await chromium.launch({ headless: true });
    const context = await browser.newContext({ viewport: { width: 1280, height: 900 } });

    // Screenshot: Light Theme
    console.log('Capturing Light Theme screenshot...');
    const lightPage = await context.newPage();
    await lightPage.goto(`${baseUrl}?theme=light`);
    await lightPage.waitForLoadState('networkidle');
    await lightPage.screenshot({
        path: path.join(screenshotDir, 'catalog-light-theme.png'),
        fullPage: true
    });
    console.log('Light theme screenshot saved.');

    // Screenshot: Dark Theme
    console.log('Capturing Dark Theme screenshot...');
    const darkPage = await context.newPage();
    await darkPage.addInitScript(() => {
        localStorage.setItem('eshop-theme', 'dark');
    });
    await darkPage.goto(`${baseUrl}?theme=dark`);
    await darkPage.waitForLoadState('networkidle');
    await darkPage.screenshot({
        path: path.join(screenshotDir, 'catalog-dark-theme.png'),
        fullPage: true
    });
    console.log('Dark theme screenshot saved.');

    // Test Theme Toggle (fresh context to avoid localStorage leakage)
    console.log('Testing theme toggle...');
    const toggleContext = await browser.newContext({ viewport: { width: 1280, height: 900 } });
    const togglePage = await toggleContext.newPage();
    await togglePage.goto(`${baseUrl}?theme=light`);
    await togglePage.waitForLoadState('networkidle');

    const initialTheme = await togglePage.evaluate(() =>
        document.documentElement.getAttribute('data-theme')
    );

    await togglePage.click('#theme-toggle');
    await togglePage.waitForFunction(
        () => document.documentElement.getAttribute('data-theme') === 'dark'
    );

    const afterToggle = await togglePage.evaluate(() =>
        document.documentElement.getAttribute('data-theme')
    );

    const storedTheme = await togglePage.evaluate(() =>
        localStorage.getItem('eshop-theme')
    );

    await togglePage.screenshot({
        path: path.join(screenshotDir, 'catalog-after-toggle.png'),
        fullPage: true
    });

    // Verify CSS variables
    const darkVars = await darkPage.evaluate(() => {
        const s = getComputedStyle(document.documentElement);
        return {
            bg: s.getPropertyValue('--color-background').trim(),
            fg: s.getPropertyValue('--color-foreground').trim(),
            footerBg: s.getPropertyValue('--color-background-footer').trim(),
        };
    });

    const lightVars = await lightPage.evaluate(() => {
        const s = getComputedStyle(document.documentElement);
        return {
            bg: s.getPropertyValue('--color-background').trim(),
            fg: s.getPropertyValue('--color-foreground').trim(),
            footerBg: s.getPropertyValue('--color-background-footer').trim(),
        };
    });

    // Assertions
    let passed = 0;
    let failed = 0;

    function assert(condition, message) {
        if (condition) { console.log(`  PASS: ${message}`); passed++; }
        else { console.log(`  FAIL: ${message}`); failed++; }
    }

    console.log('\n--- Test Results ---');
    assert(initialTheme === 'light', 'Initial theme should be light');
    assert(afterToggle === 'dark', 'After toggle, theme should be dark');
    assert(storedTheme === 'dark', 'Theme persisted in localStorage');
    assert(darkVars.bg === '#1e1e2e', 'Dark background var = #1e1e2e');
    assert(darkVars.fg === '#cdd6f4', 'Dark foreground var = #cdd6f4');
    assert(lightVars.bg === '#FFFFFF', 'Light background var = #FFFFFF');
    assert(lightVars.fg === '#000000', 'Light foreground var = #000000');

    const darkBodyBg = await darkPage.evaluate(() =>
        getComputedStyle(document.body).backgroundColor
    );
    assert(darkBodyBg === 'rgb(30, 30, 46)',
        `Dark body bg = rgb(30, 30, 46), got: ${darkBodyBg}`);

    const darkFooterBg = await darkPage.evaluate(() => {
        const el = document.querySelector('.esh-app-footer');
        const styles = getComputedStyle(el);
        return {
            bg: styles.backgroundColor,
            variable: styles.getPropertyValue('--color-background-footer'),
        };
    });
    assert(darkFooterBg.bg === 'rgb(17, 17, 27)',
        `Dark footer bg = rgb(17, 17, 27), got: ${darkFooterBg.bg} (var=${darkFooterBg.variable})`);

    const darkLinkColor = await darkPage.evaluate(() =>
        getComputedStyle(document.querySelector('.esh-identity-name')).color
    );
    assert(darkLinkColor !== 'rgb(0, 0, 0)',
        `Dark link color not black, got: ${darkLinkColor}`);

    console.log(`\n${passed} passed, ${failed} failed out of ${passed + failed} tests`);

    await browser.close();
    server.close();

    if (failed > 0) process.exit(1);
})();
