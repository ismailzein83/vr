CREATE PROCEDURE [dbo].[prGetReportVariables]
AS
BEGIN

DECLARE @date DATETIME
set @date = (select convert(datetime, convert(char, getdate(), 106)))
DECLARE @index INT 
SELECT @index= COUNT(*)+1 FROM Report r WHERE r.ReportDate > =@date
SELECT
      GETDATE() AS ReportDate ,
	  convert(CHAR(4) ,Year(@date))
	 +REPLICATE('0',2-LEN(ltrim(rtrim(convert(CHAR(2),Month(@date))))))	 +ltrim(rtrim(convert(CHAR(2),Month(@date))))
	 +REPLICATE('0',2-LEN(ltrim(rtrim(convert(CHAR(2),day(@date))))))	 +ltrim(rtrim(convert(CHAR(2) ,day(@date)))) 
	 +REPLICATE('0',4-LEN(ltrim(rtrim(convert(CHAR(4) ,@index)))))+ltrim(rtrim(convert(CHAR(4) ,@index)))
	 AS ReportNumber
END

/*

[db_GetReportVariables] 



 */