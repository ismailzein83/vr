-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_RateType_Insert]
	@Name nvarchar(255),
	@id INT OUT
AS
BEGIN
SET @id =0;
IF NOT EXISTS(SELECT 1 FROM common.[RateType] WHERE Name = @Name)
	BEGIN
		INSERT INTO common.RateType(Name)
		VALUES (@Name)

		SET @id = SCOPE_IDENTITY()
	END
END