
CREATE PROCEDURE [dbo].[prFillDailyProfile]
AS


declare @startingId int
 declare @StartingDate datetime
 declare @EndingDate  datetime
 declare @CtrlTableId int
 DECLARE @NumberOfProfileRecords INT;
 DECLARE @NumberOfCalls INT;


 INSERT INTO [dbo].[ControlTable]
           (
            [PeriodId],OperationtypeId,[StartedDateTime]
           )
     VALUES
           (1,5,GETDATE())

set @CtrlTableId=@@IDENTITY
 
 
 select @startingId=LastId,@StartingDate=EndingUnitDate from ControlTable where id=
 (
   select top 1 Id from ControlTable where PeriodId=1  AND OperationtypeId=5  and id <> @CtrlTableId  order by id desc
 )
 if isnull(@startingId,0)=0
 begin
  select @startingId= min(id) from dbo.NormalCdr
 end
 if @StartingDate is null
  begin
   
       SELECT  @StartingDate = CONVERT(datetime,CONVERT(char,min(connectdatetime),106))from dbo.NormalCDR
  end
     SELECT  @EndingDate = CONVERT(datetime,CONVERT(char,MAX(connectdatetime),106))from dbo.NormalCDR
  
    
    
-------------------------- Served Party Profile     ---------------------- 
 
  INSERT INTO NumberProfile (
    SubscriberNumber, FromDate, ToDate, Count_Out_Calls, Diff_Output_Numb, Count_Out_Inter, Count_In_Inter, InCalls_Vs_OutCalls
    , DiffDest_Vs_OutCalls, OutInter_Vs_OutCalls, Call_Out_Dur_Avg, AvrDurIn_Vs_AvrDurOut
    , OutOffNet_Vs_OutOnNet, Count_Out_Fail, Count_In_Fail, Total_Out_Volume, Total_In_Volume
    , Diff_Input_Numbers, Count_Out_SMS, Total_IMEI, Total_BTS, IsOnNet
    , Total_Data_Volume
    , PeriodId
    -- ========
    , Count_In_Calls
    , Call_In_Dur_Avg
    , Count_Out_OnNet
    , Count_In_OnNet
    , Count_Out_OffNet
    , Count_In_OffNet
    -- ==========
    )

 SELECT
    MSISDN SubscriberNumber
    , CONVERT(datetime,CONVERT(char,connectdatetime,106)) FromDate
    , CONVERT(datetime,CONVERT(char,DATEADD(day,1,connectdatetime),106)) ToDate

    ,SUM(CASE WHEN NC.Call_Type = 1 THEN 1 ELSE 0 END) Count_out_Calls
    ,COUNT(DISTINCT (CASE WHEN NC.Call_Type = 1 THEN NC.destination END)) diff_output_numb
    ,SUM(CASE WHEN NC.Call_Type = 1 AND scc.[Description] IS NOT NULL AND
    scc.NetType = 2 THEN 1 ELSE 0 END) Count_Out_Inter
    ,SUM(CASE WHEN NC.Call_Type = 2 AND  scc.[Description] IS NOT NULL AND
    scc.NetType = 2 THEN 1 ELSE 0 END) Count_In_Inter
    ,SUM(CASE WHEN NC.Call_Type = 2 THEN 1 ELSE 0 END) * 1.00 / SUM(CASE WHEN NC.Call_Type = 1 THEN 1 ELSE NULL END) * 1.00 InCalls_Vs_OutCalls
    ,COUNT(DISTINCT (CASE WHEN NC.Call_Type = 1 THEN NC.destination END)) * 1.00 / SUM(CASE WHEN NC.Call_Type = 1 THEN 1 ELSE NULL END) * 1.00 DiffDest_Vs_OutCalls
    ,SUM(CASE WHEN NC.Call_Type = 1 AND scc.[Description] IS NOT NULL AND
    scc.NetType = 2 THEN 1 ELSE 0 END) * 1.00 / SUM(CASE WHEN NC.Call_Type = 1 THEN 1 ELSE NULL END) * 1.00 OutInter_Vs_OutCalls
    ,CASE WHEN AVG(CASE WHEN NC.durationINseconds <> 0 AND NC.Call_Type = 1 THEN NC.durationInseconds / 60 END) IS NULL THEN  0 ELSE NULL END Call_out_Dur_AVG
    ,CASE WHEN AVG(CASE WHEN NC.durationINseconds <> 0 AND NC.Call_Type = 2 THEN NC.durationInseconds / 60 END) IS NULL THEN  0 ELSE NULL END * 1.00 / AVG(CASE WHEN NC.durationINseconds <> 0 AND
    NC.Call_Type = 1 THEN NC.durationInseconds / 60 END) * 1.00 AvrDurIn_Vs_AvrDurOut
    ,SUM(CASE WHEN NC.Call_Type = 1 AND(scc.[Description] IS NULL OR scc.NetType = 0) THEN 1 ELSE 0 END) * 1.00 / SUM(CASE WHEN NC.Call_Type = 1 AND
    scc.[Description] IS NOT NULL AND
    scc.NetType = 1 THEN 1 ELSE NULL END) * 1.00 OutOffNet_Vs_OutOnNet
    , SUM(CASE WHEN NC.durationINseconds = 0 AND
    NC.Call_Type = 1 THEN 1 ELSE 0 END) Count_out_Fail
    ,SUM(CASE WHEN NC.durationINseconds = 0 AND
    NC.Call_Type = 2 THEN 1 ELSE 0 END) Count_in_Fail
    ,SUM(CASE WHEN NC.Call_Type = 1 THEN NC.durationinseconds / 60 ELSE 0 END) total_out_volume
    ,SUM(CASE WHEN NC.Call_Type = 2 THEN NC.durationinseconds / 60 ELSE 0 END) total_in_volume
    ,COUNT(DISTINCT (CASE WHEN NC.Call_Type = 2 THEN NC.destination END)) diff_input_numbers
    ,SUM(CASE WHEN NC.Call_Type = 31 THEN 1 ELSE 0 END) Count_Out_SMS
    ,COUNT(DISTINCT (CASE WHEN NC.Call_Type = 1 OR
    NC.Call_Type = 2 OR  NC.Call_Type = 30 OR
    NC.Call_Type = 31 THEN IMEI END)) total_IMEI
    ,COUNT(DISTINCT (CASE WHEN NC.Call_Type = 1 OR
    NC.Call_Type = 2 OR NC.Call_Type = 30 OR
    NC.Call_Type = 31 THEN BTS_Id END)) Total_BTS
    ,1 IsOnNet
    ,SUM(NC.up_volume + NC.down_volume) Total_Data_Volume
    , 6
    -- ==========
    ,SUM(CASE WHEN NC.Call_Type = 2 THEN 1 ELSE 0 END) Count_in_Calls
    ,CASE WHEN AVG(CASE WHEN NC.durationINseconds <> 0 AND NC.Call_Type = 2 THEN NC.durationInseconds / 60 END) IS NULL THEN  0 ELSE NULL END Call_in_Dur_Avg
    ,SUM(CASE WHEN NC.Call_Type = 1 AND
    scc.[Description] IS NOT NULL AND  scc.NetType = 1 THEN 1 ELSE 0 END) Count_Out_OnNet
    ,SUM(CASE WHEN NC.Call_Type = 2 AND scc.[Description] IS NOT NULL AND
    scc.NetType = 1 THEN 1 ELSE 0 END) Count_In_OnNet
    ,SUM(CASE WHEN NC.Call_Type = 1 AND
    (scc.[Description] IS NULL OR scc.NetType = 0) THEN 1 ELSE 0 END) Count_Out_OffNet
    ,SUM(CASE WHEN NC.Call_Type = 2 AND
    (scc.[Description] IS NULL OR scc.NetType = 0) THEN 1 ELSE 0 END) Count_In_OffNet

  FROM
    NormalCDR NC
    LEFT JOIN Set_CallClass scc
      ON NC.Call_Class = scc.[Description]
  -- =====
  WHERE
    connectDateTime >= @StartingDate AND
    connectDateTime < @EndingDate
  -- =====
  GROUP BY
    MSISDN
    
    , CONVERT(datetime,CONVERT(char,connectdatetime,106))
    , CONVERT(datetime,CONVERT(char,DATEADD(day,1,connectdatetime),106))
   -- ========================   Other Party Profile    ==============================

  INSERT INTO NumberProfile (
    SubscriberNumber, FromDate, ToDate, Count_Out_Calls, Diff_Output_Numb, Count_Out_Inter, Count_In_Inter, InCalls_Vs_OutCalls
    , DiffDest_Vs_OutCalls, OutInter_Vs_OutCalls, Call_Out_Dur_Avg
    , AvrDurIn_Vs_AvrDurOut, OutOffNet_Vs_OutOnNet, Count_Out_Fail, Count_In_Fail, Total_Out_Volume, Total_In_Volume
    , Diff_Input_Numbers, Count_Out_SMS
    , Total_IMEI, Total_BTS, IsOnNet
    , Total_Data_Volume
    , PeriodId

    -- ========
    , Count_In_Calls
    , Call_In_Dur_Avg
    , Count_Out_OnNet
    , Count_In_OnNet
    , Count_Out_OffNet
    , Count_In_OffNet
    -- ==========
    )
  SELECT
    NC.Destination SubscriberNumber
    , CONVERT(datetime,CONVERT(char,connectdatetime,106)) FromDate
    , CONVERT(datetime,CONVERT(char,DATEADD(day,1,connectdatetime),106)) ToDate
    ,SUM(CASE WHEN NC.Call_Type = 2 AND
    (scc.[Description] IS NULL OR scc.NetType = 0) THEN 1 ELSE 0 END) Count_out_Calls
    ,COUNT(DISTINCT (CASE WHEN NC.Call_Type = 2 AND
    (scc.[Description] IS NULL OR scc.NetType = 0) THEN NC.MSISDN END)) diff_output_numb
    ,0 Count_Out_Inter
    ,0 Count_In_Inter
    ,SUM(CASE WHEN NC.Call_Type = 1 AND
    (scc.[Description] IS NULL OR scc.NetType = 0) THEN 1 ELSE 0 END )* 1.00 / SUM(CASE WHEN NC.Call_Type = 2 AND
    (scc.[Description] IS NULL OR scc.NetType = 0) THEN 1 ELSE NULL END)* 1.00 InCalls_Vs_OutCalls
    ,COUNT(DISTINCT (CASE WHEN NC.Call_Type = 2 AND
    (scc.[Description] IS NULL OR scc.NetType = 0) THEN NC.MSISDN END))* 1.00 / SUM(CASE WHEN NC.Call_Type = 2 AND
    (scc.[Description] IS NULL OR scc.NetType = 0) THEN 1 ELSE NULL END)* 1.00 DiffDest_Vs_OutCalls
    ,0 OutInter_Vs_OutCalls
    ,ISNULL(AVG(CASE WHEN NC.durationINseconds <> 0 AND NC.Call_Type = 2 AND(scc.[Description] IS NULL OR scc.NetType = 0) THEN NC.durationInseconds / 60 END), 0) Call_out_Dur_AVG
    ,ISNULL(AVG(CASE WHEN NC.durationINseconds <> 0 AND NC.Call_Type = 1 AND (scc.[Description] IS NULL OR scc.NetType = 0) THEN NC.durationInseconds / 60 END), 0)* 1.00 / AVG(CASE WHEN NC.durationINseconds <> 0 AND
    NC.Call_Type = 2 AND
    (scc.[Description] IS NULL OR scc.NetType = 0) THEN NC.durationInseconds / 60 END)* 1.00 AvrDurIn_Vs_AvrDurOut
    ,0 OutOffNet_Vs_OutOnNet
    ,SUM(CASE WHEN NC.durationINseconds = 0 AND
    NC.Call_Type = 2 AND
    (scc.[Description] IS NULL OR scc.NetType = 0) THEN 1 ELSE 0 END) Count_out_Fail
    ,SUM(CASE WHEN NC.durationINseconds = 0 AND
    NC.Call_Type = 1 AND
    (scc.[Description] IS NULL OR scc.NetType = 0) THEN 1 ELSE 0 END) Count_in_Fail
    ,SUM(CASE WHEN NC.Call_Type = 2 AND
    (scc.[Description] IS NULL OR scc.NetType = 0) THEN NC.durationinseconds / 60 ELSE 0 END) total_out_volume
    ,SUM(CASE WHEN NC.Call_Type = 1 AND
    (scc.[Description] IS NULL OR scc.NetType = 0) THEN NC.durationinseconds / 60 ELSE 0 END) total_in_volume
    ,COUNT(DISTINCT (CASE WHEN NC.Call_Type = 1 AND
    (scc.[Description] IS NULL OR scc.NetType = 0) THEN NC.MSISDN END)) diff_input_numbers
    ,SUM(CASE WHEN NC.Call_Type = 30 AND
    (scc.[Description] IS NULL OR scc.NetType = 0) THEN 1 ELSE 0 END) Count_Out_SMS
    ,0 total_IMEI
    ,0 Total_BTS
    ,0 IsOnNet
    ,0 Total_Data_Volume
    ,6
    -- ==========
    ,
    SUM(CASE WHEN NC.Call_Type = 1 AND
    (scc.[Description] IS NULL OR scc.NetType = 0) THEN 1 ELSE 0 END) Count_in_Calls
    ,ISNULL(AVG(CASE WHEN NC.durationINseconds <> 0 AND NC.Call_Type = 1 AND (scc.[Description] IS NULL OR scc.NetType = 0) THEN NC.durationInseconds / 60 END), 0) Call_in_Dur_Avg
    ,0 Count_Out_OnNet
    ,0 Count_In_OnNet
    ,0 Count_Out_OffNet
    ,0 Count_In_OffNet
  FROM
    NormalCDR NC
    LEFT JOIN Set_CallClass scc
      ON NC.Call_Class = scc.[Description]
  -- =====
  WHERE
    (scc.[Description] IS NULL OR
    scc.NetType <> 1) AND
    connectDateTime >= @StartingDate AND
    connectDateTime < @EndingDate
  -- =====
  GROUP BY
    Destination
    , CONVERT(datetime,CONVERT(char,connectdatetime,106))
    , CONVERT(datetime,CONVERT(char,DATEADD(day,1,connectdatetime),106))
  -- ====================


 -- ====================


  SET @NumberOfProfileRecords = 0;
  
  SELECT @NumberOfCalls = COUNT(*) FROM  NormalCDR
  WHERE 
    connectDateTime BETWEEN @StartingDate AND @EndingDate;
  UPDATE
    ControlTable
  SET
    [FinishedDateTime] = getdate()
    , [StartingUnitDate] = @StartingDate
    , [EndingUnitDate] = @EndingDate
    , [NumberOfProfileRecords] =@NumberOfProfileRecords
    , NumberOfCalls = @NumberOfCalls
  WHERE
    id = @CtrlTableId

-- ====================






/*

ts_prFillDailyProfile


*/