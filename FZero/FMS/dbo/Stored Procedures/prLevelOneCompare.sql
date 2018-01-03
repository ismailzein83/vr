




CREATE PROCEDURE [dbo].[prLevelOneCompare]
	
AS


DECLARE   @ConcatString VARCHAR(4000)
SELECT   @ConcatString =  COALESCE(@ConcatString + ';', '') + fraudprefix FROM Clients 
SELECT distinct Value as Prefix  into #DistinctPrefixes FROM dbo.split ( REPLACE(@ConcatString,';;',';')) where Value <>''


CREATE TABLE #patterns (  pattern VARCHAR(20));
INSERT INTO #patterns  select Prefix +'%' from #DistinctPrefixes


Declare @LevelOneComparisonDateTime datetime
set @LevelOneComparisonDateTime= GETDATE();

update generatedcalls set reportingstatusID=3 where  reportingstatusID=1 and statusid=4

update GeneratedCalls set reportingstatusID=3 
where ID in (	SELECT	gc.ID 
				FROM	dbo.GeneratedCalls gc with(nolock)
						inner JOIN RecievedCalls rc  with(nolock,index=I_GeneratedCallsID)  ON gc.ID=rc.GeneratedCallID
				where	gc.reportingstatusID=1 
						and	(rc.CLI IN(	SELECT  CLI   
										FROM    vwCLIs 
										where	CLI is not null))
			)
update	GeneratedCalls set  LevelOneComparisonDateTime=@LevelOneComparisonDateTime, Level1Comparison =1 
where	SourceID=5 and Level1Comparison=0

update	GeneratedCalls set statusID=3, PriorityID=1, CLI='Private Number'  
where ID in (	select	gc.Id
				from	GeneratedCalls gc with(nolock)
						inner join RecievedCalls rc with(nolock,index=I_GeneratedCallsID) on gc.ID=rc.GeneratedCallID 
				where	gc.b_number=rc.cli)


		DECLARE client_cursor CURSOR
		FOR
		   SELECT distinct Countrycode 
		   FROM FMS.dbo.Clients
		OPEN client_cursor
		DECLARE @code sysname
		FETCH NEXT FROM client_cursor INTO @code
		WHILE (@@FETCH_STATUS <> -1)
		BEGIN
		   IF (@@FETCH_STATUS <> -2)
		   BEGIN   

			update GeneratedCalls set b_number=right(b_number,LEN ( b_number )-2) where b_number like '00'+@code+'%' 
			update GeneratedCalls set b_number=right(b_number,LEN ( b_number )-1) where b_number like '+'+@code+'%'
			update RecievedCalls set b_number=right(b_number,LEN ( b_number )-2) where b_number like '00'+@code+'%' 
			update RecievedCalls set b_number=right(b_number,LEN ( b_number )-1) where b_number like '+'+@code+'%' 

		   END
		   FETCH NEXT FROM client_cursor INTO @code
		END
		CLOSE client_cursor
		DEALLOCATE client_cursor




DECLARE @ID INT
DECLARE @a_number varchar(30)
DECLARE @b_number varchar(30)
DECLARE @AttemptDateTime datetime


DECLARE @getID CURSOR
SET @getID = CURSOR FOR
SELECT  [ID] ,[a_number] ,[b_number] ,[AttemptDateTime] 
FROM	GeneratedCalls  with(nolock,index=I_AttemptDateTime)
where	attemptdatetime >DATEADD(dd, -2, GETDATE()) 
		and Level1Comparison=0 and Level2Comparison=0 and StatusID=1
OPEN @getID		FETCH NEXT FROM @getID INTO @ID  ,@a_number ,@b_number ,@AttemptDateTime
WHILE @@FETCH_STATUS = 0
BEGIN
 
 IF OBJECT_ID('tempdb..#RecievedCalls') IS NOT NULL
    DROP TABLE #RecievedCalls
   
 CREATE TABLE #RecievedCalls(ID int, AttemptDateTime datetime, a_number varchar(30), CLI varchar(30), IsEqual bit, IsLocal bit, DateDifference int, b_number varchar(30));
  

 Declare @IsEqual bit
 Declare @IsLocal bit
 Declare @HasEmptyCLI bit
  
 Declare @RecievedB_number varchar(50)
 Declare @RecievedFromAttemptDateTime datetime
 Declare @RecievedToAttemptDateTime datetime
 
 
 set @RecievedB_number = @b_number;
  Set @RecievedFromAttemptDateTime =(select DATEADD(minute,-1, @AttemptDateTime));
 Set @RecievedToAttemptDateTime =(select DATEADD(minute,1, @AttemptDateTime));

 
 INSERT INTO #RecievedCalls (ID,  CLI, a_number, b_number, AttemptDateTime, IsEqual, IsLocal, DateDifference ) 
	select	top 1 ID,  CLI, a_number, b_number, AttemptDateTime, dbo.fn_CheckIfEqual(CLI,b_number) as IsEqual , 
			dbo.fn_CheckIfLocal(CLI) as IsLocal, ABS (DATEDIFF(SECOND, AttemptDateTime, @AttemptDateTime))as DateDifference 
	from	RecievedCalls with(nolock,index=I_GeneratedCallsID)
	where	GeneratedCallID=@ID;
  
