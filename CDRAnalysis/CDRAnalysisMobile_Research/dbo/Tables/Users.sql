CREATE TABLE [dbo].[Users] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [FullName]         NVARCHAR (100) NOT NULL,
    [EmailAddress]     NVARCHAR (100) NOT NULL,
    [UserName]         NVARCHAR (100) NOT NULL,
    [Password]         NVARCHAR (500) NOT NULL,
    [LastLoginTime]    DATETIME2 (0)  NULL,
    [IsActive]         BIT            NOT NULL,
    [IsSuperUser]      BIT            CONSTRAINT [DF_Users_IsSuperUser] DEFAULT ((0)) NOT NULL,
    [VerificationCode] NCHAR (6)      NULL,
    [Address]          NVARCHAR (500) NULL,
    [MobileNumber]     NVARCHAR (50)  NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([ID] ASC)
);

