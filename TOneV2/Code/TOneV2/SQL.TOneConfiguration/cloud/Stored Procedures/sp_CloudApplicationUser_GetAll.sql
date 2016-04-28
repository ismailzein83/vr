-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE cloud.sp_CloudApplicationUser_GetAll
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT  [ApplicationID]
      ,[UserID]
      ,[Settings]    
	FROM [cloud].[CloudApplicationUser]
END