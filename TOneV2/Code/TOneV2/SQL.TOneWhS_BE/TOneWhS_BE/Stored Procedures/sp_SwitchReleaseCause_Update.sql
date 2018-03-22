-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SwitchReleaseCause_Update] 
@ID INT,
@SwitchId INT,
@ReleaseCode NVARCHAR(255),
@Settings NVARCHAR(MAX),
@SourceID NVARCHAR(50),
@LastModifiedBy int
AS
BEGIN
	IF NOT EXISTS(select 1 from [TOneWhS_BE].SwitchReleaseCause WHERE ReleaseCode = @ReleaseCode and SwitchId = @SwitchId and Id!=@ID) 
	BEGIN
		Update [TOneWhS_BE].SwitchReleaseCause
		SET ReleaseCode = @ReleaseCode,
			SwitchId = @SwitchId,
			Settings = @Settings,
			SourceID = @SourceID,
			LastModifiedBy = @LastModifiedBy,
			LastModifiedTime = GETDATE()
		WHERE ID = @ID
	END
END