@echo off
cd "C:\Program Files\Google\Chrome\Application"
chrome.exe --remote-debugging-port=9999 --user-data-dir=C:\Project\UserDirectory
exit-