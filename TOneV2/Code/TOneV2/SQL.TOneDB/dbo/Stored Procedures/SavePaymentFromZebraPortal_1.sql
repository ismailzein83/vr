

CREATE Proc [dbo].[SavePaymentFromZebraPortal]
(
	        @CustomerID VARCHAR(5)
           ,@Amount numeric(13, 5)
           ,@CurrencyID varchar(3)
           ,@Date DATETIME          
           ,@Tag varchar(255)
           ,@LastUpdate DATETIME
           ,@ReferenceNumber varchar(250)
           ,@Note varchar(250)
)
as

		
				begin
					--add new record
					insert into PrepaidAmount ([CustomerID],[Amount],[CurrencyID],[Date],[Tag],[LastUpdate],[ReferenceNumber],[Note])
					values(@CustomerID,@Amount,@CurrencyID,@Date,@Tag,@LastUpdate,@ReferenceNumber,@Note)
					return SCOPE_IDENTITY()--Insertion done succsessful
				end