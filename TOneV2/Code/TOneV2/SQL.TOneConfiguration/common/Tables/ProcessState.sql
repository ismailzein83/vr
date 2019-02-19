CREATE TABLE [common].[ProcessState] (
    [UniqueName] VARCHAR (255)  NOT NULL,
    [Settings]   NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_ProcessState] PRIMARY KEY CLUSTERED ([UniqueName] ASC)
);



