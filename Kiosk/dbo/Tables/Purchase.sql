CREATE TABLE [dbo].[Purchase] (
    [ID]         INT      IDENTITY (1, 1) NOT NULL,
    [Date]       DATETIME NOT NULL,
    [ProductID]  INT      NOT NULL,
    [StoreID]    INT      NOT NULL,
    [CustomerID] INT      NOT NULL,
    CONSTRAINT [PK_Purchase] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Purchase_Product] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product] ([ID]),
    CONSTRAINT [FK_Purchase_SCustomer] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID]),
    CONSTRAINT [FK_Purchase_Store] FOREIGN KEY ([StoreID]) REFERENCES [dbo].[Store] ([ID])
);

