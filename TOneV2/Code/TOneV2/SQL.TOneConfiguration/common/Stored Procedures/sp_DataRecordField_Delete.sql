create PROCEDURE [common].[sp_DataRecordField_Delete]
	@Id INT
AS
BEGIN
	DELETE FROM [common].DataRecordField
    WHERE Id = @Id 
END