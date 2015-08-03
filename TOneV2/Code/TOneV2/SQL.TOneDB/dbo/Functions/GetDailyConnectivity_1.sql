


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