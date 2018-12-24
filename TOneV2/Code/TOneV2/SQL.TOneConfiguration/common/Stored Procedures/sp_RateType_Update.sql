-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_RateType_Update]
	@ID int,
	@Name nvarchar(255)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM common.[RateType] WHERE ID != @Id AND Name = @Name)
	BEGIN
		Update common.RateType
		Set Name = @Name, LastModifiedTime = getdate()
		Where ID = @ID
	END
END