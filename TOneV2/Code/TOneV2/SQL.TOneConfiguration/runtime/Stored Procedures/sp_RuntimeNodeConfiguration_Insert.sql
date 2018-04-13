-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [runtime].[sp_RuntimeNodeConfiguration_Insert] 
	@Id uniqueidentifier ,
	@Name Nvarchar(255)
AS
BEGIN
	Insert into runtime.RuntimeNodeConfiguration(Id,[Name])
	values(@Id,@Name)

END