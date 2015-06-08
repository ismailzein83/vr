
 
CREATE PROCEDURE [dbo].[prFillHourlySubscriberThresholds]
(
   @StrategyId int 
)
AS

IF NOT EXISTS (SELECT TOP 1 id FROM StrategyPeriods sp WHERE sp.StrategyId=@StrategyId AND sp.PeriodId=6)

BEGIN
	return
END

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
           (6,7,GETDATE(),@StrategyId)

set @CtrlTableId=@@IDENTITY
 
 select @startingId=LastId,@StartingDate=EndingUnitDate from ControlTable where id=
 (
   select top 1 Id from ControlTable WHERE StrategyId=@StrategyId and PeriodId=6 AND OperationtypeId=7 and id <> @CtrlTableId  order by id desc
 )
 if isnull(@startingId,0)=0
 begin
  select @startingId= min(nb.id) from dbo.Subscriber_Values nb  WHERE nb.PeriodId=6
 end
 if @StartingDate is null
  begin
     SELECT  @StartingDate= DATEADD(hour, DATEDIFF(hour, 0, min(FromDate)), 0)from dbo.NumberProfile WHERE PeriodId=6
  end
     SELECT  @EndingDate= DATEADD(hour, DATEDIFF(hour, 0, MAX(FromDate)), 0)from dbo.NumberProfile  WHERE PeriodId=6

--==============================
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

--==============================

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
where StrategyId = @StrategyId and DateDay between @StartingDate and @EndingDate AND [Periodid]=6     

CREATE TABLE #temp(	[DateDay] [datetime] ,	[SubscriberNumber] [varchar](50) ,
	[Criteria1] decimal(18,2) ,	[Criteria2] decimal(18,2) ,	[Criteria3] decimal(18,2) ,
	[Criteria4] decimal(18,2) ,	[Criteria5] decimal(18,2) ,	[Criteria6] decimal(18,2) ,	
	[Criteria7] decimal(18,2) ,	[Criteria8] decimal(18,2) ,[Criteria9] decimal(18,2) ,
	[Criteria10] decimal(18,2),[Criteria11] decimal(18,2) ,[Criteria12] decimal(18,2) ,
	[Criteria13] decimal(18,2),[Criteria14] decimal(18,2) ,[Criteria15] decimal(18,2) ,
	[SuspectionLevelId] int ,
	[StrategyId] INT,   [Periodid] int )

