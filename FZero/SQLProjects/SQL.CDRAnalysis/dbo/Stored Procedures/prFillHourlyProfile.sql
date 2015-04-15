

CREATE PROCEDURE [dbo].[prFillHourlyProfile]
AS
BEGIN

 declare @startingId int
 declare @StartingDate datetime
 declare @EndingDate  datetime
 declare @CtrlTableId int
 
 
INSERT INTO [dbo].[ControlTable]
           (
            [PeriodId],OperationtypeId,[StartedDateTime]
           )
     VALUES
           (6,5,GETDATE())

set @CtrlTableId=@@IDENTITY
 
 
 select @startingId=LastId,@StartingDate=EndingUnitDate from ControlTable where id=
 (
   select top 1 Id from ControlTable where PeriodId=6  AND OperationtypeId=5  and id <> @CtrlTableId  order by id desc
 )
 if isnull(@startingId,0)=0
 begin
  select @startingId= min(id) from dbo.NormalCdr
 end
 if @StartingDate is null
  begin
     SELECT  @StartingDate= DATEADD(hour, DATEDIFF(hour, 0, min(connectdatetime)), 0)from dbo.NormalCdr
  end
    SELECT  @EndingDate= DATEADD(hour, DATEDIFF(hour, 0, MAX(connectdatetime)), 0)from dbo.NormalCdr
