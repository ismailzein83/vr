-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [QM_CLITester].[sp_TestCall_GetTotalCallsByUserId]
	@UserId INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT

	convert(varchar, [CreationDate], 101) AS CreationDate,
	COUNT(*) AS TotalCalls
	FROM	[QM_CLITester].[TestCall]  WITH(NOLOCK) 
	WHERE 
	UserID = @UserId  
	
	GROUP BY 
	 convert(varchar, [CreationDate], 101)
	 Order by CreationDate
END