-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [common].[sp_Country_UpdateFromSource]
	@ID int,
	@Name nvarchar(255)
AS
BEGIN

	Update common.[Country]
		Set Name = @Name
		Where ID = @ID
END