-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[prLevelOneCompare_HaveNoGenerated_Android]
	@LevelOneComparisonDateTime datetime
AS




DECLARE @ID INT
DECLARE @CLI varchar(30)
DECLARE @b_number varchar(30)
DECLARE @AttemptDateTime datetime
DECLARE @DurationInSeconds INT
DECLARE @OriginationNetwork varchar(50)
DECLARE @Carrier varchar(50)
DECLARE @Type varchar(50)
Declare @ClientID int

DECLARE   @ConcatString VARCHAR(4000)
SELECT   @ConcatString =  COALESCE(@ConcatString + ';', '') + fraudprefix FROM Clients 
SELECT distinct Value as Prefix  into #DistinctPrefixes FROM dbo.split ( REPLACE(@ConcatString,';;',';')) where Value <>''

 DECLARE @getID CURSOR
 SET @getID = CURSOR FOR
SELECT distinct r.[ID], r.[CLI], r.[b_number], r.[AttemptDateTime], r.[DurationInSeconds], r.[OriginationNetwork], r.[Carrier], r.[Type], r.[ClientID] FROM RecievedCalls r with(nolock,index=I_GeneratedCallsID)
INNER JOIN #DistinctPrefixes s ON
r.CLI not Like s.Prefix +'%'
where	GeneratedCallID is null and SourceID=1  
order by r.[ID] 
OPEN @getID
FETCH NEXT
FROM @getID INTO @ID ,@CLI ,@b_number ,@AttemptDateTime ,@DurationInSeconds, @OriginationNetwork, @Carrier, @Type, @ClientID
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
		WHERE     (Users.Prefix IS NOT NULL) AND (Users.Prefix <> ';;')  and Users.ClientID=@ClientID     )


  

 
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
           ,4
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