-- =============================================
-- Author:		Fadi Chamieh
-- Create date: 2007-12-17
-- Description:	Check if routing is running 
-- =============================================
CREATE PROCEDURE [dbo].[bp_IsRouteBuildRunning](@IsRunning char(1) output)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Message varchar(500)
	SELECT @IsRunning = 'Y' FROM SystemMessage WHERE MessageID = 'BuildRoutes: Status' AND [Message] IS NOT NULL
	SET @IsRunning = ISNULL(@IsRunning, 'N')
	RETURN 
END