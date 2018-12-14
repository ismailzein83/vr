ECHO OFF
ECHO.
ECHO This script will run Helpdesk tasks
pause
FOR %%i in (C:\TFS\Vanrise\Code\Resources\BatchScripts\Helpdesk\*.bat) do IF NOT "%%i" == %0 CALL %%i 
ECHO.	
ECHO End of script
pause
exit