
GO
/****** Object:  Table [dbo].[TrafficStatsDaily]    Script Date: 12/04/2014 10:21:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TrafficStatsDaily](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Calldate] [date] NULL,
	[SwitchId] [tinyint] NOT NULL,
	[CustomerID] [varchar](10) NULL,
	[OurZoneID] [int] NULL,
	[OriginatingZoneID] [int] NULL,
	[SupplierID] [varchar](10) NULL,
	[SupplierZoneID] [int] NULL,
	[Attempts] [int] NOT NULL,
	[DeliveredAttempts] [int] NOT NULL,
	[SuccessfulAttempts] [int] NOT NULL,
	[DurationsInSeconds] [numeric](19, 5) NOT NULL,
	[PDDInSeconds] [numeric](9, 2) NULL,
	[UtilizationInSeconds] [numeric](19, 5) NULL,
	[NumberOfCalls] [int] NULL,
	[DeliveredNumberOfCalls] [int] NULL,
	[PGAD] [numeric](9, 2) NULL,
	[CeiledDuration] [bigint] NULL,
	[MaxDurationInSeconds] [numeric](9, 2) NULL,
	[ReleaseSourceAParty] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE CLUSTERED INDEX [IX_TrafficStatsDaily_DateTimeFirst] ON [dbo].[TrafficStatsDaily] 
(
	[Calldate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TrafficStats]    Script Date: 12/04/2014 10:21:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TrafficStats](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[SwitchId] [tinyint] NOT NULL,
	[Port_IN] [varchar](42) NULL,
	[Port_OUT] [varchar](42) NULL,
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
	[NumberOfCalls] [int] NULL,
	[DeliveredNumberOfCalls] [int] NULL,
	[PGAD] [numeric](19, 5) NULL,
	[CeiledDuration] [bigint] NULL,
	[ReleaseSourceAParty] [int] NULL
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
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The input IP:Port or Trunk ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TrafficStats', @level2type=N'COLUMN',@level2name=N'Port_IN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The output IP:Port or Trunk ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TrafficStats', @level2type=N'COLUMN',@level2name=N'Port_OUT'
GO
/****** Object:  Table [dbo].[CDR]    Script Date: 12/04/2014 10:21:27 ******/
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
	[IN_IP] [varchar](42) NULL,
	[OUT_TRUNK] [varchar](5) NULL,
	[OUT_CIRCUIT] [smallint] NULL,
	[OUT_CARRIER] [varchar](100) NULL,
	[OUT_IP] [varchar](42) NULL,
	[CGPN] [varchar](40) NULL,
	[CDPN] [varchar](40) NULL,
	[CAUSE_FROM_RELEASE_CODE] [varchar](20) NULL,
	[CAUSE_FROM] [varchar](10) NULL,
	[CAUSE_TO_RELEASE_CODE] [varchar](20) NULL,
	[CAUSE_TO] [varchar](10) NULL,
	[Extra_Fields] [varchar](255) NULL,
	[IsRerouted] [char](1) NULL,
	[CDPNOut] [varchar](40) NULL,
	[SIP] [varchar](100) NULL,
 CONSTRAINT [PK_CDR] PRIMARY KEY CLUSTERED 
