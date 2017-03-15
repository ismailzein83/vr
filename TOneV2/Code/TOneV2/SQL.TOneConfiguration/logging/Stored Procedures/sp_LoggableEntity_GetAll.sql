-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [logging].[sp_LoggableEntity_GetAll]
AS
BEGIN
	SELECT	[ID],[UniqueName],[Settings]
	FROM	[logging].[LoggableEntity] WITH(NOLOCK)
END