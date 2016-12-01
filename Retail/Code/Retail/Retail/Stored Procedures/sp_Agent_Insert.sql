-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Agent_Insert]
	@Name NVARCHAR(255),
	@Type nvarchar(255),
	@Settings NVARCHAR(MAX),
	@SourceID nvarchar(255),
	@ID INT OUT
AS
BEGIN

		INSERT INTO Retail.Agent (Name, Type, Settings,  SourceID)
		VALUES (@Name, @Type, @Settings,@SourceID)
		SET @ID = SCOPE_IDENTITY()

END