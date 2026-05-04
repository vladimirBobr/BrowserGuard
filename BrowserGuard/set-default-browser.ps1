if ($env:OS -ne 'Windows_NT') { throw 'This script runs on Windows only' }
Stop-Process -ErrorAction Ignore -Name SystemSettings
Start-Process ms-settings:defaultapps
$ps = Get-Process -ErrorAction Stop SystemSettings
do {
    Start-Sleep -Milliseconds 100
    $ps.Refresh()
} while ([int] $ps.MainWindowHandle)
Start-Sleep -Milliseconds 200
$shell = New-Object -ComObject WScript.Shell
foreach ($i in 1..4) { $shell.SendKeys('{TAB}'); Start-Sleep -milliseconds 100 }
$shell.SendKeys("chrom"); Start-Sleep -seconds 1
$shell.SendKeys('{TAB}'); Start-Sleep -milliseconds 100
$shell.SendKeys('{ENTER}'); Start-Sleep -milliseconds 100
$shell.SendKeys('{ENTER}'); Start-Sleep -milliseconds 100
$shell.SendKeys('%{F4}')