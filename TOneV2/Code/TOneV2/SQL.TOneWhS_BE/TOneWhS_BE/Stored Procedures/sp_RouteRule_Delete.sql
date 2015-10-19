
Create PROCEDURE [TOneWhS_BE].[sp_RouteRule_Delete]
	@ID int
AS
BEGIN

	Delete TOneWhS_BE.RouteRule
	Where ID = @ID
END