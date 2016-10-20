-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [integration].[sp_DataSource_Delete]
	@Id uniqueidentifier
AS
BEGIN
	DELETE FROM [integration].[DataSource]
	WHERE ID = @Id
END