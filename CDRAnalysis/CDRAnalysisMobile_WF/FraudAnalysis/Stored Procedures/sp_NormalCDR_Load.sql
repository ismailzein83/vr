



CREATE PROCEDURE [FraudAnalysis].[sp_NormalCDR_Load] 
(
	@From datetime ,
	@To datetime
)
AS
BEGIN
SELECT  [Id] ,[MSISDN] ,[IMSI] ,[ConnectDateTime] ,[Destination] ,[DurationInSeconds] ,[DisconnectDateTime] ,[Call_Class]  ,[IsOnNet] ,[Call_Type] ,[Sub_Type] ,[IMEI]
                                                ,[BTS_Id]  ,[Cell_Id]  ,[SwitchRecordId]  ,[Up_Volume]  ,[Down_Volume] ,[Cell_Latitude]  ,[Cell_Longitude]  ,[In_Trunk]  ,[Out_Trunk]  ,[Service_Type]  ,[Service_VAS_Name] FROM NormalCDR
                                                 with(nolock)    where (connectDateTime between @From and @To)  order by MSISDN, connectdatetime
END