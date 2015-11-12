-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_IDManager_ReserveIDRange]
	@TypeID int,
	@NumberOfIDs int
AS
BEGIN

	INSERT INTO common.IDManager WITH (TABLOCK) 
	(TypeID, [LastTakenID]) 
	SELECT @TypeID, 0 
	WHERE NOT EXISTS (SELECT TOP 1 NULL FROM common.IDManager WHERE TypeID = @TypeID)

	DECLARE @StartingID bigint
	
	UPDATE common.IDManager
	SET @StartingID = LastTakenID + 1,
		LastTakenID =  LastTakenID + @NumberOfIDs	
	WHERE TypeID = @TypeID
		
	SELECT @StartingID
	
END