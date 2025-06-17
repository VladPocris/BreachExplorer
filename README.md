# BreachExplorer

BreachExplorer is a small Blazor WebAssembly app that lets you **see the biggest data‑breaches in one glance and check if your own e‑mail address has been pwned**. It pulls breach data from *Have I Been Pwned?* and serves it through a tiny ASP.NET Core Web API proxy (to dodge CORS), then renders interactive charts right in the browser.

**Live demo (OLD):** https://gcstorageacc2.z9.web.core.windows.net/  (Student account closed therefore deployment migrated to GitHub & render.com)
**Live demo (NEW):** https://vladpocris.github.io/BreachExplorer/    (API's were premium so at the moment some don't work or have limited functionality. FIRST REQUESTS TAKE UP TO 50 SECONDS BECAUSE OF Render.com FREE HOSTING PLAN, THEN IT WORKS AS INTENDED)
**Source:** https://github.com/VladPocris/BreachExplorer

## What it does
- Shows the *latest* verified breach with full details.
- Interactive bar chart of the top 15 breaches, toggle horizontal / vertical.
- Custom chart: search any breached domain, add / remove bars on the fly.
- “Was I Breached?” lookup for your email, with friendly success or breach report.
- Optional strong‑password generator (API key‑powered).

# BreachProxyAPI Service (originally deployed on azure, now on render.com)
A proxy to bypas CORS.
**Live Endpoint:** https://breachexplorer.onrender.com/api/Breaches/breachedaccount/vpocris@gmail.com

## Quick start
```bash
git clone https://github.com/VladPocris/BreachExplorer.git
dotnet run --project src/BreachExplorer   # runs on https://localhost:5001
