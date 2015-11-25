USE [ElectionDB]
GO

/****** Object:  View [dbo].[minSeatsPerParty_Bundeslevel]    Script Date: 13.11.2015 15:08:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER View [dbo].[minSeatsPerParty_Bundeslevel] as

select Election_Id, Party_Id, sum(Seats) as minSeatsBund
from DistributionWithinStates 
group by Election_Id,Party_Id



GO


