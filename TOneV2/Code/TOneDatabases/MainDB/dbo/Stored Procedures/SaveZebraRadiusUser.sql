
CREATE Proc [dbo].[SaveZebraRadiusUser]
(
		
       @CarrierAccountID VARCHAR(5)
      ,@ReqID int
      ,@CustomerCreditLimit int
      ,@ActivationStatus INT
      ,@ActivationStatusType INT
      ,@Balance decimal(18, 3)
      ,@LastUpdated DATETIME
      ,@CreationDate DATETIME
)
as
if not exists(select [CarrierAccountID] from [ZebraRadiusUser] where [CarrierAccountID]=@CarrierAccountID)--do the insertion
begin
		
					insert into [ZebraRadiusUser] (CarrierAccountID,RequestID,CustomerCreditLimit,ActivationStatus,ActivationStatusType,Balance,CreationDate,LastUpdated )
					values(@CarrierAccountID,@ReqID,@CustomerCreditLimit,@ActivationStatus,@ActivationStatusType,@Balance, @CreationDate, @LastUpdated)
					return 1--Insertion done succsessful
				end
else
begin
		--update the data
		update [ZebraRadiusUser]
		set
	
       RequestID = isnull(@ReqID,RequestID)
      ,CustomerCreditLimit = isnull(@CustomerCreditLimit,CustomerCreditLimit)
      ,ActivationStatusType = isnull(@ActivationStatusType,ActivationStatusType)
      ,ActivationStatus = isnull(@ActivationStatus,ActivationStatus)
      ,Balance = isnull(@Balance,Balance)
      ,lastUpdated = ISNULL(@lastUpdated,lastUpdated)
      
		where CarrierAccountID = @CarrierAccountID
		return 2 --update done succsessfuly
end