/* Add electable candidate info from 2013 */

insert into IsElectableCandidate
select 3, person_id, wahlkreis_id
from IsElectableCandidate
where election_id = 2


/* Add electable party info from 2013 */

insert into CandidateList
select person_id, 3, bundesland_id, position
from CandidateList
where election_id = 2

/* Add party affiliation from 2013 */

insert into PartyAffiliation
select person_id, 3, party_id
from PartyAffiliation
where Election_Id = 2