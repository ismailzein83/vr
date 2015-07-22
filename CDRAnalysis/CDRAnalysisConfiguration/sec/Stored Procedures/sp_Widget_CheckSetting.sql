-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Widget_CheckSetting]
	@Setting nvarchar(1000),
	@Id INT
AS
BEGIN

	IF EXISTS  (SELECT 1 FROM sec.Widget WHERE Setting=@Setting AND (Id!=@Id OR @Id IS NULL))
	BEGIN
	 SELECT 1;
	 END
	ELSE
	BEGIN
	 SELECT 0;
	 END
END