-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_BusinessEntityModule_UpdateParent]
	-- Add the parameters for the stored procedure here
@ModuleId uniqueidentifier,
@ParentId uniqueidentifier = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	UPDATE	sec.BusinessEntityModule
		SET		 ParentId = @ParentId
		WHERE	Id = @ModuleId
END