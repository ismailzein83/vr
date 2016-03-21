-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE sec.sp_BusinessEntity_UpdateModule
	-- Add the parameters for the stored procedure here
@EntityId INT,
@ModuleId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	UPDATE	sec.BusinessEntity
		SET		 ModuleID = @ModuleId
		WHERE	Id = @EntityId
END