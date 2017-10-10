-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SwitchReleaseCause_Update] 
@ID INT,
@ReleaseCode NVARCHAR(255)
AS
BEGIN
Update [TOneWhS_BE].SwitchReleaseCause
	SET ReleaseCode = @ReleaseCode
		
	WHERE ID = @ID
END