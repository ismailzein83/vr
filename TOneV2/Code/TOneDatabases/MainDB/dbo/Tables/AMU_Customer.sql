CREATE TABLE [dbo].[AMU_Customer] (
    [ID]               INT         IDENTITY (1, 1) NOT NULL,
    [CarrierAccountID] VARCHAR (5) NULL,
    [MctID]            INT         NULL,
    [AmuID]            INT         NULL,
    [AssignDate]       DATE        NULL,
    CONSTRAINT [PK_AMU_Customer] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_MCT_AMU_Customer] FOREIGN KEY ([MctID]) REFERENCES [dbo].[MCT] ([ID]) ON DELETE SET NULL
);

