-- =============================================
-- Author:		Rabih Fashwal
-- Create date: 2014-05-09
-- Description:	Get ExcludedList for Parent Rule without excluded from child
-- =============================================
CREATE PROCEDURE [dbo].[bp_RT_Full_GetExcludedCodesForParent]
	-- Add the parameters for the stored procedure here
	@ParentExcludeList  VARCHAR(4000),
@CodeList VARCHAR(4000),
@Result VARCHAR(4000) OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	CREATE TABLE #TempParentCodes (Code VARCHAR(20))
CREATE TABLE #TempCodes (Code VARCHAR(20))
	
	INSERT INTO #TempParentCodes With(Tablock)
	SELECT * FROM dbo.ParseArray(@ParentExcludeList,',')
	
	INSERT INTO #TempCodes  With(Tablock)
	SELECT * FROM dbo.ParseArray(@CodeList,',')
	
	
	SELECT * FROM #TempCodes tc
	SELECT * FROM #TempParentCodes tpc
	
DECLARE @Code VARCHAR(20)
DECLARE Codes CURSOR LOCAL FOR select Code from #TempCodes
OPEN Codes
FETCH NEXT FROM Codes into @Code
WHILE @@FETCH_STATUS = 0
BEGIN
DELETE FROM #TempParentCodes WHERE Code LIKE @Code

    FETCH NEXT FROM Codes into @Code
END

CLOSE Codes
DEALLOCATE Codes


 
 select @Result = COALESCE(@Result + ',', '') + Code FROM #TempParentCodes

 --DROP TABLE TempParentCodes, TempCodes
END