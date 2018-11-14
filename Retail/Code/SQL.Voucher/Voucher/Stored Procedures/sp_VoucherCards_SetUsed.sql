
CREATE PROCEDURE [Voucher].[sp_VoucherCards_SetUsed]
@PinCode varchar(255),
@UsedBy nvarchar(255),
@LastModifiedBy int
AS
BEGIN
        UPDATE [Voucher].VoucherCards
		SET    UsedBy= @UsedBy ,
		       UsedDate = getdate(),
			   LastModifiedBy = @LastModifiedBy,
			   LastModifiedTime = GETDATE()
		WHERE  PinCode = @PinCode
		       AND UsedBy IS NULL
END