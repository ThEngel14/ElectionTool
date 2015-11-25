Alter View ErststimmeWahlkreisOverview as

With OverallVotes (Election_Id, Wahlkreis_Id, Overall) as (
		select election_id, wahlkreis_id, sum(amount)
		from ErststimmeAggregated
		group by election_id, wahlkreis_id),
	 Overview as (
		select e.election_id,
			   e.wahlkreis_id, k.name as Wahlkreis_Name,
			   e.person_id, p.title, p.firstname, p.lastname,
			   pa.id as Party_Id, pa.name as Party_Name,
			   e.amount as Votes, o.overall, 1.0*e.amount / o.overall as PercentVotes
		from ErststimmeAggregated e join OverallVotes o on e.Election_Id = o.Election_Id and e.Wahlkreis_Id = o.Wahlkreis_Id
								join Wahlkreis k on e.Wahlkreis_Id = k.Id
								left join Person p on e.Person_Id = p.Id
								left join PartyAffiliation af on p.Id = af.Person_Id and e.Election_Id = af.Election_Id
								left join Party pa on af.Party_Id = pa.Id
)

select o.*, o.PercentVotes - o2.PercentVotes as Previous
from Overview o left join Overview o2 on o.Election_Id - 1 = o2.Election_Id and
										 o.Wahlkreis_Id = o2.Wahlkreis_Id and
									     o.Person_Id = o2.Person_Id