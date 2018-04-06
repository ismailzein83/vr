-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE runtime.sp_RuntimeNodeConfiguration_GetAll	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [ID]
      ,[Name]
      ,[Settings]
      ,[timestamp]
      ,[CreatedTime]
      ,[CreatedBy]
      ,[LastModifiedTime]
      ,[LastModifiedBy]
  FROM [runtime].[RuntimeNodeConfiguration] WITH (NOLOCK)

END