

CREATE PROCEDURE [dbo].[FillFactCallsDaily]
	 
	@FromDate datetime,
	@ToDate datetime
	
	 
AS

INSERT INTO [Fact_Calls]

           (
            [FK_PeriodId]
           ,[FK_ConnectTime]
           ,[MS_TotalOutCallls]
           ,[MS_TotalInCalls]
           ,[MS_TotalOutSms]
           ,[MS_TotalInSms]
           ,[MS_TotalInVolume]
           ,[MS_TotalOutVolume]
           ,[MS_TotalOutInternational]
           ,[MS_TotalInInternational]
           ,[MS_PercOnNetOriginated]
           ,[MS_PercOffNetOriginated]
           ,[MS_PercOnNetTerminated]
           ,[MS_PercOffNetTerminated]
           ,[MS_AvgOutCalls_MSISDN]
           ,[MS_AvgInCalls_MSISDN]
           ,[MS_AvgOutSMS_MSISDN]
           ,[MS_AvgInSMS_MSISDN]
           ,[MS_AvgCalledParties_MSISDN]
           ,[MS_AvgBTS_MSISDN]
           ,[MS_AvgDurationIn_MSISDN]
           ,[MS_AvgDurationOut_MSISDN]
           ,[MS_AvgOffNetOriginated_MSISDN]
           ,[MS_AvgOnNetOriginated_MSISDN]
           )

select 

 2
 ,CONVERT(datetime,CONVERT(char,DATEADD(day,1,connectdatetime),106))
 ,SUM(CASE WHEN NC.Call_Type = 1 THEN 1 ELSE 0 END) TotalOutCallls
 ,SUM(CASE WHEN NC.Call_Type = 2 THEN 1 ELSE 0 END) TotalInCalls
 ,SUM(CASE WHEN NC.Call_Type = 31 THEN 1 ELSE 0 END) TotalOutSms
 ,SUM(CASE WHEN NC.Call_Type = 30 THEN 1 ELSE 0 END) TotalInSms
 ,SUM(CASE WHEN NC.Call_Type = 2 THEN NC.durationinseconds / 60 ELSE 0 END) total_in_volume
 ,SUM(CASE WHEN NC.Call_Type = 1 THEN NC.durationinseconds / 60 ELSE 0 END) total_out_volume
 ,SUM(CASE WHEN NC.Call_Type = 1 AND scc.[Description] IS NOT NULL AND  scc.NetType = 2 THEN 1 ELSE 0 END)*1.00 Count_Out_Inter
 ,SUM(CASE WHEN NC.Call_Type = 2 AND  scc.[Description] IS NOT NULL AND scc.NetType = 2 THEN 1 ELSE 0 END)*1.00 Count_In_Inter
 ,(SUM(CASE WHEN NC.Call_Type = 1 AND scc.[Description] IS NOT NULL AND  scc.NetType = 1 THEN 1 ELSE 0 END)*1.0 /SUM(CASE WHEN NC.Call_Type = 1 THEN 1 ELSE 0 END)*1.0)*100 PercOnNetOriginated
 ,(SUM(CASE WHEN NC.Call_Type = 1 AND scc.[Description] IS NOT NULL AND  scc.NetType = 0 THEN 1 ELSE 0 END)*1.0 /SUM(CASE WHEN NC.Call_Type = 1 THEN 1 ELSE 0 END)*1.0)*100 PercOffNetOriginated
 ,(SUM(CASE WHEN NC.Call_Type = 2 AND scc.[Description] IS NOT NULL AND  scc.NetType = 1 THEN 1 ELSE 0 END)*1.0 /SUM(CASE WHEN NC.Call_Type = 2 THEN 1 ELSE 0 END)*1.0)*100 PercOnNetTerminated
 ,(SUM(CASE WHEN NC.Call_Type = 2 AND scc.[Description] IS NOT NULL AND  scc.NetType = 0 THEN 1 ELSE 0 END)*1.0 /SUM(CASE WHEN NC.Call_Type = 2 THEN 1 ELSE 0 END)*1.0)*100 PercOffNetTerminated
 ,(SUM(CASE WHEN NC.Call_Type = 1 THEN 1 ELSE 0 END))/COUNT(distinct MSISDN) AvgOutCalls_MSISDN
 ,(SUM(CASE WHEN NC.Call_Type = 2 THEN 1 ELSE 0 END))/COUNT(distinct MSISDN) AvgInCalls_MSISDN
 ,(SUM(CASE WHEN NC.Call_Type = 31 THEN 1 ELSE 0 END))/COUNT(distinct MSISDN)AvgOutSMS_MSISDN
 ,(SUM(CASE WHEN NC.Call_Type = 30 THEN 1 ELSE 0 END))/COUNT(distinct MSISDN) AvgInSMS_MSISDN
,(COUNT (distinct CASE WHEN NC.Call_Type = 1  THEN NC.destination END))*1.0/COUNT(distinct MSISDN) AvgCalledParties_MSISDN
 ,(COUNT (distinct NC.BTS_Id ))*1.0/COUNT(distinct MSISDN) AvgBTS_MSISDN
 ,SUM(CASE WHEN NC.Call_Type = 2 THEN NC.durationinseconds / 60 ELSE 0 END)/COUNT(distinct MSISDN) AvgDurationIn_MSISDN
 ,SUM(CASE WHEN NC.Call_Type = 1 THEN NC.durationinseconds / 60 ELSE 0 END)/COUNT(distinct MSISDN) AvgDurationOut_MSISDN
 ,(SUM(CASE WHEN NC.Call_Type = 1 AND scc.[Description] IS NOT NULL AND  scc.NetType = 0 THEN 1 ELSE 0 END))*1.0 /COUNT(distinct MSISDN) AvgOffNetOriginated_MSISDN
 ,(SUM(CASE WHEN NC.Call_Type = 1 AND scc.[Description] IS NOT NULL AND  scc.NetType = 1 THEN 1 ELSE 0 END))*1.0/COUNT(distinct MSISDN) AvgOnNetOriginated_MSISDN
 
 
 FROM CDRAnalysis.FraudAnalysis.NormalCDR NC LEFT JOIN [CDRAnalysis].[FraudAnalysis].[CallClass] scc
      ON NC.Call_Class = scc.[Description]
 group by  CONVERT(datetime,CONVERT(char,DATEADD(day,1,connectdatetime),106))

/*

[FillFactCallsDaily] @FromDate ='03/14/2015',@ToDate ='03/15/2015' 
select  * from Fact_Calls where FK_periodid=2

*/