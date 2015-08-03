CREATE TABLE [dbo].[AMU] (
    [ID]            INT          IDENTITY (1, 1) NOT NULL,
    [UserID]        INT          NOT NULL,
    [MctID]         INT          NULL,
    [ParentID]      INT          NULL,
    [Flag]          VARCHAR (20) NULL,
    [Level]         TINYINT      NULL,
    [NeedsApprove]  BIT          NULL,
    [ViewCustomers] TINYINT      NULL,
    [ViewSuppliers] TINYINT      NULL,
    [ApplyToNOC]    BIT          NULL,
    CONSTRAINT [PK_AMU] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_MCT_AMU] FOREIGN KEY ([MctID]) REFERENCES [dbo].[MCT] ([ID]) ON DELETE SET NULL
);

