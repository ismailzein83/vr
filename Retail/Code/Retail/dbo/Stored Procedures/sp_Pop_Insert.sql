-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_Pop_Insert]
	@Name nvarchar(255),
	@Description nvarchar(MAX),
	@Quantity int,
	@Location nvarchar(MAX),
	@Id int out
AS
BEGIN
SET @id =0;
IF NOT EXISTS(SELECT 1 FROM dbo.Pop WHERE [Name] = @Name )
	BEGIN
		Insert into dbo.Pop([Name],[Description],[Quantity],[Location])
		Values(@Name,@Description,@Quantity , @Location)
	
		Set @Id = @@IDENTITY
	END
END