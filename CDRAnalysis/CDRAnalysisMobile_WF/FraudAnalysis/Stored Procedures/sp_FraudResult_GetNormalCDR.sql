




CREATE PROCEDURE [FraudAnalysis].[sp_FraudResult_GetNormalCDR]
(
	@FromRow int ,
	@ToRow int,
	@FromDate DATETIME,
	@ToDate DATETIME,
	@MSISDN varCHAR(100)
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		;WITH NormalCDR_CTE (IMSI, ConnectDateTime, Destination, DurationInSeconds, Call_Class, Call_Type , Sub_Type, IMEI, Cell_Id, Up_Volume, Down_Volume, Service_Type, Service_VAS_Name,RowNumber) AS 
			(
				SELECT     IMSI, ConnectDateTime, Destination, DurationInSeconds, Call_Class, Call_Type , Sub_Type, IMEI, Cell_Id, Up_Volume, Down_Volume, Service_Type, Service_VAS_Name, ROW_NUMBER() OVER ( ORDER BY  ConnectDateTime ASC) AS RowNumber 
				FROM         FraudAnalysis.NormalCDR  as cdr
				where MSISDN=@MSISDN and  ConnectDateTime between   @FromDate and @ToDate
			)
			
		SELECT IMSI, ConnectDateTime, Destination, DurationInSeconds, Call_Class,  Call_Type , Sub_Type, IMEI, Cell_Id, Up_Volume, Down_Volume, Service_Type, Service_VAS_Name,RowNumber
		FROM NormalCDR_CTE WHERE RowNumber between @FromRow AND @ToRow  

		SET NOCOUNT OFF
		/*

		exec [FraudAnalysis].[sp_FraudResult_GetNormalCDR]
			@FromRow =1 ,
			@ToRow =1000,
			@FromDate ='2015-03-14 03:10:05.000',
			@ToDate ='2015-03-14 03:10:15.000',
			@MSISDN='202010904977227'
		*/
	END