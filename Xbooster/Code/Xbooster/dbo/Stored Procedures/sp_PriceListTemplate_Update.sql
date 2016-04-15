-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_PriceListTemplate_Update]
	@ID INT,
	@Name varchar(255),
	@UserId int,
	@Type VARCHAR(255),
	@ConfigDetails  nvarchar(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT NULL FROM dbo.PriceListTemplate WHERE ID != @ID AND Name = @Name AND UserID = @UserID AND [Type] = @Type )
	BEGIN
		UPDATE dbo.PriceListTemplate
		SET Name = @Name, UserID = @UserID, [Type] = @Type, ConfigDetails = @ConfigDetails
		WHERE ID = @ID
	END
END