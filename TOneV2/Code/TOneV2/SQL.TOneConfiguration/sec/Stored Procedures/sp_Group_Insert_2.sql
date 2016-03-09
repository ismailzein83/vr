﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [sec].[sp_Group_Insert] 
	@Name Nvarchar(255),
	@Description nvarchar(MAX),
	@Id int out
AS
BEGIN
IF NOT EXISTS(select 1 from sec.[Group] where Name = @Name)
	BEGIN
		Insert into [sec].[Group] ([Name], [Description])
		values(@Name, @Description)
		
		SET @Id = @@IDENTITY
	END
END