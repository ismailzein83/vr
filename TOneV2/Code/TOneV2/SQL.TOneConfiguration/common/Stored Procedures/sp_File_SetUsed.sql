
CREATE PROCEDURE [common].[sp_File_SetUsed]
	-- Add the parameters for the stored procedure here
	@Id BIGINT
AS
BEGIN
	
		UPDATE	[common].[File]
		SET		[IsTemp] = 0
		WHERE	ID = @Id
END