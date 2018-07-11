ECHO OFF
ECHO.
ECHO This script will transfer Ntegra V2 themes
pause
FOR %%i in (C:\TFS\Vanrise\Code\Resources\BatchScripts\ZebraV2\*.bat) do IF NOT "%%i" == %0 CALL %%i 
ECHO.	
ECHO End of script
pause
exit