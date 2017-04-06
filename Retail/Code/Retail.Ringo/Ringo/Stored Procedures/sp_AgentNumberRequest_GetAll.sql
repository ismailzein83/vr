

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Ringo].[sp_AgentNumberRequest_GetAll]
AS
BEGIN

SELECT [Id]
      ,[AgentId]
      ,[Settings]
      ,[Status]
  FROM [Ringo].[AgentNumberRequest] WITH(NOLOCK) 
  order by id desc
END