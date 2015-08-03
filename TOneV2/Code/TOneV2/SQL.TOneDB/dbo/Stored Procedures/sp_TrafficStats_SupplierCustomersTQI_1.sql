


CREATE PROCEDURE [dbo].[sp_TrafficStats_SupplierCustomersTQI]
    @FromDateTime DATETIME,
    @ToDateTime   DATETIME,
    @CustomerID   varchar(10) = NULL,
    @SupplierID   varchar(10) = NULL,
    @OurZoneID       INT = NULL
    
WITH recompile
AS
BEGIN


    SET NOCOUNT ON
            SELECT    CustomerID As CarrierAccountID,
                    Sum(NumberOfCalls) as Attempts,
                    Sum(DurationsInSeconds/60.0) as DurationsInMinutes,         
                    CASE WHEN Sum(NumberOfCalls) > 0 THEN Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END AS ASR,
                    case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
                    Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR,
                    CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts) ELSE NULL END as AveragePDD,
                    MAX(DurationsInSeconds)/60.0 as  MaxDuration,
                    Sum(SuccessfulAttempts)AS SuccessfulAttempts,
                    Sum(NumberOfCalls - SuccessfulAttempts) AS FailedAttempts,
                    SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
                    SUM(PDDinSeconds * TS.SuccessfulAttempts) as TotalPDD
                FROM TrafficStatsDaily TS WITH(NOLOCK)
                    WHERE
                        Calldate BETWEEN @FromDateTime AND @ToDateTime
                    AND SupplierID = @SupplierID
                    AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
                    Group By CustomerID
                    ORDER BY SUM(Attempts) DESC

    
END