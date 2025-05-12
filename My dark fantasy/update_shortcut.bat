@echo off
set SCRIPT="%TEMP%\changeShortcut.vbs"
echo Set oWS = WScript.CreateObject("WScript.Shell") > %SCRIPT%
echo sLinkFile = "%USERPROFILE%\Desktop\YEEZUS.lnk" >> %SCRIPT%
echo Set oLink = oWS.CreateShortcut(sLinkFile) >> %SCRIPT%
echo oLink.Description = "YEEZUS" >> %SCRIPT%
echo oLink.IconLocation = "C:/Users/tudor/OneDrive/Imagini/Documente/GitHub/MyDarkFantasy/My dark fantasy/Assets/StreamingAssets\cd.ico,0" >> %SCRIPT%
echo oLink.Save >> %SCRIPT%
cscript /nologo %SCRIPT%
del %SCRIPT%
