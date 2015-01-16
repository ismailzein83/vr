USE [TABS]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 03/30/2011 15:31:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Role](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [ntext] NULL,
	[IsActive] [char](1) NOT NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Enumerations]    Script Date: 03/30/2011 15:31:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Enumerations](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Enumeration] [nvarchar](120) NOT NULL,
	[Name] [nvarchar](120) NOT NULL,
	[Value] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PrepaidAmount]    Script Date: 03/30/2011 15:31:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PrepaidAmount](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerProfileID] [smallint] NULL,
	[CustomerID] [varchar](10) NULL,
	[SupplierID] [varchar](10) NULL,
	[SupplierProfileID] [smallint] NULL,
	[Amount] [numeric](13, 5) NULL,
	[CurrencyID] [varchar](3) NULL,
	[Date] [datetime] NULL,
	[Type] [smallint] NULL,
	[UserID] [int] NULL,
	[Tag] [varchar](255) NULL,
	[timestamp] [timestamp] NOT NULL,
	[LastUpdate] [datetime] NULL,
	[ReferenceNumber] [varchar](250) NULL,
	[Note] [varchar](250) NULL,
 CONSTRAINT [PK_PrepaidAmount] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_PrepaidAmount_Date] ON [dbo].[PrepaidAmount] 
(
	[Date] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PrepaidAmount_Type] ON [dbo].[PrepaidAmount] 
(
	[Type] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Billing_Stats]    Script Date: 03/30/2011 15:31:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Billing_Stats](
	[CallDate] [smalldatetime] NOT NULL,
	[CustomerID] [varchar](5) NOT NULL,
	[SupplierID] [varchar](5) NOT NULL,
	[CostZoneID] [int] NOT NULL,
	[SaleZoneID] [int] NOT NULL,
	[Cost_Currency] [varchar](3) NULL,
	[Sale_Currency] [varchar](3) NULL,
	[NumberOfCalls] [int] NULL,
	[FirstCallTime] [char](6) NULL,
	[LastCallTime] [char](6) NULL,
	[MinDuration] [numeric](13, 4) NULL,
	[MaxDuration] [numeric](13, 4) NULL,
	[AvgDuration] [numeric](13, 4) NULL,
	[Cost_Nets] [float] NULL,
	[Cost_Discounts] [float] NULL,
	[Cost_Commissions] [float] NULL,
	[Cost_ExtraCharges] [float] NULL,
	[Sale_Nets] [float] NULL,
	[Sale_Discounts] [float] NULL,
	[Sale_Commissions] [float] NULL,
	[Sale_ExtraCharges] [float] NULL,
	[SaleDuration] [numeric](13, 4) NULL,
	[Sale_Rate] [float] NULL,
	[Cost_Rate] [float] NULL,
	[Sale_RateType] [tinyint] NULL,
	[Cost_RateType] [tinyint] NULL,
	[CostDuration] [numeric](13, 4) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE CLUSTERED INDEX [IX_Billing_Stats_Date] ON [dbo].[Billing_Stats] 
(
	[CallDate] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_Stats_Customer] ON [dbo].[Billing_Stats] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_Stats_Supplier] ON [dbo].[Billing_Stats] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HeartBeat]    Script Date: 03/30/2011 15:31:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HeartBeat](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[date] [datetime] NULL,
	[TIMESTAMP] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemParameter]    Script Date: 03/30/2011 15:31:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SystemParameter](
	[Name] [varchar](50) NOT NULL,
	[Type] [tinyint] NOT NULL,
	[BooleanValue] [char](1) NULL,
	[NumericValue] [decimal](18, 8) NULL,
	[TimeSpanValue] [varchar](50) NULL,
	[DateTimeValue] [datetime] NULL,
	[TextValue] [nvarchar](max) NULL,
	[LongTextValue] [ntext] NULL,
	[Description] [ntext] NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TrafficStats]    Script Date: 03/30/2011 15:31:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TrafficStats](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[SwitchId] [tinyint] NOT NULL,
	[Port_IN] [varchar](21) NULL,
	[Port_OUT] [varchar](21) NULL,
	[CustomerID] [varchar](10) NULL,
	[OurZoneID] [int] NULL,
	[OriginatingZoneID] [int] NULL,
	[SupplierID] [varchar](10) NULL,
	[SupplierZoneID] [int] NULL,
	[FirstCDRAttempt] [datetime] NOT NULL,
	[LastCDRAttempt] [datetime] NOT NULL,
	[Attempts] [int] NOT NULL,
	[DeliveredAttempts] [int] NOT NULL,
	[SuccessfulAttempts] [int] NOT NULL,
	[DurationsInSeconds] [numeric](19, 5) NOT NULL,
	[PDDInSeconds] [numeric](19, 5) NULL,
	[MaxDurationInSeconds] [numeric](19, 5) NULL,
	[UtilizationInSeconds] [numeric](19, 5) NULL,
	[NumberOfCalls] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE CLUSTERED INDEX [IX_TrafficStats_DateTimeFirst] ON [dbo].[TrafficStats] 
(
	[FirstCDRAttempt] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TrafficStats_Customer] ON [dbo].[TrafficStats] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TrafficStats_ID] ON [dbo].[TrafficStats] 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TrafficStats_Supplier] ON [dbo].[TrafficStats] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TrafficStats_Zone] ON [dbo].[TrafficStats] 
(
	[OurZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The input IP:Port or Trunk ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TrafficStats', @level2type=N'COLUMN',@level2name=N'Port_IN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The output IP:Port or Trunk ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TrafficStats', @level2type=N'COLUMN',@level2name=N'Port_OUT'
GO
/****** Object:  UserDefinedFunction [dbo].[IsToDActive]    Script Date: 03/30/2011 15:31:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Fadi Chamieh
-- Create date: 12/09/2007
-- Description:	Determine if the Given ToD parameters are active
-- =============================================
CREATE FUNCTION [dbo].[IsToDActive] 
(
	-- Add the parameters for the function here
	@HolidayDate smalldatetime,
	@WeekDay tinyint,
	@BeginTime varchar(12),
	@EndTime varchar(12),	
	@When datetime
)
RETURNS char(1)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result char(1)
	
	-- Get the time specified by @When in the format: hh:mm:ss.fff
	DECLARE @Time varchar(50) 
	SELECT @Time = Right(CONVERT(varchar(50), @when, 121), 12)

	SET @Result = 'Y'

	-- Check Holiday	
	IF @HolidayDate IS NOT NULL AND (Month(@When)<>Month(@HolidayDate) OR Day(@When)<>Day(@HolidayDate)) 
		SET @Result = 'N'

	-- Check Weekday (Weekday stored (from .NET is 0 based)
	IF @WeekDay IS NOT NULL AND ((datepart(weekday,@When)-1) <> @WeekDay) 
		SET @Result = 'N'

	-- Check Begin and End Times
	IF @BeginTime IS NOT NULL AND NOT (@Time BETWEEN @BeginTime AND @EndTime) 
		SET @Result = 'N'

	-- Return the result of the function
	RETURN @Result

END
GO
/****** Object:  Table [dbo].[PersistedCustomReport]    Script Date: 03/30/2011 15:31:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PersistedCustomReport](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](512) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NULL,
	[RequiredPermissionID] [varchar](50) NULL,
	[UserID] [int] NULL,
	[Csharp_Code] [ntext] NULL,
	[IsEncrypted] [char](1) NOT NULL,
	[timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK__PersistedCustomR__0C66AE13] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CarrierGroup]    Script Date: 03/30/2011 15:31:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CarrierGroup](
	[CarrierGroupID] [int] IDENTITY(1,1) NOT NULL,
	[CarrierGroupName] [nvarchar](255) NOT NULL,
	[ParentID] [int] NULL,
	[ParentPath] [varchar](255) NULL,
	[Path]  AS (case when [parentID] IS NULL then [CarrierGroupName] else ([ParentPath]+'/')+[CarrierGroupName] end),
 CONSTRAINT [PK_CarrierGroup] PRIMARY KEY CLUSTERED 
(
	[CarrierGroupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_CarrierGroup] ON [dbo].[CarrierGroup] 
(
	[ParentPath] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LookupType]    Script Date: 03/30/2011 15:31:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LookupType](
	[LookupTypeID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_LookupType] PRIMARY KEY CLUSTERED 
(
	[LookupTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[EA_PrepaidCarrierTotal]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ======================================================================
-- Author: Mohammad El-Shab
-- Description: 
-- ======================================================================
CREATE PROCEDURE [dbo].[EA_PrepaidCarrierTotal]
    @ShowCustomerTotal char(1) = 'Y',
	@ShowSupplierTotal char(1) = 'Y',
	@CarrierAccountID  varchar(5) = NULL,
	@CarrierProfileID  int 
AS
BEGIN
	SET NOCOUNT ON;
	IF @ShowCustomerTotal = 'Y'
		SELECT     
			PA.CustomerID AS CarrierID,
			PA.CustomerProfileID AS ProfileID,
			SUM(PA.Amount) AS Balance,
			PA.CurrencyID AS Currency,
			(case WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END) AS CreditLimit,
			SUM(PA.Amount) - ISNULL((CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END),0) AS Tolerance
		FROM
			PrepaidAmount PA 
				LEFT JOIN CarrierAccount CA ON CA.CarrierAccountID = PA.CustomerID
				LEFT JOIN CarrierProfile CP ON CP.ProfileID = PA.CustomerProfileID  
		WHERE
			PA.CustomerID = @CarrierAccountID OR PA.CustomerProfileID = @CarrierProfileID
		GROUP BY	
			PA.CustomerProfileID,
			PA.CustomerID,
			PA.CurrencyID, 
			(CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END)
		ORDER BY Tolerance ASC
	IF @ShowSupplierTotal = 'Y'
		SELECT
		     PA.SupplierID AS CarrierID,
			 PA.SupplierProfileID AS ProfileID,
			 SUM(PA.Amount) AS Balance,
			 PA.CurrencyID AS Currency,
			 (CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierCreditLimit ELSE CP.SupplierCreditLimit END) AS CreditLimit,
			 SUM(PA.Amount) - ISNULL((CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierCreditLimit ELSE CP.SupplierCreditLimit END),0) AS Tolerance
		FROM 
			PrepaidAmount PA 
		    LEFT JOIN CarrierAccount CA ON CA.CarrierAccountID = PA.SupplierID
			LEFT JOIN CarrierProfile CP ON CP.ProfileID = PA.CustomerProfileID  
		WHERE
		    PA.SupplierID = @CarrierAccountID OR PA.SupplierProfileID = @CarrierProfileID 
		GROUP BY   
			PA.SupplierProfileID,
			PA.SupplierID,
			PA.CurrencyID, 
			(CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierCreditLimit ELSE CP.SupplierCreditLimit END)		
		ORDER BY Tolerance ASC
END
GO
/****** Object:  Table [dbo].[Alert]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Alert](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Created] [datetime] NULL,
	[Source] [varchar](255) NULL,
	[Level] [smallint] NULL,
	[Progress] [smallint] NULL,
	[Tag] [varchar](255) NULL,
	[Description] [varchar](max) NULL,
	[IsVisible] [char](1) NULL,
 CONSTRAINT [PK_Alert] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Alert_Date] ON [dbo].[Alert] 
(
	[Created] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Alert_Source] ON [dbo].[Alert] 
(
	[Source] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[ConflictsWithOtherInvoices]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =====================================================================================
-- Author:		Fadi Chamieh
-- Create date: 14/11/2007
-- Description:	Check if there are other Invoices for a Customer in the specified dates
-- =====================================================================================
CREATE FUNCTION [dbo].[ConflictsWithOtherInvoices]
(
	@InvoiceID int,
	@CustomerID varchar(10),
	@SupplierID varchar(10),
	@BeginDate smalldatetime,
	@EndDate smalldatetime
)
RETURNS char(1)
AS
BEGIN
	SET @InvoiceID = ISNULL(@InvoiceID, 0)
	RETURN 
		CASE 
			WHEN EXISTS (
					SELECT * FROM Billing_Invoice BI 
						WHERE BI.InvoiceID <> @InvoiceID 
							AND CustomerID = @CustomerID
							AND SupplierID = @SupplierID
							AND (
									BI.BeginDate BETWEEN @BeginDate AND EndDate 
									OR 
									BI.EndDate BETWEEN @BeginDate AND EndDate 
								)
							)
				THEN 'Y'
			ELSE 'N'
		END
END
GO
/****** Object:  Table [dbo].[ReleaseCode]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReleaseCode](
	[Code] [int] NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ReleaseCode] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SwitchHistory]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SwitchHistory](
	[Date] [datetime] NULL,
	[SwitchID] [tinyint] NULL,
	[Symbol] [varchar](10) NULL,
	[Name] [varchar](512) NULL,
	[Description] [text] NULL,
	[Configuration] [ntext] NULL,
	[LastCDRImportTag] [varchar](255) NULL,
	[LastImport] [datetime] NULL,
	[LastRouteUpdate] [datetime] NULL,
	[UserID] [int] NULL,
	[Enable_CDR_Import] [char](1) NOT NULL,
	[Enable_Routing] [char](1) NOT NULL,
	[LastAttempt] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_SwitchHistory] ON [dbo].[SwitchHistory] 
(
	[Date] ASC,
	[SwitchID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RouteOverride]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RouteOverride](
	[CustomerID] [varchar](5) NOT NULL,
	[Code] [varchar](15) NOT NULL,
	[IncludeSubCodes] [char](1) NOT NULL,
	[OurZoneID] [int] NOT NULL,
	[RouteOptions] [varchar](100) NULL,
	[BlockedSuppliers] [varchar](1024) NULL,
	[BeginEffectiveDate] [smalldatetime] NOT NULL,
	[EndEffectiveDate] [smalldatetime] NOT NULL,
	[IsEffective]  AS (case when getdate()>=[BeginEffectiveDate] AND getdate()<[EndEffectiveDate] then 'Y' else 'N' end),
	[Weight]  AS ((case when [Code]<>'*ALL*' then (10) else (0) end+case when [OurZoneID]<>(0) then (1) else (0) end)+case when [CustomerID]<>'*ALL*' then (100) else (0) end) PERSISTED,
	[UserID] [int] NULL,
	[Updated] [smalldatetime] NULL,
 CONSTRAINT [PK_RouteOverride] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[Code] ASC,
	[OurZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Code] ON [dbo].[RouteOverride] 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Customer] ON [dbo].[RouteOverride] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RouteOverride_Begin] ON [dbo].[RouteOverride] 
(
	[BeginEffectiveDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Zone] ON [dbo].[RouteOverride] 
(
	[OurZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_LastMonthCDR]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_LastMonthCDR]
AS
SELECT  
	SwitchID, 
	IDonSwitch, 
	Tag, 
	AttemptDateTime, 
	ConnectDateTime, 
	DisconnectDateTime, 
	DurationInSeconds, 
	IN_TRUNK,
	IN_CIRCUIT, 
	IN_CARRIER, 
	IN_IP, 
	OUT_TRUNK, 
	OUT_CIRCUIT, 
	OUT_CARRIER, 
	OUT_IP, 
	CGPN, 
	CDPN, 
	CAUSE_FROM_RELEASE_CODE, 
	CAUSE_FROM, 
	CAUSE_TO_RELEASE_CODE, 
	CAUSE_TO
FROM
	dbo.CDR WITH(NOLOCK)
WHERE 
	AttemptDateTime >= cast(convert(varchar(7),getdate(),121)+'-01' as datetime)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "CDR"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 193
               Right = 269
            End
            DisplayFlags = 280
            TopColumn = 13
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_LastMonthCDR'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_LastMonthCDR'
GO
/****** Object:  UserDefinedFunction [dbo].[ParseArrayWithPosition]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ParseArrayWithPosition](
	@Array VARCHAR(8000),
	@separator CHAR(1)
)
RETURNS @T TABLE ([value] varchar(100), Position INT)
AS
BEGIN
    
    DECLARE @Position INT
    
    SET @Position = 1
    
    -- @Array is the array we wish to parse
    -- @Separator is the separator charactor such as a comma
    DECLARE @separator_position INT -- This is used to locate each separator character
    DECLARE @array_value VARCHAR(1000) -- this holds each array value as it is returned
    -- For my loop to work I need an extra separator at the end. I always look to the
    -- left of the separator character for each array value
    
    SET @array = @array + @separator
    
    -- Loop through the string searching for separtor characters
    WHILE PATINDEX('%' + @separator + '%', @array) <> 0
    BEGIN
        -- patindex matches the a pattern against a string
        SELECT @separator_position = PATINDEX('%' + @separator + '%',@array)
        SELECT @array_value = LEFT(@array, @separator_position - 1)
        -- This is where you process the values passed.
        INSERT INTO @T
        VALUES
        (
            @array_value, @Position
        )
        SET @Position = @Position + 1
        -- Replace this select statement with your processing
        -- @array_value holds the value of this element of the array
        -- This replaces what we just processed with and empty string
        SELECT @array = STUFF(@array, 1, @separator_position, '')
    END
    RETURN
END
GO
/****** Object:  Table [dbo].[PricelistImportOption]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PricelistImportOption](
	[SupplierID] [varchar](10) NOT NULL,
	[ImporterName] [varchar](255) NULL,
	[ImportParameters] [ntext] NULL,
	[LastUpdate] [smalldatetime] NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_PricelistImportOption] PRIMARY KEY CLUSTERED 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_TopNDestination]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TrafficStats_TopNDestination]
 @TopRecord    INT,
 @FromDate     DateTime,
 @ToDate       DateTime,
 @sortOrder   VARCHAR(25) = 'DESC',
 @CustomerID varchar (25)= NULL,
 @SupplierID varchar (25) = NULL,
 @SwitchID	 varchar(50) = NULL,
 @ShowSupplier Char(1) = 'N'	
AS
BEGIN
	SET ROWCOUNT @TopRecord

--	Set @CustomerID = ISNULL(@CustomerID, '')
--	Set @SupplierID = ISNULL(@SupplierID, '')
    SET @SwitchID =isnull(@SwitchID,'')
	SET NOCOUNT ON

	Declare @FromDateStr varchar(50)
	Declare @ToDateStr  varchar(50)

	SELECT @FromDateStr = CONVERT(varchar(50), @FromDate, 121)
	SELECT @ToDateStr = CONVERT(varchar(50), @ToDate, 121)

	DECLARE @Sql varchar(8000)
if @CustomerID is null and @SupplierID is null
	SELECT @Sql =
		'SELECT  S.OurZoneID,
					Z.[Name] AS [Name],
					CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END AS SupplierID,
					SUM(Attempts) As Attempts,
					SUM(DurationsInSeconds) / 60.0 as DurationsInMinutes,
					case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
					Sum(SuccessfulAttempts)as SuccessfulAttempt,
					case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
					Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR,
				    Avg(PDDinSeconds) as AveragePDD
					FROM TrafficStats S WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
						join Zone Z WITH(nolock) on S.OurZoneID=Z.ZoneID
					WHERE 
									(
										FirstCDRAttempt BETWEEN '''+@FromDateStr+''' AND '''+@ToDateStr+'''
									)
							        and('''+@SwitchID+ '''=''''OR S.SwitchID ='''+@SwitchID +''')
		 GROUP BY S.OurZoneID,
				  Z.[Name],
				  CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END 
		 ORDER BY DurationsInMinutes '+ @sortOrder + ',
				  Attempts DESC'


if @CustomerID is not null and @SupplierID is null
	SELECT @Sql =
		'SELECT  S.OurZoneID,
					Z.[Name] AS [Name],
					CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END AS SupplierID,
					SUM(Attempts) As Attempts,
					SUM(DurationsInSeconds) / 60.0 as DurationsInMinutes,
					case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
					Sum(SuccessfulAttempts)as SuccessfulAttempt,
					case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
					Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR,
				    Avg(PDDinSeconds) as AveragePDD
					FROM TrafficStats S WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
						join Zone Z WITH(nolock) on S.OurZoneID=Z.ZoneID
					WHERE 
									(
										FirstCDRAttempt BETWEEN '''+@FromDateStr+''' AND '''+@ToDateStr+'''
									)
									and(S.CustomerID ='''+@CustomerID+''')
							        and('''+@SwitchID+ '''=''''OR S.SwitchID ='''+@SwitchID +''')
					GROUP BY S.OurZoneID,
							 Z.[Name],
							 CASE WHEN '''+@ShowSupplier+''' =''Y'' THEN S.SupplierID  ELSE NULL END 
					ORDER BY DurationsInMinutes '+ @sortOrder + ',
							 Attempts DESC'

if @CustomerID is null and @SupplierID is not null
	SELECT @Sql =
		'SELECT  S.OurZoneID,
					Z.[Name] AS [Name],
					CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END AS SupplierID,
					SUM(Attempts) As Attempts,
					SUM(DurationsInSeconds) / 60.0 as DurationsInMinutes,
					case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
					Sum(SuccessfulAttempts)as SuccessfulAttempt,
					case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
					Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR,
				    Avg(PDDinSeconds) as AveragePDD
					FROM TrafficStats S WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier))
						join Zone Z WITH(nolock) on S.OurZoneID=Z.ZoneID
					WHERE 
									(
										FirstCDRAttempt BETWEEN '''+@FromDateStr+''' AND '''+@ToDateStr+'''
									)
									and(S.SupplierID ='''+@SupplierID+''')
							        and('''+@SwitchID+ '''=''''OR S.SwitchID ='''+@SwitchID +''')
					GROUP BY S.OurZoneID,
					         Z.[Name],
					         CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END 
					ORDER BY DurationsInMinutes '+ @sortOrder + ',
					         Attempts DESC'

if @CustomerID is not null and @SupplierID is not null
	SELECT @Sql =
		'SELECT  S.OurZoneID,
					Z.[Name] AS [Name],
					CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END AS SupplierID,
					SUM(Attempts) As Attempts,
					SUM(DurationsInSeconds) / 60.0 as DurationsInMinutes,
					case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
					Sum(SuccessfulAttempts)as SuccessfulAttempt,
					case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
					Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR,
				    Avg(PDDinSeconds) as AveragePDD
					FROM TrafficStats S WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier))
						join Zone Z WITH(nolock) on S.OurZoneID=Z.ZoneID
					WHERE 
									(
										FirstCDRAttempt BETWEEN '''+@FromDateStr+''' AND '''+@ToDateStr+'''
									)
							        and('''+@SwitchID+ '''=''''OR S.SwitchID ='''+@SwitchID +''')
					GROUP BY S.OurZoneID,
					         Z.[Name],
					         CASE WHEN '''+@ShowSupplier+'''=''Y'' THEN S.SupplierID  ELSE NULL END 
					ORDER BY DurationsInMinutes '+ @sortOrder + ',
					         Attempts DESC'

	EXECUTE(@Sql)
--    PRINT @Sql






	
END
GO
/****** Object:  StoredProcedure [dbo].[bp_ShippingCDR]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<aliyouness>
-- Create date: <18-04-2008>
-- Description:	<Shipping the CDR less than the date and insert it into CDRBackup after that move the CDR Record To the TempTable after that truncate the cdr table and insert the CDRTemp >
-- =============================================
CREATE  PROCEDURE [dbo].[bp_ShippingCDR] 
	@StartingDate datetime
  --@TableName Varchar(100)
AS
BEGIN
	IF EXISTS(SELECT name FROM sys.tables
     WHERE name = 'CDRBackUp')
     BEGIN
         PRINT 'Table CDRBackUp already Exist.'
         DROP TABLE CDRBackUp
         END
  
      ELSE PRINT 'No CDRBackUp Table  Exist'
     
	SET NOCOUNT ON
	
	CREATE TABLE CDRBackUp
	(
    [CDRID] [bigint]  NOT NULL,
	[SwitchID] [tinyint] NOT NULL,
	[IDonSwitch] [bigint] NULL,
	[Tag] [varchar](20) NULL,
	[AttemptDateTime] [datetime] NULL,
	[ConnectDateTime] [datetime] NULL,
	[DisconnectDateTime] [datetime] NULL,
	[DurationInSeconds] [numeric](13, 4) NULL,
	[IN_TRUNK] [varchar](5) NULL,
	[IN_CIRCUIT] [smallint] NULL,
	[IN_CARRIER] [varchar](10) NULL,
	[IN_IP] [varchar](21) NULL,
	[OUT_TRUNK] [varchar](5) NULL,
	[OUT_CIRCUIT] [smallint] NULL,
	[OUT_CARRIER] [varchar](10) NULL,
	[OUT_IP] [varchar](21) NULL,
	[CGPN] [varchar](40) NULL,
	[CDPN] [varchar](40) NULL,
	[CAUSE_FROM_RELEASE_CODE] [varchar](20) NULL,
	[CAUSE_FROM] [char](1) NULL,
	[CAUSE_TO_RELEASE_CODE] [varchar](20) NULL,
	[CAUSE_TO] [char](1) NULL
	)
	INSERT INTO CDRBackUp
	(
    [CDRID],
	[SwitchID],
	[IDonSwitch],
	[Tag],
	[AttemptDateTime],
	[ConnectDateTime],
	[DisconnectDateTime],
	[DurationInSeconds],
	[IN_TRUNK],
	[IN_CIRCUIT],
	[IN_CARRIER],
	[IN_IP],
	[OUT_TRUNK],
	[OUT_CIRCUIT],
	[OUT_CARRIER],
	[OUT_IP],
	[CGPN],
	[CDPN],
	[CAUSE_FROM_RELEASE_CODE],
	[CAUSE_FROM],
	[CAUSE_TO_RELEASE_CODE],
	[CAUSE_TO] 
	)
	SELECT
		CDRID,
		SwitchID,
		IDonSwitch,
		Tag,
		AttemptDateTime,
		ConnectDateTime,
		DisconnectDateTime,
		DurationInSeconds,
		IN_TRUNK,
		IN_CIRCUIT,
		IN_CARRIER,
		IN_IP,
		OUT_TRUNK,
		OUT_CIRCUIT,
		OUT_CARRIER,
		OUT_IP,
		CGPN,
		CDPN,
		CAUSE_FROM_RELEASE_CODE,
		CAUSE_FROM,
		CAUSE_TO_RELEASE_CODE,
		CAUSE_TO
	FROM
		CDR
		WHERE AttemptDateTime < @StartingDate 
		
	CREATE TABLE #CDRShipping
	(
    [CDRID] [bigint]  NOT NULL,
	[SwitchID] [tinyint] NOT NULL,
	[IDonSwitch] [bigint] NULL,
	[Tag] [varchar](20) NULL,
	[AttemptDateTime] [datetime] NULL,
	[ConnectDateTime] [datetime] NULL,
	[DisconnectDateTime] [datetime] NULL,
	[DurationInSeconds] [numeric](13, 4) NULL,
	[IN_TRUNK] [varchar](5) NULL,
	[IN_CIRCUIT] [smallint] NULL,
	[IN_CARRIER] [varchar](10) NULL,
	[IN_IP] [varchar](21) NULL,
	[OUT_TRUNK] [varchar](5) NULL,
	[OUT_CIRCUIT] [smallint] NULL,
	[OUT_CARRIER] [varchar](10) NULL,
	[OUT_IP] [varchar](21) NULL,
	[CGPN] [varchar](40) NULL,
	[CDPN] [varchar](40) NULL,
	[CAUSE_FROM_RELEASE_CODE] [varchar](20) NULL,
	[CAUSE_FROM] [char](1) NULL,
	[CAUSE_TO_RELEASE_CODE] [varchar](20) NULL,
	[CAUSE_TO] [char](1) NULL
	)
	INSERT INTO #CDRShipping
	(
    [CDRID],
	[SwitchID],
	[IDonSwitch],
	[Tag],
	[AttemptDateTime],
	[ConnectDateTime],
	[DisconnectDateTime],
	[DurationInSeconds],
	[IN_TRUNK],
	[IN_CIRCUIT],
	[IN_CARRIER],
	[IN_IP],
	[OUT_TRUNK],
	[OUT_CIRCUIT],
	[OUT_CARRIER],
	[OUT_IP],
	[CGPN],
	[CDPN],
	[CAUSE_FROM_RELEASE_CODE],
	[CAUSE_FROM],
	[CAUSE_TO_RELEASE_CODE],
	[CAUSE_TO] 
	)
	

SELECT
	    CDRID,
		SwitchID,
		IDonSwitch,
		Tag,
		AttemptDateTime,
		ConnectDateTime,
		DisconnectDateTime,
		DurationInSeconds,
		IN_TRUNK,
		IN_CIRCUIT,
		IN_CARRIER,
		IN_IP,
		OUT_TRUNK,
		OUT_CIRCUIT,
		OUT_CARRIER,
		OUT_IP,
		CGPN,
		CDPN,
		CAUSE_FROM_RELEASE_CODE,
		CAUSE_FROM,
		CAUSE_TO_RELEASE_CODE,
		CAUSE_TO
		
	FROM
		CDR
		WHERE AttemptDateTime > @StartingDate
		
	TRUNCATE TABLE CDR
		
	INSERT INTO CDR 
	(
	[SwitchID],
	[IDonSwitch],
	[Tag],
	[AttemptDateTime],
	[ConnectDateTime],
	[DisconnectDateTime],
	[DurationInSeconds],
	[IN_TRUNK],
	[IN_CIRCUIT],
	[IN_CARRIER],
	[IN_IP],
	[OUT_TRUNK],
	[OUT_CIRCUIT],
	[OUT_CARRIER],
	[OUT_IP],
	[CGPN],
	[CDPN],
	[CAUSE_FROM_RELEASE_CODE],
	[CAUSE_FROM],
	[CAUSE_TO_RELEASE_CODE],
	[CAUSE_TO] 
	)	
	SELECT
		SwitchID,
		IDonSwitch,
		Tag,
		AttemptDateTime,
		ConnectDateTime,
		DisconnectDateTime,
		DurationInSeconds,
		IN_TRUNK,
		IN_CIRCUIT,
		IN_CARRIER,
		IN_IP,
		OUT_TRUNK,
		OUT_CIRCUIT,
		OUT_CARRIER,
		OUT_IP,
		CGPN,
		CDPN,
		CAUSE_FROM_RELEASE_CODE,
		CAUSE_FROM,
		CAUSE_TO_RELEASE_CODE,
		CAUSE_TO		
	FROM #CDRShipping
		
	DROP TABLE  #CDRShipping
		
	END
GO
/****** Object:  StoredProcedure [dbo].[Sp_DataBaseMonitoring]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_DataBaseMonitoring]
	
AS
BEGIN
	SET NOCOUNT ON
--DBCC UPDATEUSAGE(0) WITH COUNT_ROWS
DECLARE @Result TABLE 
( 
    [name] NVARCHAR(128),
    [rows] CHAR(11) ,
    reserved VARCHAR(18) ,-- 
    data varchar(118), --
    index_size  VARCHAR(18),
    unused  VARCHAR(18)
) 

INSERT INTO  @Result EXEC sp_msForEachTable 'EXEC sp_spaceused ''?''' 


SELECT
[name],
replace (rows,'KB','')AS rows,
Replace(reserved,'KB','')AS reserved,
replace (data ,'KB' ,'')AS data,
replace (index_size,'KB' ,'') AS index_size,
replace(unused,'KB','')AS unused
FROM  @Result  ORDER BY 1 ASC


END
GO
/****** Object:  Table [dbo].[SystemMessage]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SystemMessage](
	[MessageID] [varchar](100) NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[Message] [varchar](max) NULL,
	[Updated] [datetime] NOT NULL,
	[timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_SystemMessage] PRIMARY KEY CLUSTERED 
(
	[MessageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_SystemMessage] ON [dbo].[SystemMessage] 
(
	[Updated] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[GetHoursOfDates]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetHoursOfDates]
(
	@FromDate DATETIME,
	@ToDate DATETIME
)
RETURNS 
@Result TABLE 
(
    [Hour] int,
    Date DATETIME
)
AS
BEGIN

SET @FromDate = '2010-01-01'
SET @ToDate = '2010-01-05'

DECLARE @Days INT 

SET @Days = DATEDIFF(dd,@FromDate,@ToDate) + 1

DECLARE @Hour INT 
DECLARE @Date DATETIME 
SET @Date = @FromDate

WHILE @Date <= @ToDate
BEGIN
	SET @Hour = 0
		WHILE @Hour < 24
		BEGIN 
			INSERT INTO @Result VALUES ( @Hour,@Date)
			SET @Hour = @Hour + 1
		END
SET @Date = DATEADD(dd,1,@Date)
END

	RETURN
END
GO
/****** Object:  StoredProcedure [dbo].[bp_GetScale]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ======================================================
-- Author:		Fadi Chamieh
-- Create date: 2008-01-25 
-- Description:	Calculates a standard Scale / Scale Name
-- ======================================================
CREATE PROCEDURE [dbo].[bp_GetScale](@ScaleMax numeric(13,5), @Scale numeric(13,5) output, @ScaleName varchar(10) output)

AS
BEGIN
	SET NOCOUNT ON;

    SELECT @Scale = CASE
						WHEN @ScaleMax >= 100000000.0 THEN 100000000.0 
						WHEN @ScaleMax >= 10000000.0 THEN 10000000.0
						WHEN @ScaleMax >= 1000000.0 THEN 1000000.0
						WHEN @ScaleMax >= 100000.0 THEN 100000.0
						WHEN @ScaleMax >= 10000.0 THEN 10000.0
						WHEN @ScaleMax >= 1000.0 THEN 1000.0
						WHEN @ScaleMax >= 100.0 THEN 100.0
						WHEN @ScaleMax >= 10.0 THEN 10.0
						WHEN @ScaleMax >= 1.0 THEN 1.0
						WHEN @ScaleMax >= 0.1 THEN 0.1
						WHEN @ScaleMax >= 0.01 THEN 0.01
						WHEN @ScaleMax >= 0.001 THEN 0.001
						WHEN @ScaleMax >= 0.0001 THEN 0.0001
						WHEN @ScaleMax >= 0.00001 THEN 0.00001
						WHEN @ScaleMax >= 0.000001 THEN 0.000001
					ELSE 1.0 END

    SELECT @ScaleName = CASE 
						WHEN @ScaleMax >= 100000000.0 THEN 'x100M'
						WHEN @ScaleMax >= 10000000.0 THEN 'x10M'
						WHEN @ScaleMax >= 1000000.0 THEN 'x1M'
						WHEN @ScaleMax >= 100000.0 THEN 'x100K'
						WHEN @ScaleMax >= 10000.0 THEN 'x10K'
						WHEN @ScaleMax >= 1000.0 THEN 'x1K'
						WHEN @ScaleMax >= 100.0 THEN 'x100'
						WHEN @ScaleMax >= 10.0 THEN 'x10'
						WHEN @ScaleMax >= 1.0 THEN ''
						WHEN @ScaleMax >= 0.1 THEN '/10'
						WHEN @ScaleMax >= 0.01 THEN '/100'
						WHEN @ScaleMax >= 0.001 THEN '/1K'
						WHEN @ScaleMax >= 0.0001 THEN '/10K'
						WHEN @ScaleMax >= 0.00001 THEN '/100K'
						WHEN @ScaleMax >= 0.000001 THEN '/1M'
					ELSE '' END

END
GO
/****** Object:  UserDefinedFunction [dbo].[fnOverlaps]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   FUNCTION [dbo].[fnOverlaps]
(
 @StartDate1 DATETIME,
 @EndDate1 DATETIME,
 @StartDate2 DATETIME,
 @EndDate2 DATETIME
)
RETURNS BIT
AS BEGIN
 DECLARE @RetVal BIT
 IF  @StartDate1 <= @EndDate2 AND @StartDate2 <= @EndDate1
  SET @RetVal = 1
 ELSE
  SET @RetVal = 0
 RETURN @RetVal
END
GO
/****** Object:  UserDefinedFunction [dbo].[ParseArray]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[ParseArray](
	@Array VARCHAR(8000),
	@separator CHAR(1)
)
RETURNS @T TABLE ([value] varchar(100))
AS
BEGIN
    -- @Array is the array we wish to parse
    -- @Separator is the separator charactor such as a comma
    DECLARE @separator_position INT -- This is used to locate each separator character
    DECLARE @array_value VARCHAR(1000) -- this holds each array value as it is returned
    -- For my loop to work I need an extra separator at the end. I always look to the
    -- left of the separator character for each array value
    
    SET @array = @array + @separator
    
    -- Loop through the string searching for separtor characters
    WHILE PATINDEX('%' + @separator + '%', @array) <> 0
    BEGIN
        -- patindex matches the a pattern against a string
        SELECT @separator_position = PATINDEX('%' + @separator + '%',@array)
        SELECT @array_value = LEFT(@array, @separator_position - 1)
        -- This is where you process the values passed.
        INSERT INTO @T
        VALUES
        (
            @array_value
        )
        -- Replace this select statement with your processing
        -- @array_value holds the value of this element of the array
        -- This replaces what we just processed with and empty string
        SELECT @array = STUFF(@array, 1, @separator_position, '')
    END
    RETURN
END
GO
/****** Object:  StoredProcedure [dbo].[bp_GenerateRepeatedNumbers]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_GenerateRepeatedNumbers](@DialedCount int = 100)
AS
	DROP TABLE ##CDPN
	CREATE TABLE ##CDPN(TrafficDate smalldatetime, Customer varchar(5), CDPN varchar(30), Dialed int, Minutes numeric(13,5)) 
	INSERT INTO ##CDPN
		SELECT 
			CAST(FLOOR(CAST(AttemptDatetime AS float)) AS datetime),
			IN_CARRIER, 
			CDPN, 
			Count(*), 
			Sum(DurationInSeconds) / 60.0
		FROM
			CDR WITH(NOLOCK)
			GROUP BY CAST(FLOOR(CAST(AttemptDatetime AS float)) AS datetime), IN_CARRIER, CDPN
			HAVING Count(*) > @DialedCount

	

	SELECT * FROM ##CDPN ORDER BY Dialed DESC, TrafficDate
GO
/****** Object:  Table [dbo].[Test]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Test](
	[iD] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[iD] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[GetTimePart]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE FUNCTION [dbo].[GetTimePart] (@Date  datetime)  
RETURNS Char(5)  AS  
BEGIN 
	Declare @Hour 	char(2)
	Declare @Minute char(2)
	Declare @Time 	char(5)
	Set @Hour=convert(varchar(2),datepart(hh,@Date))
	if len(@Hour)=1
		Set @Hour='0'+@Hour
	Set @Minute=convert(char(2),datepart(n,@Date))
	if len(@Minute)=1
		Set @Minute='0'+@Minute
	Set @Time =@Hour+':'+@Minute
	Return (@Time)
END
GO
/****** Object:  StoredProcedure [dbo].[sp_BackupDatabase]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_BackupDatabase]  
       @databaseName varchar(255), 
       @backupType CHAR(1) ,
       @FilePath Varchar(4000)
AS 
BEGIN 
       SET NOCOUNT ON; 

       DECLARE @sqlCommand NVARCHAR(1000) 
       DECLARE @dateTime NVARCHAR(20) 

       SELECT @dateTime = REPLACE(CONVERT(VARCHAR, GETDATE(),111),'/','') + 
       REPLACE(CONVERT(VARCHAR, GETDATE(),108),':','')  

       IF @backupType = 'F' 
               SET @sqlCommand = 'BACKUP DATABASE ' + @databaseName + 
               ' TO DISK = '+@FilePath +  @databaseName + '_Full_' + @dateTime + '.BAK''' 
        
       IF @backupType = 'D' 
               SET @sqlCommand = 'BACKUP DATABASE ' + @databaseName + 
               ' TO DISK = '+@FilePath + @databaseName + '_Diff_' + @dateTime + '.BAK'' WITH DIFFERENTIAL' 
        
       IF @backupType = 'L' 
               SET @sqlCommand = 'BACKUP LOG ' + @databaseName + 
               ' TO DISK = '+@FilePath + @databaseName + '_Log_' + @dateTime + '.TRN''' 
        
       EXECUTE sp_executesql @sqlCommand 
END
GO
/****** Object:  Table [dbo].[InvoiceData]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceData](
	[SourceFilebytes] [image] NULL,
	[InvoiceID] [int] NOT NULL,
	[timestamp] [timestamp] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[SP_DataBaseStatistics]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_DataBaseStatistics]

AS
BEGIN

	SET NOCOUNT ON;

  -- Get Top 20 executed SP's ordered by logical reads (memory pressure)
    SELECT TOP 100 qt.text AS 'SP Name', total_logical_reads, 
    qs.total_worker_time AS 'TotalWorkerTime', 
    qs.total_worker_time/qs.execution_count AS 'AvgWorkerTime',
    qs.execution_count AS 'Execution Count', total_logical_reads/qs.execution_count AS 'AvgLogicalReads',
    qs.total_physical_reads, qs.total_physical_reads/qs.execution_count AS 'Avg Physical Reads',
    qs.total_logical_writes/DATEDIFF(Minute, qs.creation_time, GetDate()) AS 'Logical Writes/Min',  
    qs.total_logical_writes, qs.total_logical_writes/qs.execution_count AS 'AvgLogicalWrites',
    qs.execution_count/DATEDIFF(Second, qs.creation_time, GetDate()) AS 'Calls/Second', 
    qs.total_worker_time/qs.execution_count AS 'AvgWorkerTime',
    qs.total_worker_time AS 'TotalWorkerTime',
    qs.total_elapsed_time/qs.execution_count AS 'AvgElapsedTime',
    qs.total_logical_writes,
    qs.max_logical_reads, qs.max_logical_writes, qs.total_physical_reads, 
    DATEDIFF(Minute, qs.creation_time, GetDate()) AS 'Age in Cache', qt.dbid 
    FROM sys.dm_exec_query_stats AS qs
    CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) AS qt
    WHERE qt.dbid = db_id() -- Filter by current database
    ORDER BY total_logical_reads DESC
    
    
-- SELECT '%signal (cpu) waits' = CAST(100.0 * SUM(signal_wait_time_ms) / SUM (wait_time_ms) AS NUMERIC(20,2)),
--           '%resource waits'= CAST(100.0 * SUM(wait_time_ms - signal_wait_time_ms) / SUM (wait_time_ms) AS NUMERIC(20,2))
--      FROM sys.dm_os_wait_stats;

END
GO
/****** Object:  Table [dbo].[ZGM_GroupedZones]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ZGM_GroupedZones](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[GroupID] [int] NOT NULL,
	[ZoneID] [int] NOT NULL,
 CONSTRAINT [PK_GroupedZones_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LookupValue]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LookupValue](
	[LookupValueID] [int] IDENTITY(1,1) NOT NULL,
	[LookupTypeID] [int] NOT NULL,
	[Value] [nvarchar](255) NOT NULL,
	[Ordinal] [int] NULL,
 CONSTRAINT [PK_LookupValue] PRIMARY KEY CLUSTERED 
(
	[LookupValueID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LookupValue] ON [dbo].[LookupValue] 
(
	[LookupTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[bp_DatabaseBackup]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[bp_DatabaseBackup]

@Databases varchar(max),
@Directory varchar(max),
@BackupType varchar(max),
@Verify varchar(max),
@CleanupTime int

AS

SET NOCOUNT ON

----------------------------------------------------------------------------------------------------
--// Declare variables                                                                          //--
----------------------------------------------------------------------------------------------------

DECLARE @StartMessage varchar(max)
DECLARE @EndMessage varchar(max)
DECLARE @DatabaseMessage varchar(max)
DECLARE @ErrorMessage varchar(max)

DECLARE @InstanceName varchar(max)
DECLARE @FileExtension varchar(max)

DECLARE @CurrentID int
DECLARE @CurrentDatabase varchar(max)
DECLARE @CurrentDirectory varchar(max)
DECLARE @CurrentDate varchar(max)
DECLARE @CurrentFileName varchar(max)
DECLARE @CurrentFilePath varchar(max)
DECLARE @CurrentCleanupTime varchar(max)

DECLARE @CurrentCommand01 varchar(max)
DECLARE @CurrentCommand02 varchar(max)
DECLARE @CurrentCommand03 varchar(max)
DECLARE @CurrentCommand04 varchar(max)

DECLARE @CurrentCommandOutput01 int
DECLARE @CurrentCommandOutput02 int
DECLARE @CurrentCommandOutput03 int
DECLARE @CurrentCommandOutput04 int

DECLARE @DirectoryInfo TABLE (	FileExists bit,
								FileIsADirectory bit,
								ParentDirectoryExists bit)

DECLARE @tmpDatabases TABLE (	ID int IDENTITY PRIMARY KEY,
								DatabaseName varchar(max),
								Completed bit)

DECLARE @Error int

SET @Error = 0

----------------------------------------------------------------------------------------------------
--// Log initial information                                                                    //--
----------------------------------------------------------------------------------------------------

SET @StartMessage =	'DateTime: ' + CONVERT(varchar,GETDATE(),120) + CHAR(13) + CHAR(10)
SET @StartMessage = @StartMessage + 'Procedure: ' + QUOTENAME(DB_NAME(DB_ID())) + '.' + QUOTENAME(OBJECT_SCHEMA_NAME(@@PROCID)) + '.' + QUOTENAME(OBJECT_NAME(@@PROCID)) + CHAR(13) + CHAR(10)
SET @StartMessage = @StartMessage + 'Parameters: @Databases = ' + ISNULL('''' + @Databases + '''','NULL')
SET @StartMessage = @StartMessage + ', @Directory = ' + ISNULL('''' + @Directory + '''','NULL')
SET @StartMessage = @StartMessage + ', @BackupType = ' + ISNULL('''' + @BackupType + '''','NULL')
SET @StartMessage = @StartMessage + ', @Verify = ' + ISNULL('''' + @Verify + '''','NULL')
SET @StartMessage = @StartMessage + ', @CleanupTime = ' + ISNULL(CAST(@CleanupTime AS varchar),'NULL')
SET @StartMessage = @StartMessage + CHAR(13) + CHAR(10)

RAISERROR(@StartMessage,10,1) WITH NOWAIT

----------------------------------------------------------------------------------------------------
--// Select databases                                                                           //--
----------------------------------------------------------------------------------------------------

IF @Databases IS NULL OR @Databases = ''
BEGIN
	SET @ErrorMessage = 'The value for parameter @Databases is not supported.' + CHAR(13) + CHAR(10)
	RAISERROR(@ErrorMessage,16,1) WITH NOWAIT
	SET @Error = @@ERROR
END

INSERT INTO @tmpDatabases (DatabaseName, Completed)
SELECT	DatabaseName AS DatabaseName,
		0 AS Completed
FROM dbo.DatabaseSelect (@Databases)
ORDER BY DatabaseName ASC

IF @@ERROR <> 0 OR @@ROWCOUNT = 0
BEGIN
	SET @ErrorMessage = 'Error selecting databases.' + CHAR(13) + CHAR(10)
	RAISERROR(@ErrorMessage,16,1) WITH NOWAIT
	SET @Error = @@ERROR
END

----------------------------------------------------------------------------------------------------
--// Check directory                                                                            //--
----------------------------------------------------------------------------------------------------

IF NOT (@Directory LIKE '_:' OR @Directory LIKE '_:\%') OR @Directory LIKE '%\' OR @Directory IS NULL
BEGIN
	SET @ErrorMessage = 'The value for parameter @Directory is not supported.' + CHAR(13) + CHAR(10)
	RAISERROR(@ErrorMessage,16,1) WITH NOWAIT
	SET @Error = @@ERROR
END

INSERT INTO @DirectoryInfo (FileExists, FileIsADirectory, ParentDirectoryExists)
EXECUTE('EXECUTE xp_FileExist ''' + @Directory + '''')

IF NOT EXISTS (SELECT * FROM @DirectoryInfo WHERE FileExists = 0 AND FileIsADirectory = 1 AND ParentDirectoryExists = 1)
BEGIN
	SET @ErrorMessage = 'The directory does not exist.' + CHAR(13) + CHAR(10)
	RAISERROR(@ErrorMessage,16,1) WITH NOWAIT
	SET @Error = @@ERROR
END

----------------------------------------------------------------------------------------------------
--// Check backup type                                                                          //--
----------------------------------------------------------------------------------------------------

SET @BackupType = UPPER(@BackupType)

IF @BackupType NOT IN ('FULL','DIFF','LOG') OR @BackupType IS NULL
BEGIN
	SET @ErrorMessage = 'The value for parameter @BackupType is not supported.' + CHAR(13) + CHAR(10)
	RAISERROR(@ErrorMessage,16,1) WITH NOWAIT
	SET @Error = @@ERROR
END

----------------------------------------------------------------------------------------------------
--// Check Verify input                                                                         //--
----------------------------------------------------------------------------------------------------

IF @Verify NOT IN ('Y','N') OR @Verify IS NULL
BEGIN
	SET @ErrorMessage = 'The value for parameter @Verify is not supported.' + CHAR(13) + CHAR(10)
	RAISERROR(@ErrorMessage,16,1) WITH NOWAIT
	SET @Error = @@ERROR
END

----------------------------------------------------------------------------------------------------
--// Check CleanupTime input                                                                    //--
----------------------------------------------------------------------------------------------------

IF @CleanupTime < 0 OR @CleanupTime IS NULL
BEGIN
	SET @ErrorMessage = 'The value for parameter @CleanupTime is not supported.' + CHAR(13) + CHAR(10)
	RAISERROR(@ErrorMessage,16,1) WITH NOWAIT
	SET @Error = @@ERROR
END

----------------------------------------------------------------------------------------------------
--// Check error variable                                                                       //--
----------------------------------------------------------------------------------------------------

IF @Error <> 0 GOTO Logging

----------------------------------------------------------------------------------------------------
--// Set global variables                                                                       //--
----------------------------------------------------------------------------------------------------

SET @InstanceName = REPLACE(CAST(SERVERPROPERTY('servername') AS varchar),'\','$')

SELECT @FileExtension = CASE
WHEN @BackupType = 'FULL' THEN 'bak'
WHEN @BackupType = 'DIFF' THEN 'bak'
WHEN @BackupType = 'LOG' THEN 'trn'
END

----------------------------------------------------------------------------------------------------
--// Execute backup commands                                                                    //--
----------------------------------------------------------------------------------------------------

WHILE EXISTS (SELECT * FROM @tmpDatabases WHERE Completed = 0)
BEGIN

	SELECT TOP 1	@CurrentID = ID,
					@CurrentDatabase = DatabaseName
	FROM @tmpDatabases
	WHERE Completed = 0
	ORDER BY ID ASC

	-- Set database message
	SET @DatabaseMessage = 'DateTime: ' + CONVERT(varchar,GETDATE(),120) + CHAR(13) + CHAR(10)
	SET @DatabaseMessage = @DatabaseMessage + 'Database: ' + QUOTENAME(@CurrentDatabase) + CHAR(13) + CHAR(10)
	SET @DatabaseMessage = @DatabaseMessage + 'Status: ' + CAST(DATABASEPROPERTYEX(@CurrentDatabase,'status') AS varchar) + CHAR(10)
	RAISERROR(@DatabaseMessage,10,1) WITH NOWAIT

	IF DATABASEPROPERTYEX(@CurrentDatabase,'status') = 'ONLINE'
	BEGIN

		SET @CurrentDirectory = @Directory + '\' + @InstanceName + '\' + @CurrentDatabase + '\' + @BackupType

		SET @CurrentDate = REPLACE(REPLACE(REPLACE((CONVERT(varchar,GETDATE(),120)),'-',''),' ','_'),':','')
		
		SET @CurrentFileName = @InstanceName + '_' + @CurrentDatabase + '_' + @BackupType + '_' + @CurrentDate + '.' + @FileExtension

		SET @CurrentFilePath = @CurrentDirectory + '\' + @CurrentFileName

		SET @CurrentCleanupTime = CONVERT(varchar(19),(DATEADD(hh,-(@CleanupTime),GETDATE())),126)
		
		-- Create directory
		SET @CurrentCommand01 = 'DECLARE @ReturnCode int EXECUTE @ReturnCode = master.dbo.xp_create_subdir ''' + @CurrentDirectory + ''' IF @ReturnCode <> 0 RAISERROR(''Error creating directory.'', 16, 1)'
		EXECUTE @CurrentCommandOutput01 = [dbo].[CommandExecute] @CurrentCommand01, '', 1
		SET @Error = @@ERROR
		IF @ERROR <> 0 SET @CurrentCommandOutput01 = @ERROR

		-- Perform a backup
		IF @CurrentCommandOutput01 = 0
		BEGIN
			SELECT @CurrentCommand02 = CASE
			WHEN @BackupType = 'FULL' THEN 'BACKUP DATABASE ' + QUOTENAME(@CurrentDatabase) + ' TO DISK = ''' + @CurrentFilePath + ''' WITH CHECKSUM'
			WHEN @BackupType = 'DIFF' THEN 'BACKUP DATABASE ' + QUOTENAME(@CurrentDatabase) + ' TO DISK = ''' + @CurrentFilePath + ''' WITH CHECKSUM, DIFFERENTIAL'
			WHEN @BackupType = 'LOG' THEN 'BACKUP LOG ' + QUOTENAME(@CurrentDatabase) + ' TO DISK = ''' + @CurrentFilePath + ''' WITH CHECKSUM'
			END
			EXECUTE @CurrentCommandOutput02 = [dbo].[CommandExecute] @CurrentCommand02, '', 1
			SET @Error = @@ERROR
			IF @ERROR <> 0 SET @CurrentCommandOutput02 = @ERROR
		END

		-- Verify the backup
		IF @CurrentCommandOutput02 = 0 AND @Verify = 'Y'
		BEGIN
			SET @CurrentCommand03 = 'RESTORE VERIFYONLY FROM DISK = ''' + @CurrentFilePath + ''' WITH CHECKSUM'
			EXECUTE @CurrentCommandOutput03 = [dbo].[CommandExecute] @CurrentCommand03, '', 1
			SET @Error = @@ERROR
			IF @ERROR <> 0 SET @CurrentCommandOutput03 = @ERROR
		END

		-- Delete old backup files
		IF (@CurrentCommandOutput02 = 0 AND @Verify = 'N')
		OR (@CurrentCommandOutput02 = 0 AND @Verify = 'Y' AND @CurrentCommandOutput03 = 0)
		BEGIN
			SET @CurrentCommand04 = 'DECLARE @ReturnCode int EXECUTE @ReturnCode = master.dbo.xp_delete_file 0, ''' + @CurrentDirectory + ''', ''' + @FileExtension + ''', ''' + @CurrentCleanupTime + ''' IF @ReturnCode <> 0 RAISERROR(''Error deleting files.'', 16, 1)'
			EXECUTE @CurrentCommandOutput04 = [dbo].[CommandExecute] @CurrentCommand04, '', 1
			SET @Error = @@ERROR
			IF @ERROR <> 0 SET @CurrentCommandOutput04 = @ERROR
		END

	END

	-- Update that the database is completed
	UPDATE @tmpDatabases
	SET Completed = 1
	WHERE ID = @CurrentID

	-- Clear variables
	SET @CurrentID = NULL
	SET @CurrentDatabase = NULL
	SET @CurrentDirectory = NULL
	SET @CurrentDate = NULL
	SET @CurrentFileName = NULL
	SET @CurrentFilePath = NULL
	SET @CurrentCleanupTime = NULL

	SET @CurrentCommand01 = NULL
	SET @CurrentCommand02 = NULL
	SET @CurrentCommand03 = NULL
	SET @CurrentCommand04 = NULL

	SET @CurrentCommandOutput01 = NULL
	SET @CurrentCommandOutput02 = NULL
	SET @CurrentCommandOutput03 = NULL
	SET @CurrentCommandOutput04 = NULL

END

----------------------------------------------------------------------------------------------------
--// Log completing information                                                                 //--
----------------------------------------------------------------------------------------------------

Logging:

SET @EndMessage = 'DateTime: ' + CONVERT(varchar,GETDATE(),120)

RAISERROR(@EndMessage,10,1) WITH NOWAIT

----------------------------------------------------------------------------------------------------
GO
/****** Object:  Table [dbo].[PriceListImportMailMessage]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PriceListImportMailMessage](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MessageID] [int] NOT NULL,
	[Subject] [varchar](1024) NULL,
	[From] [varchar](1024) NULL,
	[To] [varchar](1024) NULL,
	[Body] [varchar](max) NULL,
	[DeliveryDate] [datetime] NULL,
	[CC] [varchar](max) NULL,
	[BCC] [varchar](max) NULL,
	[AttachmentFileName] [varchar](255) NULL,
	[Attachment] [image] NULL,
	[ContentType] [varchar](50) NULL,
	[ContentDisposition] [varchar](50) NULL,
	[Processed] [bit] NULL,
 CONSTRAINT [PK_MailMessage] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_PriceListImportMailMessage] ON [dbo].[PriceListImportMailMessage] 
(
	[MessageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CurrencyExchangeRate]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CurrencyExchangeRate](
	[CurrencyExchangeRateID] [bigint] IDENTITY(1,1) NOT NULL,
	[CurrencyID] [varchar](3) NULL,
	[Rate] [float] NULL,
	[ExchangeDate] [smalldatetime] NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_CurrencyExchangeRate] PRIMARY KEY CLUSTERED 
(
	[CurrencyExchangeRateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_CurrencyExchangeRate_Currency] ON [dbo].[CurrencyExchangeRate] 
(
	[CurrencyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CurrencyExchangeRate_Date] ON [dbo].[CurrencyExchangeRate] 
(
	[ExchangeDate] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ZGM_ZoneGroup]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ZGM_ZoneGroup](
	[GroupID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[CustomerID] [varchar](5) NULL,
	[UserID] [int] NOT NULL,
	[timestamp] [timestamp] NOT NULL,
	[Updated] [datetime] NULL,
 CONSTRAINT [PK_ZoneGroup] PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[bp_PreviewInvoice]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_PreviewInvoice]
(
	@CustomerID varchar(5),
	@FromDate Datetime,
	@ToDate Datetime
)
AS
SET NOCOUNT ON 


SET @FromDate=     CAST(
(
STR( YEAR( @FromDate ) ) + '-' +
STR( MONTH( @FromDate ) ) + '-' +
STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate=     CAST(
(
STR( YEAR( @ToDate ) ) + '-' +
STR( MONTH(@ToDate ) ) + '-' +
STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)

DECLARE @Pivot SMALLDATETIME
SET @Pivot = @FromDate

CREATE TABLE #temp
(
	
	[NAME] VARCHAR(255),
	MinAttempt DATETIME,
    MaxAttempt DATETIME,
    DurationInMinutes  numeric(13, 4) NULL,
    Rate FLOAT,
    Ratetype [tinyint] NOT NULL  DEFAULT ((0)),
    Amount FLOAT,
	Currency [varchar](3) NOT NULL,
    NumberOfCalls  INT 
)


WHILE @Pivot <= @ToDate
BEGIN
INSERT INTO #Temp
SELECT 
       Z.Name Destination,
       MIN(BS.CallDate) MinAttempt,
       MAX(BS.CallDate) MaxAttempt,
       SUM(BS.SaleDuration)/60.0 DurationInMinutes,
       Round(BS.Sale_Rate,5) Rate,
       BS.Sale_RateType Ratetype,
       ISNULL(SUM(BS.Sale_Nets ),0) Amount,
       BS.Sale_Currency Currency,
       SUM(BS.NumberOfCalls) AS NumberOfCalls
FROM   Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) 
LEFT JOIN Zone z WITH(NOLOCK) ON z.ZoneID = bs.SaleZoneID AND z.SupplierID ='SYS'
WHERE  BS.CallDate = @Pivot
  AND  BS.CustomerID = @CustomerID
  AND  BS.Sale_Currency IS NOT NULL
GROUP BY
       Z.[Name],
       Round(BS.Sale_Rate,5),
       BS.Sale_RateType,
       BS.Sale_Currency 
ORDER BY z.[Name] 
SET @Pivot = DATEADD(dd, 1, @Pivot)
END 

SELECT 
       Destination,
       MinAttempt,
       MaxAttempt,
       DurationInMinutes,
       Rate,
       Ratetype,
       Amount,
       Currency,
       NumberOfCalls 
FROM #Temp 
GROUP BY
       Destination,
       Rate,
       RateType,
       Currency
GO
/****** Object:  StoredProcedure [dbo].[bp_ErroneousPricedCDR]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_ErroneousPricedCDR]
(
	@CarrierAccountID varchar(5),
	@IsSale CHAR(1),
	@FromDate DATETIME,
	@TillDate DATETIME
)
AS
SET @FromDate = CAST(
(
STR( YEAR( @FromDate ) ) + '-' +
STR( MONTH( @FromDate ) ) + '-' +
STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @TillDate = CAST(
(
STR( YEAR( @TillDate ) ) + '-' +
STR( MONTH(@TillDate ) ) + '-' +
STR( DAY( @TillDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)

CREATE TABLE #temp (ID BIGINT PRIMARY KEY CLUSTERED) 

IF (@IsSale = 'Y' )
BEGIN
	  INSERT INTO #temp 
	  SELECT bcm.ID  
	  FROM   Billing_CDR_Main bcm WITH(
	                   NOLOCK,
	                   INDEX(IX_Billing_CDR_Main_Attempt),
	                   INDEX(IX_Billing_CDR_Main_Customer)
	               )
	        WHERE  bcm.Attempt BETWEEN   @FromDate AND @TillDate
	               AND bcm.CustomerID = @CarrierAccountID
	              
     

	SELECT  COUNT(*)
	FROM   #temp
	WHERE id  NOT IN (
	           SELECT bcs.ID
	           FROM   Billing_CDR_Sale bcs WITH(NOLOCK)
	)

END

IF (@IsSale = 'N' )
BEGIN
  INSERT INTO #temp 
	  SELECT bcm.ID  
	  FROM   Billing_CDR_Main bcm WITH(
	                   NOLOCK,
	                   INDEX(IX_Billing_CDR_Main_Attempt),
	                   INDEX(IX_Billing_CDR_Main_Supplier)
	               )
	   WHERE  bcm.Attempt BETWEEN   @FromDate AND @TillDate
	               AND bcm.SupplierID = @CarrierAccountID
	              
     

	SELECT  COUNT(*)
	FROM   #temp
	WHERE id  NOT IN (
	           SELECT bcc.ID
	           FROM   Billing_CDR_Cost bcc WITH(NOLOCK)
	)
END 

DROP TABLE #temp
GO
/****** Object:  Table [dbo].[StateBackup]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StateBackup](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StateBackupType] [tinyint] NOT NULL,
	[CustomerID] [varchar](10) NULL,
	[SupplierID] [varchar](10) NULL,
	[Created] [datetime] NOT NULL,
	[Notes] [text] NULL,
	[StateData] [image] NOT NULL,
	[UserId] [int] NOT NULL,
	[timestamp] [timestamp] NOT NULL,
	[RestoreDate] [datetime] NULL,
	[ResponsibleForRestoring] [varchar](200) NULL,
 CONSTRAINT [PK_StateChange] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ZoneMatch]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ZoneMatch](
	[OurZoneID] [int] NOT NULL,
	[SupplierZoneID] [int] NOT NULL,
 CONSTRAINT [PK_ZoneMatch] PRIMARY KEY CLUSTERED 
(
	[OurZoneID] ASC,
	[SupplierZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ZoneMatch_SupplierZoneID] ON [dbo].[ZoneMatch] 
(
	[SupplierZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PostpaidAmount]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PostpaidAmount](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerProfileID] [smallint] NULL,
	[CustomerID] [varchar](10) NULL,
	[SupplierID] [varchar](10) NULL,
	[SupplierProfileID] [smallint] NULL,
	[Amount] [numeric](13, 5) NULL,
	[CurrencyID] [varchar](3) NULL,
	[Date] [datetime] NULL,
	[Type] [smallint] NULL,
	[UserID] [int] NULL,
	[Tag] [varchar](255) NULL,
	[timestamp] [timestamp] NOT NULL,
	[LastUpdate] [datetime] NULL,
	[ReferenceNumber] [varchar](50) NULL,
	[Note] [varchar](250) NULL,
 CONSTRAINT [PK_PostpaidAmount] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_PostpaidAmount_Date] ON [dbo].[PostpaidAmount] 
(
	[Date] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PostpaidAmount_Type] ON [dbo].[PostpaidAmount] 
(
	[Type] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PricingTemplate]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PricingTemplate](
	[PricingTemplateId] [int] IDENTITY(1,1) NOT NULL,
	[CurrencyID] [varchar](3) NOT NULL,
	[Title] [varchar](100) NOT NULL,
	[Description] [varchar](250) NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
	[TemplateType] [smallint] NULL,
 CONSTRAINT [PK_PricingTemplate] PRIMARY KEY CLUSTERED 
(
	[PricingTemplateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CarrierProfile]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CarrierProfile](
	[ProfileID] [smallint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NULL,
	[CompanyName] [nvarchar](200) NULL,
	[CompanyLogo] [image] NULL,
	[CompanyLogoName] [nvarchar](500) NULL,
	[Address] [nvarchar](250) NULL,
	[Country] [nvarchar](100) NULL,
	[Telephone] [nvarchar](100) NULL,
	[Fax] [nvarchar](100) NULL,
	[BillingContact] [nvarchar](200) NULL,
	[BillingEmail] [nvarchar](256) NULL,
	[PricingContact] [nvarchar](200) NULL,
	[PricingEmail] [nvarchar](256) NULL,
	[SupportContact] [nvarchar](200) NULL,
	[SupportEmail] [nvarchar](256) NULL,
	[CurrencyID] [varchar](3) NULL,
	[DuePeriod] [tinyint] NULL,
	[PaymentTerms] [tinyint] NULL,
	[Tax1] [numeric](6, 2) NULL,
	[Tax2] [numeric](6, 2) NULL,
	[IsTaxAffectsCost] [char](1) NULL,
	[TaxFormula] [varchar](200) NULL,
	[VAT] [numeric](6, 2) NULL,
	[Services] [numeric](6, 2) NULL,
	[ConnectionFees] [money] NULL,
	[IsDeleted] [char](1) NULL,
	[BankingDetails] [ntext] NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
	[RegistrationNumber] [nvarchar](50) NULL,
	[EscalationLevel] [nvarchar](max) NULL,
	[Guarantee] [float] NULL,
	[CustomerPaymentType] [tinyint] NULL,
	[SupplierPaymentType] [tinyint] NULL,
	[CustomerCreditLimit] [int] NULL,
	[SupplierCreditLimit] [int] NULL,
	[IsNettingEnabled] [char](1) NULL,
	[CustomerActivateDate] [datetime] NULL,
	[CustomerDeactivateDate] [datetime] NULL,
	[SupplierActivateDate] [datetime] NULL,
	[SupplierDeactivateDate] [datetime] NULL,
	[VatID] [varchar](20) NULL,
 CONSTRAINT [PK_CarrierProfile] PRIMARY KEY CLUSTERED 
(
	[ProfileID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[SP_CapacityMarginCheck]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[SP_CapacityMarginCheck] 
    @FromDateTime	datetime,
	@ToDateTime		DATETIME
WITH RECOMPILE
AS
BEGIN	
	SET NOCOUNT ON

        
SELECT 
        CustomerID,
        SupplierID,
		SwitchId,
		Port_IN,
		Port_OUT,
		FirstCDRAttempt,
		Attempts,
		SuccessfulAttempts,
		DurationsInSeconds,
		UtilizationInSeconds,
		NumberOfCalls
	INTO #CarrierTraffic
	FROM TrafficStats ts WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst,IX_TrafficStats_Customer))
	WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		
	   SELECT
	        switchid,
	        customerid,
	        supplierid,
			Port_In,
			Port_Out,
			SUM(NumberOfCalls) AS Attempts,
			SUM(SuccessfulAttempts) AS  SuccesfulAttempts,
			SUM(SuccessfulAttempts) * 100.0 /
			Cast(NULLIF(SUM(NumberOfCalls) + 0.0, 0) AS NUMERIC) AS ASR,
			SUM(DurationsInSeconds)/60.0   AS DurationsInMinutes,
			SUM(UtilizationInSeconds)/60.0   AS UtilizationsInMinutes		
	   FROM #CarrierTraffic
	   GROUP BY 
	        switchid,
	        customerid,
	        supplierid,
			Port_In,
			Port_Out
			    
END
GO
/****** Object:  Table [dbo].[SwitchCarrierMapping]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SwitchCarrierMapping](
	[SwitchID] [tinyint] NULL,
	[CarrierAccountID] [varchar](10) NULL,
	[Identifier] [varchar](100) NULL,
	[IsIn] [char](1) NULL,
	[IsOut] [char](1) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Billing_CDR_Cost]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Billing_CDR_Cost](
	[ID] [bigint] NOT NULL,
	[ZoneID] [int] NOT NULL,
	[Net] [float] NOT NULL,
	[CurrencyID] [varchar](3) NOT NULL,
	[RateValue] [float] NOT NULL,
	[RateID] [bigint] NOT NULL,
	[Discount] [float] NULL,
	[RateType] [tinyint] NOT NULL,
	[ToDConsiderationID] [bigint] NULL,
	[FirstPeriod] [float] NULL,
	[RepeatFirstperiod] [tinyint] NULL,
	[FractionUnit] [tinyint] NULL,
	[TariffID] [bigint] NULL,
	[CommissionValue] [float] NOT NULL,
	[CommissionID] [int] NULL,
	[ExtraChargeValue] [float] NOT NULL,
	[ExtraChargeID] [int] NULL,
	[Updated] [smalldatetime] NOT NULL,
	[DurationInSeconds] [numeric](13, 4) NULL,
 CONSTRAINT [PK_Billing_CDR_Cost] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'''N'' for Normal (Peak), ''O'' for Offpeak and ''W'' for Weekend Rate ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Billing_CDR_Cost', @level2type=N'COLUMN',@level2name=N'RateType'
GO
/****** Object:  Table [dbo].[Billing_Invoice]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Billing_Invoice](
	[InvoiceID] [int] IDENTITY(1,1) NOT NULL,
	[BeginDate] [smalldatetime] NOT NULL,
	[EndDate] [smalldatetime] NOT NULL,
	[IssueDate] [smalldatetime] NOT NULL,
	[DueDate] [smalldatetime] NOT NULL,
	[SupplierID] [varchar](10) NOT NULL,
	[CustomerID] [varchar](10) NOT NULL,
	[SerialNumber] [varchar](50) NOT NULL,
	[Duration] [numeric](19, 6) NULL,
	[Amount] [decimal](13, 6) NOT NULL,
	[CurrencyID] [varchar](3) NULL,
	[IsLocked] [char](1) NOT NULL,
	[IsPaid] [char](1) NOT NULL,
	[PaidDate] [smalldatetime] NULL,
	[PaidAmount] [decimal](13, 6) NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
	[InvoiceAttachement] [image] NULL,
	[FileName] [varchar](255) NULL,
	[InvoicePrintedNote] [varchar](max) NULL,
	[InvoiceNotes] [varchar](max) NULL,
	[NumberOfCalls] [int] NULL,
	[VatValue] [decimal](13, 6) NULL,
	[SourceFileName] [nvarchar](100) NULL,
 CONSTRAINT [PK_Billing_Invoice] PRIMARY KEY CLUSTERED 
(
	[InvoiceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Billing_Invoice_BeginDate] ON [dbo].[Billing_Invoice] 
(
	[BeginDate] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_Invoice_Customer] ON [dbo].[Billing_Invoice] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_Invoice_IssueDate] ON [dbo].[Billing_Invoice] 
(
	[IssueDate] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_Invoice_Supplier] ON [dbo].[Billing_Invoice] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RouteOption]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RouteOption](
	[RouteID] [int] NOT NULL,
	[SupplierID] [varchar](5) NOT NULL,
	[SupplierZoneID] [int] NULL,
	[SupplierActiveRate] [real] NULL,
	[SupplierNormalRate] [real] NULL,
	[SupplierOffPeakRate] [real] NULL,
	[SupplierWeekendRate] [real] NULL,
	[SupplierServicesFlag] [smallint] NULL,
	[Priority] [tinyint] NOT NULL,
	[NumberOfTries] [tinyint] NULL,
	[State] [tinyint] NOT NULL,
	[Updated] [datetime] NULL,
	[Percentage] [tinyint] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE CLUSTERED INDEX [IDX_RouteOption_RouteID] ON [dbo].[RouteOption] 
(
	[RouteID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Defines the status of this route option: V for Valid, B for blocked, P for highest priority, S for Special Request' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RouteOption', @level2type=N'COLUMN',@level2name=N'State'
GO
/****** Object:  Table [dbo].[Billing_InvoicePayment]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Billing_InvoicePayment](
	[InvoicePaymentID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceID] [int] NULL,
	[DueDate] [smalldatetime] NULL,
	[Amount] [decimal](13, 6) NULL,
	[PaidDate] [smalldatetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SwitchReleaseCode]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SwitchReleaseCode](
	[SwitchID] [tinyint] NOT NULL,
	[ReleaseCode] [varchar](50) NOT NULL,
	[IsoCode] [varchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[IsDelivered] [char](1) NOT NULL,
 CONSTRAINT [PK_SwitchReleaseCode] PRIMARY KEY CLUSTERED 
(
	[SwitchID] ASC,
	[ReleaseCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[bp_ProcessOperationQueue]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_ProcessOperationQueue]
AS
BEGIN
	DECLARE @OperationQueue TABLE (Type char(1) NOT NULL, ID bigint NOT NULL, Operation char(1) NOT NULL)
	
	INSERT INTO @OperationQueue	SELECT Type, ID, Operation FROM OperationQueue
	
	TRUNCATE TABLE OperationQueue
	
	DECLARE @Type char(1)
	DECLARE @ID bigint
	DECLARE @Operation char(1)
	
	DECLARE @EndFlag char(1)
	DECLARE @BeginFlag char(1) 
	SET @BeginFlag = 'B'
	SET @EndFlag = 'E'	

	DECLARE OperationCur CURSOR LOCAL FORWARD_ONLY
		FOR SELECT [Type], ID, Operation FROM @OperationQueue 
	OPEN OperationCur
	FETCH NEXT FROM OperationCur INTO @Type, @ID, @Operation
	WHILE @@FETCH_STATUS <> 0 
	BEGIN
		
		FETCH NEXT FROM OperationCur INTO @Type, @ID, @Operation		
	END
		
END
GO
/****** Object:  Table [dbo].[Tariff]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tariff](
	[TariffID] [bigint] IDENTITY(1,1) NOT NULL,
	[ZoneID] [int] NULL,
	[CustomerID] [varchar](10) NOT NULL,
	[SupplierID] [varchar](10) NOT NULL,
	[CallFee] [decimal](13, 6) NOT NULL,
	[FirstPeriodRate] [decimal](13, 6) NULL,
	[FirstPeriod] [tinyint] NULL,
	[RepeatFirstPeriod] [char](1) NULL,
	[FractionUnit] [tinyint] NULL,
	[BeginEffectiveDate] [smalldatetime] NULL,
	[EndEffectiveDate] [smalldatetime] NULL,
	[IsEffective]  AS (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_Tariff] PRIMARY KEY CLUSTERED 
(
	[TariffID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Tariff_Customer] ON [dbo].[Tariff] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Tariff_Dates] ON [dbo].[Tariff] 
(
	[BeginEffectiveDate] DESC,
	[EndEffectiveDate] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Tariff_Supplier] ON [dbo].[Tariff] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Tariff_Zone] ON [dbo].[Tariff] 
(
	[ZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebsiteMenuItem]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WebsiteMenuItem](
	[ItemID] [int] IDENTITY(1,1) NOT NULL,
	[Path] [varchar](100) NOT NULL,
	[Text] [varchar](255) NOT NULL,
	[Description] [varchar](1024) NULL,
	[NavigateURL] [varchar](512) NULL,
 CONSTRAINT [PK_WebsiteMenu] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_WebsiteMenu] ON [dbo].[WebsiteMenuItem] 
(
	[Path] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CodeSupply]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CodeSupply](
	[Code] [varchar](15) NOT NULL,
	[SupplierID] [varchar](5) NOT NULL,
	[SupplierZoneID] [int] NOT NULL,
	[SupplierNormalRate] [real] NOT NULL,
	[SupplierOffPeakRate] [real] NULL,
	[SupplierWeekendRate] [real] NULL,
	[SupplierServicesFlag] [smallint] NOT NULL,
	[Updated] [datetime] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_CodeSupply_Code] ON [dbo].[CodeSupply] 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CodeSupply_ServicesFlag] ON [dbo].[CodeSupply] 
(
	[SupplierServicesFlag] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CodeSupply_Supplier] ON [dbo].[CodeSupply] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CodeSupply_Zone] ON [dbo].[CodeSupply] 
(
	[SupplierZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CarrierSwitchConnectivity]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CarrierSwitchConnectivity](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CarrierAccountID] [varchar](10) NOT NULL,
	[SwitchID] [tinyint] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Notes] [nvarchar](max) NULL,
	[ConnectionType] [tinyint] NULL,
	[NumberOfChannels_In] [int] NULL,
	[NumberOfChannels_Out] [int] NULL,
	[NumberOfChannels_Total] [int] NULL,
	[NumberOfChannels_Shared] [int] NULL,
	[BeginEffectiveDate] [smalldatetime] NOT NULL,
	[EndEffectiveDate] [smalldatetime] NULL,
	[UserID] [int] NULL,
	[Margin_Total] [real] NULL,
	[Details] [nvarchar](max) NULL,
 CONSTRAINT [PK_CarrierSwitchConnectivity] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_CSC_CarrierAccount] ON [dbo].[CarrierSwitchConnectivity] 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1 for Voip and 0 for TDM' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CarrierSwitchConnectivity', @level2type=N'COLUMN',@level2name=N'ConnectionType'
GO
/****** Object:  Table [dbo].[PriceListData]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PriceListData](
	[PriceListID] [int] NOT NULL,
	[SourceFileBytes] [image] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_PriceListData] PRIMARY KEY CLUSTERED 
(
	[PriceListID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[IsNullOrEmpty]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   FUNCTION [dbo].[IsNullOrEmpty](@Check_Expression VARCHAR(MAX), @Replacement_Value VARCHAR(MAX))
RETURNS VARCHAR(MAX)
AS
BEGIN
	
	declare @Dummy VARCHAR(MAX)
	declare @TheResult VARCHAR(MAX)
	
	SET @Dummy = Ltrim(Rtrim(REPLACE(@Check_Expression,',','')))
    
    IF(@Dummy = '' OR @Dummy IS NULL)
      set @TheResult = @Replacement_Value
    ELSE
      set @TheResult = @Check_Expression
       
	RETURN @TheResult
END
GO
/****** Object:  Table [dbo].[Changes]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Changes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ZoneID] [int] NOT NULL,
	[CodeID] [bigint] NULL,
	[Changes] [smallint] NOT NULL,
	[timestamp] [timestamp] NULL,
	[CustomerID] [varchar](5) NOT NULL,
 CONSTRAINT [PK_ZoneCodeChange] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ChangeTracker]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ChangeTracker](
	[CustomerID] [varchar](5) NOT NULL,
	[LastUpdated] [smalldatetime] NULL,
	[Notes] [varchar](500) NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_ChangeTracker_1] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CarrierDocument]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CarrierDocument](
	[DocumentID] [smallint] IDENTITY(1,1) NOT NULL,
	[ProfileID] [smallint] NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Description] [text] NULL,
	[Category] [text] NULL,
	[Document] [image] NULL,
	[Created] [smalldatetime] NOT NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_CarrierDocument] PRIMARY KEY CLUSTERED 
(
	[DocumentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CarrierDocument] ON [dbo].[CarrierDocument] 
(
	[ProfileID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permission]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Permission](
	[ID] [varchar](255) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [ntext] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PersistedRunnableTask]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PersistedRunnableTask](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ScheduleType] [tinyint] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Configuration] [ntext] NOT NULL,
	[IsEnabled] [char](1) NOT NULL,
	[TimeSpan] [varchar](20) NULL,
	[IsLastRunSuccessful] [char](1) NULL,
	[LastRun] [smalldatetime] NULL,
	[LastRunDuration] [varchar](20) NULL,
	[NextRun] [smalldatetime] NULL,
	[timestamp] [timestamp] NULL,
	[ExceptionString] [ntext] NULL,
	[GroupingExpression] [varchar](512) NULL,
 CONSTRAINT [PK_PersistedRunnableTask] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  UserDefinedFunction [dbo].[GetSupplierZoneStats]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[GetSupplierZoneStats]
(
	@FromDate datetime,
	@TillDate DATETIME,
	@ZoneID INT =null  
)
RETURNS @Stats TABLE 
(
	SupplierID VARCHAR(10)
	,OurZoneID INT
	,DurationsInMinutes  NUMERIC(13,5)
	,ASR NUMERIC(13,5)
	,ACD NUMERIC(13,5)
	PRIMARY KEY (SupplierID,OurZoneID)
)
AS
BEGIN
IF @ZoneID IS NULL 
	INSERT INTO @Stats
	SELECT 
		ISNULL(TS.SupplierID,''), 
		ISNULL(TS.OurZoneID,0), 
        SUM(DurationsInSeconds/60.) as DurationsInMinutes, 
	    (SUM(TS.SuccessfulAttempts)*100.0 / SUM(TS.Attempts)) AS ASR,
		CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(TS.DurationsInSeconds)/(60.0 * SUM(TS.SuccessfulAttempts)) ELSE 0 END AS ACD
	FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
         WHERE 
			TS.FirstCDRAttempt BETWEEN @FromDate AND @TillDate
	GROUP BY TS.SupplierID, TS.OurZoneID
ELSE
	INSERT INTO @Stats
	SELECT 
		ISNULL(TS.SupplierID,''), 
		ISNULL(TS.OurZoneID,0), 
        SUM(DurationsInSeconds/60.) as DurationsInMinutes, 
	    (SUM(TS.SuccessfulAttempts)*100.0 / SUM(TS.Attempts)) AS ASR,
		CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(TS.DurationsInSeconds)/(60.0 * SUM(TS.SuccessfulAttempts)) ELSE 0 END AS ACD
	FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
         WHERE 
			TS.FirstCDRAttempt BETWEEN @FromDate AND @TillDate
			AND TS.OurZoneID = @ZoneID
	GROUP BY TS.SupplierID, TS.OurZoneID
	RETURN 
END
GO
/****** Object:  Table [dbo].[Currency]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Currency](
	[CurrencyID] [varchar](3) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[IsMainCurrency] [char](1) NOT NULL,
	[IsVisible] [char](1) NOT NULL,
	[LastRate] [float] NULL,
	[LastUpdated] [smalldatetime] NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED 
(
	[CurrencyID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Commission]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Commission](
	[CommissionID] [int] IDENTITY(1,1) NOT NULL,
	[SupplierID] [varchar](10) NOT NULL,
	[CustomerID] [varchar](10) NOT NULL,
	[ZoneID] [int] NULL,
	[FromRate] [decimal](9, 5) NULL,
	[ToRate] [decimal](9, 5) NULL,
	[Percentage] [decimal](9, 5) NULL,
	[Amount] [decimal](9, 5) NULL,
	[BeginEffectiveDate] [smalldatetime] NULL,
	[EndEffectiveDate] [smalldatetime] NULL,
	[IsExtraCharge] [char](1) NULL,
	[IsEffective]  AS (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_Commission] PRIMARY KEY CLUSTERED 
(
	[CommissionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Commission_Customer] ON [dbo].[Commission] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Commission_Dates] ON [dbo].[Commission] 
(
	[BeginEffectiveDate] DESC,
	[EndEffectiveDate] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Commission_Supplier] ON [dbo].[Commission] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Commission_Zone] ON [dbo].[Commission] 
(
	[ZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[bp_FixUnsoldZonesForRouteBuild]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_FixUnsoldZonesForRouteBuild]
AS
BEGIN
	
	CREATE TABLE #UnsoldZones (CustomerID nVARCHAR(10), OurZoneID INT, CodeGroup nVARCHAR(10), Rate REAL, ServicesFlag smallint, PRIMARY KEY(CustomerID, OurZoneID))

	INSERT INTO #UnsoldZones (CustomerID, OurZoneID, CodeGroup, Rate, ServicesFlag)
		SELECT DISTINCT ca.CarrierAccountID, z.ZoneID, z.CodeGroup, -1, 0
		  FROM Zone Z, CarrierAccount ca WHERE
			z.SupplierID = 'SYS'
			AND z.IsEffective = 'Y' 
			AND NOT EXISTS (SELECT * FROM ZoneRate zr WHERE zr.ZoneID = z.ZoneID AND ca.CarrierAccountID = zr.CustomerID AND zr.SupplierID = 'SYS')
			AND EXISTS (SELECT * FROM ZoneRate zr, Zone oz WHERE zr.ZoneID = oz.ZoneID AND oz.CodeGroup = z.CodeGroup AND ca.CarrierAccountID = zr.CustomerID AND zr.SupplierID = 'SYS')
		
	/*
	UPDATE #UnsoldZones SET 
		MinRate = -1, 
		MaxServicesFlag = 
		(
			SELECT MAX(zr.ServicesFlag) 
				FROM ZoneRate zr, Zone z 
				WHERE 
						z.SupplierID = 'SYS' 
					AND z.CodeGroup = #UnsoldZones.CodeGroup COLLATE SQL_Latin1_General_CP1256_CI_AS 
					AND z.ZoneID = zr.ZoneID
					AND zr.CustomerID = #UnsoldZones.CustomerID COLLATE SQL_Latin1_General_CP1256_CI_AS
		)				
	*/
	
	INSERT INTO ZoneRate
	(
		ZoneID,
		SupplierID,
		CustomerID,
		NormalRate,
		OffPeakRate,
		WeekendRate,
		ServicesFlag
	)
	SELECT 
		uz.OurZoneID, 
		'SYS', 
		uz.CustomerID, 
		uz.Rate, 
		uz.Rate, 
		uz.Rate, 
		uz.ServicesFlag
	FROM #UnsoldZones uz

	--SELECT * FROM #UnsoldZones

	DROP TABLE #UnsoldZones

END
GO
/****** Object:  Table [dbo].[CDR]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CDR](
	[CDRID] [bigint] IDENTITY(1,1) NOT NULL,
	[SwitchID] [tinyint] NOT NULL,
	[IDonSwitch] [bigint] NULL,
	[Tag] [varchar](100) NULL,
	[AttemptDateTime] [datetime] NULL,
	[AlertDateTime] [datetime] NULL,
	[ConnectDateTime] [datetime] NULL,
	[DisconnectDateTime] [datetime] NULL,
	[DurationInSeconds] [numeric](13, 4) NULL,
	[IN_TRUNK] [varchar](5) NULL,
	[IN_CIRCUIT] [smallint] NULL,
	[IN_CARRIER] [varchar](100) NULL,
	[IN_IP] [varchar](21) NULL,
	[OUT_TRUNK] [varchar](5) NULL,
	[OUT_CIRCUIT] [smallint] NULL,
	[OUT_CARRIER] [varchar](100) NULL,
	[OUT_IP] [varchar](21) NULL,
	[CGPN] [varchar](40) NULL,
	[CDPN] [varchar](40) NULL,
	[CAUSE_FROM_RELEASE_CODE] [varchar](20) NULL,
	[CAUSE_FROM] [varchar](10) NULL,
	[CAUSE_TO_RELEASE_CODE] [varchar](20) NULL,
	[CAUSE_TO] [varchar](10) NULL,
	[Extra_Fields] [varchar](255) NULL,
	[IsRerouted] [char](1) NULL,
 CONSTRAINT [PK_CDR] PRIMARY KEY CLUSTERED 
(
	[CDRID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_CDR_AttemptDateTime] ON [dbo].[CDR] 
(
	[AttemptDateTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FlaggedService]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FlaggedService](
	[FlaggedServiceID] [smallint] NOT NULL,
	[Symbol] [varchar](3) NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_FlaggedService] PRIMARY KEY CLUSTERED 
(
	[FlaggedServiceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Billing_CDR_Invalid]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Billing_CDR_Invalid](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Attempt] [datetime] NULL,
	[Alert] [datetime] NULL,
	[Connect] [datetime] NULL,
	[Disconnect] [datetime] NULL,
	[DurationInSeconds] [numeric](13, 4) NULL,
	[CustomerID] [varchar](5) NULL,
	[OurZoneID] [int] NULL,
	[SupplierID] [varchar](5) NULL,
	[SupplierZoneID] [int] NULL,
	[CDPN] [varchar](50) NULL,
	[CGPN] [varchar](50) NULL,
	[ReleaseCode] [varchar](50) NULL,
	[ReleaseSource] [varchar](10) NULL,
	[SwitchID] [tinyint] NULL,
	[SwitchCdrID] [bigint] NULL,
	[Tag] [varchar](50) NULL,
	[OriginatingZoneID] [int] NULL,
	[Extra_Fields] [varchar](255) NULL,
	[IsRerouted] [char](1) NULL,
	[Port_IN] [varchar](21) NULL,
	[Port_OUT] [varchar](21) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE CLUSTERED INDEX [IX_Billing_CDR_Invalid_Attempt] ON [dbo].[Billing_CDR_Invalid] 
(
	[Attempt] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Invalid_Customer] ON [dbo].[Billing_CDR_Invalid] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Invalid_OurZoneID] ON [dbo].[Billing_CDR_Invalid] 
(
	[OurZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Invalid_Supplier] ON [dbo].[Billing_CDR_Invalid] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Billing_CDR_Sale]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Billing_CDR_Sale](
	[ID] [bigint] NOT NULL,
	[ZoneID] [int] NOT NULL,
	[Net] [float] NOT NULL,
	[CurrencyID] [varchar](3) NOT NULL,
	[RateValue] [float] NOT NULL,
	[RateID] [bigint] NOT NULL,
	[Discount] [float] NULL,
	[RateType] [tinyint] NOT NULL,
	[ToDConsiderationID] [bigint] NULL,
	[FirstPeriod] [float] NULL,
	[RepeatFirstperiod] [tinyint] NULL,
	[FractionUnit] [tinyint] NULL,
	[TariffID] [bigint] NULL,
	[CommissionValue] [float] NOT NULL,
	[CommissionID] [int] NULL,
	[ExtraChargeValue] [float] NOT NULL,
	[ExtraChargeID] [int] NULL,
	[Updated] [smalldatetime] NOT NULL,
	[DurationInSeconds] [numeric](13, 4) NULL,
 CONSTRAINT [PK_Billing_CDR_Sale] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'''N'' for Normal (Peak), ''O'' for Offpeak and ''W'' for Weekend Rate ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Billing_CDR_Sale', @level2type=N'COLUMN',@level2name=N'RateType'
GO
/****** Object:  View [dbo].[vw_RouteBlocks]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_RouteBlocks]
AS
SELECT TOP 100 percent * FROM RouteBlock rb 
WHERE rb.IsEffective = 'Y'
ORDER BY rb.CustomerID, rb.Code
GO
/****** Object:  Table [dbo].[Billing_CDR_Main]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Billing_CDR_Main](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Attempt] [datetime] NOT NULL,
	[Alert] [datetime] NULL,
	[Connect] [datetime] NOT NULL,
	[Disconnect] [datetime] NOT NULL,
	[DurationInSeconds] [numeric](13, 4) NOT NULL,
	[CustomerID] [varchar](5) NOT NULL,
	[OurZoneID] [int] NULL,
	[OriginatingZoneID] [int] NULL,
	[SupplierID] [varchar](5) NOT NULL,
	[SupplierZoneID] [int] NULL,
	[CDPN] [varchar](50) NULL,
	[CGPN] [varchar](50) NULL,
	[ReleaseCode] [varchar](50) NULL,
	[ReleaseSource] [varchar](10) NULL,
	[SwitchID] [tinyint] NOT NULL,
	[SwitchCdrID] [bigint] NULL,
	[Tag] [varchar](50) NULL,
	[Extra_Fields] [varchar](255) NULL,
	[Port_IN] [varchar](21) NULL,
	[Port_OUT] [varchar](21) NULL,
 CONSTRAINT [PK_Billing_CDR_Main] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Main_Attempt] ON [dbo].[Billing_CDR_Main] 
(
	[Attempt] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Main_Customer] ON [dbo].[Billing_CDR_Main] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Main_OurZoneID] ON [dbo].[Billing_CDR_Main] 
(
	[OurZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Main_Supplier] ON [dbo].[Billing_CDR_Main] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomTimeZoneInfo]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomTimeZoneInfo](
	[ID] [smallint] IDENTITY(1,1) NOT NULL,
	[BaseUtcOffset] [int] NOT NULL,
	[DisplayName] [varchar](250) NOT NULL,
	[UserID] [int] NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_CustomTimeZone] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CustomerTimeinfo]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomerTimeinfo](
	[ID] [smallint] IDENTITY(1,1) NOT NULL,
	[BaseUtcOffset] [int] NOT NULL,
	[DisplayName] [varchar](250) NOT NULL,
	[UserID] [int] NULL,
	[Timestamp] [timestamp] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_TrafficStats_CarrierPortMonitor]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_TrafficStats_CarrierPortMonitor]
	@FromDateTime	DATETIME,
	@ToDateTime		DATETIME,
	@InOut			varchar(3) = NULL,
	@CarrierID		varchar(10) = NULL,
	@SwitchID		tinyInt = NULL
	
WITH recompile 
AS
SET NOCOUNT ON
	
	IF (@InOut = 'IN') 
	BEGIN		
		-- Customer 
		IF @CarrierID IS NOT NULL
			SELECT	CustomerID As CarrierAccountID,
				Port_IN AS Port,
			Sum(Attempts) as Attempts, 
			Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
			WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
			AND TS.Port_IN IS NOT NULL
			AND CustomerID = @CarrierID 
			AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
			Group BY TS.CustomerID, TS.Port_IN
			ORDER by Attempts DESC
		ELSE
			-- No Customer
			SELECT CustomerID As CarrierAccountID,
				Port_IN AS Port,
			Sum(Attempts) as Attempts, 
			Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
			WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
			AND TS.Port_IN IS NOT NULL
			AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
			Group BY TS.CustomerID, TS.Port_IN
			ORDER by Attempts DESC			
	END	
	
	IF (@InOut = 'OUT') 
	BEGIN 
		-- Supplier
		IF @CarrierID IS NOT NULL
			SELECT 	SupplierID As CarrierAccountID,
				Port_OUT AS Port,
			Sum(Attempts) as Attempts, 
			Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
			Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier))
			WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
			AND TS.Port_OUT IS NOT NULL
			AND SupplierID = @CarrierID 
			AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
			Group BY TS.SupplierID, TS.Port_OUT
			ORDER by Attempts Desc
		ELSE
			-- No Supplier
			SELECT SupplierID As CarrierAccountID,
				Port_OUT AS Port,
			Sum(Attempts) as Attempts, 
			Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
			Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
			WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
			AND TS.Port_OUT IS NOT NULL
			AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
			Group BY TS.SupplierID,TS.Port_OUT
			ORDER by Attempts Desc		
	END
GO
/****** Object:  Table [dbo].[Switch]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Switch](
	[SwitchID] [tinyint] IDENTITY(1,1) NOT NULL,
	[Symbol] [varchar](10) NULL,
	[Name] [varchar](512) NULL,
	[Description] [text] NULL,
	[Configuration] [ntext] NULL,
	[LastCDRImportTag] [varchar](255) NULL,
	[LastImport] [datetime] NULL,
	[LastRouteUpdate] [datetime] NULL,
	[UserID] [int] NULL,
	[Enable_CDR_Import] [char](1) NOT NULL,
	[Enable_Routing] [char](1) NOT NULL,
	[timestamp] [timestamp] NULL,
	[LastAttempt] [datetime] NULL,
	[NominalTrunkCapacityInE1s] [int] NULL,
	[NominalVoipCapacityInE1s] [int] NULL,
 CONSTRAINT [PK_Switch] PRIMARY KEY CLUSTERED 
(
	[SwitchID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FaultTicket]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FaultTicket](
	[FaultTicketID] [int] IDENTITY(1,1) NOT NULL,
	[CarrierAccountID] [varchar](10) NOT NULL,
	[ZoneID] [int] NOT NULL,
	[TicketType] [tinyint] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[TicketDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[FromDate] [datetime] NOT NULL,
	[ToDate] [datetime] NULL,
	[Reference] [varchar](255) NULL,
	[AlertPeriod] [bigint] NULL,
	[AlertTime] [datetime] NULL,
	[Attempts] [int] NULL,
	[ASR] [float] NULL,
	[ACD] [float] NULL,
	[TestNumbers] [varchar](255) NULL,
	[FileName] [varchar](50) NULL,
	[Attachment] [image] NULL,
	[OwnReference] [varchar](50) NULL,
	[UserID] [int] NULL,
 CONSTRAINT [PK_FaultTicket] PRIMARY KEY CLUSTERED 
(
	[FaultTicketID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RouteChangeHeader]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RouteChangeHeader](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Reason] [varchar](255) NOT NULL,
	[Created] [datetime] NOT NULL,
	[UserID] [int] NOT NULL,
	[timestamp] [timestamp] NOT NULL,
	[IsEnded] [char](1) NOT NULL,
 CONSTRAINT [PK_RouteChangeHeader] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_SwitchHourlyReport]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--
-- Get an Hourly report for traffic (using given parameters)
--
CREATE  PROCEDURE [dbo].[SP_TrafficStats_SwitchHourlyReport] 
    @FromDateTime	datetime,
	@ToDateTime		datetime,
	@SwitchID		tinyInt = NULL  
AS
BEGIN	
	SET NOCOUNT ON
		SET @FromDateTime=     CAST(
     (
     STR( YEAR( @FromDateTime ) ) + '-' +
     STR( MONTH( @FromDateTime ) ) + '-' +
     STR( DAY( @FromDateTime ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDateTime= CAST(
     (
     STR( YEAR( @ToDateTime ) ) + '-' +
     STR( MONTH(@ToDateTime ) ) + '-' +
     STR( DAY( @ToDateTime ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	;
	DECLARE @IPPattern VARCHAR(10) 
	SET @IPPattern = '%.%.%.%'
	
	SELECT
        datepart(hour,LastCDRAttempt) AS [Hour],
        dateadd(dd,0, datediff(dd,0,LastCDRAttempt)) AS [Date],
        Ts.SwitchId AS SwitchID,
        SUM(CASE WHEN ts.Port_IN not LIKE @IPPattern THEN ts.DurationsInSeconds ELSE 0 END )/60.0 AS TDM_InDuration,
        SUM(CASE WHEN ts.Port_IN not LIKE @IPPattern AND ts.Port_Out not LIKE @IPPattern  THEN ts.DurationsInSeconds ELSE 0 END )/60.0 AS TDM_TDMOutDuration,
        SUM(CASE WHEN ts.Port_IN LIKE @IPPattern THEN ts.DurationsInSeconds ELSE 0 END )/60.0 AS IP_InDuration,
        SUM(CASE WHEN ts.Port_IN LIKE @IPPattern AND ts.Port_Out not LIKE @IPPattern  THEN ts.DurationsInSeconds ELSE 0 END )/60.0 AS IP_TDMOutDuration,
        SUM(CASE WHEN ts.Port_Out not LIKE @IPPattern THEN ts.DurationsInSeconds ELSE 0 END )/60.0 AS TDM_OutDuration,
        SUM(CASE WHEN ts.Port_Out LIKE @IPPattern THEN ts.DurationsInSeconds ELSE 0 END )/60.0 AS IP_OutDuration,
        
        SUM(CASE WHEN ts.Port_IN not LIKE @IPPattern THEN ts.UtilizationInSeconds ELSE 0 END )/60.0 AS TDM_InUtilDuration,
        SUM(CASE WHEN ts.Port_IN not LIKE @IPPattern AND ts.Port_Out not LIKE @IPPattern  THEN ts.UtilizationInSeconds ELSE 0 END )/60.0 AS TDM_TDMOutUtilDuration,
        SUM(CASE WHEN ts.Port_IN LIKE @IPPattern THEN ts.UtilizationInSeconds ELSE 0 END )/60.0 AS IP_InUtilDuration,
        SUM(CASE WHEN ts.Port_IN LIKE @IPPattern AND ts.Port_Out not LIKE @IPPattern  THEN ts.UtilizationInSeconds ELSE 0 END )/60.0 AS IP_TDMOutUtilDuration,
        SUM(CASE WHEN ts.Port_Out not LIKE @IPPattern THEN ts.UtilizationInSeconds ELSE 0 END )/60.0 AS TDM_OutUtilDuration,
        SUM(CASE WHEN ts.Port_Out LIKE @IPPattern THEN ts.UtilizationInSeconds ELSE 0 END )/60.0 AS IP_OutUtilDuration
         
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst) )                                     
   WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
	    AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
	GROUP BY datepart(hour,LastCDRAttempt),dateadd(dd,0, datediff(dd,0,LastCDRAttempt)),TS.SwitchId
	ORDER BY [Hour] ,[Date]

END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_SwitchCarrierConnectivity]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TrafficStats_SwitchCarrierConnectivity] 
    @FromDateTime	datetime,
	@ToDateTime		datetime,
	@CarrierAccountID varchar(10),
	@SwitchID		tinyInt = NULL ,
	@RemoveInterconnectedData CHAR(1) = 'N'
AS
BEGIN	
	SET NOCOUNT ON;
	
	SET @FromDateTime=     CAST(
     (
     STR( YEAR( @FromDateTime ) ) + '-' +
     STR( MONTH( @FromDateTime ) ) + '-' +
     STR( DAY( @FromDateTime ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDateTime= CAST(
     (
     STR( YEAR( @ToDateTime ) ) + '-' +
     STR( MONTH(@ToDateTime ) ) + '-' +
     STR( DAY( @ToDateTime ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	;
	
  DECLARE @Connectivities TABLE 
	(
		GateWay VARCHAR(250),
		Details VARCHAR(MAX),
		Date SMALLDATETIME,
		NumberOfChannels_In INT,
		NumberOfChannels_Out int,
		NumberOfChannels_Total int,
		Margin_Total INT,
		DetailList VARCHAR(MAX),
		InterconnectedSwithes CHAR(1)
	)
INSERT INTO @Connectivities
 SELECT c.*, ',' + c.Details + ',' AS DetailList  ,'N' 
 from dbo.GetDailyConnectivity(
 		null,
 		@CarrierAccountID,
 		@SwitchID,
 		'N', 
 		@FromDateTime,
 		@ToDateTime,
        'N') c   
 
  IF @RemoveInterconnectedData = 'Y'
 INSERT INTO @Connectivities
 SELECT c.*, ',' + c.Details + ',' AS DetailList  ,'Y' 
 from dbo.GetDailyConnectivity(
 		null,
 		null,
 		@SwitchID,
 		'N', 
 		@FromDateTime,
 		@ToDateTime,
        'Y') c ; 
SELECT 
		SwitchId,
		Port_IN,
		Port_OUT,
		CustomerID,
		OurZoneID,
		OriginatingZoneID,
		SupplierID,
		SupplierZoneID,
		FirstCDRAttempt,
		LastCDRAttempt,
		Attempts,
		DeliveredAttempts,
		SuccessfulAttempts,
		DurationsInSeconds,
		PDDInSeconds,
		MaxDurationInSeconds,
		UtilizationInSeconds,
		NumberOfCalls
	INTO #CarrierTraffic
	FROM TrafficStats ts WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
	WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID);      
	
  With InterConnet AS 
  (
	SELECT * FROM @Connectivities WHERE InterconnectedSwithes = 'N'
  ),	    
   TrafficTable AS 
  (
   SELECT
        datepart(hour,FirstCDRAttempt) AS [Hour],
        dbo.DateOf(FirstCDRAttempt) AS [Date],
        SUM(CASE WHEN ts.CustomerID = @CarrierAccountID  THEN  ts.DurationsInSeconds   ELSE 0 END) /60.0   AS InDurationsInMinutes,
        SUM(CASE WHEN ts.SupplierID = @CarrierAccountID  THEN  ts.DurationsInSeconds   ELSE 0 END) /60.0   AS OutDurationsInMinutes, 
        SUM(CASE WHEN ts.SupplierID = @CarrierAccountID OR ts.CustomerID = @CarrierAccountID  THEN  ts.DurationsInSeconds   ELSE 0 END)  /60.0   AS TotalDurationsInMinutes,
        SUM(CASE WHEN ts.CustomerID = @CarrierAccountID  THEN  ts.UtilizationInSeconds ELSE 0 END) /60.0   AS InUtilizationsInMinutes,
        SUM(CASE WHEN ts.SupplierID = @CarrierAccountID  THEN  ts.UtilizationInSeconds ELSE 0 END) /60.0   AS OutUtilizationsInMinutes,
        SUM(CASE WHEN ts.SupplierID = @CarrierAccountID OR ts.CustomerID = @CarrierAccountID  THEN ts.UtilizationInSeconds ELSE 0 END)  /60.0   AS TotalUtilizationsInMinutes
   FROM #CarrierTraffic AS TS
        LEFT JOIN @Connectivities DCI ON DCI.Date = dbo.DateOf(FirstCDRAttempt) AND DCI.InterconnectedSwithes = 'Y'
   WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
	    AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
	  AND (    (
		    	    TS.CustomerID = @CarrierAccountID 
		    	    AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_Out + ',%')
		    	    AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_In + ',%')
		    	  )
		          OR 
		          (
		          	 TS.SupplierID = @CarrierAccountID 
		            AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_Out + ',%')
		    	    AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_In + ',%')
		          )
		     )
	GROUP BY 
        datepart(hour,FirstCDRAttempt),
        dbo.DateOf(FirstCDRAttempt)
    )
 
    SELECT T.[Hour],
           T.[Date],
           T.InDurationsInMinutes,
           T.OutDurationsInMinutes,
           T.TotalDurationsInMinutes,
           T.InUtilizationsInMinutes,
           T.OutUtilizationsInMinutes,
           T.TotalUtilizationsInMinutes,
           C.NumberOfChannels_In AS NumberOfChannels_In,
		   C.NumberOfChannels_Out AS NumberOfChannels_Out,
		   C.NumberOfChannels_Total AS NumberOfChannels_Total,
		   C.Margin_total AS Margin_Total
    FROM   TrafficTable T
           LEFT JOIN InterConnet C ON C.[Date] = T.[Date]  
    ORDER BY T.[Date] 
    		
END
GO
/****** Object:  Table [dbo].[RetailAgent_Stats]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RetailAgent_Stats](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Date] [smalldatetime] NOT NULL,
	[AgentID] [int] NOT NULL,
	[SaleAmount] [float] NULL,
	[CostAmount] [float] NULL,
	[NumberOfCalls] [int] NULL,
	[DurationInMinutes] [numeric](13, 4) NULL,
	[UnPricedCalls] [int] NULL,
	[PricedCalls] [int] NULL,
 CONSTRAINT [PK_RetailAgent_Stats] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RetailAgent_Stats_date] ON [dbo].[RetailAgent_Stats] 
(
	[Date] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RetailAgent]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RetailAgent](
	[AgentID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Prefix] [varchar](255) NOT NULL,
	[ID] [varchar](255) NULL,
	[Tag] [varchar](255) NULL,
	[UserID] [int] NULL,
 CONSTRAINT [PK_RetailAgent] PRIMARY KEY CLUSTERED 
(
	[AgentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RoutingPool]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RoutingPool](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[IsEnabled] [char](1) NOT NULL,
 CONSTRAINT [PK_RoutingPool] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AlertCriteria]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AlertCriteria](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ClassName] [varchar](512) NULL,
	[SerializationInfo] [image] NULL,
	[Updated] [datetime] NULL,
	[IsEnabled] [char](1) NULL,
	[Tag] [varchar](50) NULL,
	[UserID] [int] NULL,
	[XMLSerializationInfo] [varchar](max) NULL,
 CONSTRAINT [PK_PersistedAlertCriteria2] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PrepaidPostpaidOptions]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PrepaidPostpaidOptions](
	[PrepaidPostpaidID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerProfileID] [int] NULL,
	[IsCustomer] [char](1) NULL,
	[Email] [varchar](255) NULL,
	[Amount] [numeric](13, 5) NULL,
	[MinimumActionEmailInterval] [varchar](50) NULL,
	[Actions] [smallint] NULL,
	[CustomerID] [varchar](10) NULL,
	[SupplierID] [varchar](10) NULL,
	[SupplierProfileID] [int] NULL,
	[Percentage] [numeric](18, 2) NULL,
	[IsPrepaid] [char](1) NULL,
 CONSTRAINT [PK_PrepaidPostpaidOptions] PRIMARY KEY CLUSTERED 
(
	[PrepaidPostpaidID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[Sp_SwapCarrierReport]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Sp_SwapCarrierReport]
    @FromDate     datetime,
    @ToDate       datetime,
    @CarrierAccountID    varchar(10) = NULL,
    @OurZoneID    int = NULL 
WITH RECOMPILE
AS
BEGIN   
    SET NOCOUNT ON;
 

    WITH TrafficTable AS
    (
    SELECT
        TS.OurZoneID AS OurZoneID,
    	SUM(CASE WHEN ts.CustomerID = @CarrierAccountID  THEN ts.Attempts ELSE 0 END)   AS InAttempts,
	    SUM(CASE WHEN ts.SupplierID = @CarrierAccountID THEN ts.Attempts ELSE 0 END)   AS OutAttempts,
	        
		SUM(CASE WHEN ts.CustomerID = @CarrierAccountID THEN ts.SuccessfulAttempts ELSE 0 END) AS  InSuccessfulAttempts,
		SUM(CASE WHEN ts.SupplierID = @CarrierAccountID THEN ts.SuccessfulAttempts ELSE 0 END) AS  OutSuccessfulAttempts,
	        
		SUM(CASE WHEN ts.CustomerID = @CarrierAccountID THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
		NULLIF(SUM(CASE WHEN ts.CustomerID = @CarrierAccountID THEN ts.Attempts ELSE 0 END), 0) AS InASR,
	        
		SUM(CASE WHEN ts.SupplierID = @CarrierAccountID THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
		NULLIF(SUM(CASE WHEN ts.SupplierID = @CarrierAccountID THEN ts.Attempts ELSE 0 END),0) AS OutASR,
	       
		SUM(CASE WHEN ts.CustomerID = @CarrierAccountID THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS InDurationsInMinutes,
		SUM(CASE WHEN ts.SupplierID = @CarrierAccountID THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS OutDurationsInMinutes 
        
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))  
   WHERE  
        FirstCDRAttempt BETWEEN @FromDate AND @ToDate
        AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
    GROUP BY TS.OurZoneID
    )

SELECT * FROM trafficTable T

END
GO
/****** Object:  Table [dbo].[CodeGroup]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CodeGroup](
	[Code] [varchar](20) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_CodeGroup] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ZoneGroup_Code] ON [dbo].[CodeGroup] 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ZoneGroup_Name] ON [dbo].[CodeGroup] 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CodeChangeLog]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CodeChangeLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Description] [text] NULL,
	[Updated] [smalldatetime] NULL,
	[SourceFileBytes] [image] NULL,
	[SourceFileName] [nvarchar](100) NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_ChangeLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[User](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](255) NULL,
	[IsActive] [char](1) NOT NULL,
	[LastLogin] [smalldatetime] NULL,
	[Description] [ntext] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PriceListChangeLog]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PriceListChangeLog](
	[LogID] [bigint] IDENTITY(1,1) NOT NULL,
	[PriceListID] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[ObjectID] [bigint] NOT NULL,
	[ObjectType] [char](1) NOT NULL,
	[ChangeType] [int] NOT NULL,
 CONSTRAINT [PK_PriceListChangeLog] PRIMARY KEY CLUSTERED 
(
	[LogID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  UserDefinedFunction [dbo].[DateOf]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[DateOf](@datetime datetime)
RETURNS Datetime
AS
BEGIN
	declare @TheDate datetime
	SET @TheDate = cast(
			(
				cast(Year(@datetime) AS varchar) 
				+ '-' + 
				cast(Month(@datetime)  AS varchar) 
				+ '-' +
				cast(Day(@datetime) AS varchar) 
			) AS datetime)

	return @TheDate
END
GO
/****** Object:  Table [dbo].[RolePermission]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RolePermission](
	[Role] [int] NOT NULL,
	[Permission] [varchar](255) NOT NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_RolePermission] PRIMARY KEY CLUSTERED 
(
	[Role] ASC,
	[Permission] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[User] [int] NOT NULL,
	[Role] [int] NOT NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[User] ASC,
	[Role] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CarrierAccount]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CarrierAccount](
	[CarrierAccountID] [varchar](5) NOT NULL,
	[ProfileID] [smallint] NOT NULL,
	[ServicesFlag] [smallint] NOT NULL,
	[ActivationStatus] [tinyint] NOT NULL,
	[RoutingStatus] [tinyint] NOT NULL,
	[AccountType] [tinyint] NOT NULL,
	[CustomerPaymentType] [tinyint] NOT NULL,
	[SupplierPaymentType] [tinyint] NULL,
	[SupplierCreditLimit] [int] NULL,
	[BillingCycleFrom] [smallint] NULL,
	[BillingCycleTo] [smallint] NULL,
	[GMTTime] [smallint] NULL,
	[IsTOD] [char](1) NULL,
	[IsDeleted] [char](1) NULL,
	[IsOriginatingZonesEnabled] [char](1) NULL,
	[Notes] [text] NULL,
	[NominalCapacityInE1s] [int] NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
	[CarrierGroupID] [int] NULL,
	[RateIncreaseDays] [int] NULL,
	[BankReferences] [varchar](max) NULL,
	[CustomerCreditLimit] [int] NULL,
	[IsPassThroughCustomer] [char](1) NULL,
	[IsPassThroughSupplier] [char](1) NULL,
	[RepresentsASwitch] [char](1) NULL,
	[IsAToZ] [char](1) NULL,
	[NameSuffix] [nvarchar](100) NULL,
	[SupplierRatePolicy] [int] NULL,
	[CustomerGMTTime] [smallint] NULL,
	[ImportEmail] [varchar](320) NULL,
	[ImportSubjectCode] [varchar](50) NULL,
	[IsNettingEnabled] [char](1) NULL,
	[Services] [numeric](6, 2) NULL,
	[ConnectionFees] [money] NULL,
	[CustomerActivateDate] [datetime] NULL,
	[CustomerDeactivateDate] [datetime] NULL,
	[SupplierActivateDate] [datetime] NULL,
	[SupplierDeactivateDate] [datetime] NULL,
 CONSTRAINT [PK_CarrierAccount] PRIMARY KEY CLUSTERED 
(
	[CarrierAccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_CarrierAccount] ON [dbo].[CarrierAccount] 
(
	[ProfileID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CarrierAccount_1] ON [dbo].[CarrierAccount] 
(
	[IsDeleted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Code]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Code](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](20) NOT NULL,
	[ZoneID] [int] NOT NULL,
	[BeginEffectiveDate] [smalldatetime] NOT NULL,
	[EndEffectiveDate] [smalldatetime] NULL,
	[IsEffective]  AS (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_Code] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Code] ON [dbo].[Code] 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Code_BED] ON [dbo].[Code] 
(
	[BeginEffectiveDate] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Code_EED] ON [dbo].[Code] 
(
	[EndEffectiveDate] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Code_ZoneID] ON [dbo].[Code] 
(
	[ZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PricingTemplatePlan]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PricingTemplatePlan](
	[PricingTemplatePlanId] [int] IDENTITY(1,1) NOT NULL,
	[PricingTemplateID] [int] NOT NULL,
	[ZoneID] [int] NULL,
	[ServicesFlag] [smallint] NULL,
	[FromPrice] [decimal](13, 6) NOT NULL,
	[ToPrice] [decimal](13, 6) NOT NULL,
	[Margin] [float] NULL,
	[MaxMargin] [float] NULL,
	[IsPerc] [char](1) NULL,
	[Fixed] [float] NULL,
	[NotContinues] [char](1) NULL,
	[Priority] [smallint] NULL,
	[Description] [varchar](200) NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
	[PricingReferenceRate] [int] NULL,
	[SupplierID] [varchar](5) NULL,
 CONSTRAINT [PK_PricingTemplatePlan1] PRIMARY KEY CLUSTERED 
(
	[PricingTemplatePlanId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Zone]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Zone](
	[ZoneID] [int] IDENTITY(1,1) NOT NULL,
	[CodeGroup] [varchar](20) NULL,
	[Name] [nvarchar](255) NOT NULL,
	[SupplierID] [varchar](5) NOT NULL,
	[ServicesFlag] [smallint] NULL,
	[IsMobile] [char](1) NULL,
	[IsProper] [char](1) NULL,
	[IsSold] [char](1) NULL,
	[BeginEffectiveDate] [smalldatetime] NOT NULL,
	[EndEffectiveDate] [smalldatetime] NULL,
	[IsEffective]  AS (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_Zone] PRIMARY KEY CLUSTERED 
(
	[ZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Zone_CodeGroup] ON [dbo].[Zone] 
(
	[CodeGroup] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Zone_Dates] ON [dbo].[Zone] 
(
	[BeginEffectiveDate] DESC,
	[EndEffectiveDate] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Zone_Name] ON [dbo].[Zone] 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Zone_SupplierID] ON [dbo].[Zone] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CodeMatch]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CodeMatch](
	[Code] [varchar](15) NOT NULL,
	[SupplierCodeID] [bigint] NOT NULL,
	[SupplierZoneID] [int] NOT NULL,
	[SupplierID] [varchar](5) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IDX_CodeMatch_Code] ON [dbo].[CodeMatch] 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_CodeMatch_Supplier] ON [dbo].[CodeMatch] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_CodeMatch_Zone] ON [dbo].[CodeMatch] 
(
	[SupplierZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CarrierAccountConnection]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CarrierAccountConnection](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SwitchID] [tinyint] NULL,
	[CarrierAccountID] [varchar](5) NULL,
	[ConnectionType] [varchar](20) NULL,
	[TAG] [varchar](20) NULL,
	[Value] [varchar](30) NULL,
	[GateWay] [varchar](30) NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_CarrierAccountConnection] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_CarrierAccountConnection_Account] ON [dbo].[CarrierAccountConnection] 
(
	[CarrierAccountID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CarrierAccountConnection_Switch] ON [dbo].[CarrierAccountConnection] 
(
	[SwitchID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Billing_Invoice_Costs]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Billing_Invoice_Costs](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[InvoiceID] [int] NOT NULL,
	[SupplierID] [varchar](5) NULL,
	[Destination] [varchar](100) NULL,
	[FromDate] [datetime] NULL,
	[TillDate] [datetime] NULL,
	[Duration] [numeric](19, 6) NULL,
	[Rate] [decimal](13, 6) NULL,
	[RateType] [char](1) NULL,
	[Amount] [decimal](13, 6) NULL,
	[CurrencyID] [varchar](3) NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
	[NumberOfCalls] [int] NULL,
 CONSTRAINT [PK_Billing_Invoice_Costs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Billing_Invoice_Costs_Invoice] ON [dbo].[Billing_Invoice_Costs] 
(
	[InvoiceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'''N'' for Normal (Peak), ''O'' for Offpeak and ''W'' for Weekend Rate ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Billing_Invoice_Costs', @level2type=N'COLUMN',@level2name=N'RateType'
GO
/****** Object:  Table [dbo].[Route]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Route](
	[RouteID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [varchar](5) NOT NULL,
	[Code] [varchar](15) NULL,
	[OurZoneID] [int] NULL,
	[OurActiveRate] [real] NULL,
	[OurNormalRate] [real] NULL,
	[OurOffPeakRate] [real] NULL,
	[OurWeekendRate] [real] NULL,
	[OurServicesFlag] [smallint] NULL,
	[State] [tinyint] NOT NULL,
	[Updated] [datetime] NULL,
	[IsToDAffected] [char](1) NOT NULL,
	[IsSpecialRequestAffected] [char](1) NOT NULL,
	[IsBlockAffected] [char](1) NOT NULL,
 CONSTRAINT [PK_Route] PRIMARY KEY CLUSTERED 
(
	[RouteID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Route_Code] ON [dbo].[Route] 
(
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Route_Customer] ON [dbo].[Route] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Route_ServicesFlag] ON [dbo].[Route] 
(
	[OurServicesFlag] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Route_Updated] ON [dbo].[Route] 
(
	[Updated] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Route_Zone] ON [dbo].[Route] 
(
	[OurZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SpecialRequest]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SpecialRequest](
	[SpecialRequestID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [varchar](5) NOT NULL,
	[ZoneID] [int] NULL,
	[Code] [varchar](20) NULL,
	[SupplierID] [varchar](5) NOT NULL,
	[Priority] [tinyint] NULL,
	[NumberOfTries] [tinyint] NULL,
	[SpecialRequestType] [tinyint] NOT NULL,
	[BeginEffectiveDate] [smalldatetime] NOT NULL,
	[EndEffectiveDate] [smalldatetime] NULL,
	[IsEffective]  AS (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
	[Percentage] [tinyint] NULL,
	[Reason] [varchar](250) NULL,
	[RouteChangeHeaderID] [int] NULL,
 CONSTRAINT [PK_SpecialRequest] PRIMARY KEY CLUSTERED 
(
	[SpecialRequestID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_SpecialRequest] ON [dbo].[SpecialRequest] 
(
	[BeginEffectiveDate] DESC,
	[EndEffectiveDate] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SpecialRequest_Customer] ON [dbo].[SpecialRequest] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SpecialRequest_Supplier] ON [dbo].[SpecialRequest] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PriceList]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PriceList](
	[PriceListID] [int] IDENTITY(1,1) NOT NULL,
	[SupplierID] [varchar](5) NOT NULL,
	[CustomerID] [varchar](5) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CurrencyID] [varchar](3) NOT NULL,
	[BeginEffectiveDate] [smalldatetime] NULL,
	[EndEffectiveDate] [smalldatetime] NULL,
	[IsEffective]  AS (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
	[SourceFileName] [nvarchar](100) NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_PriceList] PRIMARY KEY CLUSTERED 
(
	[PriceListID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_PriceList_Customer] ON [dbo].[PriceList] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PriceList_Dates] ON [dbo].[PriceList] 
(
	[BeginEffectiveDate] DESC,
	[EndEffectiveDate] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PriceList_Supplier] ON [dbo].[PriceList] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RouteBlock]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RouteBlock](
	[RouteBlockID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [varchar](5) NULL,
	[SupplierID] [varchar](5) NULL,
	[ZoneID] [int] NULL,
	[Code] [varchar](20) NULL,
	[UserID] [int] NULL,
	[UpdateDate] [smalldatetime] NULL,
	[BeginEffectiveDate] [smalldatetime] NULL,
	[EndEffectiveDate] [smalldatetime] NULL,
	[BlockType] [tinyint] NULL,
	[IsEffective]  AS (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
	[timestamp] [timestamp] NULL,
	[Reason] [varchar](250) NULL,
	[RouteChangeHeaderID] [int] NULL,
 CONSTRAINT [PK_RouteBlock] PRIMARY KEY CLUSTERED 
(
	[RouteBlockID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_RouteBlock_Customer] ON [dbo].[RouteBlock] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RouteBlock_Dates] ON [dbo].[RouteBlock] 
(
	[BeginEffectiveDate] DESC,
	[EndEffectiveDate] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RouteBlock_Supplier] ON [dbo].[RouteBlock] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RouteBlock_Zone] ON [dbo].[RouteBlock] 
(
	[ZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RatePlan]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RatePlan](
	[RatePlanID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [varchar](5) NOT NULL,
	[Description] [text] NULL,
	[CurrencyID] [varchar](3) NULL,
	[BeginEffectiveDate] [smalldatetime] NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_RatePlan] PRIMARY KEY CLUSTERED 
(
	[RatePlanID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_RatePlan_Customer] ON [dbo].[RatePlan] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ZoneRate]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ZoneRate](
	[ZoneID] [int] NOT NULL,
	[SupplierID] [varchar](5) NOT NULL,
	[CustomerID] [varchar](5) NOT NULL,
	[NormalRate] [real] NULL,
	[OffPeakRate] [real] NULL,
	[WeekendRate] [real] NULL,
	[ServicesFlag] [smallint] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_ZoneRate_Customer] ON [dbo].[ZoneRate] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ZoneRate_ServicesFlag] ON [dbo].[ZoneRate] 
(
	[ServicesFlag] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ZoneRate_Supplier] ON [dbo].[ZoneRate] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ZoneRate_Zone] ON [dbo].[ZoneRate] 
(
	[ZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Billing_Invoice_Details]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Billing_Invoice_Details](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[InvoiceID] [int] NOT NULL,
	[Destination] [varchar](100) NULL,
	[FromDate] [datetime] NULL,
	[TillDate] [datetime] NULL,
	[Duration] [numeric](19, 6) NULL,
	[Rate] [decimal](13, 6) NULL,
	[RateType] [char](1) NULL,
	[Amount] [decimal](13, 6) NULL,
	[CurrencyID] [varchar](3) NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
	[NumberOfCalls] [int] NULL,
 CONSTRAINT [PK_Billing_Invoice_Details] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Billing_Invoice_Details_Invoice] ON [dbo].[Billing_Invoice_Details] 
(
	[InvoiceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'''N'' for Normal (Peak), ''O'' for Offpeak and ''W'' for Weekend Rate ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Billing_Invoice_Details', @level2type=N'COLUMN',@level2name=N'RateType'
GO
/****** Object:  Table [dbo].[WebSiteMenuItemPermission]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WebSiteMenuItemPermission](
	[ItemID] [int] NOT NULL,
	[PermissionID] [varchar](255) NOT NULL,
 CONSTRAINT [PK_websitemenuitempermission] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC,
	[PermissionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Rate]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Rate](
	[RateID] [bigint] IDENTITY(1,1) NOT NULL,
	[PriceListID] [int] NULL,
	[ZoneID] [int] NULL,
	[Rate] [decimal](9, 5) NULL,
	[OffPeakRate] [decimal](9, 5) NULL,
	[WeekendRate] [decimal](9, 5) NULL,
	[Change] [smallint] NULL,
	[ServicesFlag] [smallint] NULL,
	[BeginEffectiveDate] [smalldatetime] NULL,
	[EndEffectiveDate] [smalldatetime] NULL,
	[IsEffective]  AS (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
	[Notes] [nvarchar](255) NULL,
 CONSTRAINT [PK_Rate] PRIMARY KEY CLUSTERED 
(
	[RateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Rate_Dates] ON [dbo].[Rate] 
(
	[BeginEffectiveDate] DESC,
	[EndEffectiveDate] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Rate_Pricelist] ON [dbo].[Rate] 
(
	[PriceListID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Rate_Zone] ON [dbo].[Rate] 
(
	[ZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlaningRate]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlaningRate](
	[PlanningRateID] [bigint] IDENTITY(1,1) NOT NULL,
	[RatePlanID] [int] NULL,
	[ZoneID] [int] NULL,
	[Rate] [decimal](9, 5) NULL,
	[OffPeakRate] [decimal](9, 5) NULL,
	[WeekendRate] [decimal](9, 5) NULL,
	[ServicesFlag] [smallint] NULL,
	[BeginEffectiveDate] [smalldatetime] NULL,
	[EndEffectiveDate] [smalldatetime] NULL,
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
	[Notes] [nvarchar](255) NULL,
 CONSTRAINT [PK_PlaningRate] PRIMARY KEY CLUSTERED 
(
	[PlanningRateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PlaningRate] ON [dbo].[PlaningRate] 
(
	[RatePlanID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FaultTicketUpdate]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FaultTicketUpdate](
	[FaultTicketUpdateID] [int] IDENTITY(1,1) NOT NULL,
	[FaultTicketID] [int] NOT NULL,
	[SendMail] [char](1) NULL,
	[Email] [varchar](255) NULL,
	[Contact] [varchar](50) NULL,
	[PhoneNo] [varchar](50) NULL,
	[UserID] [int] NULL,
	[Notes] [varchar](max) NULL,
	[Status] [tinyint] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[FileName] [varchar](50) NULL,
	[Attachment] [image] NULL,
 CONSTRAINT [PK_FaultTicketUpdate] PRIMARY KEY CLUSTERED 
(
	[FaultTicketUpdateID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RoutingPoolCustomer]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RoutingPoolCustomer](
	[ID] [int] NOT NULL,
	[CustomerID] [varchar](5) NOT NULL,
 CONSTRAINT [PK_RoutingPoolCustomer] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RoutingPoolSupplier]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RoutingPoolSupplier](
	[ID] [int] NOT NULL,
	[SupplierID] [varchar](5) NOT NULL,
 CONSTRAINT [PK_RoutingPoolSupplier] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserOption]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserOption](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[User] [int] NULL,
	[Option] [nvarchar](255) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_UserOption] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserPermissions]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserPermissions](
	[UserID] [int] NOT NULL,
	[PermissionID] [int] NOT NULL,
 CONSTRAINT [PK_UserPermissions] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[PermissionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[bp_GetPlaningRatesWithSupply]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_GetPlaningRatesWithSupply](
	@CustomerID varchar(10) = NULL, 
	@ExcludedRate float = 10, 
	@ServicesFlag smallint = NULL ,
	@ZoneID int = NULL,
	@CurrencyID varchar(3) = NULL
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	

	DECLARE @LastExchangeRate REAL
	
	DECLARE @ZoneIsPending CHAR(1)
	SET @ZoneIsPending  = ISNULL((SELECT 'Y' FROM Zone z WITH(NOLOCK) WHERE z.ZoneID = @ZoneID AND z.IsEffective = 'N'), 'N')
		
	IF @CurrencyID IS NOT NULL 
		SELECT @LastExchangeRate = LastRate FROM Currency WITH(NOLOCK) WHERE CurrencyID = @CurrencyID
	ELSE 
		SET @LastExchangeRate = 1
	
    DECLARE @OurRates TABLE(ZoneID int PRIMARY KEY, Rate real, OffPeakRate real, WeekendRate real, ServicesFlag smallint);
	
	IF @ZoneIsPending = 'N'
	  BEGIN
		INSERT INTO @OurRates (ZoneID, Rate, OffPeakRate, WeekendRate, ServicesFlag)
			SELECT  PR.ZoneID, 
					PR.Rate * @LastExchangeRate, 
					PR.OffPeakRate * @LastExchangeRate, 
					PR.WeekendRate * @LastExchangeRate, 
					PR.ServicesFlag
				FROM PlaningRate PR WITH(NOLOCK), RatePlan RP WITH(NOLOCK) WHERE
						PR.RatePlanID = RP.RatePlanID
					AND RP.CustomerID= @CustomerID 
					AND (@ZoneID IS NULL OR PR.ZoneID = @ZoneID)
					AND PR.Rate < @ExcludedRate; 

			SELECT
				OZ.ZoneID as OurZoneID, OZ.Name AS OurZone,
				R.Rate as OurNormalRate, R.OffPeakRate as OurOffPeakRate, R.WeekendRate AS OurWeekendRate, R.ServicesFlag AS OurServicesFlag,
				SR.SupplierID, ZM.SupplierZoneID AS SupplierZoneID, SZ.Name AS SupplierZone,
				SR.NormalRate * @LastExchangeRate AS SupplierNormalRate, 
				SR.OffPeakRate * @LastExchangeRate AS SupplierOffPeakRate, 
				SR.WeekendRate * @LastExchangeRate AS SupplierWeekendRate, 
				SR.ServicesFlag AS SupplierServicesFlag
				,RateTable.EndEffectiveDate -- EndEffectiveDate
				,null AS DurationsInMinutes
				,null AS ASR
				,null AS ACD
			FROM
				Zone OZ WITH(NOLOCK)
				LEFT JOIN @OurRates R ON OZ.ZoneID = R.ZoneID AND R.Rate IS NOT NULL
				LEFT JOIN ZoneMatch ZM WITH(NOLOCK) ON ZM.OurZoneID = OZ.ZoneID 
					AND ZM.SupplierZoneID IN 
						(
						SELECT SZ.ZoneID FROM Zone SZ WITH(NOLOCK) WHERE SZ.SupplierID IN 
							(
							SELECT ca.CarrierAccountID FROM CarrierAccount ca WITH(NOLOCK)
								WHERE 
									ca.ActivationStatus <> @Account_Inactive 
								AND ca.RoutingStatus <> @Account_Blocked 
								AND ca.RoutingStatus <> @Account_BlockedOutbound
							)						
						) 
				LEFT JOIN Zone SZ WITH(NOLOCK) ON SZ.ZoneID = ZM.SupplierZoneID
				LEFT JOIN ZoneRate SR WITH(NOLOCK,INDEX(IX_ZoneRate_Zone)) ON 
							ZM.SupplierZoneID = SR.ZoneID 
						AND SR.SupplierID <> ISNULL(@CustomerID,'')
						AND SR.SupplierID <> 'SYS' 
						-- AND (@AllRates = 'Y' OR SR.NormalRate < R.Rate)
						-- AND SR.ServicesFlag & ISNULL(@ServicesFlag, R.ServicesFlag) = ISNULL(@ServicesFlag, R.ServicesFlag) 
				LEFT JOIN Rate RateTable WITH(NOLOCK, INDEX(IX_Rate_Zone)) ON RateTable.ZoneID = SR.ZoneID AND RateTable.IsEffective = 'Y'
			WHERE 
				OZ.SupplierID = 'SYS'
				AND (ZM.SupplierZoneID IS NULL OR SZ.ZoneID IS NOT NULL)
				AND (@ZoneID IS NULL OR OZ.ZoneID = @ZoneID)		
			ORDER BY OurZone, SupplierNormalRate
			OPTION (RECOMPILE)
	  END
	-- Future Zone 
	ELSE
	  BEGIN
		
		DECLARE @BED DATETIME
		DECLARE @DefaultRate REAL 
		SET @DefaultRate = 0.0  
		SELECT @BED = z.BeginEffectiveDate FROM Zone z WITH(NOLOCK) WHERE z.ZoneID = @ZoneID
		
		CREATE TABLE #ExactMatchZones (SupplierZoneID INT PRIMARY KEY, SupplierID VARCHAR(5))
		CREATE INDEX IX_SupplierID ON #ExactMatchZones (SupplierID)
		
		CREATE TABLE #Codes (Code VARCHAR(15) PRIMARY KEY)
		INSERT INTO #Codes SELECT oc.Code FROM Code oc WITH(NOLOCK) WHERE oc.ZoneID = @ZoneID AND oc.BeginEffectiveDate <= @BED AND (oc.EndEffectiveDate IS NULL OR oc.EndEffectiveDate > @BED)
		
		INSERT INTO #ExactMatchZones
			SELECT DISTINCT sz.ZoneID, sz.SupplierID FROM Zone sz WITH(NOLOCK), Code sc WITH(NOLOCK) 
				WHERE sc.ZoneID = sz.ZoneID 
					AND sz.SupplierID <> 'SYS'  
					AND sz.BeginEffectiveDate <= @BED AND (sz.EndEffectiveDate IS NULL OR sz.EndEffectiveDate > @BED)
					AND sc.BeginEffectiveDate <= @BED AND (sc.EndEffectiveDate IS NULL OR sc.EndEffectiveDate > @BED)
					AND sc.Code COLLATE Latin1_General_BIN IN (SELECT oc.Code COLLATE Latin1_General_BIN FROM #Codes oc)
		
		SELECT		
			@ZoneID as OurZoneID, (SELECT OZ.Name FROM Zone oz WITH(NOLOCK) WHERE oz.ZoneID = @ZoneID) AS OurZone,
			@DefaultRate as OurNormalRate, @DefaultRate as OurOffPeakRate, @DefaultRate AS OurWeekendRate, @ServicesFlag AS OurServicesFlag,
			SZ.SupplierID, SZ.ZoneID AS SupplierZoneID, SZ.Name AS SupplierZone,
			cast(R.Rate * @LastExchangeRate AS REAL) AS SupplierNormalRate, 
			cast(R.OffPeakRate * @LastExchangeRate AS REAL) AS SupplierOffPeakRate, 
			cast(R.WeekendRate * @LastExchangeRate AS REAL) AS SupplierWeekendRate, 
			R.ServicesFlag AS SupplierServicesFlag
			,r.EndEffectiveDate 
			,null --TS.DurationsInMinutes
			,null --TS.ASR
			,null --TS.ACD
		FROM Zone SZ WITH(NOLOCK), Rate r WITH(NOLOCK) 
		WHERE 
				sz.SupplierID <> 'SYS'
			AND sz.BeginEffectiveDate <= @BED AND (sz.EndEffectiveDate IS NULL OR sz.EndEffectiveDate > @BED)
			AND sz.SupplierID NOT IN (SELECT ca.CarrierAccountID FROM CarrierAccount ca WITH(NOLOCK) WHERE ca.RoutingStatus IN (@Account_Blocked, @Account_BlockedOutbound) OR ca.ActivationStatus = @Account_Inactive)
			AND NOT EXISTS (SELECT rb.ZoneID FROM RouteBlock rb WITH(NOLOCK) WHERE rb.SupplierID = sz.SupplierID AND rb.ZoneID = sz.ZoneID AND rb.IsEffective = 'Y')
			AND 
			(
				sz.ZoneID IN (SELECT em.SupplierZoneID FROM #ExactMatchZones em WHERE em.SupplierID = sz.SupplierID COLLATE Latin1_General_BIN)
				OR
				EXISTS
				(
					SELECT * FROM Code sc, #Codes oc 
					WHERE sc.ZoneID = sz.ZoneID
						AND sc.BeginEffectiveDate <= @BED AND (sc.EndEffectiveDate IS NULL OR sc.EndEffectiveDate > @BED)
						AND sc.ZoneID NOT IN (SELECT em.SupplierZoneID FROM #ExactMatchZones em)
						AND 
						(
							(sc.Code LIKE (oc.Code + '%') COLLATE Latin1_General_BIN) 
							OR 
							(oc.Code LIKE (sc.Code + '%') COLLATE Latin1_General_BIN)
						) 	
				) 
			)
			AND r.ZoneID = SZ.ZoneID
			AND r.IsEffective ='Y'
			AND r.BeginEffectiveDate <= @BED AND (r.EndEffectiveDate IS NULL OR r.EndEffectiveDate > @BED)
			-- AND r.ServicesFlag & ISNULL(@ServicesFlag, 0) = ISNULL(@ServicesFlag, 0) 

		ORDER BY SupplierNormalRate ASC
		
		DROP TABLE #ExactMatchZones
		
	  END
END
GO
/****** Object:  StoredProcedure [dbo].[bp_CreateRatePlan]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Author:		Fadi Chamieh
-- Create date: 01/10/2007
-- Description:	Create a Rate Plan for a customer or a policy from the 
--              effective rates this customer or policy has. 
-- ========================================================================
CREATE PROCEDURE [dbo].[bp_CreateRatePlan](@CustomerID varchar(10) = NULL, @Currency VARCHAR(3) = NULL, @RatePlanID int output)
AS
BEGIN
	SET NOCOUNT ON;

	-- Check if a rate plan already exists for customer or policy
	SELECT @RatePlanID = RatePlanID	FROM RatePlan WHERE CustomerID = @CustomerID
	
	DECLARE @CurrencyID varchar(3) 
	
	IF(@Currency IS NULL )
	SELECT @CurrencyID = CurrencyID 
			FROM CarrierProfile   
			WHERE CarrierProfile.ProfileID IN 
							(SELECT ProfileID FROM CarrierAccount ca WHERE ca.CarrierAccountID=@CustomerID)
	ELSE
	SET @CurrencyID = @Currency
		
	-- Empty Planning Rates if any
	IF @RatePlanID IS NOT NULL
		RETURN
	ELSE
	BEGIN
		INSERT INTO RatePlan ( CustomerID, CurrencyID, BeginEffectiveDate )
		VALUES ( @CustomerID, @CurrencyID, getdate() ) 
		SELECT @RatePlanID = @@IDENTITY
	End		
		
	-- Current Rates
	INSERT INTO PlaningRate
		(
		RatePlanID,
		ZoneID,
		ServicesFlag,
		Rate,
		OffPeakRate,
		WeekendRate,
		BeginEffectiveDate,
		EndEffectiveDate
		)	
	SELECT 
		@RatePlanID,
		R.ZoneID,
		MAX(ServicesFlag),
		0, -- R.Rate / C.LastRate,
		0, -- R.OffPeakRate / C.LastRate,
		0, -- R.WeekendRate / C.LastRate,
		MAX(R.BeginEffectiveDate),
		MIN(R.EndEffectiveDate)
	 FROM Rate R, PriceList P 
		WHERE 
			P.CustomerID = @CustomerID
			AND P.PriceListID=R.PriceListID
			AND	R.IsEffective = 'Y'
	GROUP BY R.ZoneID 
	
	-- Pending Rates?
	INSERT INTO PlaningRate
		(
		RatePlanID,
		ZoneID,
		ServicesFlag,
		Rate,
		OffPeakRate,
		WeekendRate,
		BeginEffectiveDate,
		EndEffectiveDate
		)	
	SELECT 
		@RatePlanID,
		R.ZoneID,
		MAX(ServicesFlag),
		0, -- R.Rate / C.LastRate,
		0, -- R.OffPeakRate / C.LastRate,
		0, -- R.WeekendRate / C.LastRate,
		MAX(R.BeginEffectiveDate),
		MIN(R.EndEffectiveDate)
	 FROM Rate R, PriceList P 
		WHERE 
			R.ZoneID NOT IN(SELECT Prv.ZoneID FROM PlaningRate Prv WHERE Prv.RatePlanID = @RatePlanID)
			AND P.CustomerID = @CustomerID
			AND P.PriceListID = R.PriceListID
			AND	R.IsEffective = 'N'
			AND R.EndEffectiveDate IS NULL
	GROUP BY R.ZoneID 

END
GO
/****** Object:  StoredProcedure [dbo].[bp_GetPostPaidAccountStats]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- 
CREATE PROCEDURE [dbo].[bp_GetPostPaidAccountStats]
    @ShowCustomerTotal char(1) = 'Y',
	@ShowSupplierTotal char(1) = 'Y'
AS

	DECLARE @PostpaidType int
	SELECT @PostpaidType = e.Value
	  FROM Enumerations e 
			WHERE e.Enumeration = 'TABS.PaymentType' 
			AND [Name] = 'Postpaid'

	DECLARE @PostPaidStats TABLE
	(
		CarrierID varchar(10),
		ProfileID int,
		[Type] varchar(10) NOT NULL,
		CarrierName varchar(255),
		CurrencyID varchar(3),
		LastPaid smalldatetime NULL,
		UnpaidInvoicesNumber int NULL,
		UnpaidInvoicesAmount numeric(13,5) NULL,
		CreditLimit numeric(13,5) NULL,
		InvoiceDelayDays int NULL,
		LastDueDate smalldatetime NULL,
		Sale numeric(13,5) NULL,
		Purchase numeric(13,5) NULL,
		Net numeric(13,5) NULL,
        Tolerance numeric(13,5) NULL
	)
	
	 -- Customer
	INSERT INTO @PostPaidStats (
		CarrierID,
		ProfileID,
		[Type],
		CarrierName,
		CurrencyID,
		LastPaid,
		UnpaidInvoicesNumber, 
		UnpaidInvoicesAmount, 
		CreditLimit, 
		InvoiceDelayDays,
		LastDueDate,
		Sale,
		Purchase
	)
	SELECT 
		Ca.[CarrierAccountID] AS CarrierID,
		Ca.ProfileID,
		'Customer',
		Cp.Name,	
		Cp.CurrencyID,
		
		(SELECT TOP 1 PaidDate FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID AND bi.IsPaid = 'Y' ORDER BY bi.PaidDate DESC) 
			AS [LastPaid],
		
		(SELECT Count(*) FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID AND bi.IsPaid = 'N') 
			AS [UnpaidInvoicesNumber],
		
		(SELECT Sum(bi.Amount) FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID AND bi.IsPaid = 'N') 
			AS [UnpaidInvoicesAmount],
		
		ca.CustomerCreditLimit,
		
		(SELECT TOP 1 datediff(dd, getdate(), bi.DueDate) FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID AND bi.IsPaid = 'N' ORDER BY bi.DueDate) 
			AS [Invoice Delay],
		
		(SELECT TOP 1 bi.DueDate FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID AND bi.IsPaid = 'N' ORDER BY bi.DueDate) 
			AS [LastDueDate],

		(SELECT Sum(bs.Sale_Nets) FROM Billing_Stats bs WHERE bs.CustomerID = ca.CarrierAccountID 
			AND bs.CallDate > ISNULL((SELECT Max(bi.EndDate) FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID),'1990-01-01')
			) AS [Sale], 
		
		(SELECT Sum(bs.Cost_Nets) FROM Billing_Stats bs WHERE bs.SupplierID = ca.CarrierAccountID 
			AND bs.CallDate > ISNULL((SELECT Max(bi.EndDate) FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID),'1990-01-01')
			) AS [Purchase] 
	
	FROM CarrierAccount ca, CarrierProfile cp  
		WHERE 
				(ca.CustomerPaymentType = @PostpaidType OR cp.CustomerPaymentType = @PostpaidType)				 
			AND ca.ProfileID = cp.ProfileID
			AND (ca.CustomerCreditLimit > 0 OR cp.CustomerCreditLimit > 0)
	ORDER BY cp.Name
	
   -- supplier
      INSERT INTO @PostPaidStats (
		CarrierID, 
		ProfileID,
		[Type],
		CarrierName,
		CurrencyID,
		LastPaid,
		UnpaidInvoicesNumber, 
		UnpaidInvoicesAmount, 
		CreditLimit, 
		InvoiceDelayDays,
		LastDueDate,
		Sale,
		Purchase
	)
	SELECT 
		Ca.[CarrierAccountID] AS CarrierID,
		Ca.ProfileID,
		'Supplier',
		Cp.Name,	
		Cp.CurrencyID,
		
		(SELECT TOP 1 PaidDate FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID AND bi.IsPaid = 'Y' ORDER BY bi.PaidDate DESC) 
			AS [LastPaid],
		
		(SELECT Count(*) FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID AND bi.IsPaid = 'N') 
			AS [UnpaidInvoicesNumber],
		
		(SELECT Sum(bi.Amount) FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID AND bi.IsPaid = 'N') 
			AS [UnpaidInvoicesAmount],
		
		ca.SupplierCreditLimit,
		
		(SELECT TOP 1 datediff(dd, getdate(), bi.DueDate) FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID AND bi.IsPaid = 'N' ORDER BY bi.DueDate) 
			AS [Invoice Delay],
		
		(SELECT TOP 1 bi.DueDate FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID AND bi.IsPaid = 'N' ORDER BY bi.DueDate) 
			AS [LastDueDate],

		(SELECT Sum(bs.Sale_Nets) FROM Billing_Stats bs WHERE bs.CustomerID = ca.CarrierAccountID 
			AND bs.CallDate > ISNULL((SELECT Max(bi.EndDate) FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID),'1990-01-01')
			) AS [Sale], 
		
		(SELECT Sum(bs.Cost_Nets) FROM Billing_Stats bs WHERE bs.SupplierID = ca.CarrierAccountID 
			AND bs.CallDate > ISNULL((SELECT Max(bi.EndDate) FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID),'1990-01-01')
			) AS [Purchase] 
	
	FROM CarrierAccount ca, CarrierProfile cp  
		WHERE 
				(ca.SupplierPaymentType = @PostpaidType OR cp.SupplierPaymentType = @PostpaidType)
			AND ca.ProfileID = cp.ProfileID
			AND (ca.SupplierCreditLimit > 0 OR cp.SupplierCreditLimit > 0 )
	ORDER BY cp.Name
	
	UPDATE @PostPaidStats SET Net = ISNULL(Sale,0) - ISNULL(Purchase,0)
	UPDATE @PostPaidStats SET Tolerance = (case WHEN CreditLimit = 0 then 0 else (ISNULL(Net,0) * 100) / CreditLimit end )

   IF @ShowCustomerTotal = 'Y'   
		SELECT * FROM @PostPaidStats WHERE Type='Customer'
   IF @ShowSupplierTotal = 'Y'
		SELECT * FROM @PostPaidStats WHERE Type='Supplier'
GO
/****** Object:  StoredProcedure [dbo].[bp_PostpaidCarrierTotal]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ======================================================================
-- Author: Mohammad El-Shab
-- Description: 
-- ======================================================================
CREATE PROCEDURE [dbo].[bp_PostpaidCarrierTotal]
    @ShowCustomerTotal char(1) = 'Y',
	@ShowSupplierTotal char(1) = 'Y',
	@IsNettingEnabled CHAR(1) = 'N',
	@FromDate SMALLDATETIME = NULL,
	@ToDate SMALLDATETIME = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	IF @IsNettingEnabled = 'N'
	BEGIN
		DECLARE @BillingType SMALLINT
		SELECT @BillingType = e.[Value] FROM Enumerations e WHERE e.Enumeration = 'TABS.AmountType' AND e.[Name] = 'Billing'
		IF @ShowCustomerTotal = 'Y'
			SELECT     
				PA.CustomerID AS CarrierID,
				PA.CustomerProfileID AS ProfileID,
				SUM(PA.Amount) AS Balance,
				PA.CurrencyID AS Currency,
				(case WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END) AS CustomerCreditLimit,
				--ABS(ISNULL((CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END),0)) - ABS(SUM(PA.Amount)) AS Tolerance Old Fadi
				ISNULL((CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END),0) + SUM(PA.Amount) AS Tolerance,
				SUM(CASE WHEN PA.[Type] = @BillingType THEN PA.Amount ELSE 0 END) AS BillingAsCustomer,
				SUM(CASE WHEN PA.[Type] != @BillingType THEN PA.Amount ELSE 0 END) AS PaymentAsCustomer
			FROM
				PostpaidAmount PA 
					LEFT JOIN CarrierAccount CA ON CA.CarrierAccountID = PA.CustomerID --AND CA.IsNettingEnabled = @IsNettingEnabled
					LEFT JOIN CarrierProfile CP ON CP.ProfileID = PA.CustomerProfileID --AND CP.IsNettingEnabled = @IsNettingEnabled
			WHERE
				(PA.CustomerID IS NOT NULL OR PA.CustomerProfileID IS NOT NULL) 
				AND (CP.IsNettingEnabled = @IsNettingEnabled OR CA.IsNettingEnabled = @IsNettingEnabled)
				AND PA.Date >= (CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerActivateDate ELSE CP.CustomerActivateDate END)
				AND ((CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerDeactivateDate ELSE CP.CustomerDeactivateDate END) IS NULL 
				OR (CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerDeactivateDate ELSE CP.CustomerDeactivateDate END) > PA.Date)
				GROUP BY	
				PA.CustomerProfileID,
				PA.CustomerID,
				PA.CurrencyID, 
				(CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END)
			ORDER BY Tolerance ASC
		IF @ShowSupplierTotal = 'Y'
			SELECT
				 PA.SupplierID AS CarrierID,
				 PA.SupplierProfileID AS ProfileID,
				 SUM(PA.Amount) AS Balance,
				 PA.CurrencyID AS Currency,
				 (CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierCreditLimit ELSE CP.SupplierCreditLimit END) AS SupplierCreditLimit,
				 --ABS(ISNULL((CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierCreditLimit ELSE CP.SupplierCreditLimit END),0)) - ABS(SUM(PA.Amount)) AS Tolerance Old Fadi
				ISNULL((CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierCreditLimit ELSE CP.SupplierCreditLimit END),0) - SUM(PA.Amount) AS Tolerance,
				SUM(CASE WHEN PA.[Type] = @BillingType THEN PA.Amount ELSE 0 END) AS BillingAsSupplier,
				SUM(CASE WHEN PA.[Type] != @BillingType THEN PA.Amount ELSE 0 END) AS PaymentAsSupplier
			FROM 
				PostpaidAmount PA 
				LEFT JOIN CarrierAccount CA ON CA.CarrierAccountID = PA.SupplierID --AND CA.IsNettingEnabled = @IsNettingEnabled
				LEFT JOIN CarrierProfile CP ON CP.ProfileID = PA.SupplierProfileID --AND CP.IsNettingEnabled = @IsNettingEnabled 
			WHERE
				(PA.SupplierID IS NOT NULL OR PA.SupplierProfileID IS NOT NULL) 
				AND (CP.IsNettingEnabled = @IsNettingEnabled OR CA.IsNettingEnabled = @IsNettingEnabled)
				AND PA.Date >= (CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierActivateDate ELSE CP.SupplierActivateDate END)
				AND ((CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierDeactivateDate ELSE CP.SupplierDeactivateDate END) IS NULL 
				OR (CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierDeactivateDate ELSE CP.SupplierDeactivateDate END) > PA.Date)
			GROUP BY   
				PA.SupplierProfileID,
				PA.SupplierID,
				PA.CurrencyID, 
				(CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierCreditLimit ELSE CP.SupplierCreditLimit END)		
			ORDER BY Tolerance ASC
	END
	ELSE
		BEGIN
			DECLARE @Amounts TABLE (
					CarrierID VARCHAR(10) NULL, 
					ProfileID SMALLINT NULL, 
					Balance NUMERIC(13,5) NULL, 
					BillingAsCustomer NUMERIC(13,5) NULL,  
					BillingAsSupplier NUMERIC(13,5) NULL,
					Currency VARCHAR(3) NULL,
					CustomerCreditLimit INT NULL,
					SupplierCreditLimit INT NULL,
					Tolerance NUMERIC(13,5) NULL,
					PaymentAsCustomer NUMERIC(13,5) NULL, 
					PaymentAsSupplier NUMERIC(13,5) NULL
			)
			
			DECLARE @BillingAmountType SMALLINT
			SELECT @BillingAmountType = e.[Value] FROM Enumerations e WHERE e.Enumeration = 'TABS.AmountType' AND e.[Name] = 'Billing'
			DECLARE @PaymentAmountType SMALLINT
			SELECT @PaymentAmountType = e.[Value] FROM Enumerations e WHERE e.Enumeration = 'TABS.AmountType' AND e.[Name] = 'Payment'
			
			INSERT INTO @Amounts 
				-- Customer Account
				SELECT 
					pa.CustomerID, 
					NULL, 
					SUM(PA.Amount) AS Balance,
					SUM(CASE WHEN PA.[Type] = @BillingAmountType THEN Pa.Amount ELSE 0 END) AS BillingAsCustomer, --AND dbo.IsAPaidDate(PA.Date, ca.CarrierAccountID, NULL, NULL, NULL) = 'N'
					0.0 AS BillingAsSupplier, --AND dbo.IsAPaidDate(PA.Date, NULL, ca.CarrierAccountID, NULL, NULL) = 'N'
					PA.CurrencyID AS Currency,	
					ca.CustomerCreditLimit,
					ca.SupplierCreditLimit,
					0 AS Tolerance,
					SUM(CASE WHEN PA.[Type] != @BillingAmountType THEN Pa.Amount ELSE 0 END) AS PaymentAsCustomer, --AND dbo.IsAPaidDate(PA.Date, ca.CarrierAccountID, NULL, NULL, NULL) = 'N'
					0.0 AS PaymentAsSupplier --AND dbo.IsAPaidDate(PA.Date, NULL, ca.CarrierAccountID, NULL, NULL) = 'N'
					FROM PostpaidAmount pa, CarrierAccount ca 
				WHERE pa.CustomerID = ca.CarrierAccountID AND ca.IsNettingEnabled = @IsNettingEnabled
				AND pa.Date >= ca.CustomerActivateDate 
				AND (ca.CustomerDeactivateDate IS NULL OR ca.CustomerDeactivateDate > pa.Date)
				AND pa.CustomerID IS NOT NULL
				GROUP BY pa.CustomerID, pa.CurrencyID, ca.CustomerCreditLimit, ca.SupplierCreditLimit   
				UNION ALL
				-- Customer Profile
				SELECT 
					NULL, 
					pa.CustomerProfileID, 
					SUM(PA.Amount) AS Balance,
					SUM(CASE WHEN PA.[Type] = @BillingAmountType THEN Pa.Amount ELSE 0 END) AS BillingAsCustomer, --AND dbo.IsAPaidDate(PA.Date, NULL, NULL, cp.ProfileID, NULL) = 'N'
					0.0 AS BillingAsSupplier, --AND dbo.IsAPaidDate(PA.Date, NULL, NULL, NULL, cp.ProfileID) = 'N'
					PA.CurrencyID AS Currency,	
					cp.CustomerCreditLimit,
					cp.SupplierCreditLimit,
					0 AS Tolerance,
					SUM(CASE WHEN PA.[Type] != @BillingAmountType THEN Pa.Amount ELSE 0 END) AS PaymentAsCustomer, --AND dbo.IsAPaidDate(PA.Date, NULL, NULL, cp.ProfileID, NULL) = 'N'
					0.0 AS PaymentAsSupplier					 --AND dbo.IsAPaidDate(PA.Date, NULL, NULL, NULL, cp.ProfileID) = 'N'
					FROM PostpaidAmount pa, CarrierProfile cp 
				WHERE pa.CustomerProfileID = cp.ProfileID AND cp.IsNettingEnabled = @IsNettingEnabled 
				AND pa.Date >= cp.CustomerActivateDate 
				AND (cp.CustomerDeactivateDate IS NULL OR cp.CustomerDeactivateDate > pa.Date)  
				AND pa.CustomerProfileID IS NOT NULL
				GROUP BY pa.CustomerProfileID, pa.CurrencyID, cp.CustomerCreditLimit, cp.SupplierCreditLimit
				UNION ALL
				-- Supplier Account
				SELECT 
					pa.SupplierID, 
					NULL, 
					SUM(PA.Amount) AS Balance,
					0.0 AS BillingAsCustomer, --AND dbo.IsAPaidDate(PA.Date, ca.CarrierAccountID, NULL, NULL, NULL) = 'N'
					SUM(CASE WHEN PA.[Type] = @BillingAmountType THEN Pa.Amount ELSE 0 END) AS BillingAsSupplier, --AND dbo.IsAPaidDate(PA.Date, NULL, ca.CarrierAccountID, NULL, NULL) = 'N'
					PA.CurrencyID AS Currency,	
					ca.CustomerCreditLimit,
					ca.SupplierCreditLimit,
					0 AS Tolerance,
					0.0 AS PaymentAsCustomer, --AND dbo.IsAPaidDate(PA.Date, ca.CarrierAccountID, NULL, NULL, NULL) = 'N'
					SUM(CASE WHEN PA.[Type] != @BillingAmountType THEN Pa.Amount ELSE 0 END) AS PaymentAsSupplier --AND dbo.IsAPaidDate(PA.Date, NULL, ca.CarrierAccountID, NULL, NULL) = 'N'
					FROM PostpaidAmount pa, CarrierAccount ca 
				WHERE pa.SupplierID = ca.CarrierAccountID AND ca.IsNettingEnabled = @IsNettingEnabled
				AND pa.Date >= ca.SupplierActivateDate 
				AND (ca.SupplierDeactivateDate IS NULL OR ca.SupplierDeactivateDate > pa.Date)  
				AND pa.SupplierID IS NOT NULL
				GROUP BY pa.SupplierID, pa.CurrencyID, ca.CustomerCreditLimit, ca.SupplierCreditLimit   
				UNION ALL
				-- Supplier Profile
				SELECT 
					NULL, 
					pa.SupplierProfileID, 
					SUM(PA.Amount) AS Balance,
					0.0 AS BillingAsCustomer, --AND dbo.IsAPaidDate(PA.Date, NULL, NULL, cp.ProfileID, NULL) = 'N'
					SUM(CASE WHEN PA.[Type] = @BillingAmountType THEN Pa.Amount ELSE 0 END) AS BillingAsSupplier, --AND dbo.IsAPaidDate(PA.Date, NULL, NULL, NULL, cp.ProfileID) = 'N' 
					PA.CurrencyID AS Currency,	
					cp.CustomerCreditLimit,
					cp.SupplierCreditLimit,
					0 AS Tolerance,
					0.0 AS PaymentAsCustomer, --AND dbo.IsAPaidDate(PA.Date, NULL, NULL, cp.ProfileID, NULL) = 'N'
					SUM(CASE WHEN PA.[Type] != @BillingAmountType THEN Pa.Amount ELSE 0 END) AS PaymentAsSupplier  --AND dbo.IsAPaidDate(PA.Date, NULL, NULL, NULL, cp.ProfileID) = 'N'
					FROM PostpaidAmount pa, CarrierProfile cp 
				WHERE pa.SupplierProfileID = cp.ProfileID AND cp.IsNettingEnabled = @IsNettingEnabled   
				AND pa.Date >= cp.SupplierActivateDate 
				AND (cp.SupplierDeactivateDate IS NULL OR cp.SupplierDeactivateDate > pa.Date)  
				AND pa.SupplierProfileID IS NOT NULL
				GROUP BY pa.SupplierProfileID, pa.CurrencyID, cp.CustomerCreditLimit, cp.SupplierCreditLimit
			
			SELECT 
				am.CarrierID,
				am.ProfileID,
				Sum(am.Balance) AS Balance,
				SUM(am.BillingAsCustomer) AS BillingAsCustomer,
				SUM(am.BillingAsSupplier) AS BillingAsSupplier,
				am.Currency,
				am.CustomerCreditLimit,
				am.SupplierCreditLimit,
				CASE WHEN Sum(am.Balance) > 0 THEN am.SupplierCreditLimit - SUM(am.Balance) ELSE am.CustomerCreditLimit  + SUM(am.Balance) END AS Tolerance,
				SUM(am.PaymentAsCustomer) AS PaymentAsCustomer,
				SUM(am.PaymentAsSupplier) AS PaymentAsSupplier
			FROM @Amounts am
			GROUP BY CarrierID, ProfileID, Currency, CustomerCreditLimit, SupplierCreditLimit
			ORDER BY Tolerance  			
		END
END

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
/****** Object:  StoredProcedure [dbo].[SP_CapacityAndConnectivity_MainReport]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[SP_CapacityAndConnectivity_MainReport] 
    @FromDateTime	datetime,
	@ToDateTime		datetime,
	@CarrierAccountID varchar(10),
	@SupplierID		varchar(10) = NULL,
    @SwitchID		tinyInt = NULL,
    @OurZoneID 		INT = NULL,
    @CodeGroup		varchar(10) = NULL,
    @ConnectionType VARCHAR(20) = 'VoIP',
    @PortInOut		VARCHAR(50) = NULL,
	@SelectPort		int = NULL,
	@GroupByGateWay CHAR(1) = 'N',
	@GateWayName VARCHAR(255) = NULL,
	@RemoveInterconnectedData CHAR(1) = 'N'
WITH RECOMPILE
AS
BEGIN	
	SET NOCOUNT ON

	DECLARE @Connectivities TABLE 
	(
		GateWay VARCHAR(250),
		Details VARCHAR(MAX),
		Date SMALLDATETIME,
		NumberOfChannels_In INT,
		NumberOfChannels_Out int,
		NumberOfChannels_Total int,
		Margin_Total INT,
		DetailList VARCHAR(MAX),
		InterconnectedSwithes CHAR(1)
	)
	
	SET @FromDateTime = dbo.DateOf(@FromDateTime)
	
	SET @ToDateTime= CAST(
     (
     STR( YEAR( @ToDateTime ) ) + '-' +
     STR( MONTH(@ToDateTime ) ) + '-' +
     STR( DAY( @ToDateTime ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	;
	
	DECLARE @ConnectivityValue TINYINT
	SELECT  @ConnectivityValue = Value FROM Enumerations e WHERE e.Enumeration = 'TABS.ConnectionType' AND e.[Name] = @ConnectionType; 

DECLARE @IPpattern VARCHAR(10)
SET @IPpattern = '%.%.%.%'


INSERT INTO @Connectivities
 SELECT c.*, ',' + c.Details + ',' AS DetailList  ,'N' 
 from dbo.GetDailyConnectivity(
 		@ConnectivityValue,
 		@CarrierAccountID,
 		@SwitchID,
 		@GroupByGateWay, 
 		@FromDateTime,
 		@ToDateTime,
        'N') c   
 IF @RemoveInterconnectedData = 'Y'
 INSERT INTO @Connectivities
 SELECT c.*, ',' + c.Details + ',' AS DetailList  ,'Y' 
 from dbo.GetDailyConnectivity(
 		null,
 		null,
 		@SwitchID,
 		'N', 
 		@FromDateTime,
 		@ToDateTime,
        'Y') c ; 



-- Create Customer Stats
SELECT 
		SwitchId,
		Port_IN,
		Port_OUT,
		CustomerID,
		OurZoneID,
		SupplierID,
		FirstCDRAttempt,
		Attempts,
		SuccessfulAttempts,
		DurationsInSeconds,
		UtilizationInSeconds,
		NumberOfCalls
	INTO #CarrierTraffic
	FROM TrafficStats ts WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst,IX_TrafficStats_Customer))
	WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND ts.CustomerID = @CarrierAccountID
		AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)      
		AND (@SupplierID is null or TS.SupplierID = @SupplierID)
	
-- Create Supplier Stats
INSERT INTO #CarrierTraffic
	SELECT 
		SwitchId,
		Port_IN,
		Port_OUT,
		CustomerID,
		OurZoneID,
		SupplierID,
		FirstCDRAttempt,
		Attempts,
		SuccessfulAttempts,
		DurationsInSeconds,
		UtilizationInSeconds,
		NumberOfCalls
	 FROM TrafficStats ts WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst,IX_TrafficStats_Supplier))
	WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND ts.SupplierID = @CarrierAccountID
		AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)      
		AND (@SupplierID is null or TS.SupplierID = @SupplierID)

IF @ConnectionType LIKE 'VoIP'
BEGIN 	
WITH InterConnect AS 
(
	SELECT * FROM @Connectivities WHERE InterconnectedSwithes = 'N'
)

	SELECT
			datepart(hour,FirstCDRAttempt) AS Period,
			dbo.DateOf(FirstCDRAttempt) AS [Date],
			GateWay = CASE WHEN  @GroupByGateWay = 'Y' OR @GateWayName IS NOT NULL THEN DC.GateWay
			               ELSE  null END,
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern  THEN ts.NumberOfCalls ELSE 0 END)   AS InAttempts,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END)   AS OutAttempts,
	        
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern  THEN ts.SuccessfulAttempts ELSE 0 END) AS  InSuccesfulAttempts,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) AS  OutSuccesfulAttempts,
	        
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			Cast(NULLIF(SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END) + 0.0, 0) AS NUMERIC) AS InASR,
	        
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			cast(NULLIF(SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END)+ 0.0, 0) AS NUMERIC) AS OutASR,
	       
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS InDurationsInMinutes,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS OutDurationsInMinutes, 
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS InUtilizationsInMinutes,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS OutUtilizationsInMinutes,
			NumberOfChannels_In = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_In) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_In) FROM InterConnect CSC WHERE CSC.Date = 	dbo.DateOf(FirstCDRAttempt) )
								       ELSE 0 END,
			NumberOfChannels_Out = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_Out) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_Out) FROM InterConnect CSC WHERE CSC.Date = 	dbo.DateOf(FirstCDRAttempt) )
								       ELSE 0 END,
			NumberOfChannels_Total = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_Total) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_Total) FROM InterConnect CSC WHERE CSC.Date = 	dbo.DateOf(FirstCDRAttempt) )
								       ELSE 0 END
	   FROM #CarrierTraffic AS TS -- WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
			LEFT JOIN Zone AS Z WITH(NOLOCK) ON TS.OurZoneID = Z.ZoneID  
			LEFT JOIN @Connectivities DC ON  DC.Date = dbo.DateOf(FirstCDRAttempt) AND DC.InterconnectedSwithes = 'N'
									  AND (@GateWayName IS NULL OR (Dc.gateway = @GateWayName))
		    LEFT JOIN @Connectivities DCI ON  DCI.Date = dbo.DateOf(FirstCDRAttempt) AND DCI.InterconnectedSwithes = 'Y'	
	   WHERE   
				(@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
			AND	1 = (CASE  WHEN @SelectPort  = 0 AND TS.CustomerID = @CarrierAccountID AND TS.Port_In like @PortInOut THEN 1     
		                   WHEN @SelectPort  = 1 AND TS.SupplierID = @CarrierAccountID AND TS.Port_Out like @PortInOut THEN 1 
		                   WHEN (@SelectPort  = 2 AND TS.CustomerID = @CarrierAccountID AND TS.Port_In like @PortInOut)
		                        OR (@SelectPort  = 2 AND TS.SupplierID = @CarrierAccountID AND TS.Port_Out like @PortInOut) THEN 1 
		                   ELSE 0 END 
			) 
		    AND ( DC.DetailList IS NULL OR
		    	  (
		    	  	     TS.CustomerID = @CarrierAccountID 
		    	  	AND  DC.DetailList LIKE ( CASE @GroupByGateWay WHEN 'Y' THEN ('%,' + TS.Port_IN + ',%') ELSE '%%' END)
		    	    AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_Out + ',%')
		    	  )
		          OR 
		          (
		          	     TS.SupplierID = @CarrierAccountID 
		            AND  DC.DetailList LIKE ( CASE @GroupByGateWay WHEN 'Y' THEN ('%,' + TS.Port_Out + ',%') ELSE '%%' END)
		             AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_In + ',%'))
		        )
		GROUP BY datepart(hour,FirstCDRAttempt)
		        ,dbo.DateOf(TS.FirstCDRAttempt)
			    ,CASE WHEN  @GroupByGateWay = 'Y' OR @GateWayName IS NOT NULL THEN DC.GateWay
			          ELSE  null END

END 
IF @ConnectionType LIKE 'TDM'
BEGIN 
WITH InterConnect AS 
(
	SELECT * FROM @Connectivities WHERE InterconnectedSwithes = 'N'
)

SELECT
			datepart(hour,FirstCDRAttempt) AS Period,
			dbo.DateOf(FirstCDRAttempt) AS [Date],
			GateWay = CASE WHEN  @GroupByGateWay = 'Y' OR @GateWayName IS NOT NULL THEN DC.GateWay
			               ELSE  null END,
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not LIKE @IPpattern  THEN ts.NumberOfCalls ELSE 0 END)   AS InAttempts,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END)   AS OutAttempts,
	        
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not LIKE @IPpattern  THEN ts.SuccessfulAttempts ELSE 0 END) AS  InSuccesfulAttempts,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) AS  OutSuccesfulAttempts,
	        
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not  LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			Cast(NULLIF(SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END) + 0.0, 0) AS NUMERIC) AS InASR,
	        
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			cast(NULLIF(SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END)+ 0.0, 0) AS NUMERIC) AS OutASR,
	       
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not LIKE @IPpattern THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS InDurationsInMinutes,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not  LIKE @IPpattern THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS OutDurationsInMinutes, 
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not  LIKE @IPpattern THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS InUtilizationsInMinutes,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not  LIKE @IPpattern THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS OutUtilizationsInMinutes,
			NumberOfChannels_In = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_In) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_In) FROM InterConnect CSC WHERE CSC.Date = dbo.DateOf(FirstCDRAttempt) )
								       ELSE 0 END,
			NumberOfChannels_Out = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_Out) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_Out) FROM InterConnect CSC WHERE CSC.Date = dbo.DateOf(FirstCDRAttempt) )
								       ELSE 0 END,
			NumberOfChannels_Total = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_Total) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_Total) FROM InterConnect CSC WHERE CSC.Date = dbo.DateOf(FirstCDRAttempt))
								       ELSE 0 END
	   FROM #CarrierTraffic AS TS -- WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
			LEFT JOIN Zone AS Z WITH(NOLOCK) ON TS.OurZoneID = Z.ZoneID  
			LEFT JOIN @Connectivities DC ON  DC.Date = dbo.DateOf(FirstCDRAttempt) AND DC.InterconnectedSwithes = 'N'
									  AND (@GateWayName IS NULL OR (Dc.gateway = @GateWayName)) 
		    LEFT JOIN @Connectivities DCI ON  DCI.Date = dbo.DateOf(FirstCDRAttempt) AND DCI.InterconnectedSwithes = 'Y'	                                   
	   WHERE   
				(@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
			AND	1 = (CASE  WHEN @SelectPort  = 0 AND TS.CustomerID = @CarrierAccountID AND TS.Port_In like @PortInOut THEN 1     
		                   WHEN @SelectPort  = 1 AND TS.SupplierID = @CarrierAccountID AND TS.Port_Out like @PortInOut THEN 1 
		                   WHEN (@SelectPort  = 2 AND TS.CustomerID = @CarrierAccountID AND TS.Port_In like @PortInOut)
		                        OR (@SelectPort  = 2 AND TS.SupplierID = @CarrierAccountID AND TS.Port_Out like @PortInOut) THEN 1 
		                   ELSE 0 END 
			) 
		    AND ( DC.DetailList IS NULL OR
		    	  (
		    	  	     TS.CustomerID = @CarrierAccountID 
		    	  	AND  DC.DetailList LIKE ( CASE @GroupByGateWay WHEN 'Y' THEN ('%,' + TS.Port_IN + ',%') ELSE '%%' END)
		    	    AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_Out + ',%')
		    	  )
		          OR 
		          (
		          	     TS.SupplierID = @CarrierAccountID 
		            AND  DC.DetailList LIKE ( CASE @GroupByGateWay WHEN 'Y' THEN ('%,' + TS.Port_Out + ',%') ELSE '%%' END)
		             AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_In + ',%')
		    )
		        )
			    
		GROUP BY datepart(hour,FirstCDRAttempt)
		        ,dbo.DateOf(TS.FirstCDRAttempt)
			    ,CASE WHEN  @GroupByGateWay = 'Y' OR @GateWayName IS NOT NULL THEN DC.GateWay
			          ELSE  null END
END
END
GO
/****** Object:  StoredProcedure [dbo].[EA_PospaidCarrierTotal]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- 
CREATE PROCEDURE [dbo].[EA_PospaidCarrierTotal]
    @ShowCustomerTotal char(1) = 'Y',
	@ShowSupplierTotal char(1) = 'Y',
	@CarrierAccountID varchar(5) = NULL,
	@CarrierProfileID int
AS

DECLARE @PostpaidType int
	SELECT @PostpaidType = e.Value
	  FROM Enumerations e 
			WHERE e.Enumeration = 'TABS.PaymentType' 
			AND [Name] = 'Postpaid'

	DECLARE @PostPaidStats TABLE
	(
		CarrierID varchar(10),
		ProfileID int,
		[Type] varchar(10) NOT NULL,
		CarrierName varchar(255),
		CurrencyID varchar(3),
		LastPaid smalldatetime NULL,
		UnpaidInvoicesNumber int NULL,
		UnpaidInvoicesAmount numeric(13,5) NULL,
		CreditLimit numeric(13,5) NULL,
		InvoiceDelayDays int NULL,
		LastDueDate smalldatetime NULL,
		Sale numeric(13,5) NULL,
		Purchase numeric(13,5) NULL,
		Net numeric(13,5) NULL,
        Tolerance numeric(13,5) NULL
	)
	
	 -- Customer
	INSERT INTO @PostPaidStats (
		CarrierID,
		ProfileID,
		[Type],
		CarrierName,
		CurrencyID,
		LastPaid,
		UnpaidInvoicesNumber, 
		UnpaidInvoicesAmount, 
		CreditLimit, 
		InvoiceDelayDays,
		LastDueDate,
		Sale,
		Purchase
	)
	SELECT 
		Ca.[CarrierAccountID] AS CarrierID,
		Ca.ProfileID,
		'Customer',
		Cp.Name,	
		Cp.CurrencyID,
		
		(SELECT TOP 1 PaidDate FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID AND bi.IsPaid = 'Y' ORDER BY bi.PaidDate DESC) 
			AS [LastPaid],
		
		(SELECT Count(*) FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID AND bi.IsPaid = 'N') 
			AS [UnpaidInvoicesNumber],
		
		(SELECT Sum(bi.Amount) FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID AND bi.IsPaid = 'N') 
			AS [UnpaidInvoicesAmount],
		
		ca.CustomerCreditLimit,
		
		(SELECT TOP 1 datediff(dd, getdate(), bi.DueDate) FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID AND bi.IsPaid = 'N' ORDER BY bi.DueDate) 
			AS [Invoice Delay],
		
		(SELECT TOP 1 bi.DueDate FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID AND bi.IsPaid = 'N' ORDER BY bi.DueDate) 
			AS [LastDueDate],

		(SELECT Sum(bs.Sale_Nets) FROM Billing_Stats bs WHERE bs.CustomerID = ca.CarrierAccountID 
			AND bs.CallDate > ISNULL((SELECT Max(bi.EndDate) FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID),'1990-01-01')
			) AS [Sale], 
		
		(SELECT Sum(bs.Cost_Nets) FROM Billing_Stats bs WHERE bs.SupplierID = ca.CarrierAccountID 
			AND bs.CallDate > ISNULL((SELECT Max(bi.EndDate) FROM Billing_Invoice bi WHERE bi.CustomerID = ca.CarrierAccountID),'1990-01-01')
			) AS [Purchase] 
	
	FROM CarrierAccount ca, CarrierProfile cp  
		WHERE 
				(ca.CustomerPaymentType = @PostpaidType OR cp.CustomerPaymentType = @PostpaidType)				 
			AND ca.ProfileID = cp.ProfileID
			AND (ca.CustomerCreditLimit > 0 OR cp.CustomerCreditLimit > 0)
			AND (ca.CarrierAccountID = @CarrierAccountID OR ca.ProfileID = @CarrierProfileID)
	ORDER BY cp.Name
	
   -- supplier
      INSERT INTO @PostPaidStats (
		CarrierID, 
		ProfileID,
		[Type],
		CarrierName,
		CurrencyID,
		LastPaid,
		UnpaidInvoicesNumber, 
		UnpaidInvoicesAmount, 
		CreditLimit, 
		InvoiceDelayDays,
		LastDueDate,
		Sale,
		Purchase
	)
	SELECT 
		Ca.[CarrierAccountID] AS CarrierID,
		Ca.ProfileID,
		'Supplier',
		Cp.Name,	
		Cp.CurrencyID,
		
		(SELECT TOP 1 PaidDate FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID AND bi.IsPaid = 'Y' ORDER BY bi.PaidDate DESC) 
			AS [LastPaid],
		
		(SELECT Count(*) FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID AND bi.IsPaid = 'N') 
			AS [UnpaidInvoicesNumber],
		
		(SELECT Sum(bi.Amount) FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID AND bi.IsPaid = 'N') 
			AS [UnpaidInvoicesAmount],
		
		ca.SupplierCreditLimit,
		
		(SELECT TOP 1 datediff(dd, getdate(), bi.DueDate) FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID AND bi.IsPaid = 'N' ORDER BY bi.DueDate) 
			AS [Invoice Delay],
		
		(SELECT TOP 1 bi.DueDate FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID AND bi.IsPaid = 'N' ORDER BY bi.DueDate) 
			AS [LastDueDate],

		(SELECT Sum(bs.Sale_Nets) FROM Billing_Stats bs WHERE bs.CustomerID = ca.CarrierAccountID 
			AND bs.CallDate > ISNULL((SELECT Max(bi.EndDate) FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID),'1990-01-01')
			) AS [Sale], 
		
		(SELECT Sum(bs.Cost_Nets) FROM Billing_Stats bs WHERE bs.SupplierID = ca.CarrierAccountID 
			AND bs.CallDate > ISNULL((SELECT Max(bi.EndDate) FROM Billing_Invoice bi WHERE bi.SupplierID = ca.CarrierAccountID),'1990-01-01')
			) AS [Purchase] 
	
	FROM CarrierAccount ca, CarrierProfile cp  
		WHERE 
				(ca.SupplierPaymentType = @PostpaidType OR cp.SupplierPaymentType = @PostpaidType)
			AND ca.ProfileID = cp.ProfileID
			AND (ca.SupplierCreditLimit > 0 OR cp.SupplierCreditLimit > 0 )
			AND (ca.CarrierAccountID = @CarrierAccountID OR ca.ProfileID = @CarrierProfileID)
	ORDER BY cp.Name
	
	UPDATE @PostPaidStats SET Net = ISNULL(Sale,0) - ISNULL(Purchase,0)
	UPDATE @PostPaidStats SET Tolerance = (case WHEN CreditLimit = 0 then 0 else (ISNULL(Net,0) * 100) / CreditLimit end )

   IF @ShowCustomerTotal = 'Y'   
		SELECT * FROM @PostPaidStats WHERE Type='Customer'
   IF @ShowSupplierTotal = 'Y'
		SELECT * FROM @PostPaidStats WHERE Type='Supplier'
GO
/****** Object:  StoredProcedure [dbo].[bp_GetCodeLCR]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_GetCodeLCR]
(
	@ZoneNameFilter VARCHAR(100) = NULL,
	@CodeFilter VARCHAR(50) = NULL,
	@ServicesFlagFilter smallint = 0,
	@SupplierIDs VARCHAR(MAX) = NULL
)
AS
BEGIN

	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	DECLARE @SupplierIDsTable AS TABLE (ID VARCHAR(100))
	INSERT INTO @SupplierIDsTable SELECT * FROM [dbo].ParseArray(@SupplierIDs,',')
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	

	DECLARE @ValidCustomers TABLE (CarrierAccountID VARCHAR(10))
	INSERT INTO @ValidCustomers SELECT CarrierAccountID FROM CarrierAccount ca WHERE ca.RoutingStatus NOT IN (@Account_Blocked, @Account_BlockedInbound) AND ca.ActivationStatus <> @Account_Inactive 	

	IF @CodeFilter IS NULL
		SELECT 
			cm.Code, 
			cm.SupplierZoneID AS OurZoneID,
			oz.[Name] OurZoneName,
			(SELECT MIN(zr.NormalRate) FROM ZoneRate zr WHERE zr.ZoneID = cm.SupplierZoneID AND zr.NormalRate > 0 AND zr.CustomerID IN (SELECT * FROM @ValidCustomers) AND zr.ServicesFlag & @ServicesFlagFilter = @ServicesFlagFilter) AS MinSaleRate,
			cs.SupplierID, 
			cs.SupplierZoneID,
			sz.[Name] AS SupplierZoneName,
			cs.SupplierNormalRate,
			cs.SupplierServicesFlag 
		FROM 
			CodeSupply cs, CodeMatch cm, Zone oz, Zone sz   
		WHERE 
				cm.Code = cs.Code 
			AND cm.SupplierID = 'SYS' 
			AND oz.[Name] LIKE @ZoneNameFilter
			AND oz.ZoneID = cm.SupplierZoneID
			AND sz.ZoneID = cs.SupplierZoneID
			AND cs.SupplierServicesFlag & @ServicesFlagFilter = @ServicesFlagFilter
			AND (@SupplierIDs Is Null Or (sz.SupplierID IN (SELECT ID FROM @SupplierIDsTable)))
		ORDER BY cs.Code, cs.SupplierNormalRate
	ELSE
		SELECT 
			cm.Code, 
			cm.SupplierZoneID AS OurZoneID,
			oz.[Name] OurZoneName,
			(SELECT MIN(zr.NormalRate) FROM ZoneRate zr WHERE zr.ZoneID = cm.SupplierZoneID AND zr.NormalRate > 0 AND zr.CustomerID IN (SELECT * FROM @ValidCustomers) AND zr.ServicesFlag & @ServicesFlagFilter = @ServicesFlagFilter) AS MinSaleRate,
			cs.SupplierID, 
			cs.SupplierZoneID,
			sz.[Name] AS SupplierZoneName,
			cs.SupplierNormalRate,
			cs.SupplierServicesFlag 
		FROM 
			CodeSupply cs, CodeMatch cm, Zone oz, Zone sz   
		WHERE 
				cm.Code = cs.Code 
			AND oz.SupplierID = 'SYS' 
			AND cm.Code LIKE @CodeFilter
			AND oz.ZoneID = cm.SupplierZoneID
			AND sz.ZoneID = cs.SupplierZoneID
			AND cs.SupplierServicesFlag & @ServicesFlagFilter = @ServicesFlagFilter
			AND (@SupplierIDs Is Null Or (sz.SupplierID IN (SELECT ID FROM @SupplierIDsTable)))
		ORDER BY cs.Code, cs.SupplierNormalRate
		
END
GO
/****** Object:  StoredProcedure [dbo].[bp_PrepaidDailyTotalUpdate]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ======================================================================
-- Description: Updates the daily Prepaid Amount Billing information 
-- ======================================================================
CREATE PROCEDURE [dbo].[bp_PrepaidDailyTotalUpdate]
(
	@FromCallDate datetime = NULL,
    @ToCallDate datetime  = NULL
)
AS
BEGIN
	SET NOCOUNT ON;
	
	Declare @PrepaidAmount TABLE
	(
		[SupplierID] [varchar](10) NULL,
		[CustomerID] [varchar](10) NULL,
		[Amount] [numeric](13, 5) NULL,
		[CurrencyID] [varchar](3) NULL,
		[Date] [datetime] NULL,
		[Type] [smallint] NULL,
		[UserID] [int] NULL,
		[Tag] [varchar](255) NULL,
		[LastUpdate] [datetime] NULL,
		[ReferenceNumber] [varchar](250) NULL,
		[Note] [varchar](250) NULL,
		[CustomerProfileID] [smallint] NULL,
		[SupplierProfileID] [smallint] NULL
	)

	INSERT INTO @PrepaidAmount (UserID) VALUES (-1)

	SET @FromCallDate = ISNULL(@FromCallDate, getdate())
	SET @ToCallDate = ISNULL(@ToCallDate, getdate())
	
	SET @FromCallDate = dbo.DateOf(@FromCallDate)
	SET @ToCallDate = dbo.DateOf(@ToCallDate)

	DECLARE @PrepaidType int
	SELECT @PrepaidType = e.Value
	  FROM Enumerations e 
			WHERE e.Enumeration = 'TABS.PaymentType' 
			AND [Name] = 'Prepaid'

	DECLARE @BillingPrepaidAmountType int
	SELECT @BillingPrepaidAmountType = e.Value
	  FROM Enumerations e 
			WHERE e.Enumeration = 'TABS.AmountType' 
			AND [Name] = 'Billing'

	DECLARE @LastUpdate datetime
	SET @LastUpdate = getdate()
	
	CREATE TABLE #ExchangeRates (
			CurrencyIn VARCHAR(3) COLLATE SQL_Latin1_General_CP1256_CI_AS,
			CurrencyProfile VARCHAR(3) COLLATE SQL_Latin1_General_CP1256_CI_AS,
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(CurrencyIn,CurrencyProfile, Date)
				)
	
	INSERT INTO #ExchangeRates 
	SELECT gder1.Currency AS CurrencyIn,
		   gder2.Currency AS CurrencyProfile,
		   gder1.Date AS Date, 
		   gder1.Rate / gder2.Rate AS Rate
		FROM   dbo.GetDailyExchangeRates(@FromCallDate, @ToCallDate) gder1
		JOIN dbo.GetDailyExchangeRates(@FromCallDate, @ToCallDate) gder2 ON  gder1.Date = gder2.Date

	-- Clean up Billing Amounts
	DELETE FROM PrepaidAmount WHERE 
		Date BETWEEN @FromCallDate and @ToCallDate AND [TYPE] = @BillingPrepaidAmountType
	--

	-- Re-Insert Customer Amounts
	INSERT INTO @PrepaidAmount
	   (
			  CustomerID,
			  Date,
			  Amount,
			  CurrencyID,
			  [TYPE],
			  LastUpdate
		)
	SELECT     
			BS.CustomerID,
			BS.CallDate,
			-1 * SUM(BS.Sale_Nets)/er.Rate AS Sale_Nets,
			cp.CurrencyID,
			@BillingPrepaidAmountType,
			@LastUpdate
	FROM       
			Billing_Stats as BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
			JOIN CarrierAccount ca WITH (NOLOCK) ON BS.CustomerID = ca.CarrierAccountID AND ca.CustomerPaymentType = @PrepaidType
			JOIN CarrierProfile cp WITH (NOLOCK) ON cp.ProfileID = ca.ProfileID
			JOIN #ExchangeRates er WITH (NOLOCK) ON er.CurrencyIn = BS.Sale_Currency AND er.CurrencyProfile = cp.CurrencyID AND er.Date = BS.CallDate
	WHERE 1=1
		AND BS.CallDate >= @FromCallDate 
		AND BS.CallDate <= @ToCallDate
		AND BS.CallDate >= ca.CustomerActivateDate AND (ca.CustomerDeactivateDate IS NULL OR ca.CustomerDeactivateDate > BS.CallDate)
		--AND BS.CustomerID IN (SELECT CarrierAccountID FROM CarrierAccount WHERE CustomerPaymentType = @PrepaidType)
		AND BS.Sale_Currency IS NOT NULL
	GROUP BY   
		BS.CustomerID, 
		BS.CallDate,  
		cp.CurrencyID,
		er.Rate

	-- Re-Insert Customer Amounts for Prepaid Profiles
	INSERT INTO @PrepaidAmount
	   (
			  CustomerProfileID,
			  Date,
			  Amount,
			  CurrencyID,
			  [TYPE],
			  LastUpdate
		)
	SELECT     
			ca.ProfileID, 
			BS.CallDate,
			-1 * SUM(BS.Sale_Nets)/er.Rate AS Sale_Nets,
			cp.CurrencyID,
			@BillingPrepaidAmountType,
			@LastUpdate
	FROM       
			Billing_Stats as BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
			JOIN CarrierAccount ca WITH (NOLOCK) ON BS.CustomerID = ca.CarrierAccountID 
			JOIN CarrierProfile cp WITH (NOLOCK) ON ca.ProfileID = cp.ProfileID AND cp.CustomerPaymentType = @PrepaidType
			JOIN #ExchangeRates er WITH (NOLOCK) ON er.CurrencyIn = BS.Sale_Currency AND er.CurrencyProfile = cp.CurrencyID AND er.Date = BS.CallDate
	WHERE 1=1
		AND BS.CallDate >= @FromCallDate 
		AND BS.CallDate <= @ToCallDate
		AND BS.CallDate >= cp.CustomerActivateDate AND (cp.CustomerDeactivateDate IS NULL OR cp.CustomerDeactivateDate > BS.CallDate)
		--AND BS.CustomerID IN (SELECT CarrierAccountID FROM CarrierAccount cat, CarrierProfile cpt WHERE cat.ProfileID = cpt.ProfileID AND cpt.CustomerPaymentType = @PrepaidType)
		AND BS.Sale_Currency IS NOT NULL
	GROUP BY   
		ca.ProfileID, 
		BS.CallDate,  
		cp.CurrencyID,
		er.Rate

	-- Re-Insert Supplier Amounts
	INSERT INTO @PrepaidAmount
	   (
			  SupplierID,
			  Date,
			  Amount,
			  CurrencyID,
			  [TYPE],
			  LastUpdate
		)
	SELECT     
			BS.SupplierID,
			BS.CallDate,
			SUM(BS.Cost_Nets)/er.Rate AS Cost_Nets,
			cp.CurrencyID,
			@BillingPrepaidAmountType,
			@LastUpdate
	FROM       
			Billing_Stats as BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
			JOIN CarrierAccount ca WITH (NOLOCK) ON BS.SupplierID = ca.CarrierAccountID AND ca.SupplierPaymentType = @PrepaidType
			JOIN CarrierProfile cp WITH (NOLOCK) ON cp.ProfileID = ca.ProfileID
			JOIN #ExchangeRates er WITH (NOLOCK) ON er.CurrencyIn = BS.Cost_Currency AND er.CurrencyProfile = cp.CurrencyID AND er.Date = BS.CallDate
	WHERE 1=1
		AND BS.CallDate >= @FromCallDate 
		AND BS.CallDate <= @ToCallDate
		AND BS.CallDate >= ca.SupplierActivateDate AND (ca.SupplierDeactivateDate IS NULL OR ca.SupplierDeactivateDate > BS.CallDate)
		--AND BS.SupplierID IN (SELECT CarrierAccountID FROM CarrierAccount WHERE SupplierPaymentType = @PrepaidType)
		AND BS.Cost_Currency IS NOT NULL
	GROUP BY   
		BS.SupplierID, 
		BS.CallDate,  
		cp.CurrencyID,
		er.Rate

	-- Re-Insert Supplier Amounts Profile
	INSERT INTO @PrepaidAmount
	   (
			  SupplierProfileID,
			  Date,
			  Amount,
			  CurrencyID,
			  [TYPE],
			  LastUpdate
		)
	SELECT     
			ca.ProfileID,
			BS.CallDate,
			SUM(BS.Cost_Nets)/er.Rate AS Cost_Nets,
			cp.CurrencyID,
			@BillingPrepaidAmountType,
			@LastUpdate
	FROM       
			Billing_Stats as BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
			JOIN CarrierAccount ca WITH (NOLOCK) ON BS.SupplierID = ca.CarrierAccountID
			JOIN CarrierProfile cp WITH (NOLOCK) ON ca.ProfileID = cp.ProfileID AND cp.SupplierPaymentType = @PrepaidType
			JOIN #ExchangeRates er WITH (NOLOCK) ON er.CurrencyIn = BS.Cost_Currency AND er.CurrencyProfile = cp.CurrencyID AND er.Date = BS.CallDate
	WHERE 1=1
		AND BS.CallDate >= @FromCallDate 
		AND BS.CallDate <= @ToCallDate
		AND BS.CallDate >= cp.SupplierActivateDate AND (cp.SupplierDeactivateDate IS NULL OR cp.SupplierDeactivateDate > BS.CallDate)
		--AND BS.SupplierID IN (SELECT CarrierAccountID FROM CarrierAccount cat,CarrierProfile cpt WHERE cat.ProfileID = cpt.ProfileID AND cpt.SupplierPaymentType = @PrepaidType)
		AND BS.Cost_Currency IS NOT NULL
	GROUP BY   
		ca.ProfileID, 
		BS.CallDate,  
		cp.CurrencyID,
		er.Rate

	INSERT INTO PrepaidAmount 
		(
		[SupplierID],
		[CustomerID],
		[Amount],
		[CurrencyID],
		[Date],
		[Type],
		[UserID],
		[Tag],
		[LastUpdate],
		[ReferenceNumber],
		[Note],
		[CustomerProfileID],
		[SupplierProfileID] 
		)
	SELECT 		
			[SupplierID],
			[CustomerID],
			[Amount],
			[CurrencyID],
			[Date],
			[Type],
			[UserID],
			[Tag],
			[LastUpdate],
			[ReferenceNumber],
			[Note],
			[CustomerProfileID],
			[SupplierProfileID] 
	FROM @PrepaidAmount WHERE UserID IS NULL

END
GO
/****** Object:  StoredProcedure [dbo].[bp_PostpaidDailyTotalUpdate]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ======================================================================
-- Description: Updates the daily Postpaid Amount Billing information 
-- ======================================================================
Create PROCEDURE [dbo].[bp_PostpaidDailyTotalUpdate]
(
	@FromCallDate datetime = NULL,
    @ToCallDate datetime  = NULL
)
WITH RECOMPILE
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @PostpaidAmount TABLE
	(
		[SupplierID] [varchar](10) NULL,
		[CustomerID] [varchar](10) NULL,
		[Amount] [numeric](13, 5) NULL,
		[CurrencyID] [varchar](3) NULL,
		[Date] [datetime] NULL,
		[Type] [smallint] NULL,
		[UserID] [int] NULL,
		[Tag] [varchar](255) NULL,
		[LastUpdate] [datetime] NULL,
		[ReferenceNumber] [varchar](250) NULL,
		[Note] [varchar](250) NULL,
		[CustomerProfileID] [smallint] NULL,
		[SupplierProfileID] [smallint] NULL	
	)
	
	INSERT INTO @PostpaidAmount (UserID) VALUES (-1)
	
	SET @FromCallDate = ISNULL(@FromCallDate, getdate())
	SET @ToCallDate = ISNULL(@ToCallDate, getdate())
	
	SET @FromCallDate = dbo.DateOf(@FromCallDate)
	SET @ToCallDate = dbo.DateOf(@ToCallDate)

	DECLARE @PostpaidType int
	SELECT @PostpaidType = e.Value
	  FROM Enumerations e 
			WHERE e.Enumeration = 'TABS.PaymentType' 
			AND [Name] = 'Postpaid'

	DECLARE @BillingPostpaidAmountType int
	SELECT @BillingPostpaidAmountType = e.Value
	  FROM Enumerations e 
			WHERE e.Enumeration = 'TABS.AmountType' 
			AND [Name] = 'Billing'

	DECLARE @LastUpdate datetime
	SET @LastUpdate = getdate()
	
	CREATE TABLE #ExchangeRates (
			CurrencyIn VARCHAR(3) COLLATE SQL_Latin1_General_CP1256_CI_AS,
			CurrencyProfile VARCHAR(3) COLLATE SQL_Latin1_General_CP1256_CI_AS,
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(CurrencyIn,CurrencyProfile, Date)
	)
	
	INSERT INTO #ExchangeRates 
	SELECT gder1.Currency AS CurrencyIn,
		   gder2.Currency AS CurrencyProfile,
		   gder1.Date AS Date, 
		   gder1.Rate / gder2.Rate AS Rate
		FROM   dbo.GetDailyExchangeRates(@FromCallDate, @ToCallDate) gder1
		JOIN dbo.GetDailyExchangeRates(@FromCallDate, @ToCallDate) gder2 ON  gder1.Date = gder2.Date

	-- Clean up Billing Amounts
	DELETE FROM PostpaidAmount WHERE 
		Date BETWEEN @FromCallDate and @ToCallDate AND [TYPE] = @BillingPostpaidAmountType
	--

	-- Re-Insert Customer Amounts
	INSERT INTO @PostpaidAmount
	   (
			  CustomerID,
			  Date,
			  Amount,
			  CurrencyID,
			  [TYPE],
			  LastUpdate
		)
	SELECT     
			BS.CustomerID,
			BS.CallDate,
			-1 * SUM(BS.Sale_Nets)/er.Rate AS Sale_Nets,
			cp.CurrencyID,
			@BillingPostpaidAmountType,
			@LastUpdate
	FROM       
			Billing_Stats as BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
			JOIN CarrierAccount ca WITH (NOLOCK) ON BS.CustomerID = ca.CarrierAccountID AND ca.CustomerPaymentType = @PostpaidType
			JOIN CarrierProfile cp WITH (NOLOCK) ON cp.ProfileID = ca.ProfileID
			JOIN #ExchangeRates er WITH (NOLOCK) ON er.CurrencyIn = BS.Sale_Currency AND er.CurrencyProfile = cp.CurrencyID AND er.Date = BS.CallDate
	WHERE 1=1
		AND BS.CallDate >= @FromCallDate
		AND BS.CallDate <= @ToCallDate
		AND BS.CallDate >= ca.CustomerActivateDate AND (ca.CustomerDeactivateDate IS NULL OR ca.CustomerDeactivateDate > BS.CallDate)
		--AND BS.CustomerID IN (SELECT CarrierAccountID FROM CarrierAccount WHERE CustomerPaymentType = @PostpaidType)
		AND BS.Sale_Currency IS NOT NULL
	GROUP BY   
		BS.CustomerID, 
		BS.CallDate,  
		cp.CurrencyID,
		er.Rate

	-- Re-Insert Customer Amounts for Postpaid Profiles
	INSERT INTO @PostpaidAmount
	   (
			  CustomerProfileID,
			  Date,
			  Amount,
			  CurrencyID,
			  [TYPE],
			  LastUpdate
		)
	SELECT     
			ca.ProfileID, 
			BS.CallDate,
			-1 * SUM(BS.Sale_Nets)/er.Rate AS Sale_Nets,
			cp.CurrencyID,
			@BillingPostpaidAmountType,
			@LastUpdate
	FROM       
			Billing_Stats as BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
			JOIN CarrierAccount ca WITH (NOLOCK) ON BS.CustomerID = ca.CarrierAccountID 
			JOIN CarrierProfile cp WITH (NOLOCK) ON cp.ProfileID = ca.ProfileID AND cp.CustomerPaymentType = @PostpaidType
			JOIN #ExchangeRates er WITH (NOLOCK) ON er.CurrencyIn = BS.Sale_Currency AND er.CurrencyProfile = cp.CurrencyID AND er.Date = BS.CallDate
	WHERE 1=1
		AND BS.CallDate >= @FromCallDate 
		AND BS.CallDate <= @ToCallDate
		AND BS.CallDate >= cp.CustomerActivateDate AND (cp.CustomerDeactivateDate IS NULL OR cp.CustomerDeactivateDate > BS.CallDate)
		--AND BS.CustomerID IN (SELECT CarrierAccountID FROM CarrierAccount cat, CarrierProfile cpt WHERE cat.ProfileID = cpt.ProfileID AND cpt.CustomerPaymentType = @PostpaidType)
		AND BS.Sale_Currency IS NOT NULL
	GROUP BY   
		ca.ProfileID, 
		BS.CallDate,  
		cp.CurrencyID,
		er.Rate

	-- Re-Insert Supplier Amounts
	INSERT INTO @PostpaidAmount
	   (
			  SupplierID,
			  Date,
			  Amount,
			  CurrencyID,
			  [TYPE],
			  LastUpdate
		)
	SELECT     
			BS.SupplierID,
			BS.CallDate,
			SUM(BS.Cost_Nets)/er.Rate AS Cost_Nets,
			cp.CurrencyID,
			@BillingPostpaidAmountType,
			@LastUpdate
	FROM       
			Billing_Stats as BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
			JOIN CarrierAccount ca WITH (NOLOCK) ON BS.SupplierID = ca.CarrierAccountID AND ca.SupplierPaymentType = @PostpaidType
			JOIN CarrierProfile cp WITH (NOLOCK) ON cp.ProfileID = ca.ProfileID
			JOIN #ExchangeRates er WITH (NOLOCK) ON er.CurrencyIn = BS.Cost_Currency AND er.CurrencyProfile = cp.CurrencyID AND er.Date = BS.CallDate
	WHERE 1=1
		AND BS.CallDate >= @FromCallDate 
		AND BS.CallDate <= @ToCallDate
		AND BS.CallDate >= ca.SupplierActivateDate AND (ca.SupplierDeactivateDate IS NULL OR ca.SupplierDeactivateDate > BS.CallDate)
		--AND BS.SupplierID IN (SELECT CarrierAccountID FROM CarrierAccount WHERE SupplierPaymentType = @PostpaidType)
		AND BS.Cost_Currency IS NOT NULL
	GROUP BY   
		BS.SupplierID, 
		BS.CallDate,  
		cp.CurrencyID,
		er.Rate

-- Re-Insert Supplier Amounts Profile
	INSERT INTO @PostpaidAmount
	   (
			  SupplierProfileID,
			  Date,
			  Amount,
			  CurrencyID,
			  [TYPE],
			  LastUpdate
		)
	SELECT     
			ca.ProfileID,
			BS.CallDate,
			SUM(BS.Cost_Nets)/er.Rate AS Cost_Nets,
			cp.CurrencyID,
			@BillingPostpaidAmountType,
			@LastUpdate
	FROM       
			Billing_Stats as BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
			JOIN CarrierAccount ca WITH (NOLOCK) ON BS.SupplierID = ca.CarrierAccountID
			JOIN CarrierProfile cp WITH (NOLOCK) ON ca.ProfileID = cp.ProfileID AND cp.SupplierPaymentType = @PostpaidType
			JOIN #ExchangeRates er WITH (NOLOCK) ON er.CurrencyIn = BS.Cost_Currency AND er.CurrencyProfile = cp.CurrencyID AND er.Date = BS.CallDate
	WHERE 1=1
		AND BS.CallDate >= @FromCallDate 
		AND BS.CallDate <= @ToCallDate
		AND BS.CallDate >= cp.SupplierActivateDate AND (cp.SupplierDeactivateDate IS NULL OR cp.SupplierDeactivateDate > BS.CallDate)
		--AND BS.SupplierID IN (SELECT CarrierAccountID FROM CarrierAccount cat,CarrierProfile cpt WHERE cat.ProfileID = cpt.ProfileID AND cpt.SupplierPaymentType = @PostpaidType)
		AND BS.Cost_Currency IS NOT NULL
	GROUP BY   
		ca.ProfileID, 
		BS.CallDate,  
		cp.CurrencyID,
		er.Rate
		
	INSERT INTO PostpaidAmount 
		(
		[SupplierID],
		[CustomerID],
		[Amount],
		[CurrencyID],
		[Date],
		[Type],
		[UserID],
		[Tag],
		[LastUpdate],
		[ReferenceNumber],
		[Note],
		[CustomerProfileID],
		[SupplierProfileID] 
		)
	SELECT 		
			[SupplierID],
			[CustomerID],
			[Amount],
			[CurrencyID],
			[Date],
			[Type],
			[UserID],
			[Tag],
			[LastUpdate],
			[ReferenceNumber],
			[Note],
			[CustomerProfileID],
			[SupplierProfileID] 
	FROM @PostpaidAmount WHERE UserID IS NULL

END
GO
/****** Object:  StoredProcedure [dbo].[bp_PrepaidCarrierTotal]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ======================================================================
-- Author: Mohammad El-Shab
-- Description: 
-- ======================================================================
CREATE PROCEDURE [dbo].[bp_PrepaidCarrierTotal]
    @ShowCustomerTotal char(1) = 'Y',
	@ShowSupplierTotal char(1) = 'Y'
AS
BEGIN
	SET NOCOUNT ON;
	IF @ShowCustomerTotal = 'Y'
		SELECT     
			PA.CustomerID AS CarrierID,
			PA.CustomerProfileID AS ProfileID,
			SUM(PA.Amount) AS Balance,
			PA.CurrencyID AS Currency,
			--(case WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END) AS CreditLimit,
			--ABS(ISNULL((CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END),0)) - ABS(SUM(PA.Amount)) AS Tolerance Old Fadi
			SUM(PA.Amount) AS Tolerance,
			SUM(CASE WHEN PA.[Type] = 1 THEN PA.Amount ELSE 0 END) AS Billing,
			SUM(CASE WHEN PA.[Type] != 1 THEN PA.Amount ELSE 0 END) AS Payment
		FROM
			PrepaidAmount PA 
				LEFT JOIN CarrierAccount CA ON CA.CarrierAccountID = PA.CustomerID
				LEFT JOIN CarrierProfile CP ON CP.ProfileID = PA.CustomerProfileID  
		WHERE
			(PA.CustomerID IS NOT NULL OR PA.CustomerProfileID IS NOT NULL)
			AND PA.Date >= (CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerActivateDate ELSE CP.CustomerActivateDate END)
			AND ((CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerDeactivateDate ELSE CP.CustomerDeactivateDate END) IS NULL 
				OR (CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerDeactivateDate ELSE CP.CustomerDeactivateDate END) > PA.Date)
		GROUP BY	
			PA.CustomerProfileID,
			PA.CustomerID,
			PA.CurrencyID, 
			(CASE WHEN PA.CustomerID IS NOT NULL THEN CA.CustomerCreditLimit ELSE CP.CustomerCreditLimit END)
		ORDER BY Tolerance ASC
	IF @ShowSupplierTotal = 'Y'
		SELECT
		     PA.SupplierID AS CarrierID,
			 PA.SupplierProfileID AS ProfileID,
			 SUM(PA.Amount) AS Balance,
			 PA.CurrencyID AS Currency,
			 --(CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierCreditLimit ELSE CP.SupplierCreditLimit END) AS CreditLimit,
			 --ABS(ISNULL((CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierCreditLimit ELSE CP.SupplierCreditLimit END),0)) - ABS(SUM(PA.Amount)) AS Tolerance Old Fadi
			 -SUM(PA.Amount) AS Tolerance,
			 SUM(CASE WHEN PA.[Type] = 1 THEN PA.Amount ELSE 0 END) AS Billing,
			 SUM(CASE WHEN PA.[Type] != 1 THEN PA.Amount ELSE 0 END) AS Payment
		FROM 
			PrepaidAmount PA 
		    LEFT JOIN CarrierAccount CA ON CA.CarrierAccountID = PA.SupplierID
			LEFT JOIN CarrierProfile CP ON CP.ProfileID = PA.SupplierProfileID  
		WHERE
		    (PA.SupplierID IS NOT NULL OR PA.SupplierProfileID IS NOT NULL) 
		    AND PA.Date >= (CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierActivateDate ELSE CP.SupplierActivateDate END)
			AND ((CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierDeactivateDate ELSE CP.SupplierDeactivateDate END) IS NULL 
				OR (CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierDeactivateDate ELSE CP.SupplierDeactivateDate END) > PA.Date)
		GROUP BY   
			PA.SupplierProfileID,
			PA.SupplierID,
			PA.CurrencyID, 
			(CASE WHEN PA.SupplierID IS NOT NULL THEN CA.SupplierCreditLimit ELSE CP.SupplierCreditLimit END)		
		ORDER BY Tolerance ASC
END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_ZoneSummary]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_TrafficStats_ZoneSummary]
	@fromDate DATETIME ,
	@ToDate DATETIME,
	@ZoneID VARCHAR(15) = NULL,
	@TopRecord INT = NULL
AS
BEGIN
	DECLARE @Traffic TABLE (
	            ZoneID INT PRIMARY KEY,
	            Attempts INT,
	            DurationsInMinutes NUMERIC(13, 5),
	            ASR NUMERIC(13, 5),
	            ACD NUMERIC(13, 5),
	            DeliveredASR NUMERIC(13, 5),
	            AveragePDD NUMERIC(13, 5)
	        )
	
	SET NOCOUNT ON 	
	
	INSERT INTO @Traffic
	  (
	    ZoneID,
	    Attempts,
	    DurationsInMinutes,
	    ASR,
	    ACD,
	    DeliveredASR,
	    AveragePDD
	  )
	SELECT isnull(TS.OurZoneID,0),
	       SUM(Attempts) AS Attempts,
	       SUM(DurationsInSeconds / 60.) AS DurationsInMinutes,
	       SUM(SuccessfulAttempts) * 100.0 / SUM(Attempts) AS ASR,
	       CASE 
	            WHEN SUM(SuccessfulAttempts) > 0 THEN SUM(DurationsInSeconds) / (60.0 * SUM(SuccessfulAttempts))
	            ELSE 0
	       END AS ACD,
	       SUM(deliveredAttempts) * 100.0 / SUM(Attempts) AS DeliveredASR,
	       AVG(PDDinSeconds) AS AveragePDD
	FROM   TrafficStats TS WITH(
	           NOLOCK,
	           INDEX(IX_TrafficStats_DateTimeFirst),
	           INDEX(IX_TrafficStats_Zone)
	       )
	WHERE  FirstCDRAttempt >= @FromDate
	       AND FirstCDRAttempt <= @ToDate
	       AND (@ZoneID IS NULL OR TS.OurZoneID = @ZoneID)
	GROUP BY
	       isnull(TS.OurZoneID,0)
	
	DECLARE @Result TABLE (
	            ZoneID INT PRIMARY KEY,
	            Attempts INT,
	            DurationsInMinutes NUMERIC(13, 5),
	            ASR NUMERIC(13, 5),
	            ACD NUMERIC(13, 5),
	            DeliveredASR NUMERIC(13, 5),
	            AveragePDD NUMERIC(13, 5),
	            NumberOfCalls INT,
	            Cost_Nets FLOAT,
	            Sale_Nets FLOAT,
	            Profit NUMERIC(13, 5),
	            Percentage FLOAT
	        )
	
	INSERT INTO @Result
	  (
	    ZoneID,
	    Attempts,
	    DurationsInMinutes,
	    ASR,
	    ACD,
	    DeliveredASR,
	    AveragePDD,
	    NumberOfCalls,
	    Sale_Nets,
	    Cost_Nets,
	    Profit,
	    Percentage
	  )
	SELECT T.ZoneID,
	       T.Attempts,
	       T.DurationsInMinutes,
	       T.ASR,
	       T.ACD,
	       T.DeliveredASR,
	       T.AveragePDD,
	       ISNULL(SUM(BS.NumberOfCalls), 0) AS Calls,
	       ISNULL(SUM(BS.Sale_Nets / CS.lastrate), 0) AS Sale,
	       ISNULL(SUM(BS.Cost_Nets / CC.lastrate), 0) AS Cost,
	       ISNULL(SUM(BS.Sale_Nets / CS.lastrate), 0) -ISNULL(SUM(BS.Cost_Nets / CC.lastrate), 0) AS 
	       Profit,
	       0
	FROM   @Traffic T
	       LEFT JOIN Billing_Stats BS WITH (NOLOCK)
	            ON  T.ZoneID = BS.SaleZoneID
	            AND (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate)
	       LEFT JOIN Currency CS WITH (NOLOCK)
	            ON  CS.CurrencyID = bs.sale_currency
	       LEFT JOIN Currency CC WITH (NOLOCK)
	            ON  CC.CurrencyID = bs.cost_currency
	GROUP BY
	       T.ZoneID,
	       T.Attempts,
	       T.DurationsInMinutes,
	       T.ASR,
	       T.ACD,
	       T.DeliveredASR,
	       T.AveragePDD 
	
	DECLARE @TotalProfit NUMERIC(13, 5)
	SELECT @TotalProfit = SUM(profit)
	FROM   @Result
	
	UPDATE @Result
	SET    Percentage = CASE 
	                         WHEN @TotalProfit <> 0 THEN (Profit * 100.0 / @TotalProfit)
	                         ELSE 0
	                    END
	
	SET ROWCOUNT @TopRecord
	
	SELECT *
	FROM   @Result
	ORDER BY
	       DurationsInMinutes DESC
END
GO
/****** Object:  StoredProcedure [dbo].[rpt_Volumes_DestinationTraffic]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[rpt_Volumes_DestinationTraffic](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@OurZoneID INT,
	@TopValues INT,
	@Attempts INT,
	@Period varchar(7)
)
with Recompile
AS 
	SET @FromDate= CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	
 IF @OurZoneID=0
 SET @OurZoneID = null 
 SET ROWCOUNT @TopValues
 
 IF(@Period = 'None')
 BEGIN
   SELECT 
          bs.SaleZoneID AS SaleZoneID,
          SUM(BS.NumberOfCalls) AS Attempts,
          SUM(BS.SaleDuration)/60.0 AS Duration    
   FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
   WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
     	AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
	    AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
	    AND (@OurZoneID IS NULL  OR BS.SaleZoneID=@OurZoneID)		
        AND  BS.NumberOfCalls > @Attempts
   GROUP BY bs.SaleZoneID
   ORDER BY Duration DESC	
 END
 ELSE
 	IF(@Period = 'Daily')
	BEGIN
	   SELECT 
			  bs.CallDate AS CallDate,
			  bs.SaleZoneID AS SaleZoneID,
			  SUM(BS.NumberOfCalls) AS Attempts,
			  SUM(BS.SaleDuration)/60.0 AS Duration    
	   FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
	   WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
 			AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
			AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
			AND (@OurZoneID IS NULL  OR BS.SaleZoneID=@OurZoneID)		
			AND  BS.NumberOfCalls > @Attempts
	   GROUP BY bs.CallDate, bs.SaleZoneID
	   ORDER BY Duration DESC	
	END
	ELSE
	   IF(@Period = 'Weekly')
       BEGIN
		   SELECT 
				  datepart(week,BS.CallDate) AS CallWeek,
				  datepart(year,bs.CallDate) AS CallYear,
				  bs.SaleZoneID AS SaleZoneID,
				  SUM(BS.NumberOfCalls) AS Attempts,
				  SUM(BS.SaleDuration)/60.0 AS Duration    
		   FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
		   WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
				AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
				AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
				AND (@OurZoneID IS NULL  OR BS.SaleZoneID=@OurZoneID)		
				AND  BS.NumberOfCalls > @Attempts
		   GROUP BY DATEPART(week,BS.calldate),DATEPART(year,BS.calldate), bs.SaleZoneID
		   ORDER BY Duration DESC	
	   END
		ELSE
		  IF(@Period = 'Monthly')
	      BEGIN
		   SELECT 
			      datepart(month,BS.CallDate) AS CallMonth,
			      datepart(year,bs.CallDate) AS CallYear,
				  bs.SaleZoneID AS SaleZoneID,
				  SUM(BS.NumberOfCalls) AS Attempts,
				  SUM(BS.SaleDuration)/60.0 AS Duration    
		   FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
		   WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
				AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
				AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
				AND (@OurZoneID IS NULL  OR BS.SaleZoneID=@OurZoneID)		
				AND  BS.NumberOfCalls > @Attempts
		   GROUP BY DATEPART(month,BS.calldate),DATEPART(year,BS.calldate), bs.SaleZoneID
		   ORDER BY Duration DESC	
	      END
 SET ROWCOUNT 0
RETURN
GO
/****** Object:  StoredProcedure [dbo].[rpt_Volumes_DailyTraffic]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rpt_Volumes_DailyTraffic](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@OurZoneID INT,
	@Attempts INT,
	@Period varchar(7)
)
with Recompile
AS 
	SET @FromDate=     CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	
	
   IF(@Period = 'None')
   BEGIN
   	   SELECT 
			  SUM(BS.NumberOfCalls) AS Attempts,
			  SUM(BS.SaleDuration)/60.0 AS Duration    
	   FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
	   WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
			AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
			AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
			AND (@OurZoneID = 0 OR BS.SaleZoneID=@OurZoneID)		
			AND  BS.NumberOfCalls > @Attempts
   END
	ELSE	
		IF(@Period = 'Daily')
		BEGIN
		   SELECT 
				  cast(BS.CallDate AS varchar(12)) AS CallDate,
				  SUM(BS.NumberOfCalls) AS Attempts,
				  SUM(BS.SaleDuration)/60.0 AS Duration    
		   FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
		   WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
 				AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
				AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
				AND (@OurZoneID = 0 OR BS.SaleZoneID=@OurZoneID)		
				AND  BS.NumberOfCalls > @Attempts
		   GROUP BY BS.calldate 
		   ORDER BY BS.Calldate ASC 	 
		END
		ELSE
			IF(@Period = 'Weekly')
			BEGIN
				SELECT 
					datepart(week,BS.CallDate) AS CallWeek,
					datepart(year,bs.CallDate) AS CallYear,
					SUM(BS.NumberOfCalls) AS Attempts,
					SUM(BS.SaleDuration)/60.0 AS Duration    
				FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
				WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
					AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
					AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
					AND (@OurZoneID = 0 OR BS.SaleZoneID=@OurZoneID)		
					AND  BS.NumberOfCalls > @Attempts
				GROUP BY DATEPART(week,BS.calldate),DATEPART(year,BS.calldate)
				ORDER BY DATEPART(year,BS.calldate),DATEPART(week,BS.calldate) ASC
			END
			ELSE
				IF(@Period = 'Monthly')
				BEGIN
					SELECT 
						datepart(month,BS.CallDate) AS CallMonth,
						datepart(year,bs.CallDate) AS CallYear,
						SUM(BS.NumberOfCalls) AS Attempts,
						SUM(BS.SaleDuration)/60.0 AS Duration    
					FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
					WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
						AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
						AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
						AND (@OurZoneID = 0 OR BS.SaleZoneID=@OurZoneID)		
						AND  BS.NumberOfCalls > @Attempts
					GROUP BY DATEPART(month,BS.calldate),DATEPART(year,BS.calldate)
					ORDER BY DATEPART(year,BS.calldate),DATEPART(month,BS.calldate) ASC 
				END
RETURN
GO
/****** Object:  StoredProcedure [dbo].[bp_GetStatisticalDailySummary]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_GetStatisticalDailySummary](@date DATETIME = NULL)
WITH RECOMPILE
AS
BEGIN
	DECLARE @dateStart AS DATETIME
	DECLARE @dateFinish AS DATETIME
	SET @date = ISNULL(@date, DATEADD(DAY, -1, dbo.DateOf(GETDATE())))
	SET @dateStart = @date
	SET @dateFinish = DATEADD(ms, -3, DATEADD(DAY, +1, @dateStart))

	SELECT 
		'CDR' AS [Source],
		COUNT(*) as Attempts,
		SUM(CASE when c.DurationInSeconds > 0 THEN 1 ELSE 0 END) AS Calls,    
		SUM(c.DurationInSeconds) / 60.0 AS Minutes
	FROM CDR c WITH(NOLOCK, INDEX(IX_CDR_AttemptDateTime)) WHERE c.AttemptDateTime BETWEEN @dateStart AND @dateFinish
	UNION
	SELECT 
		'Traffic_Stats',
		SUM(ts.Attempts) as Attempts, 
		SUM(ts.SuccessfulAttempts) AS Calls,
		SUM(ts.DurationsInSeconds) / 60.0 AS Minutes 
	FROM TrafficStats ts WITH(NOLOCK, INDEX(IX_TrafficStats_DateTimeFirst)) WHERE ts.FirstCDRAttempt BETWEEN @dateStart AND @dateFinish
	UNION
	SELECT 
		'Billing_Stats_Cost',
		NULL as Attempts, 
		SUM(CASE WHEN bs.Cost_Currency IS NULL then 0 ELSE bs.NumberOfCalls END) AS Calls,
		SUM(bs.CostDuration) / 60.0 AS Minutes 
	FROM Billing_Stats bs WITH(NOLOCK, INDEX(IX_Billing_Stats_Date)) WHERE bs.CallDate = @date
	UNION
	SELECT 
		'Billing_Stats_Sale',
		NULL as Attempts, 
		SUM(CASE WHEN bs.Sale_Currency IS NULL then 0 ELSE bs.NumberOfCalls END) AS Calls,
		SUM(bs.SaleDuration) / 60.0 AS Minutes 
	FROM Billing_Stats bs WITH(NOLOCK, INDEX(IX_Billing_Stats_Date)) WHERE bs.CallDate = @date	
END
GO
/****** Object:  StoredProcedure [dbo].[bp_CleanBillingStats]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_CleanBillingStats]
(
	@Date datetime, 
	@SwitchID tinyint = NULL, 
	@CustomerID varchar(10) = NULL, 
	@SupplierID varchar(10) = NULL, 
	@Batch bigint = 500, 
	@IsDebug char(1) = 'N'	
)
AS
-----------------------------
-- Delete Billing Stats
-----------------------------
DECLARE @DeletedCount bigint
SET NOCOUNT ON 
SELECT @DeletedCount = 1
SET ROWCOUNT @Batch
WHILE @DeletedCount > 0
BEGIN
	BEGIN TRANSACTION Cleaner							
		-- No Customer, No Supplier
		IF @CustomerID IS NULL AND @SupplierID IS NULL
			DELETE Billing_Stats FROM Billing_Stats WITH(NOLOCK, INDEX(IX_Billing_Stats_Date)) WHERE CallDate = @Date
		-- No Supplier
		ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
			DELETE Billing_Stats FROM Billing_Stats WITH(NOLOCK, INDEX(IX_Billing_Stats_Date, IX_Billing_Stats_Customer)) WHERE CallDate = @Date AND CustomerID = @CustomerID
		-- Customer, Supplier
		ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL	
			DELETE Billing_Stats FROM Billing_Stats WITH(NOLOCK, INDEX(IX_Billing_Stats_Date, IX_Billing_Stats_Customer, IX_Billing_Stats_Supplier)) WHERE CallDate = @Date AND CustomerID = @CustomerID AND SupplierID = @SupplierID
		-- No Customer
		ELSE
			DELETE Billing_Stats FROM Billing_Stats WITH(NOLOCK, INDEX(IX_Billing_Stats_Date, IX_Billing_Stats_Supplier)) WHERE CallDate = @Date AND SupplierID = @SupplierID
		SET @DeletedCount = @@ROWCOUNT
	COMMIT TRANSACTION Cleaner			
END
IF @IsDebug = 'Y' PRINT 'Deleted Billing Stats ' + convert(varchar(25), getdate(), 121)
GO
/****** Object:  StoredProcedure [dbo].[rpt_Volumes_DestinationTraffic_Old]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[rpt_Volumes_DestinationTraffic_Old](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@OurZoneID INT,
	@TopValues INT,
	@Attempts INT
)
with Recompile
AS 
	SET @FromDate=     CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	
 IF @OurZoneID=0
 SET @OurZoneID = null 
 SET ROWCOUNT @TopValues
   SELECT 
          bs.SaleZoneID AS SaleZoneID,
          SUM(BS.NumberOfCalls) AS Attempts,
          SUM(BS.SaleDuration)/60.0 AS Duration    
   FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
   WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
     	AND (@CustomerID IS NULL OR BS.CustomerID=@CustomerID) 
	    AND (@SupplierID IS NULL OR BS.SupplierID=@SupplierID)
	    AND (@OurZoneID IS NULL  OR BS.SaleZoneID=@OurZoneID)		
        AND  BS.NumberOfCalls > @Attempts
   GROUP BY bs.SaleZoneID
   ORDER BY Duration DESC
 SET ROWCOUNT 0
 
RETURN
GO
/****** Object:  StoredProcedure [dbo].[sp_TrafficStats_SupplierIncreaseReport]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[sp_TrafficStats_SupplierIncreaseReport]
	@FromDate DATETIME,
	@ToDate   DATETIME,
	@ZonesCommaSeperated VARCHAR(MAX)
WITH recompile 
AS
BEGIN
	SET @FromDate = CAST(
(
STR( YEAR( @FromDate ) ) + '-' +
STR( MONTH( @FromDate ) ) + '-' +
STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate = CAST(
(
STR( YEAR( @ToDate ) ) + '-' +
STR( MONTH(@ToDate ) ) + '-' +
STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)
	SET NOCOUNT ON
	
		SELECT
			Ts.SupplierID As SupplierID,
		 	Ts.CustomerID As CustomerID,
		 	Ts.SupplierZoneID AS SupplierZoneID,
			Sum(NumberOfCalls) as Attempts,
			Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts) ELSE NULL END as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(NumberOfCalls - SuccessfulAttempts) AS FailedAttempts,
			SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
			SUM(PDDinSeconds * TS.SuccessfulAttempts) as TotalPDD
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))			
        WHERE 
				FirstCDRAttempt BETWEEN @FromDate AND @ToDate
			AND TS.SupplierZoneID IN (SELECT * FROM  dbo.ParseArray(@ZonesCommaSeperated,','))
			Group By SupplierID, CustomerID, TS.SupplierZoneID
			

END
GO
/****** Object:  StoredProcedure [dbo].[sp_TrafficStats_ZoneMonitor]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Zone Monitor Stored Procedure
CREATE  PROCEDURE [dbo].[sp_TrafficStats_ZoneMonitor]
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10) = NULL,
	@SupplierID   varchar(10) = NULL,
    @SwitchID	  tinyInt = NULL,   
	@ShowE1       char(1) = 'N',
	@GroupByGateWay char(1) = 'N',
	@ShowSupplier Char(1)='N',
    @CodeGroup varchar(10) = NULL,
    @CarrierGroupID INT = NULL 
WITH RECOMPILE
AS	
BEGIN
	
	DECLARE @CarrierGroupPath VARCHAR(255)
	SELECT @CarrierGroupPath = cg.[Path] FROM CarrierGroup cg WHERE cg.CarrierGroupID = @CarrierGroupID
	
	DECLARE @FilteredCustomers TABLE (CarrierAccountID VARCHAR(10) PRIMARY KEY)
	
	IF @CarrierGroupPath IS NULL
		INSERT INTO @FilteredCustomers SELECT ca.CarrierAccountID FROM CarrierAccount ca WHERE ca.IsDeleted = 'N'
	ELSE
		INSERT INTO @FilteredCustomers 
			SELECT DISTINCT ca.CarrierAccountID 
				FROM CarrierAccount ca, CarrierGroup cg  
			WHERE
					ca.IsDeleted = 'N'
				AND cg.CarrierGroupID = ca.CarrierGroupID
				AND cg.[Path] LIKE (@CarrierGroupPath + '%')

	Declare @Results TABLE (OurZoneID int , GateWay varchar(50), Port_out varchar(50), Port_in varchar(50), SupplierID Varchar (15) ,Attempts int, FailedAttempts int, DurationsInMinutes numeric(13,5), ASR numeric(13,5), ACD numeric(13,5),DeliveredASR numeric(13,5), AveragePDD numeric(13,5), MaxDuration numeric(13,5), LastAttempt datetime, AttemptPercentage numeric(13,5),DurationPercentage numeric(13,5),SuccesfulAttempts int )
	SET NOCOUNT ON

	-- No Customer, No Supplier
	IF @CustomerID IS NULL AND @SupplierID IS NULL
		INSERT INTO @Results (OurZoneID, GateWay, Port_out, Port_in, SupplierID ,Attempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts)
			SELECT 
				TS.OurZoneID,        
				CASE WHEN @GroupByGateWay = 'Y' THEN CAC.GateWay ELSE NULL END AS GateWay,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END AS PortOut,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END AS PortIn,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END AS SupplierID,
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay = 'Y')  
					THEN Sum(Attempts)else Sum(NumberOfCalls) END as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay = 'Y')  THEN
				Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) 
				else
				case when Sum(NumberOfCalls) > 0 
				then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) 
				ELSE 0 END END as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				Sum(deliveredAttempts) * 100.0 / Sum(Attempts) as DeliveredASR, 
				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay = 'Y')  THEN Sum(Attempts - SuccessfulAttempts)else Sum(NumberOfCalls - SuccessfulAttempts) END as FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst)) 
				LEFT JOIN CarrierAccountConnection cac WITH(NOLOCK, INDEX(IX_CarrierAccountConnection_Account)) ON TS.SupplierID = cac.CarrierAccountID AND TS.SwitchID = cac.SwitchID AND TS.Port_OUT = cac.value
				LEFT JOIN Zone AS OZ WITH (NOLOCK) ON TS.OurZoneID = OZ.ZoneID
				LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
				LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
				WHERE 
					FirstCDRAttempt BETWEEN  @FromDateTime AND @ToDateTime  
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR TS.SwitchID = @SwitchID)
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
				AND TS.CustomerID IN (SELECT * FROM @FilteredCustomers)
			Group By 
				TS.OurZoneID
				, CASE WHEN @GroupByGateWay = 'Y' THEN CAC.GateWay ELSE NULL END
				, CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END
				, CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END
				, CASE WHEN @ShowSupplier = 'Y' THEN TS.SupplierID  ELSE NULL END
	-- Customer, No Supplier
	ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
		INSERT INTO @Results (OurZoneID, GateWay, Port_out, Port_in, SupplierID ,Attempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts)
			SELECT 
				TS.OurZoneID,          
				CASE WHEN @GroupByGateWay = 'Y' THEN CAC.GateWay ELSE NULL END AS GateWay,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END AS PortOut,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END AS PortIn,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END AS SupplierID,
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay = 'Y')  THEN Sum(Attempts)else Sum(NumberOfCalls) END as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay = 'Y')  THEN
				Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) 
				else
				case when Sum(NumberOfCalls) > 0 
				then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) 
				ELSE 0 END END as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay = 'Y')  THEN Sum(Attempts - SuccessfulAttempts)else Sum(NumberOfCalls - SuccessfulAttempts) END as FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer)) 
				 LEFT JOIN CarrierAccountConnection cac WITH(NOLOCK, INDEX(IX_CarrierAccountConnection_Account)) ON TS.SupplierID = cac.CarrierAccountID AND TS.SwitchID = cac.SwitchID AND TS.Port_OUT = cac.value
				 LEFT JOIN Zone AS OZ WITH (nolock) ON TS.OurZoneID = OZ.ZoneID
				 LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
				 LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
				WHERE 
					FirstCDRAttempt BETWEEN  @FromDateTime AND @ToDateTime  
				AND TS.CustomerID = @CustomerID 
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
				AND TS.CustomerID IN (SELECT * FROM @FilteredCustomers)
			Group By 
				TS.OurZoneID , 
				CASE WHEN @GroupByGateWay = 'Y' THEN CAC.GateWay ELSE NULL END,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END
	-- No Customer, Supplier
	ELSE IF @CustomerID IS NULL AND @SupplierID IS NOT NULL
		INSERT INTO @Results (OurZoneID, GateWay, Port_out, Port_in, SupplierID ,Attempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts)
			SELECT 
				TS.OurZoneID,          
				CASE WHEN @GroupByGateWay = 'Y' THEN CAC.GateWay ELSE NULL END AS GateWay,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END AS PortOut,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END AS PortIn,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END AS SupplierID,
				Sum(Attempts) as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) ELSE 0 end as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier)) 
				LEFT JOIN CarrierAccountConnection cac WITH(NOLOCK, INDEX(IX_CarrierAccountConnection_Account)) ON TS.SupplierID = cac.CarrierAccountID AND TS.SwitchID = cac.SwitchID AND  TS.Port_OUT = cac.value
				LEFT JOIN Zone AS OZ WITH (nolock) ON TS.OurZoneID = OZ.ZoneID
				LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
				LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
				WHERE 
					FirstCDRAttempt BETWEEN  @FromDateTime AND @ToDateTime  
				AND TS.SupplierID = @SupplierID 
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
				AND TS.CustomerID IN (SELECT * FROM @FilteredCustomers)
			Group By 
				TS.OurZoneID, 
				CASE WHEN @GroupByGateWay = 'Y' THEN CAC.GateWay ELSE NULL END,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END
	-- Customer, Supplier
	ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL
		INSERT INTO @Results (OurZoneID, GateWay, Port_out, Port_in, SupplierID ,Attempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts)
			SELECT 
				TS.OurZoneID,          
				CASE WHEN @GroupByGateWay = 'Y' THEN CAC.GateWay ELSE NULL END AS GateWay,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END AS PortOut,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END AS PortIn,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END AS SupplierID,
				Sum(Attempts) as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) ELSE 0 end as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier)) 
				LEFT JOIN CarrierAccountConnection cac WITH(NOLOCK, INDEX(IX_CarrierAccountConnection_Account)) ON TS.SupplierID = cac.CarrierAccountID AND TS.SwitchID = cac.SwitchID AND TS.Port_OUT = cac.value
				LEFT JOIN Zone AS OZ WITH (nolock) ON TS.OurZoneID = OZ.ZoneID
				LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
				LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
				WHERE 
					FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime  
				AND TS.CustomerID = @CustomerID 
				AND TS.SupplierID = @SupplierID 
			AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
				AND TS.CustomerID IN (SELECT * FROM @FilteredCustomers)
			Group By 
				TS.OurZoneID ,
				CASE WHEN @GroupByGateWay = 'Y' THEN CAC.GateWay ELSE NULL END, 
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END

	--Percentage For Attempts-----
	Declare @TotalAttempts bigint
	SELECT  @TotalAttempts = SUM(Attempts) FROM @Results
	Update  @Results SET AttemptPercentage= (Attempts * 100. / @TotalAttempts)

	SELECT * from @Results Order By Attempts DESC ,DurationsInMinutes DESC

END
GO
/****** Object:  Table [dbo].[ToDConsideration]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ToDConsideration](
	[ToDConsiderationID] [bigint] IDENTITY(1,1) NOT NULL,
	[ZoneID] [int] NOT NULL,
	[SupplierID] [varchar](5) NOT NULL,
	[CustomerID] [varchar](5) NOT NULL,
	[BeginTime] [varchar](12) NULL,
	[EndTime] [varchar](12) NULL,
	[WeekDay] [tinyint] NULL,
	[HolidayDate] [smalldatetime] NULL,
	[HolidayName] [nvarchar](255) NULL,
	[RateType] [tinyint] NULL,
	[BeginEffectiveDate] [smalldatetime] NULL,
	[EndEffectiveDate] [smalldatetime] NULL,
	[IsEffective]  AS (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
	[IsActive]  AS ([dbo].[IsToDActive]([HolidayDate],[WeekDay],[BeginTime],[EndTime],getdate())),
	[UserID] [int] NULL,
	[timestamp] [timestamp] NULL,
 CONSTRAINT [PK_ToDConsideration] PRIMARY KEY CLUSTERED 
(
	[ToDConsiderationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_ToDConsideration_Customer] ON [dbo].[ToDConsideration] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ToDConsideration_Dates] ON [dbo].[ToDConsideration] 
(
	[BeginEffectiveDate] DESC,
	[EndEffectiveDate] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ToDConsideration_Supplier] ON [dbo].[ToDConsideration] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ToDConsideration_Zone] ON [dbo].[ToDConsideration] 
(
	[ZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[bp_BuildCodeSupply]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_BuildCodeSupply]
	@SupplierID varchar(10) = NULL
AS
BEGIN
	SET NOCOUNT ON

	-- If no supplier or customer given then Empty Code Supply
	IF @SupplierID IS NULL
	  BEGIN
		TRUNCATE TABLE CodeSupply
	
		-- DROP INDEXES
		DROP INDEX IX_CodeSupply_Code ON CodeSupply
		DROP INDEX IX_CodeSupply_Zone ON CodeSupply
		DROP INDEX IX_CodeSupply_ServicesFlag ON CodeSupply	
		DROP INDEX IX_CodeSupply_Supplier ON CodeSupply

	  END
	ELSE
	  BEGIN
		DELETE FROM CodeSupply WHERE SupplierID = @SupplierID
	  END

	-- Build Code Supply
	INSERT INTO CodeSupply
	(
		Code,
		SupplierID,
		SupplierZoneID,
		SupplierNormalRate,
		SupplierOffPeakRate,
		SupplierWeekendRate,
		SupplierServicesFlag
	)
	SELECT
		CM.Code,
		CM.SupplierID,
		CM.SupplierZoneID,
		ZR.NormalRate,
		ZR.OffPeakRate,
		ZR.WeekendRate,		
		ZR.ServicesFlag
	FROM
		CodeMatch CM INNER JOIN ZoneRate ZR ON CM.SupplierZoneID = ZR.ZoneID 
			AND ZR.SupplierID <> 'SYS'
			AND (@SupplierID IS NULL OR ZR.SupplierID = @SupplierID)
	
	-- If no supplier or customer given then Empty Code Supply
	IF @SupplierID IS NULL 
	  BEGIN	
		-- Recreate INDEXES
		CREATE NONCLUSTERED INDEX IX_CodeSupply_Code ON CodeSupply(Code ASC)
		CREATE NONCLUSTERED INDEX IX_CodeSupply_Supplier ON CodeSupply(SupplierID ASC)
		CREATE NONCLUSTERED INDEX IX_CodeSupply_Zone ON CodeSupply(SupplierZoneID ASC)
		CREATE NONCLUSTERED INDEX IX_CodeSupply_ServicesFlag ON CodeSupply(SupplierServicesFlag ASC)
	  END

END
GO
/****** Object:  UserDefinedFunction [dbo].[GetZoneRateOfForcedSpecialRequest]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ========================================================
-- Author:		Hassan Kheir Eddine
-- Create date: 2009-03-02
-- Description:	Get a table of Exchange Rates based on Date
-- =========================================================
CREATE FUNCTION [dbo].[GetZoneRateOfForcedSpecialRequest]
(
)
RETURNS 
@SpecialRequestRates TABLE 
(
	ZoneID int,
    Code varchar(20),
	SupplierID varchar(5),
	NormalRate real
	PRIMARY KEY(ZoneID, Code)
)
AS
BEGIN

INSERT INTO @SpecialRequestRates
(
	ZoneID,
	Code,
	SupplierID,
	NormalRate
)
SELECT 
    sr.ZoneID,
    sr.Code,
    sr.SupplierID,
    zr.NormalRate 
FROM  
     SpecialRequest sr WITH(NOLOCK,INDEX(IX_SpecialRequest_Supplier,IX_SpecialRequest)) ,
     CodeMatch cm  WITH(NOLOCK,INDEX(IDX_CodeMatch_Code,IDX_CodeMatch_Supplier,IDX_CodeMatch_Zone)) ,
     ZoneRate zr  WITH(NOLOCK,INDEX(IX_ZoneRate_Supplier,IX_ZoneRate_Zone))
WHERE sr.SpecialRequestType = 1
AND sr.IsEffective = 'Y' 
AND sr.SupplierID = zr.SupplierID 
AND cm.SupplierID = sr.SupplierID
AND (sr.ZoneID = zr.ZoneID OR (sr.Code = cm.Code AND cm.SupplierZoneID = zr.ZoneID))
	
RETURN
END
GO
/****** Object:  StoredProcedure [dbo].[sp_TrafficStats_CarrierMonitor]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_TrafficStats_CarrierMonitor]
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10) = NULL,
	@SupplierID   varchar(10) = NULL,
    @OurZoneID 	  INT = NULL,
	@SwitchID	  tinyInt = NULL, 
	@GroupBySupplier CHAR(1) = 'N',
	@IncludeCarrierGroupSummary CHAR(1) = 'N'
WITH recompile 
AS
BEGIN

	DECLARE @Results TABLE(
			CarrierAccountID VARCHAR(5),
			Attempts INT, 
			DurationsInMinutes NUMERIC(19,6),          
			ASR NUMERIC(19,6),
			ACD NUMERIC(19,6),
			DeliveredASR NUMERIC(19,6), 
			AveragePDD NUMERIC(19,6),
			MaxDuration NUMERIC(19,6),
			LastAttempt DATETIME,
			SuccessfulAttempts BIGINT,
			FailedAttempts BIGINT,
			DeliveredAttempts BIGINT,
			PDDInSeconds NUMERIC(19,6)
	)	
	
	SET NOCOUNT ON
	
	-- No Customer, No Supplier, GroupByCustomer
	IF @CustomerID IS NULL AND @SupplierID IS NULL AND @GroupBySupplier = 'N'
		INSERT INTO @Results
		SELECT	CustomerID As CarrierAccountID,
			Sum(NumberOfCalls) as Attempts,
			Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts) ELSE NULL END as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(NumberOfCalls - SuccessfulAttempts) AS FailedAttempts,
			SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
			SUM(PDDinSeconds * TS.SuccessfulAttempts) as TotalPDD
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
		LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
     WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
			AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
			AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR TS.SwitchID = @SwitchID)
			Group By CustomerID
			ORDER BY SUM(Attempts) DESC
			
	-- No Customer, No Supplier, GroupBySupplier
	IF @CustomerID IS NULL AND @SupplierID IS NULL AND @GroupBySupplier = 'Y'
		INSERT INTO @Results
		SELECT	SupplierID As CarrierAccountID,
			Sum(Attempts) as Attempts, 
			Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
			Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts) ELSE NULL END as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
			SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
			SUM(PDDinSeconds * TS.SuccessfulAttempts) as TotalPDD 
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
		LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
			AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
			AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
			Group By SupplierID
			ORDER BY SUM(Attempts) DESC
			
	-- Customer, No Supplier
	ELSE IF (@CustomerID IS NOT NULL AND @SupplierID IS NULL) OR @GroupBySupplier = 'Y' 
		INSERT INTO @Results
		SELECT	SupplierID As CarrierAccountID,
			Sum(Attempts) as Attempts, 
			Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
			Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts) ELSE NULL END as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
			SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
			SUM(PDDinSeconds * TS.SuccessfulAttempts) as TotalPDD
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
		LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
			AND CustomerID = @CustomerID 
			AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
			AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
			Group By SupplierID 
			ORDER BY SUM(Attempts) DESC
	-- No Customer, Supplier
	ELSE IF @CustomerID IS NULL AND @SupplierID IS NOT NULL AND @GroupBySupplier = 'N'
			INSERT INTO @Results
			SELECT	CustomerID As CarrierAccountID,
					Sum(NumberOfCalls) as Attempts, 
					Sum(DurationsInSeconds/60.0) as DurationsInMinutes,          
					CASE WHEN Sum(NumberOfCalls) > 0 THEN Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END AS ASR,
					case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
					Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
					CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts) ELSE NULL END as AveragePDD,
					MAX(DurationsInSeconds)/60.0 as  MaxDuration,
					Max(LastCDRAttempt) as LastAttempt,
					Sum(SuccessfulAttempts)AS SuccessfulAttempts,
					Sum(NumberOfCalls - SuccessfulAttempts) AS FailedAttempts,
					SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
					SUM(PDDinSeconds * TS.SuccessfulAttempts) as TotalPDD 
				FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier))
				LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
					WHERE 
						FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
					AND SupplierID = @SupplierID 
					AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
					AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR TS.SwitchID = @SwitchID)
					Group By CustomerID 
					ORDER BY SUM(Attempts) DESC
	-- Customer, Supplier, GroupByCustomer
	ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL AND @GroupBySupplier = 'N'
			INSERT INTO @Results
			SELECT CustomerID As CarrierAccountID,
					Sum(NumberOfCalls) as Attempts, 
					Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
					case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
					case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
					Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
					CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts) ELSE NULL END as AveragePDD,
					MAX(DurationsInSeconds)/60.0 as  MaxDuration,
					Max(LastCDRAttempt) as LastAttempt,
					Sum(SuccessfulAttempts)AS SuccessfulAttempts,
					Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
					SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
					SUM(PDDinSeconds * TS.SuccessfulAttempts) as TotalPDD 
				FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier))
				LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
					WHERE 
						FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
					AND CustomerID = @CustomerID 
					AND SupplierID = @SupplierID 
					AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
					AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR TS.SwitchID = @SwitchID)
					Group By CustomerID 
					ORDER BY SUM(Attempts) DESC
			
	-- Customer, Supplier, GroupBySupplier
	ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL AND @GroupBySupplier = 'N'
			INSERT INTO @Results
			SELECT	SupplierID As CarrierAccountID,
					Sum(Attempts) as Attempts, 
					Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
					Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
					case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
					Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
					CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts) ELSE NULL END as AveragePDD,
					MAX(DurationsInSeconds)/60.0 as  MaxDuration,
					Max(LastCDRAttempt) as LastAttempt,
					Sum(SuccessfulAttempts)AS SuccessfulAttempts,
					Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
					SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
					SUM(PDDinSeconds * TS.SuccessfulAttempts) as TotalPDD 				
				FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier))
				LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
					WHERE 
						FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
					AND CustomerID = @CustomerID 
					AND SupplierID = @SupplierID 
					AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
					AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR TS.SwitchID = @SwitchID)
					Group By SupplierID 
					ORDER BY SUM(Attempts) DESC

	-- Return the results
	SELECT * FROM @Results

	-- In case Carrier Grouping is required
	IF @IncludeCarrierGroupSummary = 'Y' 
	BEGIN
		SELECT 
			cg.CarrierGroupID, 
			cg.[Path], 
			SUM(R.Attempts) as Attempts,
			SUM(R.DurationsInMinutes) as DurationsInMinutes,
			(SUM(SuccessfulAttempts) * 100.0 / SUM(R.Attempts)) AS ASR,
			(SUM(DurationsInMinutes) / SUM(R.SuccessfulAttempts)) AS ACD,
			(SUM(R.DeliveredAttempts) * 100.0 / SUM(R.Attempts)) AS DeliveredASR,
			CASE WHEN SUM(R.SuccessfulAttempts) > 0 THEN SUM(R.PDDinSeconds) / SUM(R.SuccessfulAttempts) ELSE NULL END as AveragePDD,
			MAX(R.MaxDuration) AS MaxDuration,
			MAX(R.LastAttempt) AS LastAttempt,
			SUM(R.SuccessfulAttempts) AS SuccessfulAttempts, 
			SUM(R.FailedAttempts) AS FailedAttempts				
		FROM 
			@Results R 
				INNER JOIN CarrierAccount ca ON ca.CarrierAccountID = R.CarrierAccountID 
				LEFT JOIN CarrierGroup g ON ca.CarrierGroupID = g.CarrierGroupID
				LEFT JOIN CarrierGroup cg ON g.Path LIKE (cg.[Path] + '%')
				  
		GROUP BY 
			cg.CarrierGroupID, cg.[Path]
		ORDER BY cg.[Path]
	END

END
GO
/****** Object:  StoredProcedure [dbo].[bp_AddSwitchHistory]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_AddSwitchHistory]
	@SwitchID tinyint,
	@Symbol varchar(10),
	@Name varchar(512),
	@Description text,
	@Configuration ntext,
	@LastCDRImportTag varchar(255),
	@LastImport datetime,
	@LastRouteUpdate datetime,
	@UserID int,
	@Enable_CDR_Import char(1),
	@Enable_Routing char(1),
	@LastAttempt datetime

AS
BEGIN
	INSERT INTO [SwitchHistory]
           ([Date]
           ,[SwitchID]
           ,[Symbol]
           ,[Name]
           ,[Description]
           ,[Configuration]
           ,[LastCDRImportTag]
           ,[LastImport]
           ,[LastRouteUpdate]
           ,[UserID]
           ,[Enable_CDR_Import]
           ,[Enable_Routing]
           ,[LastAttempt])
     VALUES
           (GETDATE(),
           	@SwitchID,
			@Symbol,
			@Name,
			@Description,
			@Configuration,
			@LastCDRImportTag,
			@LastImport,
			@LastRouteUpdate,
			@UserID,
			@Enable_CDR_Import,
			@Enable_Routing,
			@LastAttempt
			)
END
GO
/****** Object:  StoredProcedure [dbo].[UsefulQueries]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UsefulQueries]
	with Recompile
AS
BEGIN

	RETURN 0

	/*
	* Convert Special Requests to Route Overrides 
	*/	
	DELETE RouteOverride 
		FROM RouteOverride, SpecialRequest sr 
		WHERE RouteOverride.CustomerID = sr.CustomerID AND RouteOverride.Code = sr.Code  

	INSERT INTO RouteOverride
	(
		CustomerID,
		Code,
		IncludeSubCodes,
		OurZoneID,
		RouteOptions,
		BlockedSuppliers,
		BeginEffectiveDate
	)
	SELECT sr.CustomerID, sr.Code, 'N', 0, '', NULL, MIN(sr.BeginEffectiveDate) 
		FROM SpecialRequest sr
		GROUP BY sr.CustomerID, sr.Code

	DECLARE @Code VARCHAR(30)
	DECLARE @CustomerID VARCHAR(30)
	DECLARE @SupplierID VARCHAR(30)

	DECLARE curSpecialRequests CURSOR LOCAL FAST_FORWARD FOR 
		SELECT sr.CustomerID, sr.Code, sr.SupplierID
			FROM SpecialRequest sr (NOLOCK)
		ORDER BY sr.CustomerID, sr.Code, sr.Priority DESC
	OPEN curSpecialRequests
	FETCH NEXT FROM curSpecialRequests INTO @CustomerID, @Code, @SupplierID 	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE RouteOverride
			SET RouteOptions = CASE WHEN RouteOptions = '' THEN @SupplierID ELSE RouteOptions + '|' + @SupplierID END
			WHERE CustomerID = @CustomerID AND Code = @Code
			AND LEN(RouteOptions) < 90
		FETCH NEXT FROM curSpecialRequests INTO @CustomerID, @Code, @SupplierID 	
	END
	CLOSE curSpecialRequests
	DEALLOCATE curSpecialRequests	

	SELECT * FROM RouteOverride ro

	SELECT * FROM SpecialRequest sr WHERE sr.SupplierID = 'C015' AND sr.CustomerID = 'C042' ORDER BY sr.Code

END
GO
/****** Object:  StoredProcedure [dbo].[bp_CleanUpAfterStateBackupRestore]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================================================
-- Author:		Fadi Chamieh
-- Create date: 2011-02-17
-- Description:	Clean Up Entities that may become invalid after
--              a state backup restore
-- =============================================================================
CREATE PROCEDURE [dbo].[bp_CleanUpAfterStateBackupRestore]
AS
BEGIN
	SET NOCOUNT ON;
	-- DELETE FROM SpecialRequest WHERE ZoneID IS NOT NULL AND ZoneID NOT IN (SELECT ZoneID FROM Zone WHERE SupplierID = 'SYS')
	DELETE FROM RouteBlock WHERE ZoneID IS NOT NULL AND ZoneID NOT IN (SELECT ZoneID FROM Zone WHERE SupplierID <> 'SYS')
	DELETE FROM RouteOverride WHERE OurZoneID IS NOT NULL AND OurZoneID NOT IN (SELECT ZoneID FROM Zone WHERE SupplierID = 'SYS')
	DELETE FROM PricingTemplatePlan WHERE ZoneID IS NOT NULL AND ZoneID NOT IN (SELECT ZoneID FROM Zone WHERE SupplierID = 'SYS')
	-- DELETE FROM Route WHERE OurZoneID > 0 AND OurZoneID NOT IN (SELECT ZoneID FROM Zone WHERE SupplierID = 'SYS')
	-- DELETE FROM RouteOption WHERE SupplierZoneID > 0 AND SupplierZoneID NOT IN (SELECT ZoneID FROM Zone WHERE SupplierID <> 'SYS')
END
GO
/****** Object:  UserDefinedFunction [dbo].[GetRouteOverrides]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =====================================================================
-- Author:		Fadi Chamieh
-- Create date: 2009-04-23
-- Update date: 2009-06-24 (Zone ID < 1)
-- Description:	Get a list of Route Overrides Matching the given params
-- =====================================================================
CREATE FUNCTION [dbo].[GetRouteOverrides](@CustomerID VARCHAR(10), @OurZoneID INT, @Code VARCHAR(15), @OverrideType VARCHAR(20) = 'ALL')
RETURNS TABLE
AS
RETURN 
(
	SELECT * FROM RouteOverride ro
		WHERE 
			@OverrideType IN ('OVERRIDES', 'ALL') 
		AND (ro.CustomerID = @CustomerID OR ro.CustomerID = '*ALL*')
		AND (ro.Code = @Code OR ro.Code = '*ALL*')
		AND (ro.OurZoneID = @OurZoneID OR ro.OurZoneID < 1)
		AND ro.IsEffective = 'Y'
		AND ro.RouteOptions IS NOT NULL
	UNION ALL
	SELECT * FROM RouteOverride ro
		WHERE 
			@OverrideType IN ('BLOCKS', 'ALL') 
		AND (ro.CustomerID = @CustomerID OR ro.CustomerID = '*ALL*')
		AND (ro.Code = @Code OR ro.Code = '*ALL*')
		AND (ro.OurZoneID = @OurZoneID OR ro.OurZoneID < 1)
		AND ro.IsEffective = 'Y'
		AND ro.BlockedSuppliers IS NOT NULL	
)
GO
/****** Object:  StoredProcedure [dbo].[bp_GetEffectiveCodeZoneRate]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_GetEffectiveCodeZoneRate](@SupplierID varchar(10), @CDPN varchar(25), @Attempt datetime)
AS
BEGIN

	SELECT 
			c.ID CodeID, 
			C.Code, 
			c.BeginEffectiveDate AS CodeBED, 
			c.EndEffectiveDate AS CodeEED,
			z.ZoneID,
			z.CodeGroup,
			z.Name,
			z.ServicesFlag AS ZoneServicesFlag,
			z.BeginEffectiveDate AS ZoneBED, 
			z.EndEffectiveDate AS ZoneEED,
			r.RateID,
			r.PriceListID,
			r.ServicesFlag AS RateServicesFlag,
			r.Rate,
			r.BeginEffectiveDate AS RateBED, 
			r.EndEffectiveDate AS RateEED			
	  FROM Code c, Zone z, Rate r WHERE c.ZoneID = z.ZoneID AND r.ZoneID = c.ZoneID 
	AND z.SupplierID = @SupplierID
	AND @CDPN LIKE c.Code + '%'  
	AND c.BeginEffectiveDate <= @Attempt
	AND ISNULL(c.EndEffectiveDate, '2025-01-01') >= @Attempt

	AND z.BeginEffectiveDate <= @Attempt
	AND ISNULL(z.EndEffectiveDate, '2025-01-01') >= @Attempt

	AND r.BeginEffectiveDate <= @Attempt
	AND ISNULL(r.EndEffectiveDate, '2025-01-01') >= @Attempt
	
END
GO
/****** Object:  StoredProcedure [dbo].[bp_GetSaleZoneRates]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ======================================================================
-- Author:		Fadi Chamieh
-- Create date: 2009-04-24
-- Description: Get the Sale Rates for the Zones defined in our System
-- ======================================================================
CREATE PROCEDURE [dbo].[bp_GetSaleZoneRates](
	@codeFilter varchar(30) = NULL, 
	@ExcludedRate float = -1,
	@zoneNameFilter varchar(50) = NULL, 
	@ServicesFlag smallint = 0,
	@Days int = NULL
)
WITH RECOMPILE
AS
BEGIN
	SET NOCOUNT ON;

	SELECT zr.CustomerID, zr.ZoneID, zr.NormalRate, zr.OffPeakRate, zr.WeekendRate, zr.ServicesFlag
	  FROM ZoneRate zr 
		WHERE 
				zr.SupplierID = 'SYS'
			AND (@zoneNameFilter IS NULL OR ZR.ZoneID IN (SELECT ZoneID FROM Zone WHERE Name LIKE @zoneNameFilter)) 
			AND (@codeFilter IS NULL OR ZR.ZoneID IN (SELECT ZoneID FROM Code WHERE Code LIKE @codeFilter))
			AND (@ServicesFlag IS NULL OR ZR.ServicesFlag & @ServicesFlag = @ServicesFlag)
			AND zr.NormalRate <> @ExcludedRate			
	ORDER BY zr.ZoneID, zr.NormalRate
		 
END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_DailyReport]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--
-- Get an Hourly report for traffic (using given parameters)
--
CREATE   PROCEDURE [dbo].[SP_TrafficStats_DailyReport] 
    @FromDateTime	datetime,
	@ToDateTime		datetime,
	@CustomerID		varchar(10) = NULL,
	@SupplierID		varchar(10) = NULL,
    @SwitchID		tinyInt = NULL,
    @OurZoneID 		int = NULL,
    @PortInOut		varchar(50) = NULL,
	@SelectPort		int = NULL,
    @ConnectionType varchar(50) = NULL ,
    @CodeGroup varchar(10) = NULL  
AS
BEGIN	
	SET NOCOUNT ON
	SELECT
         dbo.DateOf(LastCDRAttempt) AS [Day],
        Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	    Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
		MAX(DurationsInSeconds) / 60.0 as MaxDuration,
        Max(LastCDRAttempt) as LastAttempt,
        Sum(SuccessfulAttempts) AS SuccessfulAttempt
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier))
        LEFT OUTER JOIN
        Zone AS Z ON TS.OurZoneID = Z.ZoneID                                     
   WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND (@CustomerID IS NULL OR TS.CustomerID = @CustomerID) 
		AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID) 
	    AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
	    AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
	    AND (@ConnectionType IS NULL OR TS.Port_OUT = @ConnectionType)      
	    AND Ts.Port_IN like (CASE @SelectPort WHEN 0 THEN @PortInOut ELSE '%%' END)    
        AND Ts.Port_out like (CASE @SelectPort WHEN 1 THEN @PortInOut ELSE '%%' END) 
        AND (@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
   
	GROUP BY  dbo.DateOf(LastCDRAttempt)
	ORDER BY [Day] 

END
GO
/****** Object:  UserDefinedFunction [dbo].[GetSaleCodeGaps]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetSaleCodeGaps](@CustomerID varchar(10), @when datetime = NULL)
RETURNS @results TABLE 
	(
		CustomerID varchar(10), 
		ParentCode varchar(15),
		ParentCodeID bigint,
		ParentZone nvarchar(255), 
		ParentZoneID int,
		ParentRate numeric(13,5),
		ParentRateID bigint,
		Code varchar(15),
		CodeID bigint,
		Zone nvarchar(255),
		ZoneID int
	)
AS
BEGIN
	
	INSERT INTO @results
	SELECT 
			pp.CustomerID, 
			pc.Code AS ParentCode, 
			pc.ID AS ParentCodeID,
			pz.Name AS ParentZone,
			pz.ZoneID AS ParentZoneID,
			pr.Rate AS ParentRate, 
			pr.RateID AS ParentRateID,
			c.Code as Code,
			c.ID AS CodeID, 
			z.Name AS Zone,
			z.ZoneID AS ZoneID
	  FROM PriceList pp, Rate pr, Zone pz, Code pc, Code c, Zone z
	WHERE 
			pp.CustomerID = @CustomerID
		AND pp.SupplierID = 'SYS'
		AND pp.PriceListID = pr.PriceListID
		AND pr.BeginEffectiveDate <= @when AND (pr.EndEffectiveDate IS NULL OR pr.EndEffectiveDate > @when)
		AND pr.ZoneID = pz.ZoneID
		AND	pc.ZoneID = pz.ZoneID
		AND pz.BeginEffectiveDate <= @when AND (pz.EndEffectiveDate IS NULL OR pz.EndEffectiveDate > @when)
		AND pc.BeginEffectiveDate <= @when AND (pc.EndEffectiveDate IS NULL OR pc.EndEffectiveDate > @when)
		AND pz.CodeGroup = z.CodeGroup
		AND c.BeginEffectiveDate <= @when AND (c.EndEffectiveDate IS NULL OR c.EndEffectiveDate > @when)
		AND c.Code LIKE (pc.Code + '%')
		AND c.Code > pc.Code
		AND z.ZoneID = c.ZoneID
		AND z.SupplierID = 'SYS'
		AND z.BeginEffectiveDate <= @when AND (z.EndEffectiveDate IS NULL OR z.EndEffectiveDate > @when)
		AND 
			NOT EXISTS(
					SELECT * FROM Rate r, PriceList pl 
						WHERE pl.CustomerID = @CustomerID
							AND pl.SupplierID = 'SYS' 
							AND r.PriceListID = pl.PriceListID
							AND r.BeginEffectiveDate <= @when AND (r.EndEffectiveDate IS NULL OR r.EndEffectiveDate > @when)
							AND r.ZoneID = z.ZoneID
			)
	RETURN
END
GO
/****** Object:  StoredProcedure [dbo].[sp_TrafficStats_ByPeriods]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_TrafficStats_ByPeriods]
@PeriodType varchar(50) = 'Days',
@ZoneID INT = NULL ,
@SupplierID VarChar(15) = NULL ,
@CustomerID Varchar(15) = NULL ,
@CodeGroup varchar(10) = NULL ,
@FromDate DATETIME ,
@TillDate DATETIME
AS
BEGIN 


SELECT @FromDate = dbo.DateOf(@FromDate)
SET @TillDate= CAST(
(
	STR( YEAR( @TillDate ) ) + '-' +
	STR( MONTH(@TillDate ) ) + '-' +
	STR( DAY( @TillDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)	

DECLARE @Results TABLE (
    [Day] varchar(20), 
    Attempts numeric(13,5) NULL, 
	DurationsInMinutes numeric(13,5),
    ASR numeric(13,5), 
	ACD numeric(13,5), 
	DeliveredASR numeric(13,5),
	AveragePDD numeric (13,5),
	MaxDuration numeric (13,5),
	LastAttempt datetime ,
    SuccessfulAttempts int
                      )
SET NOCOUNT ON

if @CustomerID is null and @SupplierID IS NULL
	
	INSERT INTO @Results
		(   
			[Day],
			Attempts,
			DurationsInMinutes ,
			ASR,
			ACD,
			DeliveredASR,
			AveragePDD,
			MaxDuration ,
			LastAttempt ,
			SuccessfulAttempts
		 )	 	
		SELECT 
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Date],
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst)) 
			JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
		WHERE 
			TS.FirstCDRAttempt >= @FromDate AND TS.FirstCDRAttempt <=@TillDate
			AND (@ZoneID IS NULL OR OZ.ZoneID= @ZoneID) 
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		GROUP BY 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)	
			END
--		ORDER BY Min(FirstCDRAttempt) DESC 
   
if @CustomerID is not null and @SupplierID IS NULL
	
	INSERT INTO @Results
		(   
			[Day],
			Attempts,
			DurationsInMinutes ,
			ASR,
			ACD,
			DeliveredASR,
			AveragePDD,
			MaxDuration ,
			LastAttempt ,
			SuccessfulAttempts
		 )	 	
		SELECT 
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Date],
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer)) 
			JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
		WHERE 
			TS.FirstCDRAttempt >= @FromDate AND TS.FirstCDRAttempt <=@TillDate
     		AND (ts.CustomerID = @CustomerID) 
			AND (@ZoneID IS NULL OR OZ.ZoneID= @ZoneID) 
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		GROUP BY 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)	
			END
		ORDER BY Min(FirstCDRAttempt) DESC 


if @CustomerID is null and @SupplierID IS not NULL
	
	INSERT INTO @Results
		(   
			[Day],
			Attempts,
			DurationsInMinutes ,
			ASR,
			ACD,
			DeliveredASR,
			AveragePDD,
			MaxDuration ,
			LastAttempt ,
			SuccessfulAttempts
		 )	 	
		SELECT 
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Date],
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier)) 
			JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
		WHERE 
			TS.FirstCDRAttempt >= @FromDate AND TS.FirstCDRAttempt <=@TillDate
     		AND (ts.SupplierID = @SupplierID) 
			AND (@ZoneID IS NULL OR OZ.ZoneID= @ZoneID) 
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		GROUP BY 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)	
			END
		ORDER BY Min(FirstCDRAttempt) DESC 

if @CustomerID  is not null and @SupplierID IS not NULL
	
	INSERT INTO @Results
		(   
			[Day],
			Attempts,
			DurationsInMinutes ,
			ASR,
			ACD,
			DeliveredASR,
			AveragePDD,
			MaxDuration ,
			LastAttempt ,
			SuccessfulAttempts
		 )	 	
		SELECT 
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Date],
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier)) 
			JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
		WHERE 
			TS.FirstCDRAttempt >= @FromDate AND TS.FirstCDRAttempt <=@TillDate
     		AND (ts.CustomerID = @CustomerID) 
     		AND (ts.SupplierID = @SupplierID) 
			AND (@ZoneID IS NULL OR OZ.ZoneID= @ZoneID) 
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		GROUP BY 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)	
			END
		ORDER BY Min(FirstCDRAttempt) DESC 

	SELECT * FROM @Results ORDER BY [Day] 
END
GO
/****** Object:  StoredProcedure [dbo].[bp_FixErroneousEffectiveCodes]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_FixErroneousEffectiveCodes]
AS
UPDATE Code SET EndEffectiveDate = 
	(
		SELECT TOP 1 CN.BeginEffectiveDate FROM Code CN, Zone ZN, Zone ZO 
			WHERE 
				ZO.ZoneID = Code.ZoneID
			AND ZO.SupplierID = ZN.SupplierID 
			AND ZN.ZoneID = CN.ZoneID
			AND CN.Code = Code.Code 
			AND (
				(CN.BeginEffectiveDate > Code.BeginEffectiveDate AND CN.ID <> Code.ID) 
				OR 
				(CN.BeginEffectiveDate = Code.BeginEffectiveDate AND CN.ID > Code.ID)
				) 				
			AND CN.EndEffectiveDate IS NULL
			ORDER BY CN.BeginEffectiveDate ASC
	)
WHERE 
		Code.EndEffectiveDate IS NULL
	AND EXISTS 
		(
			SELECT * FROM Code CN, Zone ZN, Zone ZO 
				WHERE 
					ZO.ZoneID = Code.ZoneID
				AND ZO.SupplierID = ZN.SupplierID 
				AND ZN.ZoneID = CN.ZoneID
				AND CN.Code = Code.Code 
				AND (
					(CN.BeginEffectiveDate > Code.BeginEffectiveDate AND CN.ID <> Code.ID) 
					OR 
					(CN.BeginEffectiveDate = Code.BeginEffectiveDate AND CN.ID > Code.ID)
					) 				
				AND CN.EndEffectiveDate IS NULL
		)
GO
/****** Object:  StoredProcedure [dbo].[Sp_TrafficStats_ReleaseCodeStats]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Sp_TrafficStats_ReleaseCodeStats] 
	@FromDate	DATETIME,
	@ToDate		DATETIME,
	@CustomerID varchar(10) = NULL,
	@SupplierID varchar(10) = NULL,
    @OurZoneID 	INT = NULL,
    @SwitchID tinyint = NULL,
    @GroupByZone CHAR(1) = 'N',
    @ShowE1       char(1) = 'N'
AS
BEGIN

Declare @PrimaryResult TABLE (OurZoneID INT, ReleaseCode varchar(50), ReleaseSource Char(10) NULL,DurationsInMinutes Numeric (13,5), Attempts bigint NULL,SuccessfulAttempts bigint NULL,FirstAttempt datetime, LastAttempt DATETIME, Port_out varchar(50), Port_in varchar(50))

SET NOCOUNT ON

INSERT INTO @PrimaryResult (OurZoneID, ReleaseCode, ReleaseSource,DurationsInMinutes, Attempts,SuccessfulAttempts,FirstAttempt, LastAttempt, Port_out, Port_in)
	SELECT    
		CASE WHEN @GroupByZone = 'Y' THEN bm.OurZoneID ELSE 0 END AS OurZoneID,  
		ReleaseCode,
		ReleaseSource,
		SUM(DurationInSeconds) / 60.0 AS DurationsInMinutes,   
		Count(*),
		SUM( CASE WHEN DurationInSeconds > 0 THEN 1 ELSE 0 END),
		Min(Attempt),
		Max(Attempt),
		CASE WHEN @ShowE1 IN ('B', 'O') THEN bm.Port_OUT ELSE NULL END AS PortOut,
		CASE WHEN @ShowE1 IN ('B', 'I') THEN bm.Port_IN ELSE NULL END AS PortIn
	FROM Billing_CDR_Main bm WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt))  
	  
	WHERE       Attempt BETWEEN @FromDate AND @ToDate
			AND(@CustomerID IS NULL OR CustomerID = @CustomerID) 
			AND(@SupplierID IS NULL OR SupplierID = @SupplierID) 
			AND(@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
			AND(@SwitchID IS NULL OR SwitchID = @SwitchID)
	Group By
			ReleaseCode, ReleaseSource
			, CASE WHEN @GroupByZone = 'Y' THEN bm.OurZoneID ELSE 0 END
			, CASE WHEN @ShowE1 IN ('B', 'O') THEN bm.Port_OUT ELSE NULL END
			, CASE WHEN @ShowE1 IN ('B', 'I') THEN bm.Port_IN ELSE NULL END 			
	OPTION (RECOMPILE)

INSERT INTO @PrimaryResult (OurZoneID, ReleaseCode,ReleaseSource,DurationsInMinutes, Attempts,SuccessfulAttempts, FirstAttempt, LastAttempt, Port_out, Port_in)
	SELECT    
		CASE WHEN @GroupByZone = 'Y' THEN bm.OurZoneID ELSE 0 END AS OurZoneID,
		ReleaseCode,
		ReleaseSource,
		SUM (DurationInSeconds) / 60.0 AS DurationsInMinutes,   
		Count (*),
		SUM( CASE WHEN DurationInSeconds > 0 THEN 1 ELSE 0 END),
		Min(Attempt),
		Max(Attempt),
		CASE WHEN @ShowE1 IN ('B', 'O') THEN bm.Port_OUT ELSE NULL END AS PortOut,
		CASE WHEN @ShowE1 IN ('B', 'I') THEN bm.Port_IN ELSE NULL END AS PortIn
	FROM Billing_CDR_Invalid bm WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt))
		WHERE Attempt BETWEEN @FromDate AND @ToDate
			AND(@CustomerID IS NULL OR CustomerID = @CustomerID) 
			AND(@SupplierID IS NULL OR SupplierID = @SupplierID) 
			AND(@OurZoneID IS NULL OR  OurZoneID = @OurZoneID)
			AND(@SwitchID IS NULL OR SwitchID = @SwitchID)
		Group By ReleaseCode, ReleaseSource
		, CASE WHEN @GroupByZone = 'Y' THEN bm.OurZoneID ELSE 0 END
		, CASE WHEN @ShowE1 IN ('B', 'O') THEN bm.Port_OUT ELSE NULL END
		, CASE WHEN @ShowE1 IN ('B', 'I') THEN bm.Port_IN ELSE NULL END
	OPTION (RECOMPILE)

DECLARE @Result TABLE(OurZoneID int, OurZoneName NVARCHAR(200), ReleaseCode varchar(50),ReleaseSource Char(10) NULL,DurationsInMinutes Numeric (13,5), Attempts bigint NULL,FailedAttempts bigint NULL,FirstAttempt datetime, LastAttempt datetime,Percentage FLOAT, Port_out varchar(50), Port_in varchar(50))	

INSERT INTO @Result(OurZoneID, OurZoneName, ReleaseCode, ReleaseSource, DurationsInMinutes, Attempts,FailedAttempts, FirstAttempt, LastAttempt,Percentage, Port_out, Port_in)
	SELECT  
			z.ZoneID,  
			z.[Name],
			ReleaseCode,
			ReleaseSource,
			Sum(DurationsInMinutes),
			Sum(Attempts),
			Sum(Attempts) - Sum(SuccessfulAttempts),
			Min(FirstAttempt),	
			Max(LastAttempt),
			0,
			pr.Port_out,
			pr.Port_in
	From @PrimaryResult pr LEFT JOIN Zone z ON pr.OurZoneID = z.ZoneID     
	GROUP BY z.ZoneID, z.[Name], ReleaseCode, ReleaseSource , pr.Port_out, pr.Port_in
	ORDER BY Sum(DurationsInMinutes) DESC , Sum(Attempts)DESC

Declare @TotalAttempts bigint
SELECT  @TotalAttempts = SUM(Attempts) FROM @Result
Update  @Result SET Percentage = (Attempts * 100. / @TotalAttempts)

SELECT * from @Result ORDER BY Attempts DESC


END
GO
/****** Object:  StoredProcedure [dbo].[sp_TrafficStats_ZoneMonitorDetails]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_TrafficStats_ZoneMonitorDetails]
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10)=null,
	@SupplierID   varchar(10)=null,
	@GroupingField varchar(10)='CUSTOMERS',
    @OurZoneID 	  INT,
	@SwitchID	  tinyInt = NULL,
	@CodeGroup VARCHAR(20) = null
	
AS
	SET NOCOUNT ON
	
	IF @CustomerID IS NOT NULL SET @GroupingField = 'SUPPLIERS'
	IF @SupplierID IS NOT NULL SET @GroupingField = 'CUSTOMERS'
	
if @CustomerID is null and @SupplierID IS NULL
    select 
		CASE WHEN @GroupingField = 'CUSTOMERS' then CustomerID else TS.SupplierID end as CarrierAccountID,
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls) ELSE SUM(Attempts) END as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) END as ASR,
		
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls  - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
         LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
		 LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
		 LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
	AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR TS.SwitchID = @SwitchID)
		AND TS.CustomerID IS NOT NULL 
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
    Group by 
		(CASE WHEN @GroupingField = 'CUSTOMERS' THEN CustomerID ELSE TS.SupplierID END)
	ORDER by Attempts desc

if @CustomerID is NOT null and @SupplierID IS NULL
    select 
		CASE WHEN @GroupingField = 'CUSTOMERS' then CustomerID else TS.SupplierID end as CarrierAccountID,
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls) ELSE SUM(Attempts) END as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) END as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID	
LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND (TS.CustomerID = @CustomerID) 
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
	AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
    Group by 
		(CASE WHEN @GroupingField = 'CUSTOMERS' THEN CustomerID ELSE TS.SupplierID END)
	ORDER by Attempts desc

if @CustomerID is null and @SupplierID IS NOT NULL
    select 
		CASE WHEN @GroupingField = 'CUSTOMERS' then CustomerID ELSE TS.SupplierID end as CarrierAccountID,
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls) ELSE SUM(Attempts) END as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) END as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier))
LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID	
LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND (TS.SupplierID = @SupplierID) 
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
		AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
		AND TS.CustomerID IS NOT NULL 
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
    Group by 
		(CASE WHEN @GroupingField = 'CUSTOMERS' THEN CustomerID ELSE Ts.SupplierID END)
	ORDER by Attempts desc

if @CustomerID is NOT null and @SupplierID IS NOT NULL
    select 
		CASE WHEN @GroupingField = 'CUSTOMERS' then CustomerID else TS.SupplierID end as CarrierAccountID,
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls) ELSE SUM(Attempts) END as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) END as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier))
LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			 LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
			 LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID		
WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND (TS.SupplierID = @SupplierID) 
		AND (TS.OurZoneID = @OurZoneID)
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR TS.SwitchID = @SwitchID)
    Group by 
		(CASE WHEN @GroupingField = 'CUSTOMERS' THEN CustomerID ELSE TS.SupplierID END)
	ORDER by Attempts desc
GO
/****** Object:  StoredProcedure [dbo].[sp_TrafficStats_ByPeriodsVer1]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_TrafficStats_ByPeriodsVer1]
@PeriodType varchar(50) = 'Days',
@ZoneID INT = NULL ,
@SupplierID VarChar(15) = NULL ,
@CustomerID Varchar(15) = NULL ,
@CodeGroup varchar(10) = NULL ,
@FromDate DATETIME ,
@TillDate DATETIME
AS
BEGIN 


--SELECT @TillDate = getdate()
--SELECT @FromDate = 
--	CASE 
--		WHEN @PeriodType like 'Days' THEN dateadd(dd, -1 * (@PeriodCount - 1), @TillDate)
--		WHEN @PeriodType like 'Weeks' THEN dateadd(wk, -1 * @PeriodCount, @TillDate)
--		WHEN @PeriodType like 'Months' THEN dateadd(mm, -1 * @PeriodCount, @TillDate)
--	END

SELECT @FromDate = dbo.DateOf(@FromDate)

DECLARE @Results TABLE (
    [Day] varchar(20), 
    Attempts numeric(13,5) NULL, 
	DurationsInMinutes numeric(13,5),
    ASR numeric(13,5), 
	ACD numeric(13,5), 
	DeliveredASR numeric(13,5),
	AveragePDD numeric (13,5),
	MaxDuration numeric (13,5),
	LastAttempt datetime ,
    SuccessfulAttempts int
                      )
SET NOCOUNT ON

if @CustomerID is null and @SupplierID IS NULL
	
	INSERT INTO @Results
		(   
			[Day],
			Attempts,
			DurationsInMinutes ,
			ASR,
			ACD,
			DeliveredASR,
			AveragePDD,
			MaxDuration ,
			LastAttempt ,
			SuccessfulAttempts
		 )	 	
		SELECT 
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Date],
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
			Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst)) 
			JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
		WHERE 
			TS.FirstCDRAttempt >= @FromDate AND TS.FirstCDRAttempt <=@TillDate
			AND (@ZoneID IS NULL OR OZ.ZoneID= @ZoneID) 
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		GROUP BY 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)	
			END
--		ORDER BY Min(FirstCDRAttempt) DESC 
   
if @CustomerID is not null and @SupplierID IS NULL
	
	INSERT INTO @Results
		(   
			[Day],
			Attempts,
			DurationsInMinutes ,
			ASR,
			ACD,
			DeliveredASR,
			AveragePDD,
			MaxDuration ,
			LastAttempt ,
			SuccessfulAttempts
		 )	 	
		SELECT 
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Date],
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
			Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer)) 
			JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
		WHERE 
			TS.FirstCDRAttempt >= @FromDate AND TS.FirstCDRAttempt <=@TillDate
     		AND (ts.CustomerID = @CustomerID) 
			AND (@ZoneID IS NULL OR OZ.ZoneID= @ZoneID) 
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		GROUP BY 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)	
			END
		ORDER BY Min(FirstCDRAttempt) DESC 


if @CustomerID is null and @SupplierID IS not NULL
	
	INSERT INTO @Results
		(   
			[Day],
			Attempts,
			DurationsInMinutes ,
			ASR,
			ACD,
			DeliveredASR,
			AveragePDD,
			MaxDuration ,
			LastAttempt ,
			SuccessfulAttempts
		 )	 	
		SELECT 
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Date],
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
			Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier)) 
			JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
		WHERE 
			TS.FirstCDRAttempt >= @FromDate AND TS.FirstCDRAttempt <=@TillDate
     		AND (ts.SupplierID = @SupplierID) 
			AND (@ZoneID IS NULL OR OZ.ZoneID= @ZoneID) 
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		GROUP BY 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)	
			END
		ORDER BY Min(FirstCDRAttempt) DESC 

if @CustomerID  is not null and @SupplierID IS not NULL
	
	INSERT INTO @Results
		(   
			[Day],
			Attempts,
			DurationsInMinutes ,
			ASR,
			ACD,
			DeliveredASR,
			AveragePDD,
			MaxDuration ,
			LastAttempt ,
			SuccessfulAttempts
		 )	 	
		SELECT 
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Date],
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
			Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier)) 
			JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
		WHERE 
			TS.FirstCDRAttempt >= @FromDate AND TS.FirstCDRAttempt <=@TillDate
     		AND (ts.CustomerID = @CustomerID) 
     		AND (ts.SupplierID = @SupplierID) 
			AND (@ZoneID IS NULL OR OZ.ZoneID= @ZoneID) 
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		GROUP BY 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)	
			END
		ORDER BY Min(FirstCDRAttempt) DESC 

	SELECT * FROM @Results ORDER BY [Day] 
END
GO
/****** Object:  View [dbo].[vw_CodeRoutes]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_CodeRoutes]
As
SELECT
	RT.CustomerID,
	RT.Code,	
	OZ.ZoneID AS OurZoneID, OZ.Name AS OurZone, 
	RT.OurServicesFlag,	RT.OurNormalRate, RT.OurOffPeakRate, RT.OurWeekendRate,		
	RT.State AS RouteState,	
	RO.SupplierID,
	RO.SupplierZoneID, SZ.Name AS SupplierZone,
	RO.SupplierServicesFlag, RO.SupplierNormalRate AS SupplierNormalRate, RO.SupplierOffPeakRate, RO.SupplierWeekendRate,
	RO.State AS RouteOptionState
	
FROM
	Zone OZ
	, [Route] RT
	, RouteOption RO, 
	Zone SZ
WHERE 
		OZ.ZoneID=RT.OurZoneID
	AND RT.RouteID=RO.RouteID
	AND RO.SupplierZoneID = SZ.ZoneID
GO
/****** Object:  StoredProcedure [dbo].[EA_TrafficStats_ZoneMonitor]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Stored Procedure

CREATE  PROCEDURE [dbo].[EA_TrafficStats_ZoneMonitor]
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10) = NULL,
	@SupplierID   varchar(10) = NULL,
    @SwitchID	  tinyInt = NULL,   
    @CodeGroup varchar(10) = NULL
WITH RECOMPILE
AS	
BEGIN
	
	Declare @Results TABLE ( AttemptDateTime DATETIME, OurZoneID INT ,Attempts int, FailedAttempts int, DurationsInMinutes numeric(13,5), ASR numeric(13,5), ACD numeric(13,5), DeliveredAttempts INT, DeliveredASR numeric(13,5), AveragePDD numeric(13,5), MaxDuration numeric(13,5), LastAttempt datetime, AttemptPercentage numeric(13,5),DurationPercentage numeric(13,5),SuccesfulAttempts int )
	SET NOCOUNT ON
	-- Customer, No Supplier
	IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
		INSERT INTO @Results (AttemptDateTime ,OurZoneID,Attempts, DurationsInMinutes ,ASR,ACD, deliveredAttempts,DeliveredASR,AveragePDD,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts)
			SELECT 
			    dbo.DateOf(ts.FirstCDRAttempt) AS AttemptDateTime,
				TS.OurZoneID,          
				Sum(Attempts) as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				Sum(deliveredAttempts) AS deliveredAttempts, 
				Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer)) 
			 LEFT JOIN Zone AS OZ WITH (nolock) ON TS.OurZoneID = OZ.ZoneID
				WHERE 
					FirstCDRAttempt BETWEEN  @FromDateTime AND @ToDateTime  
				AND TS.CustomerID = @CustomerID 
				AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
			Group By 
				dbo.DateOf(ts.FirstCDRAttempt), 
				TS.OurZoneID 
	-- No Customer, Supplier
	ELSE IF @CustomerID IS NULL AND @SupplierID IS NOT NULL
		INSERT INTO @Results (AttemptDateTime, OurZoneID, Attempts, DurationsInMinutes ,ASR,ACD,DeliveredAttempts,DeliveredASR,AveragePDD,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts)
			SELECT 
			    min(ts.FirstCDRAttempt),
				TS.OurZoneID,          
				Sum(Attempts) as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				Sum(deliveredAttempts) AS DeliveredAttempts, 
				Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier)) 
			 LEFT JOIN Zone AS OZ WITH (nolock) ON TS.OurZoneID = OZ.ZoneID
				WHERE 
					FirstCDRAttempt BETWEEN  @FromDateTime AND @ToDateTime  
				AND TS.SupplierID = @SupplierID 
				AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
			Group By 
			    --dbo.DateOf(ts.FirstCDRAttempt),
				TS.OurZoneID
			
	

	--Percentage For Attempts-----
	Declare @TotalAttempts bigint
	SELECT  @TotalAttempts = SUM(Attempts) FROM @Results
	Update  @Results SET AttemptPercentage= (Attempts * 100. / @TotalAttempts)

	SELECT * from @Results Order By Attempts DESC ,DurationsInMinutes DESC

END
GO
/****** Object:  StoredProcedure [dbo].[sp_TrafficStats_ZoneMonitorSupplierZoneDetails]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_TrafficStats_ZoneMonitorSupplierZoneDetails]
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10)=null,
	@SupplierID   varchar(10)=null,
    @OurZoneID 	  INT,
	@SwitchID	  tinyInt = NULL,
	@CodeGroup VARCHAR(20) = null
	
AS
	SET NOCOUNT ON
	
	
if @CustomerID is null and @SupplierID IS NULL
    select 
		Ts.SupplierZoneID as SupplierZoneID,
		TS.SupplierID AS SupplierID,
		Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
	   Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
    LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
	WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
		AND TS.CustomerID IS NOT NULL 
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
    Group by 
		TS.SupplierZoneID,
		Ts.SupplierID
	ORDER by Attempts desc

if @CustomerID is NOT null and @SupplierID IS NULL
    select 
		Ts.SupplierZoneID as SupplierZoneID,
		TS.SupplierID AS SupplierID,
		Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
	   Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
    LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
	WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND (TS.CustomerID = @CustomerID) 
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
    Group by 
		 TS.SupplierZoneID,
		 Ts.SupplierID
	ORDER by Attempts desc

if @CustomerID is null and @SupplierID IS NOT NULL
    select 
		Ts.SupplierZoneID as SupplierZoneID,
		TS.SupplierID AS SupplierID,
		Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
	   Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier))
    LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
	WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND (TS.SupplierID = @SupplierID) 
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
		AND TS.CustomerID IS NOT NULL 
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
    Group by 
		TS.SupplierZoneID,
		Ts.SupplierID
	ORDER by Attempts desc

if @CustomerID is NOT null and @SupplierID IS NOT NULL
    select 
		Ts.SupplierZoneID as SupplierZoneID,
		TS.SupplierID AS SupplierID,
		Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
	   Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier))
    LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
	WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND (TS.SupplierID = @SupplierID) 
		AND (TS.OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
    Group by 
		Ts.SupplierZoneID,
		TS.SupplierID
	ORDER by Attempts desc
GO
/****** Object:  StoredProcedure [dbo].[Bp_GetTestingNumbers]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Bp_GetTestingNumbers] 
	 @Top          INT = 100,
	 @Fromdate     datetime,
	 @ToDate       datetime,
	 @SupplierID   varchar(20)=Null,
	 @ZoneID       INT = Null,
	 @Code		   varchar(50) = NULL,
	 @MaxDuration  decimal = 3.0
AS
BEGIN
	SELECT  
		TOP (@TOP)
		Z.[Name] as [Zone],
		cdpn,
		BM.CustomerID as CustomerID,
		Count(*) AS Attempts,
		Min(BM.DurationInSeconds) / 60.0 AS MinDuration,
		Max(BM.DurationInSeconds) / 60.0 AS MaxDuration,
		Sum(DurationInSeconds/60.0) as DurationsInMinutes
	FROM dbo.Billing_CDR_Main BM with(nolock) 
		JOIN Zone Z WITH(nolock) ON BM.OurZoneID = Z.ZoneID
	WHERE
		Attempt BETWEEN @FromDate AND @ToDate
		AND (@SupplierID IS NULL OR BM.SupplierID = @SupplierID)
		AND (@ZoneID IS NULL OR BM.OurZoneID = @ZoneID)					
		AND (@Code IS NULL OR BM.CDPN LIKE @Code + '%')					
		AND (DurationInSeconds > (@MaxDuration * 60.0))
	GROUP BY BM.CDPN, Z.Name, BM.CustomerID
	ORDER BY Count(BM.Attempt) DESC
OPTION (recompile)
END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_HourlyReport]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--
-- Get an Hourly report for traffic (using given parameters)
--
CREATE PROCEDURE [dbo].[SP_TrafficStats_HourlyReport] 
    @FromDateTime	datetime,
	@ToDateTime		datetime,
	@CustomerID		varchar(10) = NULL,
	@SupplierID		varchar(10) = NULL,
    @SwitchID		tinyInt = NULL,
    @OurZoneID 		int = NULL,
    @PortInOut		varchar(50) = NULL,
	@SelectPort		int = NULL,
    @ConnectionType varchar(50) = NULL ,
    @CodeGroup varchar(10) = NULL  
AS
BEGIN	
	SET NOCOUNT ON
if @CustomerID is not null and @SupplierID is null
	SELECT
        datepart(hour,LastCDRAttempt) AS [Hour],
        dateadd(dd,0, datediff(dd,0,LastCDRAttempt)) AS [Date],
        Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	    Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
		MAX(DurationsInSeconds) / 60.0 as MaxDuration,
        Max(LastCDRAttempt) as LastAttempt,
        Sum(SuccessfulAttempts) as SuccessfulAttempt,
        Sum(UtilizationInSeconds)/60.0 as UtilizationInMinutes
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
        LEFT OUTER JOIN
        Zone AS Z ON TS.OurZoneID = Z.ZoneID                                     
   WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND (@CustomerID IS NULL OR TS.CustomerID = @CustomerID) 
		AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID) 
	    AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
	    AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
	    AND (@ConnectionType IS NULL OR (@SelectPort = 0 AND (TS.Port_IN IN (@ConnectionType))) OR (@SelectPort = 1 AND (TS.Port_OUT IN (@ConnectionType))))
	    --AND (@ConnectionType IS NULL OR TS.Port_OUT = @ConnectionType)      
	    --AND Ts.Port_IN like (CASE @SelectPort WHEN 0 THEN @PortInOut ELSE '%%' END)    
        --AND Ts.Port_out like (CASE @SelectPort WHEN 1 THEN @PortInOut ELSE '%%' END) 
        AND (@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
   
	GROUP BY datepart(hour,LastCDRAttempt),dateadd(dd,0, datediff(dd,0,LastCDRAttempt))
	ORDER BY [Hour] ,[Date]

if @CustomerID is null and @SupplierID is not null
	SELECT
        datepart(hour,LastCDRAttempt) AS [Hour],
        dateadd(dd,0, datediff(dd,0,LastCDRAttempt)) AS [Date],
        Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	    Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
		MAX(DurationsInSeconds) / 60.0 as MaxDuration,
        Max(LastCDRAttempt) as LastAttempt,
        Sum(SuccessfulAttempts) AS SuccessfulAttempt,
        Sum(UtilizationInSeconds)/60.0 as UtilizationInMinutes
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier))
        LEFT OUTER JOIN
        Zone AS Z ON TS.OurZoneID = Z.ZoneID                                     
   WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND (@CustomerID IS NULL OR TS.CustomerID = @CustomerID) 
		AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID) 
	    AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
	    AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
	    AND (@ConnectionType IS NULL OR (@SelectPort = 0 AND (TS.Port_IN IN (@ConnectionType))) OR (@SelectPort = 1 AND (TS.Port_OUT IN (@ConnectionType))))
	    --AND (@ConnectionType IS NULL OR TS.Port_OUT = @ConnectionType)      
	    --AND Ts.Port_IN like (CASE @SelectPort WHEN 0 THEN @PortInOut ELSE '%%' END)    
        --AND Ts.Port_out like (CASE @SelectPort WHEN 1 THEN @PortInOut ELSE '%%' END) 
        AND (@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
   
	GROUP BY datepart(hour,LastCDRAttempt),dateadd(dd,0, datediff(dd,0,LastCDRAttempt))
	ORDER BY [Hour] ,[Date]

if @CustomerID is not null and @SupplierID is not null
	SELECT
        datepart(hour,LastCDRAttempt) AS [Hour],
        dateadd(dd,0, datediff(dd,0,LastCDRAttempt)) AS [Date],
        Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	    Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
		MAX(DurationsInSeconds) / 60.0 as MaxDuration,
        Max(LastCDRAttempt) as LastAttempt,
        Sum(SuccessfulAttempts) AS SuccessfulAttempt,
        Sum(UtilizationInSeconds)/60.0 as UtilizationInMinutes
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier))
        LEFT OUTER JOIN
        Zone AS Z ON TS.OurZoneID = Z.ZoneID                                     
   WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND (@CustomerID IS NULL OR TS.CustomerID = @CustomerID) 
		AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID) 
	    AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
	    AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
	    AND (@ConnectionType IS NULL OR (@SelectPort = 0 AND (TS.Port_IN IN (@ConnectionType))) OR (@SelectPort = 1 AND (TS.Port_OUT IN (@ConnectionType))))
	    --AND (@ConnectionType IS NULL OR TS.Port_OUT = @ConnectionType)      
	    --AND Ts.Port_IN like (CASE @SelectPort WHEN 0 THEN @PortInOut ELSE '%%' END)    
        --AND Ts.Port_out like (CASE @SelectPort WHEN 1 THEN @PortInOut ELSE '%%' END) 
        AND (@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
   
	GROUP BY datepart(hour,LastCDRAttempt),dateadd(dd,0, datediff(dd,0,LastCDRAttempt))
	ORDER BY [Hour] ,[Date]

if @CustomerID is null and @SupplierID is null
	SELECT
        datepart(hour,LastCDRAttempt) AS [Hour],
        dateadd(dd,0, datediff(dd,0,LastCDRAttempt)) AS [Date],
        Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	    Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
		MAX(DurationsInSeconds) / 60.0 as MaxDuration,
        Max(LastCDRAttempt) as LastAttempt,
        Sum(SuccessfulAttempts) AS SuccessfulAttempt,
        Sum(UtilizationInSeconds)/60.0 as UtilizationInMinutes
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
        LEFT OUTER JOIN
        Zone AS Z ON TS.OurZoneID = Z.ZoneID                                     
   WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND (@CustomerID IS NULL OR TS.CustomerID = @CustomerID) 
		AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID) 
	    AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
	    AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
	    AND (@ConnectionType IS NULL OR (@SelectPort = 0 AND (TS.Port_IN IN (@ConnectionType))) OR (@SelectPort = 1 AND (TS.Port_OUT IN (@ConnectionType))))
	    --AND (@ConnectionType IS NULL OR TS.Port_OUT = @ConnectionType)      
	    --AND Ts.Port_IN like (CASE @SelectPort WHEN 0 THEN @PortInOut ELSE '%%' END)    
        --AND Ts.Port_out like (CASE @SelectPort WHEN 1 THEN @PortInOut ELSE '%%' END) 
        AND (@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
   
	GROUP BY datepart(hour,LastCDRAttempt),dateadd(dd,0, datediff(dd,0,LastCDRAttempt))
	ORDER BY [Hour] ,[Date]

END
GO
/****** Object:  StoredProcedure [dbo].[bp_CleanBeforeImport]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
/****** Object:  StoredProcedure [dbo].[bp_GetZoneRates]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ======================================================================
-- Author:		Fadi Chamieh, Hassan Kheir Eddine
-- Create date: 2007-09-10
-- Description: Get the Supply Rates for the Zones defined in our System
-- ======================================================================
CREATE PROCEDURE [dbo].[bp_GetZoneRates](
	@CustomerID varchar(10) = NULL,
	@codeFilter varchar(30) = NULL, 
	@ExcludedRate float = 10,
	@zoneNameFilter varchar(50) = NULL, 
	@AllRates char(1) = 'Y',
	@ServicesFlag smallint = 0,
	@ZoneID int = NULL,
	@TopZones int = 10,
	@CurrencyID varchar(3) = NULL,
	@Days int = NULL,
	@Mode varchar(10)= NULL,
	@SupplierIDs VARCHAR(MAX) = NULL
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LastExchangeRate REAL
	Set @SupplierIDs = @SupplierIDs + ','
	/*DECLARE @SupplierIDsTable AS TABLE (ID VARCHAR(100))
	INSERT INTO @SupplierIDsTable SELECT * FROM [dbo].ParseArray(@SupplierIDs,',')*/
	IF @CurrencyID IS NOT NULL 
		SELECT @LastExchangeRate = LastRate FROM Currency WHERE CurrencyID = @CurrencyID
	ELSE 
		SET @LastExchangeRate = 1
		
	--****************************************************************************
	-- If no Zone is selected, select our zones joined to supplier zones
	--****************************************************************************
	SELECT RB.CustomerID, RB.ZoneID, RB.SupplierID  INTO #Blocks 
		    FROM RouteBlock RB WITH(NOLOCK)
			 WHERE RB.IsEffective = 'Y'
			 AND (rb.CustomerID IS NULL OR rb.CustomerID = @CustomerID)
			AND RB.ZoneID IS NOT NULL
	IF @Mode IS NULL
	BEGIN	
		DECLARE @OurRates TABLE(ZoneID int PRIMARY KEY, Rate real, OffPeakRate real, WeekendRate real, ServicesFlag SMALLINT);
		IF @CustomerID IS NULL
			INSERT INTO @OurRates (ZoneID, Rate, OffPeakRate, WeekendRate, ServicesFlag)
				    SELECT  Z.ZoneID, 
						Avg(ZR.NormalRate) * @LastExchangeRate, 
						Avg(ZR.OffPeakRate) * @LastExchangeRate, 
						Avg(ZR.WeekendRate) * @LastExchangeRate,
						@ServicesFlag
					FROM Zone Z with(nolock) LEFT JOIN ZoneRate ZR  with(nolock)
										ON 
											ZR.ZoneID = Z.ZoneID
										AND ZR.NormalRate > 0
										AND ZR.NormalRate < @ExcludedRate
										AND (@ServicesFlag IS NULL OR ZR.ServicesFlag & @ServicesFlag = @ServicesFlag)	
					WHERE 1=1
						AND	Z.SupplierID = 'SYS'
						AND (@zoneNameFilter IS NULL OR Z.Name LIKE @zoneNameFilter)
						AND (@codeFilter IS NULL OR ZR.ZoneID IN (SELECT C.ZoneID FROM Code C WHERE C.Code LIKE @codeFilter))
					GROUP BY Z.ZoneID;
		ELSE
			BEGIN 
				INSERT INTO @OurRates (ZoneID, Rate, OffPeakRate, WeekendRate, ServicesFlag)
					SELECT  ZR.ZoneID, 
							ZR.NormalRate * @LastExchangeRate, 
							ZR.OffPeakRate * @LastExchangeRate, 
							ZR.WeekendRate * @LastExchangeRate, 
							ZR.ServicesFlag
						FROM ZoneRate ZR
							WHERE
								ZR.CustomerID = @CustomerID 
							AND ZR.NormalRate < @ExcludedRate 
							AND (@zoneNameFilter IS NULL OR ZR.ZoneID IN (SELECT ZoneID FROM Zone WHERE Name LIKE @zoneNameFilter)) 
							AND (@codeFilter IS NULL OR ZR.ZoneID IN (SELECT ZoneID FROM Code WHERE Code LIKE @codeFilter))
			END;
    WITH PrimaryResult AS (
		SELECT
			OZ.ZoneID as OurZoneID, OZ.Name AS OurZone,
			R.Rate as OurNormalRate, R.OffPeakRate as OurOffPeakRate, R.WeekendRate AS OurWeekendRate, R.ServicesFlag AS OurServicesFlag,
			SZ.SupplierID, SZ.ZoneID AS SupplierZoneID, SZ.Name AS SupplierZone,
			SR.NormalRate * @LastExchangeRate AS SupplierNormalRate, 
			SR.OffPeakRate * @LastExchangeRate AS SupplierOffPeakRate, 
			SR.WeekendRate * @LastExchangeRate AS SupplierWeekendRate, 
			SR.ServicesFlag AS SupplierServicesFlag
			--Ra.EndEffectiveDate AS EndEffectiveDate
		FROM
			Zone OZ with(nolock), @OurRates R, ZoneMatch ZM with(nolock), ZoneRate SR with(nolock), Zone SZ with(nolock)
		WHERE 
				OZ.ZoneID = R.ZoneID
			AND ZM.OurZoneID = OZ.ZoneID
			AND ZM.SupplierZoneID = SR.ZoneID
			AND SR.ServicesFlag & R.ServicesFlag = R.ServicesFlag
			AND (@ServicesFlag IS NULL OR SR.ServicesFlag & @ServicesFlag = @ServicesFlag)
			AND SR.ZoneID = SZ.ZoneID
			AND SR.SupplierID <> 'SYS'
			AND (@CustomerID IS  NULL OR SZ.SupplierID <> @CustomerID)
			AND (@AllRates = 'Y' OR (SR.NormalRate <= R.Rate))
			AND SR.ZoneID NOT IN (SELECT RB.ZoneID FROM #Blocks RB)
		--ORDER BY OurZone, SupplierNormalRate,OZ.EndEffectiveDate ASC
    )
    SELECT 
    P.OurZoneID as OurZoneID, P.OurZone AS OurZone,
			P.OurNormalRate as OurNormalRate, P.OurOffPeakRate as OurOffPeakRate, P.OurWeekendRate AS OurWeekendRate, P.OurServicesFlag AS OurServicesFlag,
			P.SupplierID, P.SupplierZoneID AS SupplierZoneID, P.SupplierZone AS SupplierZone,
			P.SupplierNormalRate AS SupplierNormalRate, 
			P.SupplierOffPeakRate AS SupplierOffPeakRate, 
			P.SupplierWeekendRate AS SupplierWeekendRate, 
			P.SupplierServicesFlag AS SupplierServicesFlag,
    Ra.endeffectivedate
     FROM primaryresult P with(nolock) ,rate ra with(nolock),PriceList pl with(nolock)
    WHERE 
          Ra.ZoneID = P.SupplierZoneID 
			AND Ra.IsEffective = 'Y'
			AND Ra.PriceListID  = pl.PriceListID 
			AND pl.SupplierID = P.SupplierID
			AND (@SupplierIDs Is Null Or @SupplierIDs like '%,' + P.SupplierID + ',%' )
    ORDER BY P.OurZone, P.SupplierNormalRate ASC
	END

	ELSE
				
	--****************************************************************************
	-- If a particular Zone is selected, select only matching supplier zones
	--****************************************************************************
	BEGIN		
	 
	 DECLARE @FromDate datetime
     DECLARE @TillDate datetime

     SELECT @FromDate = CAST(CONVERT(varchar(10),DATEADD(dd,-@Days,GETDATE()),121) AS DATETIME)
     SELECT @TillDate = GETDATE()		
     
     SELECT 
            ZM.OurZoneID, 
			SZ.SupplierID, SZ.ZoneID AS SupplierZoneID, 
			SZ.Name AS SupplierZone, 
			ZR.NormalRate * @LastExchangeRate AS SupplierNormalRate,
			ZR.OffPeakRate * @LastExchangeRate  AS SupplierOffPeakRate,
			ZR.WeekendRate * @LastExchangeRate AS SupplierWeekendRate,
			ZR.ServicesFlag AS SupplierServicesFlag,
			null,
			TS.DurationsInMinutes,
			TS.ASR,
			TS.ACD
     FROM 
		Zone SZ WITH(NOLOCK) 
			INNER JOIN ZoneMatch ZM WITH(NOLOCK) 
							ON ZM.SupplierZoneID = SZ.ZoneID	
			INNER JOIN ZoneRate ZR WITH(NOLOCK) 
							ON ZR.ZoneID = ZM.SupplierZoneID AND (@CustomerID IS NOT NULL OR SZ.SupplierID <> @CustomerID )
			LEFT JOIN GetSupplierZoneStats(@FromDate, @TillDate,NULL) AS TS 
							ON TS.SupplierID = ZR.SupplierID AND TS.OurZoneID = ZM.OurZoneID
		WHERE	
		     ZR.SupplierID <> 'SYS'
			AND ZR.ServicesFlag & @ServicesFlag = @ServicesFlag				
		ORDER BY SupplierNormalRate ASC
		OPTION (RECOMPILE)
	END
END
GO
/****** Object:  StoredProcedure [dbo].[bp_IdentifyErrors]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_IdentifyErrors] 
	@CodeZoneErrors char(1) = 'Y',
	@RateZoneErrors char(1) = 'Y',
	@CodeCodeErrors char(1) = 'Y',
	@RateRateErrors char(1) = 'Y'
AS
BEGIN
SET NOCOUNT ON;

Declare @MaxDate datetime
Set @MaxDate = '2020-01-01'

-- Code / Zone Errors
IF @CodeZoneErrors = 'Y'
      SELECT   
         Z.CodeGroup, Z.Name, Z.SupplierID AS SupplierID,'' AS Supplier, Z.BeginEffectiveDate AS BeginEffectiveDate#Z,
         Z.EndEffectiveDate AS EndEffectiveDate#Z,Z.IsEffective AS IsEffective#Z, C.Code, 
         C.BeginEffectiveDate AS BeginEffectiveDate#C, C.EndEffectiveDate AS EndEffectiveDate#C, C.IsEffective AS IsEffective#C
      FROM        
         Zone AS Z INNER JOIN
         Code AS C ON Z.ZoneID = C.ZoneID 
      WHERE     
         (NOT (C.BeginEffectiveDate BETWEEN Z.BeginEffectiveDate AND ISNULL(Z.EndEffectiveDate, @MaxDate)))
          OR
         (NOT (ISNULL(C.EndEffectiveDate, @MaxDate) BETWEEN Z.BeginEffectiveDate AND ISNULL(Z.EndEffectiveDate, @MaxDate)))

-- Rate / Zone Errors
IF @RateZoneErrors = 'Y'
      SELECT     
          R.PriceListID, R.Rate, R.OffPeakRate, R.WeekendRate, R.Change, 
          R.BeginEffectiveDate AS BeginEffectiveDate#R, 
          R.EndEffectiveDate AS EndEffectiveDate#R, 
          R.IsEffective AS IsEffective#R, Z.CodeGroup, Z.Name, Z.SupplierID AS SupplierID,'' AS Supplier,
          Z.IsMobile, Z.IsProper, Z.IsSold, Z.BeginEffectiveDate AS BeginEffectiveDate#Z, 
          Z.EndEffectiveDate AS EndEffectiveDate#Z, Z.IsEffective AS IsEffective#Z
       FROM         
          Rate AS R INNER JOIN
          Zone AS Z ON R.ZoneID = Z.ZoneID 
	    WHERE
		-- Test Begin 
		   NOT (R.BeginEffectiveDate BETWEEN Z.BeginEffectiveDate AND ISNULL(Z.EndEffectiveDate, @MaxDate))
		   OR
		-- Test End
		   NOT (ISNULL(R.EndEffectiveDate,@MaxDate) BETWEEN Z.BeginEffectiveDate AND ISNULL(Z.EndEffectiveDate, @MaxDate))

-- Code / Code Errors
IF @CodeCodeErrors = 'Y'
     SELECT     
       C1.Code, Z1.SupplierID AS SupplierID,'' AS Supplier, C1.ID, Z1.Name, 
       C1.BeginEffectiveDate, C1.EndEffectiveDate, C2.ID, Z2.Name , 
       C2.BeginEffectiveDate , C2.EndEffectiveDate 
     FROM        
        Code AS C1 INNER JOIN
        Zone AS Z1 ON C1.ZoneID = Z1.ZoneID INNER JOIN
        Zone AS Z2 ON Z1.SupplierID = Z2.SupplierID INNER JOIN
        Code AS C2 ON Z2.ZoneID = C2.ZoneID AND C1.Code = C2.Code 
	WHERE 
			C1.ID <> C2.ID
		AND C2.BeginEffectiveDate >= C1.BeginEffectiveDate 
		AND C2.BeginEffectiveDate < ISNULL(C1.EndEffectiveDate, @MaxDate)
	ORDER BY Z1.SupplierID, C1.Code, C1.BeginEffectiveDate


-- Rate / Rate Errors
IF @RateRateErrors = 'Y'
       SELECT     
              P1.SupplierID AS SupplierID, '' AS Supplier, P1.CustomerID AS CustomerID,'' AS Customer, R1.ZoneID, Z.Name,
              R1.RateID, R1.Rate, R1.BeginEffectiveDate,R1.EndEffectiveDate, R2.RateID ,
              R2.Rate , R2.BeginEffectiveDate , R2.EndEffectiveDate 
           FROM         
              Rate AS R1 INNER JOIN
              PriceList AS P1 ON P1.PriceListID = R1.PriceListID INNER JOIN
              PriceList AS P2 ON P2.SupplierID = P1.SupplierID AND P2.CustomerID = P1.CustomerID AND P1.PriceListID <> P2.PriceListID INNER JOIN
              Rate AS R2 ON R1.ZoneID = R2.ZoneID AND R2.PriceListID = P2.PriceListID INNER JOIN
              Zone AS Z ON R1.ZoneID = Z.ZoneID 
              WHERE 1=1
		      AND R2.BeginEffectiveDate >= R1.BeginEffectiveDate 
		      AND R2.BeginEffectiveDate < ISNULL(R1.EndEffectiveDate, @MaxDate)
		      AND R2.BeginEffectiveDate <> R2.EndEffectiveDate
		      AND R1.Rate <> R2.Rate
	       ORDER BY P1.SupplierID, P1.CustomerID, Z.Name, R1.BeginEffectiveDate

END
GO
/****** Object:  StoredProcedure [dbo].[bp_IsRouteBuildRunning]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Fadi Chamieh
-- Create date: 2007-12-17
-- Description:	Check if routing is running 
-- =============================================
CREATE PROCEDURE [dbo].[bp_IsRouteBuildRunning](@IsRunning char(1) output)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Message varchar(500)
	SELECT @IsRunning = 'Y' FROM SystemMessage WHERE MessageID = 'BuildRoutes: Status' AND [Message] IS NOT NULL
	SET @IsRunning = ISNULL(@IsRunning, 'N')
	RETURN 
END
GO
/****** Object:  StoredProcedure [dbo].[bp_SetSystemMessage]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_SetSystemMessage](@msgID varchar(50), @message varchar(1024))
	
AS 
		IF NOT EXISTS (SELECT MessageID FROM SystemMessage WHERE MessageID LIKE @msgID)
			INSERT INTO SystemMessage
			(
				MessageID,
				Description,
				Message,
				Updated
			)
			VALUES
			(
				@msgID,
				@msgID,
				@message,
				getdate()
			)
		ELSE
			UPDATE SystemMessage SET [Message]=@message, Updated=GETDATE() WHERE MessageID=@msgID		
	RETURN
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_RateChange]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TrafficStats_RateChange]
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10) = NULL,
	@SupplierID   varchar(10) = NULL,
	@OurZoneID 	  INT,
	@ServicesFlag smallint = NULL	
	AS
BEGIN
	
	DECLARE  @AllDays TABLE (DayStart DATETIME,[DayEnd] DATETIME)
    
    DECLARE @date SMALLDATETIME
    SET @date= dbo.DateOf(@FromDateTime)
     
    WHILE(@date <= @ToDateTime)
    BEGIN 
			INSERT INTO @AllDays ( DayStart,[DayEnd]) VALUES (@date,DATEADD(dd,1,@date))
			SET @date = DATEADD(dd,1,@date)
    END 
    
    DECLARE @Results TABLE (
    		[Day] varchar(10), 
    		Rate numeric(13,5),RateUnScaled numeric(13,5), RateScale numeric(13,5), RateScaleName varchar(10), 
    		Attempts numeric(13,5) , AttemptScale numeric(13,5), AttemptScaleName varchar(10), 
    		DurationsInMinutes numeric(13,5), DurationScale numeric(13,5), DurationScaleName varchar(10), 
    		ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5),SuccessfulAttempts numeric(13,5) )
    
    SET NOCOUNT ON
    INSERT INTO @Results
    (
        [Day],
        Rate,
        RateUnScaled,
        Attempts,
        DurationsInMinutes,
        ASR,
        ACD,
        DeliveredASR,
        SuccessfulAttempts
    )	 	
    SELECT CONVERT(VARCHAR(10), A.DayStart,121),
           R.Rate,
           R.Rate,
           Sum(Attempts) AS Attempts,
           Sum(DurationsInSeconds)/60.0 AS DurationsInMinutes,
           Sum(SuccessfulAttempts) * 100.0 / Sum(Attempts) AS ASR,
           CASE 
                WHEN Sum(SuccessfulAttempts) > 0 THEN Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts)
                ELSE 0
           END AS ACD,
           Sum(deliveredAttempts) * 100.0 / SUM(Attempts) AS DeliveredASR,
           Sum(SuccessfulAttempts) AS SuccessfulAttempts
    FROM
		 @AllDays A
		INNER JOIN  PriceList P ON P.CustomerID = @CustomerID 
		INNER JOIN  Rate R  ON P.PriceListID = R.PriceListID 
							   AND  R.ZoneID = @OurZoneID 
						       AND (R.BeginEffectiveDate <= A.DayStart AND (R.EndEffectiveDate IS NULL OR  R.EndEffectiveDate > = A.DayStart))
        LEFT JOIN TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Zone),INDEX(IX_TrafficStats_Supplier)) 
				ON TS.FirstCDRAttempt >= A.DayStart 
				   AND TS.LastCDRAttempt < A.DayEnd
				   AND  (TS.OurZoneID = @OurZoneID)
				   AND  (TS.CustomerID = @CustomerID)
				   AND  (@SupplierID IS NULL OR  TS.SupplierID = @SupplierID)	 
     --AND (@ServicesFlag IS NULL OR SZ.ServicesFlag IN (SELECT M.ServiceFlag FROM ServiceFlagMask M WHERE M.Mask = @ServicesFlag))
	
		  
    GROUP BY CONVERT(VARCHAR(10), A.DayStart,121), R.Rate
    OPTION (Recompile)
    
    DECLARE @TotalAttempts bigint
    SELECT  @TotalAttempts = MAX(Attempts) FROM @Results
    DECLARE @AttemptScaleName varchar(10)
    DECLARE @AttemptScale numeric(13,5)
	EXEC bp_GetScale @TotalAttempts, @AttemptScale output, @AttemptScaleName output
	
    DECLARE @MaxDuration numeric(13,5) 
    SELECT  @MaxDuration = Max(DurationsInMinutes) FROM @Results
    DECLARE @DurationScaleName varchar(10)
    DECLARE @DurationScale numeric(13,5)
	EXEC bp_GetScale @MaxDuration, @DurationScale output, @DurationScaleName output

	Declare @MaxRate numeric(13,5)
	SELECT  @MaxRate = Max(Rate) FROM @Results
    DECLARE @RateScaleName varchar(10)
    DECLARE @RateScale numeric(13,5)
	EXEC bp_GetScale @MaxRate, @RateScale output, @RateScaleName output
    
    UPDATE @Results
		SET
			Attempts = (Attempts / @AttemptScale),
			AttemptScaleName = @AttemptScaleName,
			AttemptScale = @AttemptScale,
			DurationsInMinutes = (DurationsInMinutes /@DurationScale),
			DurationScaleName = @DurationScaleName,
			DurationScale = @DurationScale,
			Rate = (Rate /@RateScale),
			RateScaleName = @RateScaleName,
			RateScale = @RateScale
    
    SELECT * FROM @Results ORDER BY [Day]
    
END
GO
/****** Object:  StoredProcedure [dbo].[Sp_TrafficStats_ByOriginatingZone]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Sp_TrafficStats_ByOriginatingZone]
    @FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10) = NULL,
	@SupplierID   varchar(10)= NULL,
	@SwitchID	  tinyInt = NULL,
	@WhereInCondition VarChar(1024)
	
AS
	SET NOCOUNT ON
	
	Declare @Results TABLE (OriginatingZoneID int, Attempts bigint, SuccessfulAttempts bigint, DeliveredAttempts bigint, DurationsInMinutes numeric(13,5), ASR numeric(13,5), ACD numeric(13,5),DeliveredASR numeric(13,5), AveragePDD numeric(13,5), MaxDuration numeric(13,5), LastAttempt datetime, Percentage numeric(13,5))
	Declare @TotalAttempts bigint
	-- No Customer, No Supplier
	IF @CustomerID IS NULL AND @SupplierID IS NULL
		BEGIN
		INSERT INTO @Results   
				(OriginatingZoneID , Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		SELECT  TS.OriginatingZoneID AS ZoneID,
				 Sum(cast(Attempts AS bigint)) as Attempts,
				 Sum(cast(SuccessfulAttempts AS bigint)) as SuccessfulAttempts,
				 Sum(cast(deliveredAttempts AS bigint)) as deliveredAttempts,
				 Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				 Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
				 case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				 Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR,
				 Avg(PDDinSeconds) as AveragePDD, 
				 Max (MaxDurationInSeconds)/60. as MaxDuration,
				 Max(LastCDRAttempt) as LastAttempt,0
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
			WHERE 
					(@WhereInCondition IS NULL OR TS.OriginatingZoneID IN (SELECT * FROM  dbo.ParseArray(@WhereInCondition,',' ) ))
				AND (FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime)
				AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
			Group By TS.OriginatingZoneID
			ORDER by Sum(cast(Attempts AS bigint)) DESC

		
		SELECT @TotalAttempts = SUM(Attempts) FROM @Results
		Update @Results SET Percentage = (Attempts * 100. / @TotalAttempts)
			
		INSERT INTO @Results
			(OriginatingZoneID, Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		SELECT 0, Sum(Attempts), Sum(SuccessfulAttempts), Sum(DeliveredAttempts), Sum(DurationsInMinutes), 0, 0, 0, 0, Max(MaxDuration), Max(LastAttempt), 100
		FROM @Results
		UPDATE @Results SET
				ASR = SuccessfulAttempts / Attempts,
				ACD = case when SuccessfulAttempts > 0 then DurationsInMinutes / (60.0 * SuccessfulAttempts) ELSE 0 end,
				DeliveredASR = DeliveredAttempts * 100.0 / Attempts
			
		SELECT * from @Results  Order By Attempts DESC   
		END 
	-- Customer, No Supplier
	ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
		BEGIN
		INSERT INTO @Results   
			(OriginatingZoneID , Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		SELECT  TS.OriginatingZoneID AS ZoneID,
			Sum(cast(Attempts AS bigint)) as Attempts,
			Sum(cast(SuccessfulAttempts AS bigint)) as SuccessfulAttempts,
			Sum(cast(deliveredAttempts AS bigint)) as deliveredAttempts,
			Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
			Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR,
			Avg(PDDinSeconds) as AveragePDD, 
			Max (MaxDurationInSeconds)/60. as MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,0
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
			WHERE 
				(@WhereInCondition IS NULL OR TS.OriginatingZoneID IN (SELECT * FROM  dbo.ParseArray(@WhereInCondition,',' ) ))
			AND (FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime)
			AND (TS.CustomerID = @CustomerID) 
			AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
			Group By TS.OriginatingZoneID
			ORDER by Sum(cast(Attempts AS bigint)) DESC

		SELECT @TotalAttempts = SUM(Attempts) FROM @Results
		Update @Results SET Percentage = (Attempts * 100. / @TotalAttempts)
	
		INSERT INTO @Results
			(OriginatingZoneID, Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		SELECT 0, Sum(Attempts), Sum(SuccessfulAttempts), Sum(DeliveredAttempts), Sum(DurationsInMinutes), 0, 0, 0, 0, Max(MaxDuration), Max(LastAttempt), 100
		FROM @Results
		UPDATE @Results SET
			ASR = SuccessfulAttempts / Attempts,
			ACD = case when SuccessfulAttempts > 0 then DurationsInMinutes / (60.0 * SuccessfulAttempts) ELSE 0 end,
			DeliveredASR = DeliveredAttempts * 100.0 / Attempts
	
		SELECT * from @Results  Order By Attempts DESC   
		END
	-- NO Customer, Supplier
	ELSE IF @CustomerID IS NULL AND @SupplierID IS NOT NULL
		BEGIN
		INSERT INTO @Results   
			(OriginatingZoneID , Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		SELECT  TS.OriginatingZoneID AS ZoneID,
			 Sum(cast(Attempts AS bigint)) as Attempts,
			 Sum(cast(SuccessfulAttempts AS bigint)) as SuccessfulAttempts,
			 Sum(cast(deliveredAttempts AS bigint)) as deliveredAttempts,
			 Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
			 Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
			 case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			 Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR,
			 Avg(PDDinSeconds) as AveragePDD, 
			 Max (MaxDurationInSeconds)/60. as MaxDuration,
			 Max(LastCDRAttempt) as LastAttempt,0
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier))
		WHERE 
			(@WhereInCondition IS NULL OR TS.OriginatingZoneID IN (SELECT * FROM  dbo.ParseArray(@WhereInCondition,',' ) ))
			AND (FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime)
			AND (SupplierID = @SupplierID) 
			AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
		Group By TS.OriginatingZoneID
		ORDER by Sum(cast(Attempts AS bigint)) DESC

		SELECT @TotalAttempts = SUM(Attempts) FROM @Results
		Update @Results SET Percentage = (Attempts * 100. / @TotalAttempts)
	
		INSERT INTO @Results
			(OriginatingZoneID, Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		SELECT 0, Sum(Attempts), Sum(SuccessfulAttempts), Sum(DeliveredAttempts), Sum(DurationsInMinutes), 0, 0, 0, 0, Max(MaxDuration), Max(LastAttempt), 100
		FROM @Results
		UPDATE @Results SET
			ASR = SuccessfulAttempts / Attempts,
			ACD = case when SuccessfulAttempts > 0 then DurationsInMinutes / (60.0 * SuccessfulAttempts) ELSE 0 end,
			DeliveredASR = DeliveredAttempts * 100.0 / Attempts
	
		SELECT * from @Results  Order By Attempts DESC   
		END
	-- Cutomer, Supplier
	ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL
		BEGIN
		INSERT INTO @Results   
			(OriginatingZoneID , Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		SELECT  TS.OriginatingZoneID AS ZoneID,
			 Sum(cast(Attempts AS bigint)) as Attempts,
			 Sum(cast(SuccessfulAttempts AS bigint)) as SuccessfulAttempts,
			 Sum(cast(deliveredAttempts AS bigint)) as deliveredAttempts,
			 Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
			 Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
			 case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			 Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR,
			 Avg(PDDinSeconds) as AveragePDD, 
			 Max (MaxDurationInSeconds)/60. as MaxDuration,
			 Max(LastCDRAttempt) as LastAttempt,0
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier))
		WHERE 
			(@WhereInCondition IS NULL OR TS.OriginatingZoneID IN (SELECT * FROM  dbo.ParseArray(@WhereInCondition,',' ) ))
			AND (FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime)
			AND (TS.CustomerID = @CustomerID) 
			AND (SupplierID = @SupplierID) 
			AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
		Group By TS.OriginatingZoneID
		ORDER by Sum(cast(Attempts AS bigint)) DESC
	
		SELECT @TotalAttempts = SUM(Attempts) FROM @Results
		Update @Results SET Percentage = (Attempts * 100. / @TotalAttempts)
	
		INSERT INTO @Results
			(OriginatingZoneID, Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt, Percentage) 
		SELECT 0, Sum(Attempts), Sum(SuccessfulAttempts), Sum(DeliveredAttempts), Sum(DurationsInMinutes), 0, 0, 0, 0, Max(MaxDuration), Max(LastAttempt), 100
		FROM @Results
		UPDATE @Results SET
			ASR = SuccessfulAttempts / Attempts,
			ACD = case when SuccessfulAttempts > 0 then DurationsInMinutes / (60.0 * SuccessfulAttempts) ELSE 0 end,
			DeliveredASR = DeliveredAttempts * 100.0 / Attempts
	
		SELECT * from @Results  Order By Attempts DESC		
	END
GO
/****** Object:  StoredProcedure [dbo].[bp_GetHourlyChangeFlags]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_GetHourlyChangeFlags]
(
	@When datetime = getdate,
	@RateChanges char(1) output,
	@SpecialRequestChanges char(1) output,
	@RouteBlockChanges char(1) output	
)
AS
BEGIN
	SET NOCOUNT ON
	
	-- Declare and Initialize Hour Range
	DECLARE @from datetime
	DECLARE @till datetime	
	SELECT @from = cast((CONVERT(varchar(14), @When, 121)+'00:00.000') AS datetime)
	SELECT @till = dateadd(hour, 1, @from)
	
	SET @RateChanges = 'N'
	SET @SpecialRequestChanges = 'N'
	SET @RouteBlockChanges = 'N'
	
	-- Rates?
	IF EXISTS(
			SELECT * FROM Rate R 
				WHERE 
					(R.BeginEffectiveDate >= @from AND R.BeginEffectiveDate < @till)
					OR
					(R.EndEffectiveDate >= @from AND R.EndEffectiveDate < @till)
		)
	BEGIN
		SELECT @RateChanges = 'Y'
	END

	-- Special Requests
	IF EXISTS(
			SELECT * FROM SpecialRequest SR 
				WHERE 
					(SR.BeginEffectiveDate >= @from AND SR.BeginEffectiveDate < @till)
					OR
					(SR.EndEffectiveDate >= @from AND SR.EndEffectiveDate < @till)
		)
	BEGIN
		SELECT @SpecialRequestChanges = 'Y'
	END
	
	-- Route Blocks
	IF EXISTS(
			SELECT * FROM RouteBlock RB 
				WHERE 
					(RB.BeginEffectiveDate >= @from AND RB.BeginEffectiveDate < @till)
					OR
					(RB.EndEffectiveDate >= @from AND RB.EndEffectiveDate < @till)
		)
	BEGIN
		SELECT @RouteBlockChanges = 'Y'
	END
		
END
GO
/****** Object:  UserDefinedFunction [dbo].[GetExchangeRate]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  FUNCTION [dbo].[GetExchangeRate](@CurrencyID varchar(3),@Date Datetime)
RETURNS float
AS
BEGIN 
	DECLARE @rate float
	SELECT @rate = (
	SELECT TOP 1 cer.Rate FROM CurrencyExchangeRate cer 
	WHERE cer.CurrencyID = @CurrencyID AND cer.ExchangeDate <= @Date 
	ORDER BY cer.ExchangeDate DESC)
	
	RETURN isnull(@rate,1) 
END
GO
/****** Object:  UserDefinedFunction [dbo].[GetPreviousRateValue]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ====================================================================================================================
-- Author: Fadi Chamieh
-- Create date: 2008-03-26
-- Modification Date: 2009-08-20 (Changed return type to decimal from Money, reason: money is 4 decimal precision 
-- Description:	Gets the previous rate value for a given rate
-- ====================================================================================================================
CREATE FUNCTION [dbo].[GetPreviousRateValue] 
(
	-- Add the parameters for the function here
	@RateID bigint
)
RETURNS DECIMAL(9,5)
AS
BEGIN
	DECLARE @RateValue DECIMAL(9,5)
	SET @RateValue = (SELECT TOP 1 O.Rate FROM Rate R WITH(NOLOCK), Rate O WITH(NOLOCK), PriceList P WITH(NOLOCK), PriceList OP WITH(NOLOCK)
						WHERE 
							R.RateID = @RateID
							AND O.ZoneID = R.ZoneID
							AND P.PriceListID = R.PriceListID
							AND OP.PriceListID = O.PriceListID
							AND P.SupplierID = OP.SupplierID
							AND P.CustomerID = OP.CustomerID
						    AND (O.BeginEffectiveDate = R.BeginEffectiveDate OR (O.BeginEffectiveDate < R.BeginEffectiveDate AND O.BeginEffectiveDate < O.EndEffectiveDate)) 
						    AND O.RateID <> R.RateID
						ORDER BY O.BeginEffectiveDate DESC, O.RateID DESC)

	-- Declare the return variable here
	RETURN @RateValue 
END
GO
/****** Object:  StoredProcedure [dbo].[bp_FixErroneousEffectiveRates]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_FixErroneousEffectiveRates]
AS
UPDATE Rate SET EndEffectiveDate = 
	(
		SELECT TOP 1 RN.BeginEffectiveDate FROM Rate RN, PriceList PN, PriceList PO 
			WHERE 
				PO.PricelistID = Rate.PriceListID
			AND PO.SupplierID = PN.SupplierID AND PO.CustomerID = PN.CustomerID 
			AND PN.PriceListID = RN.PriceListID
			AND RN.ZoneID = Rate.ZoneID 
			AND (
				(RN.BeginEffectiveDate > Rate.BeginEffectiveDate AND RN.RateID <> Rate.RateID) 
				OR 
				(RN.BeginEffectiveDate = Rate.BeginEffectiveDate AND RN.RateID > Rate.RateID)
				) 				
			AND RN.EndEffectiveDate IS NULL
			ORDER BY RN.BeginEffectiveDate ASC
	)
WHERE 
		Rate.EndEffectiveDate IS NULL
	AND EXISTS 
		(
			SELECT * FROM Rate RN, PriceList PN, PriceList PO 
				WHERE 
					PO.PricelistID = Rate.PriceListID
				AND PO.SupplierID = PN.SupplierID AND PO.CustomerID = PN.CustomerID 
				AND PN.PriceListID = RN.PriceListID
				AND RN.ZoneID = Rate.ZoneID 
				AND (
					(RN.BeginEffectiveDate > Rate.BeginEffectiveDate AND RN.RateID <> Rate.RateID) 
					OR 
					(RN.BeginEffectiveDate = Rate.BeginEffectiveDate AND RN.RateID > Rate.RateID)
					) 				
				AND RN.EndEffectiveDate IS NULL 
		)
GO
/****** Object:  StoredProcedure [dbo].[bp_TrackChanges]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_TrackChanges](
 @ZoneID int ,
 @CodeID int=   NULL,
 @ChangesFlag smallint 
 )
AS 
BEGIN 
	
	
  TRUNCATE TABLE Changes 

INSERT  INTO Changes
(
	CustomerID,
	ZoneID,
	CodeID,
	Changes     
)
SELECT Distinct 
           P.CustomerID ,R.ZoneID,C.ID, @ChangesFlag
FROM PriceList P    
          JOIN Rate R ON R.PriceListID = P.PriceListID
          JOIN Code C ON C.ZoneID = R.ZoneID
WHERE R.IsEffective = 'Y' 
          AND P.SupplierID = 'SYS'
          AND (@CodeID IS NULL OR C.ID= @CodeID)
          AND R.ZoneID = @ZoneID
ORDER BY P.CustomerID
    
    
END
GO
/****** Object:  StoredProcedure [dbo].[SP_FixNewRateFlag]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_FixNewRateFlag]
AS
BEGIN
UPDATE Rate
SET	Change = 2
FROM Rate 
JOIN PriceList pl ON pl.PriceListID = Rate.PriceListID
WHERE 
NOT EXISTS 
(
	SELECT * FROM Rate pr JOIN PriceList prl ON prl.PriceListID = pr.PriceListID
		WHERE pr.ZoneID = Rate.ZoneID
			AND prl.SupplierID = pl.SupplierID
			AND prl.CustomerID = pl.CustomerID
			AND pr.BeginEffectiveDate < Rate.BeginEffectiveDate
			AND pr.EndEffectiveDate = Rate.BeginEffectiveDate
)
END
GO
/****** Object:  StoredProcedure [dbo].[bp_ErroneousEffectiveRates]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_ErroneousEffectiveRates]
AS
	SELECT ZoneID, SupplierID, CustomerID, Min(R.BeginEffectiveDate) FirstRate, Max(R.BeginEffectiveDate) LastRate, Count(*) FROM Rate R, PriceList P 
		WHERE R.IsEffective = 'y'
			AND R.PriceListID = P.PriceListID
	GROUP BY ZoneID, SupplierID, CustomerID
	HAVING Count(*) > 1
	ORDER BY Count(*)
GO
/****** Object:  StoredProcedure [dbo].[bp_GetCustomerRates]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =========================================================
-- Author:		Fadi Chamieh
-- Create date: 20/09/2007
-- Description:	Get the Effective Zone Rates for a Customer
-- =========================================================
CREATE PROCEDURE [dbo].[bp_GetCustomerRates](@CustomerID varchar(10))

AS
BEGIN
	SET NOCOUNT ON;

	-- Select Custom Rates
    SELECT R.* FROM Rate R, PriceList P 
		WHERE 
				P.CustomerID = @CustomerID
			AND	R.IsEffective = 'Y'
			AND P.PriceListID=R.PriceListID
	ORDER BY ZoneID	
END
GO
/****** Object:  StoredProcedure [dbo].[bp_PreviewCustomerInvoice]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[bp_PreviewCustomerInvoice]
(
	@CustomerID varchar(5),
	@GMTShifttime int,
	@TimeZoneInfo VARCHAR(MAX),
	@FromDate Datetime,
	@ToDate Datetime,
	@IssueDate Datetime,
	@DueDate Datetime
)
AS
SET NOCOUNT ON 

-- Creating the Invoice 	
DECLARE @Amount float 
DECLARE @Duration NUMERIC(19,6) 
DECLARE @NumberOfCalls INT 
DECLARE @CurrencyID varchar(3) 

--- dummy data to create table schemas
SELECT TOP 1 * INTO #Billing_Invoice  FROM Billing_Invoice bi WHERE 1=0
SELECT TOP 1 * INTO #Billing_Invoice_Details  FROM Billing_Invoice_Details bi  WHERE 1=0
-----------------------------------------

DECLARE @From SMALLDATETIME
SET @From = @FromDate

DECLARE @To SMALLDATETIME
SET @To = @ToDate
SET @FromDate = CAST(
(
STR( YEAR( @FromDate ) ) + '-' +
STR( MONTH( @FromDate ) ) + '-' +
STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate = CAST(
(
STR( YEAR( @ToDate ) ) + '-' +
STR( MONTH(@ToDate ) ) + '-' +
STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)


-- Apply the Time Shift 
SET @FromDate = dateadd(mi,-@GMTShifttime,@FromDate);
SET @ToDate = dateadd(mi,-@GMTShifttime,@ToDate);

-- building a temp table that contains all billing data requested 
--------------------------------------------------------------------------------------



INSERT INTO  #Billing_Invoice_Details
(
    InvoiceID, Destination, FromDate, TillDate, Duration, Rate, RateType, Amount, NumberOfCalls, CurrencyID, UserID
)
SELECT 
       1,
       cast (bcm.OurZoneID AS CHAR(100)) COLLATE SQL_Latin1_General_CP1256_CI_AS ,
       MIN(dateadd(mi,@GMTShifttime,Bcm.Attempt)),
       MAX(dateadd(mi,@GMTShifttime,Bcm.Attempt)),
       SUM(Bcs.DurationInSeconds)/60.0,
       Round(Bcs.RateValue,5) ,
       Bcs.RateType,
       ISNULL(Round(SUM(Bcs.Net),2),0),
       COUNT(*),
       Bcs.CurrencyID,
       -1
FROM   Billing_CDR_Main bcm WITH(NOLOCK,index(IX_Billing_CDR_Main_Attempt))
       LEFT JOIN Billing_CDR_Sale bcs WITH(NOLOCK) ON  bcs.ID = bcm.ID
WHERE  bcm.Attempt >= @FromDate
  AND  bcm.Attempt < @ToDate
  AND  bcm.CustomerID = @CustomerID 
  AND  bcs.CurrencyID IS NOT NULL
GROUP BY
        cast (bcm.OurZoneID AS CHAR(100)),
        Round(Bcs.RateValue,5) ,
		Bcs.RateType,
		Bcs.CurrencyID



-- Apply the Time Shift 
SET @FromDate = dateadd(mi,@GMTShifttime,@FromDate);
SET @ToDate = dateadd(mi,@GMTShifttime,@ToDate);
-- getting the currency exchange rate for the selected customer  
----------------------------------------------------------
SELECT @CurrencyID = CurrencyID
FROM   CarrierProfile WITH(NOLOCK)
WHERE  ProfileID = (
           SELECT ProfileID
           FROM   CarrierAccount WITH(NOLOCK)
           WHERE  CarrierAccountID = @CustomerID
       )	
         	  
-- Exchange rates 
-----------------------------------------------------------
create table #ExchangeRates (
			Currency VARCHAR(3) COLLATE SQL_Latin1_General_CP1256_CI_AS ,
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO #ExchangeRates 
SELECT gder1.Currency,
       gder1.Date,
       gder1.Rate / gder2.Rate
FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder1  
       LEFT JOIN dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder2  
            ON  gder1.Date = gder2.Date
            AND gder2.Currency = @CurrencyID
            


SELECT 
       @Duration = SUM(Duration),
       @Amount = SUM(Amount),
       @NumberOfCalls =  SUM(NumberOfCalls)
FROM  #Billing_Invoice_Details WITH(NOLOCK)

IF(@Duration IS NULL OR @Amount IS NULL) RETURN



-- saving the billing invoice 
INSERT INTO #Billing_Invoice
(
    BeginDate, EndDate, IssueDate, DueDate, CustomerID, SupplierID, SerialNumber, 
    Duration, Amount, NumberOfCalls, CurrencyID, IsLocked, IsPaid, UserID
)

SELECT @From,
       @To,
       @IssueDate,
       @DueDate,
       @CustomerID,
       'SYS',
       '1',
       0,	--ISNULL(@Duration,0)/60.0,
       0,	--ISNULL(@Amount,0),
       @NumberOfCalls,
       @CurrencyID,
       'N',
       'N',
       -1
       

            
SELECT @Duration = SUM(bid.Duration),
       @NumberOfCalls = SUM(bid.NumberOfCalls)
FROM   #Billing_Invoice_Details bid  WITH(NOLOCK)


SELECT @Amount = SUM(B.amount / ISNULL(ERS.Rate, 1)) 
FROM #Billing_Invoice_Details B WITH(NOLOCK)
LEFT JOIN #ExchangeRates ERS WITH(NOLOCK) ON ERS.Currency = B.CurrencyID AND ERS.Date = dbo.DateOf(B.TillDate)

UPDATE #Billing_Invoice
SET    Duration = ISNULL(@Duration,0),
       Amount = ISNULL(@Amount,0) ,
       NumberOfCalls  = ISNULL(@NumberOfCalls,0) 
       
       -- updating the ZoneName 
UPDATE #Billing_Invoice_Details
SET    Destination = Zone.Name
FROM   #Billing_Invoice_Details BID WITH(NOLOCK),
       Zone WITH(NOLOCK)
WHERE  BID.Destination  = Zone.ZoneID



SELECT * FROM #Billing_Invoice
SELECT * FROM #Billing_Invoice_Details



RETURN
GO
/****** Object:  UserDefinedFunction [dbo].[GetDailyConnectivity]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetDailyConnectivity]
(
	@ConnectionType TINYINT = null,
	@CarrierAccountID VARCHAR(10),
	@SwitchID TINYINT,
	@GroupByName CHAR(1),
	@From DATETIME,
	@Till DATETIME,
	@ForInterconnectedSwitches CHAR(1) = 'N'  
)
RETURNS 
@Connectivities TABLE 
(
    GateWay VARCHAR(250),
    Details VARCHAR(MAX),
	Date SMALLDATETIME,
	NumberOfChannels_In INT,
	NumberOfChannels_Out int,
	NumberOfChannels_Total int,
	Margin_Total INT
)
AS
BEGIN

	SET @From = dbo.DateOf(@From)
	DECLARE @Pivot DATETIME
	SET @Pivot = @From
	WHILE @Pivot <= @Till
	BEGIN
		INSERT INTO @Connectivities
		(
			Gateway,
			Date,
			Details,
		    NumberOfChannels_In,
		    NumberOfChannels_Out,
		    NumberOfChannels_Total,
		    Margin_Total
		)
		SELECT
		    [Name] = CASE WHEN @GroupByName = 'Y' THEN csc.[Name]
		                  ELSE NULL END,
		    @Pivot,
            [Details] = CASE WHEN @GroupByName = 'Y' THEN csc.Details
		                  ELSE  null  END,
		    sum(csc.NumberOfChannels_In),
		    sum(csc.NumberOfChannels_Out),
		    sum(csc.NumberOfChannels_Total),
		    SUM(csc.NumberOfChannels_Total * csc.Margin_Total) / SUM(csc.NumberOfChannels_Total) 
		FROM CarrierSwitchConnectivity csc WITH(NOLOCK,INDEX(IX_CSC_CarrierAccount)) 
		    LEFT JOIN CarrierAccount ca WITH(NOLOCK) ON ca.CarrierAccountID = csc.CarrierAccountID
		WHERE 
		   (csc.BeginEffectiveDate <= @Pivot AND (csc.EndEffectiveDate IS NULL OR csc.EndEffectiveDate > @Pivot))
		   AND (@ConnectionType  IS NULL OR csc.ConnectionType = @ConnectionType) 
		   AND (@CarrierAccountID IS NULL Or csc.CarrierAccountID = @CarrierAccountID)  
		   AND (@SwitchID IS NULL OR csc.SwitchID = @SwitchID) 
		   AND ca.RepresentsASwitch = @ForInterconnectedSwitches
		   AND dbo.IsNullOrEmpty(csc.Details,'') <> ''
		GROUP BY 
		CASE WHEN @GroupByName = 'Y' THEN csc.[Name]
		                  ELSE NULL END,
         
	    CASE WHEN @GroupByName = 'Y' THEN csc.Details
		                  ELSE  null END
		
		SET @Pivot = DATEADD(dd, 1, @Pivot)
	END
	
	IF(@GroupByName = 'N')
	BEGIN 
		SET @Pivot = @From
		WHILE @Pivot <= @Till
	     BEGIN
          DECLARE @det VARCHAR(MAX)
          
          SELECT  @det = COALESCE(@det + ',' ,'' ) + isnull(csc.Details,'')
          FROM CarrierSwitchConnectivity csc, CarrierAccount ca 
	      WHERE 
		   (csc.BeginEffectiveDate <= @Pivot AND (csc.EndEffectiveDate IS NULL OR csc.EndEffectiveDate > @Pivot))
		   AND (@CarrierAccountID is null or csc.CarrierAccountID = @CarrierAccountID)
		   AND ca.CarrierAccountID = csc.CarrierAccountID
		   AND (@ConnectionType  IS NULL OR csc.ConnectionType = @ConnectionType) 
		   AND (@SwitchID IS NULL OR csc.SwitchID = @SwitchID) 
		   AND ca.RepresentsASwitch = @ForInterconnectedSwitches
		   
		  UPDATE @Connectivities SET Details =  @det WHERE Date = @Pivot
		  SET @Pivot = DATEADD(dd, 1, @Pivot)
	     END
	END 
	RETURN
END
GO
/****** Object:  StoredProcedure [dbo].[bp_MissingCost]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_MissingCost]
	@Top int = 50,
	@sys_CDR_Pricing_CDRID bigint,
	@FromDate datetime,
	@TillDate DATETIME,
	@CustomerID VARCHAR(10) = NULL,
	@SupplierID VARCHAR(10) = NULL

AS
BEGIN
	
	SET ROWCOUNT @Top
	
SET @FromDate = CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @TillDate = CAST(
(
	STR( YEAR( @TillDate ) ) + '-' +
	STR( MONTH(@TillDate ) ) + '-' +
	STR( DAY( @TillDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)


SELECT *
    FROM   Billing_CDR_Main bcm WITH(NOLOCK ,INDEX(IX_Billing_CDR_Main_Attempt))
    JOIN CarrierAccount cac ON cac.CarrierAccountID = bcm.CustomerID 
    WHERE bcm.Attempt >= @FromDate
			AND  bcm.Attempt <= @TillDate
			AND (@CustomerID IS NULL OR bcm.CustomerID = @CustomerID )
			AND (@SupplierID IS NULL OR bcm.SupplierID = @SupplierID)
			AND  NOT EXISTS (
                  SELECT *
                  FROM   Billing_CDR_Cost bcc WITH(nolock)
                  WHERE  bcc.ID = bcm.ID
					   )
			AND bcm.ID <= @sys_CDR_Pricing_CDRID
			AND cac.RepresentsASwitch <> 'Y'
			AND cac.IsPassThroughCustomer <> 'Y'
			AND cac.IsPassThroughSupplier <> 'Y'

END
GO
/****** Object:  StoredProcedure [dbo].[bp_CreateSupplierInvoiceGroupByDay]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_CreateSupplierInvoiceGroupByDay]
@SupplierID varchar(5),
@FromDate Datetime,
@ToDate   Datetime,
@GMTShifttime float
AS

BEGIN
	
	SET @FromDate = CAST(
(
STR( YEAR( @FromDate ) ) + '-' +
STR( MONTH( @FromDate ) ) + '-' +
STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate = CAST(
(
STR( YEAR( @ToDate ) ) + '-' +
STR( MONTH(@ToDate ) ) + '-' +
STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)
	DECLARE @Results TABLE ( 
		                   [Day]		Datetime,
		                   [Currency] VARCHAR(3),
						   Duration 	float,
						   Amount		float
							) 

	INSERT INTO @Results	(
							[Day],
							[Currency],
							Duration,
							Amount
							)	
    
	SELECT CONVERT(varchar(10),dateadd(mi,-@GMTShifttime,BM.Attempt),121) as [Day],
	       bc.CurrencyID,
           SUM(BC.DurationInSeconds)/60.0 AS Duration,
           ISNULL(SUM(BC.NET),0) AS Amount
    FROM   Billing_CDR_Main BM WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt,IX_Billing_CDR_Main_Supplier))
    left join Billing_CDR_Cost BC WITH(nolock)
      ON   BM.ID = BC.ID
    WHERE  BM.SupplierID = @SupplierID
      AND  BM.Attempt >= @FromDate
      AND  BM.Attempt <=  @ToDate
    GROUP BY
           CONVERT(varchar(10),dateadd(mi,-@GMTShifttime,BM.Attempt),121),
           bc.CurrencyID
    ORDER BY
           CONVERT(varchar(10),dateadd(mi,-@GMTShifttime,BM.Attempt),121)
	
    SELECT		  CASE
					WHEN @GMTShifttime <= 0  THEN CONVERT(varchar(10),([Day]),121)
					WHEN @GMTShifttime > 0  THEN CONVERT(varchar(10),[Day]+1,121)
				  END AS [Day],
				         Currency,
						 Duration,
						 Amount
    FROM			@Results 
	ORDER BY		[day]

END
GO
/****** Object:  UserDefinedFunction [dbo].[IsAPaidDate]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ==================================================================
-- Author: Fadi Chamieh
-- Create date: 2009-10-14
-- Description:	Get a boolean value indicating if date is paid or not 
-- ===================================================================
CREATE FUNCTION [dbo].[IsAPaidDate](@Date SMALLDATETIME, @CustomerID VARCHAR(10), @SupplierID VARCHAR(10), @CustomerProfileID SMALLINT, @SupplierProfileID SMALLINT) 
RETURNS CHAR(1)
AS
BEGIN
	DECLARE @Result CHAR(1)
	
	IF @SupplierID IS NOT NULL
	BEGIN
		SELECT @Result = 
			CASE WHEN EXISTS(SELECT * FROM Billing_Invoice bi 
			                 WHERE bi.SupplierID = @SupplierID 
								AND @Date BETWEEN bi.BeginDate AND bi.EndDate 
								AND bi.IsPaid = 'Y'
			)
				THEN 'Y'
				ELSE 'N'
			END			
	END
	ELSE		
	IF @CustomerID IS NOT NULL
	BEGIN
		SELECT @Result = 
			CASE WHEN EXISTS(SELECT * FROM Billing_Invoice bi 
			                 WHERE bi.CustomerID = @CustomerID
								AND @Date BETWEEN bi.BeginDate AND bi.EndDate 
								AND bi.IsPaid = 'Y'
			)
				THEN 'Y'
				ELSE 'N'
			END			
	END
	ELSE
	IF @SupplierProfileID IS NOT NULL
	BEGIN
		SELECT @Result = 
			CASE WHEN EXISTS(SELECT * FROM Billing_Invoice bi 
			                 WHERE bi.SupplierID IN (SELECT ca.CarrierAccountID FROM CarrierAccount ca WHERE ca.ProfileID = @SupplierProfileID) 
								AND @Date BETWEEN bi.BeginDate AND bi.EndDate 
								AND bi.IsPaid = 'Y'
			)
				THEN 'Y'
				ELSE 'N'
			END			
	END
	ELSE		
	IF @CustomerProfileID IS NOT NULL
	BEGIN
		SELECT @Result = 
			CASE WHEN EXISTS(SELECT * FROM Billing_Invoice bi 
			                 WHERE bi.CustomerID IN (SELECT ca.CarrierAccountID FROM CarrierAccount ca WHERE ca.ProfileID = @CustomerProfileID)
								AND @Date BETWEEN bi.BeginDate AND bi.EndDate 
								AND bi.IsPaid = 'Y'
			)
				THEN 'Y'
				ELSE 'N'
			END			
	END
		
	RETURN @Result

END
GO
/****** Object:  StoredProcedure [dbo].[bp_ResetRoutes]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =======================================================================
-- Author:		Fadi Chamieh
-- Create date: 18/12/2007
-- Description:	Reset Routes / Route Options to defaults before an update
-- =======================================================================
CREATE PROCEDURE [dbo].[bp_ResetRoutes](@UpdateType varchar(10))	

AS
BEGIN
	
	DECLARE @State_Blocked tinyint
	DECLARE @State_Enabled tinyint
	SET @State_Blocked = 0
	SET @State_Enabled = 1

	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;

	-- Get the Min Update
	DECLARE @RoutesCreation datetime 
	SELECT @RoutesCreation = MIN(Updated) FROM [Route]
	
	DECLARE @UpdateStamp datetime
	SELECT @UpdateStamp = getdate()
	
	-- Reset Route Options
	UPDATE [RouteOption]		
		SET
			SupplierActiveRate = CASE WHEN @UpdateType = 'TOD' THEN SupplierNormalRate ELSE SupplierActiveRate END
			, Priority = CASE WHEN @UpdateType = 'SpecialRequests' THEN 0 ELSE Priority END
			, NumberOfTries = CASE WHEN @UpdateType = 'SpecialRequests' THEN 1 ELSE NumberOfTries END
			, State = CASE WHEN @UpdateType = 'RouteBlocks' THEN @State_Enabled ELSE RouteOption.State END
			, Updated = @UpdateStamp
		FROM [RouteOption], [Route]
		WHERE
				[RouteOption].RouteID = [Route].RouteID
			AND [Route].Updated > @RoutesCreation
			AND	(
					(@UpdateType = 'TOD' AND [Route].IsToDAffected = 'Y')
					OR
					(@UpdateType = 'SpecialReqests' AND [Route].IsSpecialRequestAffected = 'Y')
					OR
					(@UpdateType = 'RouteBlocks' AND [Route].IsBlockAffected = 'Y')
				)

	-- Reset Routes	
	UPDATE [Route]
		SET
			OurActiveRate = CASE WHEN @UpdateType = 'TOD' THEN OurNormalRate ELSE OurActiveRate END
			,State = CASE WHEN @UpdateType = 'RouteBlocks' THEN @State_Enabled ELSE [State] END
			,[Route].IsSpecialRequestAffected = CASE WHEN @UpdateType = 'SpecialReqests' THEN 'N' ELSE [Route].IsSpecialRequestAffected END
			,[Route].IsToDAffected = CASE WHEN @UpdateType = 'TOD' THEN 'N' ELSE [Route].IsToDAffected END
			,[Route].IsBlockAffected = CASE WHEN @UpdateType = 'RouteBlocks' THEN 'N' ELSE [Route].IsBlockAffected END
			,Updated = @UpdateStamp
		WHERE
			Updated > @RoutesCreation
			AND	(
					(@UpdateType = 'TOD' AND [Route].IsToDAffected = 'Y')
					OR
					(@UpdateType = 'SpecialReqests' AND [Route].IsSpecialRequestAffected = 'Y')
					OR
					(@UpdateType = 'RouteBlocks' AND [Route].IsBlockAffected = 'Y')
			)
END
GO
/****** Object:  StoredProcedure [dbo].[bp_SignalRouteUpdatesFromOptions]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_SignalRouteUpdatesFromOptions](@UpdateStamp datetime, @UpdateType varchar(20))

AS
	UPDATE [Route]
		SET [Route].Updated = [RouteOption].Updated
			,[Route].IsToDAffected =			CASE WHEN @UpdateType LIKE 'TOD' THEN 'Y' ELSE [Route].IsToDAffected END
			,[Route].IsSpecialRequestAffected = CASE WHEN @UpdateType LIKE 'SpecialRequests' THEN 'Y' ELSE [Route].IsSpecialRequestAffected END
			,[Route].IsBlockAffected =			CASE WHEN @UpdateType LIKE 'RouteBlocks' THEN 'Y' ELSE [Route].IsBlockAffected END
	FROM [Route], [RouteOption]
		WHERE [Route].RouteID=[RouteOption].RouteID
		AND [RouteOption].Updated >= @UpdateStamp
		AND [Route].Updated < [RouteOption].Updated
GO
/****** Object:  View [dbo].[vw_ValidRouteOptions]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_ValidRouteOptions]
AS
	SELECT    
		TOP (100) PERCENT R.RouteID, R.CustomerID, R.Code, O.SupplierID, O.Priority, O.SupplierActiveRate, O.NumberOfTries
		FROM         
			dbo.RouteOption AS O WITH (NOLOCK)  
			INNER JOIN dbo.Route AS R WITH (NOLOCK) ON R.RouteID = O.RouteID AND O.SupplierActiveRate < R.OurActiveRate
		WHERE     
				(R.State <> 0) 
			AND (O.State <> 0)
GO
/****** Object:  View [dbo].[vw_NonSuppliedRoutes]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_NonSuppliedRoutes]
AS
	SELECT TOP 100 PERCENT * FROM Route R WHERE NOT EXISTS(SELECT * FROM RouteOption O WHERE O.RouteID=R.RouteID) ORDER BY R.CustomerID
GO
/****** Object:  StoredProcedure [dbo].[bp_UpdateRoutesFromSupplierRouteBlock]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[bp_UpdateRoutesFromSupplierRouteBlock](
@CustomerID VARCHAR(10) = NULL,  
@SupplierZoneID  INT 
)
with Recompile
AS 

DECLARE  @Routes TABLE  (RouteID int)
INSERT INTO @Routes 
	SELECT r.RouteID FROM [Route] r
	WHERE  EXISTS (
           SELECT *
           FROM   RouteOption ro
           WHERE  ro.SupplierZoneID = @SupplierZoneID
                  AND ro.RouteID = r.RouteID
       )
       AND (@CustomerID IS NULL OR r.CustomerID = @CustomerID) 



UPDATE [Route] 
SET    Updated = GETDATE(),
       IsBlockAffected = 'Y'
FROM [Route]  WITH(NOLOCK)
WHERE  RouteID IN  (
           SELECT RTU.RouteID 
           FROM   @Routes RTU )
             
             
UPDATE RouteOption
SET    [State] = 0,
       Updated = GETDATE()
FROM RouteOption WITH(NOLOCK,INDEX(IDX_RouteOption_SupplierZoneID))
WHERE  RouteID IN  (
           SELECT RTU.RouteID 
           FROM   @Routes RTU )
AND SupplierZoneID = @SupplierZoneID

RETURN
GO
/****** Object:  StoredProcedure [dbo].[bp_BuildZoneRates]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_BuildZoneRates]
	@CustomerID varchar(10) = NULL,
	@SupplierID varchar(10) = NULL,
	@IncludeBlockedZones CHAR(1) = 'N'
WITH RECOMPILE
AS
BEGIN
	
	SET NOCOUNT ON

	DECLARE @when datetime
	SET @when = dbo.DateOf(getdate())

	
	-- If no supplier or customer given then ZoneRate is to be rebuilt
	IF @CustomerID IS NULL AND @SupplierID IS NULL 
	  BEGIN
		TRUNCATE TABLE ZoneRate
	  END
	ELSE
	  BEGIN
		DELETE FROM ZoneRate WHERE
			(@CustomerID IS NOT NULL AND CustomerID = @CustomerID)
			OR
			(@SupplierID IS NOT NULL AND SupplierID = @SupplierID)
	  END

   SELECT RB.CustomerID, RB.ZoneID, RB.SupplierID  INTO #Blocks 
   FROM RouteBlock RB WITH(NOLOCK)
     WHERE RB.IsEffective = 'Y'
	AND RB.ZoneID IS NOT NULL

	-- Insert Active Zone Rates
	INSERT INTO ZoneRate
		(
			ZoneID,
			SupplierID,
			CustomerID,
			NormalRate,
			OffPeakRate,
			WeekendRate,
			ServicesFlag
		)
		SELECT			
			Ra.ZoneID,
			P.SupplierID,
			P.CustomerID,
			dbo.GetCommissionedRate(P.SupplierID, P.CustomerID, p.CurrencyID, getdate(), Ra.ZoneID, Ra.Rate) / C.LastRate,
			dbo.GetCommissionedRate(P.SupplierID, P.CustomerID, p.CurrencyID, getdate(), Ra.ZoneID, Ra.OffPeakRate) / C.LastRate,
			dbo.GetCommissionedRate(P.SupplierID, P.CustomerID, p.CurrencyID, getdate(), Ra.ZoneID, Ra.WeekendRate) / C.LastRate,
			Ra.ServicesFlag
		FROM Rate Ra
				INNER JOIN PriceList P ON Ra.PriceListID = P.PriceListID
				INNER JOIN Currency C ON P.CurrencyID = C.CurrencyID
			WHERE Ra.BeginEffectiveDate <= @when AND (Ra.EndEffectiveDate IS NULL OR Ra.EndEffectiveDate > @when)
				AND 
				(
					(@CustomerID IS NULL AND @SupplierID IS NULL)
					OR
					(P.CustomerID = @CustomerID)
					OR
					(P.SupplierID = @SupplierID)
				)
				AND 
				(
					@IncludeBlockedZones = 'Y'
					OR
					(
						Ra.ZoneID NOT IN ( SELECT RB.ZoneID 
												FROM #Blocks RB
												WHERE RB.SupplierID = p.SupplierID 
													  AND (Rb.CustomerID is null Or Rb.CustomerID = p.CustomerID) 
													)
					)
				)

	RETURN
END
GO
/****** Object:  StoredProcedure [dbo].[bp_CDR_Backup]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ==========================================================================================
-- Author:		Fadi Chamieh
-- Create date: 30-07-2008
-- Description:	Shipping the CDR less than the date and insert it into CDRBackup 
--              after that move the CDR Record To the TempTable after that truncate 
--              the cdr table and insert the CDRTemp
-- ==========================================================================================
CREATE PROCEDURE [dbo].[bp_CDR_Backup]
	@StartingDate datetime,
	@TableName varchar(100) = 'CDR_Backup'
AS
BEGIN
	SET NOCOUNT ON	
		
	IF NOT EXISTS(SELECT name FROM sys.tables WHERE name LIKE @TableName)
    BEGIN
		EXECUTE('SELECT * INTO ' + @TableName + ' FROM CDR WHERE 0=1')
    END

	declare @startingDateStr varchar(50)
	Set @startingDateStr = convert(varchar(50), @StartingDate, 121)
		
	declare @columnList varchar(2000)
	Set @columnList = 
	'
		SwitchID,
		IDonSwitch,
		Tag,
		AttemptDateTime,
		AlertDateTime,
		ConnectDateTime,
		DisconnectDateTime,
		DurationInSeconds,
		IN_TRUNK,
		IN_CIRCUIT,
		IN_CARRIER,
		IN_IP,
		OUT_TRUNK,
		OUT_CIRCUIT,
		OUT_CARRIER,
		OUT_IP,
		CGPN,
		CDPN,
		CAUSE_FROM_RELEASE_CODE,
		CAUSE_FROM,
		CAUSE_TO_RELEASE_CODE,
		CAUSE_TO, 
		Extra_Fields
	'
	
	EXECUTE(
		'INSERT INTO ' + @TableName 
			+ '('+ @columnList +') 
		SELECT 
			' + @columnList + ' 
		FROM CDR WITH(NOLOCK)
			WHERE AttemptDateTime < ''' + @startingDateStr + '''')
	
	IF @@ERROR = 0 
	BEGIN
		DELETE FROM CDR WHERE AttemptDateTime < @StartingDate
	END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Traffic_CDR]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Traffic_CDR]
		@CDROption     INT,-- 0 for all , 1 for validCDR , 2 For InvalidCDR
        @FromDuration  Numeric(13,5) = NULL,
        @ToDuration    Numeric(13,5) = NULL,
        @FromDate      DateTime ,
        @ToDate        DateTime ,
        @TopRecord     Int,
        @SwitchID      Tinyint =NULL,
        @SupplierID    varchar(10) = NULL,
        @CustomerID    varchar(10) = NULL,
        @OurZoneID     int = NULL,
        @Number       varchar(25)=NULL,
        @CLI VARCHAR(25)=NULL,
        @ReleaseCode VARCHAR(25)=NULL
        WITH Recompile 
AS
SET NOCOUNT ON 
SET ROWCOUNT @TopRecord
if @CustomerID is null and @SupplierID is null 
    SELECT 
		    Attempt AS AttemptDateTime,
			CDPN,
			CGPN,
			ReleaseCode,
	        ReleaseSource,
		    DurationInSeconds AS Durations,
			SupplierZoneID,
			SupplierID,
			OurZoneID,
			CustomerID,
			SwitchCdrID,
			Tag            
     FROM Billing_CDR_Main M WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt))
     WHERE 1=1
        AND (@CDROption=0 OR @CDROption=1)                                     
		AND (Attempt Between @FromDate And @ToDate)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
	    AND (@Number IS NULL OR CDPN LIKE @Number)
        AND(@CLI IS NULL OR CGPN LIKE @CLI)   
        AND(@ReleaseCode IS NULL OR ReleaseCode LIKE @ReleaseCode)
        AND(@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND(@ToDuration IS NULL OR DurationInSeconds <= @ToDuration) 
UNION ALL
   SELECT 
		Attempt AS AttemptDateTime,
		CDPN,
		CGPN,
		ReleaseCode,
		ReleaseSource,
	    DurationInSeconds,
		SupplierZoneID,
		SupplierID,
		OurZoneID,
		CustomerID,
		SwitchCdrID,
		Tag
	FROM Billing_CDR_Invalid WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt))
	WHERE 1=1
        AND (@CDROption  =0 OR @CDROption =2)                                     
		AND (Attempt Between @FromDate And @ToDate)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
	    AND (@Number IS NULL OR CDPN LIKE @Number)
	    AND(@CLI IS NULL OR CGPN LIKE @CLI)   
        AND(@ReleaseCode IS NULL OR ReleaseCode LIKE @ReleaseCode)
        AND(@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND(@ToDuration IS NULL OR DurationInSeconds <= @ToDuration) 
    ORDER BY Attempt DESC   



if @CustomerID is not null and @SupplierID is null 
    SELECT 
		    Attempt AS AttemptDateTime,
			CDPN,
			CGPN,
			ReleaseCode,
	        ReleaseSource,
		    DurationInSeconds,
			SupplierZoneID,
			SupplierID,
			OurZoneID,
			CustomerID,
			SwitchCdrID,
			Tag            
     FROM Billing_CDR_Main M WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt),INDEX(IX_Billing_CDR_Main_Customer))
     WHERE 1=1
        AND (@CDROption=0 OR @CDROption=1)                                     
		AND (Attempt Between @FromDate And @ToDate)
		AND (CustomerID = @CustomerID)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
	    AND (@Number IS NULL OR CDPN LIKE @Number)
        AND(@CLI IS NULL OR CGPN LIKE @CLI)   
        AND(@ReleaseCode IS NULL OR ReleaseCode LIKE @ReleaseCode)
        AND(@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND(@ToDuration IS NULL OR DurationInSeconds <= @ToDuration) 
UNION ALL
   SELECT 
		Attempt AS AttemptDateTime,
		CDPN,
		CGPN,
		ReleaseCode,
		ReleaseSource,
	    DurationInSeconds,
		SupplierZoneID,
		SupplierID,
		OurZoneID,
		CustomerID,
		SwitchCdrID,
		Tag
	FROM Billing_CDR_Invalid WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt),INDEX(IX_Billing_CDR_InValid_Customer))
	WHERE 1=1
        AND (@CDROption  =0 OR @CDROption =2)                                     
		AND (Attempt Between @FromDate And @ToDate)
		AND (CustomerID = @CustomerID)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
	    AND (@Number IS NULL OR CDPN LIKE @Number)
	    AND(@CLI IS NULL OR CGPN LIKE @CLI)   
        AND(@ReleaseCode IS NULL OR ReleaseCode LIKE @ReleaseCode)
        AND(@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND(@ToDuration IS NULL OR DurationInSeconds <= @ToDuration) 
    ORDER BY Attempt DESC   


if @CustomerID is null and @SupplierID is not null 
    SELECT 
		    Attempt AS AttemptDateTime,
			CDPN,
			CGPN,
			ReleaseCode,
	        ReleaseSource,
		    DurationInSeconds,
			SupplierZoneID,
			SupplierID,
			OurZoneID,
			CustomerID,
			SwitchCdrID,
			Tag            
     FROM Billing_CDR_Main M WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt),INDEX(IX_Billing_CDR_Main_Supplier))
     WHERE 1=1
        AND (@CDROption=0 OR @CDROption=1)                                     
		AND (Attempt Between @FromDate And @ToDate)
		AND (SupplierID = @SupplierID)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
	    AND (@Number IS NULL OR CDPN LIKE @Number)
        AND(@CLI IS NULL OR CGPN LIKE @CLI)   
        AND(@ReleaseCode IS NULL OR ReleaseCode LIKE @ReleaseCode)
        AND(@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND(@ToDuration IS NULL OR DurationInSeconds <= @ToDuration) 
UNION ALL
   SELECT 
		Attempt AS AttemptDateTime,
		CDPN,
		CGPN,
		ReleaseCode,
		ReleaseSource,
	    DurationInSeconds,
		SupplierZoneID,
		SupplierID,
		OurZoneID,
		CustomerID,
		SwitchCdrID,
		Tag
	FROM Billing_CDR_Invalid WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt),INDEX(IX_Billing_CDR_InValid_Supplier))
	WHERE 1=1
        AND (@CDROption  =0 OR @CDROption =2)                                     
		AND (Attempt Between @FromDate And @ToDate)
		AND (SupplierID = @SupplierID)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
	    AND (@Number IS NULL OR CDPN LIKE @Number)
	    AND(@CLI IS NULL OR CGPN LIKE @CLI)   
        AND(@ReleaseCode IS NULL OR ReleaseCode LIKE @ReleaseCode)
        AND(@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND(@ToDuration IS NULL OR DurationInSeconds <= @ToDuration) 
    ORDER BY Attempt DESC   


if @CustomerID is not null and @SupplierID is not null 
    SELECT 
		    Attempt AS AttemptDateTime,
			CDPN,
			CGPN,
			ReleaseCode,
	        ReleaseSource,
		    DurationInSeconds,
			SupplierZoneID,
			SupplierID,
			OurZoneID,
			CustomerID,
			SwitchCdrID,
			Tag            
     FROM Billing_CDR_Main M WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt),INDEX(IX_Billing_CDR_Main_Customer),INDEX(IX_Billing_CDR_Main_Supplier))
     WHERE 1=1
        AND (@CDROption=0 OR @CDROption=1)                                     
		AND (Attempt Between @FromDate And @ToDate)
		AND (CustomerID = @CustomerID)
		AND (SupplierID = @SupplierID)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
	    AND (@Number IS NULL OR CDPN LIKE @Number)
        AND(@CLI IS NULL OR CGPN LIKE @CLI)   
        AND(@ReleaseCode IS NULL OR ReleaseCode LIKE @ReleaseCode)
        AND(@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND(@ToDuration IS NULL OR DurationInSeconds <= @ToDuration) 
UNION ALL
   SELECT 
		Attempt AS AttemptDateTime,
		CDPN,
		CGPN,
		ReleaseCode,
		ReleaseSource,
	    DurationInSeconds,
		SupplierZoneID,
		SupplierID,
		OurZoneID,
		CustomerID,
		SwitchCdrID,
		Tag
	FROM Billing_CDR_Invalid WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt),INDEX(IX_Billing_CDR_InValid_Customer),INDEX(IX_Billing_CDR_InValid_Supplier))
	WHERE 1=1
        AND (@CDROption  =0 OR @CDROption =2)                                     
		AND (Attempt Between @FromDate And @ToDate)
		AND (CustomerID = @CustomerID)
		AND (SupplierID = @SupplierID)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
	    AND (@Number IS NULL OR CDPN LIKE @Number)
	    AND(@CLI IS NULL OR CGPN LIKE @CLI)   
        AND(@ReleaseCode IS NULL OR ReleaseCode LIKE @ReleaseCode)
        AND(@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND(@ToDuration IS NULL OR DurationInSeconds <= @ToDuration) 
    ORDER BY Attempt DESC   


  OPTION(RECOMPILE)
GO
/****** Object:  StoredProcedure [dbo].[bp_MissingSale]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_MissingSale]
	@Top int = 50,
	@sys_CDR_Pricing_CDRID bigint,
	@FromDate datetime,
	@TillDate DATETIME,
	@CustomerID VARCHAR(10) = NULL,
	@SupplierID VARCHAR(10) = NULL

AS
BEGIN
	
	SET ROWCOUNT @Top
	
SET @FromDate = CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @TillDate = CAST(
(
	STR( YEAR( @TillDate ) ) + '-' +
	STR( MONTH(@TillDate ) ) + '-' +
	STR( DAY( @TillDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)

SELECT *
FROM   Billing_CDR_Main bcm WITH(NOLOCK ,INDEX(IX_Billing_CDR_Main_Attempt))
JOIN CarrierAccount cac ON cac.CarrierAccountID = bcm.CustomerID 
WHERE bcm.Attempt >= @FromDate
		AND  bcm.Attempt <= @TillDate
		AND (@CustomerID IS NULL OR bcm.CustomerID = @CustomerID)
		AND (@SupplierID IS NULL OR bcm.SupplierID = @SupplierID)
		AND  NOT EXISTS (
                  SELECT *
                  FROM   Billing_CDR_Sale bcs WITH(nolock)
                  WHERE  bcs.ID = bcm.ID
					   )
		AND bcm.ID <= @sys_CDR_Pricing_CDRID
		AND cac.RepresentsASwitch <> 'Y'
		AND cac.IsPassThroughCustomer <> 'Y'
		AND cac.IsPassThroughSupplier <> 'Y'
END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_RepeatedNumber]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TrafficStats_RepeatedNumber] 
	 @Fromdate datetime,
	 @ToDate   datetime,
	 @Number   INT,
	 @Type varchar(20)='ALL' -- can be  'ALL' or 'SUCCESSFUL' or 'FAILED'
	 ,@SwitchID tinyint = NULL
	 ,@CustomerID varchar(5) = NULL
AS
BEGIN
	
SET NOCOUNT ON

SELECT N.OurZoneID, N.PhoneNumber, N.CustomerID, N.SupplierID, Sum(N.Attempt) Attempt, Sum(N.DurationsInMinutes) DurationsInMinutes FROM
( 
	SELECT  
		BM.OurZoneID AS OurZoneID,
		BM.cdpn as PhoneNumber, 
		BM.CustomerID,
		BM.SupplierID,
		Count(BM.Attempt) as Attempt, 
		Sum (BM.DurationInSeconds)/60. as DurationsInMinutes 
	FROM dbo.Billing_CDR_Main BM WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt)) 
	WHERE 
			Attempt BETWEEN @FromDate AND @ToDate
		AND @Type IN ('ALL', 'SUCCESSFUL')
		AND (@SwitchID IS NULL OR BM.SwitchID = @SwitchID)
		AND (@CustomerID IS NULL OR BM.CustomerID = @CustomerID)
	GROUP BY BM.OurZoneID,cdpn, BM.CustomerID, BM.SupplierID
	
	UNION ALL

	SELECT  
		BI.OurZoneID AS OurZoneID,
		BI.cdpn as phonenumber, 
		BI.CustomerID,
		BI.SupplierID,
		Count(BI.Attempt) as Attempt, 
		Sum (BI.DurationInSeconds) / 60. as DurationsInMinutes 
	FROM dbo.Billing_CDR_Invalid BI WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt)) 
	WHERE 
			Attempt BETWEEN @FromDate AND @ToDate
		AND @Type IN ('ALL', 'FAILED')
		AND (@CustomerID IS NULL OR BI.CustomerID = @CustomerID)
		AND (@SwitchID IS NULL OR BI.SwitchID = @SwitchID)
	GROUP BY BI.OurZoneID, cdpn, BI.CustomerID, BI.SupplierID
	
) N
GROUP BY N.OurZoneID, N.PhoneNumber, N.CustomerID, N.SupplierID
HAVING Sum(N.Attempt) >= @Number 
ORDER BY Sum(N.Attempt) DESC 

OPTION (recompile)
END
GO
/****** Object:  StoredProcedure [dbo].[bp_GetMissMappedCdrs]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
 * Get CDRs that were considered invalid with unknown Customer or Supplier 
 */
CREATE PROCEDURE [dbo].[bp_GetMissMappedCdrs](@From datetime, @Till datetime, @Top int = 100, @MinDuration numeric(13,5) = 0)
AS
BEGIN
	  SET @From=     CAST(
     (
     STR( YEAR( @From ) ) + '-' +
     STR( MONTH( @From ) ) + '-' +
     STR( DAY( @From ) ) 
      )
     AS DATETIME
	)
	
	SET @Till= CAST(
     (
     STR( YEAR( @Till ) ) + '-' +
     STR( MONTH(@Till ) ) + '-' +
     STR( DAY( @Till ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)
	SET NOCOUNT ON
	
	SET ROWCOUNT @Top 
	
	SELECT * INTO #tmpCdrs 
		FROM Billing_CDR_Invalid bi WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt)) 
	JOIN CarrierAccount cac ON bi.CustomerID = cac.CarrierAccountID 
	WHERE  bi.Attempt BETWEEN @From AND @Till
			AND	bi.DurationInSeconds > @MinDuration
			AND ((bi.SupplierID IS NULL) OR (bi.CustomerID IS NULL))
	        AND cac.RepresentsASwitch <> 'Y'
	
	SELECT c.* FROM CDR c WITH(NOLOCK, INDEX(IX_CDR_AttemptDateTime)), #tmpCdrs bi 
	WHERE 
			c.AttemptDateTime = bi.Attempt
		AND c.IDonSwitch = bi.SwitchCdrID
		AND c.SwitchID = bi.SwitchID
		AND c.TAG = bi.Tag
		AND c.CDPN LIKE ('%' + bi.CDPN)
		AND c.DurationInSeconds = bi.DurationInSeconds
		
	
	DROP TABLE #tmpCdrs
	
	RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_BlockedAttempts]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: Ali Youness
-- =============================================
CREATE PROCEDURE [dbo].[SP_TrafficStats_BlockedAttempts]
    
    @FromDateTime  datetime,
	@ToDateTime    datetime,
    @CustomerID   varchar(10) = NULL,
    @OurZoneID 	  INT = NULL,
	@SwitchID	  tinyInt = NULL,
    @GroupByNumber CHAR(1) = 'N'
WITH RECOMPILE
AS
BEGIN 
SET NOCOUNT ON

if @CustomerID IS NULL
	SELECT  
		OurZoneID,
		Count (*) AS BlockAttempt, 
		ReleaseCode,
		ReleaseSource,
		CustomerID,
		Min(Attempt) AS FirstCall,
		Max(Attempt) AS LastCall,
		case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END AS PhoneNumber,
		case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END AS CLI    
	FROM  Billing_CDR_Invalid  WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt))
	WHERE
			Attempt Between @FromDateTime And @ToDateTime
		AND DurationInSeconds = 0
		AND SupplierID IS NULL
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
	GROUP BY ReleaseCode,ReleaseSource,OurZoneID,CustomerID,case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END, case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END
	ORDER BY Count (*) DESC
else
	SELECT  
		OurZoneID,
		Count (*) AS BlockAttempt, 
		ReleaseCode,
		ReleaseSource,
		CustomerID,
		MIN(Attempt) AS FirstCall,
		Max(Attempt) AS LastCall,
		case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END AS PhoneNumber,
		case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END AS CLI    
	FROM  Billing_CDR_Invalid  WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt),INDEX(IX_Billing_CDR_InValid_Customer))
	WHERE
			Attempt Between @FromDateTime And @ToDateTime
		AND CustomerID = @CustomerID
		AND DurationInSeconds = 0
		AND SupplierID IS NULL
		AND (@SwitchID IS NULL OR  SwitchID = @SwitchID)
		AND (@OurZoneID IS NULL OR  OurZoneID = @OurZoneID)		
	GROUP BY ReleaseCode,ReleaseSource,OurZoneID,CustomerID,case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END, case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END
	ORDER BY Count (*) DESC

END
GO
/****** Object:  StoredProcedure [dbo].[rpt_CDRDetails]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rpt_CDRDetails](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@FromDuration float,
	@ToDuration float
	)
WITH RECOMPILE
	AS 
		SET @FromDate=     CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	

 SELECT TOP 1000 * FROM Billing_CDR_Invalid bci WITH(NOLOCK,INDEX(IX_Billing_CDR_Invalid_Attempt),INDEX(IX_Billing_CDR_Invalid_Supplier),INDEX(IX_Billing_CDR_Invalid_Customer))  
     WHERE 1=1
        AND (Attempt Between @FromDate And @ToDate)
		AND (@CustomerID IS NULL OR CustomerID = @CustomerID)
		AND (@SupplierID IS NULL OR SupplierID = @SupplierID)
		AND (@FromDuration IS NULL OR DurationInSeconds >= @FromDuration)
        AND (@ToDuration IS NULL OR DurationInSeconds <= @ToDuration)	           
RETURN
GO
/****** Object:  StoredProcedure [dbo].[bp_MissMappedCdrsCarrier]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*

 */
CREATE PROCEDURE [dbo].[bp_MissMappedCdrsCarrier](@Carrier varchar(50),@From datetime, @Till datetime, @Top int = 100, @MinDuration numeric(13,5) = 0)
AS
BEGIN
	  SET @From=     CAST(
     (
     STR( YEAR( @From ) ) + '-' +
     STR( MONTH( @From ) ) + '-' +
     STR( DAY( @From ) ) 
      )
     AS DATETIME
	)
	
	SET @Till= CAST(
     (
     STR( YEAR( @Till ) ) + '-' +
     STR( MONTH(@Till ) ) + '-' +
     STR( DAY( @Till ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)
	SET NOCOUNT ON
	
	SET ROWCOUNT @Top 
	
	SELECT * INTO #tmpCdrs 
		FROM Billing_CDR_Invalid bi WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt)) 
		WHERE
			bi.Attempt BETWEEN @From AND @Till
			AND	bi.DurationInSeconds > @MinDuration
			AND (bi.SupplierID IS NULL OR bi.CustomerID IS NULL)
	
	SELECT c.* FROM CDR c WITH(NOLOCK, INDEX(IX_CDR_AttemptDateTime)), #tmpCdrs bi 
	WHERE 
			c.AttemptDateTime = bi.Attempt
		AND c.IDonSwitch = bi.SwitchCdrID
		AND c.SwitchID = bi.SwitchID
		AND c.TAG = bi.Tag
		AND c.CDPN = bi.CDPN
		AND c.DurationInSeconds = bi.DurationInSeconds
		AND ( (c.IN_CARRIER = '' AND c.OUT_CARRIER = @Carrier) OR (c.OUT_CARRIER = '' AND c.IN_CARRIER = @Carrier))
	
	DROP TABLE #tmpCdrs
	
	RETURN 0
END
GO
/****** Object:  StoredProcedure [dbo].[bp_MissOurZone]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_MissOurZone](
    @From         DATETIME,
    @Till         DATETIME,
    @Top          INT = 100,
    @MinDuration  NUMERIC(13, 5) = 0,
    @CustomerID VARCHAR(10) = NULL ,
    @SupplierID VARCHAR(10) = NULL 
)
AS
BEGIN
	  SET @From=     CAST(
     (
     STR( YEAR( @From ) ) + '-' +
     STR( MONTH( @From ) ) + '-' +
     STR( DAY( @From ) ) 
      )
     AS DATETIME
	)
	
	SET @Till= CAST(
     (
     STR( YEAR( @Till ) ) + '-' +
     STR( MONTH(@Till ) ) + '-' +
     STR( DAY( @Till ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)

	SET NOCOUNT ON
	
	SET ROWCOUNT @Top 
	
	SELECT * FROM Billing_CDR_Invalid bci WITH(NOLOCK,INDEX(IX_Billing_CDR_Invalid_Attempt))
			JOIN CarrierAccount cac ON  bci.CustomerID = cac.CarrierAccountID   
	WHERE  bci.Attempt BETWEEN @From AND @Till  
			AND (@CustomerID IS NULL OR bci.CustomerID = @CustomerID)
	        AND (@SupplierID IS NULL OR bci.SupplierID = @SupplierID)
	        AND bci.DurationInSeconds > @MinDuration
	        AND bci.CustomerID IS NOT NULL
	        AND bci.SupplierID IS NOT NULL
	        AND bci.OurZoneID IS NULL
	        AND cac.RepresentsASwitch <> 'Y'
	        AND cac.IsPassThroughCustomer <> 'Y'
			AND cac.IsPassThroughSupplier <> 'Y'							
						
	RETURN 
END
GO
/****** Object:  StoredProcedure [dbo].[bp_MissSupplierZone]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_MissSupplierZone](
    @From         DATETIME,
    @Till         DATETIME,
    @Top          INT = 100,
    @MinDuration  NUMERIC(13, 5) = 0,
    @CustomerID VARCHAR(10) = NULL ,
    @SupplierID VARCHAR(10) = NULL 
)
AS
BEGIN
    
    SET @From = CAST(
     (
     STR( YEAR( @From ) ) + '-' +
     STR( MONTH( @From ) ) + '-' +
     STR( DAY( @From ) ) 
      )
     AS DATETIME
	)
	
	SET @Till= CAST(
     (
     STR( YEAR( @Till ) ) + '-' +
     STR( MONTH(@Till ) ) + '-' +
     STR( DAY( @Till ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)
	SET NOCOUNT ON
	
	SET ROWCOUNT @Top 
	
	SELECT * FROM Billing_CDR_Invalid bci WITH(NOLOCK,INDEX(IX_Billing_CDR_Invalid_Attempt))
		JOIN CarrierAccount cac ON bci.CustomerID = cac.CarrierAccountID
		JOIN CarrierAccount cas ON bci.SupplierID = cas.CarrierAccountID
	WHERE  bci.Attempt BETWEEN @From AND @Till
        AND (@CustomerID IS NULL OR bci.CustomerID = @CustomerID)
        AND (@SupplierID IS NULL OR bci.SupplierID = @SupplierID)
        AND bci.DurationInSeconds > @MinDuration
        AND bci.CustomerID IS NOT NULL
        AND bci.SupplierID IS NOT NULL
        AND bci.SupplierZoneID IS NULL 		
        AND cac.RepresentsASwitch <> 'Y'
        AND cas.RepresentsASwitch <> 'Y'
		AND cac.IsPassThroughCustomer <> 'Y' 
		AND cac.IsPassThroughSupplier <> 'Y'
	
	RETURN
END
GO
/****** Object:  StoredProcedure [dbo].[bp_MissingCDPN]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_MissingCDPN](@From datetime, @Till datetime, @Top int = 100, @MinDuration numeric(13,5) = 0)
AS
BEGIN
	  SET @From=     CAST(
     (
     STR( YEAR( @From ) ) + '-' +
     STR( MONTH( @From ) ) + '-' +
     STR( DAY( @From ) ) 
      )
     AS DATETIME
	)
	
	SET @Till= CAST(
     (
     STR( YEAR( @Till ) ) + '-' +
     STR( MONTH(@Till ) ) + '-' +
     STR( DAY( @Till ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)
	SET NOCOUNT ON
	
	SET ROWCOUNT @Top 
	
    SELECT * 
    FROM Billing_CDR_Invalid bci  WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt)) 
    JOIN CarrierAccount cac ON bci.CustomerID = cac.CarrierAccountID 
    WHERE bci.DurationInSeconds > @MinDuration
			AND bci.Attempt BETWEEN @From AND @Till
			AND bci.CustomerID IS NOT NULL 
			AND bci.SupplierID IS NOT NULL           
			AND bci.CDPN IS NULL
			AND cac.RepresentsASwitch <> 'Y'
			AND cac.IsPassThroughCustomer <> 'Y'
			AND cac.IsPassThroughSupplier <> 'Y'
	RETURN 
END
GO
/****** Object:  StoredProcedure [dbo].[bp_MissMapped]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
 * Get CDRs that were considered invalid with unknown Customer or Supplier 
 */
CREATE PROCEDURE [dbo].[bp_MissMapped](@From datetime, @Till datetime, @Top int = 50, @MinDuration numeric(13,5) = 0)
AS
BEGIN
	  SET @From= CAST(
     (
		 STR( YEAR( @From ) ) + '-' +
     STR( MONTH( @From ) ) + '-' +
     STR( DAY( @From ) ) 
      )
     AS DATETIME
	)
	
	SET @Till= CAST(
     (
     STR( YEAR( @Till ) ) + '-' +
     STR( MONTH(@Till ) ) + '-' +
     STR( DAY( @Till ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)
	SET NOCOUNT ON
	
	SET ROWCOUNT @Top 
	
	SELECT bi.* INTO #tmpCustomer
		FROM Billing_CDR_Invalid bi WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt)) 
	WHERE  bi.Attempt BETWEEN @From AND @Till
			AND	bi.DurationInSeconds > @MinDuration
			AND bi.CustomerID IS NULL

	SELECT bi.* INTO #tmpSupplier
		FROM Billing_CDR_Invalid bi WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt)) 
		JOIN CarrierAccount cac ON (cac.CarrierAccountID = bi.CustomerID AND cac.RepresentsASwitch <> 'Y')
	WHERE   
			bi.Attempt BETWEEN @From AND @Till
			AND	bi.DurationInSeconds > @MinDuration
			AND bi.SupplierID IS NULL
				
	SELECT Carrier = c.IN_CARRIER,[Type] = 'Missing Customer' 
	FROM CDR c WITH(NOLOCK, INDEX(IX_CDR_AttemptDateTime)), #tmpCustomer bi 
	WHERE 
			c.AttemptDateTime = bi.Attempt
		AND c.IDonSwitch = bi.SwitchCdrID
		AND c.SwitchID = bi.SwitchID
		AND c.TAG = bi.Tag
		AND c.CDPN = bi.CDPN
		AND c.DurationInSeconds = bi.DurationInSeconds
	GROUP BY c.IN_CARRIER
	
	UNION 
	
	SELECT Carrier = c.OUT_CARRIER,[Type] = 'Missing Supplier' 
	FROM CDR c WITH(NOLOCK, INDEX(IX_CDR_AttemptDateTime)), #tmpSupplier bi 
	WHERE 
			c.AttemptDateTime = bi.Attempt
		AND c.IDonSwitch = bi.SwitchCdrID
		AND c.SwitchID = bi.SwitchID
		AND c.TAG = bi.Tag
		AND c.CDPN LIKE ('%' + bi.CDPN)
		AND c.DurationInSeconds = bi.DurationInSeconds
	GROUP BY c.OUT_CARRIER
	
	DROP TABLE #tmpSupplier
	DROP TABLE #tmpCustomer
	
	RETURN 
END
GO
/****** Object:  StoredProcedure [dbo].[SP_CapacityPeakMarginCheck]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create  PROCEDURE [dbo].[SP_CapacityPeakMarginCheck] 
    @TheDate DATETIME
WITH RECOMPILE
AS
BEGIN	
	SET NOCOUNT ON
DECLARE @FromDateTime DATETIME
DECLARE @ToDateTime DATETIME

   SET @FromDateTime = dbo.DateOf(@TheDate)
	
   SET @ToDateTime= CAST(
     (
     STR( YEAR(@TheDate ) ) + '-' +
     STR( MONTH(@TheDate ) ) + '-' +
     STR( DAY(@TheDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	   )	  
SELECT 
        CustomerID,
        SupplierID,
		SwitchId,
		Port_IN,
		Port_OUT,
		FirstCDRAttempt,
		Attempts,
		SuccessfulAttempts,
		DurationsInSeconds,
		UtilizationInSeconds,
		NumberOfCalls
	INTO #CarrierTraffic
	FROM TrafficStats ts WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst,IX_TrafficStats_Customer))
	WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		
	   SELECT
	        DATEPART(hour,FirstCDRAttempt) AS [Hour],
	        switchid,
	        customerid,
	        supplierid,
			Port_In,
			Port_Out,
			SUM(NumberOfCalls) AS Attempts,
			SUM(SuccessfulAttempts) AS  SuccesfulAttempts,
			SUM(SuccessfulAttempts) * 100.0 /
			Cast(NULLIF(SUM(NumberOfCalls) + 0.0, 0) AS NUMERIC) AS ASR,
			SUM(DurationsInSeconds)/60.0   AS DurationsInMinutes,
			SUM(UtilizationInSeconds)/60.0   AS UtilizationsInMinutes		
	   FROM #CarrierTraffic
	   GROUP BY 
            DATEPART(hour,FirstCDRAttempt),
	        switchid,
	        customerid,
	        supplierid,
			Port_In,
			Port_Out
			    
END
GO
/****** Object:  StoredProcedure [dbo].[Ext_RebuildPrepaidMonthlyBillingTotals]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[Ext_RebuildPrepaidMonthlyBillingTotals] @Days int
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	declare @FromCallDate datetime
	declare @ToCallDate datetime

	set @FromCallDate =dbo.DateOf(dateadd(d,-@Days,getdate()))
	set @ToCallDate =dbo.DateOf(getdate())

	Exec [dbo].[bp_PrepaidDailyTotalUpdate]	@FromCallDate , @ToCallDate 

END
GO
/****** Object:  StoredProcedure [dbo].[bp_UpdateOperationQueue]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_UpdateOperationQueue]
(
	@From datetime,
	@Till datetime 
)
AS
BEGIN
	DECLARE @OperationQueue TABLE ([Type] char(1) NOT NULL, [ID] bigint NOT NULL, Operation char(1) NOT NULL)
	DECLARE @WorkingType char(1)
	DECLARE @EndFlag char(1)
	DECLARE @BeginFlag char(1) 
	SET @BeginFlag = 'B'
	SET @EndFlag = 'E'	
		
	-- Process ToD
	SET @WorkingType = 'T'
	INSERT INTO @OperationQueue 
		SELECT @WorkingType, T.ToDConsiderationID, @BeginFlag 
			FROM ToDConsideration T WITH(NOLOCK) 
			WHERE 
					(T.BeginEffectiveDate BETWEEN @From AND @Till) 
				AND 'Y' = dbo.IsToDActive(T.HolidayDate, T.WeekDay, T.BeginTime, T.EndTime, @From)
	INSERT INTO @OperationQueue 
		SELECT @WorkingType, T.ToDConsiderationID, @EndFlag 
			FROM ToDConsideration T WITH(NOLOCK) 
			WHERE 
					(T.EndEffectiveDate BETWEEN @From AND @Till) 
				OR (					
					(T.BeginEffectiveDate BETWEEN @From AND @Till) 
						AND 'N' = dbo.IsToDActive(T.HolidayDate, T.WeekDay, T.BeginTime, T.EndTime, @From)
					)
	
	-- Process Special Requests
	SET @WorkingType = 'S'
	INSERT INTO @OperationQueue 
		SELECT @WorkingType, S.SpecialRequestID, @BeginFlag 
			FROM SpecialRequest S WITH(NOLOCK)
			WHERE S.BeginEffectiveDate BETWEEN @From AND @Till
	INSERT INTO @OperationQueue 
		SELECT @WorkingType, S.SpecialRequestID, @EndFlag 
			FROM SpecialRequest S WITH(NOLOCK) 
			WHERE S.EndEffectiveDate BETWEEN @From AND @Till
	
	-- Process Route Blocks
	SET @WorkingType = 'B'
	INSERT INTO @OperationQueue 
		SELECT @WorkingType, B.RouteBlockID, @BeginFlag 
			FROM RouteBlock B WITH(NOLOCK) 
			WHERE B.BeginEffectiveDate BETWEEN @From AND @Till
	INSERT INTO @OperationQueue 
		SELECT @WorkingType, B.RouteBlockID, @EndFlag 
			FROM RouteBlock B WITH(NOLOCK) 
			WHERE B.EndEffectiveDate BETWEEN @From AND @Till

	-- Update OperationQueue
	INSERT INTO OperationQueue 
		(Type, ID, Operation, Created)
	SELECT 
		Type, ID, Operation, @From 
		FROM @OperationQueue
	
	-- Clean Up OperationQueue
	DELETE FROM @OperationQueue
	INSERT INTO @OperationQueue(Type, ID, Operation)
		SELECT Type, ID, MIN(Operation) 
			FROM OperationQueue WITH(NOLOCK)
			GROUP BY Type, ID
			HAVING Count(*) > 1  
	 
	-- DELETE Duplicate Entries
	DELETE FROM OperationQueue  
		WHERE EXISTS(SELECT * FROM @OperationQueue O WHERE O.Type = OperationQueue.Type AND O.ID = OperationQueue.ID)	

END
GO
/****** Object:  StoredProcedure [dbo].[bp_UpdateRoutesFromToD]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_UpdateRoutesFromToD]
	@Check char(1) = 'Y'

AS

	SET NOCOUNT ON

	IF @Check = 'Y' 
	BEGIN	
		DECLARE @IsRunning char(1)
		EXEC bp_IsRouteBuildRunning @IsRunning output
		IF @IsRunning = 'Y' 
		BEGIN
			PRINT 'Build Routes is already Runnning'
			RETURN 
		END 
		EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = 'Updating From ToD Considerations'
		EXEC bp_ResetRoutes @UpdateType = 'TOD'
	END	

	DECLARE @UpdateStamp datetime
	SET @UpdateStamp = getdate()

	-- Update Rates for Routes that have a ToD active
	UPDATE [Route]
		SET OurActiveRate = 
			CASE 
				WHEN T.RateType = 1 THEN [Route].OurOffPeakRate -- OffPeak
				WHEN T.RateType = 2 THEN [Route].OurWeekendRate -- Weekend 
				WHEN T.RateType = 4 THEN [Route].OurWeekendRate -- Holiday
				ELSE [Route].OurNormalRate
			END
			, Updated = @UpdateStamp
			, IsToDAffected = 'Y'
	FROM [Route], ToDConsideration T
		WHERE 
			T.IsEffective='Y'
			AND T.IsActive='Y'
			AND T.ZoneID=[Route].OurZoneID
			AND T.CustomerID = [Route].CustomerID

	-- Update Rates for Route Options that have a ToD active
	UPDATE [RouteOption]
		SET SupplierActiveRate = 
			CASE 
				WHEN T.RateType = 1 THEN [RouteOption].SupplierOffPeakRate -- OffPeak
				WHEN T.RateType = 2 THEN [RouteOption].SupplierWeekendRate -- Weekend 
				WHEN T.RateType = 4 THEN [RouteOption].SupplierWeekendRate -- Holiday
				ELSE [RouteOption].SupplierNormalRate
			END
			, Updated = @UpdateStamp
	FROM [RouteOption], ToDConsideration T
		WHERE 
			T.IsEffective='Y'
			AND T.IsActive='Y'
			AND T.ZoneID=[RouteOption].SupplierZoneID

	-- Now Set to Updated the Routes That have the Route Options Updated
	EXEC bp_SignalRouteUpdatesFromOptions @UpdateStamp=@UpdateStamp, @UpdateType='TOD'
	
	IF @Check = 'Y'
	BEGIN
		EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = NULL
	END
GO
/****** Object:  StoredProcedure [dbo].[bp_UpdateRoutingFromOverrides]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =================================================================
-- Author:		Fadi Chamieh
-- Create date: 2009-04-30
-- Updated: 2010-04-26 (Fixed Problem with Route Override On Our Zone when partially matching Supplier Zone Is Blocked)
-- Updated: 2010-05-17 (fixed blocked and onactive customers, Reverted to old way of managing overrides on our zones)
-- Description:	Updates Routes / Route Options from Route Overrides
-- =================================================================
CREATE PROCEDURE [dbo].[bp_UpdateRoutingFromOverrides] 
	@CustomerID VARCHAR(10) = NULL,
	@Code VARCHAR(20) = NULL,
	@OurZoneID INT = NULL,
	@RouteOptions VARCHAR(100) = NULL,
	@BlockedSuppliers VARCHAR(100) = NULL,
	@RouteBatch BIGINT = 25000
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @State_Blocked tinyint
	DECLARE @State_Enabled tinyint
	SET @State_Blocked = 0
	SET @State_Enabled = 1

	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	

	CREATE TABLE #tmpRO (CustomerID nvarchar(5), Code nvarchar(15), OurZoneID int, SupplierID nvarchar(5), Position INT, SupplierZoneID INT NULL)
	CREATE INDEX IDX_Customer ON #tmpRO (CustomerID)
	CREATE INDEX IDX_Code ON #tmpRO (Code)
	CREATE INDEX IDX_OurZoneID ON #tmpRO (OurZoneID)
	CREATE INDEX IDX_SupplierZoneID ON #tmpRO (SupplierZoneID)
	CREATE INDEX IDX_SupplierID ON #tmpRO (SupplierID)
	
	DECLARE @ValidCustomers TABLE(CustomerID NVARCHAR(5) PRIMARY KEY)
	INSERT INTO @ValidCustomers(CustomerID) 
		SELECT ca.CarrierAccountID FROM CarrierAccount ca 
	        WHERE ca.RoutingStatus NOT IN (@Account_Blocked, @Account_BlockedInbound)
	        AND ca.ActivationStatus NOT IN (@Account_Inactive)
	        AND ca.IsDeleted = 'N' 

	DECLARE @MaxSuppliersPerRoute INT 
	SET @MaxSuppliersPerRoute = 8

	-- Process All Routing Overrides 
	IF @CustomerID IS NULL
		BEGIN

			INSERT INTO #tmpRO (CustomerID, Code, OurZoneID, SupplierID, Position)
			SELECT 
					ro.CustomerID, ro.Code, ro.OurZoneID, 
					(SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 1) SupplierID,
					1 as Position 
				FROM RouteOverride ro WHERE ro.BlockedSuppliers IS NULL AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 1) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc)				  
			UNION
				SELECT 
					ro.CustomerID, ro.Code, ro.OurZoneID, 
					(SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 2) SupplierID,
					2 as Position 
				FROM RouteOverride ro WHERE ro.BlockedSuppliers IS NULL  AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 2) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc)  
			UNION	
				SELECT 
					ro.CustomerID, ro.Code, ro.OurZoneID, 
					(SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 3) SupplierID,
					3 as Position 
				FROM RouteOverride ro WHERE ro.BlockedSuppliers IS NULL   AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 3) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc)
			UNION
			SELECT 
					ro.CustomerID, ro.Code, ro.OurZoneID, 
					(SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 4) SupplierID,
					4 as Position 
				FROM RouteOverride ro WHERE ro.BlockedSuppliers IS NULL AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 4) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc)  
			UNION
				SELECT 
					ro.CustomerID, ro.Code, ro.OurZoneID, 
					(SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 5) SupplierID,
					5 as Position 
				FROM RouteOverride ro WHERE ro.BlockedSuppliers IS NULL  AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 5) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc)  
			UNION	
				SELECT 
					ro.CustomerID, ro.Code, ro.OurZoneID, 
					(SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 6) SupplierID,
					6 as Position 
				FROM RouteOverride ro WHERE ro.BlockedSuppliers IS NULL   AND (SELECT o.[value] FROM dbo.ParseArrayWithPosition(ro.RouteOptions, '|') o WHERE o.Position = 6) IS NOT NULL
					AND ro.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc)
			ORDER BY ro.CustomerID, ro.Code, Position

			-- Set the Matching Supplier Zones from Codes in RO
			UPDATE #tmpRO
			SET
				SupplierZoneID = cm.SupplierZoneID
			FROM #tmpRO tr, CodeMatch cm 
			WHERE tr.Code = cm.Code COLLATE Latin1_General_BIN AND tr.SupplierID = cm.SupplierID COLLATE Latin1_General_BIN

			-- Set the Matching Supplier Zones
			UPDATE #tmpRO
			SET
				SupplierZoneID = cm.SupplierZoneID
			FROM #tmpRO tr, CodeMatch cm,ZoneMatch zm 
			WHERE   tr.SupplierZoneID IS NULL      
				  AND tr.OurZoneID = zm.OurZoneID 
			      AND  zm.SupplierZoneID = cm.SupplierZoneID 
			      AND  tr.SupplierID = cm.SupplierID COLLATE Latin1_General_BIN

			-- Insert Route Overrides as Codes instead of Zones to match with supplier zones
			/*
			INSERT INTO #tmpRO (CustomerID, Code, OurZoneID, SupplierID, SupplierZoneID, Position)
			SELECT ro.CustomerID, oc.Code, ro.OurZoneID, ro.SupplierID, sc.SupplierZoneID, ro.Position 
				FROM #tmpRO ro WITH(NOLOCK, INDEX(IDX_Code), INDEX(IDX_SupplierZoneID), INDEX(IDX_SupplierID), INDEX(IDX_OurZoneID)), 
					CodeMatch oc WITH(NOLOCK, INDEX(IDX_CodeMatch_Zone)),  
					ZoneMatch zm WITH(NOLOCK, INDEX(IX_ZoneMatch_SupplierZoneID)),
					CodeMatch sc WITH(NOLOCK, INDEX(IDX_CodeMatch_Code), INDEX(IDX_CodeMatch_Supplier))
				WHERE 
					ro.SupplierZoneID IS NULL
					AND ro.Code = '*ALL*' 
					AND oc.SupplierZoneID = ro.OurZoneID 
					AND zm.OurZoneID = ro.OurZoneID
					AND sc.Code = oc.Code
					AND ro.SupplierID = sc.SupplierID COLLATE Latin1_General_CI_AI
					AND zm.SupplierZoneID = sc.SupplierZoneID
			*/
			
			-- Delete Where Supplier Zone ID is not defined
			DELETE FROM #tmpRO WHERE SupplierZoneID IS NULL

			DECLARE @MinRouteID bigint
			DECLARE @MaxRouteID bigint
			DECLARE @CurrentRouteID bigint
			DECLARE @ProcessedRoutes bigint 
			DECLARE @TotalRoutes bigint
			DECLARE @NextRouteID bigint
			DECLARE @Message VARCHAR(4000)
				
			SELECT @MinRouteID = Min(RouteID), @MaxRouteID = Max(RouteID), @TotalRoutes = Count(*) FROM [Route]

			SET @CurrentRouteID = @MinRouteID
			
			SET @ProcessedRoutes = 0
			
			WHILE @CurrentRouteID <= @MaxRouteID
			BEGIN
				
				SET @NextRouteID = @CurrentRouteID + @RouteBatch - 1

				BEGIN TRANSACTION 
				
				/*************************************
				* Process Blocks 
				*************************************/
				UPDATE RouteOption SET [State] = @State_Blocked, Updated = GETDATE()
					FROM [Route] r, RouteOverride rov, RouteOption WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
					WHERE r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
						AND rov.BlockedSuppliers IS NOT NULL 
						AND rov.CustomerID = r.CustomerID
						AND rov.Code = r.Code					 
						AND RouteOption.RouteID = r.RouteID
						AND RouteOption.SupplierID IN (SELECT pa.[value] FROM dbo.ParseArray(rov.BlockedSuppliers, '|') pa)

				UPDATE RouteOption SET [State] = @State_Blocked, Updated = GETDATE()
					FROM [Route] r, RouteOverride rov, RouteOption WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
					WHERE r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
						AND rov.BlockedSuppliers IS NOT NULL 
						AND rov.CustomerID = r.CustomerID
						AND rov.OurZoneID = r.OurZoneID 
						AND RouteOption.RouteID = r.RouteID
						AND RouteOption.SupplierID IN (SELECT pa.[value] FROM dbo.ParseArray(rov.BlockedSuppliers, '|') pa)

				COMMIT

				/*************************************
				* Delete Overrides / Options 
				*************************************/
				BEGIN TRANSACTION 
			
				DELETE RouteOption 
					FROM [Route] r, RouteOverride rov, RouteOption WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
					WHERE 
							r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
						AND rov.RouteOptions IS NOT NULL 
						AND rov.CustomerID = r.CustomerID
						AND rov.Code = r.Code
						AND RouteOption.RouteID = r.RouteID

				DELETE RouteOption 
					FROM [Route] r, RouteOverride rov, RouteOption WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
					WHERE 
							r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
						AND rov.RouteOptions IS NOT NULL 
						AND rov.CustomerID = r.CustomerID
						AND rov.OurZoneID = r.OurZoneID 
						AND RouteOption.RouteID = r.RouteID
				
				COMMIT
				
				/*************************************
				* Insert Overrides / Options 
				*************************************/

				BEGIN TRANSACTION 
				
				-- Insert Override Options (Code based)
				INSERT INTO RouteOption (RouteID, SupplierID, SupplierZoneID, SupplierActiveRate, SupplierNormalRate, SupplierServicesFlag, Priority, [State], NumberOfTries, Updated)
					SELECT r.RouteID, ca.CarrierAccountID, rov.SupplierZoneID, -1, -1, ca.ServicesFlag, @MaxSuppliersPerRoute - rov.Position + 1, 1, 1, GETDATE()
					  FROM [Route] r, #tmpRO rov, CarrierAccount ca  
					WHERE 
							r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
						AND rov.CustomerID = r.CustomerID COLLATE Latin1_General_BIN
						AND rov.Code = r.Code COLLATE Latin1_General_BIN
						AND ca.CarrierAccountID = rov.SupplierID COLLATE Latin1_General_BIN
						AND ca.ActivationStatus <> @Account_Inactive
						AND ca.RoutingStatus <> @Account_Blocked
						AND ca.RoutingStatus <> @Account_BlockedOutbound
						AND ca.IsDeleted = 'N'

				-- Insert Override Options (Zone based)
				INSERT INTO RouteOption (RouteID, SupplierID, SupplierZoneID, SupplierActiveRate, SupplierNormalRate, SupplierServicesFlag, Priority, [State], NumberOfTries, Updated)
					SELECT r.RouteID, ca.CarrierAccountID, rov.SupplierZoneID, -1, -1, ca.ServicesFlag, @MaxSuppliersPerRoute - rov.Position + 1, 1, 1, GETDATE()
					  FROM [Route] r, #tmpRO rov, CarrierAccount ca  
					WHERE 
							r.RouteID BETWEEN @CurrentRouteID AND @NextRouteID
						AND rov.CustomerID = r.CustomerID COLLATE Latin1_General_BIN
						AND rov.OurZoneID = r.OurZoneID 
						AND ca.CarrierAccountID = rov.SupplierID COLLATE Latin1_General_BIN
						AND ca.ActivationStatus <> @Account_Inactive
						AND ca.RoutingStatus <> @Account_Blocked
						AND ca.RoutingStatus <> @Account_BlockedOutbound		
						AND ca.IsDeleted = 'N'		
				
				COMMIT

				SET @ProcessedRoutes = @ProcessedRoutes + @RouteBatch
				IF @ProcessedRoutes > @TotalRoutes SET @ProcessedRoutes = @TotalRoutes
				SET @CurrentRouteID = @CurrentRouteID + @RouteBatch
				
				SET @Message = cast(@ProcessedRoutes AS varchar) + ' / ' + cast(@TotalRoutes AS varchar)
				EXEC bp_SetSystemMessage 'BuildRoutes: Route Overrides Processing', @Message
				
			END		

		END
	-- A particular Route Override
	ELSE
		BEGIN
				
			DECLARE @UpdateStamp DATETIME 
			SET @UpdateStamp = GETDATE()
					
			/*************************************
			* Process Blocks 
			*************************************/
			IF(@BlockedSuppliers IS NOT NULL)
				BEGIN
					IF @Code <> '*ALL*'
						UPDATE RouteOption SET [State] = @State_Blocked, Updated = GETDATE()
							FROM [Route] r, RouteOption WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
							WHERE    
									r.CustomerID = @CustomerID
									AND r.Code = @Code					 
									AND RouteOption.RouteID = r.RouteID
									AND RouteOption.SupplierID IN (SELECT pa.[value] FROM dbo.ParseArray(@BlockedSuppliers, '|') pa)

					IF @OurZoneID > 0
						UPDATE RouteOption SET [State] = @State_Blocked, Updated = GETDATE()
							FROM [Route] r, RouteOption WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
							WHERE      
									r.CustomerID = @CustomerID
									AND r.OurZoneID = @OurZoneID 
									AND RouteOption.RouteID = r.RouteID
									AND RouteOption.SupplierID IN (SELECT pa.[value] FROM dbo.ParseArray(@BlockedSuppliers, '|') pa)			
				END
			ELSE
				BEGIN
				
					INSERT INTO #tmpRO (CustomerID, Code, OurZoneID, SupplierID, Position)
						SELECT	@CustomerID, @Code, @OurZoneID, pawp.[value], pawp.Position
						FROM dbo.ParseArrayWithPosition(@RouteOptions, '|') pawp 
				
					-- Set the Matching Supplier Zones
					UPDATE #tmpRO
					SET
						SupplierZoneID = cm.SupplierZoneID
					FROM #tmpRO tr, CodeMatch cm WHERE tr.Code = cm.Code COLLATE Latin1_General_BIN AND tr.SupplierID = cm.SupplierID COLLATE Latin1_General_BIN

					-- Set the Matching Supplier Zones
					UPDATE #tmpRO
					SET
						SupplierZoneID = cm.SupplierZoneID
					FROM #tmpRO tr, CodeMatch cm,ZoneMatch zm 
					WHERE   tr.SupplierZoneID IS NULL      
						  AND tr.OurZoneID = zm.OurZoneID 
						  AND  zm.SupplierZoneID = cm.SupplierZoneID 
						  AND  tr.SupplierID = cm.SupplierID COLLATE Latin1_General_BIN


					-- Delete Where Supplier Zone ID is not defined
					DELETE FROM #tmpRO WHERE SupplierZoneID IS NULL

					/*************************************
					* Delete Overrides / Options 
					*************************************/
					IF @Code <> '*ALL*'
						DELETE RouteOption 
							FROM [Route] r, RouteOverride rov, RouteOption WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
							WHERE 
									r.CustomerID = @CustomerID
								AND r.Code = @Code
								AND RouteOption.RouteID = r.RouteID
					IF @OurZoneID > 0
						DELETE RouteOption 
							FROM [Route] r,RouteOption WITH(NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
							WHERE 
								   r.CustomerID = @CustomerID
								AND r.OurZoneID = @OurZoneID
								AND RouteOption.RouteID = r.RouteID
					
					/*************************************
					* Insert Overrides / Options 
					*************************************/
					-- Insert Override Options (Code based)
					IF @Code <> '*ALL*'
						INSERT INTO RouteOption (RouteID, SupplierID, SupplierZoneID, SupplierActiveRate, SupplierNormalRate, SupplierServicesFlag, Priority, [State], NumberOfTries, Updated)
							SELECT r.RouteID, ca.CarrierAccountID, rov.SupplierZoneID, -1, -1, ca.ServicesFlag, @MaxSuppliersPerRoute - rov.Position + 1, 1, 1, GETDATE()
							  FROM [Route] r, #tmpRO rov, CarrierAccount ca  
							WHERE 
								    rov.CustomerID = r.CustomerID COLLATE Latin1_General_BIN
								AND rov.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc)
								AND rov.Code = r.Code COLLATE Latin1_General_BIN
								AND ca.CarrierAccountID = rov.SupplierID COLLATE Latin1_General_BIN
								AND ca.ActivationStatus <> @Account_Inactive
								AND ca.RoutingStatus <> @Account_Blocked
								AND ca.RoutingStatus <> @Account_BlockedOutbound
								AND ca.IsDeleted = 'N'
								
					IF @OurZoneID > 0
						-- Insert Override Options (Zone based)
						INSERT INTO RouteOption (RouteID, SupplierID, SupplierZoneID, SupplierActiveRate, SupplierNormalRate, SupplierServicesFlag, Priority, [State], NumberOfTries, Updated)
							SELECT r.RouteID, ca.CarrierAccountID, rov.SupplierZoneID, -1, -1, ca.ServicesFlag, @MaxSuppliersPerRoute - rov.Position + 1, 1, 1, GETDATE()
							  FROM [Route] r, #tmpRO rov, CarrierAccount ca  
							WHERE 
								    rov.CustomerID = r.CustomerID COLLATE Latin1_General_BIN
								AND rov.CustomerID COLLATE Latin1_General_BIN IN (SELECT vc.CustomerID FROM @ValidCustomers vc) 
								AND rov.OurZoneID = r.OurZoneID 
								AND ca.CarrierAccountID = rov.SupplierID COLLATE Latin1_General_BIN
								AND ca.ActivationStatus <> @Account_Inactive
								AND ca.RoutingStatus <> @Account_Blocked
								AND ca.RoutingStatus <> @Account_BlockedOutbound
								AND ca.IsDeleted = 'N'
				END	
		
				EXEC bp_SignalRouteUpdatesFromOptions
					@UpdateStamp = @UpdateStamp,
					@UpdateType = 'RouteOverride'
		
		END

	DROP TABLE #tmpRO
END
GO
/****** Object:  StoredProcedure [dbo].[SP_CapacityAndConnectivity_MainReport_WithPort]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[SP_CapacityAndConnectivity_MainReport_WithPort] 
    @FromDateTime	datetime,
	@ToDateTime		datetime,
	@CarrierAccountID varchar(10),
	@SupplierID		varchar(10) = NULL,
    @SwitchID		tinyInt = NULL,
    @OurZoneID 		INT = NULL,
    @CodeGroup		varchar(10) = NULL,
    @ConnectionType VARCHAR(20) = 'VoIP',
    @PortInOut		VARCHAR(50) = NULL,
	@SelectPort		int = NULL,
	@GroupByGateWay CHAR(1) = 'N',
	@GateWayName VARCHAR(255) = NULL,
	@RemoveInterconnectedData CHAR(1) = 'N',
	@GroupPortOption CHAR(1)       --- I (in) O (Out) B (both)
WITH RECOMPILE
AS
BEGIN	
	SET NOCOUNT ON

	DECLARE @Connectivities TABLE 
	(
		GateWay VARCHAR(250),
		Details VARCHAR(MAX),
		Date SMALLDATETIME,
		NumberOfChannels_In INT,
		NumberOfChannels_Out int,
		NumberOfChannels_Total int,
		Margin_Total INT,
		DetailList VARCHAR(MAX),
		InterconnectedSwithes CHAR(1)
	)
	
	SET @FromDateTime = dbo.DateOf(@FromDateTime)
	
	SET @ToDateTime= CAST(
     (
     STR( YEAR( @ToDateTime ) ) + '-' +
     STR( MONTH(@ToDateTime ) ) + '-' +
     STR( DAY( @ToDateTime ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	;
	
	DECLARE @ConnectivityValue TINYINT
	SELECT  @ConnectivityValue = Value FROM Enumerations e WHERE e.Enumeration = 'TABS.ConnectionType' AND e.[Name] = @ConnectionType; 

DECLARE @IPpattern VARCHAR(10)
SET @IPpattern = '%.%.%.%'


INSERT INTO @Connectivities
 SELECT c.*, ',' + c.Details + ',' AS DetailList  ,'N' 
 from dbo.GetDailyConnectivity(
 		@ConnectivityValue,
 		@CarrierAccountID,
 		@SwitchID,
 		@GroupByGateWay, 
 		@FromDateTime,
 		@ToDateTime,
        'N') c   
 IF @RemoveInterconnectedData = 'Y'
 INSERT INTO @Connectivities
 SELECT c.*, ',' + c.Details + ',' AS DetailList  ,'Y' 
 from dbo.GetDailyConnectivity(
 		null,
 		null,
 		@SwitchID,
 		'N', 
 		@FromDateTime,
 		@ToDateTime,
        'Y') c ; 



-- Create Customer Stats
SELECT 
		SwitchId,
		Port_IN,
		Port_OUT,
		CustomerID,
		OurZoneID,
		SupplierID,
		FirstCDRAttempt,
		Attempts,
		SuccessfulAttempts,
		DurationsInSeconds,
		UtilizationInSeconds,
		NumberOfCalls
	INTO #CarrierTraffic
	FROM TrafficStats ts WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst,IX_TrafficStats_Customer))
	WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND ts.CustomerID = @CarrierAccountID
		AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)      
		AND (@SupplierID is null or TS.SupplierID = @SupplierID)
	
-- Create Supplier Stats
INSERT INTO #CarrierTraffic
	SELECT 
		SwitchId,
		Port_IN,
		Port_OUT,
		CustomerID,
		OurZoneID,
		SupplierID,
		FirstCDRAttempt,
		Attempts,
		SuccessfulAttempts,
		DurationsInSeconds,
		UtilizationInSeconds,
		NumberOfCalls
	 FROM TrafficStats ts WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst,IX_TrafficStats_Supplier))
	WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND ts.SupplierID = @CarrierAccountID
		AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)      
		AND (@SupplierID is null or TS.SupplierID = @SupplierID)

IF @ConnectionType LIKE 'VoIP'
BEGIN 	
WITH InterConnect AS 
(
	SELECT * FROM @Connectivities WHERE InterconnectedSwithes = 'N'
)

	SELECT
			datepart(hour,FirstCDRAttempt) AS Period,
			dbo.DateOf(FirstCDRAttempt) AS [Date],
			GateWay = CASE WHEN  @GroupByGateWay = 'Y' OR @GateWayName IS NOT NULL THEN DC.GateWay
			               ELSE  null END,
			--- Port Grouping
			
			PortIn = CASE WHEN  @GroupPortOption = 'I' OR @GroupPortOption = 'B' THEN ts.Port_IN
			               ELSE  null END,
			PortOut = CASE WHEN @GroupPortOption = 'O' OR @GroupPortOption = 'B' THEN ts.Port_Out
			               ELSE  null END,
			---------
			
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern  THEN ts.NumberOfCalls ELSE 0 END)   AS InAttempts,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END)   AS OutAttempts,
	        
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern  THEN ts.SuccessfulAttempts ELSE 0 END) AS  InSuccesfulAttempts,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) AS  OutSuccesfulAttempts,
	        
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			Cast(NULLIF(SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END) + 0.0, 0) AS NUMERIC) AS InASR,
	        
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			cast(NULLIF(SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END)+ 0.0, 0) AS NUMERIC) AS OutASR,
	       
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS InDurationsInMinutes,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS OutDurationsInMinutes, 
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS InUtilizationsInMinutes,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS OutUtilizationsInMinutes,
			NumberOfChannels_In = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_In) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_In) FROM InterConnect CSC WHERE CSC.Date = 	dbo.DateOf(FirstCDRAttempt) )
								       ELSE 0 END,
			NumberOfChannels_Out = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_Out) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_Out) FROM InterConnect CSC WHERE CSC.Date = 	dbo.DateOf(FirstCDRAttempt) )
								       ELSE 0 END,
			NumberOfChannels_Total = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_Total) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_Total) FROM InterConnect CSC WHERE CSC.Date = 	dbo.DateOf(FirstCDRAttempt) )
								       ELSE 0 END
	   FROM #CarrierTraffic AS TS -- WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
			LEFT JOIN Zone AS Z WITH(NOLOCK) ON TS.OurZoneID = Z.ZoneID  
			LEFT JOIN @Connectivities DC ON  DC.Date = dbo.DateOf(FirstCDRAttempt) AND DC.InterconnectedSwithes = 'N'
									  AND (@GateWayName IS NULL OR (Dc.gateway = @GateWayName))
		    LEFT JOIN @Connectivities DCI ON  DCI.Date = dbo.DateOf(FirstCDRAttempt) AND DCI.InterconnectedSwithes = 'Y'	
	   WHERE   
				(@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
			AND	1 = (CASE  WHEN @SelectPort  = 0 AND TS.CustomerID = @CarrierAccountID AND TS.Port_In like @PortInOut THEN 1     
		                   WHEN @SelectPort  = 1 AND TS.SupplierID = @CarrierAccountID AND TS.Port_Out like @PortInOut THEN 1 
		                   WHEN (@SelectPort  = 2 AND TS.CustomerID = @CarrierAccountID AND TS.Port_In like @PortInOut)
		                        OR (@SelectPort  = 2 AND TS.SupplierID = @CarrierAccountID AND TS.Port_Out like @PortInOut) THEN 1 
		                   ELSE 0 END 
			) 
		    AND ( DC.DetailList IS NULL OR
		    	  (
		    	  	     TS.CustomerID = @CarrierAccountID 
		    	  	AND  DC.DetailList LIKE ( CASE @GroupByGateWay WHEN 'Y' THEN ('%,' + TS.Port_IN + ',%') ELSE '%%' END)
		    	    AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_Out + ',%')
		    	  )
		          OR 
		          (
		          	     TS.SupplierID = @CarrierAccountID 
		            AND  DC.DetailList LIKE ( CASE @GroupByGateWay WHEN 'Y' THEN ('%,' + TS.Port_Out + ',%') ELSE '%%' END)
		             AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_In + ',%'))
		        )
		GROUP BY datepart(hour,FirstCDRAttempt)
		        ,dbo.DateOf(TS.FirstCDRAttempt)
			    ,CASE WHEN  @GroupByGateWay = 'Y' OR @GateWayName IS NOT NULL THEN DC.GateWay
			          ELSE  null END
			    ,CASE WHEN  @GroupPortOption = 'I' OR @GroupPortOption = 'B' THEN ts.Port_IN
			               ELSE  null END
			    ,CASE WHEN @GroupPortOption = 'O' OR @GroupPortOption = 'B' THEN ts.Port_Out
			               ELSE  null END
			          

END 
IF @ConnectionType LIKE 'TDM'
BEGIN 
WITH InterConnect AS 
(
	SELECT * FROM @Connectivities WHERE InterconnectedSwithes = 'N'
)

SELECT
			datepart(hour,FirstCDRAttempt) AS Period,
			dbo.DateOf(FirstCDRAttempt) AS [Date],
			GateWay = CASE WHEN  @GroupByGateWay = 'Y' OR @GateWayName IS NOT NULL THEN DC.GateWay
			               ELSE  null END,
			--- Port Grouping
			
			PortIn = CASE WHEN  @GroupPortOption = 'I' OR @GroupPortOption = 'B' THEN ts.Port_IN
			               ELSE  null END,
			PortOut = CASE WHEN @GroupPortOption = 'O' OR @GroupPortOption = 'B' THEN ts.Port_Out
			               ELSE  null END,
			---------
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not LIKE @IPpattern  THEN ts.NumberOfCalls ELSE 0 END)   AS InAttempts,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END)   AS OutAttempts,
	        
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not LIKE @IPpattern  THEN ts.SuccessfulAttempts ELSE 0 END) AS  InSuccesfulAttempts,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) AS  OutSuccesfulAttempts,
	        
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not  LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			Cast(NULLIF(SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END) + 0.0, 0) AS NUMERIC) AS InASR,
	        
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			cast(NULLIF(SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END)+ 0.0, 0) AS NUMERIC) AS OutASR,
	       
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not LIKE @IPpattern THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS InDurationsInMinutes,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not  LIKE @IPpattern THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS OutDurationsInMinutes, 
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not  LIKE @IPpattern THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS InUtilizationsInMinutes,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not  LIKE @IPpattern THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS OutUtilizationsInMinutes,
			NumberOfChannels_In = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_In) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_In) FROM InterConnect CSC WHERE CSC.Date = dbo.DateOf(FirstCDRAttempt) )
								       ELSE 0 END,
			NumberOfChannels_Out = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_Out) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_Out) FROM InterConnect CSC WHERE CSC.Date = dbo.DateOf(FirstCDRAttempt) )
								       ELSE 0 END,
			NumberOfChannels_Total = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_Total) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_Total) FROM InterConnect CSC WHERE CSC.Date = dbo.DateOf(FirstCDRAttempt))
								       ELSE 0 END
	   FROM #CarrierTraffic AS TS -- WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
			LEFT JOIN Zone AS Z WITH(NOLOCK) ON TS.OurZoneID = Z.ZoneID  
			LEFT JOIN @Connectivities DC ON  DC.Date = dbo.DateOf(FirstCDRAttempt) AND DC.InterconnectedSwithes = 'N'
									  AND (@GateWayName IS NULL OR (Dc.gateway = @GateWayName)) 
		    LEFT JOIN @Connectivities DCI ON  DCI.Date = dbo.DateOf(FirstCDRAttempt) AND DCI.InterconnectedSwithes = 'Y'	                                   
	   WHERE   
				(@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
			AND	1 = (CASE  WHEN @SelectPort  = 0 AND TS.CustomerID = @CarrierAccountID AND TS.Port_In like @PortInOut THEN 1     
		                   WHEN @SelectPort  = 1 AND TS.SupplierID = @CarrierAccountID AND TS.Port_Out like @PortInOut THEN 1 
		                   WHEN (@SelectPort  = 2 AND TS.CustomerID = @CarrierAccountID AND TS.Port_In like @PortInOut)
		                        OR (@SelectPort  = 2 AND TS.SupplierID = @CarrierAccountID AND TS.Port_Out like @PortInOut) THEN 1 
		                   ELSE 0 END 
			) 
		    AND ( DC.DetailList IS NULL OR
		    	  (
		    	  	     TS.CustomerID = @CarrierAccountID 
		    	  	AND  DC.DetailList LIKE ( CASE @GroupByGateWay WHEN 'Y' THEN ('%,' + TS.Port_IN + ',%') ELSE '%%' END)
		    	    AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_Out + ',%')
		    	  )
		          OR 
		          (
		          	     TS.SupplierID = @CarrierAccountID 
		            AND  DC.DetailList LIKE ( CASE @GroupByGateWay WHEN 'Y' THEN ('%,' + TS.Port_Out + ',%') ELSE '%%' END)
		             AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_In + ',%')
		    )
		        )
			    
		GROUP BY datepart(hour,FirstCDRAttempt)
		        ,dbo.DateOf(TS.FirstCDRAttempt)
			    ,CASE WHEN  @GroupByGateWay = 'Y' OR @GateWayName IS NOT NULL THEN DC.GateWay
			          ELSE  null END
			    ,CASE WHEN  @GroupPortOption = 'I' OR @GroupPortOption = 'B' THEN ts.Port_IN
			               ELSE  null END
			    ,CASE WHEN @GroupPortOption = 'O' OR @GroupPortOption = 'B' THEN ts.Port_Out
			               ELSE  null END
END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_CapacityAndConnectivity_InterConnectReport]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[SP_CapacityAndConnectivity_InterConnectReport] 
    @FromDateTime	datetime,
	@ToDateTime		datetime,
	@CarrierAccountID varchar(10),
	@SupplierID		varchar(10) = NULL,
    @SwitchID		tinyInt = NULL,
    @OurZoneID 		INT = NULL,
    @CodeGroup		varchar(10) = NULL,
    @ConnectionType VARCHAR(20) = 'VoIP',
    @PortInOut		VARCHAR(50) = NULL,
	@SelectPort		int = NULL,
	@GroupByGateWay CHAR(1) = 'N',
	@GateWayName VARCHAR(255) = NULL
WITH RECOMPILE
AS
BEGIN	
	SET NOCOUNT ON
 
	DECLARE @Connectivities TABLE 
	(
		GateWay VARCHAR(250),
		Details VARCHAR(MAX),
		Date SMALLDATETIME,
		NumberOfChannels_In INT,
		NumberOfChannels_Out int,
		NumberOfChannels_Total int,
		Margin_Total INT,
		DetailList VARCHAR(MAX),
		InterconnectedSwithes CHAR(1)
	)
	
	SET @FromDateTime = dbo.DateOf(@FromDateTime)
	
	SET @ToDateTime= CAST(
     (
     STR( YEAR( @ToDateTime ) ) + '-' +
     STR( MONTH(@ToDateTime ) ) + '-' +
     STR( DAY( @ToDateTime ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	;
	
	DECLARE @ConnectivityValue TINYINT
	SELECT  @ConnectivityValue = Value FROM Enumerations e WHERE e.Enumeration = 'TABS.ConnectionType' AND e.[Name] = @ConnectionType; 

DECLARE @IPpattern VARCHAR(10)
SET @IPpattern = '%.%.%.%'


INSERT INTO @Connectivities
 SELECT c.*, ',' + c.Details + ',' AS DetailList  ,'Y' 
 from dbo.GetDailyConnectivity(
 		@ConnectivityValue,
 		@CarrierAccountID,
 		@SwitchID,
 		@GroupByGateWay, 
 		@FromDateTime,
 		@ToDateTime,
        'Y') c   
 
DECLARE @Continue CHAR(1)
SELECT @Continue = CASE WHEN COUNT(*) = 0 THEN 'Y' ELSE 'N' END  FROM @Connectivities 

	CREATE table #CarrierTraffic 
	(
		Period INT,
		[Date] DATETIME,
		port_in  VARCHAR(25) COLLATE  SQL_Latin1_General_CP1256_CI_AS, 
		port_out  VARCHAR(25) COLLATE  SQL_Latin1_General_CP1256_CI_AS,
		Attempts INT ,
		SuccessfulAttempts INT ,
		DurationsInSeconds NUMERIC(13,5),
		UtilizationInSeconds NUMERIC(13,5),
		NumberOfCalls INT
	) 
IF(@Continue = 'Y') 
RETURN ;

--	SELECT 
--		SwitchId,
--		Port_IN,
--		Port_OUT,
--		CustomerID,
--		OurZoneID,
--		OriginatingZoneID,
--		SupplierID,
--		SupplierZoneID,
--		FirstCDRAttempt,
--		LastCDRAttempt,
--		Attempts,
--		DeliveredAttempts,
--		SuccessfulAttempts,
--		DurationsInSeconds,
--		PDDInSeconds,
--		MaxDurationInSeconds,
--		UtilizationInSeconds,
--		NumberOfCalls
--     INTO #CarrierTraffic
--	 FROM TrafficStats ts WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
--	WHERE   
--		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
--		AND TS.SwitchID = @SwitchID 

WITH temptraffic AS (
SELECT 
		SwitchId,
		Port_IN,
		Port_OUT,
		FirstCDRAttempt,
		Attempts,
		SuccessfulAttempts,
		DurationsInSeconds,
		UtilizationInSeconds,
		NumberOfCalls
	FROM TrafficStats  WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
	WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND SwitchID = @SwitchID 
	)	

INSERT INTO #CarrierTraffic 
SELECT  datepart(hour,FirstCDRAttempt) AS Period,
		dbo.DateOf(FirstCDRAttempt) AS [Date],
		port_in, 
		port_out,
		SUM(Attempts) AS Attempts ,
		Sum(SuccessfulAttempts) AS SuccessfulAttempts,
		Sum(DurationsInSeconds) AS DurationsInSeconds,
		Sum(UtilizationInSeconds) AS UtilizationInSeconds,
		Sum(NumberOfCalls) AS NumberOfCalls 
FROM  temptraffic
GROUP BY datepart(hour,FirstCDRAttempt),
			dbo.DateOf(FirstCDRAttempt),
			port_in, 
			port_out
			
CREATE INDEX [IX_Traff_1] ON #CarrierTraffic([Period] ASC)
CREATE INDEX [IX_Traff_2] ON #CarrierTraffic([Date] ASC)
--SELECT * FROM #CarrierTraffic 
 
 SELECT
			TS.Period AS Period,
			TS.[Date],
			GateWay = CASE WHEN  @GroupByGateWay = 'Y' OR @GateWayName IS NOT NULL THEN DC.GateWay
			               ELSE  null END,
			               '0','0',
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_IN + ',%')  THEN ts.NumberOfCalls ELSE 0 END)   AS InAttempts,
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_Out + ',%') THEN ts.NumberOfCalls ELSE 0 END)   AS OutAttempts,
	        
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_IN + ',%') THEN ts.SuccessfulAttempts ELSE 0 END) AS  InSuccesfulAttempts,
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_Out + ',%') THEN ts.SuccessfulAttempts ELSE 0 END) AS  OutSuccesfulAttempts,
	        
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_IN + ',%') THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			Cast(NULLIF(SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_IN + ',%') THEN ts.NumberOfCalls ELSE 0 END) + 0.0, 0) AS NUMERIC) AS InASR,
	        
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_Out + ',%') THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			cast(NULLIF(SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_Out + ',%')THEN ts.NumberOfCalls ELSE 0 END)+ 0.0, 0) AS NUMERIC) AS OutASR,
	       
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_IN + ',%') THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS InDurationsInMinutes,
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_Out + ',%') THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS OutDurationsInMinutes, 
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_IN + ',%') THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS InUtilizationsInMinutes,
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_Out + ',%') THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS OutUtilizationsInMinutes,
			NumberOfChannels_In = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_In) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_In) FROM @Connectivities CSC WHERE CSC.Date = 	TS.[Date] )
								       ELSE 0 END,
			NumberOfChannels_Out = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_Out) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_Out) FROM @Connectivities CSC WHERE CSC.Date = 	TS.[Date])
								       ELSE 0 END,
			NumberOfChannels_Total = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_Total) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_Total) FROM @Connectivities CSC WHERE CSC.Date = 	TS.[Date] )
								       ELSE 0 END
	   FROM #CarrierTraffic AS TS WITH(NOLOCK,INDEX([IX_Traff_1],[IX_Traff_2]))   
			LEFT JOIN @Connectivities DC ON  DC.Date = TS.[Date]
									  AND (@GateWayName IS NULL OR (Dc.gateway LIKE @GateWayName))	
	   WHERE   
				1 = (CASE  WHEN @SelectPort  = 0 AND TS.Port_In like @PortInOut THEN 1     
		                   WHEN @SelectPort  = 1 AND TS.Port_Out like @PortInOut THEN 1 
		                   WHEN (@SelectPort  = 2 AND TS.Port_In like @PortInOut)
		                        OR (@SelectPort  = 2 AND TS.Port_Out like @PortInOut) THEN 1 
		                   ELSE 0 END 
			          ) 
		    
		GROUP BY TS.Period
		        ,TS.[Date]
			    ,CASE WHEN  @GroupByGateWay = 'Y' OR @GateWayName IS NOT NULL THEN DC.GateWay
			          ELSE  null END
END
GO
/****** Object:  StoredProcedure [dbo].[bp_BuildBillingStats]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_BuildBillingStats]
(
	@Day DateTime = NULL,
	@CustomerID VARCHAR(10) = NULL,
	@Batch INT = 5000
)
WITH RECOMPILE
AS
SET NOCOUNT ON

SELECT @Day = ISNULL(@Day, dateadd(dd , -1 , getdate()))

DECLARE @FromDate Datetime
DECLARE @ToDate Datetime 

SET @FromDate = dbo.DateOf(@Day)

SET @ToDate = dateadd(dd, 1, @FromDate)
PRINT 'Begin Delete on ' + cast(@FromDate AS varchar(20))
PRINT CONVERT(varchar,getdate(),121)
	
-----------------------------
-- Delete Billing Stats
-----------------------------
EXEC bp_CleanBillingStats @Date = @FromDate, @CustomerID = @CustomerID, @Batch = @Batch

PRINT 'Begin Insert on ' + cast(@FromDate AS varchar(20))
PRINT CONVERT(varchar,getdate(),121)
	
BEGIN TRANSACTION 
	
IF @CustomerID IS NULL
BEGIN
    
    INSERT INTO Billing_Stats
    (
        CallDate, CustomerID, SupplierID, CostZoneID, SaleZoneID, Cost_Currency, 
        Sale_Currency, SaleDuration,CostDuration, NumberOfCalls, FirstCallTime, LastCallTime, 
        MinDuration, MaxDuration, AvgDuration, Cost_Nets, Cost_Discounts, 
        Cost_Commissions, Cost_ExtraCharges, Sale_Nets, Sale_Discounts, 
        Sale_Commissions, Sale_ExtraCharges, Sale_Rate, Cost_Rate,Sale_RateType,
        Cost_RateType
    )
    
    SELECT dbo.DateOf(bcm.Attempt),
           bcm.CustomerID,
           bcm.SupplierID,
           bcm.SupplierZoneID,
           bcm.OurZoneID,
           bcc.CurrencyID,
           bcs.CurrencyID,
           SUM(bcs.DurationInSeconds),
           SUM(bcc.DurationInSeconds),
           COUNT(*),
           dbo.GetTimePart(MIN(bcm.Attempt)),
           dbo.GetTimePart(MAX(bcm.Attempt)),
           MIN(bcm.DurationInSeconds),
           MAX(bcm.DurationInSeconds),
           AVG(bcm.DurationInSeconds),
           SUM(bcc.Net),
           SUM(ISNULL(bcc.Discount,0)),
           SUM(ISNULL(bcc.CommissionValue,0)),
           SUM(ISNULL(bcc.ExtraChargeValue,0)),
           SUM(bcs.Net),
           SUM(ISNULL(bcs.Discount,0)),
           SUM(ISNULL(bcs.CommissionValue,0)),
           SUM(ISNULL(bcs.ExtraChargeValue,0)),
           AVG(ISNULL(bcs.RateValue,0)),
           AVG(ISNULL(bcc.RateValue,0)),
           bcs.RateType,
           bcc.RateType
    FROM   Billing_CDR_Main bcm WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt))
           LEFT JOIN  Billing_CDR_Sale bcs (NOLOCK) ON bcs.ID = bcm.ID
           LEFT JOIN Billing_CDR_Cost bcc (NOLOCK) ON  bcc.ID = bcm.ID
    WHERE  bcm.Attempt BETWEEN @FromDate AND @ToDate
    GROUP BY
           dbo.DateOf(bcm.Attempt),
           bcm.CustomerID,
           bcm.SupplierID,
           bcm.SupplierZoneID,
           bcm.OurZoneID,
           bcc.CurrencyID,
           bcs.CurrencyID,
           bcs.RateType,
           bcc.RateType
END
ELSE 
BEGIN
    
    INSERT INTO Billing_Stats
    (
        CallDate, CustomerID, SupplierID, CostZoneID, SaleZoneID, Cost_Currency, 
        Sale_Currency, SaleDuration, CostDuration, NumberOfCalls, FirstCallTime, LastCallTime, 
        MinDuration, MaxDuration, AvgDuration, Cost_Nets, Cost_Discounts, 
        Cost_Commissions, Cost_ExtraCharges, Sale_Nets, Sale_Discounts, 
        Sale_Commissions, Sale_ExtraCharges, Sale_Rate, Cost_Rate,Sale_RateType,
        Cost_RateType
    )
    
    SELECT dbo.DateOf(bcm.Attempt),
           bcm.CustomerID,
           bcm.SupplierID,
           bcm.SupplierZoneID,
           bcm.OurZoneID,
           bcc.CurrencyID,
           bcs.CurrencyID,
           SUM(bcs.DurationInSeconds),
           SUM(bcc.DurationInSeconds),
           COUNT(*),
           dbo.GetTimePart(MIN(bcm.Attempt)),
           dbo.GetTimePart(MAX(bcm.Attempt)),
           MIN(bcm.DurationInSeconds),
           MAX(bcm.DurationInSeconds),
           AVG(bcm.DurationInSeconds),
           SUM(bcc.Net),
           SUM(ISNULL(bcc.Discount,0)),
           SUM(ISNULL(bcc.CommissionValue,0)),
           SUM(ISNULL(bcc.ExtraChargeValue,0)),
           SUM(bcs.Net),
           SUM(ISNULL(bcs.Discount,0)),
           SUM(ISNULL(bcs.CommissionValue,0)),
           SUM(ISNULL(bcs.ExtraChargeValue,0)),
           AVG(ISNULL(bcs.RateValue,0)),
           AVG(ISNULL(bcc.RateValue,0)),
           bcs.RateType,
           bcc.RateType
    FROM   Billing_CDR_Main bcm WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt,IX_Billing_CDR_Main_Customer))
           LEFT JOIN Billing_CDR_Sale bcs (NOLOCK) ON bcs.ID = bcm.ID
           LEFT JOIN Billing_CDR_Cost bcc (NOLOCK) ON bcc.ID = bcm.ID
    WHERE  bcm.Attempt BETWEEN @FromDate AND @ToDate
      AND  bcm.CustomerID = @CustomerID
    GROUP BY
           dbo.DateOf(bcm.Attempt),
           bcm.CustomerID,
           bcm.SupplierID,
           bcm.SupplierZoneID,
           bcm.OurZoneID,
           bcc.CurrencyID,
           bcs.CurrencyID,
           bcs.RateType,
           bcc.RateType
END 
COMMIT 
   PRINT 'Finished'
   PRINT CONVERT(varchar,getdate(),121)
     
RETURN
GO
/****** Object:  StoredProcedure [dbo].[CleanOldData]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CleanOldData](
	@Keep_TrafficStats_Days INT = 60,
	@Keep_BillingMain_Days INT = 60,
	@Keep_BillingInvalid_Days INT = 30,
	@Keep_CDR_Days INT = 90,
	@Keep_EndedSpecialRequests_Days INT = 7,
	@Keep_EndedRouteBlocks_Days INT = 7 
)
AS
BEGIN

	-- Check Validity of Days to keep
	IF @Keep_TrafficStats_Days < 30 SET @Keep_TrafficStats_Days = 30
	IF @Keep_BillingMain_Days < 30 SET @Keep_BillingMain_Days = 30
	IF @Keep_BillingInvalid_Days < 7 SET @Keep_BillingInvalid_Days = 7
	IF @Keep_CDR_Days < 60 SET @Keep_CDR_Days = 60
	IF @Keep_EndedSpecialRequests_Days < 7 SET @Keep_EndedSpecialRequests_Days = 7
	IF @Keep_EndedRouteBlocks_Days < 7 SET @Keep_EndedRouteBlocks_Days = 7

	SET NOCOUNT ON
	SET ROWCOUNT 1000
	
	DECLARE @Counter BIGINT
	DECLARE @Msg VARCHAR(MAX)
	
	DECLARE @Today DATETIME
	SET @Today = dbo.Dateof(GETDATE())
	
	DECLARE @DELETED INT

	-- Traffic Stats
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-TrafficStats-Start', @message = NULL
	SET @DELETED = -1
	SET @Counter = 0
	WHILE @DELETED <> 0
	BEGIN
		DELETE FROM TrafficStats WHERE FirstCDRAttempt < DATEADD(day, -@Keep_TrafficStats_Days, @Today)
		SET @DELETED = @@ROWCOUNT
		SET @Counter = @Counter + @DELETED  		
	END
	SET @Msg = 'Days: '+ @Keep_TrafficStats_Days + '. Deleted: ' + @Counter 
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-TrafficStats-End', @message = @Msg

	-- Billing Invalid
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-Invalid-Start', @message = @Msg
	SET @DELETED = -1
	SET @Counter = 0
	WHILE @DELETED <> 0
	BEGIN
		DELETE FROM Billing_CDR_Invalid WHERE Attempt < DATEADD(day, -@Keep_BillingInvalid_Days, @Today)
		SET @DELETED = @@ROWCOUNT  		
		SET @Counter = @Counter + @DELETED
	END
	SET @Msg = 'Days: '+ @Keep_BillingInvalid_Days + '. Deleted: ' + @Counter 
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-Invalid-End', @message = @Msg

	-- Billing Main, Cost and Sale
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-Main-Start', @message = NULL
	DECLARE @MainIDs TABLE (ID BIGINT PRIMARY KEY) 
	SET @DELETED = -1
	SET @Counter = 0
	WHILE @DELETED <> 0
	BEGIN
		INSERT INTO @MainIDs (ID) SELECT ID FROM Billing_CDR_Main WHERE Attempt < DATEADD(day, -@Keep_BillingMain_Days, @Today)
		SET @DELETED = @@ROWCOUNT
		DELETE FROM Billing_CDR_Cost WHERE ID IN (SELECT ID FROM @MainIDs)
		DELETE FROM Billing_CDR_Sale WHERE ID IN (SELECT ID FROM @MainIDs)
		DELETE FROM Billing_CDR_Main WHERE ID IN (SELECT ID FROM @MainIDs)
		SET @Counter = @Counter + @DELETED
	END
	SET @Msg = 'Days: '+ @Keep_BillingMain_Days + '. Deleted: ' + @Counter 
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-Main-End', @message = @Msg

	-- CDR
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-CDR-Start', @message = NULL
	SET @DELETED = -1
	SET @Counter = 0
	WHILE @DELETED <> 0
	BEGIN
		DELETE FROM CDR WHERE AttemptDateTime < DATEADD(day, -@Keep_CDR_Days, @Today)
		SET @DELETED = @@ROWCOUNT  		
		SET @Counter = @Counter + @DELETED
	END
	SET @Msg = 'Days: '+ @Keep_CDR_Days + '. Deleted: ' + @Counter 
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-CDR-End', @message = @Msg

	-- Special Requests
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-SpecialRequests-Start', @message = NULL
	SET @DELETED = -1
	SET @Counter = 0
	WHILE @DELETED <> 0
	BEGIN
		DELETE FROM SpecialRequest WHERE EndEffectiveDate < DATEADD(day, -@Keep_EndedSpecialRequests_Days, @Today)
		SET @DELETED = @@ROWCOUNT  		
		SET @Counter = @Counter + @DELETED
	END
	SET @Msg = 'Days: '+ @Keep_EndedSpecialRequests_Days + '. Deleted: ' + @Counter 
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-SpecialRequests-End', @message = @Msg

	-- Route Blocks
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-RouteBlocks-Start', @message = NULL
	SET @DELETED = -1
	SET @Counter = 0
	WHILE @DELETED <> 0
	BEGIN
		DELETE FROM RouteBlock WHERE EndEffectiveDate < DATEADD(day, -@Keep_EndedRouteBlocks_Days, @Today)
		SET @DELETED = @@ROWCOUNT  		
		SET @Counter = @Counter + @DELETED
	END	
	SET @Msg = 'Days: '+ @Keep_EndedRouteBlocks_Days + '. Deleted: ' + @Counter 
	EXEC bp_SetSystemMessage @msgID = 'Clean-Old-RouteBlocks-End', @message = @Msg
			
END
GO
/****** Object:  StoredProcedure [dbo].[bp_UpdateRoutesFromSpecialRequests]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--
-- Update the Routes and Route Options from the defined Special Requests
--
CREATE PROCEDURE [dbo].[bp_UpdateRoutesFromSpecialRequests]
(
	@Check char(1) = 'Y'
)
AS
BEGIN
	
	SET NOCOUNT ON

	IF @Check = 'Y'
	BEGIN
		DECLARE @IsRunning char(1)
		EXEC bp_IsRouteBuildRunning @IsRunning output
		IF @IsRunning = 'Y' 
		BEGIN
			PRINT 'Build Routes is already Runnning'
			RETURN 
		END 
		EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = 'Updating From Special Requests'
		EXEC bp_ResetRoutes @UpdateType = 'SpecialRequests'
	END		

	DECLARE @UpdateStamp datetime
	SET @UpdateStamp = getdate()

	DECLARE @HighestPriority tinyint
	SET @HighestPriority = 0

	DECLARE @ForcedRoute tinyint
	SET @ForcedRoute = 1


	-- insert forced routes
	INSERT INTO RouteOption
	(
		RouteID,
		SupplierID,
		SupplierZoneID,
		SupplierActiveRate,
		SupplierNormalRate,
		SupplierOffPeakRate,
		SupplierWeekendRate,
		SupplierServicesFlag,
		Priority,
		NumberOfTries,
		[State],
		Updated
	)
	SELECT 
		RouteID,
		sr.SupplierID,
		zr.ZoneID,
		zr.NormalRate,
		zr.NormalRate,
		zr.OffPeakRate,
		zr.WeekendRate,
		zr.ServicesFlag,
		sr.Priority,
		sr.NumberOfTries,
		1,
		GETDATE()
		FROM 
			SpecialRequest sr, CarrierAccount ca, [Route] r, CodeMatch cm, ZoneRate zr  
		WHERE 
				sr.IsEffective = 'Y'
			AND sr.SpecialRequestType = @ForcedRoute
			AND sr.CustomerID = r.CustomerID 
			AND sr.SupplierID = ca.CarrierAccountID
			AND ca.IsDeleted = 'N' 
			AND ca.ActivationStatus IN (1,2)
			AND ca.RoutingStatus IN (1,3)
			AND sr.Code = r.Code -- Removed by Fadi (sr.ZoneID = r.OurZoneID)
			AND sr.CustomerID = r.CustomerID
			AND NOT EXISTS(SELECT * FROM RouteOption ro WHERE ro.RouteID = r.RouteID AND ro.SupplierID = sr.SupplierID)
			AND cm.Code = r.Code
			AND cm.SupplierID = sr.SupplierID
			AND zr.ZoneID = cm.SupplierZoneID

	-- Now Set to Updated the Routes That have the Route Options Updated
	EXEC bp_SignalRouteUpdatesFromOptions @UpdateStamp=@UpdateStamp, @UpdateType='SpecialRequests'
	
		-- Priority Special Requests: We Move the priority up one notch for suppliers in the Special Requests
	-- Forced Special Requests: We Flag the priority and number of tries of Forced routes to 255
	
	-- Special Requests for Customer, Code/Zone, Supplier
	UPDATE [RouteOption] 
		SET 
			Priority = S.Priority 
			, Updated = GETDATE()
			, NumberOfTries = S.NumberOfTries 
	FROM 
		[RouteOption], [Route] R, SpecialRequest S
	WHERE
		S.IsEffective='Y'
		AND [RouteOption].RouteID = R.RouteID
		AND S.SupplierID = [RouteOption].SupplierID
		AND S.CustomerID = R.CustomerID
		AND s.SpecialRequestType = @HighestPriority
		AND S.Code = r.Code -- Removed by Fadi -- OR S.ZoneID = r.OurZoneID

	IF @Check = 'Y'
	BEGIN
		EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = NULL
	END

END
GO
/****** Object:  StoredProcedure [dbo].[bp_FixRateChanges]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[bp_FixRateChanges]
	@RateID bigint = NULL,
	@PriceListID bigint = NULL,
	@AllRates char(1) = 'N'
AS 
	SET NOCOUNT ON
	
	IF @RateID IS NOT NULL
		UPDATE Rate SET Change = 
			CASE 			
				WHEN ISNULL(dbo.GetPreviousRateValue(Rate.RateID), Rate) > Rate.Rate THEN -1
				WHEN ISNULL(dbo.GetPreviousRateValue(Rate.RateID), Rate) < Rate.Rate THEN 1
				ELSE 0
			END
		WHERE RateID = @RateID
	ELSE	
	BEGIN
		IF @PriceListID IS NOT NULL
		BEGIN
			DECLARE curRates CURSOR LOCAL FORWARD_ONLY
			FOR SELECT RateID FROM Rate WHERE PriceListID = @PriceListID
			
			OPEN curRates
			FETCH NEXT FROM curRates INTO @RateID
			WHILE @@FETCH_STATUS = 0
			BEGIN
				EXEC bp_FixRateChanges @RateID = @RateID
				FETCH NEXT FROM curRates INTO @RateID
			END		
			CLOSE curRates
			DEALLOCATE curRates
			
			EXEC bp_FixErroneousEffectiveRates 
		END
		ELSE IF @AllRates = 'Y'
		BEGIN
			DECLARE curRates CURSOR LOCAL FORWARD_ONLY
			FOR SELECT RateID FROM Rate ORDER BY RateID
			
			OPEN curRates
			FETCH NEXT FROM curRates INTO @RateID
			WHILE @@FETCH_STATUS = 0
			BEGIN
				EXEC bp_FixRateChanges @RateID = @RateID
				FETCH NEXT FROM curRates INTO @RateID
			END		
			CLOSE curRates
			DEALLOCATE curRates
			
			EXEC bp_FixErroneousEffectiveRates
		END
	END
GO
/****** Object:  UserDefinedFunction [dbo].[GetDailyExchangeRates]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ========================================================
-- Author:		Fadi Chamieh
-- Create date: 2009-03-02
-- Description:	Get a table of Exchange Rates based on Date
-- =========================================================
CREATE FUNCTION [dbo].[GetDailyExchangeRates]
(
	-- Add the parameters for the function here
	@From DATETIME,
	@Till DATETIME
)
RETURNS 
@ExchangeRates TABLE 
(
	Currency VARCHAR(3),
	Date SMALLDATETIME,
	Rate FLOAT
	PRIMARY KEY(Currency, Date)
)
AS
BEGIN
	SET @From = dbo.DateOf(@From)
	DECLARE @Pivot DATETIME
	SET @Pivot = @From
	WHILE @Pivot <= @Till
	BEGIN
		INSERT INTO @ExchangeRates(Currency, Date, Rate)
			SELECT c.CurrencyID, @Pivot, dbo.GetExchangeRate(c.CurrencyID, @Pivot) FROM Currency c WHERE c.IsVisible = 'Y'
		SET @Pivot = DATEADD(dd, 1, @Pivot)
	END
	RETURN
END
GO
/****** Object:  UserDefinedFunction [dbo].[GetCommission]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ============================================================================================================
-- Author:		Fadi Chamieh
-- Create date: 11/07/2007
-- Description:	Get the commission (or Extra Charge) Sale/Purchase Rate for a Customer/Supplier on a given Zone
-- ============================================================================================================
CREATE FUNCTION [dbo].[GetCommission]
(
	@SupplierID varchar(10),
	@CustomerID varchar(10),
	@RateCurrency CHAR(3),
	@EffectiveDate smalldatetime,
	@ZoneID int,
	@Rate float,
	@IsExtraCharge char(1)
)
RETURNS float
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Commission float
	DECLARE @Operator float
	DECLARE @ProfileCurrency CHAR(3)
	
	
	SET @Commission = 0
	
	SET @Operator = CASE WHEN @SupplierID <> 'SYS' THEN 1 ELSE -1 END
	
	IF (@SupplierID = 'SYS')
    SELECT @ProfileCurrency = cp.CurrencyID
	  FROM CarrierProfile cp WITH(NOLOCK) WHERE cp.ProfileID IN 
	(
	  SELECT ca.ProfileID
	  FROM CarrierAccount ca WITH(NOLOCK) WHERE ca.CarrierAccountID = @CustomerID
	)
	
	IF (@CustomerID = 'SYS')
	SELECT @ProfileCurrency = cp.CurrencyID
	  FROM CarrierProfile cp WITH(NOLOCK) WHERE cp.ProfileID IN 
	(
	  SELECT ca.ProfileID
	  FROM CarrierAccount ca WITH(NOLOCK) WHERE ca.CarrierAccountID = @SupplierID
	)
	
	DECLARE @Factor FLOAT 
	
	IF(@ProfileCurrency <> @RateCurrency)
    	SET @Factor = dbo.GetExchangeRate(@ProfileCurrency,GETDATE()) / dbo.GetExchangeRate(@RateCurrency,GETDATE())
	ELSE
		SET @Factor = 1
	

	-- Compute Commission Or Extra Charge
	SELECT @Commission = @Operator * (CASE WHEN Amount  IS NOT NULL THEN Amount / @Factor ELSE @Rate * Percentage / 100.0  END)
		FROM [Commission]
		WHERE 1=1 
			AND SupplierID = @SupplierID
			AND CustomerID = @CustomerID
			AND @EffectiveDate BETWEEN BeginEffectiveDate AND ISNULL(EndEffectiveDate,getdate())
			AND ZoneID = @ZoneID
			AND (FromRate IS NULL OR (@Rate * @Factor BETWEEN FromRate AND ToRate))
			AND IsExtraCharge = @IsExtraCharge

	-- Return the result of the function
	RETURN @Commission 

END
GO
/****** Object:  StoredProcedure [dbo].[bp_CleanBillingAndStats]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_CleanBillingAndStats]
(@From datetime, @Till datetime, @SwitchID tinyint=NULL, @CustomerID varchar(10)=NULL, @SupplierID varchar(10)=NULL, @Batch bigint=1000, @IsDebug char(1)='N', @IncludeTrafficStats char(1)='Y')
WITH 
RECOMPILE,
EXECUTE AS CALLER
AS
BEGIN
	
	DECLARE @DeletedCount bigint
	Declare @WorkingDay datetime
	Declare @WorkingDayEnd datetime
	DECLARE @MainIDs TABLE(ID bigint NOT NULL PRIMARY key)
	Declare @MinId bigint
	Declare @MaxId BIGINT
	DECLARE @WorkingDayDesc VARCHAR(10)

	SET @WorkingDay = dbo.DateOf(@From)
	SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)	
	SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))
	
	DECLARE @Dummy int	

	SET NOCOUNT ON	

	WHILE @WorkingDay <= @Till
	BEGIN		
		SET ROWCOUNT @Batch
		
		DECLARE @Message VARCHAR(4000)
		DECLARE @MsgID VARCHAR(100)
		DECLARE @MsgSub VARCHAR(100)
		SET @MsgID = 'Clean Billing And Stats'
		
		SET @Message = 'Working On: ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgID, @message = @Message
				
		--  ********* Removed By Fadi Since this is called by Build Billing Stats **********
		--		-----------------------------
		--		-- Delete Billing Stats
		--		-----------------------------
		--		SET @MsgSub = @MsgID + ': Billing Stats'
		--		SET @Message = 'Cleaning Billing Stats for ' + @WorkingDayDesc
		--		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message
		--
		--		EXEC bp_CleanBillingStats 
		--			@Date = @WorkingDay,
		--			@CustomerID = @CustomerID, 
		--			@SupplierID = @SupplierID, 
		--			@Batch = @Batch,
		--			@IsDebug = @IsDebug		
		
		-----------------------------
		-- Delete Invalid Billing Info
		-----------------------------
		SET @MsgSub = @MsgID + ': Billing CDR Invalid'
		SET @Message = 'Cleaning Billing CDR Invalid ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message
		
		SET @DeletedCount = 1
		WHILE @DeletedCount > 0
		BEGIN
			BEGIN TRANSACTION Cleaner
				
				IF @IncludeTrafficStats = 'Y'
					BEGIN
						-- No Customer, No Supplier
						IF @CustomerID IS NULL AND @SupplierID IS NULL
							DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd
						-- No Supplier
						ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
							DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt, IX_Billing_CDR_Invalid_Customer)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND CustomerID = @CustomerID
						-- Customer, Supplier
						ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL	
							DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt, IX_Billing_CDR_Invalid_Customer,IX_Billing_CDR_Invalid_Supplier)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND CustomerID = @CustomerID AND SupplierID = @SupplierID
						-- No Customer
						ELSE
							DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt, IX_Billing_CDR_Invalid_Supplier)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND SupplierID = @SupplierID
					END
				ELSE
					BEGIN
						-- No Customer, No Supplier
						IF @CustomerID IS NULL AND @SupplierID IS NULL
							DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND DurationInSeconds > 0
						-- No Supplier
						ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
							DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt,IX_Billing_CDR_Invalid_Customer)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND CustomerID = @CustomerID AND DurationInSeconds > 0
						-- Customer, Supplier
						ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL	
							DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt,IX_Billing_CDR_Invalid_Customer,IX_Billing_CDR_Invalid_Supplier)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND CustomerID = @CustomerID AND SupplierID = @SupplierID AND DurationInSeconds > 0
						-- No Customer
						ELSE
							DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt,IX_Billing_CDR_Invalid_Supplier)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND SupplierID = @SupplierID AND DurationInSeconds > 0
					END
				SET @DeletedCount = @@ROWCOUNT
				
			COMMIT TRANSACTION Cleaner
		END
		IF @IsDebug = 'Y' PRINT 'Deleted Invalid CDRs ' + convert(varchar(25), getdate(), 121)
		
		-----------------------------
		-- Get Main Billing CDR ids
		-----------------------------
		SET @MsgSub = @MsgID + ': Main, Cost, Sale - Start'
		SET @Message = 'Deleting Billing CDR Main ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message

		-- Clear Batch Row Count Number (to insert all Mains)
		SET @DeletedCount = 1
		
		-- While there are still Main CDRs
		WHILE @DeletedCount > 0
		BEGIN
			
			DELETE FROM @MainIDs
			
			SET ROWCOUNT @Batch
			
			-- No Customer, No Supplier
			IF @CustomerID IS NULL AND @SupplierID IS NULL
				INSERT INTO @MainIDs SELECT ID FROM Billing_CDR_Main WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd 
			-- No Supplier
			ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
				INSERT INTO @MainIDs SELECT ID FROM Billing_CDR_Main WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt, IX_Billing_CDR_Main_Customer)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND CustomerID = @CustomerID 
			-- Customer, Supplier
			ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL	
				INSERT INTO @MainIDs SELECT ID FROM Billing_CDR_Main WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt, IX_Billing_CDR_Main_Customer, IX_Billing_CDR_Main_Supplier)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND CustomerID = @CustomerID AND SupplierID = @SupplierID 
			-- No Customer
			ELSE
				INSERT INTO @MainIDs SELECT ID FROM Billing_CDR_Main WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt, IX_Billing_CDR_Main_Supplier)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd AND SupplierID = @SupplierID 
			
			SELECT @DeletedCount = @@ROWCOUNT
			
			-- If there is something to delete
			IF @DeletedCount > 0
			BEGIN				
				SET ROWCOUNT 0

				BEGIN TRANSACTION Cleaner
					-----------------------------
					-- Delete Billing Costs 
					-----------------------------
					DELETE Billing_CDR_Cost FROM Billing_CDR_Cost WITH(NOLOCK) WHERE ID IN (SELECT ID FROM @MainIDs)

					-----------------------------
					-- Delete Billing Sales
					-----------------------------
					DELETE Billing_CDR_Sale FROM Billing_CDR_Sale WITH(NOLOCK) WHERE ID IN (SELECT ID FROM @MainIDs)

					-----------------------------
					-- Delete Main Billing CDRs
					-----------------------------
					DELETE Billing_CDR_Main FROM Billing_CDR_Main WITH(NOLOCK) WHERE ID IN (SELECT ID FROM @MainIDs)
			
				COMMIT TRANSACTION Cleaner	 	
			END	-- If @Deleted > 0	
		
		END -- While
		
		SET @MsgSub = @MsgID + ': Main, Cost, Sale - End'
		SET @Message = 'Deleted Billing CDR Main with sales and costs for ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message
		
		-----------------------------
		-- Move to next Day
		-----------------------------
		IF @IsDebug = 'Y' PRINT 'Finished: ' + @WorkingDayDesc
		SET @WorkingDay = dateadd(day, 1, @WorkingDay)
		SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)
		SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))

		IF @IsDebug = 'Y' PRINT 'Next: ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT '---------------------------------------------'

	END

END
GO
/****** Object:  StoredProcedure [dbo].[bp_UpdateRoutesFromRouteBlocks]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--
-- Update the Routes and Route Options from the defined route blocks
--
CREATE PROCEDURE [dbo].[bp_UpdateRoutesFromRouteBlocks]
	@Check char(1) = 'Y'
AS

	SET NOCOUNT ON

	IF @Check = 'Y' 
	BEGIN	
		DECLARE @IsRunning char(1)
		EXEC bp_IsRouteBuildRunning @IsRunning output
		IF @IsRunning = 'Y' 
		BEGIN
			PRINT 'Build Routes is already Runnning'
			RETURN 
		END 
		EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = 'Updating From Route Blocks'
		EXEC bp_ResetRoutes @UpdateType = 'RouteBlocks'
	END

	DECLARE @UpdateStamp datetime
	SET @UpdateStamp = getdate()
	
	
	/* Supplier Zone Blocks: All Customers */
	-- Block Route Options that have their supplier zones blocked
	UPDATE [RouteOption] SET State = 0, Updated = GETDATE()
		FROM RouteBlock B, [RouteOption] 
			WHERE 
					B.IsEffective='Y' 
				AND B.CustomerID IS NULL 
				AND B.ZoneID IS NOT NULL 
				AND B.ZoneID = [RouteOption].SupplierZoneID

	/* Supplier Zone Blocks: Specific Customers */
	-- Block Route Options for Customer specified Supplier Zone Block
	UPDATE [RouteOption] SET State = 0, Updated = GETDATE()
		FROM RouteBlock B, [RouteOption], [Route] R
		WHERE
			B.IsEffective='Y'
			AND B.CustomerID IS NOT NULL 
			AND B.ZoneID IS NOT NULL
			AND B.SupplierID IS NOT NULL 
			AND B.ZoneID = RouteOption.SupplierZoneID 
			AND B.SupplierID = RouteOption.SupplierID
			AND B.CustomerID = R.CustomerID
			AND R.RouteID = [RouteOption].RouteID 

	-- Block Route Options matching other criteria
	UPDATE [RouteOption] SET State = 0, Updated = GETDATE()
		FROM RouteBlock B, [RouteOption], [Route] R
		WHERE
			B.IsEffective='Y'
			AND B.Code IS NOT NULL -- OR B.ZoneID IS NOT NULL)
			AND B.CustomerID = R.CustomerID
			AND R.RouteID = [RouteOption].RouteID 
			AND B.SupplierID = [RouteOption].SupplierID 
			AND B.Code = R.Code -- OR B.ZoneID = R.OurZoneID)

	-- Now Set to Updated the Routes That have the Route Options Updated
	EXEC bp_SignalRouteUpdatesFromOptions @UpdateStamp=@UpdateStamp, @UpdateType='RouteBlocks'

	-- Block Route Options for all Blocked Routes
	UPDATE RouteOption
		SET
		[State] = 0, Updated = @UpdateStamp
		WHERE RouteID IN (SELECT r.RouteID FROM [Route] r WHERE r.[State] = 0)

	IF @Check = 'Y'
	BEGIN
		EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = NULL
	END
GO
/****** Object:  StoredProcedure [dbo].[bp_CleanTrafficStats]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_CleanTrafficStats]
(@From datetime, @Till datetime, @SwitchID tinyint=NULL, @CustomerID varchar(10)=NULL, @SupplierID varchar(10)=NULL, @Batch bigint=1000, @IsDebug char(1)='N')
WITH 
RECOMPILE,
EXECUTE AS CALLER
AS
BEGIN
	
	DECLARE @DeletedCount bigint
	Declare @WorkingDay datetime
	Declare @WorkingDayEnd datetime
	DECLARE @WorkingDayDesc VARCHAR(10)

	DECLARE @Message VARCHAR(4000)
	DECLARE @MsgID VARCHAR(100)
	SET @MsgID = 'Clean Traffic Stats'

	SET @WorkingDay = dbo.DateOf(@From)
	SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)	
	SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))
	
	DECLARE @Dummy int	

	SET NOCOUNT ON	

	WHILE @WorkingDay <= @Till
	BEGIN		
		SET ROWCOUNT @Batch
		
		SET @Message = 'Cleaning Traffic Stats for ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgID, @message = @Message

		-----------------------------
		-- Delete Traffic Stats
		-----------------------------
		SELECT @DeletedCount = 1
		WHILE @DeletedCount > 0
		BEGIN
			BEGIN TRANSACTION Cleaner
				-- No Customer, No Supplier
				IF @CustomerID IS NULL AND @SupplierID IS NULL
					DELETE TrafficStats FROM TrafficStats WITH(NOLOCK, INDEX(IX_TrafficStats_DateTimeFirst)) WHERE FirstCDRAttempt BETWEEN @WorkingDay AND @WorkingDayEnd 
				-- No Supplier
				ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
					DELETE TrafficStats FROM TrafficStats WITH(NOLOCK, INDEX(IX_TrafficStats_DateTimeFirst, IX_TrafficStats_Customer)) WHERE FirstCDRAttempt BETWEEN @WorkingDay AND @WorkingDayEnd AND CustomerID = @CustomerID 
				-- Customer, Supplier
				ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL	
					DELETE TrafficStats FROM TrafficStats WITH(NOLOCK, INDEX(IX_TrafficStats_DateTimeFirst, IX_TrafficStats_Customer, IX_TrafficStats_Supplier)) WHERE FirstCDRAttempt BETWEEN @WorkingDay AND @WorkingDayEnd AND CustomerID = @CustomerID AND SupplierID = @SupplierID
				-- No Customer
				ELSE
					DELETE TrafficStats FROM TrafficStats WITH(NOLOCK, INDEX(IX_TrafficStats_DateTimeFirst, IX_TrafficStats_Supplier)) WHERE FirstCDRAttempt BETWEEN @WorkingDay AND @WorkingDayEnd AND SupplierID = @SupplierID
				SET @DeletedCount = @@ROWCOUNT
			COMMIT TRANSACTION Cleaner	
		END
		IF @IsDebug = 'Y' PRINT 'Deleted Traffic Stats ' + convert(varchar(25), getdate(), 121)
		
		-----------------------------
		-- Move to next Day
		-----------------------------
		SET @WorkingDay = dateadd(day, 1, @WorkingDay)
		SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)
		SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))

		IF @IsDebug = 'Y' PRINT 'Next: ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT '---------------------------------------------'

		SET @Message = 'Cleaned Traffic Stats for ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgID, @message = @Message

	END

END
GO
/****** Object:  StoredProcedure [dbo].[bp_CompressTrafficStats]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_CompressTrafficStats]
	@StartingDateTime datetime,
	@EndingDateTime datetime
AS
BEGIN
	
	SET NOCOUNT ON

	DECLARE @Message VARCHAR(4000)
	DECLARE @MsgID VARCHAR(100)
	DECLARE @DeletedCount bigint
	Declare @WorkingDay datetime
	Declare @WorkingDayEnd datetime
	DECLARE @WorkingDayDesc VARCHAR(10)

	SET @WorkingDay = dbo.DateOf(@StartingDateTime)
	SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))
	SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)	

	SET @MsgID = 'TrafficStatsCompression:Start'
	SET @Message = convert(varchar(25), GETDATE(), 121)	
	EXEC bp_SetSystemMessage @msgID = @MsgID, @message = @Message

	CREATE TABLE #TrafficStatsCompressed
	(
		[FirstCDRAttempt] [datetime] NOT NULL,
		[LastCDRAttempt] [datetime] NOT NULL,
		[SwitchId] [tinyint] NOT NULL,
		[CustomerID] [varchar](10) NULL,
		[OurZoneID] [int] NULL,
		[OriginatingZoneID] [int] NULL,
		[SupplierID] [varchar](10) NULL,
		[SupplierZoneID] [int] NULL,
		[Attempts] [int] NULL,
		[NumberOfCalls] [int] NULL,
		[DeliveredAttempts] [int] NULL,
		[SuccessfulAttempts] [int] NULL,
		[DurationsInSeconds] [numeric](13, 5) NULL,
		[PDDInSeconds] [numeric](13, 5) NULL,
		[MaxDurationInSeconds] [numeric](13, 5) NULL,
		[Port_IN]	varchar(21)	NULL,
        [Port_OUT]	varchar(21)	NULL,
        [UtilizationInSeconds] NUMERIC(13, 5)
	) 
	
	WHILE @WorkingDay >= @StartingDateTime AND @WorkingDay <= @EndingDateTime
	BEGIN
		
		SET @MsgID = 'TrafficStatsCompression:' + @WorkingDayDesc + ':Start'
		SET @Message = convert(varchar(25), GETDATE(), 121)	
		EXEC bp_SetSystemMessage @msgID = @MsgID, @message = @Message
			
		-- Clean Temp
		TRUNCATE TABLE #TrafficStatsCompressed
		
		BEGIN TRANSACTION TrafficDay 
		
		-- Create Stats for this working day
		INSERT INTO #TrafficStatsCompressed
		(
			FirstCDRAttempt,
			LastCDRAttempt,
			SwitchId,
			CustomerID,
			OurZoneID,
			OriginatingZoneID,
			SupplierID,
			SupplierZoneID,
			Attempts,
			NumberOfCalls,			
			DeliveredAttempts,
			SuccessfulAttempts,
			DurationsInSeconds,
			PDDInSeconds,
			MaxDurationInSeconds,
			Port_IN,
			Port_OUT,
			UtilizationInSeconds
		)
		SELECT
			Min(FirstCDRAttempt),
			Max(LastCDRAttempt),
			SwitchId,
			CustomerID,
			OurZoneID,
			OriginatingZoneID,
			SupplierID,
			SupplierZoneID,
			Sum(Attempts),
			SUM(NumberOfCalls),
			Sum(DeliveredAttempts),
			Sum(SuccessfulAttempts),
			Sum(DurationsInSeconds),
			Avg(PDDInSeconds),
			Max(MaxDurationInSeconds),
			Port_IN,
			Port_OUT,
			SUM(UtilizationInSeconds)
		FROM TrafficStats WITH(NOLOCK, INDEX(IX_TrafficStats_DateTimeFirst))
			WHERE FirstCDRAttempt BETWEEN @WorkingDay AND @WorkingDayEnd
		GROUP BY
			SwitchId,
			CustomerID,
			OurZoneID,
			OriginatingZoneID,
			SupplierID,
			SupplierZoneID,
			Port_IN,
			Port_OUT,
			CONVERT (varchar(10), FirstCDRAttempt, 121)

		-- Remove non necessary stats
		SET @DeletedCount = 1
		SET ROWCOUNT 5000
		WHILE @DeletedCount > 0
		BEGIN
			DELETE TrafficStats FROM TrafficStats WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
				WHERE FirstCDRAttempt BETWEEN @WorkingDay AND @WorkingDayEnd
			SET @DeletedCount = @@ROWCOUNT				
		END
		
		SET ROWCOUNT 0
		
		-- Insert grouped
		INSERT INTO TrafficStats
			(
			FirstCDRAttempt,
			LastCDRAttempt,
			SwitchId,
			CustomerID,
			OurZoneID,
			OriginatingZoneID,
			SupplierID,
			SupplierZoneID,
			Attempts,
			NumberOfCalls,
			DeliveredAttempts,
			SuccessfulAttempts,
			DurationsInSeconds,
			PDDInSeconds,
			MaxDurationInSeconds,
			Port_IN,
			Port_OUT,
			UtilizationInSeconds
			)
			SELECT 
				FirstCDRAttempt,
				LastCDRAttempt,
				SwitchId,
				CustomerID,
				OurZoneID,
				OriginatingZoneID,
				SupplierID,
				SupplierZoneID,
				Attempts,
				NumberOfCalls,
				DeliveredAttempts,
				SuccessfulAttempts,
				DurationsInSeconds,
				PDDInSeconds,
				MaxDurationInSeconds,
				Port_IN,
				Port_OUT,
				UtilizationInSeconds
			FROM 
				#TrafficStatsCompressed

		COMMIT TRANSACTION TrafficDay

		SET @MsgID = 'TrafficStatsCompression:' + @WorkingDayDesc + ':End'
		SET @Message = convert(varchar(25), GETDATE(), 121)	
		EXEC bp_SetSystemMessage @msgID = @MsgID, @message = @Message
		
		SET @WorkingDay = DATEADD(DAY, 1, @WorkingDay)
		SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))
		SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)

	END

	SET @MsgID = 'TrafficStatsCompression:End'
	SET @Message = convert(varchar(25), GETDATE(), 121)	
	EXEC bp_SetSystemMessage @msgID = @MsgID, @message = @Message
		
	DROP TABLE #TrafficStatsCompressed
END
GO
/****** Object:  StoredProcedure [dbo].[bp_GetSaleCodeGaps]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_GetSaleCodeGaps] (@IsDebug CHAR(1) = 'N')
AS
BEGIN

	DECLARE @today datetime
	SET @today = getdate()
	DECLARE @CarrierAccountID varchar(10)

	SET NOCOUNT ON

	SELECT * INTO #CustomersCodeGaps FROM dbo.GetSaleCodeGaps('--NONE--', @today) 

	DECLARE my_cursor CURSOR LOCAL FAST_FORWARD READ_ONLY 
		FOR SELECT CarrierAccountID FROM CarrierAccount ca WHERE ca.IsDeleted = 'N' 
	OPEN my_cursor

		FETCH FROM my_cursor INTO @CarrierAccountID
		
		IF @IsDebug = 'Y' PRINT 'Started *** ' + CONVERT(VARCHAR(20), GETDATE(), 121)

		WHILE @@FETCH_STATUS = 0
		BEGIN
			
			IF @IsDebug = 'Y' PRINT ('Codes For: ' + @CarrierAccountID + ' *** ' + RIGHT(CONVERT(VARCHAR(20), GETDATE(), 121), 8))
			
			INSERT INTO #CustomersCodeGaps
				SELECT * FROM dbo.GetSaleCodeGaps(@CarrierAccountID, @today) 
			
			FETCH FROM my_cursor INTO @CarrierAccountID
			IF @IsDebug = 'Y' PRINT ('Done *** ' + RIGHT(CONVERT(VARCHAR(20), GETDATE(), 121), 8))

		END

	CLOSE my_cursor
	DEALLOCATE my_cursor 

	SELECT * FROM #CustomersCodeGaps

	DROP TABLE #CustomersCodeGaps
	
END
GO
/****** Object:  StoredProcedure [dbo].[bp_CreateCustomerInvoice]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_CreateCustomerInvoice]
(
	@CustomerID varchar(5),
	@Serial varchar(50),
	@GMTShifttime int,
	@TimeZoneInfo VARCHAR(MAX),
	@FromDate Datetime,
	@ToDate Datetime,
	@IssueDate Datetime,
	@DueDate Datetime,
	@UserID INT,
	@InvoiceID int output
)
AS
SET NOCOUNT ON 

-- Creating the Invoice 	
DECLARE @Amount float 
DECLARE @Duration NUMERIC(19,6) 
DECLARE @NumberOfCalls INT 
DECLARE @CurrencyID varchar(3) 

DECLARE @From SMALLDATETIME
SET @From = @FromDate

DECLARE @To SMALLDATETIME
SET @To = @ToDate

SET @FromDate = CAST(
(
STR( YEAR( @FromDate ) ) + '-' +
STR( MONTH( @FromDate ) ) + '-' +
STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate = CAST(
(
STR( YEAR( @ToDate ) ) + '-' +
STR( MONTH(@ToDate ) ) + '-' +
STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)

-- check if an invoice is issued before for the same customer and for the same period 
DECLARE @OldSerial varchar(50)
DECLARE @OldNotes VARCHAR(MAX)
SELECT @InvoiceID = bi.InvoiceID,
       @OldSerial = bi.SerialNumber,
       @OldNotes = bi.InvoicePrintedNote
FROM   Billing_Invoice bi (NOLOCK)
WHERE  bi.CustomerID = @CustomerID 
AND 
     (
	     bi.BeginDate BETWEEN @FromDate AND dbo.DateOf(@ToDate)
       OR 
         bi.EndDate BETWEEN @FromDate AND dbo.DateOf(@ToDate)
       )


-- Apply the Time Shift 
SET @FromDate = dateadd(mi,-@GMTShifttime,@FromDate);
SET @ToDate = dateadd(mi,-@GMTShifttime,@ToDate);

-- building a temp table that contains all billing data requested 
--------------------------------------------------------------------------------------

DECLARE @Pivot SMALLDATETIME
SET @Pivot = @FromDate

    SELECT 
	   bcm.Attempt,
	   bcm.Connect,
       bcm.CustomerID, bcm.OurZoneID,
       bcm.SupplierID, bcm.SupplierZoneID, bcs.ZoneID, bcs.Net, bcs.CurrencyID,
       bcs.RateValue, bcs.RateType, bcs.DurationInSeconds
   INTO   #TempBillingCDR
FROM   Billing_CDR_Main bcm WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt))
       LEFT JOIN Billing_CDR_Sale bcs (NOLOCK) ON  bcs.ID = bcm.ID
WHERE  bcm.Attempt ='1995-01-01'


WHILE @Pivot <= @ToDate
BEGIN
 ----------------------
 DECLARE @BeginDate DATETIME
 DECLARE @EndDate DATETIME
 SET @BeginDate = @Pivot

SET @EndDate = DATEADD(dd, 1, @Pivot)

INSERT INTO  #TempBillingCDR
    SELECT 
	   bcm.Attempt,
	   bcm.Connect,
       bcm.CustomerID, bcm.OurZoneID,
       bcm.SupplierID, bcm.SupplierZoneID, bcs.ZoneID, bcs.Net, bcs.CurrencyID,
       bcs.RateValue, bcs.RateType, bcs.DurationInSeconds  
FROM   Billing_CDR_Main bcm WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt))
       LEFT JOIN Billing_CDR_Sale bcs (NOLOCK) ON  bcs.ID = bcm.ID
WHERE  bcm.Attempt >= @BeginDate
  AND  bcm.Attempt <= @EndDate
  AND  bcm.CustomerID = @CustomerID 
  AND  bcs.CurrencyID IS NOT NULL
  ------------------------------ 
    SET @Pivot = DATEADD(dd, 1, @Pivot)
END 


UPDATE #TempBillingCDR
SET Attempt =  dateadd(mi,@GMTShifttime,Attempt),
    Connect =  dateadd(mi,@GMTShifttime,Connect)

-- Apply the Time Shift 
SET @FromDate = dateadd(mi,@GMTShifttime,@FromDate);
SET @ToDate = dateadd(mi,@GMTShifttime,@ToDate);
-- getting the currency exchange rate for the selected customer  
----------------------------------------------------------
SELECT @CurrencyID = CurrencyID
FROM   CarrierProfile WITH(NOLOCK)
WHERE  ProfileID = (
           SELECT ProfileID
           FROM   CarrierAccount WITH(NOLOCK)
           WHERE  CarrierAccountID = @CustomerID
       )	
         	  
-- Exchange rates 
-----------------------------------------------------------
DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates 
SELECT gder1.Currency,
       gder1.Date,
       gder1.Rate / gder2.Rate
FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder1 
       LEFT JOIN dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder2
            ON  gder1.Date = gder2.Date
            AND gder2.Currency = @CurrencyID
            


SELECT 
       @Duration = SUM(DurationInSeconds)/60.0,
       @Amount = SUM(Net),
       @NumberOfCalls =  COUNT(*)
FROM  #TempBillingCDR

IF(@Duration IS NULL OR @Amount IS NULL) RETURN

BEGIN TRANSACTION;     
	
IF @InvoiceID IS NOT NULL
BEGIN
    DELETE Billing_Invoice_Costs
    FROM   Billing_Invoice_Costs WITH(NOLOCK,INDEX(IX_Billing_Invoice_Costs_Invoice))
    WHERE  InvoiceID = @InvoiceID
    
    DELETE Billing_Invoice_Details
    FROM   Billing_Invoice_Details WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice))
    WHERE  InvoiceID = @InvoiceID
    
    DELETE Billing_Invoice
    FROM   Billing_Invoice WITH(NOLOCK,INDEX(PK_Billing_Invoice))
    WHERE  InvoiceID = @InvoiceID
    
    SET @Serial = @OldSerial
END

-- saving the billing invoice 
INSERT INTO Billing_Invoice
(
    BeginDate, EndDate, IssueDate, DueDate, CustomerID, SupplierID, SerialNumber, InvoicePrintedNote,
    Duration, Amount, NumberOfCalls, CurrencyID, IsLocked, IsPaid, UserID
)

SELECT @From,
       @To,
       @IssueDate,
       @DueDate,
       @CustomerID,
       'SYS',
       @Serial,
       @OldNotes,
       0,	--ISNULL(@Duration,0)/60.0,
       0,	--ISNULL(@Amount,0),
       @NumberOfCalls,
       @CurrencyID,
       'N',
       'N',
       @UserID 
       
       -- Getting the Invoice ID saved 
SELECT @InvoiceID = @@IDENTITY 

-- Getting the Billing Invoice Details 

INSERT INTO Billing_Invoice_Details
(
    InvoiceID, Destination, FromDate, TillDate, Duration, Rate, RateType, Amount, NumberOfCalls, CurrencyID, UserID
)
SELECT 
       @InvoiceID,
       B.ZoneID,
       MIN(B.Attempt),
       MAX(B.Attempt),
       SUM(B.DurationInSeconds)/60.0,
       Round(B.RateValue,5) ,
       B.RateType,
       ISNULL(Round(SUM(B.Net),2),0),
       COUNT(*),
       B.CurrencyID,
       @UserID
FROM   #TempBillingCDR B
GROUP BY
       B.ZoneID,
       Round(B.RateValue,5),
       B.RateType,
       B.CurrencyID
       

            
SELECT @Duration = SUM(bid.Duration),
       @NumberOfCalls = SUM(bid.NumberOfCalls)
FROM   Billing_Invoice_Details bid WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice))
WHERE  bid.InvoiceID = @InvoiceID

SELECT @Amount = SUM(B.Net / ISNULL(ERS.Rate, 1)) 
FROM #TempBillingCDR B
LEFT JOIN @ExchangeRates ERS ON ERS.Currency = B.CurrencyID AND ERS.Date = dbo.DateOf(B.Attempt)

UPDATE Billing_Invoice
SET    Duration = ISNULL(@Duration,0),
       Amount = ISNULL(@Amount,0) ,
       NumberOfCalls  = ISNULL(@NumberOfCalls,0) 
WHERE  InvoiceID = @InvoiceID

-- calculating the billing invoice costs (Gouped by the Out Carrier 'Supplier') 
INSERT INTO Billing_Invoice_Costs
(
    InvoiceID, SupplierID, Destination, FromDate, TillDate, Duration, Rate, RateType, Amount, NumberOfCalls, CurrencyID, UserID
)      
SELECT 
       @InvoiceID,
       B.SupplierID,
       B.SupplierZoneID,
       MIN(B.Attempt),
       MAX(B.Attempt),
       SUM(B.DurationInSeconds)/60.0,
       Round(B.RateValue,5),
       B.RateType,
       ISNULL(Round(SUM(B.Net),2),0),
       COUNT(*),
       B.CurrencyID,
       @UserID
FROM    #TempBillingCDR B
GROUP BY
       B.SupplierID,
       B.SupplierZoneID,
       Round(B.RateValue,5),
       B.RateType,
       B.CurrencyID
       
       -- updating the ZoneName 
UPDATE Billing_Invoice_Details
SET    Destination = Zone.Name
FROM   Billing_Invoice_Details WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice)),
       Zone WITH(NOLOCK)
WHERE  
		Billing_Invoice_Details.InvoiceID = @InvoiceID
		AND  Billing_Invoice_Details.Destination = Zone.ZoneID

UPDATE Billing_Invoice_Costs
SET    Destination = Zone.Name
FROM   Billing_Invoice_Costs WITH(NOLOCK,INDEX(IX_Billing_Invoice_Costs_Invoice)),
       Zone WITH(NOLOCK)
WHERE  Billing_Invoice_Costs.InvoiceID = @InvoiceID
	   And Billing_Invoice_Costs.Destination = Zone.ZoneID
	   

UPDATE Billing_Invoice
SET InvoiceNotes = @TimeZoneInfo WHERE InvoiceID = @InvoiceID

  -- Rollback the transaction if there were any errors
IF @@ERROR <> 0
BEGIN
    -- Rollback the transaction
    ROLLBACK
    
    -- Raise an error and return
    -- RAISERROR ('Error Creating Invoice', 16, 1)
    RETURN
END
COMMIT
	RETURN
GO
/****** Object:  StoredProcedure [dbo].[bp_CreateInvoice]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_CreateInvoice]
(
	@CustomerID varchar(5),
	@Serial varchar(50),
	@FromDate Datetime,
	@ToDate Datetime,
	@IssueDate Datetime,
	@DueDate Datetime,
	@UserID INT,
	@TimeZoneInfo VARCHAR(MAX),
	@InvoiceID int output
)
AS
SET NOCOUNT ON 

-- Creating the Invoice 	
DECLARE @Amount float 
DECLARE @Duration NUMERIC(19,6) 
DECLARE @NumberOfCalls INT 
DECLARE @CurrencyID varchar(3) 

SET @FromDate = CAST(
(
STR( YEAR( @FromDate ) ) + '-' +
STR( MONTH( @FromDate ) ) + '-' +
STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate = CAST(
(
STR( YEAR( @ToDate ) ) + '-' +
STR( MONTH(@ToDate ) ) + '-' +
STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)

-- building a temp table that contains all billing data requested 
--------------------------------------------------------------------------------------

DECLARE @Pivot SMALLDATETIME
SET @Pivot = @FromDate

SELECT * INTO #TempBillingStats 
FROM Billing_Stats bs WITH(NOLOCK) WHERE bs.CallDate='1995-01-01'

WHILE @Pivot <= @ToDate
BEGIN
	
    --------------    
INSERT INTO  #TempBillingStats
SELECT  *   
FROM Billing_Stats bs  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
WHERE  BS.CallDate = @Pivot
	  AND  BS.CustomerID = @CustomerID
	  AND  BS.Sale_Currency IS NOT NULL 
    --------------
SET @Pivot = DATEADD(dd, 1, @Pivot)
END 
-- getting the currency exchange rate for the selected customer  
----------------------------------------------------------
SELECT @CurrencyID = CurrencyID
FROM   CarrierProfile (NOLOCK)
WHERE  ProfileID = (
           SELECT ProfileID
           FROM   CarrierAccount (NOLOCK)
           WHERE  CarrierAccountID = @CustomerID
       )	
         	  
-- Exchange rates 
-----------------------------------------------------------
DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates 
SELECT gder1.Currency,
       gder1.Date,
       gder1.Rate / gder2.Rate
FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder1
       LEFT JOIN dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder2
            ON  gder1.Date = gder2.Date
            AND gder2.Currency = @CurrencyID

-- check if an invoice is issued before for the same customer 
-----------------------------------------------------------
DECLARE @OldSerial varchar(50)
DECLARE @OldNotes VARCHAR(MAX)
SELECT @InvoiceID = bi.InvoiceID,
       @OldSerial = bi.SerialNumber,
       @OldNotes = bi.InvoicePrintedNote
FROM   Billing_Invoice bi WITH(NOLOCK,INDEX(IX_Billing_Invoice_BeginDate,IX_Billing_Invoice_Customer))
WHERE (
	     bi.BeginDate BETWEEN @FromDate AND dbo.DateOf(@ToDate)
       OR 
         bi.EndDate BETWEEN @FromDate AND dbo.DateOf(@ToDate)
       )
  AND bi.CustomerID = @CustomerID

-- Check if there is data in billing stats 
----------------------------------------------------------
SELECT 
       @Duration = SUM(BS.SaleDuration)/60.0,
       @Amount = SUM(BS.Sale_Nets),
       @NumberOfCalls =  SUM(BS.NumberOfCalls)
FROM   #TempBillingStats BS

IF(@Duration IS NULL OR @Amount IS NULL) RETURN


BEGIN TRANSACTION;     

-- get the old invoiceID and the Serial number 
----------------------------------------------------------	
IF @InvoiceID IS NOT NULL
BEGIN
    DELETE Billing_Invoice_Costs
    FROM   Billing_Invoice_Costs WITH(NOLOCK,INDEX(IX_Billing_Invoice_Costs_Invoice))
    WHERE  InvoiceID = @InvoiceID
    
    DELETE Billing_Invoice_Details
    FROM   Billing_Invoice_Details WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice))
    WHERE  InvoiceID = @InvoiceID
    
    DELETE Billing_Invoice
    FROM   Billing_Invoice WITH(NOLOCK,INDEX(PK_Billing_Invoice))
    WHERE  InvoiceID = @InvoiceID
    
    SET @Serial = @OldSerial
END

-- saving the billing invoice 
-----------------------------------------------------------
INSERT INTO Billing_Invoice
(
    BeginDate, EndDate, IssueDate, DueDate, CustomerID, SupplierID, SerialNumber,InvoicePrintedNote, Duration, 
    Amount, NumberOfCalls, CurrencyID, IsLocked, IsPaid, UserID
)

SELECT @FromDate,
       @ToDate,
       @IssueDate,
       @DueDate,
       @CustomerID,
       'SYS',
       @Serial,
       @OldNotes,
       0,
       0,
       @NumberOfCalls,
       @CurrencyID,
       'N',
       'N',
       @UserID 
       
-- Getting the Invoice ID saved 
SELECT @InvoiceID = @@IDENTITY 
UPDATE Billing_Invoice
SET    EndDate = dateadd(dd,-1,EndDate)
WHERE  InvoiceID = @InvoiceID


-- Getting the Billing Invoice Details 

INSERT INTO Billing_Invoice_Details
(
    InvoiceID, Destination, FromDate, TillDate, Duration, Rate, RateType, Amount, NumberOfCalls, CurrencyID, UserID
)
SELECT 
       @InvoiceID,
       BS.SaleZoneID,
       MIN(BS.CallDate),
       MAX(BS.CallDate),
       SUM(BS.SaleDuration)/60.0,
       Round(BS.Sale_Rate,5),
       BS.Sale_RateType,
       ISNULL(Round(SUM(BS.Sale_Nets),2),0),
       SUM(BS.NumberOfCalls),
       BS.Sale_Currency,
       @UserID
FROM   #TempBillingStats BS 
GROUP BY
       BS.SaleZoneID,
       Round(BS.Sale_Rate,5),
       BS.Sale_RateType,
       BS.Sale_Currency

-----------------------------------------------------------

SELECT @Duration = SUM(bid.Duration),
       @NumberOfCalls = SUM(bid.NumberOfCalls)
FROM   Billing_Invoice_Details bid WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice))
WHERE  bid.InvoiceID = @InvoiceID

SELECT @Amount = SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) 
FROM #TempBillingStats BS
LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = dbo.DateOf(BS.CallDate)

-----------------------------------------------------------

UPDATE Billing_Invoice
SET    Duration = ISNULL(@Duration,0),
       Amount = ISNULL(@Amount,0), 
       NumberOfCalls = ISNULL(@NumberOfCalls,0)
WHERE  InvoiceID = @InvoiceID

-- calculating the billing invoice costs (Gouped by the Out Carrier 'Supplier') 
INSERT INTO Billing_Invoice_Costs
(
    InvoiceID, SupplierID, Destination, FromDate, TillDate, Duration, Rate, 
    RateType, Amount, NumberOfCalls, CurrencyID,UserID
)      
SELECT 
       @InvoiceID,
       BS.SupplierID,
       BS.CostZoneID,
       MIN(BS.CallDate),
       MAX(BS.CallDate),
       SUM(BS.SaleDuration)/60.0,
       Round(BS.Sale_Rate,5),
       BS.Sale_RateType,
       ISNULL(Round(SUM(BS.Sale_Nets),2),0),
       SUM(BS.NumberOfCalls),
       BS.Sale_Currency,
       @UserID
FROM   #TempBillingStats BS 
GROUP BY
       BS.SupplierID,
       BS.CostZoneID,
       Round(BS.Sale_Rate,5),
       BS.Sale_RateType,
       BS.Sale_Currency 
       
       -- updating the ZoneName 
UPDATE Billing_Invoice_Details
SET    Destination = Zone.Name
FROM   Billing_Invoice_Details WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice)),
       Zone WITH(NOLOCK)
WHERE  
		Billing_Invoice_Details.InvoiceID = @InvoiceID
		AND  Billing_Invoice_Details.Destination = Zone.ZoneID

UPDATE Billing_Invoice_Costs
SET    Destination = Zone.Name
FROM   Billing_Invoice_Costs WITH(NOLOCK,INDEX(IX_Billing_Invoice_Costs_Invoice)),
       Zone WITH(NOLOCK)
WHERE  Billing_Invoice_Costs.InvoiceID = @InvoiceID
	   And Billing_Invoice_Costs.Destination = Zone.ZoneID
  
UPDATE Billing_Invoice
SET InvoiceNotes = @TimeZoneInfo  WHERE InvoiceID = @InvoiceID
  
  -- Rollback the transaction if there were any errors
IF @@ERROR <> 0
BEGIN
    -- Rollback the transaction
    ROLLBACK
    
    -- Raise an error and return
    -- RAISERROR ('Error Creating Invoice', 16, 1)
    RETURN
END
COMMIT
	RETURN
GO
/****** Object:  StoredProcedure [dbo].[bp_BuildRoutesOld]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--
-- Build the Route and Route Option Tables
--
CREATE PROCEDURE [dbo].[bp_BuildRoutesOld]
	 @RebuildCodeSupply char(1) = 'Y' -- Y to Rebuild the code supply table
	,@CheckToD char(1) = 'Y' -- Y to Check the ToD Considerations after the build
	,@CheckSpecialRequests char(1) = 'Y' -- Y to Check special requests and alter the tables accordingly
	,@CheckRouteBlocks char(1) = 'Y' -- Y to check route blocks	
	,@MaxSuppliersPerRoute INT = 10
	,@IncludeBlockedZones CHAR(1) = 'N'
	,@UpdateStamp datetime output
WITH RECOMPILE
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @RowCount bigint

	DECLARE @State_Blocked tinyint
	DECLARE @State_Enabled tinyint
	SET @State_Blocked = 0
	SET @State_Enabled = 1

	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	

	-- If Rebuild Routes is already running error and return
	DECLARE @IsRunning char(1)
	SELECT @IsRunning = 'Y' FROM SystemMessage WHERE MessageID = 'BuildRoutes: Status' AND [Message] IS NOT NULL
	IF @IsRunning = 'Y' 
	BEGIN
		RAISERROR (N'Build Routes is already Runnning', 15, 1); 
		RETURN 
	END 

	DECLARE @MessageID varchar(50) 
	DECLARE @Description varchar(450) 
	DECLARE @Message varchar(500) 

	DELETE FROM SystemMessage WHERE MessageID LIKE 'BuildRoutes: %'
	
	SET @Message = CONVERT(varchar, getdate(), 121) 
	EXEC bp_SetSystemMessage 'BuildRoutes: Start', @Message 	

	SET @Message = ('Build Routes Started at: ' + CONVERT(varchar, getdate(), 121)) 
	EXEC bp_SetSystemMessage 'BuildRoutes: Status', @Message 
	
	SET @UpdateStamp = getdate() 
	DECLARE @TruncateLogSQL varchar(max) 
	SELECT @TruncateLogSQL = 'BACKUP LOG ' + db_name() + ' WITH TRUNCATE_ONLY' 
	
	-- Re-Build Code Supply if required 
	IF @RebuildCodeSupply = 'Y' 
	BEGIN 
		-- Build Zone Rates 
		EXEC bp_BuildZoneRates @IncludeBlockedZones=@IncludeBlockedZones 

		-- Fix Unsold Zones 
		EXEC bp_FixUnsoldZonesForRouteBuild 
		
		-- Rebuild the Code Supply
		EXEC bp_BuildCodeSupply 

		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Code Supply Rebuilt', @Message
	END
	
	ALTER TABLE [Route] NOCHECK CONSTRAINT ALL
	TRUNCATE TABLE [Route]

	-- DROP Route INDEXES
	DROP INDEX [IX_Route_Code] ON [Route]
	DROP INDEX [IX_Route_Customer] ON [Route]
	DROP INDEX [IX_Route_Zone] ON [Route]
	DROP INDEX [IX_Route_ServicesFlag] ON [Route]
	DROP INDEX [IX_Route_Updated] ON [Route]

	INSERT INTO Route 
	(
		CustomerID,
		Code,
		OurZoneID,
		OurActiveRate,
		OurNormalRate,
		OurOffPeakRate,
		OurWeekendRate,
		OurServicesFlag,
		[State],		
		Updated	
	)
	SELECT 
		R.CustomerID, 
		CM.Code, 
		CM.SupplierZoneID,
		R.NormalRate,
		R.NormalRate,
		R.OffPeakRate,
		R.WeekendRate,
		R.ServicesFlag,
		@State_Enabled,
		@UpdateStamp
	FROM
		CarrierAccount A WITH (NOLOCK, index(PK_CarrierAccount))
		INNER JOIN ZoneRate R WITH (NOLOCK, index(IX_ZoneRate_Customer, IX_ZoneRate_Supplier, IX_ZoneRate_Zone)) ON A.CarrierAccountID = R.CustomerID
		INNER JOIN CodeMatch CM WITH (NOLOCK, index(IDX_CodeMatch_Zone)) ON CM.SupplierZoneID = R.ZoneID AND R.SupplierID = 'SYS'
	WHERE
			A.ActivationStatus <> @Account_Inactive
		AND A.RoutingStatus <> @Account_Blocked
		AND A.RoutingStatus <> @Account_BlockedInbound
	ORDER BY R.CustomerID, CM.Code
	OPTION (RECOMPILE)

	ALTER TABLE [Route] CHECK CONSTRAINT ALL
	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: Routes Inserted', @Message

	CREATE INDEX [IX_Route_Code] ON [Route]([Code] ASC) -- ON INDEXES
	CREATE INDEX [IX_Route_Zone] ON [Route]([OurZoneID] ASC) -- ON INDEXES
	CREATE INDEX [IX_Route_Customer] ON [Route]([CustomerID] ASC) -- ON INDEXES
	CREATE INDEX [IX_Route_ServicesFlag] ON [Route]([OurServicesFlag] ASC) -- ON INDEXES
	CREATE INDEX [IX_Route_Updated] ON [Route](Updated DESC) -- ON INDEXES
  	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: Routes Indexes Re-Created', @Message

	-- Clear Route Options
	TRUNCATE TABLE [RouteOption]
	-- DROP INDEX IDX_RouteOption_RouteID ON RouteOption
	DROP INDEX IDX_RouteOption_SupplierZoneID ON RouteOption
	DROP INDEX IDX_RouteOption_Updated ON RouteOption
	ALTER TABLE [RouteOption] NOCHECK CONSTRAINT ALL

	-- EXECUTE( @TruncateLogSQL )
	DECLARE @CurrentCustomer VARCHAR(5)
	DECLARE @CurrentProfile INT 
	
	DECLARE customersCursor CURSOR LOCAL FAST_FORWARD FOR 
		SELECT 
			DISTINCT CustomerID 
			FROM [Route] 
			ORDER BY CustomerID;  

	OPEN customersCursor
	FETCH NEXT FROM customersCursor INTO @CurrentCustomer

	WHILE @@FETCH_STATUS = 0
	BEGIN
		BEGIN TRANSACTION; 
		
		SELECT @CurrentProfile = ProfileID FROM CarrierAccount ca WHERE ca.CarrierAccountID = @CurrentCustomer;

		WITH TheOptions AS
		(
			SELECT
				Rt.RouteID,
				CS.SupplierID,
				CS.SupplierZoneID,
				CS.SupplierNormalRate AS SupplierActiveRate,
				CS.SupplierNormalRate,
				CS.SupplierOffPeakRate,
				CS.SupplierWeekendRate,
				CS.SupplierServicesFlag,
				(ROW_NUMBER() OVER (PARTITION BY Rt.RouteID ORDER BY CS.SupplierNormalRate)) as RowNumber,
				1 AS NumberOfTries,
				@State_Enabled AS [State]
			FROM
				[Route] Rt WITH (NOLOCK, INDEX(IX_Route_Customer))
					, CodeSupply CS WITH (NOLOCK, INDEX(IX_CodeSupply_Code, IX_CodeSupply_Supplier))
					, CarrierAccount S WITH (NOLOCK) -- Supplier
				WHERE Rt.CustomerID = @CurrentCustomer
					AND Rt.Code = CS.Code	
					AND CS.SupplierID = S.CarrierAccountID
					AND	S.ActivationStatus <> @Account_Inactive 
					AND S.RoutingStatus <> @Account_Blocked 
					AND S.RoutingStatus <> @Account_BlockedOutbound
					AND S.ProfileID <> @CurrentProfile -- Prevent Looping
					AND (CS.SupplierServicesFlag & Rt.OurServicesFlag) = Rt.OurServicesFlag
					AND CS.SupplierNormalRate < Rt.OurNormalRate				
		)

		-- Build Route Options!
		INSERT INTO RouteOption
		(
			RouteID,
			SupplierID,
			SupplierZoneID,
			SupplierActiveRate,
			SupplierNormalRate,
			SupplierOffPeakRate,
			SupplierWeekendRate,
			SupplierServicesFlag,
			Priority,
			NumberOfTries,
			[State],
			Updated
		)
		SELECT 
			RouteID,
			SupplierID,
			SupplierZoneID,
			SupplierActiveRate,
			SupplierNormalRate,
			SupplierOffPeakRate,
			SupplierWeekendRate,
			SupplierServicesFlag,
			0,
			NumberOfTries,
			[State],
			GETDATE()
		FROM TheOptions WHERE RowNumber <= @MaxSuppliersPerRoute 
		
		COMMIT
		
		SET @Message = CONVERT(varchar, getdate(), 121)
		SET @MessageID = 'BuildRoutes: Customer: ' + @CurrentCustomer
		EXEC bp_SetSystemMessage @MessageID, @Message
		
		FETCH NEXT FROM customersCursor INTO @CurrentCustomer
	END

	CLOSE customersCursor
	DEALLOCATE customersCursor

	-- EXECUTE( @TruncateLogSQL )

	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: Route Options Inserted', @Message

	ALTER TABLE [RouteOption] CHECK CONSTRAINT ALL

	-- CREATE INDEX IDX_RouteOption_RouteID ON RouteOption(RouteID) -- ON INDEXES
	-- Message
	-- SET @Message = CONVERT(varchar, getdate(), 121)
	-- EXEC bp_SetSystemMessage 'BuildRoutes: RouteID Index Built for RouteOptions', @Message

	CREATE INDEX IDX_RouteOption_SupplierZoneID ON RouteOption(SupplierZoneID) -- ON INDEXES
	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: SupplierZoneID Index Built for RouteOptions', @Message

	CREATE INDEX IDX_RouteOption_Updated ON RouteOption(Updated) -- ON INDEXES
	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: [Updated] Index Built for RouteOptions', @Message
	
	-- Check ToD
	IF @CheckToD = 'Y'
	BEGIN
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Processing ToD', @Message

		EXEC bp_UpdateRoutesFromToD @Check='N'
		-- EXECUTE( @TruncateLogSQL )

		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: ToD Processed', @Message
	End	
		
	-- Check Special Requests
	IF @CheckSpecialRequests = 'Y'
	BEGIN
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Processing Special Requests', @Message
		
		-- Special Requests
		EXEC bp_UpdateRoutesFromSpecialRequests @Check='N'
		
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Special Requests Processed', @Message
	End

	-- Route Overrides
	EXEC bp_UpdateRoutingFromOverrides

	-- Check Route Blocks
	IF @CheckRouteBlocks = 'Y'
	BEGIN
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Processing Route Blocks', @Message

		-- Route Blocks
		EXEC bp_UpdateRoutesFromRouteBlocks @Check='N'
		
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Route Blocks Processed', @Message
	END

	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: End', @Message

	-- Get the last changed route of the route build
	SELECT @UpdateStamp = MAX(Updated) FROM [Route]

	-- Set Status to NULL (Not Running)
	EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = NULL

	RETURN;
END
GO
/****** Object:  StoredProcedure [dbo].[bp_BuildRoutes]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--
-- Build the Route and Route Option Tables
--
CREATE PROCEDURE [dbo].[bp_BuildRoutes]
	 @RebuildCodeSupply char(1) = 'Y' -- Y to Rebuild the code supply table
	,@CheckToD char(1) = 'Y' -- Y to Check the ToD Considerations after the build
	,@CheckSpecialRequests char(1) = 'Y' -- Y to Check special requests and alter the tables accordingly
	,@CheckRouteBlocks char(1) = 'Y' -- Y to check route blocks	
	,@MaxSuppliersPerRoute INT = 10
	,@IncludeBlockedZones CHAR(1) = 'N'
	,@UpdateStamp datetime output
	,@RoutingTableFileGroup nvarchar(255) = 'PRIMARY'
	,@RoutingIndexesFileGroup nvarchar(255) = 'PRIMARY'
	,@SORT_IN_TEMPDB nvarchar(10) = 'ON' 
WITH RECOMPILE
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @RowCount bigint

	DECLARE @State_Blocked tinyint
	DECLARE @State_Enabled tinyint
	SET @State_Blocked = 0
	SET @State_Enabled = 1

	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	

	-- If Rebuild Routes is already running error and return
	DECLARE @IsRunning char(1)
	SELECT @IsRunning = 'Y' FROM SystemMessage WHERE MessageID = 'BuildRoutes: Status' AND [Message] IS NOT NULL
	IF @IsRunning = 'Y' 
	BEGIN
		RAISERROR (N'Build Routes is already Runnning', 15, 1); 
		RETURN 
	END 

	DECLARE @MessageID varchar(50) 
	DECLARE @Description varchar(450) 
	DECLARE @Message varchar(500) 

	DELETE FROM SystemMessage WHERE MessageID LIKE 'BuildRoutes: %'
	
	SET @Message = CONVERT(varchar, getdate(), 121) 
	EXEC bp_SetSystemMessage 'BuildRoutes: Start', @Message 	

	SET @Message = ('Build Routes Started at: ' + CONVERT(varchar, getdate(), 121)) 
	EXEC bp_SetSystemMessage 'BuildRoutes: Status', @Message 
	
	SET @UpdateStamp = getdate() 
	DECLARE @TruncateLogSQL varchar(max) 
	SELECT @TruncateLogSQL = 'BACKUP LOG ' + db_name() + ' WITH TRUNCATE_ONLY' 
	
	-- Re-Build Code Supply if required 
	IF @RebuildCodeSupply = 'Y' 
	BEGIN 
		-- Build Zone Rates 
		EXEC bp_BuildZoneRates @IncludeBlockedZones=@IncludeBlockedZones 

		-- Fix Unsold Zones 
		EXEC bp_FixUnsoldZonesForRouteBuild 
		
		-- Rebuild the Code Supply
		EXEC bp_BuildCodeSupply 

		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Code Supply Rebuilt', @Message
	END
	
	-- Make sure to Define Parameters for table creation properly
	IF ISNULL(@SORT_IN_TEMPDB, '') = '' SET @SORT_IN_TEMPDB = 'ON'
	IF @SORT_IN_TEMPDB <> 'ON' SET @SORT_IN_TEMPDB = 'OFF'
	IF ISNULL(@RoutingTableFileGroup, '') = '' SET @RoutingTableFileGroup = 'PRIMARY'
	IF ISNULL(@RoutingIndexesFileGroup, '') = '' SET @RoutingIndexesFileGroup = 'PRIMARY'

	DECLARE @RouteTableCreationSQL nvarchar(max) 
	DECLARE @RouteIndexesCreationSQL nvarchar(max)
	DECLARE @RouteOptionTableCreationSQL nvarchar(max) 
	DECLARE @RouteOptionIndexesCreationSQL nvarchar(max)

	SET @RouteTableCreationSQL = '
		-- Temp Route
		CREATE TABLE [dbo].[TempRoute](
			[RouteID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
			[CustomerID] [varchar](5) NOT NULL,
			[Code] [varchar](15) NULL,
			[OurZoneID] [int] NULL,
			[OurActiveRate] [real] NULL,
			[OurNormalRate] [real] NULL,
			[OurOffPeakRate] [real] NULL,
			[OurWeekendRate] [real] NULL,
			[OurServicesFlag] [smallint] NULL,
			[State] [tinyint] NOT NULL,
			[Updated] [datetime] NULL,
			[IsToDAffected] [char](1) NOT NULL DEFAULT(''N''),
			[IsSpecialRequestAffected] [char](1) NOT NULL DEFAULT(''N''),
			[IsBlockAffected] [char](1) NOT NULL DEFAULT(''N''),
		) ON [' + @RoutingTableFileGroup + ']
	';

	SET @RouteIndexesCreationSQL = '
		CREATE NONCLUSTERED INDEX [IX_Route_Code] ON [dbo].[TempRoute]([Code] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
		CREATE NONCLUSTERED INDEX [IX_Route_Customer] ON [dbo].[TempRoute]([CustomerID] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
		CREATE NONCLUSTERED INDEX [IX_Route_Updated] ON [dbo].[TempRoute]([Updated] DESC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
		CREATE NONCLUSTERED INDEX [IX_Route_Zone] ON [dbo].[TempRoute]([OurZoneID] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
	';

	Set @RouteOptionTableCreationSQL = '
 		-- Temp Route Option
		CREATE TABLE [dbo].[TempRouteOption](
			[RouteID] [int] NOT NULL,
			[SupplierID] [varchar](5) NOT NULL,
			[SupplierZoneID] [int] NULL,
			[SupplierActiveRate] [real] NULL,
			[SupplierNormalRate] [real] NULL,
			[SupplierOffPeakRate] [real] NULL,
			[SupplierWeekendRate] [real] NULL,
			[SupplierServicesFlag] [smallint] NULL,
			[Priority] [tinyint] NOT NULL,
			[NumberOfTries] [tinyint] NULL,
			[State] [tinyint] NOT NULL DEFAULT ((0)),
			[Updated] [datetime] NULL,
			[Percentage] [tinyint] NULL
		) ON [' + @RoutingTableFileGroup + ']
		CREATE CLUSTERED INDEX [IDX_RouteOption_RouteID] ON [dbo].[TempRouteOption]([RouteID] ASC)
	';

	SET @RouteOptionIndexesCreationSQL = '
		CREATE NONCLUSTERED INDEX [IDX_RouteOption_SupplierZoneID] ON [dbo].[TempRouteOption]([SupplierZoneID] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
		CREATE NONCLUSTERED INDEX [IDX_RouteOption_Updated] ON [dbo].[TempRouteOption]([Updated] DESC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
	';

	-- Create Temp Working Route Table
	IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TempRoute]') AND type in (N'U'))
	BEGIN
		DROP TABLE [dbo].[TempRoute]
	END
	EXEC (@RouteTableCreationSQL)

	INSERT INTO [dbo].[TempRoute]
	(
		CustomerID,
		Code,
		OurZoneID,
		OurActiveRate,
		OurNormalRate,
		OurOffPeakRate,
		OurWeekendRate,
		OurServicesFlag,
		[State],		
		Updated	
	)
	SELECT 
		R.CustomerID, 
		CM.Code, 
		CM.SupplierZoneID,
		R.NormalRate,
		R.NormalRate,
		R.OffPeakRate,
		R.WeekendRate,
		R.ServicesFlag,
		@State_Enabled,
		@UpdateStamp
	FROM
		CarrierAccount A WITH (NOLOCK, index(PK_CarrierAccount))
		INNER JOIN ZoneRate R WITH (NOLOCK, index(IX_ZoneRate_Customer, IX_ZoneRate_Supplier, IX_ZoneRate_Zone)) ON A.CarrierAccountID = R.CustomerID
		INNER JOIN CodeMatch CM WITH (NOLOCK, index(IDX_CodeMatch_Zone)) ON CM.SupplierZoneID = R.ZoneID AND R.SupplierID = 'SYS'
	WHERE
			A.ActivationStatus <> @Account_Inactive
		AND A.RoutingStatus <> @Account_Blocked
		AND A.RoutingStatus <> @Account_BlockedInbound
	ORDER BY R.CustomerID, CM.Code
	OPTION (RECOMPILE)

	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: Routes Inserted', @Message
	
	EXEC (@RouteIndexesCreationSQL)  	
  	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: Routes Indexes Re-Created', @Message

	-- Temp Route Option
	IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TempRouteOption]') AND type in (N'U'))
	BEGIN
		DROP TABLE [dbo].[TempRouteOption]
	END
	EXEC (@RouteOptionTableCreationSQL)

	-- EXECUTE( @TruncateLogSQL )
	DECLARE @CurrentCustomer VARCHAR(5)
	DECLARE @CurrentProfile INT 
	
	DECLARE customersCursor CURSOR LOCAL FAST_FORWARD FOR 
		SELECT 
			DISTINCT CustomerID 
			FROM [dbo].[TempRoute]
			ORDER BY CustomerID;  

	OPEN customersCursor
	FETCH NEXT FROM customersCursor INTO @CurrentCustomer

	WHILE @@FETCH_STATUS = 0
	BEGIN
		BEGIN TRANSACTION; 
		
		SELECT @CurrentProfile = ProfileID FROM CarrierAccount ca WHERE ca.CarrierAccountID = @CurrentCustomer;

		WITH TheOptions AS
		(
			SELECT
				Rt.RouteID,
				CS.SupplierID,
				CS.SupplierZoneID,
				CS.SupplierNormalRate AS SupplierActiveRate,
				CS.SupplierNormalRate,
				CS.SupplierOffPeakRate,
				CS.SupplierWeekendRate,
				CS.SupplierServicesFlag,
				(ROW_NUMBER() OVER (PARTITION BY Rt.RouteID ORDER BY CS.SupplierNormalRate)) as RowNumber,
				1 AS NumberOfTries,
				@State_Enabled AS [State]
			FROM
				[dbo].[TempRoute] Rt WITH (NOLOCK, INDEX(IX_Route_Customer))
					, CodeSupply CS WITH (NOLOCK, INDEX(IX_CodeSupply_Code, IX_CodeSupply_Supplier))
					, CarrierAccount S WITH (NOLOCK) -- Supplier
				WHERE Rt.CustomerID = @CurrentCustomer
					AND Rt.Code = CS.Code COLLATE DATABASE_DEFAULT	
					AND CS.SupplierID = S.CarrierAccountID COLLATE DATABASE_DEFAULT
					AND	S.ActivationStatus <> @Account_Inactive 
					AND S.RoutingStatus <> @Account_Blocked 
					AND S.RoutingStatus <> @Account_BlockedOutbound
					AND S.ProfileID <> @CurrentProfile -- Prevent Looping
					AND (CS.SupplierServicesFlag & Rt.OurServicesFlag) = Rt.OurServicesFlag
					AND CS.SupplierNormalRate <= Rt.OurNormalRate				
		)

		-- Build Route Options!
		INSERT INTO [dbo].[TempRouteOption]
		(
			RouteID,
			SupplierID,
			SupplierZoneID,
			SupplierActiveRate,
			SupplierNormalRate,
			SupplierOffPeakRate,
			SupplierWeekendRate,
			SupplierServicesFlag,
			Priority,
			NumberOfTries,
			[State],
			Updated
		)
		SELECT 
			RouteID,
			SupplierID,
			SupplierZoneID,
			SupplierActiveRate,
			SupplierNormalRate,
			SupplierOffPeakRate,
			SupplierWeekendRate,
			SupplierServicesFlag,
			0,
			NumberOfTries,
			[State],
			GETDATE()
		FROM TheOptions WHERE RowNumber <= @MaxSuppliersPerRoute 
		
		COMMIT
		
		SET @Message = CONVERT(varchar, getdate(), 121)
		SET @MessageID = 'BuildRoutes: Customer: ' + @CurrentCustomer
		EXEC bp_SetSystemMessage @MessageID, @Message
		
		FETCH NEXT FROM customersCursor INTO @CurrentCustomer
	END

	CLOSE customersCursor
	DEALLOCATE customersCursor

	-- EXECUTE( @TruncateLogSQL )

	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: Route Options Inserted', @Message

	EXEC (@RouteOptionIndexesCreationSQL)
	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: Indexes Built for RouteOptions', @Message
	
	-- Rename Temp to Route and Route Options
	BEGIN TRANSACTION		
		DROP TABLE [dbo].[Route]
		EXEC sp_rename 'TempRoute', 'Route'
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: [TempRoutes] Renamed to [Route]', @Message
		
		DROP TABLE [dbo].[RouteOption]
		EXEC sp_rename 'TempRouteOption', 'RouteOption'
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: [TempRouteOptions] Renamed to [RouteOptions]', @Message
	COMMIT TRANSACTION

	-- Check ToD
	IF @CheckToD = 'Y'
	BEGIN
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Processing ToD', @Message

		EXEC bp_UpdateRoutesFromToD @Check='N'
		-- EXECUTE( @TruncateLogSQL )

		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: ToD Processed', @Message
	End	
		
	-- Check Special Requests
	IF @CheckSpecialRequests = 'Y'
	BEGIN
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Processing Special Requests', @Message
		
		-- Special Requests
		EXEC bp_UpdateRoutesFromSpecialRequests @Check='N'
		
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Special Requests Processed', @Message
	End

	-- Route Overrides
	EXEC bp_UpdateRoutingFromOverrides

	-- Check Route Blocks
	IF @CheckRouteBlocks = 'Y'
	BEGIN
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Processing Route Blocks', @Message

		-- Route Blocks
		EXEC bp_UpdateRoutesFromRouteBlocks @Check='N'
		
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Route Blocks Processed', @Message
	END

	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: End', @Message

	-- Get the last changed route of the route build
	SELECT @UpdateStamp = MAX(Updated) FROM [Route]

	-- Set Status to NULL (Not Running)
	EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = NULL

	RETURN;
END
GO
/****** Object:  StoredProcedure [dbo].[rpt_DailySummaryForcasting]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rpt_DailySummaryForcasting](
	@FromDate Datetime ,
	@ToDate Datetime 
)
with Recompile
	AS 
		SET @FromDate=     CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	

		DECLARE @ExchangeRates TABLE(
						Currency VARCHAR(3),
						Date SMALLDATETIME,
						Rate FLOAT
						PRIMARY KEY(Currency, Date)
						)

		INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

           SELECT 
				CAST(bs.calldate AS varchar(11))  [Day],  
				SUM(bs.sale_nets / ISNULL(ERS.Rate, 1))  AS SaleNet,           			
				SUM(bs.cost_nets / ISNULL(ERC.Rate, 1))  AS  CostNet
			FROM Billing_Stats bs  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
			LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate			
            LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
			WHERE bs.calldate >=@fromdate AND bs.calldate<@ToDate
			GROUP BY 
			    CAST(bs.calldate AS varchar(11))   
			ORDER BY CAST(bs.calldate AS varchar(11))   DESC	
	
	RETURN
GO
/****** Object:  StoredProcedure [dbo].[rpt_CustomerSummary]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rpt_CustomerSummary](
	@CustomerID varchar(10) = NULL,
	@FromDate Datetime ,
	@ToDate Datetime 
)
with Recompile
	AS 
		SET @FromDate=CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)


BEGIN
SELECT bs.CustomerID AS Carrier
	  ,SUM(bs.SaleDuration /60.0) AS SaleDuration
      ,SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS SaleNet
      ,SUM(bs.CostDuration /60.0) AS CostDuration
      ,SUM(bs.Cost_Nets / ISNULL(ERC.Rate, 1)) AS CostNet
FROM   Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
WHERE  (@CustomerID IS NULL OR  bs.CustomerID = @CustomerID)
 AND  bs.CallDate >= @FromDate AND  bs.CallDate <= @ToDate
GROUP BY bs.CustomerID

DECLARE @NumberOfDays INT 
SET @NumberOfDays = DATEDIFF(dd,@FromDate,@ToDate) + 1

--SELECT isnull(SUM(ca.Services + ca.ConnectionFees) * @NumberOfDays,0) AS Services
--    FROM CarrierAccount ca 
--WHERE ca.CarrierAccountID IN 
--(
--SELECT DISTINCT bs.SupplierID	
--  FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
--WHERE  (@CustomerID IS NULL OR  bs.CustomerID = @CustomerID)
--  AND  bs.CallDate >= @FromDate
--  AND  bs.CallDate <= @ToDate
--)

--SELECT ((SUM(isnull(ca.Services,0) +isnull(ca.ConnectionFees,0))/ISNULL(ERS.Rate,1))*@NumberOfDays) AS Services
--FROM CarrierAccount ca 
--LEFT JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
--LEFT JOIN @ExchangeRates ERS ON ERS.Currency = cp.CurrencyID AND ERS.Date = getdate()

SELECT ((SUM((isnull(ca.Services,0) +isnull(ca.ConnectionFees,0))/ISNULL(ER.Rate, 1)))*@NumberOfDays) AS Services
FROM CarrierAccount ca 
JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
LEFT JOIN @ExchangeRates ER ON ER.Currency = cp.CurrencyID AND ER.Date = @FromDate
END

RETURN
GO
/****** Object:  StoredProcedure [dbo].[rpt_ZoneSummaryDetailed]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[rpt_ZoneSummaryDetailed](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@Cost char(1)='Y',
	@CurrencyID varchar(3)
)

	AS 

	SET @FromDate=     CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	

DECLARE @ExchangeRates TABLE(
		CurrencyIn VARCHAR(3) COLLATE SQL_Latin1_General_CP1256_CI_AS,
		CurrencyOut VARCHAR(3) COLLATE SQL_Latin1_General_CP1256_CI_AS,
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(CurrencyIn,CurrencyOut, Date))

INSERT INTO @ExchangeRates 
SELECT gder1.Currency AS CurrencyIn,
		   gder2.Currency AS CurrencyOut,
		   gder1.Date AS Date, 
		   gder1.Rate / gder2.Rate AS Rate
FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder1
JOIN dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder2 ON  gder1.Date = gder2.Date

	IF (@Cost = 'Y')
	BEGIN 
		DECLARE @NumberOfDays INT 
		SET @NumberOfDays = DATEDIFF(dd,@FromDate,@ToDate) + 1
		IF(@CustomerID IS NULL AND @SupplierID IS NULL)
		BEGIN 
			SELECT 
				Z.Name AS Zone,
				SUM(bs.NumberOfCalls) AS Calls,
				bs.Cost_Rate/ISNULL(ERC.Rate, 1) AS Rate,
				bs.Cost_RateType AS RateType,
				SUM(bs.CostDuration/60.0) AS DurationInSeconds,
				SUM(bs.cost_nets /ISNULL(ERC.Rate, 1)) AS  Net,
				SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
				SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
			FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
			LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.CostZoneID   
			LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
			WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
			GROUP BY Z.Name,bs.Cost_Rate/ISNULL(ERC.Rate, 1),bs.Cost_RateType
			ORDER BY Z.Name ASC 
		END
		ELSE
			IF (@CustomerID IS NULL AND @SupplierID IS NOT NULL)
			BEGIN 
			SELECT 
				Z.Name AS Zone,
				SUM(bs.NumberOfCalls) AS Calls,
				bs.Cost_Rate/ISNULL(ERC.Rate, 1) AS Rate,
				bs.Cost_RateType AS RateType,
				SUM(bs.CostDuration/60.0) AS DurationInSeconds,
				SUM(bs.cost_nets /ISNULL(ERC.Rate, 1)) AS  Net,
				SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
				SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
			FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
			LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.CostZoneID   
			LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
			WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
	             AND bs.SupplierID=@SupplierID
			GROUP BY Z.Name,bs.Cost_Rate/ISNULL(ERC.Rate, 1),bs.Cost_RateType
			ORDER BY Z.Name ASC 	
			END 
		    ELSE 
		    	IF(@CustomerID IS NOT NULL AND @SupplierID IS NULL)
		    	BEGIN 
				SELECT 
					Z.Name AS Zone,
					SUM(bs.NumberOfCalls) AS Calls,
					bs.Cost_Rate/ISNULL(ERC.Rate, 1) AS Rate,
					bs.Cost_RateType AS RateType,
					SUM(bs.CostDuration/60.0) AS DurationInSeconds,
					SUM(bs.cost_nets /ISNULL(ERC.Rate, 1)) AS  Net,
					SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
					SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
				FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
				LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.CostZoneID   
				LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
				WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
					 AND bs.CustomerID=@CustomerID
				GROUP BY Z.Name,bs.Cost_Rate/ISNULL(ERC.Rate, 1),bs.Cost_RateType
				ORDER BY Z.Name ASC
		    	END
				ELSE
					IF(@CustomerID IS NOT NULL AND @SupplierID IS NOT NULL)
					BEGIN 
					SELECT 
						Z.Name AS Zone,
						SUM(bs.NumberOfCalls) AS Calls,
						bs.Cost_Rate/ISNULL(ERC.Rate, 1) AS Rate,
						bs.Cost_RateType AS RateType,
						SUM(bs.CostDuration/60.0) AS DurationInSeconds,
						--(bs.Cost_Rate/ISNULL(ERC.Rate, 1))*(SUM(bs.CostDuration/60.0)) AS Net
						SUM(bs.cost_nets /ISNULL(ERC.Rate, 1)) AS  Net,
						SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
						SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
					FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
					LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.CostZoneID   
					LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
					WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
						AND bs.CustomerID=@CustomerID
						AND bs.SupplierID=@SupplierID
					GROUP BY Z.Name,bs.Cost_Rate/ISNULL(ERC.Rate, 1),bs.Cost_RateType
					ORDER BY Z.Name ASC 		
					END
--					SELECT isnull(SUM(ca.Services + ca.ConnectionFees) * @NumberOfDays,0) AS Services
--					FROM CarrierAccount ca 
--					WHERE ca.CarrierAccountID IN 
--						(
--							SELECT DISTINCT bs.SupplierID	
--							FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
--							WHERE  bs.CustomerID = @CustomerID
--							AND  bs.CallDate >= @FromDate AND  bs.CallDate <= @ToDate
--						)	
					SELECT ((SUM((isnull(ca.Services,0) +isnull(ca.ConnectionFees,0))/ISNULL(ER.Rate, 1)))*@NumberOfDays) AS Services
					FROM CarrierAccount ca 
					JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
					LEFT JOIN @ExchangeRates ER ON ER.CurrencyIn = cp.CurrencyID AND ER.CurrencyOut = @CurrencyID AND ER.Date = @FromDate
	END
	ELSE 
	BEGIN 
		IF(@CustomerID IS NULL AND @SupplierID IS NULL)
		BEGIN 
			SELECT 
				Z.Name AS Zone,
				SUM(bs.NumberOfCalls) AS Calls,
				bs.Sale_Rate/ISNULL(ERS.Rate, 1) AS Rate,
				bs.Sale_RateType AS RateType,
				SUM(bs.SaleDuration/60.0) AS DurationInSeconds,
				--(bs.Sale_Rate/ISNULL(ERS.Rate, 1))*(SUM(bs.SaleDuration/60.0)) AS Net
				SUM(bs.Sale_nets /ISNULL(ERS.Rate, 1)) AS  Net,
				SUM(bs.Sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
				SUM(bs.Sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
			FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
			LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
			LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
			WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
			GROUP BY Z.Name,bs.Sale_Rate/ISNULL(ERS.Rate, 1),bs.Sale_RateType
			ORDER BY Z.Name ASC
		END
		ELSE 
			IF(@CustomerID IS NULL AND @SupplierID IS NOT NULL)
			BEGIN
				SELECT 
					Z.Name AS Zone,
					SUM(bs.NumberOfCalls) AS Calls,
					bs.Sale_Rate/ISNULL(ERS.Rate, 1) AS Rate,
					bs.Sale_RateType AS RateType,
					SUM(bs.SaleDuration/60.0) AS DurationInSeconds,
					--(bs.Sale_Rate/ISNULL(ERS.Rate, 1))*(SUM(bs.SaleDuration/60.0)) AS Net
					SUM(bs.Sale_nets /ISNULL(ERS.Rate, 1)) AS  Net,
					SUM(bs.Sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
					SUM(bs.Sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
				FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
				LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
				LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
				WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
					 AND bs.SupplierID=@SupplierID
				GROUP BY Z.Name,bs.Sale_Rate/ISNULL(ERS.Rate, 1),bs.Sale_RateType
				ORDER BY Z.Name ASC
			END
			ELSE
				IF(@CustomerID IS NOT NULL AND @SupplierID IS NULL)
				BEGIN
					SELECT 
						Z.Name AS Zone,
						SUM(bs.NumberOfCalls) AS Calls,
						bs.Sale_Rate/ISNULL(ERS.Rate, 1) AS Rate,
						bs.Sale_RateType AS RateType,
						SUM(bs.SaleDuration/60.0) AS DurationInSeconds,
						--(bs.Sale_Rate/ISNULL(ERS.Rate, 1))*(SUM(bs.SaleDuration/60.0)) AS Net
						SUM(bs.Sale_nets /ISNULL(ERS.Rate, 1)) AS  Net,
						SUM(bs.Sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
						SUM(bs.Sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
					FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
					LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
					LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
					WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
						 AND bs.CustomerID=@CustomerID
					GROUP BY Z.Name,bs.Sale_Rate/ISNULL(ERS.Rate, 1),bs.Sale_RateType
					ORDER BY Z.Name ASC
				END
				ELSE
					IF(@CustomerID IS NOT NULL AND @SupplierID IS NOT NULL)
					BEGIN
						 SELECT 
							Z.Name AS Zone,
							SUM(bs.NumberOfCalls) AS Calls,
							bs.Sale_Rate/ISNULL(ERS.Rate, 1) AS Rate,
							bs.Sale_RateType AS RateType,
							SUM(bs.SaleDuration/60.0) AS DurationInSeconds,
							--(bs.Sale_Rate/ISNULL(ERS.Rate, 1))*(SUM(bs.SaleDuration/60.0)) AS Net
							SUM(bs.Sale_nets /ISNULL(ERS.Rate, 1)) AS  Net,
							SUM(bs.Sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
							SUM(bs.Sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
						FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
						LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
						LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
						WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
							 AND bs.CustomerID=@CustomerID
							 AND bs.SupplierID=@SupplierID
						GROUP BY Z.Name,bs.Sale_Rate/ISNULL(ERS.Rate, 1),bs.Sale_RateType
						ORDER BY Z.Name ASC
					END
    END 
	RETURN
GO
/****** Object:  StoredProcedure [dbo].[rpt_ZoneSupplierSummary]    Script Date: 03/30/2011 15:31:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rpt_ZoneSupplierSummary]
(
	@FromDate Datetime,
	@ToDate Datetime,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@Top int = 100 
)
WITH Recompile
AS
SET NOCOUNT ON 
SET @FromDate=     CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate= CAST(
(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)	
 DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

CREATE TABLE #Zones(
			ID int,
			Durations float,
			PRIMARY KEY(ID))
	
INSERT INTO #Zones 
SELECT bs.SaleZoneID AS ID,
	   SUM(bs.SaleDuration)
FROM   Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
WHERE  bs.CallDate >= @FromDate
  AND  bs.CallDate < @ToDate
  AND  ( @CustomerID IS NULL OR  bs.CustomerID = @CustomerID )
  AND  ( @SupplierID IS NULL OR  bs.SupplierID = @SupplierID )
GROUP BY
	   bs.SaleZoneID 
ORDER BY SUM(bs.SaleDuration) DESC 

DECLARE @Zones TABLE(ID INT)

SET ROWCOUNT @Top
INSERT INTO @Zones SELECT #Zones.ID FROM #Zones ORDER BY Durations DESC 
SET ROWCOUNT 0

SELECT bs.SaleZoneID AS SaleZoneID,
	   bs.SupplierID AS SupplierID,
	   SUM(bs.SaleDuration)/60.0 AS Duration,
	   SUM(bs.Cost_Nets / ISNULL(ERC.Rate, 1)) AS CostNet,
	   SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS SaleNet,
	   avg(TS.ASR) AS ASR,
	   Avg(TS.ACD) AS ACD
FROM   Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
	LEFT JOIN GetSupplierZoneStats(@FromDate,@ToDate,NULL) AS TS ON TS.SupplierID = bs.SupplierID AND TS.OurZoneID = bs.SaleZoneID
	LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
	LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
WHERE  bs.CallDate >= @FromDate
  AND  bs.CallDate < @ToDate
  AND  ( @CustomerID IS NULL OR  bs.CustomerID = @CustomerID )
  AND  ( @SupplierID IS NULL OR  bs.SupplierID = @SupplierID )
  AND  bs.SaleZoneID IN (SELECT Id FROM @Zones)
GROUP BY
	   bs.SaleZoneID,
	   bs.SupplierID 
	   
DROP TABLE #Zones

RETURN
GO
/****** Object:  StoredProcedure [dbo].[Sp_AccountManagerStatus]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[Sp_AccountManagerStatus]
    @FromDate    datetime,
    @ToDate      DATETIME,
    @Type VARCHAR(20)
WITH RECOMPILE
AS
BEGIN   
    SET NOCOUNT ON;
    
    DECLARE @ExchangeRates TABLE(
        Currency VARCHAR(3),
        Date SMALLDATETIME,
        Rate FLOAT
        PRIMARY KEY(Currency, Date)
    )
   
    INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate);
IF(@Type = 'Customers')
  BEGIN  
    WITH TrafficTable AS
    (
    SELECT
        TS.CustomerID AS CarrierAccountID,
        Sum(Attempts) as Attempts,
        Sum(SuccessfulAttempts) AS SuccessfulAttempts,
        Sum(SuccessfulAttempts) * 100.0 / Sum(Attempts) as ASR,
        Sum(DurationsInSeconds /60.0) as DurationsInMinutes,
        case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))  
   WHERE  
        FirstCDRAttempt BETWEEN @FromDate AND @ToDate
    GROUP BY 
        TS.CustomerID
    ),

BillingTable AS 
(
SELECT 
        bs.CustomerID AS CarrierAccountID
       ,AVG(bs.Sale_Rate / ISNULL(ERS.Rate, 1)) AS SaleRate
       ,SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS SaleNet
       ,SUM(bs.SaleDuration / 60.0) AS SaleDuration
       ,AVG(bs.Cost_Rate / ISNULL(ERC.Rate, 1)) AS CostRate
       ,SUM(bs.Cost_Nets /ISNULL(ERC.Rate, 1)) AS CostNet
       ,SUM(bs.CostDuration / 60.0) AS CostDuration
       ,SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1) - bs.Cost_Nets / ISNULL(ERC.Rate, 1)) / sum(bs.SaleDuration / 60.0)  AS MarginPerMinute
       ,SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1) - bs.Cost_Nets / ISNULL(ERC.Rate, 1))  AS Margin  
FROM Billing_Stats bs  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
WHERE bs.CallDate BETWEEN  @FromDate AND @ToDate  
GROUP BY bs.CustomerID 
)

SELECT * FROM trafficTable T
LEFT JOIN BillingTable B 
ON T.CarrierAccountID = B.CarrierAccountID
 
  END
  
  IF(@Type = 'Suppliers')
  BEGIN  
    WITH TrafficTable AS
    (
    SELECT
        TS.SupplierID AS CarrierAccountID,
        Sum(Attempts) as Attempts,
        Sum(SuccessfulAttempts) AS SuccessfulAttempts,
        Sum(SuccessfulAttempts) * 100.0 / Sum(Attempts) as ASR,
        Sum(DurationsInSeconds /60.0) as DurationsInMinutes,
        case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))  
   WHERE  
        FirstCDRAttempt BETWEEN @FromDate AND @ToDate
    GROUP BY 
        TS.SupplierID
    ),

BillingTable AS 
(
SELECT 
        bs.SupplierID AS CarrierAccountID
       ,AVG(bs.Sale_Rate / ISNULL(ERS.Rate, 1)) AS SaleRate
       ,SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS SaleNet
       ,SUM(bs.SaleDuration / 60.0) AS SaleDuration
       ,AVG(bs.Cost_Rate / ISNULL(ERC.Rate, 1)) AS CostRate
       ,SUM(bs.Cost_Nets /ISNULL(ERC.Rate, 1)) AS CostNet
       ,SUM(bs.CostDuration / 60.0) AS CostDuration
       ,SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1) - bs.Cost_Nets / ISNULL(ERC.Rate, 1)) / sum(bs.SaleDuration / 60.0)  AS MarginPerMinute
       ,SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1) - bs.Cost_Nets / ISNULL(ERC.Rate, 1))  AS Margin  
FROM Billing_Stats bs  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
WHERE bs.CallDate BETWEEN  @FromDate AND @ToDate  
GROUP BY bs.SupplierID 
)

SELECT * FROM trafficTable T
LEFT JOIN BillingTable B 
ON T.CarrierAccountID = B.CarrierAccountID
 
  END
   
END
GO
/****** Object:  StoredProcedure [dbo].[rpt_Volumes_CompareInOutTraffic]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[rpt_Volumes_CompareInOutTraffic](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5),
	@Period varchar(7)
)
with Recompile
AS 
	SET @FromDate=     CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	
	
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

   INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

   Declare @Results Table(CallDate datetime,CallWeek int,CallMonth int,CallYear int,TrafficDirection varchar(10),
						   Duration numeric(13,5), Net numeric(13,5), 
                           PercDuration varchar(100), PercNet varchar(100), 
                           TotalDuration numeric(13,5), TotalNet numeric(13,5))

	IF(@Period = 'None')
		BEGIN
			INSERT INTO @Results(
				TrafficDirection,
				Duration,
				Net,
				PercDuration,
				PercNet,
				TotalDuration,
				TotalNet
			)
			SELECT
				  'IN' AS TrafficDirection, 
				  SUM(BS.SaleDuration)/60.0 AS  Duration,
				  SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Net,
				  0,0,0,0
			FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
			WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate AND BS.CustomerID=@CustomerID
			UNION ALL	
			SELECT 
				  'OUT' AS TrafficDirection, 
				  SUM(BS.CostDuration)/60.0 AS  Duration,
				  SUM(bs.Cost_Nets / ISNULL(ERS.Rate, 1)) AS Net,
				  0,0,0,0
			FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Cost_Currency AND ERS.Date = BS.CallDate
			WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate AND BS.SupplierID=@CustomerID 		
		END
		ELSE
			IF(@Period = 'Daily')
			BEGIN
				INSERT INTO @Results(
					CallDate,
					TrafficDirection,
					Duration,
					Net,
					PercDuration,
					PercNet,
					TotalDuration,
					TotalNet
				)
				SELECT
					  bs.CallDate AS CallDate,
					  'IN' AS TrafficDirection, 
					  SUM(BS.SaleDuration)/60.0 AS  Duration,
					  SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Net,
					  0,0,0,0
				FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
				LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
				WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate AND BS.CustomerID=@CustomerID
				GROUP BY bs.CallDate 
				UNION ALL	
				SELECT 
					  bs.CallDate AS CallDate,
					  'OUT' AS TrafficDirection, 
					  SUM(BS.CostDuration)/60.0 AS  Duration,
					  SUM(bs.Cost_Nets / ISNULL(ERS.Rate, 1)) AS Net,
					  0,0,0,0
				FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
				LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Cost_Currency AND ERS.Date = BS.CallDate
				WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
 					AND BS.SupplierID=@CustomerID 	
				GROUP BY bs.CallDate 
			END
			ELSE
				IF(@Period = 'Weekly')
				BEGIN
						INSERT INTO @Results(
						CallWeek,
						CallYear,
						TrafficDirection,
						Duration,
						Net,
						PercDuration,
						PercNet,
						TotalDuration,
						TotalNet
					)
					SELECT
						  datepart(week,BS.CallDate) AS CallWeek,
						  datepart(year,bs.CallDate) AS CallYear,
						  'IN' AS TrafficDirection, 
						  SUM(BS.SaleDuration)/60.0 AS  Duration,
						  SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Net,
						  0,0,0,0
					FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
					LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
					WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
						AND BS.CustomerID=@CustomerID
					GROUP BY DATEPART(year,BS.calldate),DATEPART(week,BS.calldate)
					UNION ALL	
					SELECT 
						  datepart(week,BS.CallDate) AS CallWeek,
						  datepart(year,bs.CallDate) AS CallYear,
						  'OUT' AS TrafficDirection, 
						  SUM(BS.CostDuration)/60.0 AS  Duration,
						  SUM(bs.Cost_Nets / ISNULL(ERS.Rate, 1)) AS Net,
						  0,0,0,0
					FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
					LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Cost_Currency AND ERS.Date = BS.CallDate
					WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
 						AND BS.SupplierID=@CustomerID 	
 					GROUP BY DATEPART(year,BS.calldate),DATEPART(week,BS.calldate)
				END
				ELSE 
					IF(@Period = 'Monthly')
						BEGIN
								INSERT INTO @Results(
								CallMonth,
								CallYear,
								TrafficDirection,
								Duration,
								Net,
								PercDuration,
								PercNet,
								TotalDuration,
								TotalNet
							)
							SELECT
							      datepart(month,BS.CallDate) AS CallMonth,
								  datepart(year,bs.CallDate) AS CallYear,
								  'IN' AS TrafficDirection, 
								  SUM(BS.SaleDuration)/60.0 AS  Duration,
								  SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Net,
								  0,0,0,0
							FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
							LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
							WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
 								AND BS.CustomerID=@CustomerID
							GROUP BY DATEPART(year,BS.calldate),DATEPART(month,BS.calldate)
							UNION ALL	
							SELECT 
	    						  datepart(month,BS.CallDate) AS CallMonth,
								  datepart(year,bs.CallDate) AS CallYear,
								  'OUT' AS TrafficDirection, 
								  SUM(BS.CostDuration)/60.0 AS  Duration,
								  SUM(bs.Cost_Nets / ISNULL(ERS.Rate, 1)) AS Net,
								  0,0,0,0
							FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
							LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Cost_Currency AND ERS.Date = BS.CallDate
							WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
 								AND BS.SupplierID=@CustomerID 	
							GROUP BY DATEPART(year,BS.calldate),DATEPART(month,BS.calldate)
						END

   
-- Calculate Totals, then Percentages
Update @Results SET 
	TotalDuration = (SELECT Sum(Duration) FROM @Results),
	TotalNet = (SELECT Sum(Net) FROM @Results)

Update @Results SET 
	PercDuration = TrafficDirection + ' ' + cast(cast((Duration / TotalDuration * 100) as numeric(13,2)) as varchar(20)) + '%' ,
	PercNet = TrafficDirection + ' ' + cast(cast((Net / TotalNet * 100) as numeric(13,2)) as varchar(20)) + '%' 

SELECT * 
FROM @Results 
ORDER BY CallYear,CallMonth,CallWeek,CallDate ASC

RETURN
GO
/****** Object:  StoredProcedure [dbo].[rpt_ExchangeCarriersSummary]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE    PROCEDURE [dbo].[rpt_ExchangeCarriersSummary](@FromDate DATETIME, @ToDate DATETIME)
WITH RECOMPILE
AS
	SET @FromDate = CAST(
	        (
	            STR(YEAR(@FromDate)) + '-' +
	            STR(MONTH(@FromDate)) + '-' +
	            STR(DAY(@FromDate))
	        )
	        AS DATETIME
	    )
	
	SET @ToDate = CAST(
	        (
	            STR(YEAR(@ToDate)) + '-' +
	            STR(MONTH(@ToDate)) + '-' +
	            STR(DAY(@ToDate)) + ' 23:59:59.99'
	        )
	        AS DATETIME
	    )
	
	DECLARE @ExchangeRates TABLE(
	            Currency VARCHAR(3),
	            Date SMALLDATETIME,
	            Rate FLOAT
	            PRIMARY KEY(Currency, Date)
	        )
	
	INSERT INTO @ExchangeRates
	SELECT *
	FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate);
	
	BEGIN
		--Customer part 
		WITH CustomerSummary AS 
		(
		    SELECT bs.CustomerID AS CarrierID,
		           SUM(isnull(bs.Sale_Nets,0) / ISNULL(ERS.Rate, 1)) AS Sale,
		           SUM(isnull(bs.Cost_Nets,0) / ISNULL(ERC.Rate, 1)) AS Cost
		    FROM   Billing_Stats bs WITH(NOLOCK, INDEX(IX_Billing_Stats_Date))
		           LEFT JOIN @ExchangeRates ERS
		                ON  ERS.Currency = bs.Sale_Currency
		                AND ERS.Date = bs.CallDate
		           LEFT JOIN @ExchangeRates ERC
		                ON  ERC.Currency = bs.Cost_Currency
		                AND ERC.Date = bs.CallDate
		    WHERE  bs.CallDate >= @FromDate
		           AND bs.CallDate <= @ToDate
		    GROUP BY
		           bs.CustomerID
		),
		SupplierSummary AS 
		(
		    -- Supplier part 
		    SELECT bs.SupplierID AS CarrierID,
		           SUM(isnull(bs.CostDuration,0) / 60.0) AS Duration,
		           SUM(isnull(bs.Sale_Nets,0) / ISNULL(ERS.Rate, 1)) AS Sale,
		           SUM(isnull(bs.Cost_Nets,0) / ISNULL(ERC.Rate, 1)) AS Cost
		    FROM   Billing_Stats bs WITH(NOLOCK, INDEX(IX_Billing_Stats_Date))
		           LEFT JOIN @ExchangeRates ERS
		                ON  ERS.Currency = bs.Sale_Currency
		                AND ERS.Date = bs.CallDate
		           LEFT JOIN @ExchangeRates ERC
		                ON  ERC.Currency = bs.Cost_Currency
		                AND ERC.Date = bs.CallDate
		    WHERE  bs.CallDate >= @FromDate
		           AND bs.CallDate <= @ToDate
		    GROUP BY
		           bs.SupplierID
		) 
		
		
		SELECT  CASE WHEN  CS.CarrierID IS NULL  THEN SS.CarrierID
		            ELSE   CS.CarrierID END AS CustomerID,
		       CS.Sale - CS.Cost AS CustomerProfit,
		       SS.Sale - SS.Cost AS SupplierProfit
		FROM   CustomerSummary CS
		       FULL JOIN SupplierSummary SS
		            ON  CS.CarrierID = SS.CarrierID
	END
	
	RETURN
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_SupplierCostZone]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TrafficStats_SupplierCostZone]
   @fromDate datetime,
   @ToDate datetime,
   @SupplierID Varchar(5) = null
AS
BEGIN
SET @FromDate=     CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate= CAST(
(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)	

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	
Declare @Traffic TABLE (ZoneID int primary KEY, Attempts int, SuccessfulAttempts int, DurationsInMinutes numeric(13,5), ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5))
SET NOCOUNT ON 	

IF @SupplierID IS NULL
	INSERT INTO @Traffic(ZoneID,Attempts, SuccessfulAttempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD)
	SELECT
		   ISNULL(TS.SupplierZoneID, ''), 
		   Sum(Attempts) as Attempts, 
		   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		   Avg(PDDinSeconds) as AveragePDD 
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
		LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.SupplierID = CA.CarrierAccountID
		WHERE FirstCDRAttempt BETWEEN @FromDate AND @ToDate		 		
		AND CA.RepresentsASwitch='N'
		GROUP BY TS.SupplierZoneID
ELSE
	INSERT INTO @Traffic(ZoneID,Attempts, SuccessfulAttempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD)
	SELECT
		   ISNULL(TS.SupplierZoneID, ''), 
		   Sum(Attempts) as Attempts, 
		   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		   Avg(PDDinSeconds) as AveragePDD 
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier))
		LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.SupplierID = CA.CarrierAccountID	
		WHERE FirstCDRAttempt BETWEEN @FromDate AND @ToDate AND (TS.SupplierID = @SupplierID)
		AND CA.RepresentsASwitch='N'
		GROUP BY TS.SupplierZoneID


DECLARE @Result TABLE (
		ZoneID INT PRIMARY KEY,Attempts int, SuccessfulAttempts int, AttemptPercentage DECIMAL(13,5), DurationsInMinutes numeric(13,5), ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5),
		NumberOfCalls int,Cost_Nets float, Sale_Nets float,Profit numeric (13,5),Percentage numeric (13,5) 
		)

IF @SupplierID IS NULL
	INSERT INTO @Result(ZoneID ,Attempts, SuccessfulAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Cost_Nets, Sale_Nets, Profit, Percentage)
		SELECT 
			  TS.ZoneID, TS.Attempts, TS.SuccessfulAttempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,	
			  Sum(BS.NumberOfCalls) AS Calls,
			  SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Cost,
			  Sum(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Sale,
			  SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) - SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit,
			  0	      
		FROM @Traffic TS 
			LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date)) ON TS.ZoneID= BS.CostZoneID AND (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) 
			LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
		GROUP BY TS.ZoneID, TS.Attempts, TS.SuccessfulAttempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD
else
	INSERT INTO @Result(ZoneID ,Attempts, SuccessfulAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Cost_Nets, Sale_Nets, Profit, Percentage)
		SELECT 
			  TS.ZoneID, TS.Attempts, TS.SuccessfulAttempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,	
			  Sum(BS.NumberOfCalls) AS Calls,
			  SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Cost,
			  Sum(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Sale,
			  SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) - SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit,
			  0	      
		FROM @Traffic TS 
			LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) ON TS.ZoneID= BS.CostZoneID AND (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) AND (BS.SupplierID=@SupplierID) 
			LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
		GROUP BY TS.ZoneID, TS.Attempts, TS.SuccessfulAttempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD

Declare @TotalProfit numeric(13,5)
SELECT  @TotalProfit = SUM(profit) FROM @Result
UPDATE  @Result SET Percentage = (Profit * 100. / @TotalProfit)
Declare @TotalAttempts bigint
SELECT  @TotalAttempts = SUM(Attempts) FROM @Result
Update  @Result SET AttemptPercentage = (Attempts * 100. / @TotalAttempts)

SELECT * FROM @Result ORDER BY DurationsInMinutes DESC 

END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_CustomerSaleZone]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TrafficStats_CustomerSaleZone]
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = null
AS
BEGIN
SET @FromDate=     CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate= CAST(
(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)	

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	
Declare @Traffic TABLE (ZoneID int primary KEY, Attempts int, SuccessfulAttempts int, DurationsInMinutes numeric(13,5), ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5))
SET NOCOUNT ON 	

IF @CustomerID IS NULL
	INSERT INTO @Traffic(ZoneID,Attempts, SuccessfulAttempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD)
	SELECT
		   ISNULL(TS.OurZoneID, ''), 
		   Sum(Attempts) as Attempts, 
		   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		   Avg(PDDinSeconds) as AveragePDD 
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
		LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID= CA.CarrierAccountID
		WHERE FirstCDRAttempt BETWEEN @FromDate AND @ToDate 		 
		AND ca.RepresentsASwitch = 'N'		
		GROUP BY TS.OurZoneID
ELSE
	INSERT INTO @Traffic(ZoneID,Attempts, SuccessfulAttempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD)
	SELECT
		   ISNULL(TS.OurZoneID, ''), 
		   Sum(Attempts) as Attempts, 
		   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		   Avg(PDDinSeconds) as AveragePDD 
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
		LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID= CA.CarrierAccountID
		WHERE FirstCDRAttempt BETWEEN @FromDate AND @ToDate AND TS.CustomerID = @CustomerID
		AND ca.RepresentsASwitch ='N' 		 		
		GROUP BY TS.OurZoneID


DECLARE @Result TABLE (
		ZoneID INT PRIMARY KEY,Attempts int, SuccessfulAttempts int, AttemptPercentage DECIMAL(13,5), DurationsInMinutes numeric(13,5), ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5),
		NumberOfCalls int,Cost_Nets float, Sale_Nets float,Profit numeric (13,5),Percentage numeric (13,5) 
		)

IF @CustomerID IS NULL
	INSERT INTO @Result(ZoneID ,Attempts, SuccessfulAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Cost_Nets, Sale_Nets, Profit, Percentage)
		SELECT 
			  TS.ZoneID, TS.Attempts, TS.SuccessfulAttempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,	
			  Sum(BS.NumberOfCalls) AS Calls,
			  SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Cost,
			  Sum(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Sale,
			  SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) - SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit,
			  0	      
		FROM @Traffic TS 
			LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date)) ON TS.ZoneID= BS.SaleZoneID AND (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate)  
			LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
		GROUP BY TS.ZoneID, TS.Attempts, TS.SuccessfulAttempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD
else
	INSERT INTO @Result(ZoneID ,Attempts, SuccessfulAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Cost_Nets, Sale_Nets, Profit, Percentage)
		SELECT 
			  TS.ZoneID, TS.Attempts, TS.SuccessfulAttempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,	
			  Sum(BS.NumberOfCalls) AS Calls,
			  SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Cost,
			  Sum(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Sale,
			  SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) - SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit,
			  0	      
		FROM @Traffic TS 
			LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) ON TS.ZoneID= BS.SaleZoneID AND (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) AND (BS.CustomerID=@CustomerID) 
			LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
		GROUP BY TS.ZoneID, TS.Attempts, TS.SuccessfulAttempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD

Declare @TotalProfit numeric(13,5)
SELECT  @TotalProfit = SUM(profit) FROM @Result
UPDATE  @Result SET Percentage = (Profit * 100. / @TotalProfit)
Declare @TotalAttempts bigint
SELECT  @TotalAttempts = SUM(Attempts) FROM @Result
Update  @Result SET AttemptPercentage = (Attempts * 100. / @TotalAttempts)

SELECT * FROM @Result ORDER BY DurationsInMinutes DESC 

END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_CustomerSummary]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TrafficStats_CustomerSummary]
   @fromDate datetime ,
   @ToDate datetime,
   @CustomerID varchar(15)=null,
   @TopRecord INT =NULL
AS
BEGIN
	SET @FromDate=     CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)
 
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

Declare @Traffic TABLE (CustomerID VARCHAR(15) primary KEY, Attempts int, DurationsInMinutes numeric(13,5), ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5))

SET NOCOUNT ON 	
if @CustomerID is Null
	INSERT INTO @Traffic(CustomerID, Attempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD)

	 SELECT 
		   ISNULL(TS.CustomerID, ''), 
		   Sum(Attempts) as Attempts, 
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		   Avg(PDDinSeconds) as AveragePDD 
	     
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
	    LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
        WHERE ( FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <=  @ToDate )
        AND CA.RepresentsASwitch='N'
		GROUP BY ISNULL(TS.CustomerID, '')  

else
	INSERT INTO @Traffic(CustomerID, Attempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD)

	 SELECT 
		   ISNULL(TS.CustomerID, ''), 
		   Sum(Attempts) as Attempts, 
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		   Avg(PDDinSeconds) as AveragePDD 
	     
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
		LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID 
	    WHERE( FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <=  @ToDate ) AND (TS.CustomerID=@CustomerID)
		AND CA.RepresentsASwitch='N' 
		GROUP BY ISNULL(TS.CustomerID, '')  

DECLARE @Result TABLE (CustomerID VARCHAR(15)PRIMARY KEY, Attempts int,DurationsInMinutes numeric(13,5),ASR numeric(13,5), ACD numeric(13,5),DeliveredASR numeric(13,5), AveragePDD numeric(13,5), NumberOfCalls int, Cost_Nets float, Sale_Nets float, Profit numeric(13,5), Percentage Float)

if @CustomerID is Null
	INSERT INTO @Result(CustomerID, Attempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Sale_Nets, Cost_Nets, Profit, Percentage)
		SELECT 
			  T.CustomerID, T.Attempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD,
			  ISNULL(SUM(BS.NumberOfCalls),0) AS Calls,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0) AS Sale,
			  ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Cost,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0)-ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Profit,
			  0
		FROM 
			@Traffic T 
				LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date)) ON T.CustomerID = BS.CustomerID AND (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) 
				LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
                LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate 
		GROUP BY T.CustomerID, T.Attempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD 
else
	INSERT INTO @Result(CustomerID, Attempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Sale_Nets, Cost_Nets, Profit, Percentage)
		SELECT 
			  T.CustomerID, T.Attempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD,
			  ISNULL(SUM(BS.NumberOfCalls),0) AS Calls,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0) AS Sale,
			  ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Cost,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0)-ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Profit,
			  0
		FROM 
			@Traffic T 
				LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) ON T.CustomerID = BS.CustomerID AND (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) AND (BS.CustomerID=@CustomerID) 
			    LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
                LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
		GROUP BY T.CustomerID, T.Attempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD 

Declare @TotalProfit numeric(13,5)
SELECT  @TotalProfit = SUM(profit) FROM @Result
UPDATE  @Result SET Percentage = CASE WHEN @TotalProfit <> 0 THEN (Profit * 100.0 / @TotalProfit) ELSE 0 END

SET ROWCOUNT @TopRecord

SELECT * FROM @Result ORDER  BY DurationsInMinutes DESC 

END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_ByCustomer]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TrafficStats_ByCustomer]
   @fromDate datetime,
   @ToDate datetime,
   @SupplierID Varchar(5) = null
AS
BEGIN
SET @FromDate=     CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate= CAST(
(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)	

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)


Declare @Traffic TABLE (CustomerID VarChar(15)  primary KEY, Attempts int, DurationsInMinutes numeric(13,5), SuccessfulAttempts numeric (13,5),ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5))
SET NOCOUNT ON 	
INSERT INTO @Traffic(CustomerID, Attempts, SuccessfulAttempts, DurationsInMinutes, ASR,ACD,DeliveredASR,AveragePDD)
SELECT 
	   ISNULL(TS.CustomerID, ''),
	   Sum(Attempts) as Attempts, 
	   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
	   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
	   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
	   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
	   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	   Avg(PDDinSeconds) as AveragePDD 
      
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier))
    WHERE (FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <= @ToDate)
			AND (TS.SupplierID = @SupplierID) 		 		
	GROUP BY TS.CustomerID
	
DECLARE @Result TABLE (
		CustomerID VarChar(15)primary KEY,Attempts int, AttemptPercentage DECIMAL(13,5), DurationsInMinutes numeric(13,5), SuccessfulAttempts numeric (13,5),ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5),
		NumberOfCalls int,Cost_Nets float, Sale_Nets float,Profit numeric (13,5),AverageSaleRate DECIMAL(13,5),AverageCostRate DECIMAL(13,5), Percentage numeric (13,5) 
		)

INSERT INTO @Result(CustomerID ,Attempts, DurationsInMinutes,SuccessfulAttempts, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Cost_Nets, Sale_Nets, Profit,AverageSaleRate,AverageCostRate, Percentage)
	SELECT 
		  TS.CustomerID, TS.Attempts, TS.DurationsInMinutes,TS.SuccessfulAttempts,TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,
	      Sum(BS.NumberOfCalls) AS Calls,
	      SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Cost,
	      Sum(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Sale,
	      SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) - SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit,
	      AVG(BS.Sale_Rate),
	      AVG(BS.Cost_Rate),
	      0	      
    FROM @Traffic TS 
		LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) ON TS.CustomerID= BS.CustomerID AND (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) AND (BS.SupplierID=@SupplierID)
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	GROUP BY TS.CustomerID, TS.Attempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,TS.SuccessfulAttempts
	 
Declare @TotalProfit numeric(13,5)
SELECT  @TotalProfit = SUM(profit) FROM @Result
UPDATE  @Result SET Percentage = (Profit * 100. / @TotalProfit)
Declare @TotalAttempts bigint
SELECT  @TotalAttempts = SUM(Attempts) FROM @Result
Update  @Result SET AttemptPercentage = (Attempts * 100. / @TotalAttempts)

SELECT * FROM @Result ORDER BY DurationsInMinutes DESC 

END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_ZoneSuppliers]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TrafficStats_ZoneSuppliers]
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = NULL,
   @ZoneID INT 
AS
BEGIN
	SET @FromDate=     CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate= CAST(
(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)	

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	

Declare @Traffic TABLE (SupplierID VarChar(15)  primary KEY, Attempts int, DurationsInMinutes numeric(13,5), SuccessfulAttempts numeric (13,5),ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5))
SET NOCOUNT ON 	
INSERT INTO @Traffic(SupplierID, Attempts, SuccessfulAttempts, DurationsInMinutes, ASR,ACD,DeliveredASR,AveragePDD)
SELECT 
	   ISNULL(TS.SupplierID, ''),
	   Sum(Attempts) as Attempts, 
	   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
	   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
	   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
	   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
	   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	   Avg(PDDinSeconds) as AveragePDD 
      
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
    WHERE (FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <= @ToDate)
			AND (TS.CustomerID = @CustomerID)
			AND TS.OurZoneID = @ZoneID 		 		
	GROUP BY TS.SupplierID
	
DECLARE @Result TABLE (
		SupplierID VarChar(15)primary KEY,Attempts int, AttemptPercentage DECIMAL(13,5), DurationsInMinutes numeric(13,5), SuccessfulAttempts numeric (13,5),ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5),
		NumberOfCalls int,Cost_Nets float, Sale_Nets float,Profit numeric (13,5),Percentage numeric (13,5) 
		)

INSERT INTO @Result(SupplierID ,Attempts, DurationsInMinutes,SuccessfulAttempts, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Cost_Nets, Sale_Nets, Profit, Percentage)
	SELECT 
		  TS.SupplierID, TS.Attempts, TS.DurationsInMinutes,TS.SuccessfulAttempts,TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,
	      Sum(BS.NumberOfCalls) AS Calls,
	      SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Cost,
	      Sum(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Sale,
	      SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) - SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit,
	      0	      
    FROM @Traffic TS 
		LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) ON TS.SupplierID= BS.SupplierID 
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	Where (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) AND (BS.CustomerID=@CustomerID) AND BS.SaleZoneID = @ZoneID
	GROUP BY TS.SupplierID, TS.Attempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,TS.SuccessfulAttempts
	 
Declare @TotalProfit numeric(13,5)
SELECT  @TotalProfit = SUM(profit) FROM @Result
UPDATE  @Result SET Percentage = (Profit * 100. / @TotalProfit)
Declare @TotalAttempts bigint
SELECT  @TotalAttempts = SUM(Attempts) FROM @Result
Update  @Result SET AttemptPercentage = (Attempts * 100. / @TotalAttempts)

SELECT * FROM @Result ORDER BY DurationsInMinutes DESC 

END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_SupplierSummary]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TrafficStats_SupplierSummary]
   @fromDate datetime ,
   @ToDate datetime,
   @SupplierID varchar(15)=null,
   @TopRecord INT =NULL
AS
BEGIN
	SET @FromDate=     CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)
 
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

Declare @Traffic TABLE (SupplierID VARCHAR(15) primary KEY, Attempts int, DurationsInMinutes numeric(13,5), ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5))

SET NOCOUNT ON 	
if @SupplierID is Null
	INSERT INTO @Traffic(SupplierID, Attempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD)

	 SELECT 
		   ISNULL(TS.SupplierID, ''), 
		   Sum(Attempts) as Attempts, 
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		   Avg(PDDinSeconds) as AveragePDD 
	     
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
	    LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.SupplierID = CA.CarrierAccountID
        WHERE ( FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <=  @ToDate )
        AND CA.RepresentsASwitch='N'
		GROUP BY ISNULL(TS.SupplierID, '')  

else
	INSERT INTO @Traffic(SupplierID, Attempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD)

	 SELECT 
		   ISNULL(TS.SupplierID, ''), 
		   Sum(Attempts) as Attempts, 
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		   Avg(PDDinSeconds) as AveragePDD 
	     
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier))
		LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.SupplierID = CA.CarrierAccountID
		 WHERE( FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <=  @ToDate ) AND (TS.SupplierID=@SupplierID)
		 AND CA.RepresentsASwitch='N'
		GROUP BY ISNULL(TS.SupplierID, '')  

DECLARE @Result TABLE (SupplierID VARCHAR(15)PRIMARY KEY, Attempts int,DurationsInMinutes numeric(13,5),ASR numeric(13,5), ACD numeric(13,5),DeliveredASR numeric(13,5), AveragePDD numeric(13,5), NumberOfCalls int, Cost_Nets float, Sale_Nets float, Profit numeric(13,5), Percentage Float)

if @SupplierID is Null
	INSERT INTO @Result(SupplierID, Attempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Sale_Nets, Cost_Nets, Profit, Percentage)
		SELECT 
			  T.SupplierID, T.Attempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD,
			  ISNULL(SUM(BS.NumberOfCalls),0) AS Calls,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0) AS Sale,
			  ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Cost,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0)-ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Profit,
			  0
		FROM 
			@Traffic T 
				LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date)) ON T.SupplierID = BS.SupplierID AND  (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate)
				LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
                LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate 
		GROUP BY T.SupplierID, T.Attempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD 
else
	INSERT INTO @Result(SupplierID, Attempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Sale_Nets, Cost_Nets, Profit, Percentage)
		SELECT 
			  T.SupplierID, T.Attempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD,
			  ISNULL(SUM(BS.NumberOfCalls),0) AS Calls,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0) AS Sale,
			  ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Cost,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0)-ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Profit,
			  0
		FROM 
			@Traffic T 
				LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier)) ON T.SupplierID = BS.SupplierID AND (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) AND (BS.SupplierID=@SupplierID)
			    LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
                LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
		GROUP BY T.SupplierID, T.Attempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD 

Declare @TotalProfit numeric(13,5)
SELECT  @TotalProfit = SUM(profit) FROM @Result
UPDATE  @Result SET Percentage = CASE WHEN @TotalProfit <> 0 THEN (Profit * 100.0 / @TotalProfit) ELSE 0 END

SET ROWCOUNT @TopRecord

SELECT * FROM @Result ORDER  BY DurationsInMinutes DESC 

END
GO
/****** Object:  StoredProcedure [dbo].[EA_TrafficStats_TopNDestination]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EA_TrafficStats_TopNDestination]
 @TopRecords   INT,
 @FromDate     DateTime,
 @ToDate       DateTime,
 @CustomerID varchar (25)= NULL,
 @SupplierID varchar (25)= NULL,
 @HighestTraffic CHAR(1) = 'Y'	
AS
BEGIN
	SET @FromDate=     CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)
DECLARE @ExchangeRates TABLE(
            Currency VARCHAR(3),
            Date SMALLDATETIME,
            Rate FLOAT
            PRIMARY KEY(Currency, Date)
        )
	
INSERT INTO @ExchangeRates
SELECT *
FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate);

Declare @Zones TABLE ( OurZoneID INT , Attempts int, DurationsInSeconds numeric(13,5))

SET ROWCOUNT @TopRecords

IF(@HighestTraffic = 'Y')
INSERT INTO @Zones
    SELECT TS.OurZoneID AS OurZoneID,
           SUM(Attempts) AS Attempts,
           SUM(TS.DurationsInSeconds) AS DurationsInSeconds
    FROM   TrafficStats ts WITH( NOLOCK, INDEX(IX_TrafficStats_DateTimeFirst), INDEX(IX_TrafficStats_Customer))
    WHERE  FirstCDRAttempt BETWEEN @FromDate AND @ToDate
           AND (@CustomerID IS NULL OR TS.CustomerID = @CustomerID)
           AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID)
           AND ts.OurZoneID IS NOT NULL 
    GROUP BY
           TS.OurZoneID
    ORDER BY
           DurationsInSeconds DESC,
           Attempts DESC
ELSE
INSERT INTO @Zones
    SELECT TS.OurZoneID AS OurZoneID,
           SUM(Attempts) AS Attempts,
           SUM(TS.DurationsInSeconds) AS DurationsInSeconds
    FROM   TrafficStats ts WITH( NOLOCK, INDEX(IX_TrafficStats_DateTimeFirst), INDEX(IX_TrafficStats_Customer))
    WHERE  FirstCDRAttempt BETWEEN @FromDate AND @ToDate
           AND (@CustomerID IS NULL OR TS.CustomerID = @CustomerID)
           AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID)
           AND ts.OurZoneID IS NOT NULL 
    GROUP BY
           TS.OurZoneID
    ORDER BY
           DurationsInSeconds ASC,
           Attempts DESC
	
SET ROWCOUNT 0;

With TrafficTable AS 
(
    SELECT dbo.DateOf(TS.FirstCDRAttempt) AS Date,
           TS.OurZoneID,
           SUM(TS.Attempts) AS Attempts,
           SUM(TS.DurationsInSeconds / 60.) AS DurationsInMinutes,
           SUM(TS.SuccessfulAttempts) * 100.0 / SUM(TS.Attempts) AS ASR,
           CASE 
                WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(TS.DurationsInSeconds) 
                     / (60.0 * SUM(TS.SuccessfulAttempts))
                ELSE 0
           END AS ACD,
           SUM(TS.deliveredAttempts) AS deliveredAttempts,
           SUM(Ts.deliveredAttempts) * 100.0 / SUM(Ts.Attempts) AS DeliveredASR,
           AVG(Ts.PDDinSeconds) AS AveragePDD,
           MAX(Ts.MaxDurationInSeconds) / 60.0 AS MaxDuration,
           MAX(Ts.LastCDRAttempt) AS LastAttempt,
           SUM(Ts.SuccessfulAttempts) AS SuccessfulAttempts,
           SUM(Ts.Attempts - Ts.SuccessfulAttempts) AS FailedAttempts
    FROM   TrafficStats ts WITH(
               NOLOCK,
               INDEX(IX_TrafficStats_DateTimeFirst),
               INDEX(IX_TrafficStats_Customer)
           )
           JOIN @Zones z
                ON  TS.OurZoneID = Z.OurZoneID
    WHERE  Ts.FirstCDRAttempt BETWEEN @FromDate AND @ToDate
           AND (@CustomerID IS NULL OR TS.CustomerID = @CustomerID)
           AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID)
    GROUP BY
           dbo.DateOf(TS.FirstCDRAttempt),
           TS.OurZoneID
),BillingTable AS 
(
    SELECT bs.CallDate AS Date,
           bs.SaleZoneID AS OurZoneID,
           AVG(bs.Sale_Rate / ISNULL(ERS.Rate,1)) AS Rate,
           SUM(bs.Sale_Nets / ISNULL(ERS.Rate,1)) AS Amount
    FROM   Billing_Stats bs WITH(NOLOCK, INDEX(IX_Billing_Stats_Date))
           LEFT JOIN @ExchangeRates ERS
                ON  ERS.Currency = bs.Sale_Currency
                AND ERS.Date = bs.CallDate
    WHERE  bs.CallDate BETWEEN @FromDate AND @ToDate
           AND (@CustomerID IS NULL OR bs.CustomerID = @CustomerID)
           AND (@SupplierID IS NULL OR bs.SupplierID = @SupplierID)
           AND EXISTS (SELECT * FROM @Zones Z WHERE Z.OurZoneId = bs.SaleZoneID)
    GROUP BY
           bs.CallDate,
           bs.SaleZoneID
)

SELECT Min(T.Date) AS MinDate,
       Max(T.Date) AS MaxDate,
       T.OurZoneID AS OurZoneID,
       SUM(T.Attempts) AS Attempts,
       Sum(SuccessfulAttempts) AS SuccesfulAttempts,
       SUM(DurationsInMinutes) AS DurationsInMinutes,
       B.Rate AS Rate,
       SUM(B.Amount) AS Amount,
       AVG(T.ASR) AS ASR,
       AVG(T.ACD) AS ACD 
FROM   TrafficTable T
       LEFT JOIN BillingTable B ON  T.OurZoneID = B.OurZoneID AND T.Date = B.Date
GROUP BY T.OurZoneID ,B.Rate
ORDER BY Attempts DESC 

END
GO
/****** Object:  StoredProcedure [dbo].[EA_BillingSummary]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EA_BillingSummary]
(
	@FromDate Datetime,
	@ToDate Datetime,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@SaleZoneID INT = NULL 
)
WITH Recompile
AS
SET @FromDate=     CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate= CAST(
(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)	

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

	SELECT 
		bs.CallDate AS CallDate,
		bs.SaleZoneID AS SaleZone, 
		bs.CostZoneID AS CostZone, 
		bs.CustomerID AS CustomerID,
		bs.SupplierID  AS SupplierID,
		bs.Sale_Rate  / ISNULL(ERS.Rate, 1) AS SaleRate,
		bs.Cost_Rate / ISNULL(ERC.Rate, 1) AS CostRate,
		SUM(bs.SaleDuration/60.0) AS SaleDuration,
		SUM(bs.CostDuration/60.0) AS CostDuration,
		SUM(bs.sale_nets / ISNULL(ERS.Rate, 1)) AS SaleNet,
		SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)) AS  CostNet         			
    FROM
	    Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer),INDEX(IX_Billing_Stats_Supplier))
	    LEFT JOIN Zone SZ WITH(NOLOCK) ON SZ.ZoneID = bs.SaleZoneID
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE        
			bs.calldate>=@FromDate AND bs.calldate<@ToDate
	 	AND (@CustomerID IS NULL OR bs.CustomerID=@CustomerID) 
        AND (@SupplierID IS NULL OR bs.SupplierID=@SupplierID)
        AND (@SaleZoneID IS Null OR SZ.ZoneID = @SaleZoneID)
        AND bs.Sale_Rate <> 0
	 	AND bs.Cost_Rate <> 0
	GROUP BY 
	     bs.CallDate
	    ,bs.CustomerID
	    ,bs.SupplierID
	    ,bs.SaleZoneID
	    ,bs.CostZoneID
	  	,bs.Sale_Rate  / ISNULL(ERS.Rate, 1) 
		,bs.Cost_Rate / ISNULL(ERC.Rate, 1)
	ORDER BY 
	     bs.CallDate
	    ,bs.CustomerID
	    ,bs.SupplierID
	    ,bs.SaleZoneID
	    ,bs.CostZoneID
		,bs.Sale_Rate  / ISNULL(ERS.Rate, 1) 
		,bs.Cost_Rate / ISNULL(ERC.Rate, 1)
	

RETURN
GO
/****** Object:  StoredProcedure [dbo].[rpt_Volumes_CompareInOutTrafficBarChart]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[rpt_Volumes_CompareInOutTrafficBarChart](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5),
	@Period varchar(7)
)
with Recompile
AS 
	SET @FromDate=     CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	
	
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

   INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

   Declare @Results Table(CallDate datetime,CallWeek int,CallMonth int,CallYear int,DurationIn numeric(13,5), DurationOut numeric(13,5), NetIn numeric(13,5), NetOut numeric(13,5))

	IF(@Period = 'None')
		BEGIN
			INSERT INTO @Results(
				DurationIn,
				DurationOut,
				NetIn,
				NetOut
			)
			SELECT
				  SUM(CASE WHEN BS.CustomerID=@CustomerID THEN (BS.SaleDuration / 60.0) ELSE 0 END) AS DurationIn,
				  SUM(CASE WHEN BS.SupplierID=@CustomerID THEN (BS.CostDuration / 60.0) ELSE 0 END) AS DurationOut,
				  
				  SUM(CASE WHEN BS.CustomerID=@CustomerID THEN bs.Sale_Nets / ISNULL(ERS.Rate, 1) ELSE 0 END) AS NetIn,
				  SUM(CASE WHEN BS.SupplierID=@CustomerID THEN bs.Cost_Nets / ISNULL(ERC.Rate, 1) ELSE 0 END) AS NetOut
			FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
			LEFT JOIN @ExchangeRates ERC ON ERC.Currency = BS.Cost_Currency AND ERC.Date = BS.CallDate
			WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
		END
		ELSE
			IF(@Period = 'Daily')
			BEGIN
				INSERT INTO @Results(
					CallDate,
					DurationIn,
					DurationOut,
					NetIn,
					NetOut
				)
				SELECT
				  bs.CallDate AS CallDate,
				  SUM(CASE WHEN BS.CustomerID=@CustomerID THEN (BS.SaleDuration /60.0) ELSE 0 END) AS DurationIn,
				  SUM(CASE WHEN BS.SupplierID=@CustomerID THEN (BS.CostDuration / 60.0) ELSE 0 END) AS DurationOut,
				  
				  SUM(CASE WHEN BS.CustomerID=@CustomerID THEN bs.Sale_Nets / ISNULL(ERS.Rate, 1) ELSE 0 END) AS NetIn,
				  SUM(CASE WHEN BS.SupplierID=@CustomerID THEN bs.Cost_Nets / ISNULL(ERC.Rate, 1) ELSE 0 END) AS NetOut
				FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
				LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
				LEFT JOIN @ExchangeRates ERC ON ERC.Currency = BS.Cost_Currency AND ERC.Date = BS.CallDate
				WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
				GROUP BY bs.CallDate 
				ORDER BY bs.CallDate
			END
			ELSE
				IF(@Period = 'Weekly')
				BEGIN
						INSERT INTO @Results(
						CallWeek,
						CallYear,
						DurationIn,
						DurationOut,
						NetIn,
						NetOut
					)
					SELECT
						  datepart(week,BS.CallDate) AS CallWeek,
						  datepart(year,bs.CallDate) AS CallYear,
						  SUM(CASE WHEN BS.CustomerID=@CustomerID THEN (BS.SaleDuration /60.0) ELSE 0 END) AS DurationIn,
						  SUM(CASE WHEN BS.SupplierID=@CustomerID THEN (BS.CostDuration / 60.0) ELSE 0 END) AS DurationOut,
						  
						  SUM(CASE WHEN BS.CustomerID=@CustomerID THEN bs.Sale_Nets / ISNULL(ERS.Rate, 1) ELSE 0 END) AS NetIn,
						  SUM(CASE WHEN BS.SupplierID=@CustomerID THEN bs.Cost_Nets / ISNULL(ERC.Rate, 1) ELSE 0 END) AS NetOut
					FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
					LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
					LEFT JOIN @ExchangeRates ERC ON ERC.Currency = BS.Cost_Currency AND ERC.Date = BS.CallDate
					WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
					GROUP BY DATEPART(week,BS.calldate),DATEPART(year,BS.calldate)
					ORDER BY DATEPART(year,BS.calldate) ,DATEPART(week,BS.calldate) ASC
				END
				ELSE 
					IF(@Period = 'Monthly')
						BEGIN
								INSERT INTO @Results(
								CallMonth,
								CallYear,
								DurationIn,
								DurationOut,
								NetIn,
								NetOut
							)
							SELECT
							      datepart(month,BS.CallDate) AS CallMonth,
								  datepart(year,bs.CallDate) AS CallYear,
								  SUM(CASE WHEN BS.CustomerID=@CustomerID THEN (BS.SaleDuration / 60.0) ELSE 0 END) AS DurationIn,
								  SUM(CASE WHEN BS.SupplierID=@CustomerID THEN (BS.CostDuration / 60.0) ELSE 0 END) AS DurationOut,
								  
								  SUM(CASE WHEN BS.CustomerID=@CustomerID THEN bs.Sale_Nets / ISNULL(ERS.Rate, 1) ELSE 0 END) AS NetIn,
								  SUM(CASE WHEN BS.SupplierID=@CustomerID THEN bs.Cost_Nets / ISNULL(ERC.Rate, 1) ELSE 0 END) AS NetOut
							FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
							LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
							LEFT JOIN @ExchangeRates ERC ON ERC.Currency = BS.Cost_Currency AND ERC.Date = BS.CallDate
							WHERE BS.CallDate>=@FromDate  AND BS.CallDate<= @ToDate
							GROUP BY DATEPART(month,BS.calldate),DATEPART(year,BS.calldate)
							ORDER BY DATEPART(year,BS.calldate),DATEPART(month,BS.calldate) ASC 
						END

SELECT * FROM @Results

RETURN
GO
/****** Object:  StoredProcedure [dbo].[rpt_RateLoss]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[rpt_RateLoss](
	@FromDate Datetime,
	@ToDate Datetime,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@ZoneID int = NULL,
	@Margin FLOAT = 0 
)with Recompile
	AS 
		SET @FromDate = CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate = CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	
	
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)
	
	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

    SELECT 
		CZ.Name AS CostZone, 
		SZ.Name AS SaleZone, 
		bs.SupplierID  AS SupplierID, 
		bs.CustomerID AS CustomerID,
		AVG(bs.Sale_Rate / ISNULL(ERS.Rate, 1)) AS SaleRate,
		AVG(bs.Cost_Rate / ISNULL(ERC.Rate, 1)) AS CostRate,
		SUM(bs.CostDuration/60.0) AS CostDuration,
		SUM(bs.SaleDuration/60.0) AS SaleDuration, 
		SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)) AS  CostNet, 
		SUM(bs.sale_nets / ISNULL(ERS.Rate, 1)) AS SaleNet         			
	 FROM
	     Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer),INDEX(IX_Billing_Stats_Supplier))
	    LEFT JOIN Zone SZ WITH(NOLOCK) ON SZ.ZoneID = bs.SaleZoneID
		LEFT JOIN Zone CZ WITH(NOLOCK) ON CZ.ZoneID = bs.CostZoneID
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE bs.calldate>=@FromDate AND bs.calldate<@ToDate
	 	  AND (@CustomerID IS NULL OR bs.CustomerID=@CustomerID) 
          AND (@SupplierID IS NULL OR bs.SupplierID=@SupplierID)
          AND (@ZoneID IS Null OR SZ.ZoneID = @ZoneID)
  
	GROUP BY 
	    CZ.Name,SZ.Name,bs.SupplierID,bs.CustomerID
	HAVING 	
        (SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)) - SUM(bs.sale_nets / ISNULL(ERS.Rate, 1))) > @Margin
  
	  
	
RETURN
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_SupplierZones]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TrafficStats_SupplierZones]
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = NULL,
   @SupplierID Varchar(5) 
AS
BEGIN
	SET @FromDate=     CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate= CAST(
(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)	

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	

Declare @Traffic TABLE (ZoneID int  primary KEY, Attempts int, DurationsInMinutes numeric(13,5), SuccessfulAttempts numeric (13,5),ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5))
SET NOCOUNT ON 	
INSERT INTO @Traffic(ZoneID, Attempts, SuccessfulAttempts, DurationsInMinutes, ASR,ACD,DeliveredASR,AveragePDD)
SELECT 
	   ISNULL(TS.SupplierZoneID, ''),
	   Sum(Attempts) as Attempts, 
	   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
	   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
	   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
	   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
	   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	   Avg(PDDinSeconds) as AveragePDD 
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
    WHERE (FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <= @ToDate)
			AND (TS.CustomerID = @CustomerID)
			AND TS.SupplierID = @SupplierID
	GROUP BY TS.SupplierZoneID
	
DECLARE @Result TABLE (
		ZoneID int primary KEY,Attempts int, AttemptPercentage DECIMAL(13,5), DurationsInMinutes numeric(13,5), SuccessfulAttempts numeric (13,5),ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5),
		NumberOfCalls int,Cost_Nets float, Sale_Nets float,Profit numeric (13,5),Percentage numeric (13,5) 
		)

INSERT INTO @Result(ZoneID ,Attempts, DurationsInMinutes,SuccessfulAttempts, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Cost_Nets, Sale_Nets, Profit, Percentage)
	SELECT 
		  TS.ZoneID , TS.Attempts, TS.DurationsInMinutes,TS.SuccessfulAttempts,TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,
	      Sum(BS.NumberOfCalls) AS Calls,
	      SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Cost,
	      Sum(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Sale,
	      SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) - SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit,
	      0	      
    FROM @Traffic TS 
		LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) ON TS.ZoneID= BS.CostZoneID
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	Where (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) AND (BS.CustomerID=@CustomerID) AND BS.SupplierID = @SupplierID
	GROUP BY TS.ZoneID, TS.Attempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,TS.SuccessfulAttempts
	 
Declare @TotalProfit numeric(13,5)
SELECT  @TotalProfit = SUM(profit) FROM @Result
UPDATE  @Result SET Percentage = (Profit * 100. / @TotalProfit)
Declare @TotalAttempts bigint
SELECT  @TotalAttempts = SUM(Attempts) FROM @Result
Update  @Result SET AttemptPercentage = (Attempts * 100. / @TotalAttempts)

SELECT * FROM @Result ORDER BY DurationsInMinutes DESC 

END
GO
/****** Object:  StoredProcedure [dbo].[rpt_ZoneSummary]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rpt_ZoneSummary](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@Cost char(1)='Y',
	@CurrencyID varchar(3)
)

	AS 

	SET @FromDate=     CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	

DECLARE @ExchangeRates TABLE(
		CurrencyIn VARCHAR(3) COLLATE SQL_Latin1_General_CP1256_CI_AS,
		CurrencyOut VARCHAR(3) COLLATE SQL_Latin1_General_CP1256_CI_AS,
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(CurrencyIn,CurrencyOut, Date))

INSERT INTO @ExchangeRates 
SELECT gder1.Currency AS CurrencyIn,
		   gder2.Currency AS CurrencyOut,
		   gder1.Date AS Date, 
		   gder1.Rate / gder2.Rate AS Rate
FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder1
JOIN dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder2 ON  gder1.Date = gder2.Date


	IF (@Cost = 'Y')
	BEGIN 
		DECLARE @NumberOfDays INT 
		SET @NumberOfDays = DATEDIFF(dd,@FromDate,@ToDate) + 1
		IF (@CustomerID IS NULL AND @SupplierID IS NULL)
		BEGIN 
			SELECT
				Z.Name AS Zone,
				SUM(bs.NumberOfCalls) AS Calls,
				SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
				SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END) AS DurationInSeconds,
				--AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS Rate,
				AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END) AS Rate,
				--(SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS Net,
				SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  Net,
				SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
				--AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS OffPeakRate,
				AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END) AS OffPeakRate,
				--(SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
				SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
				SUM(bs.Cost_Discounts/ISNULL(ERC.Rate, 1)) AS Discount,
				SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
				SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
			FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
			LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.CostZoneID   
			LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
			WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
			GROUP BY Z.Name
			ORDER BY Z.Name ASC 
		END
		ELSE
			IF (@CustomerID IS NULL AND @SupplierID IS NOT NULL)
			BEGIN 
				SELECT
					Z.Name AS Zone,
					SUM(bs.NumberOfCalls) AS Calls,
					SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
					SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END) AS DurationInSeconds,
					--AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS Rate,
					AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END) AS Rate,
					--(SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS Net,
					SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  Net,
					SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
					--AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS OffPeakRate,
					AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END) AS OffPeakRate,
					--(SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
					SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
					SUM(bs.Cost_Discounts/ISNULL(ERC.Rate, 1)) AS Discount,
					SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
					SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
				FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
				LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.CostZoneID   
				LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
				WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
					 AND bs.SupplierID=@SupplierID
				GROUP BY Z.Name
				ORDER BY Z.Name ASC 
			END
			ELSE 
				IF(@CustomerID IS NOT NULL AND @SupplierID IS NULL)
				BEGIN 
					SELECT
						Z.Name AS Zone,
						SUM(bs.NumberOfCalls) AS Calls,
						SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
						SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END) AS DurationInSeconds,
						--AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS Rate,
						AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END) AS Rate,
						--(SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS Net,
						SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  Net,
						SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
						--AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS OffPeakRate,
						AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END) AS OffPeakRate,
						--(SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
						SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
						SUM(bs.Cost_Discounts/ISNULL(ERC.Rate, 1)) AS Discount,
						SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
						SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
					FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
					LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.CostZoneID   
					JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = BS.CallDate
					WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
						 AND bs.CustomerID=@CustomerID
					GROUP BY Z.Name
					ORDER BY Z.Name ASC 
				END
				ELSE
					IF(@CustomerID IS NOT NULL AND @SupplierID IS NOT NULL)
					BEGIN 
						SELECT
							Z.Name AS Zone,
							SUM(bs.NumberOfCalls) AS Calls,
							SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
							SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END) AS DurationInSeconds,
							--AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS Rate,
							AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END) AS Rate,
							--(SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS Net,
							SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  Net,
							SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
							--AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS OffPeakRate,
							AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END) AS OffPeakRate,
							--(SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
							SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
							SUM(bs.Cost_Discounts/ISNULL(ERC.Rate, 1)) AS Discount,
							SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
							SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue
						FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
						LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.CostZoneID   
						LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate
						WHERE bs.calldate >=@FromDate AND bs.calldate<=@ToDate
							 AND bs.CustomerID=@CustomerID 
							 AND bs.SupplierID=@SupplierID
						GROUP BY Z.Name
						ORDER BY Z.Name ASC 
					END
--					SELECT isnull(SUM(ca.Services + ca.ConnectionFees) * @NumberOfDays,0) AS Services
--					FROM CarrierAccount ca 
--					WHERE ca.CarrierAccountID IN 
--     					(
--						SELECT DISTINCT bs.SupplierID	
--						FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
--						WHERE  bs.CallDate >= @FromDate	AND  bs.CallDate <= @ToDate
--						)
				SELECT ((SUM((isnull(ca.Services,0) +isnull(ca.ConnectionFees,0))/ISNULL(ER.Rate, 1)))*@NumberOfDays) AS Services
				FROM CarrierAccount ca 
				JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
				LEFT JOIN @ExchangeRates ER ON ER.CurrencyIn = cp.CurrencyID AND ER.CurrencyOut = @CurrencyID AND ER.Date = @FromDate
	END 
	ELSE 
	BEGIN 
		IF(@CustomerID IS NULL AND @SupplierID IS NULL)
			BEGIN 
				SELECT 
					Z.Name AS Zone,
					SUM(bs.NumberOfCalls) AS Calls,
					SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
					SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS DurationInSeconds,
					--AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE NULL END) AS Rate,
					AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END) AS Rate,
					--(SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS Net,
					SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  Net,
					SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
					--AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE NULL END) AS OffPeakRate,
					AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS OffPeakRate,
					--(SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
					SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
					SUM(bs.sale_Discounts/ISNULL(ERS.Rate, 1)) AS Discount,
					SUM(bs.sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
					SUM(bs.sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
				FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
				LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
				LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
				WHERE bs.calldate>=@FromDate AND bs.calldate<@ToDate
				GROUP BY Z.Name
				ORDER BY Z.Name ASC
			END
			ELSE
				IF(@CustomerID IS NULL AND @SupplierID IS NOT NULL)
				BEGIN 
					SELECT 
						Z.Name AS Zone,
						SUM(bs.NumberOfCalls) AS Calls,
						SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
						SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS DurationInSeconds,
						--AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE NULL END) AS Rate,
						AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END) AS Rate,
						--(SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS Net,
						SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  Net,
						SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
						--AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE NULL END) AS OffPeakRate,
						AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS OffPeakRate,
						--(SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
						SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
						SUM(bs.sale_Discounts/ISNULL(ERS.Rate, 1)) AS Discount,
						SUM(bs.sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
						SUM(bs.sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
					FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
					LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
					LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
					WHERE bs.calldate>=@FromDate AND bs.calldate<@ToDate
						 AND bs.SupplierID=@SupplierID
					GROUP BY Z.Name
					ORDER BY Z.Name ASC
				END
				ELSE 
					IF(@CustomerID IS NOT NULL AND @SupplierID IS NULL)
					BEGIN 
				SELECT 
								Z.Name AS Zone,
								SUM(bs.NumberOfCalls) AS Calls,
								SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
								SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS DurationInSeconds,
								--AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE NULL END) AS Rate,
								AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END) AS Rate,
								--(SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS Net,
								SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  Net,
								SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
								--AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE NULL END) AS OffPeakRate,
								AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS OffPeakRate,
								--(SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
								SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
								SUM(bs.sale_Discounts/ISNULL(ERS.Rate, 1)) AS Discount,
								SUM(bs.sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
								SUM(bs.sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
							FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
							LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
							LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
							WHERE bs.calldate>=@FromDate AND bs.calldate<@ToDate
								 AND bs.CustomerID=@CustomerID
							GROUP BY Z.Name
							ORDER BY Z.Name ASC
						
					END
					ELSE
						IF(@CustomerID IS NOT NULL AND @SupplierID IS NOT NULL)
						BEGIN 
							SELECT 
								Z.Name AS Zone,
								SUM(bs.NumberOfCalls) AS Calls,
								SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
								SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS DurationInSeconds,
								--AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE NULL END) AS Rate,
								AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END) AS Rate,
								--(SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS Net,
								SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  Net,
								SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
								--AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE NULL END) AS OffPeakRate,
								AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS OffPeakRate,
								--(SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
								SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
								SUM(bs.sale_Discounts/ISNULL(ERS.Rate, 1)) AS Discount,
								SUM(bs.sale_Commissions/ISNULL(ERS.Rate, 1)) AS CommissionValue,
								SUM(bs.sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS ExtraChargeValue
							FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
							LEFT JOIN Zone Z WITH(NOLOCK INDEX(PK_Zone)) ON Z.ZoneID=bs.SaleZoneID
							LEFT JOIN @ExchangeRates ERS ON ERS.CurrencyIn = bs.Sale_Currency AND ERS.CurrencyOut = @CurrencyID AND ERS.Date = bs.CallDate
							WHERE bs.calldate>=@FromDate AND bs.calldate<@ToDate
								 AND bs.CustomerID=@CustomerID
								 AND bs.SupplierID=@SupplierID
							GROUP BY Z.Name
							ORDER BY Z.Name ASC
						END	
    END 
	RETURN
GO
/****** Object:  StoredProcedure [dbo].[EA_CustomerSummary]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EA_CustomerSummary](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL
	)
with Recompile
	AS 
	
	SET @FromDate = dbo.DateOf(@FromDate)
	SET @ToDate = dbo.DateOf(@ToDate)
	
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)
	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

	SELECT 
		bs.SaleZoneID AS SaleZoneID,
		bs.Sale_Rate AS SaleRate,
		bs.Sale_Currency AS SaleCurrency,
		SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS SaleNet,
		SUM(bs.NumberOfCalls) AS NumberOfCalls, 
		SUM(bs.SaleDuration)/60.0 AS DurationsInMinutes
		--,SUM(bs.Sale_Rate * bs.SaleDuration) AS TotalRate
	FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date, IX_Billing_Stats_Customer))
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE  bs.calldate BETWEEN @FromDate AND @ToDate
		AND bs.CustomerID = @CustomerID
	GROUP BY 
		bs.SaleZoneID,
		bs.Sale_Rate,
		bs.Sale_Currency

  RETURN
GO
/****** Object:  StoredProcedure [dbo].[bp_CreateSupplierInvoiceZeroShift]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_CreateSupplierInvoiceZeroShift]
(
	@SupplierID varchar(5),
	@TimeZoneInfo VARCHAR(MAX),
	@FromDate Datetime,
	@ToDate Datetime,
	@IssueDate Datetime,
	@DueDate DATETIME,
	@UserID INT,
	@invoiceID int output
)
AS
BEGIN
	
	SET NOCOUNT ON 

	-- Creating the Invoice 	
	DECLARE @Amount float 
	DECLARE @Duration NUMERIC(19,6) 
	DECLARE @NumberOfCalls INT 
	DECLARE @CurrencyID varchar(3) 
	DECLARE @Serial int
	DECLARE @CurrencyExchangeRate decimal 

	SET @FromDate = CAST(
	(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) )
	)
	AS DATETIME
	)

	SET @ToDate = CAST(
	(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
	)
	AS DATETIME
	)
-- building a temp table that contains all billing data requested 
--------------------------------------------------------------------------------------
DECLARE @Pivot SMALLDATETIME
SET @Pivot = @FromDate
SELECT * INTO #TempBillingStats 
FROM Billing_Stats bs WITH(NOLOCK) WHERE bs.CallDate='1995-01-01'

WHILE @Pivot <= @ToDate
BEGIN
    --------------    
INSERT INTO  #TempBillingStats 
SELECT *   
FROM Billing_Stats bs  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
WHERE  BS.CallDate = @Pivot
	  AND  BS.SupplierID COLLATE SQL_Latin1_General_CP1256_CI_AS  = @SupplierID COLLATE SQL_Latin1_General_CP1256_CI_AS 
	  AND  BS.Cost_Currency IS NOT NULL 
    --------------
SET @Pivot = DATEADD(dd, 1, @Pivot)
END 


-- getting the currency exchange rate for the selected customer  
----------------------------------------------------------
SELECT @CurrencyID = CurrencyID
FROM   CarrierProfile  (NOLOCK)
WHERE  ProfileID = (
           SELECT ProfileID
           FROM   CarrierAccount (NOLOCK)
           WHERE  CarrierAccountID = @SupplierID
       )	
         	  
-- Exchange rates 
-----------------------------------------------------------
DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates 
SELECT gder1.Currency,
       gder1.Date,
       gder1.Rate / gder2.Rate
FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder1
       LEFT JOIN dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder2
            ON  gder1.Date = gder2.Date
            AND gder2.Currency = @CurrencyID
	-- calculating the total duration and amount for the invoice to be created 
	SET @Serial = '1' 
SELECT 
       @Duration = SUM(SaleDuration)/60.0,
       @Amount = SUM(Sale_Nets),
       @NumberOfCalls =  COUNT(*)
FROM  #TempBillingStats

IF(@Duration IS NULL OR @Amount IS NULL) RETURN
	BEGIN TRANSACTION;
		-- saving the billing invoice 
		
	INSERT INTO Billing_Invoice
	(
		BeginDate, EndDate, IssueDate, DueDate, CustomerID, SupplierID, SerialNumber, 
		Duration, Amount, NumberOfCalls, CurrencyID, IsLocked, IsPaid, UserID
	)

	SELECT @FromDate,
		   @ToDate,
		   @IssueDate,
		   @DueDate,
		   'SYS',
		   @SupplierID,
		   @Serial,
		   ISNULL(@Duration,0)/60.0,
		   ISNULL(@Amount,0),
		   ISNULL(@NumberOfCalls,0),
		   @CurrencyID,
		   'N',
		   'N',
			@UserID 
	       
		   -- Getting the Invoice ID saved 
	SELECT @InvoiceID = @@IDENTITY 
	UPDATE Billing_Invoice
	SET    EndDate = dateadd(dd,-1,EndDate)
	WHERE  InvoiceID = @InvoiceID


	-- Getting the Billing Invoice Details 

	INSERT INTO Billing_Invoice_Details
	(
		InvoiceID, Destination, FromDate, TillDate, Duration, Rate, RateType, Amount, NumberOfCalls, CurrencyID, UserID
	)
	SELECT @InvoiceID,
		   CAST( BS.CostZoneID AS CHAR(100)) COLLATE SQL_Latin1_General_CP1256_CI_AS ,
		   MIN(BS.CallDate),
		   MAX(BS.CallDate),
		   SUM(BS.CostDuration)/60.0,
		   Round(BS.Cost_Rate,5),
		   CAST( BS.Cost_RateType  AS CHAR(100)) COLLATE SQL_Latin1_General_CP1256_CI_AS  ,
		   ISNULL(Round(SUM(BS.Cost_Nets),2),0),
		   SUM(BS.NumberOfCalls) ,
		   BS.Cost_Currency,
		   @UserID
	 FROM  #TempBillingStats BS
	GROUP BY
		   CAST( BS.CostZoneID AS CHAR(100)) COLLATE SQL_Latin1_General_CP1256_CI_AS ,
		   Round(BS.Cost_Rate,5),
		   CAST( BS.Cost_RateType  AS CHAR(100)) COLLATE SQL_Latin1_General_CP1256_CI_AS  ,
		   BS.Cost_Currency


-----------------------------------------------------------

SELECT @Duration = SUM(bid.Duration),
       @NumberOfCalls = SUM(bid.NumberOfCalls)
FROM   Billing_Invoice_Details bid WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice))
WHERE  bid.InvoiceID = @InvoiceID

SELECT @Amount = SUM(BS.Cost_Nets / ISNULL(ERS.Rate, 1)) 
FROM #TempBillingStats BS
LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Cost_Currency AND ERS.Date = dbo.DateOf(BS.CallDate)

-----------------------------------------------------------


	UPDATE Billing_Invoice
	SET    Duration = ISNULL(@Duration,0),
		   Amount = ISNULL(@Amount,0) ,
		   NumberOfCalls = ISNULL(@NumberOfCalls,0)
	WHERE  InvoiceID = @InvoiceID

	-- updating the ZoneName 
	UPDATE Billing_Invoice_Details
	SET    Destination = Zone.Name
	FROM   Billing_Invoice_Details WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice)),
		   Zone WITH(NOLOCK)
	WHERE  Billing_Invoice_Details.Destination = Zone.ZoneID
	  AND  Billing_Invoice_Details.InvoiceID = @InvoiceID
	  

	UPDATE Billing_Invoice
	SET InvoiceNotes = @TimeZoneInfo WHERE InvoiceID = @InvoiceID
	-- Rollback the transaction if there were any errors
	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
	    
		-- Raise an error and return
		-- RAISERROR ('Error Creating Invoice', 16, 1)
		RETURN
	END
	COMMIT
     
	RETURN

END
GO
/****** Object:  StoredProcedure [dbo].[rpt_DailyCarrierSummary]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rpt_DailyCarrierSummary](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@Cost char(1)='Y',
	@IsGroupedByDay char(1)='N' 
	)
with Recompile
	AS 
		SET @FromDate=     CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	
	
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)
	
	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	
  IF @cost='Y'
		BEGIN
     		SELECT 
   			CASE WHEN @IsGroupedByDay ='Y' THEN cast(bs.calldate  AS varchar(11)) ELSE  bs.SupplierID END AS [Day],
   			bs.SupplierID AS CarrierID,
			SUM(bs.NumberOfCalls) AS Attempts, 
			SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet], 
			SUM(bs.CostDuration)/60.0 AS Duration,
			SUM(bs.Cost_Nets / ISNULL(ERC.Rate, 1))  AS Net
			FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
			LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
			WHERE  bs.calldate>=@FromDate AND bs.calldate<@ToDate
			AND (@CustomerID IS NULL OR bs.CustomerID=@CustomerID) 
			AND (@SupplierID IS NULL OR bs.SupplierID=@SupplierID)
			GROUP BY 
			bs.SupplierID,CASE WHEN @IsGroupedByDay ='Y' THEN cast(bs.calldate AS varchar(11))ELSE  bs.SupplierID  END
		END
  ELSE 
		BEGIN
			SELECT 
			CASE WHEN @IsGroupedByDay ='Y' THEN cast(bs.calldate AS varchar(11)) ELSE  bs.CustomerID END AS [Day], 
			bs.CustomerID AS CarrierID,
			SUM(bs.NumberOfCalls) AS Attempts, 
			SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet], 
			SUM(bs.SaleDuration)/60.0 AS Duration,
			SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Net
			FROM   Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
			WHERE  bs.calldate >=@FromDate AND bs.calldate<@ToDate
			AND (@CustomerID IS NULL OR bs.CustomerID=@CustomerID) 
			AND (@SupplierID IS NULL OR bs.SupplierID=@SupplierID)
			GROUP BY 
			bs.CustomerID,CASE WHEN @IsGroupedByDay ='Y' THEN cast(bs.calldate AS varchar(11))ELSE  bs.CustomerID END  
        END           
RETURN
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_CarrierZones]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TrafficStats_CarrierZones]
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = NULL,
   @SupplierID Varchar(5) 
AS
BEGIN
	SET @FromDate=     CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate= CAST(
(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)	

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	

Declare @Traffic TABLE (ZoneID int  primary KEY, Attempts int, DurationsInMinutes NUMERIC(19,6), SuccessfulAttempts NUMERIC(19,6) ,ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5))
SET NOCOUNT ON 	
INSERT INTO @Traffic(ZoneID, Attempts, SuccessfulAttempts, DurationsInMinutes, ASR,ACD,DeliveredASR,AveragePDD)
SELECT 
	   ISNULL(TS.OurZoneID, ''),
	   Sum(Attempts) as Attempts, 
	   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
	   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
	   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
	   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
	   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	   Avg(PDDinSeconds) as AveragePDD 
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
    WHERE (FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <= @ToDate)
			AND (TS.CustomerID = @CustomerID)
			AND TS.SupplierID = @SupplierID
	GROUP BY TS.OurZoneID
	
DECLARE @Result TABLE (
		ZoneID int primary KEY,Attempts int, AttemptPercentage DECIMAL(13,5), DurationsInMinutes  NUMERIC(19,6), SuccessfulAttempts NUMERIC(19,6),ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5),
		NumberOfCalls int,Cost_Nets float, Sale_Nets float,Profit NUMERIC(19,6),Percentage numeric (13,5) 
		)

INSERT INTO @Result(ZoneID ,Attempts, DurationsInMinutes,SuccessfulAttempts, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Cost_Nets, Sale_Nets, Profit, Percentage)
	SELECT 
		  TS.ZoneID , TS.Attempts, TS.DurationsInMinutes,TS.SuccessfulAttempts,TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,
	      Sum(BS.NumberOfCalls) AS Calls,
	      SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Cost,
	      Sum(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Sale,
	      SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) - SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit,
	      0	      
    FROM @Traffic TS 
		LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) ON TS.ZoneID= BS.SaleZoneID
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	Where (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) AND (BS.CustomerID=@CustomerID) AND BS.SupplierID = @SupplierID
	GROUP BY TS.ZoneID, TS.Attempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,TS.SuccessfulAttempts
	 
Declare @TotalProfit numeric(13,5)
SELECT  @TotalProfit = SUM(profit) FROM @Result
UPDATE  @Result SET Percentage = (Profit * 100. / @TotalProfit)
Declare @TotalAttempts bigint
SELECT  @TotalAttempts = SUM(Attempts) FROM @Result
Update  @Result SET AttemptPercentage = (Attempts * 100. / @TotalAttempts)

SELECT * FROM @Result ORDER BY DurationsInMinutes DESC 

END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_CustomerZones]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TrafficStats_CustomerZones]
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = NULL,
   @SupplierID Varchar(5) 
AS
BEGIN
	SET @FromDate=     CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate= CAST(
(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)	

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	

Declare @Traffic TABLE (ZoneID int  primary KEY, Attempts int, DurationsInMinutes numeric(13,5), SuccessfulAttempts numeric (13,5),ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5))
SET NOCOUNT ON 	
INSERT INTO @Traffic(ZoneID, Attempts, SuccessfulAttempts, DurationsInMinutes, ASR,ACD,DeliveredASR,AveragePDD)
SELECT 
	   ISNULL(TS.OurZoneID, ''),
	   Sum(Attempts) as Attempts, 
	   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
	   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
	   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
	   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
	   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	   Avg(PDDinSeconds) as AveragePDD 
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
    WHERE (FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <= @ToDate)
			AND (TS.CustomerID = @CustomerID)
			AND TS.SupplierID = @SupplierID
	GROUP BY TS.OurZoneID
	
DECLARE @Result TABLE (
		ZoneID int primary KEY,Attempts int, AttemptPercentage DECIMAL(13,5), DurationsInMinutes numeric(13,5), SuccessfulAttempts numeric (13,5),ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5),
		NumberOfCalls int,Cost_Nets float, Sale_Nets float,Profit numeric (13,5),Percentage numeric (13,5) 
		)

INSERT INTO @Result(ZoneID ,Attempts, DurationsInMinutes,SuccessfulAttempts, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Cost_Nets, Sale_Nets, Profit, Percentage)
	SELECT 
		  TS.ZoneID , TS.Attempts, TS.DurationsInMinutes,TS.SuccessfulAttempts,TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,
	      Sum(BS.NumberOfCalls) AS Calls,
	      SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Cost,
	      Sum(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Sale,
	      SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) - SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit,
	      0	      
    FROM @Traffic TS 
		LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) ON TS.ZoneID= BS.SaleZoneID
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	Where (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) AND (BS.CustomerID=@CustomerID) AND BS.SupplierID = @SupplierID
	GROUP BY TS.ZoneID, TS.Attempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,TS.SuccessfulAttempts
	 
Declare @TotalProfit numeric(13,5)
SELECT  @TotalProfit = SUM(profit) FROM @Result
UPDATE  @Result SET Percentage = (Profit * 100. / @TotalProfit)
Declare @TotalAttempts bigint
SELECT  @TotalAttempts = SUM(Attempts) FROM @Result
Update  @Result SET AttemptPercentage = (Attempts * 100. / @TotalAttempts)

SELECT * FROM @Result ORDER BY DurationsInMinutes DESC 

END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_ZoneCustomers]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TrafficStats_ZoneCustomers]
   @fromDate datetime,
   @ToDate datetime,
   @SupplierID Varchar(5) = NULL,
   @ZoneID INT 
AS
BEGIN
	SET @FromDate=     CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate= CAST(
(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)	

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	

Declare @Traffic TABLE (CustomerID VarChar(15)  primary KEY, Attempts int, DurationsInMinutes numeric(13,5), SuccessfulAttempts numeric (13,5),ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5))
SET NOCOUNT ON 	
INSERT INTO @Traffic(CustomerID, Attempts, SuccessfulAttempts, DurationsInMinutes, ASR,ACD,DeliveredASR,AveragePDD)
SELECT 
	   ISNULL(TS.CustomerID, ''),
	   Sum(Attempts) as Attempts, 
	   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
	   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
	   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
	   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
	   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	   Avg(PDDinSeconds) as AveragePDD 
      
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier))
    WHERE (FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <= @ToDate)
			AND (TS.SupplierID = @SupplierID)
			AND TS.SupplierZoneID = @ZoneID 		 		
	GROUP BY TS.CustomerID
	
DECLARE @Result TABLE (
		CustomerID VarChar(15)primary KEY,Attempts int, AttemptPercentage DECIMAL(13,5), DurationsInMinutes numeric(13,5), SuccessfulAttempts numeric (13,5),ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5),
		NumberOfCalls int,Cost_Nets float, Sale_Nets float,Profit numeric (13,5),Percentage numeric (13,5) 
		)

INSERT INTO @Result(CustomerID ,Attempts, DurationsInMinutes,SuccessfulAttempts, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Cost_Nets, Sale_Nets, Profit, Percentage)
	SELECT 
		  TS.CustomerID, TS.Attempts, TS.DurationsInMinutes,TS.SuccessfulAttempts,TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,
	      Sum(BS.NumberOfCalls) AS Calls,
	      SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Cost,
	      Sum(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Sale,
	      SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) - SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit,
	      0	      
    FROM @Traffic TS 
		LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) ON TS.CustomerID= BS.CustomerID
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	Where (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) AND (BS.SupplierID=@SupplierID) AND BS.CostZoneID = @ZoneID
	GROUP BY TS.CustomerID, TS.Attempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,TS.SuccessfulAttempts
	 
Declare @TotalProfit numeric(13,5)
SELECT  @TotalProfit = SUM(profit) FROM @Result
UPDATE  @Result SET Percentage = (Profit * 100. / @TotalProfit)
Declare @TotalAttempts bigint
SELECT  @TotalAttempts = SUM(Attempts) FROM @Result
Update  @Result SET AttemptPercentage = (Attempts * 100. / @TotalAttempts)

SELECT * FROM @Result ORDER BY DurationsInMinutes DESC 

END
GO
/****** Object:  StoredProcedure [dbo].[rpt_CarrierSummary]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rpt_CarrierSummary]
(
	@FromDate Datetime,
	@ToDate Datetime,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL
)
WITH RECOMPILE
AS
SET @FromDate = CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate = CAST(
(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)	
	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
	)
	
	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

	SELECT bs.SupplierID AS SupplierID,
		   bs.CustomerID AS CustomerID,
		   SUM(bs.SaleDuration)/60.0 AS SaleDuration,
		   SUM(bs.CostDuration)/60.0 AS CostDuration,
		   SUM(bs.Cost_Nets / ISNULL(ERC.Rate, 1)) AS CostNet,
		   SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS SaleNet,
		   SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CostCommissionValue,
		   SUM(bs.Sale_Commissions/ISNULL(ERS.Rate, 1)) AS SaleCommissionValue,
		   SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS CostExtraChargeValue,
		   SUM(bs.Sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS SaleExtraChargeValue
	FROM   Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE  bs.CallDate >= @FromDate
	  AND  bs.CallDate < @ToDate
	  AND  ( @CustomerID IS NULL OR  bs.CustomerID = @CustomerID )
	  AND  ( @SupplierID IS NULL OR  bs.SupplierID = @SupplierID )
	GROUP BY
		   bs.SupplierID,
		   bs.CustomerID
	RETURN
GO
/****** Object:  StoredProcedure [dbo].[rpt_ZoneSummary_Old]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[rpt_ZoneSummary_Old](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@Cost char(1)='Y' 
)

	AS 

	SET @FromDate=     CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	

	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

	IF (@Cost = 'Y')
	BEGIN 
			SELECT 
				Z.Name AS Zone,
				SUM(bs.NumberOfCalls) AS Calls,
			    SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
				SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END) AS DurationInSeconds,
				--AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS Rate,
				AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END) AS Rate,
				--(SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS Net,
				SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  Net,
				SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
				--AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS OffPeakRate,
				AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END) AS OffPeakRate,
				--(SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
				SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
				SUM(bs.Cost_Discounts) AS Discount,
				SUM(bs.Cost_Commissions) AS CommissionValue,
				SUM(bs.Cost_ExtraCharges) AS ExtraChargeValue
			FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
			LEFT JOIN Zone Z WITH(NOLOCK INDEX(IX_Zone_Name)) ON Z.ZoneID=bs.CostZoneID   
			LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
			WHERE bs.calldate >=@FromDate AND bs.calldate<@ToDate
			     AND (@CustomerID IS NULL OR bs.CustomerID=@CustomerID) 
	             AND (@SupplierID IS NULL OR bs.SupplierID=@SupplierID)		
			GROUP BY 
			    Z.Name
			ORDER BY Z.Name ASC 
				
	END 
	ELSE 
	BEGIN 
		     SELECT 
				Z.Name AS Zone,
				SUM(bs.NumberOfCalls) AS Calls,
			    SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
				SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS DurationInSeconds,
				--AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE NULL END) AS Rate,
				AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END) AS Rate,
				--(SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS Net,
				SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  Net,
				SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
				--AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE NULL END) AS OffPeakRate,
				AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS OffPeakRate,
				--(SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
				SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
				SUM(bs.sale_Discounts) AS Discount,
				SUM(bs.sale_Commissions) AS CommissionValue,
				SUM(bs.sale_ExtraCharges) AS ExtraChargeValue
			FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
			LEFT JOIN Zone Z WITH(NOLOCK INDEX(IX_Zone_Name)) ON Z.ZoneID=bs.SaleZoneID
		    LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
			WHERE bs.calldate>=@FromDate AND bs.calldate<@ToDate
			     AND (@CustomerID IS NULL OR bs.CustomerID=@CustomerID) 
	             AND (@SupplierID IS NULL OR bs.SupplierID=@SupplierID)		
			GROUP BY 
			    Z.Name
			ORDER BY Z.Name ASC
    END 
	RETURN
GO
/****** Object:  StoredProcedure [dbo].[GISAD_DashBoard]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GISAD_DashBoard]
	@FromDate datetime ,
	@ToDate datetime 
AS
SET @FromDate = CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate = CAST(
(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)
BEGIN
SET NOCOUNT ON
DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
)
INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	SELECT
		bs.CallDate AS Date,
		SUM(bs.NumberOfCalls) AS NumberOfCalls,
		SUM(bs.SaleDuration)/60.0 AS SaleDuration,
		SUM(bs.CostDuration)/60.0 AS CostDuration,
		SUM(bs.Cost_Nets / ISNULL(ERC.Rate, 1)) AS CostNet,
		SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS SaleNet
	INTO #billing 			
	FROM   Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE  bs.CallDate >= @FromDate
	   AND bs.CallDate < @ToDate
	GROUP BY bs.CallDate
				
	SELECT 
		dbo.dateof(ts.FirstCDRAttempt) AS Date,
		Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		Sum(SuccessfulAttempts) AS SuccessfulAttempts,
		Sum(NumberOfCalls) AS NumberOfCalls,
		Sum(deliveredAttempts) AS deliveredAttempts,
		Sum(DurationsInSeconds) AS DurationsInSeconds,
		Sum(Attempts) AS Attempts,
		Avg(PDDinSeconds) as AveragePDD, 
		Max (MaxDurationInSeconds)/60.0 as MaxDuration,
		Max(LastCDRAttempt) as LastAttempt
	INTO #traffic
	FROM TrafficStats ts WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst)) 
	WHERE  ts.FirstCDRAttempt BETWEEN  @FromDate AND @ToDate  
	GROUP BY dbo.dateof(ts.FirstCDRAttempt)
	
	SELECT 
		Sum(b.NumberOfCalls) AS NumberOfCalls,
		Sum(b.SaleDuration) AS SaleDuration,
		Sum(b.CostDuration) AS CostDuration,
		Sum(b.CostNet) AS CostNet,
		Sum(b.SaleNet) AS SaleNet,
		Sum(b.SaleNet) - Sum(b.CostNet) AS Profit,
		Sum(t.DurationsInMinutes) AS DurationsInMinutes,
		Sum(t.SuccessfulAttempts)*100.0 / Sum(t.NumberOfCalls) as ASR,
		case when Sum(t.SuccessfulAttempts) > 0 then Sum(t.DurationsInSeconds)/(60.0*Sum(t.SuccessfulAttempts)) ELSE 0 end as ACD,
		Sum(t.deliveredAttempts) * 100.0 / Sum(t.Attempts) as DeliveredASR, 
		Avg(t.AveragePDD) AS AveragePDD,
		Max(t.MaxDuration) AS MaxDuration,
		Max(t.LastAttempt) AS LastAttempt,
		Sum(t.SuccessfulAttempts) AS SuccessfulAttempts
	FROM #traffic t
	LEFT JOIN #billing b ON t.Date = b.Date

END
GO
/****** Object:  StoredProcedure [dbo].[rpt_CarrierLostReport]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rpt_CarrierLostReport]
(
	@FromDate Datetime,
	@ToDate Datetime,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@Margin int
)
WITH RECOMPILE
AS
SET @FromDate=     CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate= CAST(
(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)	 

	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)
	
	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	
	
	SELECT  bs.CustomerID AS CustomerID,
			bs.SaleZoneID AS SaleZoneID,
			bs.CostZoneID AS CostZoneID,
			bs.SupplierID AS SupplierID,
			SUM(bs.SaleDuration)/60.0 AS Duration,
			SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)) AS CostNet,
			SUM(bs.sale_nets / ISNULL(ERS.Rate, 1)) AS SaleNet
		FROM   Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
		JOIN CarrierAccount cac ON cac.CarrierAccountID = bs.CustomerID AND cac.IsPassThroughCustomer ='N' 
		JOIN CarrierAccount cas ON cas.CarrierAccountID = bs.SupplierID AND cas.IsPassThroughSupplier ='N'
	    WHERE	  bs.CallDate >= @FromDate
			AND	  bs.CallDate < @ToDate
			AND  (@CustomerID IS NULL OR  bs.CustomerID = @CustomerID)
			AND  (@SupplierID IS NULL OR  bs.SupplierID = @SupplierID)
		GROUP BY
		bs.CustomerID, bs.SaleZoneID, bs.CostZoneID, bs.SupplierID
	HAVING  ( 1 - (sum(bs.cost_nets / ISNULL(ERC.Rate, 1)) / sum(bs.sale_nets / ISNULL(ERS.Rate, 1))) ) * 100  < @Margin
RETURN
GO
/****** Object:  StoredProcedure [dbo].[rpt_ZoneProfit]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[rpt_ZoneProfit](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL
)with Recompile
	AS 
		SET @FromDate=     CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	
	
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)
	
	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

    SELECT 
		CZ.Name AS CostZone, 
		SZ.Name AS SaleZone, 
		bs.SupplierID  AS SupplierID, 
		SUM(bs.NumberOfCalls) AS Calls,
		SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
		SUM(bs.CostDuration/60.0) AS CostDuration,
		SUM(bs.SaleDuration/60.0) AS SaleDuration, 
		SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)) AS  CostNet, 
		SUM(bs.sale_nets / ISNULL(ERS.Rate, 1)) AS SaleNet         			
	 FROM
	     Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer),INDEX(IX_Billing_Stats_Supplier))
	    LEFT JOIN Zone SZ WITH(NOLOCK) ON SZ.ZoneID = bs.SaleZoneID
		LEFT JOIN Zone CZ WITH(NOLOCK) ON CZ.ZoneID = bs.CostZoneID
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE bs.calldate>=@FromDate AND bs.calldate<@ToDate
	 	  AND (@CustomerID IS NULL OR bs.CustomerID=@CustomerID) 
          AND (@SupplierID IS NULL OR bs.SupplierID=@SupplierID)	
	GROUP BY 
	    CZ.Name,SZ.Name,bs.SupplierID
	ORDER BY CZ.Name,SZ.Name,bs.SupplierID ASC 
	
RETURN
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_BySupplier]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TrafficStats_BySupplier]
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = null
AS
BEGIN
SET @FromDate=     CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate= CAST(
(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)	

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)


Declare @Traffic TABLE (SupplierID VarChar(15)  primary KEY, Attempts int, DurationsInMinutes numeric(13,5), SuccessfulAttempts numeric (13,5),ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5))
SET NOCOUNT ON 	
INSERT INTO @Traffic(SupplierID, Attempts, SuccessfulAttempts, DurationsInMinutes, ASR,ACD,DeliveredASR,AveragePDD)
SELECT 
	   ISNULL(TS.SupplierID, ''),
	   Sum(Attempts) as Attempts, 
	   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
	   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
	   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
	   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
	   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	   Avg(PDDinSeconds) as AveragePDD 
      
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
    WHERE (FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <= @ToDate)
			AND TS.CustomerID = @CustomerID 		 		
	GROUP BY TS.SupplierID
	
DECLARE @Result TABLE (
		SupplierID VarChar(15)primary KEY,Attempts int, AttemptPercentage DECIMAL(13,5), DurationsInMinutes numeric(13,5), SuccessfulAttempts numeric (13,5),ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5),
		NumberOfCalls int,Cost_Nets float, Sale_Nets float,Profit numeric (13,5),AverageSaleRate DECIMAL(13,5), AverageCostRate DECIMAL(13,5), Percentage numeric (13,5) 
		)

INSERT INTO @Result(SupplierID ,Attempts, DurationsInMinutes,SuccessfulAttempts, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Cost_Nets, Sale_Nets, Profit,AverageSaleRate,AverageCostRate, Percentage)
	SELECT 
		  TS.SupplierID, TS.Attempts, TS.DurationsInMinutes,TS.SuccessfulAttempts,TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,
	      Sum(BS.NumberOfCalls) AS Calls,
	      SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Cost,
	      Sum(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Sale,
	      SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) - SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit,
	      AVG(bs.Sale_Rate),
	      AVG(bs.Cost_Rate),
	      0	      
    FROM @Traffic TS 
		LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) ON TS.SupplierID= BS.SupplierID AND (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) AND (BS.CustomerID=@CustomerID) 
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	GROUP BY TS.SupplierID, TS.Attempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,TS.SuccessfulAttempts
	 
Declare @TotalProfit numeric(13,5)
SELECT  @TotalProfit = SUM(profit) FROM @Result
UPDATE  @Result SET Percentage = (Profit * 100. / @TotalProfit)
Declare @TotalAttempts bigint
SELECT  @TotalAttempts = SUM(Attempts) FROM @Result
Update  @Result SET AttemptPercentage = (Attempts * 100. / @TotalAttempts)

SELECT * FROM @Result ORDER BY DurationsInMinutes DESC 

END
GO
/****** Object:  StoredProcedure [dbo].[rpt_DailySummary]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rpt_DailySummary](
	@FromDate Datetime ,
	@ToDate Datetime 
)
with Recompile
	AS 
		SET @FromDate=     CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	
	
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)
	
	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

     SELECT 
			CAST(bs.calldate AS varchar(11))  [Day],
			SUM(bs.NumberOfCalls) AS Calls,
			SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],    
			SUM(bs.SaleDuration / 60.0) [SaleDuration], 
			SUM(bs.sale_nets / ISNULL(ERS.Rate, 1)) AS SaleNet,           			
			SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)) AS  CostNet
		    FROM Billing_Stats bs  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
		    LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate			
            LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
            WHERE bs.calldate >=@fromdate AND bs.calldate<@ToDate
			GROUP BY 
			    CAST(bs.calldate AS varchar(11))   
			ORDER BY CAST(bs.calldate AS varchar(11))   DESC	
	
	RETURN
GO
/****** Object:  StoredProcedure [dbo].[Sp_SummaryReport]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[Sp_SummaryReport]
    @FromDate    datetime,
    @ToDate        datetime,
    @CustomerID        varchar(10) = NULL,
    @SupplierID        varchar(10) = NULL,
    @SwitchID        tinyInt = NULL,
    @OurZoneID         int = NULL ,
    @PeriodType VARCHAR(10)
WITH RECOMPILE
AS
BEGIN   
    SET NOCOUNT ON

    DECLARE @ExchangeRates TABLE(
        Currency VARCHAR(3),
        Date SMALLDATETIME,
        Rate FLOAT
        PRIMARY KEY(Currency, Date)
    )
   
    INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate);
   
    WITH TrafficTable AS
    (
    SELECT
        Hour = CASE @PeriodType
        WHEN 'Hourly' THEN datepart(hour,FirstCDRAttempt)
        ELSE NULL END,
        [Date] = CASE @PeriodType
        WHEN 'Hourly' THEN datepart(dd,FirstCDRAttempt)
        WHEN 'Daily'  THEN datepart(dd,FirstCDRAttempt)
        ELSE NULL END,
        Week = CASE @PeriodType
        WHEN 'Weekly' THEN datepart(ww,FirstCDRAttempt)
        ELSE NULL END,
        [Month] = CASE @PeriodType
        WHEN 'Hourly' THEN datepart(month,FirstCDRAttempt)
        WHEN 'Daily' THEN datepart(month,FirstCDRAttempt)
        ELSE NULL END,
        [Year] = datepart(year,FirstCDRAttempt),
        TS.OurZoneID AS OurZoneID,
        Sum(Attempts) as Attempts,
        Sum(SuccessfulAttempts) AS SuccessfulAttempts,
        Sum(SuccessfulAttempts) * 100.0 / Sum(Attempts) as ASR,
        AVG(PDDInSeconds / 60.0) AS AvgPDD,
        Sum(DurationsInSeconds /60.0) as DurationsInMinutes,
        case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))  
   WHERE  
        FirstCDRAttempt BETWEEN @FromDate AND @ToDate
        AND (@CustomerID IS NULL OR TS.CustomerID = @CustomerID)
        AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID)
        AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
        AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
    GROUP BY  
		CASE @PeriodType
        WHEN 'Hourly' THEN datepart(hour,FirstCDRAttempt)
        ELSE NULL END,
        CASE @PeriodType
        WHEN 'Hourly' THEN datepart(dd,FirstCDRAttempt)
        WHEN 'Daily'  THEN datepart(dd,FirstCDRAttempt)
        ELSE NULL END,
        CASE @PeriodType
        WHEN 'Weekly' THEN datepart(ww,FirstCDRAttempt)
        ELSE NULL END,
        CASE @PeriodType
        WHEN 'Hourly' THEN datepart(month,FirstCDRAttempt)
        WHEN 'Daily' THEN datepart(month,FirstCDRAttempt)
        ELSE NULL END,
        datepart(year,FirstCDRAttempt),
        TS.OurZoneID
      
    ),

BillingTable AS 
(
SELECT 
        Hour = CASE @PeriodType
        WHEN 'Hourly' THEN datepart(hour,bcm.Attempt)
        ELSE NULL END,
        [Date] = CASE @PeriodType
        WHEN 'Hourly' THEN datepart(dd,bcm.Attempt)
        WHEN 'Daily'  THEN datepart(dd,bcm.Attempt)
        ELSE NULL END,
        Week = CASE @PeriodType
        WHEN 'Weekly' THEN datepart(ww,bcm.Attempt)
        ELSE NULL END,
        [Month] = CASE @PeriodType
        WHEN 'Hourly' THEN datepart(month,bcm.Attempt)
        WHEN 'Daily' THEN datepart(month,bcm.Attempt)
        ELSE NULL END,
        [Year] = datepart(year,bcm.Attempt)
       ,bcm.OurZoneID AS OurZoneID
       ,AVG(bcs.RateValue) AS SaleRate
       ,SUM(bcs.Net) AS SaleNet
       ,SUM(bcs.DurationInSeconds / 60.0) AS SaleDuration
       ,AVG(bcc.RateValue) AS CostRate
       ,SUM(bcc.Net) AS CostNet
       ,SUM(bcc.DurationInSeconds / 60.0) AS CostDuration
       ,SUM(bcs.Net - bcc.Net) / sum(bcs.DurationInSeconds / 60.0)  AS MarginPerMinute
       ,SUM(bcs.Net - bcc.Net) AS Margin  
      
FROM Billing_CDR_Main bcm  WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt))
   LEFT JOIN Billing_CDR_Sale bcs WITH(nolock) ON bcs.ID = bcm.ID  
   LEFT JOIN Billing_CDR_Cost bcc WITH(nolock) ON bcc.ID = bcm.ID
WHERE bcm.attempt BETWEEN @FromDate AND @ToDate 
        AND (@CustomerID IS NULL OR bcm.CustomerID = @CustomerID)
        AND (@SupplierID IS NULL OR bcm.SupplierID = @SupplierID)
        AND (@SwitchID IS NULL OR bcm.SwitchID = @SwitchID)
        AND (@OurZoneID IS NULL OR bcm.OurZoneID = @OurZoneID) 
GROUP BY
		CASE @PeriodType
        WHEN 'Hourly' THEN datepart(hour,bcm.Attempt)
        ELSE NULL END,
        CASE @PeriodType
        WHEN 'Hourly' THEN datepart(dd,bcm.Attempt)
        WHEN 'Daily'  THEN datepart(dd,bcm.Attempt)
        ELSE NULL END,
        CASE @PeriodType
        WHEN 'Weekly' THEN datepart(ww,bcm.Attempt)
        ELSE NULL END,
        CASE @PeriodType
        WHEN 'Hourly' THEN datepart(month,bcm.Attempt)
        WHEN 'Daily' THEN datepart(month,bcm.Attempt)
        ELSE NULL END,
        datepart(year,bcm.Attempt),
        bcm.OurZoneID
)

SELECT * FROM trafficTable T
LEFT JOIN BillingTable B 
ON T.OurZoneID = B.OurZoneID 
 AND T.Hour = B.Hour
 AND T.[Date] = B.[Date]
 AND T.Week = B.Week
 AND T.[Month] = B.[Month]
 AND T.[Year] = B.[Year]
order BY T.[Year],T.[Month],T.Week,T.[Date],T.Hour
END
GO
/****** Object:  StoredProcedure [dbo].[bp_CreateSupplierInvoice]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_CreateSupplierInvoice]
(
	@SupplierID varchar(5),
	@GMTShifttime int,
	@TimeZoneInfo VARCHAR(MAX),
	@FromDate Datetime,
	@ToDate Datetime,
	@IssueDate Datetime,
	@DueDate DATETIME,
	@UserID INT,
	@InvoiceID int output
)
AS
BEGIN
	
	SET NOCOUNT ON 

	-- Creating the Invoice 	
	DECLARE @Amount float 
	DECLARE @Duration NUMERIC(19,6)  
	DECLARE @NumberOfCalls INT 
	DECLARE @CurrencyID varchar(3) 
	DECLARE @Serial int
	DECLARE @CurrencyExchangeRate decimal 
DECLARE @From SMALLDATETIME
SET @From = @FromDate

DECLARE @To SMALLDATETIME
SET @To = @ToDate
	SET @FromDate = CAST(
	(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) )
	)
	AS DATETIME
	)

	SET @ToDate = CAST(
	(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
	)
	AS DATETIME
	)

-- Apply the Time Shift 
SET @FromDate = dateadd(mi,-@GMTShifttime,@FromDate);
SET @ToDate = dateadd(mi,-@GMTShifttime,@ToDate);

-- building a temp table that contains all billing data requested 
--------------------------------------------------------------------------------------

DECLARE @Pivot SMALLDATETIME
SET @Pivot = @FromDate

SELECT 
bcm.Attempt, bcm.Connect,
       bcm.CustomerID, bcm.OurZoneID,
       bcm.SupplierID, bcm.SupplierZoneID, bcc.ZoneID, bcc.Net, bcc.CurrencyID,
       bcc.RateValue, bcc.RateType, bcc.DurationInSeconds 
 INTO #TempBillingCDR 
  FROM  Billing_CDR_Main bcm WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt))
	   LEFT JOIN Billing_CDR_Cost bcc (NOLOCK) ON  bcc.ID = bcm.ID
 WHERE  bcm.Attempt  = '1900-01-01'
  
WHILE @Pivot <= @ToDate
BEGIN
 ----------------------
 DECLARE @BeginDate DATETIME
 DECLARE @EndDate DATETIME
 SET @BeginDate = @Pivot

 SET @EndDate = DATEADD(dd, 1, @Pivot)

INSERT  INTO  #TempBillingCDR  
SELECT bcm.Attempt, bcm.Connect,
       bcm.CustomerID, bcm.OurZoneID,
       bcm.SupplierID, bcm.SupplierZoneID, bcc.ZoneID, bcc.Net, bcc.CurrencyID,
       bcc.RateValue, bcc.RateType, bcc.DurationInSeconds
 FROM  Billing_CDR_Main bcm WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt))
	   LEFT JOIN Billing_CDR_Cost bcc (NOLOCK) ON  bcc.ID = bcm.ID
 WHERE  bcm.Attempt >= @BeginDate
	  AND  bcm.Attempt <= @EndDate
	  AND  bcm.SupplierID = @SupplierID  
	  AND  bcc.CurrencyID IS NOT NULL 
  ------------------------------ 
    SET @Pivot = DATEADD(dd, 1, @Pivot)
END 


UPDATE #TempBillingCDR
SET Attempt =  dateadd(mi,@GMTShifttime,Attempt),
    Connect =  dateadd(mi,@GMTShifttime,Connect)

-- Apply the Time Shift 
SET @FromDate = dateadd(mi,@GMTShifttime,@FromDate);
SET @ToDate = dateadd(mi,@GMTShifttime,@ToDate);
-- getting the currency exchange rate for the selected customer  
----------------------------------------------------------
SELECT @CurrencyID = CurrencyID
FROM   CarrierProfile (NOLOCK)
WHERE  ProfileID = ( 
           SELECT ProfileID
           FROM   CarrierAccount (NOLOCK)
           WHERE  CarrierAccountID = @SupplierID
       )	
         	  
-- Exchange rates 
-----------------------------------------------------------
DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates 
SELECT gder1.Currency,
       gder1.Date,
       gder1.Rate / gder2.Rate
FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder1
       LEFT JOIN dbo.GetDailyExchangeRates(@FromDate, @ToDate) gder2
            ON  gder1.Date = gder2.Date
            AND gder2.Currency = @CurrencyID
	-- calculating the total duration and amount for the invoice to be created 
	SET @Serial = '1' 
SELECT 
       @Duration = SUM(DurationInSeconds)/60.0,
       @Amount = SUM(Net),
       @NumberOfCalls =  COUNT(*)
FROM  #TempBillingCDR

IF(@Duration IS NULL OR @Amount IS NULL) RETURN
	BEGIN TRANSACTION;
		-- saving the billing invoice 
		
	INSERT INTO Billing_Invoice
	(
		BeginDate, EndDate, IssueDate, DueDate, CustomerID, SupplierID, SerialNumber, 
		Duration, Amount, NumberOfCalls, CurrencyID, IsLocked, IsPaid, UserID
	)

	SELECT @From,
		   @To,
		   @IssueDate,
		   @DueDate,
		   'SYS',
		   @SupplierID,
		   @Serial,
		   ISNULL(@Duration,0)/60.0,
		   ISNULL(@Amount,0),
		   ISNULL(@NumberOfCalls,0),
		   @CurrencyID,
		   'N',
		   'N',
			@UserID 
	       
		   -- Getting the Invoice ID saved 
	SELECT @InvoiceID = @@IDENTITY 

	-- Getting the Billing Invoice Details 

	INSERT INTO Billing_Invoice_Details
	(
		InvoiceID, Destination, FromDate, TillDate, Duration, Rate, RateType, Amount, NumberOfCalls, CurrencyID, UserID
	)
	SELECT 
      @InvoiceID,
       B.ZoneID,
       MIN(B.Attempt),
       MAX(B.Attempt),
       SUM(B.DurationInSeconds)/60.0,
       Round(B.RateValue,5) ,
       B.RateType,
       ISNULL(Round(SUM(B.Net),2),0),
       COUNT(*),
       B.CurrencyID,
       @UserID
    FROM    #TempBillingCDR B
	GROUP BY
		   B.ZoneID,
		   Round(B.RateValue,5),
		   B.RateType,
		   B.CurrencyID

SELECT @Duration = SUM(bid.Duration),
       @NumberOfCalls = SUM(bid.NumberOfCalls)
FROM   Billing_Invoice_Details bid WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice))
WHERE  bid.InvoiceID = @InvoiceID

SELECT @Amount = SUM(B.Net / ISNULL(ERS.Rate, 1)) 
FROM #TempBillingCDR B
LEFT JOIN @ExchangeRates ERS ON ERS.Currency = B.CurrencyID AND ERS.Date = dbo.DateOf(B.Attempt)


	UPDATE Billing_Invoice
	SET    Duration = ISNULL(@Duration,0),
		   Amount = ISNULL(@Amount,0) ,
		   NumberOfCalls = ISNULL(@NumberOfCalls,0)
	WHERE  InvoiceID = @InvoiceID

	-- updating the ZoneName 
	UPDATE Billing_Invoice_Details
	SET    Destination = Zone.Name
	FROM   Billing_Invoice_Details WITH(NOLOCK,INDEX(IX_Billing_Invoice_Details_Invoice)),
		   Zone WITH(NOLOCK)
	WHERE  Billing_Invoice_Details.Destination = Zone.ZoneID
	  AND  Billing_Invoice_Details.InvoiceID = @InvoiceID



	UPDATE Billing_Invoice
	SET InvoiceNotes = @TimeZoneInfo WHERE InvoiceID = @InvoiceID
	
	select @InvoiceID
	-- Rollback the transaction if there were any errors
	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
	    
		-- Raise an error and return
		-- RAISERROR ('Error Creating Invoice', 16, 1)
		RETURN
	END
	COMMIT
     
	RETURN

END
GO
/****** Object:  UserDefinedFunction [dbo].[GetCommissionedRate]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ===============================================================================================================
-- Author:		Fadi Chamieh
-- Create date: 11/07/2007
-- Description:	Get the commissioned Rate (Commission and Extra Charges) for a Supplier/Customer on a given Zone
-- ===============================================================================================================
CREATE FUNCTION [dbo].[GetCommissionedRate]
(
	@SupplierID varchar(10) = NULL,
	@CustomerID varchar(10) = NULL,
	@RateCurrency CHAR(3),
	@EffectiveDate smalldatetime = getdate,
	@ZoneID int,
	@Rate float
)
RETURNS float
AS
BEGIN
	-- Declare the return variable here
	DECLARE @CommissionedRate float

	SELECT @CommissionedRate = 
		@Rate 
			-- Add Commission (if any)
			+ dbo.GetCommission(@SupplierID, @CustomerID,@RateCurrency, @EffectiveDate, @ZoneID, @Rate, 'N')
			-- Add Extra Charges (if any)
			+ dbo.GetCommission(@SupplierID, @CustomerID,@RateCurrency, @EffectiveDate, @ZoneID, @Rate, 'Y')

	-- Return the result of the function
	RETURN @CommissionedRate

END
GO
/****** Object:  StoredProcedure [dbo].[bp_CompressAndClean]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_CompressAndClean]
 (   @Batch BIGINT=1000
 	,@TrafficCompressDays INT = 7 -- 7 Days
 	,@TrafficStatsRemainDays INT = 31 -- 1 month
 	,@BillingRemainDays INT = 31 -- 1 month
 	,@CDRRemainDays INT = 93 -- 3 months
 	,@IsDebug char(1)='N'
 )
WITH 
RECOMPILE,
EXECUTE AS CALLER
AS
BEGIN
	
	DECLARE @DeletedCount bigint
	Declare @WorkingDay datetime
	Declare @WorkingDayEnd datetime
	DECLARE @MainIDs TABLE(ID bigint NOT NULL PRIMARY key)
	Declare @MinId bigint
	Declare @MaxId BIGINT
	DECLARE @WorkingDayDesc VARCHAR(10)
    DECLARE @Till DATETIME 

	DECLARE @Now DATETIME 
	SET @Now = dbo.DateOf(GETDATE())
	
	DECLARE @Dummy int	

	SET NOCOUNT ON	

	DECLARE @Message VARCHAR(4000)
	DECLARE @MsgID VARCHAR(100)
	DECLARE @MsgSub VARCHAR(100)
	SET @MsgID = 'Compress And Clean'
	
	-----------------------------
        -- Compress Traffic Stats
	-----------------------------
	SET @MsgSub = @MsgID + ': Traffic Stats'
	SET @Message = 'compress Traffic Stats'
	IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message
	
	DECLARE @From DATETIME 
	SET @From =DATEADD(DAY,-@TrafficCompressDays,@Now)
	
	EXEC bp_CompressTrafficStats
		@StartingDateTime = @From,
		@EndingDateTime = @Now		
		
    -----------------------------
		-- Delete Traffic Stats
	-----------------------------
	-- Setting the traffic stats period
    	
    SELECT @WorkingDay = MIN(TS.FirstCDRAttempt) FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
	SET @WorkingDayDesc = CONVERT(varchar(10), @WorkingDay, 121)	
  	SET @WorkingDayEnd = DATEADD(ms, -3, DATEADD(DAY, 1, @WorkingDay))
    SET @Till = DATEADD(ms, -3, DATEADD(DAY,-@TrafficStatsRemainDays , @WorkingDay))  
     
	WHILE @WorkingDay <= @Till
	BEGIN		
		SET ROWCOUNT @Batch
		
		SET @Message = 'Working On: ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgID, @message = @Message
				
		SET @Message = 'Cleaning Traffic Stats for ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message

		BEGIN
			SELECT @DeletedCount = 1
			WHILE @DeletedCount > 0
			BEGIN
				BEGIN TRANSACTION
			        DELETE TrafficStats FROM TrafficStats WITH(NOLOCK, INDEX(IX_TrafficStats_DateTimeFirst)) WHERE FirstCDRAttempt BETWEEN @WorkingDay AND @WorkingDayEnd
					SET @DeletedCount = @@ROWCOUNT
				COMMIT TRANSACTION	
			END
			IF @IsDebug = 'Y' PRINT 'Deleted Traffic Stats ' + convert(varchar(25), getdate(), 121)
		END

		-- Move to next Day
		IF @IsDebug = 'Y' PRINT 'Finished: ' + @WorkingDayDesc
		SET @WorkingDay = dateadd(day, 1, @WorkingDay)
		SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)
		SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))

		IF @IsDebug = 'Y' PRINT 'Next: ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT '---------------------------------------------'
	END
    
     -----------------------------
		-- Delete Traffic Stats
	-----------------------------
	-- Setting the CDR period
    	
    SELECT @WorkingDay = MIN(C.AttemptDateTime) FROM CDR C WITH(NOLOCK,INDEX(IX_CDR_AttemptDateTime))
	SET @WorkingDayDesc = CONVERT(varchar(10), @WorkingDay, 121)	
  	SET @WorkingDayEnd = DATEADD(ms, -3, DATEADD(DAY, 1, @WorkingDay))
    SET @Till = DATEADD(ms, -3, DATEADD(DAY,-@CDRRemainDays , @WorkingDay))  
     
	WHILE @WorkingDay <= @Till
	BEGIN		
		SET ROWCOUNT @Batch
		
		SET @MsgSub = @MsgID + ': CDR'
		SET @Message = 'Cleaning CDR ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message
		
		SET @DeletedCount = 1
		WHILE @DeletedCount > 0
		BEGIN
			BEGIN TRANSACTION
					BEGIN
						DELETE CDR FROM CDR WITH(NOLOCK, INDEX(IX_CDR_AttemptDateTime)) WHERE AttemptDateTime BETWEEN @WorkingDay AND @WorkingDayEnd
					END
				SET @DeletedCount = @@ROWCOUNT
				
			COMMIT TRANSACTION
		END
		IF @IsDebug = 'Y' PRINT 'Deleted CDRs ' + convert(varchar(25), getdate(), 121)

		-- Move to next Day
		IF @IsDebug = 'Y' PRINT 'Finished: ' + @WorkingDayDesc
		SET @WorkingDay = dateadd(day, 1, @WorkingDay)
		SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)
		SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))

		IF @IsDebug = 'Y' PRINT 'Next: ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT '---------------------------------------------'
	END

    -----------------------------
	   -- Delete Billing_CDR_Invalid
	-----------------------------
	-- Setting the Billing_CDR_Invalid period
    	
    SELECT @WorkingDay = MIN(BI.Attempt) FROM Billing_CDR_Invalid BI WITH(NOLOCK,INDEX(IX_Billing_CDR_Invalid_Attempt))
	SET @WorkingDayDesc = CONVERT(varchar(10), @WorkingDay, 121)	
  	SET @WorkingDayEnd = DATEADD(ms, -3, DATEADD(DAY, 1, @WorkingDay))
    SET @Till = DATEADD(ms, -3, DATEADD(DAY,-@CDRRemainDays , @WorkingDay))  
     
	WHILE @WorkingDay <= @Till
	BEGIN		
		SET ROWCOUNT @Batch
		
	    SET @MsgSub = @MsgID + ': Billing CDR Invalid'
		SET @Message = 'Cleaning Billing CDR Invalid ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message
		
		SET @DeletedCount = 1
		WHILE @DeletedCount > 0
		BEGIN
			BEGIN TRANSACTION
					BEGIN
						DELETE Billing_CDR_Invalid FROM Billing_CDR_Invalid WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd
					END
				SET @DeletedCount = @@ROWCOUNT
				
			COMMIT TRANSACTION
		END
		IF @IsDebug = 'Y' PRINT 'Deleted Invalid CDRs ' + convert(varchar(25), getdate(), 121)

		-- Move to next Day
		IF @IsDebug = 'Y' PRINT 'Finished: ' + @WorkingDayDesc
		SET @WorkingDay = dateadd(day, 1, @WorkingDay)
		SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)
		SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))

		IF @IsDebug = 'Y' PRINT 'Next: ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT '---------------------------------------------'
	END


    -----------------------------
	   -- Delete Billing_CDR_Main
	-----------------------------
	-- Setting the Billing_CDR_Main period
    	
    SELECT @WorkingDay = MIN(BM.Attempt) FROM Billing_CDR_Main BM WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt))
	SET @WorkingDayDesc = CONVERT(varchar(10), @WorkingDay, 121)	
  	SET @WorkingDayEnd = DATEADD(ms, -3, DATEADD(DAY, 1, @WorkingDay))
    SET @Till = DATEADD(ms, -3, DATEADD(DAY,-@CDRRemainDays , @WorkingDay))  
     
	WHILE @WorkingDay <= @Till
	BEGIN		
		SET ROWCOUNT @Batch
		
	   SET @MsgSub = @MsgID + ': Main, Cost, Sale - Start'
		SET @Message = 'Deleting Billing CDR Main ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message

		-- Clear Batch Row Count Number (to insert all Mains)
		SET @DeletedCount = 1
		
		-- While there are still Main CDRs
		WHILE @DeletedCount > 0
		BEGIN
			
			SET ROWCOUNT @Batch

			DELETE FROM @MainIDs
			
			INSERT INTO @MainIDs SELECT ID FROM Billing_CDR_Main WITH(NOLOCK, INDEX(IX_Billing_CDR_Main_Attempt)) WHERE Attempt BETWEEN @WorkingDay AND @WorkingDayEnd 
			
			SET ROWCOUNT 0
			
			SELECT @DeletedCount = COUNT(*) FROM @MainIDs
		
			-- If there is something to delete
			IF @DeletedCount > 0
			BEGIN				
				BEGIN TRANSACTION
					-----------------------------
					-- Delete Billing Costs 
					-----------------------------
					DELETE Billing_CDR_Cost FROM Billing_CDR_Cost WITH(NOLOCK) WHERE ID IN (SELECT ID FROM @MainIDs)

					-----------------------------
					-- Delete Billing Sales
					-----------------------------
					DELETE Billing_CDR_Sale FROM Billing_CDR_Sale WITH(NOLOCK) WHERE ID IN (SELECT ID FROM @MainIDs)

					-----------------------------
					-- Delete Main Billing CDRs
					-----------------------------
					DELETE Billing_CDR_Main FROM Billing_CDR_Main WITH(NOLOCK) WHERE ID IN (SELECT ID FROM @MainIDs)
			
				COMMIT TRANSACTION		
			END	-- If @Deleted > 0	
		
		END -- While
		
		SET @MsgSub = @MsgID + ': Main, Cost, Sale - End'
		SET @Message = 'Deleted Billing CDR Main with sales and costs for ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT @Message ELSE EXEC bp_SetSystemMessage @msgID = @MsgSub, @message = @Message

		-- Move to next Day
		IF @IsDebug = 'Y' PRINT 'Finished: ' + @WorkingDayDesc
		SET @WorkingDay = dateadd(day, 1, @WorkingDay)
		SET @WorkingDayDesc = convert(varchar(10), @WorkingDay, 121)
		SET @WorkingDayEnd = dateadd(ms, -3, dateadd(day, 1, @WorkingDay))

		IF @IsDebug = 'Y' PRINT 'Next: ' + @WorkingDayDesc
		IF @IsDebug = 'Y' PRINT '---------------------------------------------'
	END


END
GO
/****** Object:  StoredProcedure [dbo].[bp_BuildBillingStatsPeriodDefined]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[bp_BuildBillingStatsPeriodDefined]
(
	@CustomerID VARCHAR(5) = null,
	@FromDate DateTime,
	@ToDate Datetime
)
AS
SET NOCOUNT ON

DECLARE @n int
DECLARE @Date datetime


SELECT @Date = @FromDate
SET @n = datediff(dd,@FromDate,@ToDate)
--SELECT @n

WHILE (@n >= 0)
BEGIN
    --SELECT @Date
    PRINT 'Working on ' + cast(@Date AS varchar(20))
    EXEC bp_BuildBillingStats
         @CustomerID = @CustomerID,
         @Day = @Date
         SET @Date = dateadd(d,1,@Date)
         SET @n = @n-1
END  
	
     
RETURN
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_CarrierSummary]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TrafficStats_CarrierSummary]
   @CarrierType VARCHAR(10),
   @fromDate datetime ,
   @ToDate datetime,
   @CustomerID varchar(15)=null,
   @SupplierID varchar(15)=null,
   @TopRecord INT =NULL
AS
BEGIN
	
	IF(@CarrierType = 'Supplier')
	 EXEC SP_TrafficStats_SupplierSummary
	 	@fromDate = @fromDate,
	 	@ToDate = @ToDate,
	 	@SupplierID = @SupplierID,
	 	@TopRecord = @TopRecord

    IF(@CarrierType = 'Customer')
	 EXEC SP_TrafficStats_CustomerSummary
	 	@fromDate = @fromDate,
	 	@ToDate = @ToDate,
	 	@CustomerID = @CustomerID,
	 	@TopRecord = @TopRecord

END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_CarrierSummary_beta]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[SP_TrafficStats_CarrierSummary_beta]
   @CarrierType VARCHAR(10),
   @fromDate datetime ,
   @ToDate datetime,
   @CustomerID varchar(15)=null,
   @SupplierID varchar(15)=null,
   @TopRecord INT =NULL
AS
BEGIN
	
	IF(@CarrierType = 'Supplier')
	 EXEC SP_TrafficStats_SupplierSummary
	 	@fromDate = @fromDate,
	 	@ToDate = @ToDate,
	 	@SupplierID = @SupplierID,
	 	@TopRecord = @TopRecord

    IF(@CarrierType = 'Customer')
	 EXEC SP_TrafficStats_SupplierSummary
	 	@fromDate = @fromDate,
	 	@ToDate = @ToDate,
	 	@CustomerID = @CustomerID,
	 	@TopRecord = @TopRecord

END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_ZoneCarriers]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[SP_TrafficStats_ZoneCarriers]
   @CarrierType VARCHAR(10),
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = NULL,
   @SupplierID Varchar(5) = NULL,
   @ZoneID INT 
AS
BEGIN
	
	IF(@CarrierType = 'Supplier')
	EXEC SP_TrafficStats_ZoneCustomers
		@fromDate = @fromDate,
		@ToDate = @ToDate,
		@SupplierID = @SupplierID,
		@ZoneID = @ZoneID
		
    IF(@CarrierType = 'Customer')
	EXEC SP_TrafficStats_ZoneSuppliers
		@fromDate = @fromDate,
		@ToDate = @ToDate,
		@CustomerID = @CustomerID,
		@ZoneID = @ZoneID

END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_ByCarrier]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[SP_TrafficStats_ByCarrier]
   @CarrierType VARCHAR(10),
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = NULL,
   @SupplierID Varchar(5) = NULL
AS
BEGIN

IF(@CarrierType = 'Supplier')
EXEC SP_TrafficStats_ByCustomer
	@fromDate = @fromDate,
	@ToDate = @ToDate,
	@SupplierID = @SupplierID

IF(@CarrierType = 'Customer')
EXEC SP_TrafficStats_BySupplier
	@fromDate = @fromDate,
	@ToDate = @ToDate,
	@CustomerID = @CustomerID

END
GO
/****** Object:  StoredProcedure [dbo].[SP_TrafficStats_ByZone]    Script Date: 03/30/2011 15:31:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[SP_TrafficStats_ByZone]
   @CarrierType VARCHAR(10),  
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = NULL,
   @SupplierID Varchar(5) = null
AS
BEGIN

IF(@CarrierType = 'Customer')
EXEC SP_TrafficStats_CustomerSaleZone
	@fromDate = @fromDate,
	@ToDate = @ToDate,
	@CustomerID = @CustomerID

IF(@CarrierType = 'Supplier')
EXEC SP_TrafficStats_SupplierCostZone
	@fromDate = @fromDate,
	@ToDate = @ToDate,
	@SupplierID =@SupplierID

END
GO
/****** Object:  Default [DF_Role_IsActive]    Script Date: 03/30/2011 15:31:13 ******/
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_IsActive]  DEFAULT ('N') FOR [IsActive]
GO
/****** Object:  Default [DF_Billing_Stats_Sale_RateType]    Script Date: 03/30/2011 15:31:13 ******/
ALTER TABLE [dbo].[Billing_Stats] ADD  CONSTRAINT [DF_Billing_Stats_Sale_RateType]  DEFAULT ((0)) FOR [Sale_RateType]
GO
/****** Object:  Default [DF_Billing_Stats_Cost_RateType]    Script Date: 03/30/2011 15:31:13 ******/
ALTER TABLE [dbo].[Billing_Stats] ADD  CONSTRAINT [DF_Billing_Stats_Cost_RateType]  DEFAULT ((0)) FOR [Cost_RateType]
GO
/****** Object:  Default [DF__HeartBeat__date__0A7E65A1]    Script Date: 03/30/2011 15:31:13 ******/
ALTER TABLE [dbo].[HeartBeat] ADD  DEFAULT (getdate()) FOR [date]
GO
/****** Object:  Default [DF_TrafficMonitor_SampleDate]    Script Date: 03/30/2011 15:31:13 ******/
ALTER TABLE [dbo].[TrafficStats] ADD  CONSTRAINT [DF_TrafficMonitor_SampleDate]  DEFAULT (getdate()) FOR [FirstCDRAttempt]
GO
/****** Object:  Default [DF_TrafficStats_MaxDurationInSeconds]    Script Date: 03/30/2011 15:31:13 ******/
ALTER TABLE [dbo].[TrafficStats] ADD  CONSTRAINT [DF_TrafficStats_MaxDurationInSeconds]  DEFAULT ((0)) FOR [MaxDurationInSeconds]
GO
/****** Object:  Default [DF__Persisted__Creat__0D5AD24C]    Script Date: 03/30/2011 15:31:14 ******/
ALTER TABLE [dbo].[PersistedCustomReport] ADD  CONSTRAINT [DF__Persisted__Creat__0D5AD24C]  DEFAULT (getdate()) FOR [Created]
GO
/****** Object:  Default [DF_PersistedCustomReport_IsEncrypted]    Script Date: 03/30/2011 15:31:14 ******/
ALTER TABLE [dbo].[PersistedCustomReport] ADD  CONSTRAINT [DF_PersistedCustomReport_IsEncrypted]  DEFAULT ('Y') FOR [IsEncrypted]
GO
/****** Object:  Default [DF_Alert_Created]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Alert] ADD  CONSTRAINT [DF_Alert_Created]  DEFAULT (getdate()) FOR [Created]
GO
/****** Object:  Default [DF_Alert_IsVisible]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Alert] ADD  CONSTRAINT [DF_Alert_IsVisible]  DEFAULT ('Y') FOR [IsVisible]
GO
/****** Object:  Default [DF_RouteOverride_IncludeSubCodes]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[RouteOverride] ADD  CONSTRAINT [DF_RouteOverride_IncludeSubCodes]  DEFAULT ('N') FOR [IncludeSubCodes]
GO
/****** Object:  Default [DF_RouteOverride_EndEffectiveDate]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[RouteOverride] ADD  CONSTRAINT [DF_RouteOverride_EndEffectiveDate]  DEFAULT ('2020-01-01') FOR [EndEffectiveDate]
GO
/****** Object:  Default [DF_PricelistImportOption_LastUpdate]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[PricelistImportOption] ADD  CONSTRAINT [DF_PricelistImportOption_LastUpdate]  DEFAULT (getdate()) FOR [LastUpdate]
GO
/****** Object:  Default [DF_SystemMessage_Updated]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[SystemMessage] ADD  CONSTRAINT [DF_SystemMessage_Updated]  DEFAULT (getdate()) FOR [Updated]
GO
/****** Object:  Default [DF_StateBackup_StateBackupType]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[StateBackup] ADD  CONSTRAINT [DF_StateBackup_StateBackupType]  DEFAULT ((0)) FOR [StateBackupType]
GO
/****** Object:  Default [DF_StateBackup_Created]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[StateBackup] ADD  CONSTRAINT [DF_StateBackup_Created]  DEFAULT (getdate()) FOR [Created]
GO
/****** Object:  Default [DF_CarrierProfile_IsNettingEnabled]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[CarrierProfile] ADD  CONSTRAINT [DF_CarrierProfile_IsNettingEnabled]  DEFAULT ('N') FOR [IsNettingEnabled]
GO
/****** Object:  Default [DF_Billing_CDR_Cost_RateType]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Billing_CDR_Cost] ADD  CONSTRAINT [DF_Billing_CDR_Cost_RateType]  DEFAULT ((0)) FOR [RateType]
GO
/****** Object:  Default [DF__Billing.C__Commi__5C37ACAD]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Billing_CDR_Cost] ADD  CONSTRAINT [DF__Billing.C__Commi__5C37ACAD]  DEFAULT ((0)) FOR [CommissionValue]
GO
/****** Object:  Default [DF__Billing.C__Extra__5D2BD0E6]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Billing_CDR_Cost] ADD  CONSTRAINT [DF__Billing.C__Extra__5D2BD0E6]  DEFAULT ((0)) FOR [ExtraChargeValue]
GO
/****** Object:  Default [DF_RouteOption_Status]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[RouteOption] ADD  CONSTRAINT [DF_RouteOption_Status]  DEFAULT ((0)) FOR [State]
GO
/****** Object:  Default [DF_SwitchReleaseCode_SwitchID]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[SwitchReleaseCode] ADD  CONSTRAINT [DF_SwitchReleaseCode_SwitchID]  DEFAULT ((0)) FOR [SwitchID]
GO
/****** Object:  Default [DF_SwitchReleaseCode_IsDelivered]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[SwitchReleaseCode] ADD  CONSTRAINT [DF_SwitchReleaseCode_IsDelivered]  DEFAULT ('N') FOR [IsDelivered]
GO
/****** Object:  Default [DF_Tariff_CallFee]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Tariff] ADD  CONSTRAINT [DF_Tariff_CallFee]  DEFAULT ((0)) FOR [CallFee]
GO
/****** Object:  Default [DF_CodeSupply_SupplierServicesFlag]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[CodeSupply] ADD  CONSTRAINT [DF_CodeSupply_SupplierServicesFlag]  DEFAULT ((0)) FOR [SupplierServicesFlag]
GO
/****** Object:  Default [DF_CodeSupply_Updated]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[CodeSupply] ADD  CONSTRAINT [DF_CodeSupply_Updated]  DEFAULT (getdate()) FOR [Updated]
GO
/****** Object:  Default [DF_CarrierSwitchConnectivity_ConnectionType]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[CarrierSwitchConnectivity] ADD  CONSTRAINT [DF_CarrierSwitchConnectivity_ConnectionType]  DEFAULT ((0)) FOR [ConnectionType]
GO
/****** Object:  Default [DF_CarrierDocument_Created]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[CarrierDocument] ADD  CONSTRAINT [DF_CarrierDocument_Created]  DEFAULT (getdate()) FOR [Created]
GO
/****** Object:  Default [DF_PersistedRunnableTask_IsEnabled]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[PersistedRunnableTask] ADD  CONSTRAINT [DF_PersistedRunnableTask_IsEnabled]  DEFAULT ('N') FOR [IsEnabled]
GO
/****** Object:  Default [DF_Currency_IsMainCurrency]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Currency] ADD  CONSTRAINT [DF_Currency_IsMainCurrency]  DEFAULT ('N') FOR [IsMainCurrency]
GO
/****** Object:  Default [DF_Currency_IsVisible]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Currency] ADD  CONSTRAINT [DF_Currency_IsVisible]  DEFAULT ('Y') FOR [IsVisible]
GO
/****** Object:  Default [DF_Commission_IsExtraCharges]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Commission] ADD  CONSTRAINT [DF_Commission_IsExtraCharges]  DEFAULT ('N') FOR [IsExtraCharge]
GO
/****** Object:  Default [DF_CDR_Is_Rerouted]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[CDR] ADD  CONSTRAINT [DF_CDR_Is_Rerouted]  DEFAULT ('N') FOR [IsRerouted]
GO
/****** Object:  Default [DF_Billing_CDR_Invalid_Is_Rerouted]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Billing_CDR_Invalid] ADD  CONSTRAINT [DF_Billing_CDR_Invalid_Is_Rerouted]  DEFAULT ('N') FOR [IsRerouted]
GO
/****** Object:  Default [DF_Billing_CDR_Sale_RateType]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Billing_CDR_Sale] ADD  CONSTRAINT [DF_Billing_CDR_Sale_RateType]  DEFAULT ((0)) FOR [RateType]
GO
/****** Object:  Default [DF__Billing.C__Commi__5F141958]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Billing_CDR_Sale] ADD  CONSTRAINT [DF__Billing.C__Commi__5F141958]  DEFAULT ((0)) FOR [CommissionValue]
GO
/****** Object:  Default [DF__Billing.C__Extra__60083D91]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Billing_CDR_Sale] ADD  CONSTRAINT [DF__Billing.C__Extra__60083D91]  DEFAULT ((0)) FOR [ExtraChargeValue]
GO
/****** Object:  Default [DF_Switch_Enable_CDR_Import]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Switch] ADD  CONSTRAINT [DF_Switch_Enable_CDR_Import]  DEFAULT ('Y') FOR [Enable_CDR_Import]
GO
/****** Object:  Default [DF_Switch_Enable_Routing]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Switch] ADD  CONSTRAINT [DF_Switch_Enable_Routing]  DEFAULT ('Y') FOR [Enable_Routing]
GO
/****** Object:  Default [DF_Switch_NominalCapacityInE1s]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Switch] ADD  CONSTRAINT [DF_Switch_NominalCapacityInE1s]  DEFAULT ((64)) FOR [NominalTrunkCapacityInE1s]
GO
/****** Object:  Default [DF_RouteChangeHeader_IsEnded]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[RouteChangeHeader] ADD  CONSTRAINT [DF_RouteChangeHeader_IsEnded]  DEFAULT ('N') FOR [IsEnded]
GO
/****** Object:  Default [DF_RoutingPool_IsEnable]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[RoutingPool] ADD  CONSTRAINT [DF_RoutingPool_IsEnable]  DEFAULT ('N') FOR [IsEnabled]
GO
/****** Object:  Default [DF_PrepaidPostpaidOptions_IsCustomer]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[PrepaidPostpaidOptions] ADD  CONSTRAINT [DF_PrepaidPostpaidOptions_IsCustomer]  DEFAULT ('Y') FOR [IsCustomer]
GO
/****** Object:  Default [DF_User_IsActive]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_IsActive]  DEFAULT ('N') FOR [IsActive]
GO
/****** Object:  Default [DF_CarrierAccount_IsOriginatingZonesEnabled]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[CarrierAccount] ADD  CONSTRAINT [DF_CarrierAccount_IsOriginatingZonesEnabled]  DEFAULT ('N') FOR [IsOriginatingZonesEnabled]
GO
/****** Object:  Default [DF_CarrierAccount_IsPassThroughCustomer]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[CarrierAccount] ADD  CONSTRAINT [DF_CarrierAccount_IsPassThroughCustomer]  DEFAULT ('N') FOR [IsPassThroughCustomer]
GO
/****** Object:  Default [DF_CarrierAccount_IsPassThroughSupplier]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[CarrierAccount] ADD  CONSTRAINT [DF_CarrierAccount_IsPassThroughSupplier]  DEFAULT ('N') FOR [IsPassThroughSupplier]
GO
/****** Object:  Default [DF_CarrierAccount_RepresentsASwitch]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[CarrierAccount] ADD  CONSTRAINT [DF_CarrierAccount_RepresentsASwitch]  DEFAULT ('N') FOR [RepresentsASwitch]
GO
/****** Object:  Default [DF_CarrierAccount_IsAToZ]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[CarrierAccount] ADD  CONSTRAINT [DF_CarrierAccount_IsAToZ]  DEFAULT ('N') FOR [IsAToZ]
GO
/****** Object:  Default [DF_CarrierAccount_IsNettingEnabled]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[CarrierAccount] ADD  CONSTRAINT [DF_CarrierAccount_IsNettingEnabled]  DEFAULT ('N') FOR [IsNettingEnabled]
GO
/****** Object:  Default [DF_Route_State]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Route] ADD  CONSTRAINT [DF_Route_State]  DEFAULT ((0)) FOR [State]
GO
/****** Object:  Default [DF_Route_IsToDAffected]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Route] ADD  CONSTRAINT [DF_Route_IsToDAffected]  DEFAULT ('N') FOR [IsToDAffected]
GO
/****** Object:  Default [DF_Route_IsSpecialRequestAffected]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Route] ADD  CONSTRAINT [DF_Route_IsSpecialRequestAffected]  DEFAULT ('N') FOR [IsSpecialRequestAffected]
GO
/****** Object:  Default [DF_Route_IsBlockAffected]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Route] ADD  CONSTRAINT [DF_Route_IsBlockAffected]  DEFAULT ('N') FOR [IsBlockAffected]
GO
/****** Object:  ForeignKey [FK_RolePermission_Permission]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[RolePermission]  WITH CHECK ADD  CONSTRAINT [FK_RolePermission_Permission] FOREIGN KEY([Permission])
REFERENCES [dbo].[Permission] ([ID])
GO
ALTER TABLE [dbo].[RolePermission] CHECK CONSTRAINT [FK_RolePermission_Permission]
GO
/****** Object:  ForeignKey [FK_RolePermission_Role]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[RolePermission]  WITH NOCHECK ADD  CONSTRAINT [FK_RolePermission_Role] FOREIGN KEY([Role])
REFERENCES [dbo].[Role] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RolePermission] CHECK CONSTRAINT [FK_RolePermission_Role]
GO
/****** Object:  ForeignKey [FK_UserRole_Role]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[UserRole]  WITH NOCHECK ADD  CONSTRAINT [FK_UserRole_Role] FOREIGN KEY([Role])
REFERENCES [dbo].[Role] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_Role]
GO
/****** Object:  ForeignKey [FK_UserRole_User]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[UserRole]  WITH NOCHECK ADD  CONSTRAINT [FK_UserRole_User] FOREIGN KEY([User])
REFERENCES [dbo].[User] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_User]
GO
/****** Object:  ForeignKey [FK_CarrierAccount_CarrierGroup]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[CarrierAccount]  WITH CHECK ADD  CONSTRAINT [FK_CarrierAccount_CarrierGroup] FOREIGN KEY([CarrierGroupID])
REFERENCES [dbo].[CarrierGroup] ([CarrierGroupID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[CarrierAccount] CHECK CONSTRAINT [FK_CarrierAccount_CarrierGroup]
GO
/****** Object:  ForeignKey [FK_Code_Zone]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Code]  WITH NOCHECK ADD  CONSTRAINT [FK_Code_Zone] FOREIGN KEY([ZoneID])
REFERENCES [dbo].[Zone] ([ZoneID])
GO
ALTER TABLE [dbo].[Code] CHECK CONSTRAINT [FK_Code_Zone]
GO
/****** Object:  ForeignKey [FK_PricingTemplatePlan_PricingTemplate]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[PricingTemplatePlan]  WITH CHECK ADD  CONSTRAINT [FK_PricingTemplatePlan_PricingTemplate] FOREIGN KEY([PricingTemplateID])
REFERENCES [dbo].[PricingTemplate] ([PricingTemplateId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PricingTemplatePlan] CHECK CONSTRAINT [FK_PricingTemplatePlan_PricingTemplate]
GO
/****** Object:  ForeignKey [FK_Zone_CarrierAccount]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Zone]  WITH NOCHECK ADD  CONSTRAINT [FK_Zone_CarrierAccount] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Zone] CHECK CONSTRAINT [FK_Zone_CarrierAccount]
GO
/****** Object:  ForeignKey [FK_CodeMatch_CarrierAccount]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[CodeMatch]  WITH NOCHECK ADD  CONSTRAINT [FK_CodeMatch_CarrierAccount] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
GO
ALTER TABLE [dbo].[CodeMatch] CHECK CONSTRAINT [FK_CodeMatch_CarrierAccount]
GO
/****** Object:  ForeignKey [FK_CarrierAccountConnection_CarrierAccount]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[CarrierAccountConnection]  WITH CHECK ADD  CONSTRAINT [FK_CarrierAccountConnection_CarrierAccount] FOREIGN KEY([CarrierAccountID])
REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CarrierAccountConnection] CHECK CONSTRAINT [FK_CarrierAccountConnection_CarrierAccount]
GO
/****** Object:  ForeignKey [FK_CarrierAccountConnection_Switch]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[CarrierAccountConnection]  WITH CHECK ADD  CONSTRAINT [FK_CarrierAccountConnection_Switch] FOREIGN KEY([SwitchID])
REFERENCES [dbo].[Switch] ([SwitchID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CarrierAccountConnection] CHECK CONSTRAINT [FK_CarrierAccountConnection_Switch]
GO
/****** Object:  ForeignKey [FK_Billing_Invoice_Costs_Billing_Invoice]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Billing_Invoice_Costs]  WITH CHECK ADD  CONSTRAINT [FK_Billing_Invoice_Costs_Billing_Invoice] FOREIGN KEY([InvoiceID])
REFERENCES [dbo].[Billing_Invoice] ([InvoiceID])
GO
ALTER TABLE [dbo].[Billing_Invoice_Costs] CHECK CONSTRAINT [FK_Billing_Invoice_Costs_Billing_Invoice]
GO
/****** Object:  ForeignKey [FK_Billing_Invoice_Costs_CarrierAccount]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Billing_Invoice_Costs]  WITH CHECK ADD  CONSTRAINT [FK_Billing_Invoice_Costs_CarrierAccount] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Billing_Invoice_Costs] CHECK CONSTRAINT [FK_Billing_Invoice_Costs_CarrierAccount]
GO
/****** Object:  ForeignKey [FK_Route_CarrierAccount]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Route]  WITH NOCHECK ADD  CONSTRAINT [FK_Route_CarrierAccount] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
GO
ALTER TABLE [dbo].[Route] CHECK CONSTRAINT [FK_Route_CarrierAccount]
GO
/****** Object:  ForeignKey [FK_SpecialRequest_CarrierAccount]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[SpecialRequest]  WITH CHECK ADD  CONSTRAINT [FK_SpecialRequest_CarrierAccount] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
GO
ALTER TABLE [dbo].[SpecialRequest] CHECK CONSTRAINT [FK_SpecialRequest_CarrierAccount]
GO
/****** Object:  ForeignKey [FK_SpecialRequest_CarrierAccount1]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[SpecialRequest]  WITH CHECK ADD  CONSTRAINT [FK_SpecialRequest_CarrierAccount1] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
GO
ALTER TABLE [dbo].[SpecialRequest] CHECK CONSTRAINT [FK_SpecialRequest_CarrierAccount1]
GO
/****** Object:  ForeignKey [FK_PriceList_Customer]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[PriceList]  WITH NOCHECK ADD  CONSTRAINT [FK_PriceList_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
GO
ALTER TABLE [dbo].[PriceList] CHECK CONSTRAINT [FK_PriceList_Customer]
GO
/****** Object:  ForeignKey [FK_PriceList_Supplier]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[PriceList]  WITH NOCHECK ADD  CONSTRAINT [FK_PriceList_Supplier] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
GO
ALTER TABLE [dbo].[PriceList] CHECK CONSTRAINT [FK_PriceList_Supplier]
GO
/****** Object:  ForeignKey [FK_RouteBlock_CarrierAccount]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[RouteBlock]  WITH CHECK ADD  CONSTRAINT [FK_RouteBlock_CarrierAccount] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
GO
ALTER TABLE [dbo].[RouteBlock] CHECK CONSTRAINT [FK_RouteBlock_CarrierAccount]
GO
/****** Object:  ForeignKey [FK_RouteBlock_CarrierAccount1]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[RouteBlock]  WITH CHECK ADD  CONSTRAINT [FK_RouteBlock_CarrierAccount1] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
GO
ALTER TABLE [dbo].[RouteBlock] CHECK CONSTRAINT [FK_RouteBlock_CarrierAccount1]
GO
/****** Object:  ForeignKey [FK_RatePlan_CarrierAccount]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[RatePlan]  WITH CHECK ADD  CONSTRAINT [FK_RatePlan_CarrierAccount] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RatePlan] CHECK CONSTRAINT [FK_RatePlan_CarrierAccount]
GO
/****** Object:  ForeignKey [FK_ZoneRate_Customer]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[ZoneRate]  WITH CHECK ADD  CONSTRAINT [FK_ZoneRate_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
GO
ALTER TABLE [dbo].[ZoneRate] CHECK CONSTRAINT [FK_ZoneRate_Customer]
GO
/****** Object:  ForeignKey [FK_ZoneRate_Supplier]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[ZoneRate]  WITH CHECK ADD  CONSTRAINT [FK_ZoneRate_Supplier] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
GO
ALTER TABLE [dbo].[ZoneRate] CHECK CONSTRAINT [FK_ZoneRate_Supplier]
GO
/****** Object:  ForeignKey [FK_Billing_Invoice_Details_Billing_Invoice]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Billing_Invoice_Details]  WITH CHECK ADD  CONSTRAINT [FK_Billing_Invoice_Details_Billing_Invoice] FOREIGN KEY([InvoiceID])
REFERENCES [dbo].[Billing_Invoice] ([InvoiceID])
GO
ALTER TABLE [dbo].[Billing_Invoice_Details] CHECK CONSTRAINT [FK_Billing_Invoice_Details_Billing_Invoice]
GO
/****** Object:  ForeignKey [FK_WebSiteMenuItemPermission_Permission]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[WebSiteMenuItemPermission]  WITH CHECK ADD  CONSTRAINT [FK_WebSiteMenuItemPermission_Permission] FOREIGN KEY([PermissionID])
REFERENCES [dbo].[Permission] ([ID])
GO
ALTER TABLE [dbo].[WebSiteMenuItemPermission] CHECK CONSTRAINT [FK_WebSiteMenuItemPermission_Permission]
GO
/****** Object:  ForeignKey [FK_websitemenuitempermission_WebsiteMenuItem]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[WebSiteMenuItemPermission]  WITH CHECK ADD  CONSTRAINT [FK_websitemenuitempermission_WebsiteMenuItem] FOREIGN KEY([ItemID])
REFERENCES [dbo].[WebsiteMenuItem] ([ItemID])
GO
ALTER TABLE [dbo].[WebSiteMenuItemPermission] CHECK CONSTRAINT [FK_websitemenuitempermission_WebsiteMenuItem]
GO
/****** Object:  ForeignKey [FK_Rate_PriceList]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[Rate]  WITH NOCHECK ADD  CONSTRAINT [FK_Rate_PriceList] FOREIGN KEY([PriceListID])
REFERENCES [dbo].[PriceList] ([PriceListID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Rate] CHECK CONSTRAINT [FK_Rate_PriceList]
GO
/****** Object:  ForeignKey [FK_PlaningRate_RatePlan]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[PlaningRate]  WITH CHECK ADD  CONSTRAINT [FK_PlaningRate_RatePlan] FOREIGN KEY([RatePlanID])
REFERENCES [dbo].[RatePlan] ([RatePlanID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PlaningRate] CHECK CONSTRAINT [FK_PlaningRate_RatePlan]
GO
/****** Object:  ForeignKey [FK_FaultTicketUpdate_FaultTicket]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[FaultTicketUpdate]  WITH CHECK ADD  CONSTRAINT [FK_FaultTicketUpdate_FaultTicket] FOREIGN KEY([FaultTicketID])
REFERENCES [dbo].[FaultTicket] ([FaultTicketID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FaultTicketUpdate] CHECK CONSTRAINT [FK_FaultTicketUpdate_FaultTicket]
GO
/****** Object:  ForeignKey [fk_RPC_ID]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[RoutingPoolCustomer]  WITH CHECK ADD  CONSTRAINT [fk_RPC_ID] FOREIGN KEY([ID])
REFERENCES [dbo].[RoutingPool] ([ID])
GO
ALTER TABLE [dbo].[RoutingPoolCustomer] CHECK CONSTRAINT [fk_RPC_ID]
GO
/****** Object:  ForeignKey [fk_RPS_ID]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[RoutingPoolSupplier]  WITH CHECK ADD  CONSTRAINT [fk_RPS_ID] FOREIGN KEY([ID])
REFERENCES [dbo].[RoutingPool] ([ID])
GO
ALTER TABLE [dbo].[RoutingPoolSupplier] CHECK CONSTRAINT [fk_RPS_ID]
GO
/****** Object:  ForeignKey [FK_UserOption_User]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[UserOption]  WITH NOCHECK ADD  CONSTRAINT [FK_UserOption_User] FOREIGN KEY([User])
REFERENCES [dbo].[User] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserOption] CHECK CONSTRAINT [FK_UserOption_User]
GO
/****** Object:  ForeignKey [fk_UP_ID]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[UserPermissions]  WITH CHECK ADD  CONSTRAINT [fk_UP_ID] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[UserPermissions] CHECK CONSTRAINT [fk_UP_ID]
GO
/****** Object:  ForeignKey [FK_ToDConsideration_CarrierAccount]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[ToDConsideration]  WITH CHECK ADD  CONSTRAINT [FK_ToDConsideration_CarrierAccount] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
GO
ALTER TABLE [dbo].[ToDConsideration] CHECK CONSTRAINT [FK_ToDConsideration_CarrierAccount]
GO
/****** Object:  ForeignKey [FK_ToDConsideration_CarrierAccount1]    Script Date: 03/30/2011 15:31:16 ******/
ALTER TABLE [dbo].[ToDConsideration]  WITH CHECK ADD  CONSTRAINT [FK_ToDConsideration_CarrierAccount1] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
GO
ALTER TABLE [dbo].[ToDConsideration] CHECK CONSTRAINT [FK_ToDConsideration_CarrierAccount1]
GO