-------------------------- Served Party Profile     ---------------------- 
   
     insert into dbo.Numberprofile
     select MSISDN SubscriberNumber 
     ,convert(varchar(12), connectDateTime , 102 ) Date_Day  
     ,DATEPART(hour, connectdatetime ) Day_Hour  , 
	 sum( case when nc.Call_Type=1 then  1 else 0 end ) Count_out_Calls  
	,sum( case when nc.Call_Type=2 then  1 else 0 end ) Count_in_Calls 
	,sum( case when nc.durationINseconds  = 0 and nc.Call_Type=1 then 1 else 0 end ) Count_out_Fail
	,sum( case when nc.durationINseconds  = 0 and nc.Call_Type=2 then 1 else 0 end ) Count_in_Fail
	
	,ISNULL(AVG ( case when nc.durationINseconds  <> 0 and nc.Call_Type=1  then nc.durationInseconds/60   end ),0)    Call_out_Dur_AVG 
	,ISNULL(AVG ( case when nc.durationINseconds  <> 0 and nc.Call_Type=2  then nc.durationInseconds/60   end ),0) Call_in_Dur_Avg 
	
	,SUM(case when  nc.Call_Type=1 then nc.durationinseconds/60 else 0 end ) total_out_volume
	,SUM(case when  nc.Call_Type=2 then nc.durationinseconds/60 else 0 end ) total_in_volume
	,COUNT(distinct(case when  nc.Call_Type=1  then  nc.destination  end ))  diff_output_numb 
	,COUNT(distinct(case when  nc.Call_Type=2  then  nc.destination  end ))  diff_input_numbers 
	,COUNT( distinct (case when  nc.Call_Type=1 then nc.Call_Class end) ) diff_dest_net
	,COUNT( distinct (case when  nc.Call_Type=2 then nc.Call_Class end) ) diff_sources_net
	,COUNT(distinct(case when  nc.Call_Type=2  then  nc.BTS_id  end )) Total_BTS_In
	,COUNT(distinct(case when  nc.Call_Type=1  then  nc.BTS_id  end )) Total_BTS_Out
	,sum( case when nc.Call_Type=31 then  1 else 0 end ) Count_Out_SMS   
	,sum( case when nc.Call_Type=30 then  1 else 0 end ) Count_In_SMS   
	,count(distinct (case when  nc.Call_Type=1 or nc.Call_Type=2 OR nc.Call_Type=30 or nc.Call_Type=31 then  IMEI end ))  total_IMEI
    ,sum( case when nc.Call_Type=1 AND scc.[Description] IS NOT NULL  AND scc.NetType=1  then  1 else 0 end ) Count_Out_OnNet
	,sum( case when nc.Call_Type=2 AND scc.[Description] IS NOT null  AND scc.NetType=1 then  1 else 0 end ) Count_In_OnNet
	,sum( case when nc.Call_Type=1 AND(scc.[Description] IS null  or scc.NetType=0) then  1 else 0 end ) Count_Out_OffNet
	,sum( case when nc.Call_Type=2 AND (scc.[Description] IS null  or scc.NetType=0) then  1 else 0 end ) Count_In_OffNet
	,count(distinct (case when  nc.Call_Type=1 or nc.Call_Type=2 OR nc.Call_Type=30 or nc.Call_Type=31 then  BTS_id end ))  Total_BTS

	,SUM(case when nc.Call_Type=1 AND scc.[Description] IS NOT NULL  AND scc.NetType=2  then  1 else 0 end ) Count_Out_Inter
	,SUM(case when nc.Call_Type=2 AND scc.[Description] IS NOT NULL  AND scc.NetType=2  then  1 else 0 end ) Count_In_Inter
	,1 IsOnNet
    ,sum( nc.up_volume + nc.down_volume ) Total_Data_Volume
    
     FROM NormalCDR nc LEFT JOIN Set_CallClass scc ON Nc.Call_Class=scc.[Description]
	--=====
	where connectDateTime >=  @StartingDate and connectDateTime < @EndingDate 
	--=====
	group by MSISDN , convert(varchar(12), connectDateTime , 102 )  ,  DATEPART(hour, connectdatetime )

	-------------------------------------------- Other Party Profile  ----------
    
     insert into dbo.Numberprofile
	 SELECT nc.Destination SubscriberNumber 
	 ,convert(varchar(12), connectDateTime , 102 ) Date_Day  
	 ,DATEPART(hour, connectdatetime ) Day_Hour
	 ,sum( case when nc.Call_Type=2 AND  (scc.[Description] IS NULL  OR  scc.NetType=0) then  1 else 0 end ) Count_out_Calls  
	 ,sum( case when nc.Call_Type=1 AND  (scc.[Description] IS NULL  OR  scc.NetType=0) then  1 else 0 end ) Count_in_Calls  
	 ,sum( case when nc.durationINseconds  = 0 and nc.Call_Type=2 AND  (scc.[Description] IS NULL  OR  scc.NetType=0) then  1 else 0 end ) Count_out_Fail  
	 ,sum( case when nc.durationINseconds  = 0 and nc.Call_Type=1 AND  (scc.[Description] IS NULL  OR  scc.NetType=0) then  1 else 0 end ) Count_in_Fail  
	 ,ISNULL(AVG ( case when nc.durationINseconds  <> 0 and nc.Call_Type=2  AND  (scc.[Description] IS NULL  OR  scc.NetType=0) then nc.durationInseconds/60  end ),0) Call_out_Dur_AVG 
	 ,ISNULL(AVG ( case when nc.durationINseconds  <> 0 and nc.Call_Type=1  AND  (scc.[Description] IS NULL  OR  scc.NetType=0) then nc.durationInseconds/60  end ),0) Call_in_Dur_Avg 
	 ,SUM(case when  nc.Call_Type=2 AND  (scc.[Description] IS NULL  OR  scc.NetType=0) then nc.durationinseconds/60 else 0 end ) total_out_volume
	 ,SUM(case when  nc.Call_Type=1 AND  (scc.[Description] IS NULL  OR  scc.NetType=0) then nc.durationinseconds/60 else 0 end ) total_in_volume
     ,COUNT(distinct(case when  nc.Call_Type=2 AND  (scc.[Description] IS NULL  OR  scc.NetType=0) then  nc.MSISDN  end ))  diff_output_numb 
	 ,COUNT(distinct(case when  nc.Call_Type=1 AND  (scc.[Description] IS NULL  OR  scc.NetType=0) then  nc.MSISDN  end ))  diff_input_numbers 
	 ,COUNT( distinct (case when  nc.Call_Type=2 AND  (scc.[Description] IS NULL  OR  scc.NetType=0)then nc.Call_Class end) ) diff_dest_net
	 ,COUNT( distinct (case when  nc.Call_Type=1 AND  (scc.[Description] IS NULL  OR  scc.NetType=0)then nc.Call_Class end) ) diff_sources_net
	 ,0 Total_BTS_In
	 ,0 Total_BTS_Out
	
	,sum( case when nc.Call_Type=30 AND  (scc.[Description] IS NULL  OR  scc.NetType=0) then  1 else 0 end ) Count_Out_SMS   
	,sum( case when nc.Call_Type=31 AND  (scc.[Description] IS NULL  OR  scc.NetType=0)then  1 else 0 end ) Count_In_SMS   
	,0 total_IMEI
    ,0 Count_Out_OnNet
	,0 Count_In_OnNet
	,0 Count_Out_OffNet
	,0 Count_In_OffNet
	,0 Total_BTS
	,0 Count_Out_Inter
	,0 Count_In_Inter
	,0 IsOnNet
    ,0 Total_Data_Volume
	FROM NormalCDR nc LEFT JOIN Set_CallClass scc ON Nc.Call_Class=scc.[Description]
	--=====
	 WHERE (scc.[Description] IS NULL  OR  scc.NetType <> 1 ) and connectDateTime >=  @StartingDate and connectDateTime < @EndingDate 
	--=====
	group by Destination , convert(varchar(12), connectDateTime , 102 )  ,  DATEPART(hour, connectdatetime )

    --===========
    declare @NumberOfProfileRecords int
    set @NumberOfProfileRecords =0
    
    declare @NumberOfCalls int
    select @NumberOfCalls=count(*) from NormalCdr where connectDateTime between  @StartingDate and @EndingDate 
    
    UPDATE [ControlTable]
       SET 
       [FinishedDateTime] =GETDATE()
      ,[StartingUnitDate] = @StartingDate
      ,[EndingUnitDate] = @EndingDate
      ,[NumberOfProfileRecords]=@NumberOfProfileRecords
      ,NumberOfCalls=@NumberOfCalls
     WHERE id=@CtrlTableId

     --==========
END

/*

 [prFillHourlyProfile]
 --truncate table Numberprofile
 select * from Numberprofile
 
*/