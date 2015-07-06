CREATE PROCEDURE [dbo].[Ol_prFillDailySubscriberThresholds]
(
   @StrategyId int 
)
AS

IF NOT EXISTS (SELECT TOP 1 id FROM StrategyPeriods sp WHERE sp.StrategyId=@StrategyId AND sp.PeriodId=1)

BEGIN
	return
END

 declare @minValue int
 select @minValue= Min_count_value from dbo.Strategy_Min_Values where strategyid =@StrategyId and criteriaid=9

--========== setting Control Table    =================

 declare @startingId int
 declare @StartingDate datetime
 declare @EndingDate  datetime
 declare @CtrlTableId INT

INSERT INTO [ControlTable]
           (
            PeriodId,OperationtypeId,[StartedDateTime],StrategyId
           )
     VALUES
           (1,7,GETDATE(),@StrategyId)

set @CtrlTableId=@@IDENTITY
 
 
 select @startingId=LastId,@StartingDate=EndingUnitDate from ControlTable where id=
 (
   select top 1 Id from ControlTable WHERE StrategyId=@StrategyId and PeriodId=1 AND OperationtypeId=7 and id <> @CtrlTableId  order by id desc
 )
 if isnull(@startingId,0)=0
 begin
  select @startingId= min(sv.id) from dbo.Subscriber_Values sv  WHERE sv.PeriodId=1
 end
 if @StartingDate is null
  begin
     SELECT  @StartingDate= CONVERT(datetime,CONVERT(char,min(FromDate),106))from dbo.Subscriber_Values WHERE PeriodId=1
  END
     SELECT  @EndingDate= CONVERT(datetime,CONVERT(char,MAX(FromDate),106))from dbo.Subscriber_Values WHERE PeriodId=1
    
--==============================
declare @ThCr1 decimal(18,2), @ThCr2 decimal(18,2), @ThCr3 decimal(18,2), @ThCr4 decimal(18,2), @ThCr5 decimal(18,2)
declare @ThCr6 decimal(18,2), @ThCr7 decimal(18,2), @ThCr8 decimal(18,2), @ThCr9 decimal(18,2), @ThCr10 decimal(18,2)
declare @ThCr11 decimal(18,2),@ThCr12 decimal(18,2),@ThCr13 decimal(18,2),@ThCr14 decimal(18,2),@ThCr15 decimal(18,2)

select @ThCr1=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =1
select @ThCr2=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =2
select @ThCr3=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =3
select @ThCr4=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =4
select @ThCr5=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =5
select @ThCr6=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =6
select @ThCr7=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =7
select @ThCr8=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =8
select @ThCr9=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =9
select @ThCr10=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =10
select @ThCr11=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =11
select @ThCr12=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =12
select @ThCr13=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =13
select @ThCr14=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =14
select @ThCr15=MAxValue from StrategyThreshold where StrategyId=@StrategyId and criteriaID =15


declare @prCr1 int, @prCr2 int, @prCr3 int, @prCr4 int, @prCr5 int, @prCr6 int
declare @prCr7 int, @prCr8 int, @prCr9 int, @prCr10 int, @prCr11 int, @prCr12 int
declare @prCr13 int, @prCr14 int, @prCr15 int

select @prCr1=periodId from strategyPeriods where StrategyId=@StrategyId and criteriaID =1
select @prCr2=periodId from strategyPeriods where StrategyId=@StrategyId and criteriaID =2
select @prCr3=periodId from strategyPeriods where StrategyId=@StrategyId and criteriaID =3
select @prCr4=periodId from strategyPeriods where StrategyId=@StrategyId and criteriaID =4
select @prCr5=periodId from strategyPeriods where StrategyId=@StrategyId and criteriaID =5
select @prCr6=periodId from strategyPeriods where StrategyId=@StrategyId and criteriaID =6
select @prCr7=periodId from strategyPeriods where StrategyId=@StrategyId and criteriaID =7
select @prCr8=periodId from strategyPeriods where StrategyId=@StrategyId and criteriaID =8
select @prCr9=periodId from strategyPeriods where StrategyId=@StrategyId and criteriaID =9
select @prCr10=periodId from strategyPeriods where StrategyId=@StrategyId and criteriaID =10
select @prCr11=periodId from strategyPeriods where StrategyId=@StrategyId and criteriaID =11
select @prCr12=periodId from strategyPeriods where StrategyId=@StrategyId and criteriaID =12
select @prCr13=periodId from strategyPeriods where StrategyId=@StrategyId and criteriaID =13
select @prCr14=periodId from strategyPeriods where StrategyId=@StrategyId and criteriaID =14
select @prCr15=periodId from strategyPeriods where StrategyId=@StrategyId and criteriaID =15

