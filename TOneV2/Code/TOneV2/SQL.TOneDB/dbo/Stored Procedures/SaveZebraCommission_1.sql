


CREATE Proc [dbo].[SaveZebraCommission]
(
		    @CommissionID INT
       	   ,@SupplierID varchar(5)
           ,@FromRate decimal(9,5)
           ,@ToRate decimal(9,5)
           ,@IsPercentage bit
           ,@Amount decimal(9,5)
           ,@BeginEffectiveDate smalldatetime
           ,@EndEffectiveDate smalldatetime
           ,@IsActive bit
           ,@UserID int
           ,@CreationDate smalldatetime
           ,@LastUpdated smalldatetime
)
as
if not exists(select [CommissionID] from [ZebraCommission] where [CommissionID] = @CommissionID)--do the insertion
begin
		
					insert into ZebraCommission ([SupplierID],[FromRate],[ToRate],[IsPercentage],[Amount],[BeginEffectiveDate],[EndEffectiveDate],[IsActive],[UserID],[CreationDate],[LastUpdated] )
					values(@SupplierID,@FromRate,@ToRate,@IsPercentage,@Amount,@BeginEffectiveDate,@EndEffectiveDate,@IsActive,@UserID,@CreationDate,@LastUpdated)
					return SCOPE_IDENTITY()
				end
else
begin
		--update the data
		update [ZebraCommission]
		set
	
       EndEffectiveDate = isnull(@EndEffectiveDate,EndEffectiveDate)
      ,LastUpdated = isnull(@LastUpdated,LastUpdated)
      ,IsActive = isnull(@IsActive,IsActive)
      
		where [CommissionID] = @CommissionID
		return 2 --update done succsessfuly
end