--=============================================



 INSERT INTO #temp
    (DateDay, SubscriberNumber, Criteria1, Criteria2, Criteria3, Criteria4, Criteria5, Criteria6, Criteria7,
     Criteria8, Criteria11, Criteria12, Criteria13,Criteria15
    ,SuspectionLevelId, StrategyId, Periodid)
    
  SELECT
    FromDate,
    nb.SubscriberNumber,
    nb.InCalls_Vs_OutCalls * 1.00 / @ThCr1 * 1.00 Criteria1,
    nb.diff_output_numb * 1.00 / @ThCr2 * 1.00 Criteria2,
    nb.Count_out_Calls * 1.00 / @ThCr3 * 1.00 Criteria3,
    isnull(nb.Total_BTS, 0) * 1.00 / @ThCr4 * 1.00 Criteria4,
    nb.total_out_volume * 1.00 / @ThCr5 * 1.00 Criteria5,
    nb.total_IMEI * 1.00 / @ThCr6 * 1.00 Criteria6,
    nb.AvrDurIn_Vs_AvrDurOut * 1.00 / @ThCr7 * 1.00 Criteria7,
    nb.OutOffNet_Vs_OutOnNet * 1.00 / @ThCr8 * 1.00 Criteria8,
    isnull(nb.Count_Out_SMS, 0) * 1.00 / @ThCr11 * 1.00 Criteria11,
    nb.DiffDest_Vs_OutCalls * 1.00 / @ThCr12 * 1.00 Criteria12,
    nb.OutInter_Vs_OutCalls * 1.00 / @ThCr13 * 1.00 Criteria13,
    nb.Total_Data_Volume * 1.00 / @ThCr15 * 1.00 Criteria15,
    1,
    @StrategyId,
    6
  FROM
    NumberProfile nb
  WHERE
    nb.FromDate >= @StartingDate AND
    nb.FromDate <= @EndingDate AND
    nb.PeriodId = 6 
  --GROUP BY
  --  nb.SubscriberNumber,
  --  nb.PeriodId,
  --  nb.InCalls_Vs_OutCalls,
  --  nb.FromDate,
  --  diff_output_numb
 
 UPDATE
     st
  SET
    st.SuspectionLevelId =
    (
    SELECT
      top 1 sl.LevelId
    FROM
      Strategy_Suspicion_Level sl
    WHERE
      sl.StrategyId = st.StrategyId
      AND
      ((st.Criteria1 <= sl.Cr1Per AND sl.CriteriaId1 = 1) OR
      sl.CriteriaId1 = 0) -- OR st.Criteria1 IS null )
      AND
      ((st.Criteria2 >= sl.Cr2Per AND sl.CriteriaId2 = 1) OR
      sl.CriteriaId2 = 0) -- OR st.Criteria2 IS null )
      AND
      ((st.Criteria3 >= sl.Cr3Per AND sl.CriteriaId3 = 1) OR
      sl.CriteriaId3 = 0) -- OR st.Criteria3 IS null )
      AND
      ((st.Criteria4 <= sl.Cr4Per AND sl.CriteriaId4 = 1) OR
      sl.CriteriaId4 = 0) -- OR st.Criteria4 IS null )
      AND
      ((st.Criteria5 >= sl.Cr5Per AND sl.CriteriaId5 = 1) OR
      sl.CriteriaId5 = 0) -- OR st.Criteria5 IS null )
      AND
      ((st.Criteria6 >= sl.Cr6Per AND sl.CriteriaId6 = 1) OR
      sl.CriteriaId6 = 0) -- OR st.Criteria6 IS null )
      AND
      ((st.Criteria7 <= sl.Cr7Per AND sl.CriteriaId7 = 1) OR
      sl.CriteriaId7 = 0) -- OR st.Criteria7 IS null )
      AND
      ((st.Criteria8 <= sl.Cr8Per AND sl.CriteriaId8 = 1) OR
      sl.CriteriaId8 = 0) -- OR st.Criteria8 IS null )
      AND
      ((st.Criteria9 <= sl.Cr9Per AND sl.CriteriaId9 = 1) OR
      sl.CriteriaId9 = 0) -- OR st.Criteria9 IS null )
      AND
      ((st.Criteria10 >= sl.Cr10Per AND sl.CriteriaId10 = 1) OR
      sl.CriteriaId10 = 0) -- OR st.Criteria10 IS null )
      AND
      ((st.Criteria11 <= sl.Cr11Per AND sl.CriteriaId11 = 1) OR
      sl.CriteriaId11 = 0) -- OR st.Criteria11 IS null )
      AND
      ((st.Criteria12 >= sl.Cr12Per AND sl.CriteriaId12 = 1) OR
      sl.CriteriaId12 = 0) -- OR st.Criteria12 IS null )
      AND
      ((st.Criteria13 <= sl.Cr13Per AND sl.CriteriaId13 = 1) OR
      sl.CriteriaId13 = 0) -- OR st.Criteria13 IS null )
      AND
      ((st.Criteria14 <= sl.Cr14Per AND sl.CriteriaId14 = 1) OR
      sl.CriteriaId14 = 0) -- OR st.Criteria14 IS null )
      AND
      ((st.Criteria15 <= sl.Cr15Per AND sl.CriteriaId15 = 1) OR
      sl.CriteriaId15 = 0) -- OR st.Criteria15 IS null )
    ORDER BY
      sl.LevelId DESC
     
    )
 FROM #temp st
  WHERE
    st.StrategyId = @StrategyId AND
    st.DateDay BETWEEN @StartingDate AND @EndingDate  


INSERT INTO SubscriberThresholds
    (DateDay
    , SubscriberNumber
    , Criteria1, Criteria2, Criteria3, Criteria4, Criteria5, Criteria6
    , Criteria7, Criteria8, Criteria9, Criteria10, Criteria11, Criteria12
    , Criteria13, Criteria14, Criteria15
    , SuspectionLevelId
    , StrategyId
    , Periodid)
  SELECT
    DateDay,
    SubscriberNumber,
    Criteria1, Criteria2,Criteria3, Criteria4,Criteria5,Criteria6, Criteria7,Criteria8,
    Criteria9, Criteria10,Criteria11, Criteria12,Criteria13,Criteria14,Criteria15,
    SuspectionLevelId,    StrategyId,
    Periodid
  FROM
    #temp
    where SuspectionLevelId <> 1
  --GROUP BY
  --  DateDay,
  --  SubscriberNumber,
  --  StrategyId,
  --  SuspectionLevelId,
  --  Periodid
    
    
     -- ================

  UPDATE
    ControlTable
  SET
    FinishedDateTime = GETDATE()
    ,StartingUnitDate = @StartingDate
    ,EndingUnitDate = @EndingDate
  --      ,`NumberOfProfileRecords`=v_NumberOfProfileRecords
  --      ,NumberOfCalls=v_NumberOfCalls
  WHERE
    id = @CtrlTableId 

-- ==============

/*

ts_prFillHourlySubscriberThresholds 1

 */