USE [ElectionDB]
GO

/****** Object:  View [dbo].[DistributionWithinStates]    Script Date: 26.11.2015 14:58:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO







alter View [dbo].[DistributionWithinStates] as
--stored as "MinSeatsForPartyPerState.sql

--output max(seats gained by zweitstimme, seats gained by erststimme


select distinct	z.Election_Id as Election_Id, z.Bundesland_Id as Bundesland_Id,
					 iif(e.NumberOfVictories > z.Seats, e.NumberOfVictories, z.seats ) as Seats ,
					z.Party_Id as Party_Id
 from seatsGainedByZweitstimme z, SeatsGainedByErststimme e
 where e.Election_Id=z.Election_Id 
 and e.Bundesland_Id=z.Bundesland_Id
 and e.Party_Id = z.Party_Id










GO


