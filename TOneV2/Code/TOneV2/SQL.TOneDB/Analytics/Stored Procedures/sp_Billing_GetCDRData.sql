CREATE procedure [Analytics].[sp_Billing_GetCDRData]
@fromDate datetime,
@toDate datetime, 
@nRecords int,
@CDROption varchar(10)
AS
BEGIN	
if (@CDROption='All')
    Begin
            SELECT TOP (@NRecords) * FROM Billing_CDR_Main,Billing_CDR_Invalid where  Billing_CDR_Main.Attempt between @FromDate AND @ToDate And Billing_CDR_Invalid.Attempt between @FromDate AND @ToDate 	
    End
Else if(@CDROption='Successful')
    Begin
           SELECT TOP (@NRecords) * FROM Billing_CDR_Main where  Attempt between @FromDate AND @ToDate	
    End 
Else if(@CDROption='Failed')
    Begin
           SELECT TOP (@NRecords) * FROM Billing_CDR_Invalid where  Attempt between @FromDate AND @ToDate	
    End    
			
END