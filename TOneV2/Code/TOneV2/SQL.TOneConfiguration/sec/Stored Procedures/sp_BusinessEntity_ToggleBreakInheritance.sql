-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_BusinessEntity_ToggleBreakInheritance] 
@EntityId uniqueidentifier
AS
BEGIN

	UPDATE sec.BusinessEntity 
	SET BreakInheritance = 1 - BreakInheritance, LastModifiedTime = GETDATE()
	Where Id = @EntityId
END