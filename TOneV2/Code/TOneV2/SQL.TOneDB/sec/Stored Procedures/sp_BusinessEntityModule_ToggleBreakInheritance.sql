-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE sec.sp_BusinessEntityModule_ToggleBreakInheritance 
@EntityId int
AS
BEGIN

	UPDATE sec.BusinessEntityModule 
	SET BreakInheritance = 1 - BreakInheritance
	Where Id = @EntityId
END