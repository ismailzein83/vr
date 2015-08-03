-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_Mismatch_GetBillingCDRs]
	-- Add the parameters for the stored procedure here
	@QueueCreationDate DATETIME
	,@SwitchID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	;WITH BillingCDRInvalid AS (SELECT 
                                  [CustomerID]
                                  ,[SupplierID]
                                  ,[CDPN]
                                  ,[OurCode]
                                  ,[SupplierCode]
                                  ,MIN(Attempt) MinAttempt
                                  ,MAX(Attempt) MaxAttempt
      
                              FROM [Billing_CDR_Invalid]
	                            WHERE  SwitchID = @SwitchID AND CustomerID IS NOT NULL AND SupplierID IS NOT NULL AND CDPN IS NOT NULL AND SupplierCode IS NOT NULL AND OurCode IS NOT NULL AND Attempt >= @QueueCreationDate
                            GROUP BY CustomerID,SupplierID, CDPN, OurCode, SupplierCode
                            ),
                            BillingCDRMain AS (SELECT 
                                  [CustomerID]
                                  ,[SupplierID]
                                  ,[CDPN]
                                  ,[OurCode]
                                  ,[SupplierCode]
                                  ,MIN(Attempt) MinAttempt
                                  ,MAX(Attempt) MaxAttempt
                              FROM [Billing_CDR_Main]
                              WHERE Attempt >= @QueueCreationDate AND SwitchID = @SwitchID
                              GROUP BY CustomerID,SupplierID, CDPN, OurCode, SupplierCode
                            ),

                            ALLCDRs AS (
                            SELECT * FROM BillingCDRInvalid
                            UNION ALL
                            SELECT * FROM BillingCDRMain	
                            )

                            SELECT c.CustomerID, c.SupplierID, c.CDPN, c.OurCode, c.SupplierCode, MinAttempt, MaxAttempt FROM ALLCDRs c
                            
END