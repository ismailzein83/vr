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