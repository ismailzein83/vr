-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SwitchReleaseCause_Insert] 
	@ReleaseCode NVARCHAR(255),
	@ID INT OUT
AS
BEGIN
	INSERT INTO SwitchReleaseCause (ReleaseCode)
	VALUES (@ReleaseCode)
	SET  @ID = scope_identity()
END