# rundll32-killer

This program kills all rundll32 processes every hour. In my case, Windows 10 creates about 1.5k rundll processes per my normal workday (8h), and after that the only way to work is to restart the PC.
To make it work:
-Build project
-Go to Windows Task Scheduler
-Create new task with full admin rights
-Add trigger "on PC boot" or something like that
-Add action - execute RundllKiller.exe file(full path), generated with build
