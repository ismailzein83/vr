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