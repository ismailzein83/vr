-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_FlaggedService_GetById]
@FlaggedServiceID TINYINT
AS
BEGIN
	SELECT [FlaggedServiceID], [Symbol], [ServiceColor] FROM [FlaggedService]
	WHERE FlaggedServiceID = @FlaggedServiceID
END