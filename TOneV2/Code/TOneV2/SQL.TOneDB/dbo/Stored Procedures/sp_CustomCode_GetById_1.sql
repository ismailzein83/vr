-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_CustomCode_GetById] 
@ID    int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
	SELECT TOP 1000 [ID]
      ,[Description]
      ,[FQTN]
      ,[Code]
      ,[CreatedTime]
	FROM [dbo].[CustomCode]
	where [dbo].[CustomCode].[ID] = @ID
	
END