-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_Delete]
	@Id INT
AS
BEGIN
	DELETE FROM [integration].[DataSource]
	WHERE ID = @Id
END