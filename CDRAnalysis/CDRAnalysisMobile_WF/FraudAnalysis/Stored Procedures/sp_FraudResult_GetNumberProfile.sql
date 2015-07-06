




CREATE PROCEDURE [FraudAnalysis].[sp_FraudResult_GetNumberProfile]
(
	@FromRow int ,
	@ToRow int,
	@FromDate DATETIME,
	@ToDate DATETIME,
	@SubscriberNumber varCHAR(100)
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		;WITH NumberProfile_CTE (FromDate, ToDate, Count_Out_Calls, Diff_Output_Numb, Count_Out_Inter, Count_In_Inter, Call_Out_Dur_Avg, Count_Out_Fail, Count_In_Fail, 
                      Total_Out_Volume, Total_In_Volume, Diff_Input_Numbers, Count_Out_SMS, Total_IMEI, Total_BTS, Total_Data_Volume, Count_In_Calls, Call_In_Dur_Avg, 
                      Count_Out_OnNet, Count_In_OnNet, Count_Out_OffNet, Count_In_OffNet, CountFailConsecutiveCalls, CountConsecutiveCalls, CountInLowDurationCalls,RowNumber) AS 
			(
				
				SELECT     FromDate, ToDate, Count_Out_Calls, Diff_Output_Numb, Count_Out_Inter, Count_In_Inter, Call_Out_Dur_Avg, Count_Out_Fail, Count_In_Fail, 
                      Total_Out_Volume, Total_In_Volume, Diff_Input_Numbers, Count_Out_SMS, Total_IMEI, Total_BTS, Total_Data_Volume, Count_In_Calls, Call_In_Dur_Avg, 
                      Count_Out_OnNet, Count_In_OnNet, Count_Out_OffNet, Count_In_OffNet, CountFailConsecutiveCalls, CountConsecutiveCalls, CountInLowDurationCalls
                      , ROW_NUMBER() OVER ( ORDER BY  FromDate ASC) AS RowNumber 
				FROM         FraudAnalysis.NumberProfile
				where SubscriberNumber=@SubscriberNumber and  FromDate >=   @FromDate and ToDate<=@ToDate
			)
			
		SELECT FromDate, ToDate, Count_Out_Calls, Diff_Output_Numb, Count_Out_Inter, Count_In_Inter, Call_Out_Dur_Avg, Count_Out_Fail, Count_In_Fail, 
                      Total_Out_Volume, Total_In_Volume, Diff_Input_Numbers, Count_Out_SMS, Total_IMEI, Total_BTS, Total_Data_Volume, Count_In_Calls, Call_In_Dur_Avg, 
                      Count_Out_OnNet, Count_In_OnNet, Count_Out_OffNet, Count_In_OffNet, CountFailConsecutiveCalls, CountConsecutiveCalls, CountInLowDurationCalls,RowNumber
		FROM NumberProfile_CTE WHERE RowNumber between @FromRow AND @ToRow  

		SET NOCOUNT OFF
		/*

		exec [FraudAnalysis].[sp_FraudResult_GetNumberProfile]
			@FromRow =1 ,
			@ToRow =1000,
			@FromDate ='2015-03-14 01:59:59.000',
			@ToDate ='2015-03-14 05:59:59.000',
			@SubscriberNumber='202010904977227'
		*/
	END