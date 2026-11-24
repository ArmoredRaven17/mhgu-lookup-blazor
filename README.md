# MHGU LookUp (Blazor)

A C# / Blazor WebAssembly port of [MHGU LookUp](https://github.com/ArmoredRaven17/MHGU-LookUp), a reference app for
**Monster Hunter Generations Ultimate** whose data is read straight from the game's ROM. The original is React +
TypeScript; this version runs the same app on .NET in the browser, as a static site on GitHub Pages with no server.

Created by **Armored_Raven**.

## What's in it

Every tab of the original: Monsters, Weapons, Armor, Armor Skills, Decorations, Hunting Styles, Hunter Arts, Quests,
Gathering, Items, Combo List, Canteen, Palico Equipment, Palico Skills & Moves, Bookmarks, Notes, Settings, Help and
About, with the same layout, search, filters, deep links, bookmarks, notes and colour themes.

## How it's built

```
src/MhguLookup.Core           Models, JSON converters, parsing and list logic. Plain .NET, no UI or browser dependency.
src/MhguLookup.Web            The Blazor WebAssembly app: pages, components and browser services.
tests/MhguLookup.Core.Tests   xUnit tests for Core, including tests that load every real data file.
```

- **Core is separate from the UI.** The models, parsing the game's text markup (`GameText`), filtering the list and
  drawing the upgrade tree's branch lines (`ListView`), grouping (`Grouping`: Hunter Art tiers, Special Permit
  Deviants), and the colours (`Appearance`, `GameColors`) are plain C#, so they are unit tested without a browser.
- **Typed models over loosely-shaped JSON.** The data files use positional arrays (`["Iron Ore", 3]`,
  `[itemId, name, qty, percent]`) and tagged unions (`{"kind": "quest", …}`). Small `JsonConverter`s in
  [`Core/Json`](src/MhguLookup.Core/Json) read the arrays into records, and `[JsonPolymorphic]` maps each `kind` to its
  own type, so pages work with `Material.Qty` and `switch` over unlock types instead of indexes and strings.
- **Source-generated JSON.** Data files are read with a `System.Text.Json` source-generated context
  (`LookupJsonContext`). It needs no runtime reflection, which is slow under WebAssembly, and survives the publish
  step's trimming of unused code. `AllDataTests` deserializes every shipped file, so a change to the data's shape
  fails a test rather than a page.
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
game. To refresh them, re-run those tools there and copy `public/data`, `public/assets` and `public/icons` across.
`AllDataTests` and `SkillDataTests` check the shipped files against the C# models.

## Performance

Most tabs open in about 0.4 s on a first visit, and every tab opens in under 0.1 s once its data is loaded (each file
is fetched and parsed once per session). The exception is a first visit to the three largest files: Quests (2.7 MB),
Armor (2.4 MB) and an item's sources (3.9 MB) take about 2 s each. That time is System.Text.Json parsing under the
WebAssembly interpreter, measured on the Release build: the same quests file takes 17 ms with the browser's native
`JSON.parse`, and rendering the tab afterwards takes under 0.1 s. Ahead-of-time (AOT) compilation would cut the
parse time, at the cost of a larger download and a longer build.

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
