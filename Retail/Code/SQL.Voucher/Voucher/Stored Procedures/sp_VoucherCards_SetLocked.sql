
create PROCEDURE [Voucher].[sp_VoucherCards_SetLocked]
@PinCode varchar(255),
@LockedBy nvarchar(255),
@LastModifiedBy int
AS
BEGIN
        UPDATE [Voucher].VoucherCards
		SET    LockedBy= @LockedBy ,
		       LockedDate = getdate(),
			   LastModifiedBy = @LastModifiedBy,
			   LastModifiedTime = GETDATE()
		WHERE  PinCode = @PinCode
		       AND LockedBy IS NULL
END