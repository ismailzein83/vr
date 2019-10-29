﻿CREATE PROCEDURE [TOneWhS_BE].[sp_SellingNumberPlan_Insert]
	@Name nvarchar(255),
	@LOBID uniqueidentifier,
	@CreatedBy int,
	@LastModifiedBy int,
	@id INT OUT
AS

BEGIN
SET @id =0;
IF NOT EXISTS(SELECT 1 FROM TOneWhS_BE.[SellingNumberPlan] WHERE Name = @Name)
	BEGIN
		INSERT INTO TOneWhS_BE.SellingNumberPlan(Name,LOBID, CreatedBy, LastModifiedBy, LastModifiedTime)
		VALUES (@Name,@LOBID, @CreatedBy, @LastModifiedBy, GETDATE())

		SET @id = SCOPE_IDENTITY()
	END
END


/*sp_SellingNumberPlan_GetAll*/