
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_IDManager_Update]
	@TypeID int,
	@LastTakenID bigint
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM common.IDManager WHERE TypeID = @TypeID)
	Insert Into common.IDManager (TypeID, LastTakenID) values (@TypeID, @LastTakenID)
	
	Else
	Update common.IDManager
	Set LastTakenID=@LastTakenID
	where TypeID=@TypeID
	
END