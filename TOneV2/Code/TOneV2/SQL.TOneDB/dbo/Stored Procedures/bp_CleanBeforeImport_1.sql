CREATE PROCEDURE [dbo].[bp_CleanBeforeImport]

AS
BEGIN

	TRUNCATE TABLE Rate
	TRUNCATE TABLE PlaningRate
	-- TRUNCATE TABLE PricelistImportOption
	TRUNCATE TABLE CarrierDocument
	TRUNCATE TABLE CodeMatch
	TRUNCATE TABLE ZoneMatch
	TRUNCATE TABLE ZoneRate
	TRUNCATE TABLE CodeSupply	
	TRUNCATE TABLE Billing_CDR_Cost 
	TRUNCATE TABLE Billing_CDR_Sale
	TRUNCATE TABLE Billing_CDR_Invalid	
	TRUNCATE TABLE Billing_Stats
	TRUNCATE TABLE Billing_Invoice_Details
	TRUNCATE TABLE Billing_Invoice_Costs	
	TRUNCATE TABLE PricingTemplatePlan
	TRUNCATE TABLE Commission
	TRUNCATE TABLE SpecialRequest
	TRUNCATE TABLE PricelistImportOption
	TRUNCATE TABLE CarrierAccountConnection
	TRUNCATE TABLE Tariff
	TRUNCATE TABLE ToDConsideration
	TRUNCATE TABLE RouteBlock
	TRUNCATE TABLE RouteOption
	TRUNCATE TABLE [Route]
	TRUNCATE TABLE TrafficStats
	TRUNCATE TABLE [SystemMessage]	
	TRUNCATE TABLE StateBackup
	-- TRUNCATE TABLE [SystemParameter]
	-- TRUNCATE TABLE CDR
	-- Update Switch Set LastCDRImportTag = 0

	--TRUNCATE TABLE FaultTicketTests

    TRUNCATE TABLE Billing_CDR_Main
    UPDATE SystemParameter SET NumericValue=0 WHERE name LIKE 'sys_CDR_Pricing_CDRID'

	DELETE FROM Billing_Invoice
	DBCC CHECKIDENT ('Billing_Invoice', RESEED, 1)
	DELETE FROM Code
	DBCC CHECKIDENT ('Code', RESEED, 1)
	DELETE FROM Pricelist
	DBCC CHECKIDENT ('PriceList', RESEED, 1)
	DELETE FROM RatePlan
	DBCC CHECKIDENT ('RatePlan', RESEED, 1)
	DELETE FROM Zone
	DBCC CHECKIDENT ('Zone', RESEED, 1)
	DELETE FROM CarrierAccount
	DELETE FROM CarrierProfile
	DBCC CHECKIDENT ('CarrierProfile', RESEED, 1)	
	DELETE FROM FaultTicketUpdate
	DBCC CHECKIDENT ('FaultTicketUpdate', RESEED, 1)
	DELETE FROM FaultTicket
	DBCC CHECKIDENT ('FaultTicket', RESEED, 1)

END