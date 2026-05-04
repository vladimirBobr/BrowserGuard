# BrowserGuard

Windows application that monitors your default browser and automatically resets it to your preferred browser.

## Why do you need this?

There are many situations where you need to keep a specific browser as default:

### Personal use
- You prefer a specific browser but sometimes other apps change your default
- You want consistency across all links opening in your chosen browser

### Enterprise / Organization use
- **Corporate policies**: Your IT department mandates using a specific browser for security compliance
- **Software compatibility**: Certain internal tools work only with a specific browser
- **Testing**: QA teams need to test in a specific browser environment
- **Kiosk mode**: Public computers that should always open links in a designated browser

### The problem
Windows and various applications frequently change the default browser (updates, new browser installations, user actions). There's no built-in Windows setting to permanently lock the default browser.

### The solution
BrowserGuard continuously monitors your default browser and automatically restores it to your preferred one whenever it changes.

## Features

- Monitors default browser at configurable interval
- Automatically resets to your preferred browser
- System tray application - runs in background
- Customizable preferred browser
- Add to Windows startup option

## Installation

1. Download the latest release
2. Run `BrowserGuard.exe`
3. On first run, enter your preferred browser name (e.g., Chrome, Edge)

## Configuration

Edit `config.ini`:
```
preferredBrowser=Chrome
usePowerShellMethod=0
checkInterval=15000
```

| Parameter | Description | Default |
|-----------|-------------|---------|
| preferredBrowser | Browser name to check (Chrome, Edge, Firefox) | Chrome |
| usePowerShellMethod | 1 = PowerShell method, 0 = Native UI | 0 |
| checkInterval | Check interval in milliseconds | 15000 |

## Methods

### Native UI Method (default)
Uses Windows Settings UI automation to set the default browser. Opens `ms-settings:defaultapps` and simulates keyboard input via SendKeys. Works without additional dependencies.

### PowerShell Method
Alternative method that uses PowerShell script (`set-default-browser.ps1`) to interact with Windows Settings. Enable with `usePowerShellMethod=1`.

**Requirements for PowerShell method:**
- PowerShell 5.1 (Windows PowerShell) or PowerShell 7+
- PowerShell execution policy must allow running scripts

The script performs the same UI automation as the native method but runs in a separate PowerShell process.

## Requirements

- Windows 10+
- .NET Framework 4.8

## License

MIT