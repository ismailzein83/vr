CREATE PROCEDURE [common].[sp_File_Delete] 
	@Id bigint
AS
BEGIN
	delete from  common.[File] 
	where ID=@Id
END