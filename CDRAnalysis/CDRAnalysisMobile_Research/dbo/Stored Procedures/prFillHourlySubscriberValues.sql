CREATE PROCEDURE [dbo].[prFillHourlySubscriberValues]

AS


 declare @startingId int
 declare @StartingDate datetime
 declare @EndingDate  datetime
 declare @CtrlTableId INT
 
 
INSERT INTO [ControlTable]
           (
            PeriodId,OperationtypeId,[StartedDateTime]
           )
     VALUES
           (6,6,GETDATE())

set @CtrlTableId=@@IDENTITY
 
 
 select @startingId=LastId,@StartingDate=EndingUnitDate from ControlTable where id=
 (
   select top 1 Id from ControlTable where PeriodId=6 AND OperationtypeId=6 and id <> @CtrlTableId  order by id desc
 )
 if isnull(@startingId,0)=0
 begin
  select @startingId= min(np.id) from dbo.Ol_NumberProfile np WHERE np.Day_Hour BETWEEN 0 AND 23
 end
 if @StartingDate is null
  begin
     SELECT  @StartingDate= DATEADD(hour, DATEDIFF(hour, 0, min(DATEADD(hour,day_hour,date_day))), 0)from dbo.Ol_NumberProfile WHERE Day_Hour BETWEEN 0 AND 23
  end
     SELECT  @EndingDate= DATEADD(hour, DATEDIFF(hour, 0, MAX(DATEADD(hour,day_hour +1,date_day))), 0)from dbo.Ol_NumberProfile  WHERE Day_Hour BETWEEN 0 AND 23

--======== criteria periods --------

declare @Cr1PrdId int
declare @Cr1PrdValue decimal
create table #Cr1Periods(Datefrom datetime , DateTo Datetime ,CriteriaId int )
declare @RecFromDate datetime
declare @RecToDate datetime

declare @CriteriaId int
set @CriteriaId =1
while @CriteriaId <=15
  begin 
  	
    	insert into #Cr1Periods 
  	    SELECT DISTINCT  DATEADD(hour ,nb.Day_Hour,nb.Date_Day)
  	    ,DATEADD(hour,nb.Day_Hour+1,nb.Date_Day),@CriteriaId
  	   
  	    FROM Ol_NumberProfile nb WHERE nb.Day_Hour BETWEEN 0 AND 23
  	     AND nb.Date_Day >= CONVERT(datetime,CONVERT(char,@StartingDate,106))       
  	     AND nb.Date_Day<=  CONVERT(datetime,CONVERT(char,@EndingDate,106)) 
  			
     set @CriteriaId=@CriteriaId+1
   end

