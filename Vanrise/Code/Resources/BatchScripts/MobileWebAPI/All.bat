ECHO OFF
ECHO.
ECHO This script will transfer MobileWebAPI website to its related published destinations
pause
FOR %%i in (C:\TFS\Vanrise\Code\Resources\BatchScripts\MobileWebAPI\*.bat) do IF NOT "%%i" == %0 CALL %%i 
ECHO.	
ECHO End of script
pause
exit