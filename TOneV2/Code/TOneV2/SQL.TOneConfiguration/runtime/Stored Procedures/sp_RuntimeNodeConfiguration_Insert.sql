-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_RuntimeNodeConfiguration_Insert] 
	@Id uniqueidentifier ,
	@Name Nvarchar(255),
	@Setting nvarchar(max)
AS
BEGIN
	Insert into runtime.RuntimeNodeConfiguration(Id,[Name],Settings)
	values(@Id,@Name,@Setting)

END