delete from [SubscriberThresholds]
where StrategyId = @StrategyId and DateDay between @StartingDate and @EndingDate  AND [Periodid]=1        

CREATE TABLE #temp(	[DateDay] [datetime] ,	[SubscriberNumber] [varchar](50) ,
	[Criteria1] decimal(18,2) ,	[Criteria2] decimal(18,2) ,	[Criteria3] decimal(18,2) ,
	[Criteria4] decimal(18,2) ,	[Criteria5] decimal(18,2) ,	[Criteria6] decimal(18,2) ,	
	[Criteria7] decimal(18,2) ,	[Criteria8] decimal(18,2) ,[Criteria9] decimal(18,2) ,
	[Criteria10] decimal(18,2),[Criteria11] decimal(18,2) ,[Criteria12] decimal(18,2) ,
	[Criteria13] decimal(18,2),[Criteria14] decimal(18,2) ,[Criteria15] decimal(18,2) ,
	[SuspectionLevelId] int ,
	[StrategyId] INT,   [Periodid] int )


--==========================================
					INSERT INTO #temp
				   ([DateDay]
				  ,[SubscriberNumber]
				  ,[Criteria1]
				  ,[Criteria2]
				  ,[Criteria3]
				  ,[Criteria4]
				  ,[Criteria5]
				  ,[Criteria6]
				  ,[Criteria7]
				  ,[Criteria8]
				  --,[Criteria9]
				  ,[Criteria10]
				  ,[Criteria11]
				  ,[Criteria12]
				  ,[Criteria13]
				  --,[Criteria14]
				  ,[Criteria15]
				  ,[SuspectionLevelId]
				  ,[StrategyId]
				  ,[Periodid])
				    
				  select 
				 sv.FromDate
				,sv.SubscriberNumber--,sv.FromDate,sv.todate
				,case when sv.CriteriaId = 1 AND sv.PeriodId= 1 and @prCr1=1  then sv.VALUE*1.00/@ThCr1*1.00 else null end Criteria1
				,case when sv.CriteriaId = 2  AND sv.PeriodId= 1 and @prCr2=1 then sv.VALUE*1.00/@ThCr2*1.00 else null end Criteria2
				,case when sv.CriteriaId = 3  AND sv.PeriodId= 1 and @prCr3=1 then sv.VALUE*1.00/@ThCr3*1.00 else null end Criteria3
				,case when sv.CriteriaId = 4  AND sv.PeriodId= 1 and @prCr4=1 then sv.VALUE*1.00/@ThCr4*1.00 else null end Criteria4
				,case when sv.CriteriaId = 5  AND sv.PeriodId= 1 and @prCr5=1 then sv.VALUE*1.00/@ThCr5*1.00 else null end Criteria5
				,case when sv.CriteriaId = 6  AND sv.PeriodId= 1 and @prCr6=1 then sv.VALUE*1.00/@ThCr6*1.00 else null end Criteria6
				,case when sv.CriteriaId = 7  AND sv.PeriodId= 1 and @prCr7=1 then sv.VALUE*1.00/@ThCr7*1.00 else null end Criteria7
				,case when sv.CriteriaId = 8  AND sv.PeriodId= 1 and @prCr8=1 then sv.VALUE*1.00/@ThCr8*1.00 else null end Criteria8
				--,case when sv.CriteriaId = 9  AND sv.PeriodId= 6 and @prCr9=6 then sv.VALUE*1.00/@ThCr9*1.00 else null end Criteria9
				,case when sv.CriteriaId = 10  AND sv.PeriodId= 1 and @prCr10=1 then sv.VALUE*1.00/@ThCr10*1.00 else null end Criteria10
				,case when sv.CriteriaId = 11  AND sv.PeriodId= 1 and @prCr11=1 then sv.VALUE*1.00/@ThCr11*1.00 else null end Criteria11
				,case when sv.CriteriaId = 12  AND sv.PeriodId= 1 and @prCr12=1 then sv.VALUE*1.00/@ThCr12*1.00 else null end Criteria12
				,case when sv.CriteriaId = 13  AND sv.PeriodId= 1 and @prCr13=1 then sv.VALUE*1.00/@ThCr13*1.00 else null end Criteria13
				--,case when sv.CriteriaId = 14  AND sv.PeriodId= 6 and @prCr14=6 then sv.VALUE*1.00/@ThCr14*1.00 else null end Criteria14
				,case when sv.CriteriaId = 15  AND sv.PeriodId= 1 and @prCr15=1 then sv.VALUE*1.00/@ThCr15*1.00 else null end Criteria15
				,1
				,@StrategyId
				,1
				from dbo.Subscriber_Values sv 
				--where @RecurDate between  sv.FromDate and sv.ToDate 
				where sv.FromDate >= @StartingDate and  sv.FromDate <= @EndingDate
				  GROUP BY sv.SubscriberNumber,sv.CriteriaId,sv.PeriodId,sv.VALUE,sv.FromDate
				 --=================== 9 and 14  
				 
				 
				  INSERT INTO #temp
						   ([DateDay]
						   ,[SubscriberNumber]
						   ,[Criteria9]
						   ,[Criteria14]
						   ,[SuspectionLevelId]
						   ,[StrategyId]
						   ,[Periodid])
				    
				  select 
				 sv.FromDate
				,sv.SubscriberNumber
							
				,case when sv.CriteriaId = 3 AND sv.PeriodId= 6 AND sv.VALUE > @minValue and @prCr9=1  --	@minValue  
			     then COUNT(DISTINCT DATEPART(hour,sv.FromDate))/@ThCr9  end Criteria9
								
				,case when sv.CriteriaId = 3 AND sv.PeriodId= 6 and @prCr14=1  AND DATEPART(hour,sv.FromDate) IN (select [peak_hour]  from dbo.Peak_time where strategyID=@StrategyId)
				then sum(sv.VALUE)/@ThCr14 else 0 end Criteria14
				,1
				,@StrategyId
				,1
				from dbo.Subscriber_Values sv 
				where sv.FromDate >= @StartingDate and  sv.FromDate <= @EndingDate
				GROUP BY sv.SubscriberNumber,sv.CriteriaId,sv.PeriodId,sv.VALUE, DATEPART(day,sv.FromDate),sv.FromDate
				  
				 --========= 
	
	
	
	
	
				  
