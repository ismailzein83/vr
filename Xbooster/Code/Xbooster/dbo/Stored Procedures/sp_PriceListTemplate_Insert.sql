-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_PriceListTemplate_Insert]
	@Name varchar(255),
	@UserId int,
	@Type VARCHAR(255),
	@ConfigDetails  nvarchar(MAX),
	@ID INT OUT
AS
BEGIN
	IF NOT EXISTS(SELECT NULL FROM dbo.PriceListTemplate WHERE Name = @Name AND UserID = @UserID AND [Type] = @Type)
	BEGIN
		INSERT INTO dbo.PriceListTemplate (Name, UserID, [Type], ConfigDetails)
		VALUES (@Name, @UserId, @Type, @ConfigDetails)
		SET @ID = SCOPE_IDENTITY()
	END
END