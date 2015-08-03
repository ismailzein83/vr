


CREATE PROCEDURE [dbo].[bp_ErroneousPricedCDR]
(
	@CarrierAccountID varchar(5),
	@IsSale CHAR(1),
	@FromDate DATETIME,
	@TillDate DATETIME
)
WITH RECOMPILE
AS


IF (@IsSale = 'Y' )
BEGIN

	With SaleIDCTE as (
		Select bcs.ID from  Billing_CDR_Sale bcs WITH(NOLOCK)
		WHERE  bcs.Attempt >= @FromDate AND bcs.Attempt <@TillDate
	)	
,BillingMainCTE as (Select bcm.ID
		FROM   Billing_CDR_Main bcm WITH(NOLOCK)
--		FROM   Billing_CDR_Main bcm WITH(NOLOCK,index(IX_Billing_CDR_Main_Attempt,IX_Billing_CDR_Main_Customer))
		WHERE	bcm.Attempt >= @FromDate AND 
				bcm.Attempt <@TillDate AND 
				bcm.CustomerID = @CarrierAccountID )

		Select COUNT(*)
		FROM   BillingMainCTE bcm
		WHERE	not exists(Select SaleIDCTE.ID From SaleIDCTE where SaleIDCTE.id=bcm.id)

END

IF (@IsSale = 'N' )
BEGIN

	With CostIDCTE as 
		(
		Select bcc.ID from  Billing_CDR_Cost bcc WITH(NOLOCK)
		WHERE  bcc.Attempt >= @FromDate AND bcc.Attempt <@TillDate
		)	
		
,BillingMainCTE as (Select bcm.ID
		FROM   Billing_CDR_Main bcm WITH(NOLOCK)
--		FROM   Billing_CDR_Main bcm WITH(NOLOCK,index(IX_Billing_CDR_Main_Attempt,IX_Billing_CDR_Main_Supplier))
		WHERE	bcm.Attempt >= @FromDate AND 
				bcm.Attempt <@TillDate AND 
				bcm.SupplierID = @CarrierAccountID )

		Select COUNT(*)
		FROM   BillingMainCTE bcm
		WHERE	not exists(Select CostIDCTE.ID From CostIDCTE where CostIDCTE.id=bcm.id)
		
		
		

	
End