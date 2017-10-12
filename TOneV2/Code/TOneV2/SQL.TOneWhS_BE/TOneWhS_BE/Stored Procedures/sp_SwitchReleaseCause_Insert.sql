-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SwitchReleaseCause_Insert] 
@SwitchId INT,
@ReleaseCode NVARCHAR(255),
@Settings NVARCHAR(MAX),
@ID INT OUT
AS
BEGIN
	INSERT INTO SwitchReleaseCause (SwitchId,ReleaseCode,Settings)
	VALUES (@SwitchId,@ReleaseCode,@Settings)
	SET  @ID = scope_identity()
END