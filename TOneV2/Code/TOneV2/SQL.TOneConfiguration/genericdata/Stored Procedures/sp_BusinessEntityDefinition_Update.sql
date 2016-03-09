-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [genericdata].[sp_BusinessEntityDefinition_Update] 
	@ID int ,
	@Name nvarchar(50),
	@Title  nvarchar(50),
	@Settings nvarchar(max)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
IF NOT EXISTS(select 1 from genericdata.BusinessEntityDefinition where Name = @Name and ID!=@ID) 
	BEGIN
	UPDATE	genericdata.BusinessEntityDefinition
	SET	    Name = @Name,
			Title =@Title,
			Settings = @Settings
	WHERE	ID = @ID
	END
END