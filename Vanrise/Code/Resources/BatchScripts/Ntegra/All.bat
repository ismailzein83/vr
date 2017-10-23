ECHO OFF
ECHO.
ECHO This script will transfer TOne website, plug-ins, and switches to its related published destinations
pause
FOR %%i in (C:\Publishing_Data\Batch_Scripts\Ntegra\*.bat) do IF NOT "%%i" == %0 CALL %%i 
ECHO.	
ECHO End of script
pause
exit