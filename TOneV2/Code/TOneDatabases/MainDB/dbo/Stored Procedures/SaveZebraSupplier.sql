

CREATE Proc [dbo].[SaveZebraSupplier]
(
		
       @CarrierAccountID VARCHAR(5)
      ,@SupplierName nvarchar(50)=null
      ,@Prefix nvarchar(50)=NULL
      ,@ActivationStatus INT
      ,@LastUpdated DATETIME =null
      ,@UserID INT
      ,@CreationDate DATETIME
)
as
if not exists(select [CarrierAccountID] from [ZebraSupplier] where [CarrierAccountID]=@CarrierAccountID)--do the insertion
begin
		
					insert into [ZebraSupplier] (CarrierAccountID,SupplierName,Prefix,ActivationStatus,LastUpdated,UserID,CreationDate )
					values(@CarrierAccountID,@SupplierName,@Prefix,@ActivationStatus,@LastUpdated,@UserID,@CreationDate)
					return 1--Insertion done succsessful
				end
else
begin
		--update the data
		update [ZebraSupplier]
		set
	
       SupplierName = isnull(@SupplierName,SupplierName)
      ,Prefix = isnull(@Prefix,Prefix)
      ,ActivationStatus = isnull(@ActivationStatus,ActivationStatus)
      ,LastUpdated = isnull(@LastUpdated,LastUpdated)
      ,UserID = isnull(@UserID,UserID)
      
		where CarrierAccountID = @CarrierAccountID
		return 2 --update done succsessfuly
end