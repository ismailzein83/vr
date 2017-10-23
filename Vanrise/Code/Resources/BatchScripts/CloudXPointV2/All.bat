ECHO OFF
ECHO.
ECHO This script will transfer CloudXPointV2 Extentions to its related published destinations
pause
FOR %%i in (C:\TFS\Vanrise\Code\Resources\BatchScripts\CloudXPointV2\*.bat) do IF NOT "%%i" == %0 CALL %%i 
ECHO.	
ECHO End of script
pause
exit