CREATE TABLE [sec].[OrgChartLinkedEntity] (
    [OrgChartID]             INT           NOT NULL,
    [LinkedEntityIdentifier] VARCHAR (850) NOT NULL,
    CONSTRAINT [PK_OrgChartLinkedEntity] PRIMARY KEY CLUSTERED ([OrgChartID] ASC, [LinkedEntityIdentifier] ASC),
    CONSTRAINT [FK_OrgChartLinkedEntity_OrgChart] FOREIGN KEY ([OrgChartID]) REFERENCES [sec].[OrgChart] ([Id])
);

