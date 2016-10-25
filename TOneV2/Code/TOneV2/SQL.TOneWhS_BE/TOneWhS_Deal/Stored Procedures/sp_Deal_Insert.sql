-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
		
CREATE PROCEDURE [TOneWhS_Deal].[sp_Deal_Insert]
	@Name nvarchar(255),
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN
SET @id =0;
IF NOT EXISTS(SELECT 1 FROM TOneWhS_Deal.Deal WHERE [Name] = @Name)
	BEGIN
		Insert into TOneWhS_Deal.Deal(Name,Settings)
		Values(@Name,@Settings)
	
		Set @Id = SCOPE_IDENTITY()
	END
END