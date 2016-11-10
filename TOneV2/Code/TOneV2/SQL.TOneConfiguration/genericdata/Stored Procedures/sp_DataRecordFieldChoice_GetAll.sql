

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataRecordFieldChoice_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	drfc.[ID],drfc.[Name],drfc.[Settings]
    FROM	[genericdata].DataRecordFieldChoice drfc WITH(NOLOCK) 
	ORDER BY drfc.[Name]
END