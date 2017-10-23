ECHO OFF
ECHO.
ECHO This script will transfer Zebra API, plug-ins, and OCS to its related published destinations
pause
FOR %%i in (C:\Publishing_Data\Batch_Scripts\Zebra\*.bat) do IF NOT "%%i" == %0 CALL %%i 
ECHO.	
ECHO End of script
pause
exit