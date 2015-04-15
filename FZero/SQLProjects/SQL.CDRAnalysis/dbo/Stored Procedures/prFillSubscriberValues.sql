

CREATE PROCEDURE [dbo].[prFillSubscriberValues]
(
@FromDate datetime,
@ToDate datetime,
@StrategyId int
)
AS

--======== criteria periods --------

delete from [CDRAnalysis].[dbo].[Subscriber_Values]
where StrategyId = @StrategyId and (FromDate >=@FromDate or ToDate <=@ToDate)

declare @Cr1PrdId int
declare @Cr1PrdValue decimal
create table #Cr1Periods(Datefrom datetime , DateTo Datetime ,CriteriaId int )
declare @RecFromDate datetime
declare @RecToDate datetime

declare @CriteriaId int
set @CriteriaId =1
while @CriteriaId <=6
  begin 


			select @Cr1PrdId=stp.PeriodId ,@Cr1PrdValue=stp.Value from Strategy st 
			 inner join StrategyPeriods stp on st.Id=stp.StrategyId where stp.criteriaID=@CriteriaId and st.Id=@StrategyId
			set @RecFromDate=@FromDate
			set @RecToDate =@FromDate

				while @RecToDate <=@ToDate
				begin
						if @Cr1PrdId=1
						 set @RecToDate=DATEADD( DAY ,@Cr1PrdValue-1,@RecToDate)
						if @Cr1PrdId=2
						 set @RecToDate=DATEADD( week  ,@Cr1PrdValue,@RecToDate)
						if @Cr1PrdId=3
						 set @RecToDate=DATEADD( MONTH  ,@Cr1PrdValue,@RecToDate)
						if @Cr1PrdId=4
						 set @RecToDate=DATEADD( QUARTER ,@Cr1PrdValue,@RecToDate)
						 if @Cr1PrdId=5
						 set @RecToDate=DATEADD( YEAR ,@Cr1PrdValue,@RecToDate)
						 
						 if @RecToDate <=@ToDate
						  insert into #Cr1Periods select @RecFromDate,@RecToDate,@CriteriaId
						
						 set @RecToDate=DATEADD(DAY,1,@RecToDate) 
						 						 
						 set @RecFromDate=@RecToDate
				end

     set @CriteriaId=@CriteriaId+1
   end

--========

--======== filling criteria 1 data
   
   INSERT INTO [CDRAnalysis].[dbo].[Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[StrategyId]
           ,[Value])
   select pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId,@StrategyId
   ,sum(case when total_in_volume=-1 then 0 else count_in end)*1.00/sum(case when total_out_volume=-1 then 0 else count_out end )*1.00
   from  dbo.Numberprofile pr , #Cr1Periods cp
    where  cp.CriteriaId=1 
     and pr.Date_Day between cp.Datefrom and cp.DateTo and pr.Day_Hour =25
    group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId
   having sum(case when total_out_volume=-1 then 0 else total_out_volume end ) >0
 

--======== filling criteria 2 data
   declare @minValue int
   select @minValue= Min_count_value from dbo.Strategy_Min_Values where strategyid =@StrategyId and criteriaid=2
   INSERT INTO [CDRAnalysis].[dbo].[Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[StrategyId]
           ,[Value])
   select pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,cp.CriteriaId,@StrategyId
   ,COUNT (pr.Day_Hour )
   
   from  dbo.Numberprofile pr , #Cr1Periods cp
    where  cp.CriteriaId=2 
     --and pr.Date_Day between cp.Datefrom and cp.DateTo and pr.Day_Hour between 1 and 24 and Count_out > isnull(@minValue,0) and Count_Out<>-1
       and pr.Date_Day >= cp.Datefrom and pr.Date_Day< cp.DateTo and pr.Day_Hour between 1 and 23 and Count_out > isnull(@minValue,0) and Count_Out<>-1
   group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId
   
 ----======== filling criteria 3 data

   INSERT INTO [CDRAnalysis].[dbo].[Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[StrategyId]
           ,[Value])
   select nr.a_temp as subscriberNumber  , cp.Datefrom,cp.DateTo,cp.CriteriaId,@StrategyId
   ,count(distinct(left(nr.b_temp,4)))
   from  dbo.normalcdr nr , #Cr1Periods cp
    where  cp.CriteriaId=3 
     and nr.connectdatetime between cp.Datefrom and cp.DateTo  --and pr.Day_Hour =25 and pr.diff_dest_codes <>-1
   group by nr.a_temp   , cp.Datefrom,cp.DateTo,CriteriaId
   


 ----======== filling criteria 4 data

   INSERT INTO [CDRAnalysis].[dbo].[Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[StrategyId]
           ,[Value])
   select nr.a_temp as subscriberNumber  , cp.Datefrom,cp.DateTo,cp.CriteriaId,@StrategyId
   ,count(distinct nr.b_temp)
   from  dbo.normalcdr nr , #Cr1Periods cp
    where  cp.CriteriaId=4 
     and nr.connectdatetime between cp.Datefrom and cp.DateTo  --and pr.Day_Hour =25 and pr.diff_dest_codes <>-1
   group by nr.a_temp   , cp.Datefrom,cp.DateTo,CriteriaId
   
   
   
--======== filling criteria 5 data
   
   INSERT INTO [CDRAnalysis].[dbo].[Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[StrategyId]
           ,[Value])
   select pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId,@StrategyId
   ,sum(case when total_out_volume=-1 then 0 else total_out_volume end)
   from  dbo.Numberprofile pr , #Cr1Periods cp
    where  cp.CriteriaId=5 
     and pr.Date_Day between cp.Datefrom and cp.DateTo and pr.Day_Hour =25
    group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId
  

--select * from #Cr1Periods

--======== filling criteria 6 data
   --declare @minValue int
   --select @minValue= Min_count_value from dbo.Strategy_Min_Values where strategyid =@StrategyId and criteriaid=2
   INSERT INTO [CDRAnalysis].[dbo].[Subscriber_Values]
           ([SubscriberNumber]
           ,FromDate
           ,[ToDate]
           ,[CriteriaId]
           ,[StrategyId]
           ,[Value])
   select pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,cp.CriteriaId,@StrategyId
   ,sum(case when Count_out=-1 then 0 else Count_out end )
   from  dbo.Numberprofile pr , #Cr1Periods cp
    where  cp.CriteriaId=6
    -- and pr.Date_Day between cp.Datefrom and cp.DateTo
    and pr.Date_Day >= cp.Datefrom and pr.Date_Day < cp.DateTo 
      
     and pr.Day_Hour in (select [peak_hour]  from dbo.Peak_time where strategyID=@StrategyId)
      --and Count_out > @minValue
   group by pr.SubscriberNumber  , cp.Datefrom,cp.DateTo,CriteriaId






/*

[prFillSubscriberValues]

@FromDate = '05/01/2014',
@ToDate ='06/01/2014',
@StrategyId =1



truncate table Subscriber_Values

select * from [Subscriber_Values] --where  criteriaid=6 --and SubscriberNumber='015225268'
 order by FromDate asc
 
 
 
 
*/