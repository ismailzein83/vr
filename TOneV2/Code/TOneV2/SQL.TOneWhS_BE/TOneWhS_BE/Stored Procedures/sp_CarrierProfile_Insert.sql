﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CarrierProfile_Insert]
	@Name nvarchar(255),
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN
IF NOT EXISTS(select 1 from  TOneWhS_BE.CarrierProfile where Name = @Name)
BEGIN
	Insert into TOneWhS_BE.CarrierProfile([Name],[Settings])
	Values(@Name,@Settings)
	
	Set @Id = @@IDENTITY
	END
END