--===================================================
INSERT INTO [SubscriberThresholds]
           ([DateDay]
           ,[SubscriberNumber]
           ,[Criteria1],[Criteria2],[Criteria3],[Criteria4],[Criteria5],[Criteria6]
           ,[Criteria7],[Criteria8],[Criteria9],[Criteria10],[Criteria11],[Criteria12]
           ,[Criteria13] ,[Criteria14] ,[Criteria15]
           ,[SuspectionLevelId]
           ,[StrategyId]
           ,[Periodid])

select [DateDay]  ,	[SubscriberNumber]  ,	sum([Criteria1])  ,
	sum([Criteria2])  ,	sum([Criteria3])  ,	sum([Criteria4])  ,
	sum([Criteria5])  ,	sum([Criteria6]),	sum([Criteria7])  ,
	sum([Criteria8]),sum([Criteria9]),sum([Criteria10]),sum([Criteria11]) ,
	sum([Criteria12]),sum([Criteria13]),sum([Criteria14]),sum([Criteria15])
	,[SuspectionLevelId],[StrategyId],[Periodid]
from #temp 
 group by [DateDay],[SubscriberNumber],[StrategyId],[SuspectionLevelId],[Periodid]

update st
set st.SuspectionLevelId=
(
select top 1 sl.LevelId from  dbo.Strategy_suspicion_level sl where sl.StrategyId = st.StrategyId 

 AND ((st.Criteria1 <= sl.Cr1Per AND sl.CriteriaId1=1) or sl.CriteriaId1=0 )--OR st.Criteria1 IS null )
 AND ((st.Criteria2 >= sl.Cr2Per AND sl.CriteriaId2=1) or sl.CriteriaId2=0 )--OR st.Criteria2 IS null )
 AND ((st.Criteria3 >= sl.Cr3Per AND sl.CriteriaId3=1) or sl.CriteriaId3=0 )--OR st.Criteria3 IS null )
 AND ((st.Criteria4 <= sl.Cr4Per AND sl.CriteriaId4=1) or sl.CriteriaId4=0 )--OR st.Criteria4 IS null )
 AND ((st.Criteria5 >= sl.Cr5Per AND sl.CriteriaId5=1) or sl.CriteriaId5=0 )--OR st.Criteria5 IS null )
 AND ((st.Criteria6 >= sl.Cr6Per AND sl.CriteriaId6=1) or sl.CriteriaId6=0 )--OR st.Criteria6 IS null )
 AND ((st.Criteria7 <= sl.Cr7Per AND sl.CriteriaId7=1) or sl.CriteriaId7=0 )--OR st.Criteria7 IS null )
 AND ((st.Criteria8 <= sl.Cr8Per AND sl.CriteriaId8=1) or sl.CriteriaId8=0 )--OR st.Criteria8 IS null )
 AND ((st.Criteria9 >= sl.Cr9Per AND sl.CriteriaId9=1) or sl.CriteriaId9=0 )--OR st.Criteria9 IS null )
 AND ((st.Criteria10 >= sl.Cr10Per AND sl.CriteriaId10=1) or sl.CriteriaId10=0 )--OR st.Criteria10 IS null )
 AND ((st.Criteria11 <= sl.Cr11Per AND sl.CriteriaId11=1) or sl.CriteriaId11=0 )--OR st.Criteria11 IS null )
 AND ((st.Criteria12 >= sl.Cr12Per AND sl.CriteriaId12=1) or sl.CriteriaId12=0 )--OR st.Criteria12 IS null )
 AND ((st.Criteria13 <= sl.Cr13Per AND sl.CriteriaId13=1) or sl.CriteriaId13=0 )--OR st.Criteria13 IS null )
 AND ((st.Criteria14 <= sl.Cr14Per AND sl.CriteriaId14=1) or sl.CriteriaId14=0 )--OR st.Criteria14 IS null )
 AND ((st.Criteria15 <= sl.Cr15Per AND sl.CriteriaId15=1) or sl.CriteriaId15=0 )--OR st.Criteria15 IS null )
 

 order by sl.LevelId desc
 )
 from SubscriberThresholds st 
where st.StrategyId=@StrategyId and st.DateDay between @StartingDate and @EndingDate AND st.Periodid=1
--where st.StrategyId=@StrategyId and st.DateDay >= @StartingDate and st.DateDay <@EndingDate AND st.Periodid=1

 UPDATE [ControlTable]
       SET 
       [FinishedDateTime] =GETDATE()
      ,[StartingUnitDate] = @StartingDate
      ,[EndingUnitDate] = @EndingDate
      --,[NumberOfProfileRecords]=@NumberOfProfileRecords
      --,NumberOfCalls=@NumberOfCalls
     WHERE id=@CtrlTableId


/*

[db_FillDailySubscriberThresholds]
     
 @StrategyId =28

select * from dbo.StrategyPeriods

select * from [SubscriberThresholds] where [SubscriberNumber]= 015228084 order by dateday

truncate table SubscriberThresholds

delete from SubscriberThresholds where periodid=1

select * from subscriberthresholds where periodid=1 and criteria15 is not null
*/