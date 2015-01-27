CREATE TABLE [dbo].[RoutingImage] (
    [telephone_number] VARCHAR (50)  NOT NULL,
    [priority]         SMALLINT      NOT NULL,
    [route_type]       INT           NOT NULL,
    [tech_prefix]      VARCHAR (255) NOT NULL,
    [id_route]         INT           NOT NULL,
    [call_type]        INT           NOT NULL,
    [type]             INT           NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [Routing_Image_AllFileds]
    ON [dbo].[RoutingImage]([telephone_number] ASC, [route_type] ASC, [id_route] ASC) WITH (ALLOW_PAGE_LOCKS = OFF);

