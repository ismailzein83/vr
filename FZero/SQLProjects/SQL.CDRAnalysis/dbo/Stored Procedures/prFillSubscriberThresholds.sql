
CREATE PROCEDURE [dbo].[prFillSubscriberThresholds]
(
  @FromDate datetime,
  @ToDate datetime,
  @StrategyId int
)

AS

declare @ThCr1 decimal(18,2)
declare @ThCr2 decimal(18,2)
declare @ThCr3 decimal(18,2)
declare @ThCr4 decimal(18,2)
declare @ThCr5 decimal(18,2)
declare @ThCr6 decimal(18,2)

select @ThCr1=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =1
select @ThCr2=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =2
select @ThCr3=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =3
select @ThCr4=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =4
select @ThCr5=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =5
select @ThCr6=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =6

delete from [CDRAnalysis].[dbo].[SubscriberThresholds]
where StrategyId = @StrategyId and DateDay between @FromDate and @ToDate     


CREATE TABLE #temp(	[DateDay] [datetime] ,	[SubscriberNumber] [varchar](50) ,	[Criteria1] int ,
	[Criteria2] int ,	[Criteria3] int ,	[Criteria4] int ,
	[Criteria5] int ,	[Criteria6] int ,	[SuspectionLevelId] int ,
	[StrategyId] int )

declare @RecurDate datetime
set @RecurDate=@FromDate

while @RecurDate <= @ToDate
       begin
					INSERT INTO #temp
						   ([DateDay]
						   ,[SubscriberNumber]
						   ,[Criteria1]
						   ,[Criteria2]
						   ,[Criteria3]
						   ,[Criteria4]
						   ,[Criteria5]
						   ,[Criteria6]
						   ,[SuspectionLevelId]
						   ,[StrategyId])
				    
				  select 
				 @RecurDate
				 ,sv.SubscriberNumber--,sv.FromDate,sv.todate
				,case when sv.CriteriaId = 1 and Value < @ThCr1  then 1 else 0 end Criteria1
				,case when sv.CriteriaId = 2 and Value > @ThCr2 then 1 else 0 end Criteria2
				,case when sv.CriteriaId = 3 and Value > @ThCr3 then 1 else 0 end Criteria3
				,case when sv.CriteriaId = 4 and Value > @ThCr4 then 1 else 0 end Criteria4
				,case when sv.CriteriaId = 5 and Value > @ThCr5 then 1 else 0 end Criteria5
				,case when sv.CriteriaId = 6 and Value > @ThCr6 then 1 else 0 end Criteria6
				,1
				,@StrategyId
				from dbo.Subscriber_Values sv 
				where @RecurDate between FromDate and ToDate
				set @RecurDate =DATEADD(day,1,@RecurDate)
     end

INSERT INTO [CDRAnalysis].[dbo].[SubscriberThresholds]
           ([DateDay]
           ,[SubscriberNumber]
           ,[Criteria1]
           ,[Criteria2]
           ,[Criteria3]
           ,[Criteria4]
           ,[Criteria5]
           ,[Criteria6]
           ,[SuspectionLevelId]
           ,[StrategyId])

select [DateDay]  ,	[SubscriberNumber]  ,	sum([Criteria1])  ,
	sum([Criteria2])  ,	sum([Criteria3])  ,	sum([Criteria4])  ,
	sum([Criteria5])  ,	sum([Criteria6])  ,	[SuspectionLevelId],
	[StrategyId]
from #temp 
 group by [DateDay],[SubscriberNumber],[StrategyId],[SuspectionLevelId]


--update st
--set st.SuspectionLevelId=sl.LevelId
--from SubscriberThresholds st inner join dbo.Strategy_suspection_level sl
-- on st.StrategyId = sl.StrategyId 
-- and st.Criteria1=sl.CriteriaId1 
-- and st.Criteria2=sl.CriteriaId2
-- and st.Criteria3=sl.CriteriaId3
-- and st.Criteria4=sl.CriteriaId4
-- and st.Criteria5=sl.CriteriaId5
-- and st.Criteria6=sl.CriteriaId6
--where st.StrategyId=@StrategyId and dateday between @FromDate and @ToDate



update st
set st.SuspectionLevelId=
(
select top 1 sl.LevelId from  dbo.Strategy_suspection_level sl where sl.StrategyId = st.StrategyId 
 and st.Criteria1 >= sl.CriteriaId1  
 and st.Criteria2 >= sl.CriteriaId2 
 and st.Criteria3 >= sl.CriteriaId3
 and st.Criteria4 >= sl.CriteriaId4 
 and st.Criteria5 >= sl.CriteriaId5
 and st.Criteria6 >= sl.CriteriaId6
 order by sl.LevelId desc
 )
 from SubscriberThresholds st 
where st.StrategyId=@StrategyId and st.DateDay between @FromDate and @ToDate




/*




 db_FillSubscriberThresholds
    @FromDate ='05/01/2014',
    @ToDate ='08/01/2014',
    @StrategyId =1

select * from dbo.StrategyPeriods

select * from [SubscriberThresholds] where [SubscriberNumber]= 015228084 order by dateday

truncate table SubscriberThresholds


*/