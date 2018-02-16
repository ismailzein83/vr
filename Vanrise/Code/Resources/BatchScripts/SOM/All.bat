ECHO OFF
ECHO.
ECHO This script will transfer SOM website and runtime
pause
FOR %%i in (C:\TFS\Vanrise\Code\Resources\BatchScripts\SOM\*.bat) do IF NOT "%%i" == %0 CALL %%i 
ECHO.	
ECHO End of script
pause
exit