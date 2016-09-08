
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_Agent_GetAll]
AS
BEGIN
	SELECT	ID, Name, Type, Settings, SourceID
	FROM	Retail.Agent  with(nolock)
END