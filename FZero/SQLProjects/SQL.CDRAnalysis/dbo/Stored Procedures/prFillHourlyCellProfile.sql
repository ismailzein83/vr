

CREATE PROCEDURE [dbo].[prFillHourlyCellProfile]
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
           (6,9,GETDATE())

set @CtrlTableId=@@IDENTITY
 
 select @startingId=LastId,@StartingDate=EndingUnitDate from ControlTable where id=
 (
   select top 1 Id from ControlTable where PeriodId=6  AND OperationtypeId=9  and id <> @CtrlTableId  order by id desc
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
    
    -------------------------------
    INSERT INTO [CellProfile]([Cell_Id] ,[Date_Day] ,[Day_Hour] ,[Distinct_MSISDN_Calls] ,[Distinct_IMEI],[Distinct_MSISDN_Msg] )
 	select Cell_id Cell ,
	convert(varchar(12), connectDateTime , 102 ) Date_Day,
	DATEPART(hour, connectdatetime ) Day_Hour   ,
	COUNT(DISTINCT (case when call_type=1 then MSISDN end ))Distinct_MSISDN_Calls,
	COUNT(DISTINCT (case when call_type=1 then IMEI  end ))Distinct_IMEI,
	COUNT(DISTINCT (case when call_type=2 then MSISDN  end ))Distinct_MSISDN_Msg
	from NormalCdr
	--=====
	where connectDateTime >=  @StartingDate and connectDateTime < @EndingDate 
	--=====
	group by cell_id , convert(varchar(12), connectDateTime , 102 )  ,  DATEPART(hour, connectdatetime )

    --==========================================
    declare @NumberOfProfileRecords INT
    SET @NumberOfProfileRecords=@@ROWCOUNT
    --select @NumberOfProfileRecords =count(*) from outgoing_t1
    
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
 

 

 --select @startingId startingId, @StartingDate  StartingDate, @EndingDate  EndingDate , @CtrlTableId  CtrlTableId

	
END

/*

 [[db_FillHourlyCellProfile]]
 
*/