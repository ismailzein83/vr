
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[prLevelOneCompare_HaveNoGenerated]
	@LevelOneComparisonDateTime datetime
AS


DECLARE   @ConcatString VARCHAR(4000)
SELECT   @ConcatString =  COALESCE(@ConcatString + ';', '') + fraudprefix FROM Clients 
SELECT distinct Value as Prefix  into #DistinctPrefixes FROM dbo.split ( REPLACE(@ConcatString,';;',';')) where Value <>''
select distinct   Prefix+'%' as CLIPrefix    ,  c.CountryCode +'%' as b_numberPrefix , Length as NumberLength 
into #Settings
 from #DistinctPrefixes dp inner join Clients c 
on  c.FraudPrefix like '%' +  dp.Prefix + '%'

DECLARE @ID INT
DECLARE @CLI varchar(30)
DECLARE @b_number varchar(30)
DECLARE @AttemptDateTime datetime
DECLARE @DurationInSeconds INT
DECLARE @OriginationNetwork varchar(50)
DECLARE @Carrier varchar(50)
DECLARE @Type varchar(50)
DECLARE @ClientID int

DECLARE @getID CURSOR
SET @getID = CURSOR FOR
SELECT  [ID],[CLI],[b_number],[AttemptDateTime],[DurationInSeconds],[OriginationNetwork],[Carrier],[Type], [ClientID]
FROM	RecievedCalls  with(nolock,index=I_AttemptDateTime)
 inner join #Settings s  on LEN (CLI)= s.NumberLength and  CLI like s.CLIPrefix and  b_number  like s.b_numberPrefix
where	AttemptDateTime < dateadd(hh, -1, getdate())
		and  GeneratedCallID is null 
order by ID 
OPEN @getID FETCH NEXT FROM @getID INTO @ID ,@CLI ,@b_number ,@AttemptDateTime ,@DurationInSeconds, @OriginationNetwork, @Carrier, @Type, @ClientID
WHILE @@FETCH_STATUS = 0
BEGIN
 Declare @RecievedB_number varchar(50)
    

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

			 IF  CHARINDEX(@code,@b_number) > 0 
			set @RecievedB_number= RIGHT(@b_number, charindex(REVERSE(@code), REVERSE(@b_number))-1)
    
			IF  CHARINDEX('00'+@code,@b_number) > 0 
			set @RecievedB_number= RIGHT(@b_number, charindex(REVERSE('00'+@code), REVERSE(@b_number))-1)
    
			IF  CHARINDEX('+'+ @code,@b_number) > 0 
			set @RecievedB_number= RIGHT(@b_number, charindex(REVERSE('+'+@code), REVERSE(@b_number))-1)

		   END
		   FETCH NEXT FROM client_cursor INTO @code
		END
		CLOSE client_cursor
		DEALLOCATE client_cursor
    

		DECLARE @MobileOperatorID INT

		set @MobileOperatorID = 3 -- PSTN
		
		set @MobileOperatorID = (
		
		
		
		SELECT    top 1 MobileOperators.ID AS MobileOperatorID
		FROM         MobileOperators 
		                INNER JOIN Users     ON MobileOperators.UserID = Users.ID
						INNER JOIN Clients c ON Users.Prefix like '%' + LEFT(@RecievedB_number, PrefixLength) + '%'
		WHERE     (Users.Prefix IS NOT NULL) AND (Users.Prefix <> ';;')   and Users.ClientID=@ClientID     )

  

 
 INSERT INTO [FMS].[dbo].[GeneratedCalls]
           ([SourceID] ,[MobileOperatorID] ,[StatusID]  ,[PriorityID]  ,[ReportingStatusID]
           ,[DurationInSeconds]  ,[MobileOperatorFeedbackID]      ,[a_number]    ,[b_number]
           ,[CLI]     ,[OriginationNetwork]    ,[AssignedTo]  ,[AssignedBy]   ,[ReportID]  
           ,[AttemptDateTime]        ,[LevelOneComparisonDateTime]   ,[LevelTwoComparisonDateTime] 
           ,[FeedbackDateTime] ,[AssignmentDateTime]  ,[ImportID]   ,[ReportingStatusChangedBy]
           ,[Level1Comparison]  ,[Level2Comparison]   ,[ToneFeedbackID]       ,[FeedbackNotes], [Carrier], [Type])
           Values
 (         8
           ,@MobileOperatorID
           ,2
           ,NULL
           ,1
           ,@DurationInSeconds
           ,NULL
           ,'Unknown'
           ,@b_number
           ,NULL
           ,@OriginationNetwork
           ,NULL
           ,NULL
           ,NULL
           ,@AttemptDateTime
           ,@LevelOneComparisonDateTime
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,1
           ,0
           ,NULL
           ,NULL
		   ,@Carrier
		   ,@Type)
    ;
 
 

 DECLARE @GeneratedCallID int
set @GeneratedCallID=SCOPE_IDENTITY();


 
 update RecievedCalls set  GeneratedCallID=@GeneratedCallID where ID= @ID;
 
 
FETCH NEXT
FROM @getID INTO @ID  ,@CLI ,@b_number ,@AttemptDateTime ,@DurationInSeconds, @OriginationNetwork, @Carrier, @Type, @ClientID
END
 CLOSE @getID
DEALLOCATE @getID