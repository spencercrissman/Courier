
/****** Object:  Table [dbo].[companies]    Script Date: 05/24/2011 09:40:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[companies](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](150) NOT NULL,
	[category] [nvarchar](150) NOT NULL,
	[symbol] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_companies] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('ACC Ltd.','CEMENT AND CEMENT PRODUCTS','ACC');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Ambuja Cements Ltd.','CEMENT AND CEMENT PRODUCTS','AMBUJACEM');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Axis Bank Ltd.','BANKS','AXISBANK');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Bajaj Auto Ltd.','AUTOMOBILES - 2 AND 3 WHEELERS','BAJAJ-AUTO');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Bharat Heavy Electricals Ltd.','ELECTRICAL EQUIPMENT','BHEL');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Bharat Petroleum Corporation Ltd.','REFINERIES','BPCL');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Bharti Airtel Ltd.','TELECOMMUNICATION - SERVICES','BHARTIARTL');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Cairn India Ltd.','OIL EXPLORATION/PRODUCTION','CAIRN');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Cipla Ltd.','PHARMACEUTICALS','CIPLA');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('DLF Ltd.','CONSTRUCTION','DLF');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Dr. Reddys Laboratories Ltd.','PHARMACEUTICALS','DRREDDY');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('GAIL (India) Ltd.','GAS','GAIL');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('HCL Technologies Ltd.','COMPUTERS - SOFTWARE','HCLTECH');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('HDFC Bank Ltd.','BANKS','HDFCBANK');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Hero Honda Motors Ltd.','AUTOMOBILES - 2 AND 3 WHEELERS','HEROHONDA');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Hindalco Industries Ltd.','ALUMINIUM','HINDALCO');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Hindustan Unilever Ltd.','DIVERSIFIED','HINDUNILVR');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Housing Development Finance Corporation Ltd.','FINANCE - HOUSING','HDFC');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('I T C Ltd.','CIGARETTES','ITC');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('ICICI Bank Ltd.','BANKS','ICICIBANK');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Infosys Technologies Ltd.','COMPUTERS - SOFTWARE','INFOSYSTCH');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Infrastructure Development Finance Co. Ltd.','FINANCIAL INSTITUTION','IDFC');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Jaiprakash Associates Ltd.','DIVERSIFIED','JPASSOCIAT');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Jindal Steel & Power Ltd.','STEEL AND STEEL PRODUCTS','JINDALSTEL');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Kotak Mahindra Bank Ltd.','BANKS','KOTAKBANK');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Larsen & Toubro Ltd.','ENGINEERING','LT');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Mahindra & Mahindra Ltd.','AUTOMOBILES - 4 WHEELERS','M&M');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Maruti Suzuki India Ltd.','AUTOMOBILES - 4 WHEELERS','MARUTI');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('NTPC Ltd.','POWER','NTPC');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Oil & Natural Gas Corporation Ltd.','OIL EXPLORATION/PRODUCTION','ONGC');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Power Grid Corporation of India Ltd.','POWER','POWERGRID');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Punjab National Bank','BANKS','PNB');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Ranbaxy Laboratories Ltd.','PHARMACEUTICALS','RANBAXY');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Reliance Capital Ltd.','FINANCE','RELCAPITAL');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Reliance Communications Ltd.','TELECOMMUNICATION - SERVICES','RCOM');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Reliance Industries Ltd.','REFINERIES','RELIANCE');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Reliance Infrastructure Ltd.','POWER','RELINFRA');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Reliance Power Ltd.','POWER','RPOWER');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Sesa Goa Ltd.','MINING','SESAGOA');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Siemens Ltd.','ELECTRICAL EQUIPMENT','SIEMENS');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('State Bank of India','BANKS','SBIN');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Steel Authority of India Ltd.','STEEL AND STEEL PRODUCTS','SAIL');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Sterlite Industries (India) Ltd.','METALS','STER');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Sun Pharmaceutical Industries Ltd.','PHARMACEUTICALS','SUNPHARMA');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Suzlon Energy Ltd.','ELECTRICAL EQUIPMENT','SUZLON');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Tata Consultancy Services Ltd.','COMPUTERS - SOFTWARE','TCS');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Tata Motors Ltd.','AUTOMOBILES - 4 WHEELERS','TATAMOTORS');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Tata Power Co. Ltd.','POWER','TATAPOWER');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Tata Steel Ltd.','STEEL AND STEEL PRODUCTS','TATASTEEL');
INSERT INTO Companies ([Name],[Category],[Symbol]) VALUES('Wipro Ltd.','COMPUTERS - SOFTWARE','WIPRO');
