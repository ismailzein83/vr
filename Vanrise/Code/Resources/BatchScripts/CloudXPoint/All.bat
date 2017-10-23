ECHO OFF
ECHO.
ECHO This script will transfer CloudXPoint website, plug-ins, and switches to its related published destinations
pause
FOR %%i in (C:\Publishing_Data\Batch_Scripts\CloudXPoint\*.bat) do IF NOT "%%i" == %0 CALL %%i 
ECHO.	
ECHO End of script
pause
exit