(
	[CDRID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [TOne_CDR]
) ON [TOne_CDR]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_CDR_AttemptDateTime] ON [dbo].[CDR] 
(
	[AttemptDateTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [TOne_Index]
GO
/****** Object:  Table [dbo].[Billing_CDR_Sale]    Script Date: 12/04/2014 10:21:27 ******/
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
	[Code] [varchar](20) NULL,
	[Attempt] [datetime] NULL
) ON [TOne_CDR]
GO
SET ANSI_PADDING OFF
GO
CREATE CLUSTERED INDEX [IX_Billing_CDR_Sale_Attempt] ON [dbo].[Billing_CDR_Sale] 
(
	[Attempt] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [TOne_CDR]
GO
CREATE UNIQUE NONCLUSTERED INDEX [PK_Billing_CDR_Sale] ON [dbo].[Billing_CDR_Sale] 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [TOne_CDR]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'''N'' for Normal (Peak), ''O'' for Offpeak and ''W'' for Weekend Rate ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Billing_CDR_Sale', @level2type=N'COLUMN',@level2name=N'RateType'
GO
/****** Object:  Table [dbo].[Billing_CDR_Main]    Script Date: 12/04/2014 10:21:26 ******/
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
	[Port_IN] [varchar](42) NULL,
	[Port_OUT] [varchar](42) NULL,
	[OurCode] [varchar](20) NULL,
	[SupplierCode] [varchar](20) NULL,
	[CDPNOut] [varchar](40) NULL,
	[SubscriberID] [bigint] NULL,
	[SIP] [varchar](100) NULL,
 CONSTRAINT [PK_Billing_CDR_Main] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [TOne_CDR]
) ON [TOne_CDR]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Main_Attempt] ON [dbo].[Billing_CDR_Main] 
(
	[Attempt] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 50) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Main_Customer] ON [dbo].[Billing_CDR_Main] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 50) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Main_OurZoneID] ON [dbo].[Billing_CDR_Main] 
(
	[OurZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [TOne_CDR]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Main_Supplier] ON [dbo].[Billing_CDR_Main] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 50) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Billing_CDR_Invalid]    Script Date: 12/04/2014 10:21:26 ******/
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
	[Port_IN] [varchar](42) NULL,
	[Port_OUT] [varchar](42) NULL,
	[OurCode] [varchar](20) NULL,
	[SupplierCode] [varchar](20) NULL,
	[CDPNOut] [varchar](50) NULL,
	[SubscriberID] [bigint] NULL,
	[SIP] [varchar](100) NULL
) ON [TOne_CDR]
GO
SET ANSI_PADDING OFF
GO
CREATE CLUSTERED INDEX [IX_Billing_CDR_Invalid_Attempt] ON [dbo].[Billing_CDR_Invalid] 
(
	[Attempt] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [TOne_CDR]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Invalid_Customer] ON [dbo].[Billing_CDR_Invalid] 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [TOne_Index]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Invalid_OurZoneID] ON [dbo].[Billing_CDR_Invalid] 
(
	[OurZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [TOne_Index]
GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Invalid_Supplier] ON [dbo].[Billing_CDR_Invalid] 
(
	[SupplierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [TOne_Index]
GO
/****** Object:  Table [dbo].[Billing_CDR_Cost]    Script Date: 12/04/2014 10:21:26 ******/
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
	[Code] [varchar](20) NULL,
	[Attempt] [datetime] NULL
) ON [TOne_CDR]
GO
SET ANSI_PADDING OFF
GO
CREATE CLUSTERED INDEX [IX_Billing_CDR_Cost_Attempt] ON [dbo].[Billing_CDR_Cost] 
(
	[Attempt] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [TOne_CDR]
GO
CREATE UNIQUE NONCLUSTERED INDEX [PK_Billing_CDR_Cost] ON [dbo].[Billing_CDR_Cost] 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [TOne_CDR]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'''N'' for Normal (Peak), ''O'' for Offpeak and ''W'' for Weekend Rate ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Billing_CDR_Cost', @level2type=N'COLUMN',@level2name=N'RateType'
GO
/****** Object:  Default [DF_Billing_CDR_Cost_RateType]    Script Date: 12/04/2014 10:21:26 ******/
ALTER TABLE [dbo].[Billing_CDR_Cost] ADD  CONSTRAINT [DF_Billing_CDR_Cost_RateType]  DEFAULT ((0)) FOR [RateType]
GO
/****** Object:  Default [DF__Billing.C__Commi__5C37ACAD]    Script Date: 12/04/2014 10:21:26 ******/
ALTER TABLE [dbo].[Billing_CDR_Cost] ADD  CONSTRAINT [DF__Billing.C__Commi__5C37ACAD]  DEFAULT ((0)) FOR [CommissionValue]
GO
/****** Object:  Default [DF__Billing.C__Extra__5D2BD0E6]    Script Date: 12/04/2014 10:21:26 ******/
ALTER TABLE [dbo].[Billing_CDR_Cost] ADD  CONSTRAINT [DF__Billing.C__Extra__5D2BD0E6]  DEFAULT ((0)) FOR [ExtraChargeValue]
GO
/****** Object:  Default [DF_Billing_CDR_Invalid_Is_Rerouted]    Script Date: 12/04/2014 10:21:26 ******/
ALTER TABLE [dbo].[Billing_CDR_Invalid] ADD  CONSTRAINT [DF_Billing_CDR_Invalid_Is_Rerouted]  DEFAULT ('N') FOR [IsRerouted]
GO
/****** Object:  Default [DF_Billing_CDR_Main_SIP]    Script Date: 12/04/2014 10:21:26 ******/
ALTER TABLE [dbo].[Billing_CDR_Main] ADD  CONSTRAINT [DF_Billing_CDR_Main_SIP]  DEFAULT ('') FOR [SIP]
GO
/****** Object:  Default [DF_Billing_CDR_Sale_RateType]    Script Date: 12/04/2014 10:21:27 ******/
ALTER TABLE [dbo].[Billing_CDR_Sale] ADD  CONSTRAINT [DF_Billing_CDR_Sale_RateType]  DEFAULT ((0)) FOR [RateType]
GO
/****** Object:  Default [DF__Billing.C__Commi__5F141958]    Script Date: 12/04/2014 10:21:27 ******/
ALTER TABLE [dbo].[Billing_CDR_Sale] ADD  CONSTRAINT [DF__Billing.C__Commi__5F141958]  DEFAULT ((0)) FOR [CommissionValue]
GO
/****** Object:  Default [DF__Billing.C__Extra__60083D91]    Script Date: 12/04/2014 10:21:27 ******/
ALTER TABLE [dbo].[Billing_CDR_Sale] ADD  CONSTRAINT [DF__Billing.C__Extra__60083D91]  DEFAULT ((0)) FOR [ExtraChargeValue]
GO
/****** Object:  Default [DF_CDR_Is_Rerouted]    Script Date: 12/04/2014 10:21:27 ******/
ALTER TABLE [dbo].[CDR] ADD  CONSTRAINT [DF_CDR_Is_Rerouted]  DEFAULT ('N') FOR [IsRerouted]
GO
/****** Object:  Default [DF_Billing_CDR_Retail_SIP]    Script Date: 12/04/2014 10:21:27 ******/
ALTER TABLE [dbo].[CDR] ADD  CONSTRAINT [DF_Billing_CDR_Retail_SIP]  DEFAULT ('') FOR [SIP]
GO
/****** Object:  Default [DF_TrafficMonitor_SampleDate]    Script Date: 12/04/2014 10:21:27 ******/
ALTER TABLE [dbo].[TrafficStats] ADD  CONSTRAINT [DF_TrafficMonitor_SampleDate]  DEFAULT (getdate()) FOR [FirstCDRAttempt]
GO
/****** Object:  Default [DF_TrafficStats_MaxDurationInSeconds]    Script Date: 12/04/2014 10:21:27 ******/
ALTER TABLE [dbo].[TrafficStats] ADD  CONSTRAINT [DF_TrafficStats_MaxDurationInSeconds]  DEFAULT ((0)) FOR [MaxDurationInSeconds]
GO
