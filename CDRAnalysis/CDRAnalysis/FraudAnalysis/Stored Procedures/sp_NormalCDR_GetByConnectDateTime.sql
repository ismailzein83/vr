



CREATE PROCEDURE [FraudAnalysis].[sp_NormalCDR_GetByConnectDateTime] 
(
	@From datetime ,
	@To datetime
)
AS
BEGIN

SELECT  cdrs.[Id] ,cdrs.[MSISDN] ,cdrs.[IMSI] ,cdrs.[ConnectDateTime] ,cdrs.[Destination] ,
		cdrs.[DurationInSeconds] ,cdrs.[DisconnectDateTime] ,cdrs.[Call_Class]  ,cdrs.[IsOnNet] ,
		cdrs.[Call_Type] ,cdrs.[Sub_Type] ,cdrs.[IMEI]
		,cdrs.[BTS_Id]  ,cdrs.[Cell_Id]  ,cdrs.[SwitchId]  ,cdrs.[Up_Volume]  ,cdrs.[Down_Volume] ,
		cdrs.[Cell_Latitude]  ,cdrs.[Cell_Longitude]  ,cdrs.[In_Trunk]  ,cdrs.[Out_Trunk]  ,cdrs.[Service_Type]  ,cdrs.[Service_VAS_Name] 
		,cdrs.[InTrunkID], cdrs.[OutTrunkID]
                                                
FROM	NormalCDR cdrs with(nolock,index=IX_NormalCDR_MSISDN)
		--LEFT JOIN [FraudAnalysis].AccountCase WhiteNbs with(nolock) ON WhiteNbs.AccountNumber = cdrs.MSISDN AND StatusId=4 and ValidTill >= getdate()
WHERE	cdrs.connectDateTime between @From and @To --and WhiteNbs.AccountNumber IS NULL
ORDER BY cdrs.MSISDN, cdrs.connectdatetime
END