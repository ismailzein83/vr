ECHO OFF
::Make Directory from current date as YYYYMMDD
Set CURRDATE=%TEMP%\CURRDATE.TMP
DATE /T > %CURRDATE%
Set PARSEARG="eol=; tokens=1,2,3,4* delims=/, "
For /F %PARSEARG% %%i in (%CURRDATE%) Do SET YYYYMMDD=%%l%%j%%k

ECHO WHSCDRAnalysis WebSite

xcopy "C:\Publish\CDRAnalysis" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\%YYYYMMDD%\" /EXCLUDE:C:\TFS\Vanrise\Code\Resources\BatchScripts\CDRAnalysis\list-of-excluded-files.txt
xcopy "C:\TFS\Vanrise\Code\Resources\DLLs\Aspose.Cells.lic" /y /v /z /i /Q /R "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\%YYYYMMDD%\Bin\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Web\Client" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\%YYYYMMDD%\Client\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Security.Web\Security" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\%YYYYMMDD%\Client\Modules\Security\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.BusinessProcess.Web\BusinessProcess" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\%YYYYMMDD%\Client\Modules\BusinessProcess\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Common.Web\Common" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\%YYYYMMDD%\Client\Modules\Common\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Runtime.Web\Runtime" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\%YYYYMMDD%\Client\Modules\Runtime\"
xcopy "C:\TFS\CDRAnalysis\Vanrise.Fzero.FraudAnalysis.Web\FraudAnalysis" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\%YYYYMMDD%\Client\Modules\FraudAnalysis\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.Integration.Web\Integration" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\%YYYYMMDD%\Client\Modules\Integration\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.BI.Web\BI" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\%YYYYMMDD%\Client\Modules\BI\"
xcopy "C:\TFS\CDRAnalysis\PSTN.BusinessEntity.Web\PSTN_BusinessEntity" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\%YYYYMMDD%\Client\Modules\PSTN_BusinessEntity\"
xcopy "C:\TFS\BusinessProcess\Code\Vanrise.Queueing.Web\Queueing" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\%YYYYMMDD%\Client\Modules\Queueing\"
xcopy "C:\TFS\Vanrise\Code\Vanrise.GenericData.Web\VR_GenericData" /S /E /R /y /v /i /z /Q "\\192.168.110.185\Fixes\WebSite\CDRAnalysis\%YYYYMMDD%\Client\Modules\VR_GenericData\"	