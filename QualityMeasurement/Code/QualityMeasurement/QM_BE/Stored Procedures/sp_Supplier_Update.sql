﻿CREATE PROCEDURE [QM_BE].[sp_Supplier_Update]
	@ID int,
	@Name nvarchar(255),
	@Settings nvarchar(max),
	@IsDeleted bit
AS
BEGIN
IF NOT EXISTS(select 1 from QM_BE.Supplier where Name = @Name and Id!=@ID) 
BEGIN
	Update QM_BE.Supplier
	Set
	 Name = @Name,
	 Settings = @Settings,
	 IsDeleted = @IsDeleted
	Where ID = @ID
END
END