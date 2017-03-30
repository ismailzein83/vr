

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE Ringo.[sp_AgentNumberRequest_GetAll]
AS
BEGIN

SELECT [Id]
      ,[AgentId]
      ,[Settings]
      ,[Status]
  FROM [Ringo].[AgentNumberRequest] WITH(NOLOCK) 
END