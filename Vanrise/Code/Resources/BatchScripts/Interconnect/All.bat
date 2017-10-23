ECHO OFF
ECHO.
ECHO This script will transfer Interconnect website, and plug-ins to its related published destinations
pause
FOR %%i in (C:\TFS\Vanrise\Code\Resources\BatchScripts\Interconnect\*.bat) do IF NOT "%%i" == %0 CALL %%i 
ECHO.	
ECHO End of script
pause
exit