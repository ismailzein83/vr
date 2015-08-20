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


 DECLARE @getID CURSOR
 SET @getID = CURSOR FOR
SELECT  [ID],[CLI],[b_number],[AttemptDateTime],[DurationInSeconds],[OriginationNetwork],[Carrier],[Type]
FROM	RecievedCalls   with(nolock,index=I_GeneratedCallsID)
where	GeneratedCallID is null
		and (CLI not like '07%' or CLI not like '09%') and SourceID=1 
order by ID 
OPEN @getID
FETCH NEXT
FROM @getID INTO @ID ,@CLI ,@b_number ,@AttemptDateTime ,@DurationInSeconds, @OriginationNetwork, @Carrier, @Type
WHILE @@FETCH_STATUS = 0
BEGIN


 Declare @RecievedB_number varchar(50)
    
    IF  CHARINDEX('964',@b_number) > 0 
    set @RecievedB_number= RIGHT(@b_number, charindex(REVERSE('964'), REVERSE(@b_number))-1)
    
    IF  CHARINDEX('00964',@b_number) > 0 
    set @RecievedB_number= RIGHT(@b_number, charindex(REVERSE('00964'), REVERSE(@b_number))-1)
    
    IF  CHARINDEX('+964',@b_number) > 0 
    set @RecievedB_number= RIGHT(@b_number, charindex(REVERSE('+964'), REVERSE(@b_number))-1)

	 IF  CHARINDEX('963',@b_number) > 0 
    set @RecievedB_number= RIGHT(@b_number, charindex(REVERSE('963'), REVERSE(@b_number))-1)
    
    IF  CHARINDEX('00963',@b_number) > 0 
    set @RecievedB_number= RIGHT(@b_number, charindex(REVERSE('00963'), REVERSE(@b_number))-1)
    
    IF  CHARINDEX('+963',@b_number) > 0 
    set @RecievedB_number= RIGHT(@b_number, charindex(REVERSE('+963'), REVERSE(@b_number))-1)
    


		DECLARE @MobileOperatorID INT

		set @MobileOperatorID = 3 -- PSTN


		set @MobileOperatorID = (SELECT    top 1 MobileOperators.ID AS MobileOperatorID
		FROM         MobileOperators INNER JOIN
							  Users ON MobileOperators.UserID = Users.ID
		WHERE     (Users.Prefix IS NOT NULL) AND (Users.Prefix <> ';;') AND ((Users.Prefix LIKE '%' + LEFT(@RecievedB_number, 4) + '%')  OR (Users.Prefix LIKE '%' + LEFT(@RecievedB_number, 2) + '%') )         )



  

 
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
FROM @getID INTO @ID  ,@CLI ,@b_number ,@AttemptDateTime ,@DurationInSeconds, @OriginationNetwork, @Carrier, @Type
END
 CLOSE @getID
DEALLOCATE @getID