if ((select COUNT(*) from #RecievedCalls) = 0)
   Begin
   ---- Duration for fraud Prefix
   If exists (select 1 from RecievedCalls rc  inner JOIN #patterns p ON (rc.cli LIKE p.pattern)   where
    (AttemptDateTime >= @RecievedFromAttemptDateTime  and AttemptDateTime <= @RecievedToAttemptDateTime)
				and (b_number =@RecievedB_number or b_number in (SELECT distinct '0'+REPLACE(@RecievedB_number, CountryCode,'') from Clients)) 
				and (GeneratedCallID is NULL )	
				)
		--- 
		begin
			INSERT INTO #RecievedCalls (ID,  CLI, a_number, b_number, AttemptDateTime, IsEqual, IsLocal, DateDifference ) 
			select	top 1 ID,  CLI, a_number, b_number, AttemptDateTime, dbo.fn_CheckIfEqual(CLI,b_number) as IsEqual ,  dbo.fn_CheckIfLocal(CLI) as IsLocal, 
					ABS (DATEDIFF(SECOND, AttemptDateTime, @AttemptDateTime))as DateDifference 
			from	RecievedCalls with(nolock,index=I_AttemptDateTime) 
			where (AttemptDateTime >= @RecievedFromAttemptDateTime  and AttemptDateTime <= @RecievedToAttemptDateTime)
			and (DurationInSeconds >= 1 and DurationInSeconds <= 33)
					and (b_number =@RecievedB_number or b_number in (SELECT distinct '0'+REPLACE(@RecievedB_number, CountryCode,'') from Clients)) 
					and (GeneratedCallID is NULL )				  
			order by DateDifference;
		end
		---
	Else
		----- -----
		begin 
			INSERT INTO #RecievedCalls (ID,  CLI, a_number, b_number, AttemptDateTime, IsEqual, IsLocal, DateDifference ) 
			select	top 1 ID,  CLI, a_number, b_number, AttemptDateTime, dbo.fn_CheckIfEqual(CLI,b_number) as IsEqual ,  dbo.fn_CheckIfLocal(CLI) as IsLocal, 
					ABS (DATEDIFF(SECOND, AttemptDateTime, @AttemptDateTime))as DateDifference 
			from	RecievedCalls with(nolock,index=I_AttemptDateTime) 
			where (AttemptDateTime >= @RecievedFromAttemptDateTime  and AttemptDateTime <= @RecievedToAttemptDateTime)
					and (b_number =@RecievedB_number or b_number in (SELECT distinct '0'+REPLACE(@RecievedB_number, CountryCode,'') from Clients)) 
					and (GeneratedCallID is NULL )				  
			order by DateDifference;
		
		end
		----- -----
   End
    
   
if ((select COUNT(*) from #RecievedCalls) >0)
	Begin
		select @IsEqual = (select top 1 IsEqual from #RecievedCalls);
		select @IsLocal = (select top 1 IsLocal from #RecievedCalls);

		Declare @CLI varchar(10)
		select @CLI =(select top 1 CLI from #RecievedCalls);

		select @HasEmptyCLI =(SELECT CASE  WHEN (@CLI IS  NULL) THEN 1   WHEN (@CLI ='') THEN 1   ELSE 0 END );

		update	RecievedCalls 
		set		GeneratedCallID=@ID 
		where	ID= (select top 1 ID from #RecievedCalls);
      
      
      
		if (@HasEmptyCLI=1)
			Begin
			  update GeneratedCalls set StatusID=3, PriorityID=3, LevelOneComparisonDateTime=@LevelOneComparisonDateTime, Level1Comparison =1 where ID = @ID 
			End      
			else if (@IsEqual=0 and @IsLocal=1)
				Begin	
					if ((select max(case when @CLI like Prefix+'%' and @b_number like  c.CountryCode +'%' then 1 else 0 end) IsTermination from #DistinctPrefixes dp inner join Clients c on  c.FraudPrefix like '%' +  dp.Prefix + '%')=1)
					
					Begin
						update	GeneratedCalls 
						set		StatusID=2, LevelOneComparisonDateTime=@LevelOneComparisonDateTime, Level1Comparison =1 
						where	ID = @ID 
					End		   
			   End
	   else 
		   Begin
			  update	GeneratedCalls 
			  set		StatusID=4, LevelOneComparisonDateTime=@LevelOneComparisonDateTime, Level1Comparison =1 
			  where		ID = @ID 
		   End
      
	End
    else if (@AttemptDateTime <= DATEADD(DD, -4 ,@LevelOneComparisonDateTime) )
			Begin
				update  GeneratedCalls 
				set		Level1Comparison=1 , StatusID=5, LevelOneComparisonDateTime=@LevelOneComparisonDateTime 
				where	ID = @ID 
			End 
FETCH NEXT	FROM @getID INTO @ID  ,@a_number ,@b_number ,@AttemptDateTime
END
CLOSE @getID
DEALLOCATE @getID

--exec prLevelOneCompare_HaveNoGenerated @LevelOneComparisonDateTime
--exec prLevelOneCompare_HaveNoGenerated_Android  @LevelOneComparisonDateTime