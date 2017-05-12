
CREATE PROCEDURE [Retail_BE].[sp_DID_Insert]
	@Settings nvarchar(MAX),
	@SourceID nvarchar(50),
	@Id int out
AS
BEGIN
	Insert into [Retail_BE].DID ([Settings], [SourceID])
	values(@Settings, @SourceID)
		
	SET @Id = SCOPE_IDENTITY()
END