-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_BE].[sp_RecurringChargeDefinition_Insert]
	-- Add the parameters for the stored procedure here
	@ID uniqueidentifier,
	@Name nvarchar(255),
	@Settings nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
IF NOT EXISTS(select 1 from [Retail_BE].[RecurringChargeDefinition] where Name = @Name)
	BEGIN	
		INSERT INTO [Retail_BE].[RecurringChargeDefinition](ID,Name,Settings) 
		VALUES (@ID,@Name, @Settings)
	END
END