USE [ElectionDB]
GO

/****** Object:  View [dbo].[DistributionWithinStates]    Script Date: 13.11.2015 14:26:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER View [dbo].[DistributionWithinStates] as
--stored as "MinSeatsForPartyPerState.sql

--output max(seats gained by zweitstimme, seats gained by erststimme


select distinct	z.Election_Id as Election_Id, z.Bundesland_Id as Bundesland_Id,
			case when z.Party_Id=e.Party_Id then  (Select Max(y) from (values(e.NumberOfVictories), (z.seats)) as x(y) ) 
			when z.seats>0  then z.seats
			when e.numberOfVictories>0 then coalesce(e.numberOfVictories,0)
			End
			 as Seats, 
		case when z.Party_Id=e.Party_Id then z.Party_Id
			when z.seats>0 then z.Party_Id
			else e.Party_Id
		end as Party_Id 
 from seatsGainedByZweitstimme z, SeatsGainedByErststimme e
 where e.Election_Id=z.Election_Id 
 and e.Bundesland_Id=z.Bundesland_Id


/*
select a.Election_Id as Election_Id, a.Bundesland_Id as Bundesland_Id, a.Party_Id as Party_Id, +--maximum of (votes.seats),wk.numberOfVictories that is computed in line below
	b.seats as ZweitstimmenSitze, a.numberOfVictories as Wahlkreis_Siege
	--(Select max(seats) 
from SeatsGainedByErststimme a, seatsGainedByZweitstimme b
where a.Election_Id = b.Election_Id 
and a.Bundesland_Id= b.Bundesland_Id
 and a.Party_Id=b.Party_Id
 
 --group by a.Election_Id, a.Bundesland_Id--, a.Party_Id, b.seats, a.numberOfVictories

 */






/***************************************************************************
--necessary temporary data
with Divisor as ... 

seatsGainedByZweitstimme as (

select d.Election_ID as Election_Id, d.Country as Bundesland_Id, p.Id as Party_Id,  
 round( coalesce(votes.amount,0) / d.Divisor, 0)	as seats

from , Parties5 p, IsElectableParty electable, ZweitstimmenState votes
where d.Election_ID=p.Election_Id and d.Election_Id = electable.Election_Id
and electable.Party_Id=p.Id and p.Id=electable.Party_Id 
and d.Country= electable.Bundesland_Id
 --for every party >5% which is electable in a country
 and votes.Election_id=d.Election_ID and votes.Party_Id=p.Id and votes.Bundesland_Id=d.Country
 --votes for the party in that year in that state
 ),

 SeatsByWahlkreissieger as (
 select wks.ElectionId, wk.Bundesland_Id, wks.MemberOfParty as Party_Id, count(wks.Person_Id) as numberOfVictories
from Wahlkreissieger wks, Wahlkreis wk
where wks.Wahlkreis_Id= wk.Id
group by wks.MemberOfParty,wks.ElectionId, wk.Bundesland_Id)


select votes.Election_Id as Election_Id, votes.Bundesland_Id as Bundesland_Id, votes.Party_Id as Party_Id, +--maximum of (votes.seats),wk.numberOfVictories that is computed in line below
	(Select max(seats) 
	from (values(votes.seats,wk.numberOfVictories)) 
	as MinSeats ) as MinSeats
from SeatsByWahlkreissieger wk, seatsGainedByZweitstimme votes
where wk.Bundesland_Id=votes.Bundesland_Id and wk.Party_Id=votes.Party_Id
*/




GO


