alter View ParliamentMembersDashboard as


with candidatesTemp(Election_Id, Party_Id, Bundesland_Id, Person_Id, Position) as 
	(select cand.Election_Id, p.Party_Id, cand.Bundesland_Id,cand.Person_Id,  rank() over (partition by cand.Election_Id, Bundesland_Id, p.Party_Id order by cand.Position) as Rang
	from CandidateList cand, PartyAffiliation p
	where p.Election_Id = cand.Election_Id
	and p.Person_Id = cand.Person_Id 
	and cand.Person_Id  not in (select wks.Person_Id
								from Wahlkreissieger wks
								where wks.ElectionID= cand.Person_Id)
), 

SeatsPerList(Election_Id, Person_Id, Bundesland_Id, Party_Id) as (
select c.Election_Id,  c.Person_Id, c.Bundesland_Id, c.Party_Id
from candidatesTemp c, FinalPartySeatsState pss
where 
--join
c.Election_Id = pss.Election_Id
and c.Bundesland_Id = pss.Bundesland_Id
and c.Party_Id = pss.Party_ID
--logic
and c.Position <= pss.AmountListSeats
),

UnionView (Election_Id, HowCome, Person_Id, Party_Id, Bundesland_Id ) as (

select ElectionID, 'Direktmandat',  Person_Id, wks.MemberOfParty, w.Bundesland_Id
from Wahlkreissieger wks, Wahlkreis w
where wks.Wahlkreis_Id=w.Id
union 
select Election_Id, 'Listenplatz', Person_Id, Party_Id, Bundesland_Id
from SeatsPerList )



select  uv.Election_Id, coalesce (p.Title,'') + ' ' + Firstname + ' '+ p.Lastname, uv.HowCome, b.Name
from UnionView uv, Person p, Bundesland b, Party par
where p.Id=uv.Person_Id
and b.Id=uv.Bundesland_Id
--fehlt noch: Party, da mapping mit parteilosen Direktkandidaten schwierig. nachschaun!




