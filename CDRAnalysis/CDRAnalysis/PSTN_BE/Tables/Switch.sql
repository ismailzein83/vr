CREATE TABLE [PSTN_BE].[Switch] (
    [ID]           INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (255) NOT NULL,
    [TypeID]       INT            NOT NULL,
    [AreaCode]     VARCHAR (10)   NOT NULL,
    [TimeOffset]   VARCHAR (50)   NOT NULL,
    [DataSourceID] INT            NOT NULL,
    CONSTRAINT [PK_Switch] PRIMARY KEY CLUSTERED ([ID] ASC)
);



