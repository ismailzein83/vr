
CREATE PROCEDURE [common].[sp_EntityPersonalization_InsertIfNotExistsOrUpdate]
	@EntityUniqueName varchar(1000),
	@UserID int = null,
	@Details nvarchar(max),
	@CreatedBy int
AS
BEGIN	
	IF NOT EXISTS (
		SELECT NULL FROM common.[EntityPersonalization] WITH(NOLOCK)
			 WHERE
			[EntityUniqueName] = @EntityUniqueName AND ([UserID] = @UserID OR (@UserID IS NULL AND UserID IS NULL))
	)
	BEGIN
		INSERT INTO common.[EntityPersonalization] (
			[UserID]
           ,[EntityUniqueName]
           ,[Details]
           ,[CreatedTime]
           ,[CreatedBy]
		   ,[LastModifiedTime]
		   ,[LastModifiedBy]
        )
		 VALUES
			   (@UserID
			   ,@EntityUniqueName
			   ,@Details
			   ,GetDate()
			   ,@CreatedBy
			   ,GETDATE()
			   ,@CreatedBy)
	END
	ELSE
	BEGIN
		UPDATE [common].[EntityPersonalization]
		   SET [Details] = @Details
			  ,[LastModifiedTime] = GETDATE()
			  ,[LastModifiedBy] = @CreatedBy
		 WHERE 
		  [EntityUniqueName] = @EntityUniqueName AND ([UserID] = @UserID OR (@UserID IS NULL AND UserID IS NULL))
	END
END