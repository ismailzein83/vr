CREATE PROCEDURE [genericdata].[sp_BusinessEntityHistoryStack_Delete]
	@ID bigint
AS
BEGIN
   UPDATE [genericdata].[BusinessEntityHistoryStack] 
   SET IsDeleted = 1
   WHERE ID=@ID;
END