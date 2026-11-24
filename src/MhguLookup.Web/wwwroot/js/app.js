// The few things Blazor cannot do from C#: setting CSS variables on <html>, scrolling an element into view,
// and the list panel's drag-to-resize (mouse moves are handled here so a drag does not cross into .NET
// on every pixel; .NET hears about it once, when the mouse is released).
window.mhgu = {
  setCssVar(name, value) {
    document.documentElement.style.setProperty(name, value)
  },

  scrollToId(id) {
    document.getElementById(id)?.scrollIntoView({ block: 'start' })
  },

  // Touch devices can leave a just-tapped nav link focused after navigating, which reads as "still selected".
  blurNav() {
    const el = document.activeElement
    if (el instanceof HTMLElement && el.closest('nav')) el.blur()
  },

  // Drag the list panel's handle to resize it; a press without a drag is a click, which collapses the panel.
  // Calls dotnet.OnResized(width) after a drag, or dotnet.OnHandleClicked() after a click.
  startPanelDrag(dotnet, panel, startX, min, max) {
    const DRAG_THRESHOLD = 4
    const startWidth = panel.getBoundingClientRect().width
    let moved = false
    let width = startWidth
    document.body.style.userSelect = 'none'
    document.body.style.cursor = 'ew-resize'

    const onMove = ev => {
      const dx = ev.clientX - startX
      if (Math.abs(dx) > DRAG_THRESHOLD) moved = true
      width = Math.round(Math.min(max, Math.max(min, startWidth + dx)))
      panel.style.width = panel.style.minWidth = `${width}px`
    }
    const onUp = () => {
      window.removeEventListener('mousemove', onMove)
      window.removeEventListener('mouseup', onUp)
      document.body.style.userSelect = ''
      document.body.style.cursor = ''
      if (moved) dotnet.invokeMethodAsync('OnResized', width)
      else dotnet.invokeMethodAsync('OnHandleClicked')
    }
    window.addEventListener('mousemove', onMove)
    window.addEventListener('mouseup', onUp)
  },
}
