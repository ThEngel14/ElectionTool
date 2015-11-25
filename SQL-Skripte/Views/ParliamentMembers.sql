USE [ElectionDB]
GO

/****** Object:  View [dbo].[ParliamentMembers]    Script Date: 16.11.2015 13:21:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER View [dbo].[ParliamentMembers] (Election_Id, FirstName, LastName, HowCome, State, Party) as 


with SeatsGainedByZweitstimme1 as  (
/*clone from SeatsGainedByZweitstimme-View. variying DivisorFunction**/

select votes.Election_Id as Election_Id, votes.Bundesland_Id as Bundesland_Id, votes.party_Id as Party_Id, 
	round(1.0*votes.Amount/dbo.divisorParty(votes.Election_Id, votes.Party_ID),0)  as seats
 from  ZweitstimmenState votes 
where votes.Party_ID in (select p.Id from Parties5 p
						where p.Election_Id=votes.Election_ID)),


fillUp(Number, Election_Id, Bundesland_Id, Party_Id) as (
		select
			 case when (z.seats - e.numberOfVictories)<1 then 0 
			else z.seats-e.numberOfVictories
			end,
			z.Election_Id, z.Bundesland_Id, z.Party_Id
		from SeatsGainedByZweitstimme1 z, SeatsGainedByErststimme e
		where z.Election_Id = e.Election_Id
		and z.Bundesland_Id = e.Bundesland_Id
		and z.Party_Id = e.Party_Id),


		--temp view for all candidates for each party in each state, where candidate not a wahlkreissieger
CandidateTemp(Election_Id, Bundesland_Id, Party_Id, Person_Id, Rang) as (
Select cand.Election_Id, cand.Bundesland_Id, partyAff.Party_Id, partyAff.Person_Id, rank() over (
												partition by cand.Election_Id, cand.Bundesland_Id, partyAff.Party_Id order by cand.Position) as Rang
from CandidateList cand, PartyAffiliation partyAff
where cand.Election_Id = partyAff.Election_Id
and cand.Person_Id = partyAff.Person_Id
and partyAff.Person_Id not in (select wks.Person_Id from Wahlkreissieger wks where wks.ElectionID = partyAff.Election_Id)

)



--alle Wahlkreissieger
select wks.ElectionID as ELection_Id, pers.Firstname as FirstName, pers.Lastname as LastName,
 'Erststimme' as HowCome, b.Name as State, coalesce(p.Name,'Parteilos') as Party 
from Wahlkreissieger wks, Person pers, Party p, Wahlkreis wk, Bundesland b
where pers.Id=wks.Person_Id
and wks.MemberOfParty=p.Id
and wks.Wahlkreis_Id = wk.Id
and wk.Bundesland_Id= b.Id

union

--|Seats for a Party in a state| - |seats not available anymore, because already demanded by bistrict winner|


/*
	select fu.Election_Id as Election_Id,Vorname as FirstName, Nachname as Lastname, HowCome, BName as state, PName as Party  
	from  fillUp fu cross apply LoopInserterFunction(fu.Election_Id, fu.Bundesland_Id, fu.Party_Id, fu.Number)
	*/

	Select  candidates.Election_Id as Election_Id, pers.Firstname as FirstName, pers.Lastname as LastName, 
		'Listenplatz' as HowCome, b.Name as State, part.Name as Party 
	from fillUp fu, CandidateTemp candidates, Person pers, Bundesland b, Party part
	where fu.Election_Id = candidates.Election_Id
	and fu.Bundesland_Id = candidates.Bundesland_Id
	and fu.Party_Id = candidates.Party_Id 
	and pers.Id= candidates.Person_Id
	and part.Id= candidates.Party_Id
	and b.Id = candidates.Bundesland_Id
	and candidates.Rang <= fu.Number

--Number-mal
	/*Select fu.Election_Id, 'Person mit Listenplatz n', 'Nachname Person', 'Parteizugehoerigkeit', b.Name,p.Name 
	from fillUp fu, Bundesland b, Party p
	where b.Id=fu.Bundesland_Id
	and p.Id= fu.Party_Id
	*/



GO


