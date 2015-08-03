


CREATE Proc [dbo].[SaveZebrapaymentQueue]
(
		
       @CarrierAccountID VARCHAR(5)
      ,@Amount numeric(13, 5)
      ,@CurrencyID varchar(3)
      ,@Tag varchar(255)
      ,@CreationDate datetime
      ,@LastUpdate datetime
      ,@ReferenceNumber varchar(50)
      ,@Note varchar(250)
)
as

		
					insert into [ZebraPaymentQueue] (ZebraCustomerID,Amount,CurrencyID,Tag,CreationDate,LastUpdate,ReferenceNumber,Note )
					values(@CarrierAccountID,@Amount,@CurrencyID,@Tag,@CreationDate,@LastUpdate,@ReferenceNumber,@Note)
					return 1--Insertion done succsessful