CREATE PROCEDURE [dbo].[prFillDailyCellProfile]
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
           (1,9,GETDATE())

set @CtrlTableId=@@IDENTITY
 
 
 select @startingId=LastId,@StartingDate=EndingUnitDate from ControlTable where id=
 (
   select top 1 Id from ControlTable where PeriodId=1  AND OperationtypeId=9 and id <> @CtrlTableId  order by id desc
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
  
  --==========================================
	INSERT INTO [CellProfile]([Cell_Id] ,[Date_Day] ,[Day_Hour] ,[Distinct_MSISDN_Calls] ,[Distinct_IMEI],[Distinct_MSISDN_Msg] )
 	select Cell_id Cell ,
	convert(varchar(12), connectDateTime , 102 ) Date_Day,
	25 Day_Hour
	,ISNULL(COUNT(DISTINCT (case when call_type=1 then MSISDN end )),0) Distinct_MSISDN_Calls
	,COUNT(DISTINCT (case when call_type=1 then IMEI end ))Distinct_IMEI
	,COUNT(DISTINCT (case when call_type=2 then MSISDN end ))Distinct_MSISDN_Msg
	from NormalCdr
	--=====
	where connectDateTime >=  @StartingDate and connectDateTime < @EndingDate 
	--=====
	group by Cell_id , convert(varchar(12), connectDateTime , 102 )  
	
	
	
 
    --===========
    declare @NumberOfProfileRecords INT
    
set @NumberOfProfileRecords =@@ROWCOUNT
    
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

 [db_FillDailyCellProfile]
 
*/