-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE rules.sp_Rule_Delete 
	@ID INT
AS
BEGIN
	DELETE FROM [rules].[Rule]
	WHERE ID=@ID

END