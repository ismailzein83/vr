

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[ZeroToMax]
(
	@Value float
)
RETURNS float
AS
BEGIN
	if(@Value = 0)
	  SET @Value = 99999999999999999;
	  
RETURN @Value

END