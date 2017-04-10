
CREATE PROCEDURE [Retail_BE].[sp_DID_Insert]
	@Number varchar(50),
	@Settings nvarchar(MAX),
	@SourceID nvarchar(50),
	@Id int out
AS
BEGIN
IF NOT EXISTS(select 1 from [Retail_BE].DID where Number = @Number)
	BEGIN
		Insert into [Retail_BE].DID ([Number], [Settings], [SourceID])
		values(@Number, @Settings, @SourceID)
		
		SET @Id = SCOPE_IDENTITY()
	END
END