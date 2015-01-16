USE [TABS]
GO
/****** Object:  Table [dbo].[RouteOption]    Script Date: 11/25/2009 10:57:25 ******/
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
	[State] [tinyint] NOT NULL CONSTRAINT [DF_RouteOption_Status]  DEFAULT ((0)),
	[Updated] [datetime] NULL,
	[Percentage] [tinyint] NULL
) ON [VOLATILE]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IDX_RouteOption_RouteID] ON [dbo].[RouteOption] 
(
	[RouteID] ASC
)ON [VOLATILE]
GO
CREATE NONCLUSTERED INDEX [IDX_RouteOption_SupplierZoneID] ON [dbo].[RouteOption] 
(
	[SupplierZoneID] ASC
)ON [VOLATILE]
GO
CREATE NONCLUSTERED INDEX [IDX_RouteOption_Updated] ON [dbo].[RouteOption] 
(
	[Updated] ASC
)ON [VOLATILE]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Defines the status of this route option: V for Valid, B for blocked, P for highest priority, S for Special Request' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RouteOption', @level2type=N'COLUMN',@level2name=N'State'
GO
/****** Object:  Table [dbo].[Route]    Script Date: 11/25/2009 10:57:21 ******/
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
	[State] [tinyint] NOT NULL CONSTRAINT [DF_Route_State]  DEFAULT ((0)),
	[Updated] [datetime] NULL,
	[IsToDAffected] [char](1) NOT NULL CONSTRAINT [DF_Route_IsToDAffected]  DEFAULT ('N'),
	[IsSpecialRequestAffected] [char](1) NOT NULL CONSTRAINT [DF_Route_IsSpecialRequestAffected]  DEFAULT ('N'),
	[IsBlockAffected] [char](1) NOT NULL CONSTRAINT [DF_Route_IsBlockAffected]  DEFAULT ('N'),
 CONSTRAINT [PK_Route] PRIMARY KEY CLUSTERED 
(
	[RouteID] ASC
)ON [VOLATILE]
) ON [VOLATILE]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Route_Code] ON [dbo].[Route] 
(
	[Code] ASC
)ON [VOLATILE]
GO
CREATE NONCLUSTERED INDEX [IX_Route_Customer] ON [dbo].[Route] 
(
	[CustomerID] ASC
)ON [VOLATILE]
GO
CREATE NONCLUSTERED INDEX [IX_Route_ServicesFlag] ON [dbo].[Route] 
(
	[OurServicesFlag] ASC
)ON [VOLATILE]
GO
CREATE NONCLUSTERED INDEX [IX_Route_Updated] ON [dbo].[Route] 
(
	[Updated] DESC
)ON [VOLATILE]
GO
CREATE NONCLUSTERED INDEX [IX_Route_Zone] ON [dbo].[Route] 
(
	[OurZoneID] ASC
)ON [VOLATILE]
GO
