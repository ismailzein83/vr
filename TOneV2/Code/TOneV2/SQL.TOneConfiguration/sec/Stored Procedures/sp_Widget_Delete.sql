-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE sec.sp_Widget_Delete
	@Id INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	Delete from sec.Widget
    where Id = @Id 
END