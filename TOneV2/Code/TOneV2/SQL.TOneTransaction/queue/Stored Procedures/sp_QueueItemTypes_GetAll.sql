
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueItemTypes_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [ID]
      ,[ItemFQTN]
      ,[Title]
      ,[DefaultQueueSettings]
      ,[CreatedTime]
	FROM [queue].[QueueItemType] WITH(NOLOCK) 
    
END