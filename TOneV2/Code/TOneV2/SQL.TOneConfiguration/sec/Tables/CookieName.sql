CREATE TABLE [sec].[CookieName] (
    [ID]         INT            IDENTITY (1, 1) NOT NULL,
    [CookieName] NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_CookieName] PRIMARY KEY CLUSTERED ([ID] ASC)
);

