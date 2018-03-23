ECHO OFF
ECHO.
ECHO This script will transfer RA website, plug-ins, and switches to its related published destinations
pause
FOR %%i in (C:\TFS\Vanrise\Code\Resources\BatchScripts\RA\*.bat) do IF NOT "%%i" == %0 CALL %%i 
ECHO.	
ECHO End of script
pause
exit