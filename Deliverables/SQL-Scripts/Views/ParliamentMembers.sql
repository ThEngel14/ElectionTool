USE [ElectionDB]
GO

/****** Object:  View [dbo].[ParliamentMembers]    Script Date: 03.01.2016 13:37:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO








ALTER View [dbo].[ParliamentMembers] (Election_Id, Person_Id, Title, Firstname, Lastname, HowCome, Party_Id, Party_Name, Bundesland_Id, Bundesland_Name, Wahlkreis_Id, Wahlkreis_Name) as 


with SeatsGainedByZweitstimme1 as  (
/*clone from SeatsGainedByZweitstimme-View. variying DivisorFunction**/

	select votes.Election_Id as Election_Id, 
		votes.Bundesland_Id as Bundesland_Id,
		 votes.party_Id as Party_Id, 
		cast(round(1.0*votes.Amount/dbo.divisorParty(votes.Election_Id, votes.Party_ID, s.SeatsParty),0)as int)  as seats
	from  ZweitstimmenState votes, Sitzverteilung s 
	where s.Election_Id=votes.Election_Id 
		and s.Party_Id = votes.Party_ID
		and votes.Party_ID in (select p.Id from Parties5 p
								where p.Election_Id=votes.Election_ID)),

SeatsGainedByErststimme1 as (
	select * from SeatsGainedbyErststimme

	union
	
	--Parties, who did not gain seats by erststimme with numberOfVictories 0
	select p5.Election_Id as Election_Id, b.Id as Bundesland_Id, p5.Id as Party_Id, 0 as numberOfVictories
	from Parties5 p5, Bundesland b
	where p5.Id not in (select Party_Id from SeatsGainedByErststimme winners 
						where p5.Election_Id=winners.Election_Id 
						and winners.Bundesland_Id=b.Id)),


fillUp(Number, Election_Id, Bundesland_Id, Party_Id) as (
		select
			 iif(z.seats> e.numberOfVictories, z.seats-e.numberOfVictories, 0),
			 z.Election_Id, z.Bundesland_Id, z.Party_Id
		from SeatsGainedByZweitstimme1 z, SeatsGainedByErststimme1 e
		where z.Election_Id = e.Election_Id
		and z.Bundesland_Id = e.Bundesland_Id
		and z.Party_Id = e.Party_Id),


--temp view for all candidates for each party in each state, where candidate not a firstvote winner
CandidateTemp(Election_Id, Bundesland_Id, Party_Id, Person_Id, Rang) as (
	Select cand.Election_Id,
		cand.Bundesland_Id, 
		partyAff.Party_Id, 
		partyAff.Person_Id, 
		rank() over (partition by cand.Election_Id, cand.Bundesland_Id, partyAff.Party_Id order by cand.Position) as Rang
from CandidateList cand, PartyAffiliation partyAff
where cand.Election_Id = partyAff.Election_Id
	and cand.Person_Id = partyAff.Person_Id
	and partyAff.Person_Id not in (select wks.Person_Id 
									from Wahlkreissieger wks 
									where wks.ElectionID = partyAff.Election_Id)

)



--alle firstvote winners

select wks.ElectionID as Election_Id, 
	   pers.Id as Person_Id,
	   pers.Title,
		pers.Firstname,
		pers.Lastname,
       'Erststimme' as HowCome, 
	   p.Id as Party_Id,
	   p.Name as Party_Name,
	   b.Id as Bundesland_Id,
	   b.Name as Bundesland_Name,
	   wk.Id as Wahlkreis_Id,
	   wk.Name as Wahlkreis_Name
 from Wahlkreissieger wks, Person pers, Party p, Wahlkreis wk, Bundesland b
where pers.Id=wks.Person_Id
	and wks.MemberOfParty=p.Id
	and wks.Wahlkreis_Id = wk.Id
	and wk.Bundesland_Id= b.Id

union

--|Seats for a Party in a state| - |seats not available anymore, because already demanded by bistrict winner|
  

Select  candidates.Election_Id as Election_Id, 
        pers.Id as Person_Id, pers.Title, pers.Firstname, pers.Lastname, 
	    'Listenplatz' as HowCome, 
		part.Id as Party_Id, part.Name as Party_Name,
		b.Id as Bundesland_Id, b.Name as Bundesland_Name,
		null as Wahlkreis_Id, null as Wahlkreis_Name
	from fillUp fu, CandidateTemp candidates, Person pers, Bundesland b, Party part
	where --join
		fu.Election_Id = candidates.Election_Id
	and fu.Bundesland_Id = candidates.Bundesland_Id
	and fu.Party_Id = candidates.Party_Id
	--join for select information 
	and pers.Id= candidates.Person_Id
	and part.Id= candidates.Party_Id
	and b.Id = candidates.Bundesland_Id
	--logic
	and candidates.Rang <= fu.Number




GO


