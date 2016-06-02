
CREATE PROCEDURE  Fill_BTS
	
AS
INSERT INTO [dbo].[Dim_BTS]
           ([Pk_BTSId]
           ,[Name])
    SELECT distinct FK_BTS,FK_BTS  FROM dbo.Fact_Cases where FK_BTS is not null


/*

Fill_BTS
select * from [Dim_BTS]
*/