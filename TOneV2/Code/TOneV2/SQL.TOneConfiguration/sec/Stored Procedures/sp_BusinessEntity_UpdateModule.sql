-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_BusinessEntity_UpdateModule]
	-- Add the parameters for the stored procedure here
@EntityId uniqueidentifier,
@ModuleId uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	UPDATE	sec.BusinessEntity
		SET		 ModuleID = @ModuleId
		WHERE	Id = @EntityId
END