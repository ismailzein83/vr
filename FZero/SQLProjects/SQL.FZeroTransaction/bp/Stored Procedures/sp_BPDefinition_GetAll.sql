﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE  [bp].[sp_BPDefinition_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   SELECT [ID]
	  ,Name
	  ,Title
      ,[FQTN]
      ,[Config]
  FROM [bp].[BPDefinition] WITH(NOLOCK)
END