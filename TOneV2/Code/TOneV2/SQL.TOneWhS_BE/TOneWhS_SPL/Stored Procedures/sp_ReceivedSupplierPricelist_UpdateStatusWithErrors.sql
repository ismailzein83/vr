﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>

-- =============================================
CREATE PROCEDURE [TOneWhS_SPL].[sp_ReceivedSupplierPricelist_UpdateStatusWithErrors]
	@ReceivedPricelistId int,
	@Status smallint,
	@serializedErrors nvarchar(max)
AS
BEGIN
		UPDATE [TOneWhS_SPL].ReceivedSupplierPricelist 
		SET [Status] = @Status,
		[ErrorDetails] = @serializedErrors
		WHERE ID = @ReceivedPricelistId and [Status] < 50
END