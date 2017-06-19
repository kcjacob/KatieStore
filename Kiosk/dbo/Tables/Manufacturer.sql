CREATE TABLE [dbo].[Manufacturer] (
    [Name]      NVARCHAR (100) NOT NULL,
    [AddressID] INT            NULL,
    CONSTRAINT [PK_Manufacturer] PRIMARY KEY CLUSTERED ([Name] ASC),
    CONSTRAINT [FK_Manufacturer_Address] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([ID])
);

