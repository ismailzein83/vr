-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_BigDataService_Insert]
	@ServiceURL varchar(1000),
	@RuntimeProcessId int,
	@id bigint OUT
AS
BEGIN
	INSERT INTO common.BigDataService
	(ServiceURL, RuntimeProcessID)
	VALUES
	(@ServiceURL, @RuntimeProcessId)
	
	SET @id = SCOPE_IDENTITY()
END