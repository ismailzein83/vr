





CREATE PROCEDURE [dbo].[delete_prFillHourlySubscriberValues]

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
  select @startingId= min(np.id) from dbo.Numberprofile np WHERE np.Day_Hour BETWEEN 0 AND 23
 end
 if @StartingDate is null
  begin
     SELECT  @StartingDate= DATEADD(hour, DATEDIFF(hour, 0, min(DATEADD(hour,day_hour,date_day))), 0)from dbo.Numberprofile WHERE Day_Hour BETWEEN 0 AND 23
  end
     SELECT  @EndingDate= DATEADD(hour, DATEDIFF(hour, 0, MAX(DATEADD(hour,day_hour +1,date_day))), 0)from dbo.Numberprofile  WHERE Day_Hour BETWEEN 0 AND 23

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

			set @RecFromDate=@StartingDate
			set @RecToDate =@StartingDate
				while @RecToDate < @EndingDate
				begin
						 set @RecToDate=DATEADD( hour ,1,@RecToDate)
						 if @RecToDate <=@EndingDate
						 insert into #Cr1Periods select @RecFromDate,@RecToDate,@CriteriaId
						 --set @RecToDate=DATEADD(hour,1,@RecToDate) 
						 set @RecFromDate=@RecToDate
				 end

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
   from  dbo.Numberprofile pr , #Cr1Periods cp
    where  cp.CriteriaId=1 
     and pr.Date_Day>=

     (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
      AND pr.Date_Day<
     (select CONVERT(datetime,CONVERT(char,DATEADD(DAY,1,cp.DateTo),106)))
     and pr.Day_Hour = DATEPART(hour,cp.Datefrom)
    group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId
   having sum(case when total_out_volume=0 then 0 else count_out_calls end ) >0

----======== filling criteria 2 ( Count of distinct destinations )
   
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
   from  dbo.Numberprofile pr , #Cr1Periods cp
    where  cp.CriteriaId=2 
     and pr.Date_Day>=
     (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
      AND pr.Date_Day<
     (select CONVERT(datetime,CONVERT(char, DATEADD(DAY,1,cp.DateTo),106)))
     and pr.Day_Hour =DATEPART(hour,cp.Datefrom)
   -- group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId,diff_output_numb
 
 ------------------------------filling criteria 3  (Count of outgoing calls)
   
  INSERT INTO [Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[Value]
           ,PeriodId
           )
   select pr.subscriberNumber  , cp.Datefrom,cp.DateTo,cp.CriteriaId
   ,count_out_calls
   ,6
   from  dbo.Numberprofile pr , #Cr1Periods cp
   where  cp.CriteriaId=3
     and pr.Date_Day>=
     (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
      AND pr.Date_Day<
     (select CONVERT(datetime,CONVERT(char, DATEADD(DAY,1,cp.DateTo),106)))
     and pr.Day_Hour = DATEPART(hour,cp.Datefrom)
   --group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId

 

------------------------------filling criteria 4 (Low or no mobility: Number of cell per SIM/ IMEI)
   
  INSERT INTO [Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[Value]
           ,PeriodId
           )
   select pr.subscriberNumber  , cp.Datefrom,cp.DateTo,cp.CriteriaId
   ,case when pr.total_cell IS NULL THEN 0 ELSE total_cell end
   ,6
   from  dbo.Numberprofile pr , #Cr1Periods cp
    where  cp.CriteriaId=4 
     and pr.Date_Day>=
 
     (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
      AND pr.Date_Day<
     (select CONVERT(datetime,CONVERT(char, DATEADD(DAY,1,cp.DateTo),106)))
     and pr.Day_Hour = DATEPART(hour,cp.Datefrom)
   --group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId,total_cell

------------------------------filling criteria 5 (Total Originated Volume per SIM)
  INSERT INTO [Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[Value]
           ,PeriodId
           )
   select pr.subscriberNumber  , cp.Datefrom,cp.DateTo,cp.CriteriaId 
   ,pr.total_out_volume
   ,6
   from  dbo.Numberprofile pr , #Cr1Periods cp
    where  cp.CriteriaId=5
     and pr.Date_Day>=
 
     (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
      AND pr.Date_Day<
     (select CONVERT(datetime,CONVERT(char, DATEADD(DAY,1,cp.DateTo),106)))
     and pr.Day_Hour = DATEPART(hour,cp.Datefrom)
  --group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId,pr.total_out_volume



------------------------------filling criteria 6 (count of total IMEI per MSISDN (IMSI ))

  INSERT INTO [Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[Value]
           ,PeriodId
           )
   select pr.subscriberNumber  , cp.Datefrom,cp.DateTo,cp.CriteriaId 
   ,pr.total_imei
   ,6
   from  dbo.Numberprofile pr , #Cr1Periods cp
    where  cp.CriteriaId=6 
     and pr.Date_Day>=
 
     (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
      AND pr.Date_Day<
     (select CONVERT(datetime,CONVERT(char, DATEADD(DAY,1,cp.DateTo),106)))
     and pr.Day_Hour = DATEPART(hour,cp.Datefrom)
 --group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId,pr.total_out_volume
 
 
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
   from  dbo.Numberprofile pr , #Cr1Periods cp
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
   from  dbo.Numberprofile pr , #Cr1Periods cp
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
   from  dbo.Numberprofile pr , #Cr1Periods cp
   where  cp.CriteriaId=10 
   and pr.Date_Day>=
   (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
   AND pr.Date_Day<
   (select CONVERT(datetime,CONVERT(char, DATEADD(DAY,1,cp.DateTo),106)))
   and pr.Day_Hour =DATEPART(hour,cp.Datefrom) AND pr.Day_Hour IN(23,0,1,2,3,4,5)
-- group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId,diff_output_numb
  
   -----======== filling criteria 11 ( Voice – only service usage (no sent SMSs )
   INSERT INTO [Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[Value]
           ,PeriodId
           )
   select pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId--,@StrategyId
   ,case when pr.Count_Out_SMS IS NULL THEN 0 ELSE pr.Count_Out_SMS end
   ,6
   from  dbo.Numberprofile pr , #Cr1Periods cp
   where  cp.CriteriaId=11
   and pr.Date_Day>=
   (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
   AND pr.Date_Day<
   (select CONVERT(datetime,CONVERT(char,DATEADD(DAY,1,cp.DateTo),106)))
    and pr.Day_Hour = DATEPART(hour,cp.Datefrom)
    --group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId,count_orig_SMS

    ---======== filling criteria 12 (Ratio of distinct destination vs total numbers of calls )
    
   INSERT INTO [Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[Value]
           ,PeriodId
           )
   select pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId--,@StrategyId
   ,(Diff_Output_Numb/count_out_calls)
   ,6
   from  dbo.Numberprofile pr , #Cr1Periods cp
   where  cp.CriteriaId=12 AND count_out_calls>0
   and pr.Date_Day>=
   (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
   AND pr.Date_Day<
   (select CONVERT(datetime,CONVERT(char,DATEADD(DAY,1,cp.DateTo),106)))
   and pr.Day_Hour = DATEPART(hour,cp.Datefrom)
    --group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId

    ---======== filling criteria 13 (Ratio International Originated Vs outgoing calls)

   INSERT INTO [Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[Value]
           ,PeriodId
           )

   select pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId--,@StrategyId
   ,count_out_inter/count_out_calls
   
   ,6
   from  dbo.Numberprofile pr , #Cr1Periods cp
    where count_out_calls>0 and cp.CriteriaId=13 
    and pr.Date_Day >= 
     (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
    AND pr.Date_Day< (select CONVERT(datetime,CONVERT(char,DATEADD(DAY,1,cp.DateTo),106)))
    and pr.Day_Hour = DATEPART(hour,cp.Datefrom)
  

   ---======== filling criteria 14 (Count of outgoing calls during peak hours)
   --- Not available in hourly values
  
   ---======== filling criteria 15 (Data Usage)

   
   INSERT INTO [Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[Value]
           ,PeriodId
           )

   select pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId--,@StrategyId
   ,total_Data_volume
   ,6
   from  dbo.Numberprofile pr , #Cr1Periods cp
    where cp.CriteriaId=15 
    and pr.Date_Day >= 
    (select CONVERT(datetime,CONVERT(char, cp.Datefrom ,106)))
    AND pr.Date_Day< (select CONVERT(datetime,CONVERT(char,DATEADD(DAY,1,cp.DateTo),106)))
    and pr.Day_Hour = DATEPART(hour,cp.Datefrom)
 
  
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
 --  from  dbo.Numberprofile pr , #Cr1Periods cp
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