-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ApplicationParameter_Update]
	@Id INT,
	@Value INT
AS
BEGIN
	SET NOCOUNT ON;
	
	UPDATE [dbo].[ApplicationParameter]
	SET Value = @Value
	WHERE Id = @Id
END