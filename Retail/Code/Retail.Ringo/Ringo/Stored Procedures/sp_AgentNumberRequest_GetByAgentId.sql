


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [Ringo].[sp_AgentNumberRequest_GetByAgentId]
@AgnetId bigint
AS
BEGIN

SELECT [Id]
      ,[AgentId]
      ,[Settings]
      ,[Status]
  FROM [Ringo].[AgentNumberRequest] WITH(NOLOCK) 
  where AgentId = @AgnetId
END