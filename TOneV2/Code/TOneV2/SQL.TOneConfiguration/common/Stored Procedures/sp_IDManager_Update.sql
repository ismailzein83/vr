
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [common].[sp_IDManager_Update]
	@TypeID int,
	@LastTakenID bigint
AS
BEGIN

	Update common.IDManager
	Set LastTakenID=@LastTakenID
	where TypeID=@TypeID
	
END