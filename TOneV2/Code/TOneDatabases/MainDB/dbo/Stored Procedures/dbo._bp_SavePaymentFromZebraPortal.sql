
CREATE Proc [dbo].[dbo.[bp_SavePaymentFromZebraPortal]
(
		
			@PaymentID int
	
           ,@CustomerID VARCHAR(5)
           ,@Amount numeric(13, 5)
           ,@CurrencyID varchar(3)
           ,@Date DATETIME
           ,@Type smallint
           
           ,@Tag varchar(255)
           ,@LastUpdate DATETIME
           ,@ReferenceNumber varchar(250)
           ,@Note varchar(250)
)
as

		
				begin
					--add new record
					insert into PrepaidAmount ([CustomerID],[Amount],[CurrencyID],[Date],[Type],[Tag],[LastUpdate],[ReferenceNumber],[Note])
					values(@CustomerID,@Amount,@CurrencyID,@Date,@Type,@Tag,@LastUpdate,@ReferenceNumber,@Note)
					return SCOPE_IDENTITY()--Insertion done succsessful
				end