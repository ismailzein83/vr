
CREATE PROCEDURE [common].[sp_File_SetUsed]
	-- Add the parameters for the stored procedure here
	@Id INT,
	@IsUsed bit
AS
BEGIN
	
		UPDATE	[common].[File]
		SET		[IsUsed] = @IsUsed
		WHERE	Id = @Id
END