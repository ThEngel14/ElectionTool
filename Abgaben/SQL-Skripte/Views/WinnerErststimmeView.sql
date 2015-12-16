Alter View WinnerErststimme as

With ExtendedErststimmeAggregated (Election_Id, Wahlkreis_Id, Person_Id, RankAmount) as (
		select Election_Id, Wahlkreis_Id, Person_Id, rank() over (partition by election_id, wahlkreis_id order by amount desc)
		from ErststimmeAggregated
)

select e.Election_Id,
       e.Wahlkreis_Id, k.Name as Wahlkreis_Name, 
       e.Person_Id, p.Title, p.Firstname, p.Lastname, 
       pa.Id as Party_Id, pa.Name as Party_Name
from ExtendedErststimmeAggregated e left join Person p on e.Person_Id = p.Id
			  join PartyAffiliation af on p.Id = af.Person_Id and e.Election_Id = af.Election_Id
			  join Party pa on af.Party_Id = pa.Id
			  join Wahlkreis k on e.Wahlkreis_Id = k.Id
where e.RankAmount = 1