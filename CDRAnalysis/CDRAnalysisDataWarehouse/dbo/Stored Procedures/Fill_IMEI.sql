

CREATE PROCEDURE  [dbo].[Fill_IMEI]  
	
AS
INSERT INTO dbo.Dim_IMEI
    SELECT distinct FK_IMEI  FROM dbo.Fact_Cases where FK_IMEI is not null


/*

Fill_IMEI
select * from [Dim_IMEI]
*/