-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [InterConnect_BE].[sp_OperatorAccount_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT	[ID]
      ,[Suffix]
      ,[ProfileID]
      ,[Settings]
	FROM [InterConnect_BE].OperatorAccount
END