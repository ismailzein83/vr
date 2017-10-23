ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO ClearVoice DataBase First Deployment Scripts

xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Security.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\ClearVoice\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Common.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\ClearVoice\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Runtime.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\ClearVoice\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
::xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\BI.PostDeployemnt.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\ClearVoice\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\ClearVoice.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\ClearVoice\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\QualityMeasurement\Code\QualityMeasurement\Script.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\ClearVoice\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Configuration.txt" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\ClearVoice\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Configuration\"
xcopy "C:\TFS\TOneV2\Code\TOneV2\SQL.TOneLogging\Script.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\ClearVoice\%YYYYMMDD%\DataBaseFirstDeploymentScripts\Logging\"

::xcopy "C:\TFS\QualityMeasurement\Code\QualityMeasurementBI\ClearVoiceBI.PostDeployment.sql" /y /v /z /i /Q /R  "\\192.168.110.185\Fixes\WebSite\ClearVoice\%YYYYMMDD%\DataBaseFirstDeploymentScripts\BI\"


::if not exist "\\192.168.110.185\Fixes\WebSite\ClearVoice\%YYYYMMDD%\DataBaseFirstDeploymentScripts\DBsStructure\" mkdir "\\192.168.110.185\Fixes\WebSite\ClearVoice\%YYYYMMDD%\DataBaseFirstDeploymentScripts\DBsStructure\"