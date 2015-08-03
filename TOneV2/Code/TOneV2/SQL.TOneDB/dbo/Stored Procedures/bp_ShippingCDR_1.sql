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