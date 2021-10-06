CREATE TABLE [dbo].[Products]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Product] NCHAR(40) NOT NULL, 
    [Calories] INT NOT NULL, 
    [Protein] INT NOT NULL, 
    [Carbs] INT NOT NULL, 
    [Fat] INT NOT NULL
)
