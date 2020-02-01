USE [master]
GO
/****** Object:  Database [krupagallarydb]    Script Date: 2/1/2020 11:22:12 PM ******/
CREATE DATABASE [krupagallarydb]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'krupagallarydb', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER12\MSSQL\DATA\krupagallarydb.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'krupagallarydb_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER12\MSSQL\DATA\krupagallarydb_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [krupagallarydb] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [krupagallarydb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [krupagallarydb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [krupagallarydb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [krupagallarydb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [krupagallarydb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [krupagallarydb] SET ARITHABORT OFF 
GO
ALTER DATABASE [krupagallarydb] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [krupagallarydb] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [krupagallarydb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [krupagallarydb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [krupagallarydb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [krupagallarydb] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [krupagallarydb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [krupagallarydb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [krupagallarydb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [krupagallarydb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [krupagallarydb] SET  DISABLE_BROKER 
GO
ALTER DATABASE [krupagallarydb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [krupagallarydb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [krupagallarydb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [krupagallarydb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [krupagallarydb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [krupagallarydb] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [krupagallarydb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [krupagallarydb] SET RECOVERY FULL 
GO
ALTER DATABASE [krupagallarydb] SET  MULTI_USER 
GO
ALTER DATABASE [krupagallarydb] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [krupagallarydb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [krupagallarydb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [krupagallarydb] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'krupagallarydb', N'ON'
GO
USE [krupagallarydb]
GO
/****** Object:  Table [dbo].[Tbl_AdminRoles]    Script Date: 2/1/2020 11:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_AdminRoles](
	[PK_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](150) NULL,
	[IsActive] [bit] NULL,
	[IsDelete] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_AdminRole] PRIMARY KEY CLUSTERED 
(
	[PK_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tbl_AdminUsers]    Script Date: 2/1/2020 11:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_AdminUsers](
	[Pk_AdminUserId] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](350) NULL,
	[LastName] [nvarchar](250) NULL,
	[UserName] [nvarchar](150) NULL,
	[Email] [nvarchar](350) NULL,
	[Password] [nvarchar](150) NULL,
	[MobileNo] [nvarchar](20) NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_AdminUser] PRIMARY KEY CLUSTERED 
(
	[Pk_AdminUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tbl_Categories]    Script Date: 2/1/2020 11:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_Categories](
	[PK_CategoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](450) NULL,
	[CategoryImage] [nvarchar](450) NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
	[IsActive] [bit] NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_Tbl_Category] PRIMARY KEY CLUSTERED 
(
	[PK_CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tbl_ClientOtherDetails]    Script Date: 2/1/2020 11:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_ClientOtherDetails](
	[Pk_ClientOtherDetailsId] [bigint] IDENTITY(1,1) NOT NULL,
	[ClientUserId] [bigint] NULL,
	[Addharcardno] [nvarchar](50) NULL,
	[Pancardno] [nvarchar](50) NULL,
	[GSTno] [nvarchar](50) NULL,
	[Photo] [nvarchar](250) NULL,
	[City] [nvarchar](150) NULL,
	[State] [nvarchar](150) NULL,
	[Address] [nvarchar](250) NULL,
	[ShipCity] [nvarchar](150) NULL,
	[ShipState] [nvarchar](100) NULL,
	[ShipAddress] [nvarchar](250) NULL,
	[CreditAmt] [numeric](18, 2) NULL,
	[AmountDue] [numeric](18, 2) NULL,
	[IsActive] [bit] NULL,
	[IsDelete] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_ClientOtherDetails] PRIMARY KEY CLUSTERED 
(
	[Pk_ClientOtherDetailsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tbl_ClientRoles]    Script Date: 2/1/2020 11:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_ClientRoles](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](150) NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_ClientRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tbl_ClientUsers]    Script Date: 2/1/2020 11:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_ClientUsers](
	[Pk_ClientUserId] [bigint] NOT NULL,
	[FirstName] [nvarchar](250) NULL,
	[LastName] [nvarchar](250) NULL,
	[Email] [nvarchar](150) NULL,
	[MobileNo] [nvarchar](50) NULL,
	[UserName] [nvarchar](50) NULL,
	[Password] [nvarchar](150) NULL,
	[ClientRoleId] [bigint] NULL,
	[CompanyName] [nvarchar](150) NULL,
	[IsActive] [bit] NULL,
	[IsDelete] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_ClientUsers] PRIMARY KEY CLUSTERED 
(
	[Pk_ClientUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tbl_ItemStocks]    Script Date: 2/1/2020 11:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_ItemStocks](
	[Pk_StockId] [bigint] IDENTITY(1,1) NOT NULL,
	[Fk_ProductId] [bigint] NULL,
	[Fk_CategoryId] [bigint] NULL,
	[Fk_SubProductId] [bigint] NULL,
	[Fk_ProductItemId] [bigint] NULL,
	[Qty] [bigint] NULL,
	[IsActive] [bit] NULL,
	[IsDelete] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_ItemStocks] PRIMARY KEY CLUSTERED 
(
	[Pk_StockId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tbl_Offers]    Script Date: 2/1/2020 11:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_Offers](
	[Pk_Offer_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[OfferName] [nvarchar](350) NULL,
	[Fk_ProductId] [bigint] NULL,
	[Fk_CategoryId] [bigint] NULL,
	[Fk_SubproductId] [bigint] NULL,
	[Fk_ProductItemId] [bigint] NULL,
	[OfferPrice] [numeric](18, 2) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[IsActive] [bit] NULL,
	[IsDelete] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_Offers] PRIMARY KEY CLUSTERED 
(
	[Pk_Offer_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tbl_OrderItemDetails]    Script Date: 2/1/2020 11:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_OrderItemDetails](
	[Pk_OrderDetail_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Fk_OrderId] [bigint] NULL,
	[Fk_ProductItemId] [bigint] NULL,
	[Qty] [bigint] NULL,
	[Price] [numeric](18, 2) NULL,
	[ItemName] [nvarchar](250) NULL,
	[Sku] [nvarchar](50) NULL,
	[GSTAmt] [numeric](18, 2) NULL,
	[IGSTAmt] [numeric](18, 2) NULL,
	[IsActive] [bit] NULL,
	[IsDelete] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_OrderItemDetails] PRIMARY KEY CLUSTERED 
(
	[Pk_OrderDetail_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tbl_Orders]    Script Date: 2/1/2020 11:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_Orders](
	[Pk_Order_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Fk_ClientUserId] [bigint] NULL,
	[OrderAmount] [decimal](18, 2) NULL,
	[OrderShipCity] [nvarchar](100) NULL,
	[OrderShipState] [nvarchar](100) NULL,
	[OrderShipAddress] [nvarchar](250) NOT NULL,
	[OrderShipPincode] [nvarchar](10) NULL,
	[OrderShipClientName] [nvarchar](150) NULL,
	[OrderShipClientPhone] [nvarchar](50) NULL,
	[OrderStatusId] [bigint] NULL,
	[PaymentType] [nvarchar](150) NULL,
	[IsActive] [bit] NULL,
	[IsDelete] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_Orders] PRIMARY KEY CLUSTERED 
(
	[Pk_Order_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tbl_ProductItemImages]    Script Date: 2/1/2020 11:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_ProductItemImages](
	[Pk_ItemImage_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Fk_ProductItem_Id] [bigint] NULL,
	[ItemImage] [nvarchar](450) NULL,
	[IsActive] [bit] NULL,
	[IsDelete] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_ProductItemImages] PRIMARY KEY CLUSTERED 
(
	[Pk_ItemImage_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tbl_ProductItems]    Script Date: 2/1/2020 11:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_ProductItems](
	[Pk_ProductItem_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Fk_SubProductId] [bigint] NULL,
	[Fk_ProductId] [bigint] NULL,
	[Fk_CategoryId] [bigint] NULL,
	[ProductItemName] [nvarchar](650) NULL,
	[MRPPrice] [decimal](18, 2) NULL,
	[DistributorPrice] [decimal](18, 2) NULL,
	[SalePrice] [decimal](18, 2) NULL,
	[GST] [decimal](18, 2) NULL,
	[IGST] [decimal](18, 2) NULL,
	[Cess] [decimal](18, 2) NULL,
	[ItemDescription] [nvarchar](max) NULL,
	[MainImage] [nvarchar](350) NULL,
	[Notification] [nvarchar](max) NULL,
	[Sku] [nvarchar](50) NULL,
	[IsActive] [bit] NULL,
	[IsDelete] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_ProductItems] PRIMARY KEY CLUSTERED 
(
	[Pk_ProductItem_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tbl_Products]    Script Date: 2/1/2020 11:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_Products](
	[PK_Product_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ProductName] [nvarchar](450) NULL,
	[Fk_CategoryId] [bigint] NULL,
	[ProductImage] [nvarchar](450) NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
	[IsActive] [bit] NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_Tbl_Product] PRIMARY KEY CLUSTERED 
(
	[PK_Product_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tbl_SubProducts]    Script Date: 2/1/2020 11:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_SubProducts](
	[Pk_SubProduct_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[SubProductName] [nvarchar](450) NULL,
	[Fk_ProductId] [bigint] NULL,
	[Fk_CategoryId] [bigint] NULL,
	[SubProductImage] [nvarchar](350) NULL,
	[IsActive] [bit] NULL,
	[IsDelete] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_SubProduct] PRIMARY KEY CLUSTERED 
(
	[Pk_SubProduct_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Tbl_AdminUsers] ON 

INSERT [dbo].[Tbl_AdminUsers] ([Pk_AdminUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'krupagallaryadmin', NULL, N'admin', NULL, N'1234', NULL, NULL, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Tbl_AdminUsers] OFF
SET IDENTITY_INSERT [dbo].[Tbl_Categories] ON 

INSERT [dbo].[Tbl_Categories] ([PK_CategoryId], [CategoryName], [CategoryImage], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [IsDelete]) VALUES (1, N'CategoryName1', N'cat1.png', 1, NULL, NULL, NULL, 1, 0)
INSERT [dbo].[Tbl_Categories] ([PK_CategoryId], [CategoryName], [CategoryImage], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [IsDelete]) VALUES (2, N'CategoryName2', N'cat2.png', 1, NULL, NULL, NULL, 1, 0)
SET IDENTITY_INSERT [dbo].[Tbl_Categories] OFF
SET IDENTITY_INSERT [dbo].[Tbl_ProductItems] ON 

INSERT [dbo].[Tbl_ProductItems] ([Pk_ProductItem_Id], [Fk_SubProductId], [Fk_ProductId], [Fk_CategoryId], [ProductItemName], [MRPPrice], [DistributorPrice], [SalePrice], [GST], [IGST], [Cess], [ItemDescription], [MainImage], [Notification], [Sku], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, 1, 1, 1, N'Item1', CAST(150.00 AS Decimal(18, 2)), CAST(130.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, N'itm123', 1, 0, 1, NULL, 1, NULL)
INSERT [dbo].[Tbl_ProductItems] ([Pk_ProductItem_Id], [Fk_SubProductId], [Fk_ProductId], [Fk_CategoryId], [ProductItemName], [MRPPrice], [DistributorPrice], [SalePrice], [GST], [IGST], [Cess], [ItemDescription], [MainImage], [Notification], [Sku], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, 1, 1, 1, N'Item2', CAST(220.00 AS Decimal(18, 2)), CAST(200.00 AS Decimal(18, 2)), CAST(210.00 AS Decimal(18, 2)), CAST(8.00 AS Decimal(18, 2)), CAST(8.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, N'item888', 1, 0, 1, NULL, 1, NULL)
INSERT [dbo].[Tbl_ProductItems] ([Pk_ProductItem_Id], [Fk_SubProductId], [Fk_ProductId], [Fk_CategoryId], [ProductItemName], [MRPPrice], [DistributorPrice], [SalePrice], [GST], [IGST], [Cess], [ItemDescription], [MainImage], [Notification], [Sku], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, 3, 2, 1, N'Item3', CAST(500.00 AS Decimal(18, 2)), CAST(450.00 AS Decimal(18, 2)), CAST(470.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, N'item787', 1, 0, 1, NULL, 1, NULL)
INSERT [dbo].[Tbl_ProductItems] ([Pk_ProductItem_Id], [Fk_SubProductId], [Fk_ProductId], [Fk_CategoryId], [ProductItemName], [MRPPrice], [DistributorPrice], [SalePrice], [GST], [IGST], [Cess], [ItemDescription], [MainImage], [Notification], [Sku], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (4, 0, 3, 2, N'Item4', CAST(555.00 AS Decimal(18, 2)), CAST(544.00 AS Decimal(18, 2)), CAST(550.00 AS Decimal(18, 2)), CAST(6.00 AS Decimal(18, 2)), CAST(6.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, N'T444', 1, 0, 1, NULL, 1, NULL)
SET IDENTITY_INSERT [dbo].[Tbl_ProductItems] OFF
SET IDENTITY_INSERT [dbo].[Tbl_Products] ON 

INSERT [dbo].[Tbl_Products] ([PK_Product_Id], [ProductName], [Fk_CategoryId], [ProductImage], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [IsDelete]) VALUES (1, N'Product1', 1, N'p1.png', 1, NULL, 1, NULL, 1, 0)
INSERT [dbo].[Tbl_Products] ([PK_Product_Id], [ProductName], [Fk_CategoryId], [ProductImage], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [IsDelete]) VALUES (2, N'Product2', 1, N'p2.png', 1, NULL, 1, NULL, 1, 0)
INSERT [dbo].[Tbl_Products] ([PK_Product_Id], [ProductName], [Fk_CategoryId], [ProductImage], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsActive], [IsDelete]) VALUES (3, N'Product3', 2, N'p3.png', 1, NULL, 1, NULL, 1, 0)
SET IDENTITY_INSERT [dbo].[Tbl_Products] OFF
SET IDENTITY_INSERT [dbo].[Tbl_SubProducts] ON 

INSERT [dbo].[Tbl_SubProducts] ([Pk_SubProduct_Id], [SubProductName], [Fk_ProductId], [Fk_CategoryId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'SubProduct1', 1, 1, N'sp1.png', 1, 0, 1, NULL, 1, NULL)
INSERT [dbo].[Tbl_SubProducts] ([Pk_SubProduct_Id], [SubProductName], [Fk_ProductId], [Fk_CategoryId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'SubProduct2', 1, 1, N'sp2.png', 1, 0, 1, NULL, 1, NULL)
INSERT [dbo].[Tbl_SubProducts] ([Pk_SubProduct_Id], [SubProductName], [Fk_ProductId], [Fk_CategoryId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, N'SubProduct3', 2, 1, N'sp3.png', 1, 0, 1, NULL, 1, NULL)
SET IDENTITY_INSERT [dbo].[Tbl_SubProducts] OFF
USE [master]
GO
ALTER DATABASE [krupagallarydb] SET  READ_WRITE 
GO
