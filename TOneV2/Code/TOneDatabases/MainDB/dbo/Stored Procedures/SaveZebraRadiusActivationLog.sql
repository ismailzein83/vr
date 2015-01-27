
CREATE Proc [dbo].[SaveZebraRadiusActivationLog]
(
		
       @CarrierAccountID VARCHAR(5)
      ,@ReqID int
      ,@CustomerCreditLimit int
      ,@ActivationStatus INT
      ,@ActivationStatusType INT
      ,@Balance decimal(18, 3)
      ,@LastUpdated DATETIME
      ,@CreationDate DATETIME
      ,@OrderType INT
      ,@IsApplied BIT
)
as

		
					insert into [RadiusUserActivationLog] (CarrierAccountID,RequestID,CustomerCreditLimit,ActivationStatus,ActivationStatusType,Balance,CreationDate,LastUpdated,OrderType,IsApplied )
					values(@CarrierAccountID,@ReqID,@CustomerCreditLimit,@ActivationStatus,@ActivationStatusType,@Balance,@CreationDate ,@LastUpdated ,@OrderType,@IsApplied)
					return 1--Insertion done succsessful