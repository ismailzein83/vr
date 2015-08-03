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