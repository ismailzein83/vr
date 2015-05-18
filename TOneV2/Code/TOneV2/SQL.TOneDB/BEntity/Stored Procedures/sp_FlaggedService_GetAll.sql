-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_FlaggedService_GetAll]
AS
BEGIN
	SELECT [FlaggedServiceID], [Symbol] FROM [FlaggedService]
END