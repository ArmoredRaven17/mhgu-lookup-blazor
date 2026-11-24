# MHGU LookUp (Blazor)

A C# / Blazor WebAssembly port of [MHGU LookUp](https://github.com/ArmoredRaven17/MHGU-LookUp), a reference app for
**Monster Hunter Generations Ultimate** whose data is read straight from the game's ROM. The original is React +
TypeScript; this version runs the same app on .NET in the browser, as a static site on GitHub Pages with no server.

Created by **Armored_Raven**.

## Port status

The port goes one tab at a time. Tabs that aren't ported yet link to the same tab in the
[React version](https://armoredraven17.github.io/MHGU-LookUp/).

| Tab | Status |
|---|---|
| App shell: top bar, routing, resizable/collapsible list panel, search, bookmarks, notes, colour theme | Ported |
| Armor Skills | Ported |
| Weapons, Armor, Monsters, Decorations, Hunting Styles, Hunter Arts, Quests, Gathering, Items, Combo List, Canteen, Palico, Bookmarks, Notes, Settings, Help, About | Not yet |

## How it's built

```
src/MhguLookup.Core     Models, parsing and list logic. Plain .NET, with no UI or browser dependency.
src/MhguLookup.Web      The Blazor WebAssembly app: pages, components and browser services.
tests/MhguLookup.Core.Tests   xUnit tests for Core, including checks on the real data files.
```

- **Core is separate from the UI.** Parsing the game's text markup (`GameText`), filtering the list and drawing the
  upgrade tree's branch lines (`ListView`), and the colour themes (`Appearance`) are plain C#, so they are unit
  tested without a browser.
- **Source-generated JSON.** Data files are read with a `System.Text.Json` source-generated context
  (`LookupJsonContext`). It needs no runtime reflection, which is slow under WebAssembly, and survives the publish
  step's trimming of unused code.
- **Synchronous JS interop.** Under WebAssembly, .NET and JavaScript share a thread, so localStorage is read through
  `IJSInProcessRuntime` directly into a component's first render, with no async loading state.
- **Text scaling in CSS.** Every font size is `calc(Npx * var(--text-scale))`, so changing the text size updates one
  CSS variable instead of re-rendering every component.
- **Minimal JavaScript.** [`wwwroot/js/app.js`](src/MhguLookup.Web/wwwroot/js/app.js) is about 50 lines: setting CSS
  variables, scrolling to an element, and the list panel's drag-to-resize (kept in JS so a drag doesn't cross into
  .NET on every mouse move).
- **Static hosting.** GitHub Pages serves the site from `/<repo>/`, has no URL rewrites, and runs Jekyll. The deploy
  workflow sets `<base href>`, copies `index.html` to `404.html` so deep links load the app, and adds `.nojekyll` so
  the `_framework` folder is published.

Bookmarks, notes and settings are kept in the browser's localStorage under `mhgu-lookup-blazor-*` keys. The React
app is on the same github.io origin and uses `mhgu-lookup-*`, so the two never overwrite each other's data.

## Data

`src/MhguLookup.Web/wwwroot/data/*.json` and the icons are copied unchanged from MHGU LookUp, whose
[Python tools](https://github.com/ArmoredRaven17/MHGU-LookUp/tree/master/tools) read them from a dumped copy of the
game. To refresh them, re-run those tools there and copy `public/data` and `public/assets` across.
`SkillDataTests` checks the shipped `skills.json` against the C# models.

## Build and run

Needs the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet run --project src/MhguLookup.Web   # http://localhost:5640
dotnet test
dotnet publish src/MhguLookup.Web -c Release -o publish
```

Pushing to `master` runs the tests and deploys to GitHub Pages
([.github/workflows/deploy-pages.yml](.github/workflows/deploy-pages.yml)). Pull requests run the tests only.

## License

Source code: MIT ([LICENSE](LICENSE)). **Monster Hunter** names, text and other game content are
© CAPCOM CO., LTD., used as non-commercial fan content; this app is not affiliated with or endorsed by Capcom.
Built with substantial help from [Claude Code](https://claude.com/claude-code), directed and reviewed by the author.