--======== filling criteria 1 ( Ratio incoming Vs Outgoing )

   INSERT INTO [Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[Value]
           ,PeriodId
           )
  select pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId--,@StrategyId
   ,sum(case when total_in_volume=0 then 0 else count_in_calls end)*1.00/sum(case when total_out_volume=0 then 0 else count_out_calls end)*1.00
   ,6
   from  dbo.Ol_NumberProfile pr , #Cr1Periods cp
   where  cp.CriteriaId=1 
   and pr.Date_Day>=
   (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
   AND pr.Date_Day<
   (select CONVERT(datetime,CONVERT(char,DATEADD(DAY,1,cp.DateTo),106)))
   and pr.Day_Hour = DATEPART(hour,cp.Datefrom)
   group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId
   having sum(case when total_out_volume=0 then 0 else count_out_calls end ) >0
----======== filling criteria (2,3,4,5,6,11,15)
  INSERT INTO [Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[Value]
           ,PeriodId
           )
  select pr.subscriberNumber  , cp.Datefrom,cp.DateTo,cp.CriteriaId
 ,CASE WHEN cp.CriteriaId=2  THEN Diff_Output_Numb -- criteria 2 ( Count of distinct destinations )
       WHEN cp.CriteriaId=3 THEN count_out_calls -- criteria 3  (Count of outgoing calls)
       WHEN cp.CriteriaId=4 THEN case when pr.total_BTS IS NULL THEN 0 ELSE total_BTS end -- criteria 4 (Low or no mobility: Number of cell per SIM/ IMEI)
       WHEN cp.CriteriaId=5 THEN pr.total_out_volume -- criteria 5 (Total Originated Volume per SIM)
       WHEN cp.CriteriaId=6 THEN pr.total_imei -- criteria 6 (count of total IMEI per MSISDN (IMSI ))
       WHEN cp.CriteriaId=11 THEN  case when pr.Count_Out_SMS IS NULL THEN 0 ELSE pr.Count_Out_SMS end   -- criteria 11 ( Voice – only service usage (no sent SMSs )
       WHEN cp.CriteriaId=15 THEN total_Data_volume -- criteria 15 (Data Usage)
 
  END
  ,6
  from  dbo.Ol_NumberProfile pr , #Cr1Periods cp
  WHERE cp.CriteriaId IN(2,3,4,5,6,11,15)
  and pr.Date_Day>=
   (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
   AND pr.Date_Day<
   (select CONVERT(datetime,CONVERT(char,DATEADD(DAY,1,cp.DateTo),106)))
   and pr.Day_Hour = DATEPART(hour,cp.Datefrom)

 
 --======== filling criteria 7  (Ratio Average incoming duration vs Average outgoing duration )

   INSERT INTO [Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[Value] 
           ,PeriodId
           )

 select pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId--,@StrategyId
   ,(call_in_dur_avg)*1.00/(call_out_dur_avg)*1.00
   ,6
   from  dbo.Ol_NumberProfile pr , #Cr1Periods cp
   WHERE  call_out_dur_avg>0 and cp.CriteriaId=7 
   and pr.Date_Day>=
   (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
   AND pr.Date_Day<
   (select CONVERT(datetime,CONVERT(char,DATEADD(DAY,1,cp.DateTo),106)))
   and pr.Day_Hour = DATEPART(hour,cp.Datefrom)
   
   --group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId,Call_Out_Dur_Avg,Call_In_Dur_Avg
   --having call_out_dur_avg >0

 --======== filling criteria 8 ( Ratio Off-net originated calls on Vs On-net originated vs )

   INSERT INTO [Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[Value]
           ,PeriodId
           )

 select pr.SubscriberNumber ,cp.Datefrom,cp.DateTo,cp.CriteriaId--,@StrategyId
   ,pr.Count_Out_OffNet/pr.Count_Out_OnNet
   ,6
   from  dbo.Ol_NumberProfile pr , #Cr1Periods cp
   WHERE pr.Count_Out_OnNet >0 and cp.CriteriaId=8 
     and pr.Date_Day>=
     (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
      AND pr.Date_Day<
     (select CONVERT(datetime,CONVERT(char,DATEADD(DAY,1,cp.DateTo),106)))
     and pr.Day_Hour = DATEPART(hour,cp.Datefrom)
   --group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId,count_on_net,count_off_net
   --having count_off_net >0
   
 --======== filling criteria 9 ( Count of daily active hours)
 --- Not available in hourly values
   
 ---======== filling criteria 10  ( Distinct destination of night calls )
 
 INSERT INTO [Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[Value]
           ,PeriodId
           )
   select pr.subscriberNumber  , cp.Datefrom,cp.DateTo,cp.CriteriaId
   --,case when Diff_Output_Numb=0 then 0 else Diff_Output_Numb END
   , Diff_Output_Numb
   ,6
   from  dbo.Ol_NumberProfile pr , #Cr1Periods cp
   where  cp.CriteriaId=10 
   and pr.Date_Day>=
   (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
   AND pr.Date_Day<
   (select CONVERT(datetime,CONVERT(char, DATEADD(DAY,1,cp.DateTo),106)))
   and pr.Day_Hour =DATEPART(hour,cp.Datefrom) AND pr.Day_Hour IN(23,0,1,2,3,4,5)
-- group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId,diff_output_numb
  
  ---======== filling criteria 12 and 13 
   
   INSERT INTO [Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[Value]
           ,PeriodId
           )
  select pr.subscriberNumber  , cp.Datefrom,cp.DateTo,cp.CriteriaId
 ,CASE WHEN cp.CriteriaId=12  THEN (Diff_Output_Numb/count_out_calls)-- criteria 12 (Ratio of distinct destination vs total numbers of calls )
       WHEN cp.CriteriaId=13 THEN count_out_inter/count_out_calls --criteria 13 (Ratio International Originated Vs outgoing calls)
 
  END
  ,6
  from  dbo.Ol_NumberProfile pr , #Cr1Periods cp
  WHERE cp.CriteriaId IN(12,13)  AND count_out_calls>0
  and pr.Date_Day>=
   (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
   AND pr.Date_Day<
   (select CONVERT(datetime,CONVERT(char,DATEADD(DAY,1,cp.DateTo),106)))
   and pr.Day_Hour = DATEPART(hour,cp.Datefrom)
  
  

   ---======== filling criteria 14 (Count of outgoing calls during peak hours)
   --- Not available in hourly values
  
  --------------------------------filling criteria  (bts per SIM)--------
   
 -- INSERT INTO [Subscriber_Values]
 --          ([SubscriberNumber]
 --          ,FromDate
 --          ,[ToDate]
 --          ,[CriteriaId]
 --          ,[Value]
 --          ,PeriodId
 --          )
 --  select pr.subscriberNumber  , cp.Datefrom,cp.DateTo,cp.CriteriaId
 --  ,case when pr.total_BTS IS NULL THEN 0 ELSE total_BTS end
 --  ,6
 --  from  dbo.Ol_NumberProfile pr , #Cr1Periods cp
 --  where  cp.CriteriaId=4 
 --    and pr.Date_Day>=
 --    (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
 --     AND pr.Date_Day<
 --    (select CONVERT(datetime,CONVERT(char, DATEADD(DAY,1,cp.DateTo),106)))
 --    and pr.Day_Hour = DATEPART(hour,cp.Datefrom) 
  
  
  
   --======================================= 
    UPDATE [ControlTable]
       SET 
       [FinishedDateTime] =GETDATE()
      ,[StartingUnitDate] = @StartingDate
      ,[EndingUnitDate] = @EndingDate
      --,[NumberOfProfileRecords]=@NumberOfProfileRecords
      --,NumberOfCalls=@NumberOfCalls
     WHERE id=@CtrlTableId




/*

[prFillHourlySubscriberValues]


---delete from ControlTable where operationtypeid=6

SELECT * FROM ControlTable ct where operationtypeid=1
select * from Subscriber_Values

truncate table Subscriber_Values

select * from [Subscriber_Values] --where  criteriaid=6 --and SubscriberNumber='015225268'
 order by FromDate asc
 
 
 
 
*/