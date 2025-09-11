@echo off
:menu1
echo.
type SKIN.txt
echo v3.0
echo creator: Adam>:D
echo A wonderful software with which you can hack any network and more :D
echo.
echo (1) Dir command
echo (2) Write all
echo (3) Exit
echo (4) Help
echo (5) Update
echo (6) Try auto hack WiFi (alpha)
set /p input=Choose an option: 
if /i %input%==1 goto 1
if /i %input%==2 goto 2
if /i %input%==3 goto 3
if /i %input%==4 goto 4
if /i %input%==5 goto 5
if /i %input%==6 goto 6
goto menu

:1
color a
netsh wlan show profile

:b
echo.
echo Type ENTER to return to the menu.
set /p input=

if /i "%input%"=="ENTER" goto enter 

echo Invalid input, please try again.
goto b

:2
echo.
echo Profiles saved to available drives.
color 4
setlocal
set "outputFolder=Wi-FiProfiles"
set "profileName=YourProfileName.xml"

for /d %%D in (C:\ D:\ E:\ F:\ G:\ H:\ I:\ J:\ K:\ L:\ M:\ N:\ O:\ P:\ Q:\ R:\ S:\ T:\ U:\ V:\ W:\ X:\ Y:\ Z:\) do (
  if exist "%%D" (
    mkdir "%%D\%outputFolder%"
    start netsh WLAN export profile folder="%%D\%outputFolder%" key=clear
    ren "%%D\%outputFolder%\Wi-Fi-*.xml" "%profileName%"
  )
)

endlocal
echo.

:c
echo.
echo Type ENTER to return to the menu.
set /p input=

if /i "%input%"=="ENTER" goto enter 

echo Invalid input, please try again.
goto c 

:3
echo.
echo Exiting...
exit

:4
echo.
color 04
echo Help Menu:
echo 1 - Show networks
echo 2 - Export WiFi profiles to drives
echo 3 - Exit program

:g
echo.
echo Type ENTER to return to the menu.
set /p input=

if /i "%input%"=="ENTER" goto enter 

echo Invalid input, please try again.
goto g

:5
echo.
color a
dir /s
echo.

:t
echo.
echo Type ENTER to return to the menu.
set /p input=

if /i "%input%"=="ENTER" goto enter 

echo Invalid input, please try again.
goto t

:6
echo.
echo What should it be?
echo (1) Make a custom passlist
echo (2) Attack Wifi
set /p input=Choose an option:
if /i %input%==1 goto 9
if /i %input%==2 goto 8

:8
cd ice1
start bruteforce.cmd
goto menu


:9
cd ice1
start keycombinator.cmd
goto menu

:enter
goto menu


