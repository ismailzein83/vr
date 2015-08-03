-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ApplicationParameter_GetById]
	@ParameterId INT
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT Value FROM [dbo].[ApplicationParameter]
	WHERE Id = @ParameterId
END